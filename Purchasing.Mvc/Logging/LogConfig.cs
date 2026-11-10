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
        public static Serilog.Core.Logger CreateLogger(IConfiguration configuration) =>
            ConfigureLogging(new LoggerConfiguration(), configuration).CreateLogger();

        public static LoggerConfiguration ConfigureLogging(LoggerConfiguration logger, IConfiguration configuration) =>
            logger
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
                .Enrich.FromLogContext();

    }
}
