using System;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Ninject.Modules;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Purchasing.Core.Telemetry;
using Purchasing.Jobs.Common.Logging;
using Serilog;

namespace Purchasing.Jobs.Common
{
    public abstract class WebJobBase
    {
        public static IConfigurationRoot configuration;

        protected static void Run(Action<IKernel> execute) =>
            RunAsync(kernel =>
            {
                execute(kernel);
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        protected static async Task RunAsync(Func<IKernel, Task> execute)
        {
            using var diagnostics = new TelemetryDiagnostics();
            ServiceProvider telemetry = null;
            Activity activity = null;
            try
            {
                configuration = BuildConfiguration();
                var jobName = Assembly.GetEntryAssembly().GetName().Name;
                LogHelper.ConfigureLogging(configuration, jobName);
                var services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(configuration);
                services.AddPurchasingTelemetry(configuration, jobName, web: false);
                telemetry = services.BuildServiceProvider();
                // Console jobs have no hosted-service lifecycle to start the providers for them.
                telemetry.GetRequiredService<TracerProvider>();
                telemetry.GetRequiredService<MeterProvider>();
                activity = TelemetryConfiguration.JobActivities.StartActivity(jobName);
                activity?.SetTag("job.name", jobName);

                Log.Information("Build Number: {0}", Assembly.GetEntryAssembly().GetName().Version);
                using var kernel = new StandardKernel(new INinjectModule[] { new ServiceModule(configuration) });
                ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));
                await execute(kernel);
            }
            catch (Exception exception)
            {
                activity?.SetStatus(ActivityStatusCode.Error, exception.GetType().Name);
                Log.Fatal(exception, "Job terminated unexpectedly");
                throw;
            }
            finally
            {
                activity?.Dispose();
                // Finish the root span before disposing providers, which flush their pending data.
                telemetry?.Dispose();
                Log.CloseAndFlush();
            }
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "development", StringComparison.OrdinalIgnoreCase);
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            //only add secrets in development
            if (isDevelopment)
            {
                builder.AddUserSecrets<WebJobBase>();
            }
            return builder.Build();
        }
    }
}
