using System;
using System.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using UCDArch.Core;
using Microsoft.Extensions.Configuration;
using Serilog.Exceptions;

namespace Purchasing.Jobs.Common.Logging
{
    public static class LogHelper
    {
        private static bool _loggingSetup = false;

        public static void ConfigureLogging(IConfiguration configuration)
        {
            if (_loggingSetup) return; //only setup logging once

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteToElasticSearchCustom(configuration)
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Application", configuration["Stackify.AppName"])
                .Enrich.WithProperty("AppEnvironment", configuration["Stackify.Environment"])
                .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException +=
                (sender, eventArgs) =>
                    Log.Error(eventArgs.ExceptionObject as Exception, eventArgs.ExceptionObject.ToString());

            AppDomain.CurrentDomain.ProcessExit += (_, _2) => Log.CloseAndFlush();

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
                IndexFormat = "aspnet-purchasing-{0:yyyy.MM.dd}"
            });
        }

    }
}
