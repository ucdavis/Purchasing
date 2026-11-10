using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Purchasing.Core.Models.Configuration;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Purchasing.Core.Telemetry
{
    public static class TelemetryConfiguration
    {
        public const string JobActivitySourceName = "Purchasing.Jobs";
        public static readonly ActivitySource JobActivities = new ActivitySource(JobActivitySourceName);

        public static IDictionary<string, object> ResourceAttributes(IConfiguration configuration, string serviceName) =>
            ResourceAttributes(configuration.Get<TelemetryOptions>() ?? new TelemetryOptions(), serviceName);

        private static IDictionary<string, object> ResourceAttributes(TelemetryOptions options, string serviceName)
        {
            var attributes = new Dictionary<string, object>
            {
                ["service.name"] = serviceName,
                ["service.namespace"] = "purchasing",
                ["deployment.environment.name"] = options.EnvironmentName ?? "Production"
            };
            foreach (var pair in ParsePairs(options.ResourceAttributes))
                attributes[pair.Key] = pair.Value;
            if (!string.IsNullOrWhiteSpace(options.ServiceName))
                attributes["service.name"] = options.ServiceName;
            return attributes;
        }

        public static LoggerConfiguration WriteToOpenTelemetry(this LoggerConfiguration logger,
            IConfiguration configuration, string serviceName)
        {
            var telemetry = configuration.Get<TelemetryOptions>() ?? new TelemetryOptions();
            var attributes = ResourceAttributes(telemetry, serviceName);
            logger.Enrich.WithProperty("Application", attributes["service.name"])
                .Enrich.WithProperty("AppEnvironment", attributes["deployment.environment.name"]);

            var logs = ResolveSignal(telemetry, TelemetrySignal.Logs);
            var endpoint = Endpoint(logs);
            if (endpoint == null)
                return logger;

            return logger.WriteTo.OpenTelemetry(options =>
            {
                // Only Serilog exports logs. The OpenTelemetry SDK owns traces and metrics.
                options.Endpoint = null;
                options.LogsEndpoint = endpoint.ToString();
                options.Protocol = Protocol(logs.Protocol) == OtlpExportProtocol.Grpc
                    ? OtlpProtocol.Grpc : OtlpProtocol.HttpProtobuf;
                options.Headers = ParsePairs(logs.Headers);
                options.ResourceAttributes = attributes;
                options.OnBeginSuppressInstrumentation = SuppressInstrumentationScope.Begin;
                options.BatchingOptions.BufferingTimeLimit = TimeSpan.FromSeconds(2);
            }, ignoreEnvironment: true);
        }

        public static void AddPurchasingTelemetry(this IServiceCollection services,
            IConfiguration configuration, string serviceName, bool web)
        {
            // Azure SDK releases that still gate ActivitySource support require this switch.
            AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
            var telemetry = configuration.Get<TelemetryOptions>() ?? new TelemetryOptions();
            var attributes = ResourceAttributes(telemetry, serviceName);
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddAttributes(attributes))
                .WithTracing(tracing =>
                {
                    tracing.AddSource(JobActivitySourceName, "Azure.Storage.Blobs.*")
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation();
                    if (web)
                        tracing.AddAspNetCoreInstrumentation();
                    var traces = ResolveSignal(telemetry, TelemetrySignal.Traces);
                    var endpoint = Endpoint(traces);
                    if (endpoint != null)
                        tracing.AddOtlpExporter(options => ConfigureExporter(options, traces, endpoint));
                })
                .WithMetrics(metrics =>
                {
                    metrics.AddHttpClientInstrumentation().AddSqlClientInstrumentation();
                    if (web)
                        metrics.AddAspNetCoreInstrumentation();
                    var metricOptions = ResolveSignal(telemetry, TelemetrySignal.Metrics);
                    var endpoint = Endpoint(metricOptions);
                    if (endpoint != null)
                        metrics.AddOtlpExporter(options => ConfigureExporter(options, metricOptions, endpoint));
                });
        }

        private static void ConfigureExporter(OtlpExporterOptions options, SignalOptions signal, Uri endpoint)
        {
            options.Protocol = Protocol(signal.Protocol);
            options.Endpoint = endpoint;
            options.Headers = signal.Headers;
        }

        private enum TelemetrySignal { Logs, Traces, Metrics }

        private sealed record SignalOptions(TelemetrySignal Signal, string Endpoint, string Protocol,
            string Headers, bool HasSpecificEndpoint);

        private static SignalOptions ResolveSignal(TelemetryOptions options, TelemetrySignal signal)
        {
            var (endpoint, protocol, headers) = signal switch
            {
                TelemetrySignal.Logs => (options.LogsEndpoint, options.LogsProtocol, options.LogsHeaders),
                TelemetrySignal.Traces => (options.TracesEndpoint, options.TracesProtocol, options.TracesHeaders),
                TelemetrySignal.Metrics => (options.MetricsEndpoint, options.MetricsProtocol, options.MetricsHeaders),
                _ => throw new ArgumentOutOfRangeException(nameof(signal))
            };
            return new SignalOptions(signal, endpoint ?? options.Endpoint, protocol ?? options.Protocol,
                headers ?? options.Headers, endpoint != null);
        }

        private static OtlpExportProtocol Protocol(string protocol)
        {
            return protocol switch
            {
                null or "" or "grpc" => OtlpExportProtocol.Grpc,
                "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
                _ => throw new InvalidOperationException("OTLP protocol must be grpc or http/protobuf.")
            };
        }

        private static Uri Endpoint(SignalOptions signal)
        {
            var value = signal.Endpoint;
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "https" && uri.Scheme != "http") ||
                !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
                throw new InvalidOperationException("OTLP endpoint must be an HTTP(S) URL without credentials, query, or fragment. Use OTLP headers for authentication.");
            if (!signal.HasSpecificEndpoint && Protocol(signal.Protocol) == OtlpExportProtocol.HttpProtobuf)
                return new Uri(value.TrimEnd('/') + "/v1/" + signal.Signal.ToString().ToLowerInvariant());
            return uri;
        }

        private static Dictionary<string, string> ParsePairs(string value)
        {
            var pairs = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(value))
                return pairs;
            foreach (var entry in value.Split(','))
            {
                var separator = entry.IndexOf('=');
                if (separator <= 0)
                    throw new InvalidOperationException("OTEL attributes and headers must be comma-separated key=value pairs.");
                var key = entry.Substring(0, separator).Trim();
                var item = Uri.UnescapeDataString(entry.Substring(separator + 1).Trim());
                if (key.Any(char.IsControl) || item.Any(char.IsControl))
                    throw new InvalidOperationException("OTEL attributes and headers cannot contain control characters.");
                pairs[key] = item;
            }
            return pairs;
        }
    }
}
