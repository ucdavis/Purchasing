using System;
using System.Diagnostics.Tracing;
using Serilog.Debugging;

namespace Purchasing.Core.Telemetry
{
    /// <summary>Reports exporter failures to stderr without echoing endpoints, headers, or payloads.</summary>
    public sealed class TelemetryDiagnostics : EventListener
    {
        public TelemetryDiagnostics()
        {
            SelfLog.Enable(_ => Console.Error.WriteLine("Serilog telemetry export failed. Check OTLP connectivity, authentication, and receiver health."));
        }

        public override void Dispose()
        {
            SelfLog.Disable();
            base.Dispose();
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name.StartsWith("OpenTelemetry-", StringComparison.Ordinal))
                EnableEvents(eventSource, EventLevel.Warning);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // The SDK reports unselected instruments and the deliberately absent OTel logger
            // provider as warnings. Neither indicates a delivery failure.
            if (eventData.EventName == "MetricInstrumentIgnored" || eventData.EventName == "LoggerProviderNotRegistered")
                return;
            Console.Error.WriteLine($"OpenTelemetry exporter diagnostic: {eventData.EventSource.Name}/{eventData.EventName}. Check OTLP configuration and receiver health.");
        }
    }
}
