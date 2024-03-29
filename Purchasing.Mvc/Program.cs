﻿// This application entry point is based on ASP.NET Core new project templates and is included
// as a starting point for app host configuration.
// This file may need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core hosting, see https://docs.microsoft.com/aspnet/core/fundamentals/host/web-host

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Purchasing.Mvc.Logging;
using Elastic.Apm.NetCoreAll;

namespace Purchasing.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "development", StringComparison.OrdinalIgnoreCase);
            var isProductionDebug = !isDevelopment && string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_DEBUG_PRODUCTION"), "true", StringComparison.OrdinalIgnoreCase);
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            //only add secrets in development or when debugging in production
            if (isDevelopment || isProductionDebug)
            {
                builder.AddUserSecrets<Program>();
            }
            var configuration = builder.Build();

            LogConfig.ConfigureLogging(configuration);

            try
            {
                Log.Information("Building web host");
                var host = CreateHostBuilder(args, isProductionDebug && !isDevelopment).Build();

                Log.Information("Starting web host");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, bool addUserSecrets) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseWindsorContainerServiceProvider()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    if (addUserSecrets)
                    {
                        // override builder's default behavior of not adding secrets in prod environment
                        webBuilder.ConfigureAppConfiguration(builder => builder.AddUserSecrets<Program>());
                    }
                })
                .UseAllElasticApm();
    }
}
