using System;
using System.Reflection;
using CommonServiceLocator;
using Microsoft.Extensions.Configuration;
using Ninject;
using Ninject.Modules;
using Purchasing.Jobs.Common.Logging;
using Serilog;

namespace Purchasing.Jobs.Common
{
    public abstract class WebJobBase
    {
        protected static IKernel ConfigureServices()
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
            var configuration = builder.Build();

            LogHelper.ConfigureLogging(configuration);

            Log.Information("Build Number: {0}", Assembly.GetEntryAssembly().GetName().Version);

            // register services
            var kernel = new StandardKernel(new INinjectModule[] {new ServiceModule(configuration)});
            //kernel.Components.Add<IInjectionHeuristic, PropertySetterInjectionHeuristic>();
            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));

            return kernel;
        }
    }
}
