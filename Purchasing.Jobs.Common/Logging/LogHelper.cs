using Serilog;
using Purchasing.Core.Telemetry;
using Microsoft.Extensions.Configuration;
using Serilog.Exceptions;

namespace Purchasing.Jobs.Common.Logging
{
    public static class LogHelper
    {
        private static bool _loggingSetup = false;

        public static void ConfigureLogging(IConfiguration configuration, string serviceName)
        {
            if (_loggingSetup) return; //only setup logging once

            Log.Logger = CreateLogger(configuration, serviceName);

            _loggingSetup = true;
        }

        public static Serilog.Core.Logger CreateLogger(IConfiguration configuration, string serviceName) =>
            new LoggerConfiguration()
                .WriteTo.Console()
                .WriteToOpenTelemetry(configuration, serviceName)
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .CreateLogger();

    }
}
