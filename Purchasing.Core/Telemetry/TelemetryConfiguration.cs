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
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Purchasing.Core.Telemetry
{
    public static class TelemetryConfiguration
    {
        public const string JobActivitySourceName = "Purchasing.Jobs";
        public static readonly ActivitySource JobActivities = new ActivitySource(JobActivitySourceName);

        public static IDictionary<string, object> ResourceAttributes(IConfiguration configuration, string serviceName)
        {
            var attributes = new Dictionary<string, object>
            {
                ["service.name"] = serviceName,
                ["service.namespace"] = "purchasing",
                ["deployment.environment.name"] = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production"
            };
            foreach (var pair in ParsePairs(configuration["OTEL_RESOURCE_ATTRIBUTES"]))
                attributes[pair.Key] = pair.Value;
            if (!string.IsNullOrWhiteSpace(configuration["OTEL_SERVICE_NAME"]))
                attributes["service.name"] = configuration["OTEL_SERVICE_NAME"];
            return attributes;
        }

        public static LoggerConfiguration WriteToOpenTelemetry(this LoggerConfiguration logger,
            IConfiguration configuration, string serviceName)
        {
            var attributes = ResourceAttributes(configuration, serviceName);
            logger.Enrich.WithProperty("Application", attributes["service.name"])
                .Enrich.WithProperty("AppEnvironment", attributes["deployment.environment.name"]);

            var endpoint = Endpoint(configuration, "LOGS");
            if (endpoint == null)
                return logger;

            return logger.WriteTo.OpenTelemetry(options =>
            {
                // Only Serilog exports logs. The OpenTelemetry SDK owns traces and metrics.
                options.Endpoint = null;
                options.LogsEndpoint = endpoint.ToString();
                options.Protocol = Protocol(configuration, "LOGS") == OtlpExportProtocol.Grpc
                    ? OtlpProtocol.Grpc : OtlpProtocol.HttpProtobuf;
                options.Headers = ParsePairs(Setting(configuration, "LOGS", "HEADERS"));
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
            var attributes = ResourceAttributes(configuration, serviceName);
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddAttributes(attributes))
                .WithTracing(tracing =>
                {
                    tracing.AddSource(JobActivitySourceName, "Azure.Storage.Blobs.*")
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation();
                    if (web)
                        tracing.AddAspNetCoreInstrumentation();
                    if (Endpoint(configuration, "TRACES") != null)
                        tracing.AddOtlpExporter(options => ConfigureExporter(options, configuration, "TRACES"));
                })
                .WithMetrics(metrics =>
                {
                    metrics.AddHttpClientInstrumentation().AddSqlClientInstrumentation();
                    if (web)
                        metrics.AddAspNetCoreInstrumentation();
                    if (Endpoint(configuration, "METRICS") != null)
                        metrics.AddOtlpExporter(options => ConfigureExporter(options, configuration, "METRICS"));
                });
        }

        private static void ConfigureExporter(OtlpExporterOptions options, IConfiguration configuration, string signal)
        {
            options.Protocol = Protocol(configuration, signal);
            options.Endpoint = Endpoint(configuration, signal);
            options.Headers = Setting(configuration, signal, "HEADERS");
        }

        private static string Setting(IConfiguration configuration, string signal, string name) =>
            configuration[$"OTEL_EXPORTER_OTLP_{signal}_{name}"] ?? configuration[$"OTEL_EXPORTER_OTLP_{name}"];

        private static OtlpExportProtocol Protocol(IConfiguration configuration, string signal)
        {
            return Setting(configuration, signal, "PROTOCOL") switch
            {
                null or "" or "grpc" => OtlpExportProtocol.Grpc,
                "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
                _ => throw new InvalidOperationException("OTLP protocol must be grpc or http/protobuf.")
            };
        }

        private static Uri Endpoint(IConfiguration configuration, string signal)
        {
            var specific = configuration[$"OTEL_EXPORTER_OTLP_{signal}_ENDPOINT"];
            var value = specific ?? configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "https" && uri.Scheme != "http") ||
                !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
                throw new InvalidOperationException("OTLP endpoint must be an HTTP(S) URL without credentials, query, or fragment. Use OTLP headers for authentication.");
            if (specific == null && Protocol(configuration, signal) == OtlpExportProtocol.HttpProtobuf)
                return new Uri(value.TrimEnd('/') + "/v1/" + signal.ToLowerInvariant());
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
