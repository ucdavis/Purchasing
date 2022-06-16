using System;
using System.Configuration;
using System.Web;
using Serilog;
using Serilog.Configuration;
using Serilog.Sinks.Elasticsearch;
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
                .WriteToElasticSearchCustom()
                .Enrich.With<HttpSessionIdEnricher>()
                .Enrich.With<UserNameEnricher>()
                .Enrich.FromLogContext()
                .Filter.ByExcluding(e => e.Exception != null && e.Exception.GetBaseException() is HttpException) //filter out those 404s and headers exceptions
                .CreateLogger();

            _loggingSetup = true;
        }

        public static LoggerConfiguration WriteToElasticSearchCustom(this LoggerConfiguration logConfig)
        {
            var esUrl = ConfigurationManager.AppSettings["Stackify.ElasticUrl"];

            // only continue if a valid http url is setup in the config
            if (esUrl == null || !esUrl.StartsWith("http"))
            {
                return logConfig;
            }

            logConfig.Enrich.WithProperty("Application", ConfigurationManager.AppSettings["Stackify.AppName"]);
            logConfig.Enrich.WithProperty("AppEnvironment", ConfigurationManager.AppSettings["Stackify.Environment"]);
            logConfig.Enrich.WithClientIp();
            logConfig.Enrich.WithClientAgent();

            return logConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esUrl))
            {
                IndexFormat = "aspnet-purchasing-{0:yyyy.MM}"
            });
        }
    }
}