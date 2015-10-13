using System.Web;
using Serilog;
using SerilogWeb.Classic.Enrichers;

namespace Purchasing.Mvc
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
        public static void ConfigureLogging()
        {
            if (_loggingSetup) return; //only setup logging once

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Stackify()
                .Enrich.With<HttpSessionIdEnricher>()
                .Enrich.With<UserNameEnricher>()
                .Enrich.FromLogContext()
                .Filter.ByExcluding(e=>e.Exception.GetType() == typeof(HttpException)) //filter out those 404s and headers exceptions
                .CreateLogger();

            _loggingSetup = true;
        }
    }
}