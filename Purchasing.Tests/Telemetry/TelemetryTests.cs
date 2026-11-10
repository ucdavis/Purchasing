using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Purchasing.Core.Telemetry;
using Purchasing.Jobs.Common.Logging;
using Purchasing.Mvc.Logging;
using Purchasing.Tests.Otlp;
using Serilog;

namespace Purchasing.Tests.Telemetry
{
    [TestClass]
    [DoNotParallelize]
    public class TelemetryTests
    {
        [TestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        public async Task WebAndJobLoggersExportCorrelatedStructuredEventsAndFlush(bool web, bool grpc)
        {
            await using var receiver = await OtlpReceiver.Start(grpc);
            var configuration = Configuration(receiver.Endpoint, grpc);
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddPurchasingTelemetry(configuration, "fallback", web);
            using var provider = services.BuildServiceProvider();
            var traces = provider.GetRequiredService<TracerProvider>();
            var metrics = provider.GetRequiredService<MeterProvider>();
            var accessor = new HttpContextAccessor();
            accessor.HttpContext = new DefaultHttpContext();
            accessor.HttpContext.TraceIdentifier = "http-request-123";
            accessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "telemetry-user") }, "test"));
            accessor.HttpContext.Request.Headers.UserAgent = "telemetry-test";
            accessor.HttpContext.Connection.RemoteIpAddress = IPAddress.Loopback;
            string traceId;
            string spanId;
            try
            {
                using (var activity = TelemetryConfiguration.JobActivities.StartActivity("export-test"))
                {
                    Assert.IsNotNull(activity);
                    traceId = activity.TraceId.ToHexString();
                    spanId = activity.SpanId.ToHexString();
                    using (var logger = web ? LogConfig.CreateLogger(configuration) : LogHelper.CreateLogger(configuration, "fallback"))
                    {
                        logger.Error(new InvalidOperationException("synthetic exception"), "Exported order {OrderId}", 123);
                        // Exercise real HTTP instrumentation and metric collection.
                        using var client = new HttpClient();
                        await client.GetAsync(receiver.Endpoint + "/health");
                    }
                }
                Assert.IsTrue(traces.ForceFlush(10000));
                Assert.IsTrue(metrics.ForceFlush(10000));
            }
            finally
            {
                accessor.HttpContext = null;
            }

            var requests = receiver.Requests.ToArray();
            var logs = requests.Where(r => r.Signal == "logs").Select(r => ExportLogsRequest.Parser.ParseFrom(r.Body))
                .SelectMany(r => r.ResourceLogs).ToArray();
            var record = logs.SelectMany(r => r.ScopeLogs).SelectMany(r => r.LogRecords).Single();
            Assert.AreEqual("Exported order 123", record.Body.StringValue);
            Assert.AreEqual(traceId, Convert.ToHexString(record.TraceId.ToByteArray()).ToLowerInvariant());
            Assert.AreEqual(spanId, Convert.ToHexString(record.SpanId.ToByteArray()).ToLowerInvariant());
            Assert.AreEqual(123L, record.Attributes.Single(a => a.Key == "OrderId").Value.IntValue);
            Assert.AreEqual("purchasing.telemetry-test", record.Attributes.Single(a => a.Key == "Application").Value.StringValue);
            Assert.AreEqual("Test", record.Attributes.Single(a => a.Key == "AppEnvironment").Value.StringValue);
            Assert.IsTrue(record.Attributes.Any(a => a.Key == "exception.message" && a.Value.StringValue == "synthetic exception"));
            Assert.IsTrue(record.Attributes.Any(a => a.Key == "ExceptionDetail"));
            if (web)
            {
                Assert.AreEqual("http-request-123", record.Attributes.Single(a => a.Key == "RequestId").Value.StringValue);
                Assert.AreEqual("telemetry-user", record.Attributes.Single(a => a.Key == "User").Value.StringValue);
                Assert.IsFalse(record.Attributes.Any(a => a.Key == "TraceId"));
            }
            var spans = requests.Where(r => r.Signal == "traces").Select(r => ExportTracesRequest.Parser.ParseFrom(r.Body))
                .SelectMany(r => r.ResourceSpans).ToArray();
            var allSpans = spans.SelectMany(r => r.ScopeSpans).SelectMany(r => r.Spans).ToArray();
            Assert.AreEqual(1, allSpans.Count(s => s.Name == "export-test"));
            Assert.AreEqual(1, allSpans.Count(s => s.Kind == 3), "Only the application HTTP call should be traced, never the exporters.");
            Assert.IsTrue(requests.Where(r => r.Signal == "metrics").Select(r => ExportMetricsRequest.Parser.ParseFrom(r.Body))
                .SelectMany(r => r.ResourceMetrics).SelectMany(r => r.ScopeMetrics).SelectMany(r => r.Metrics)
                .Any(m => m.Name == "http.client.request.duration"));
            foreach (var resource in logs.Select(r => r.Resource).Concat(spans.Select(r => r.Resource)))
                Assert.AreEqual("purchasing.telemetry-test", resource.Attributes.Single(a => a.Key == "service.name").Value.StringValue);
            Assert.IsTrue(requests.All(r => r.Authorization == "ApiKey synthetic-test-key"));
        }

        [TestMethod]
        public async Task SignalSpecificHttpEndpointAndHeadersOverrideSharedConfiguration()
        {
            await using var receiver = await OtlpReceiver.Start(false);
            var configuration = Configuration("http://127.0.0.1:1", false, new Dictionary<string, string>
            {
                ["OTEL_EXPORTER_OTLP_LOGS_ENDPOINT"] = receiver.Endpoint + "/custom/logs",
                ["OTEL_EXPORTER_OTLP_LOGS_HEADERS"] = "Authorization=ApiKey%20signal-key"
            });
            using (var logger = LogHelper.CreateLogger(configuration, "job"))
                logger.Information("Signal override");
            var request = receiver.Requests.Single();
            Assert.AreEqual("/custom/logs", request.Path);
            Assert.AreEqual("ApiKey signal-key", request.Authorization);
        }

        [TestMethod]
        public async Task BlobSdkOperationsKeepTheirOwnSpansAndParentCorrelation()
        {
            await using var receiver = await OtlpReceiver.Start(false);
            var configuration = Configuration(receiver.Endpoint, false);
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddPurchasingTelemetry(configuration, "job", web: false);
            using var provider = services.BuildServiceProvider();
            var traces = provider.GetRequiredService<TracerProvider>();
            string parent;
            using (var activity = TelemetryConfiguration.JobActivities.StartActivity("blob-job"))
            {
                Assert.IsNotNull(activity);
                parent = activity.SpanId.ToHexString();
                var blob = new BlobClient(new Uri(receiver.Endpoint + "/container/blob"));
                await blob.DownloadContentAsync();
            }
            Assert.IsTrue(traces.ForceFlush(10000));
            var spans = receiver.Requests.Where(r => r.Signal == "traces")
                .Select(r => ExportTracesRequest.Parser.ParseFrom(r.Body)).SelectMany(r => r.ResourceSpans)
                .SelectMany(r => r.ScopeSpans).SelectMany(r => r.Spans).ToArray();
            Assert.IsTrue(spans.Any(s => s.Name.StartsWith("Blob") &&
                Convert.ToHexString(s.ParentSpanId.ToByteArray()).ToLowerInvariant() == parent),
                string.Join(", ", spans.Select(s => s.Name)));
        }

        [TestMethod]
        [DataRow("https://user:secret@example.com")]
        [DataRow("file:///tmp/telemetry")]
        [DataRow("https://example.com?token=secret")]
        public void InvalidEndpointsFailWithoutExposingCredentials(string endpoint)
        {
            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => LogHelper.CreateLogger(Configuration(endpoint, false), "job"));
            Assert.IsFalse(exception.Message.Contains("secret"));
            Assert.IsFalse(exception.Message.Contains(endpoint));
        }

        [TestMethod]
        public void UnconfiguredExportKeepsStructuredConsoleLogging()
        {
            var output = new StringWriter();
            var original = Console.Out;
            Console.SetOut(output);
            try
            {
                using var logger = LogHelper.CreateLogger(new ConfigurationBuilder().Build(), "job");
                logger.Information("Console order {OrderId}", 456);
                StringAssert.Contains(output.ToString(), "Console order 456");
            }
            finally { Console.SetOut(original); }
        }

        [TestMethod]
        [DataRow(401)]
        [DataRow(403)]
        [DataRow(503)]
        public async Task ReceiverFailuresAreDiagnosableWithoutExposingSecrets(int status)
        {
            await using var receiver = await OtlpReceiver.Start(false, status);
            var output = new StringWriter();
            var original = Console.Error;
            Console.SetError(TextWriter.Synchronized(output));
            try
            {
                using var diagnostics = new TelemetryDiagnostics();
                using (var logger = LogHelper.CreateLogger(Configuration(receiver.Endpoint, false), "job"))
                    logger.Information("Failure test");
                Assert.IsTrue(receiver.Requests.Count > 0);
                StringAssert.Contains(output.ToString(), "Serilog telemetry export failed");
                Assert.IsFalse(output.ToString().Contains("synthetic-test-key"));
            }
            finally { Console.SetError(original); }
        }

        [TestMethod]
        public void UnavailableReceiverDoesNotPreventJobLoggerShutdown()
        {
            var output = new StringWriter();
            var original = Console.Error;
            Console.SetError(TextWriter.Synchronized(output));
            try
            {
                using var diagnostics = new TelemetryDiagnostics();
                using (var logger = LogHelper.CreateLogger(Configuration("http://127.0.0.1:1", false), "job"))
                    logger.Information("Unavailable receiver");
                StringAssert.Contains(output.ToString(), "Serilog telemetry export failed");
                Assert.IsFalse(output.ToString().Contains("synthetic-test-key"));
            }
            finally { Console.SetError(original); }
        }

        private static IConfiguration Configuration(string endpoint, bool grpc, Dictionary<string, string> overrides = null)
        {
            var settings = new Dictionary<string, string>
            {
                ["OTEL_EXPORTER_OTLP_ENDPOINT"] = endpoint,
                ["OTEL_EXPORTER_OTLP_PROTOCOL"] = grpc ? "grpc" : "http/protobuf",
                ["OTEL_EXPORTER_OTLP_HEADERS"] = "Authorization=ApiKey%20synthetic-test-key",
                ["OTEL_SERVICE_NAME"] = "purchasing.telemetry-test",
                ["OTEL_RESOURCE_ATTRIBUTES"] = "deployment.environment.name=Test"
            };
            if (overrides != null)
                foreach (var pair in overrides) settings[pair.Key] = pair.Value;
            return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        }

        private sealed record ReceivedRequest(string Signal, string Path, string Authorization, byte[] Body);

        private sealed class OtlpReceiver : IAsyncDisposable
        {
            private readonly WebApplication _app;
            public string Endpoint { get; private set; }
            public ConcurrentQueue<ReceivedRequest> Requests { get; } = new ConcurrentQueue<ReceivedRequest>();
            private OtlpReceiver(WebApplication app) { _app = app; }

            public static async Task<OtlpReceiver> Start(bool grpc, int status = 200)
            {
                var builder = WebApplication.CreateBuilder();
                builder.Logging.ClearProviders();
                builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, 0,
                    listen => listen.Protocols = grpc ? HttpProtocols.Http2 : HttpProtocols.Http1));
                var app = builder.Build();
                var receiver = new OtlpReceiver(app);
                app.Run(async context =>
                {
                    if (context.Request.Method == "GET") { context.Response.StatusCode = 200; return; }
                    using var body = new MemoryStream();
                    await context.Request.Body.CopyToAsync(body);
                    var path = context.Request.Path.Value;
                    var signal = path.Contains("Logs") || path.EndsWith("logs") ? "logs" :
                        path.Contains("Trace") || path.EndsWith("traces") ? "traces" : "metrics";
                    receiver.Requests.Enqueue(new ReceivedRequest(signal, path, context.Request.Headers.Authorization,
                        grpc ? body.ToArray().Skip(5).ToArray() : body.ToArray()));
                    context.Response.StatusCode = status;
                    context.Response.ContentType = grpc ? "application/grpc" : "application/x-protobuf";
                    if (grpc)
                    {
                        context.Response.DeclareTrailer("grpc-status");
                        await context.Response.Body.WriteAsync(new byte[5]);
                        context.Response.AppendTrailer("grpc-status", "0");
                    }
                });
                await app.StartAsync();
                receiver.Endpoint = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>().Addresses.Single();
                return receiver;
            }

            public async ValueTask DisposeAsync() { await _app.StopAsync(); await _app.DisposeAsync(); }
        }
    }
}
