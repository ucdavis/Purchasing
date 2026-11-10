using Serilog;
using Purchasing.Core.Telemetry;
using Serilog.Exceptions;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace Purchasing.Mvc.Logging
{
    /// <summary>
    /// Configure Application Logging
    /// </summary>
    public static class LogConfig
    {
        private static bool _loggingSetup;

        /// <summary>
        /// Configure Application Logging
        /// </summary>
        public static void ConfigureLogging(IConfiguration configuration)
        {
            if (_loggingSetup) return; //only setup logging once

            Log.Logger = CreateLogger(configuration);

            _loggingSetup = true;
        }

        public static Serilog.Core.Logger CreateLogger(IConfiguration configuration) =>
            new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                // .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning) // uncomment this to hide EF core general info logs
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteToOpenTelemetry(configuration, "purchasing.web")
                .Enrich.WithClientIp()
                .Enrich.WithRequestHeader("User-Agent", "ClientAgent")
                .Enrich.WithExceptionDetails()
                .Enrich.With<SerilogHttpContextEnricher>()
                .Enrich.FromLogContext()
                //.Filter.ByExcluding(e => e.Exception != null && e.Exception.GetBaseException() is HttpException) //filter out those 404s and headers exceptions
                .CreateLogger();

    }
}
