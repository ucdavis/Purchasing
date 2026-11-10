using Microsoft.Extensions.Configuration;

namespace Purchasing.Core.Models.Configuration
{
    public sealed class TelemetryOptions
    {
        [ConfigurationKeyName("ASPNETCORE_ENVIRONMENT")]
        public string EnvironmentName { get; set; }

        [ConfigurationKeyName("OTEL_SERVICE_NAME")]
        public string ServiceName { get; set; }

        [ConfigurationKeyName("OTEL_RESOURCE_ATTRIBUTES")]
        public string ResourceAttributes { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_ENDPOINT")]
        public string Endpoint { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_PROTOCOL")]
        public string Protocol { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_HEADERS")]
        public string Headers { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT")]
        public string LogsEndpoint { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_LOGS_PROTOCOL")]
        public string LogsProtocol { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_LOGS_HEADERS")]
        public string LogsHeaders { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT")]
        public string TracesEndpoint { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_TRACES_PROTOCOL")]
        public string TracesProtocol { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_TRACES_HEADERS")]
        public string TracesHeaders { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_METRICS_ENDPOINT")]
        public string MetricsEndpoint { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_METRICS_PROTOCOL")]
        public string MetricsProtocol { get; set; }

        [ConfigurationKeyName("OTEL_EXPORTER_OTLP_METRICS_HEADERS")]
        public string MetricsHeaders { get; set; }
    }
}
