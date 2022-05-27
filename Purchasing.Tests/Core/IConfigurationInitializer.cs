using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Extensions.Configuration;

namespace Purchasing.Tests.Core
{
    public static class IConfigurationInitializer
    {
        public static void Init(IWindsorContainer container, Action<IDictionary<string, string>> extendSettings = null)
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"MainDB:Schema", "main"},
                {"MainDB:IsSqlite", "true"},
                {"MainDB:BatchSize", "25"},
                {"ConnectionStrings:MainDB", "Data Source=:memory:;"},
            };
            extendSettings?.Invoke(inMemorySettings);
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            container.Register(Component.For<IConfiguration>().Instance(configuration));
        }
    }
}
