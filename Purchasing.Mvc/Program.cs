// This application entry point is based on ASP.NET Core new project templates and is included
// as a starting point for app host configuration.
// This file may need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core hosting, see https://docs.microsoft.com/aspnet/core/fundamentals/host/web-host

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Purchasing.Mvc.Logging;
using Purchasing.Core.Telemetry;
using Castle.Windsor.MsDependencyInjection;

namespace Purchasing.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "development", StringComparison.OrdinalIgnoreCase);
            var isProductionDebug = !isDevelopment && string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_DEBUG_PRODUCTION"), "true", StringComparison.OrdinalIgnoreCase);

            using var diagnostics = new TelemetryDiagnostics();
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
            IHost host = null;

            try
            {
                Log.Information("Building web host");
                host = CreateHostBuilder(args, isProductionDebug).Build();

                Log.Information("Starting web host");
                host.Start();
                host.WaitForShutdown();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                Environment.ExitCode = 1;
            }
            finally
            {
                host?.Dispose();
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, bool addUserSecrets) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, logger) => LogConfig.ConfigureLogging(logger, context.Configuration))
                .UseServiceProviderFactory(new WindsorServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    if (addUserSecrets)
                    {
                        // override builder's default behavior of not adding secrets in prod environment
                        webBuilder.ConfigureAppConfiguration(builder => builder.AddUserSecrets<Program>());
                    }
                })
                .ConfigureServices((context, services) =>
                    services.AddPurchasingTelemetry(context.Configuration, "purchasing.web", web: true));
    }
}
