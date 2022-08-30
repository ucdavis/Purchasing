using System;
using System.Web;
using Serilog;
using Serilog.Configuration;
using Serilog.Sinks.Elasticsearch;
using Serilog.Exceptions;
using Microsoft.Extensions.Configuration;
using Elastic.Apm.SerilogEnricher;

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

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteToElasticSearchCustom(configuration)
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Application", configuration["Stackify.AppName"])
                .Enrich.WithProperty("AppEnvironment", configuration["Stackify.Environment"])
                .Enrich.WithElasticApmCorrelationInfo()
                .Enrich.With<SerilogHttpContextEnricher>()
                .Enrich.FromLogContext()
                //.Filter.ByExcluding(e => e.Exception != null && e.Exception.GetBaseException() is HttpException) //filter out those 404s and headers exceptions
                .CreateLogger();

            _loggingSetup = true;
        }

        public static LoggerConfiguration WriteToElasticSearchCustom(this LoggerConfiguration logConfig, IConfiguration configuration)
        {
            var esUrl = configuration["Stackify.ElasticUrl"];

            // only continue if a valid http url is setup in the config
            if (esUrl == null || !esUrl.StartsWith("http"))
            {
                return logConfig;
            }

            logConfig.Enrich.WithProperty("Application", configuration["Stackify.AppName"]);
            logConfig.Enrich.WithProperty("AppEnvironment", configuration["Stackify.Environment"]);


            return logConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esUrl))
            {
                IndexFormat = "aspnet-purchasing-{0:yyyy.MM}"
            });
        }
    }
}