using System;
using FluentNHibernate.Cfg.Db;
using UCDArch.Core;
using Microsoft.Extensions.Configuration;

namespace UCDArch.Data.NHibernate.Mapping
{
    public static class PersistenceConfiguration
    {
        public static IPersistenceConfigurer GetConfigurer()
        {
            var configuration = SmartServiceLocator<IConfiguration>.GetService();
            return configuration.GetValue<bool>("MainDB:IsSqlite", false)
                ? SQLiteConfiguration.Standard
                    .DefaultSchema(configuration["MainDB:Schema"])
                    .ConnectionString(configuration["ConnectionStrings:MainDB"])
                    .AdoNetBatchSize(configuration.GetValue<int>("MainDB:BatchSize", 25))
                : MsSqlConfiguration.MsSql2008
                    .DefaultSchema(configuration["MainDB:Schema"])
                    .ConnectionString(configuration["ConnectionStrings:MainDB"])
                    .AdoNetBatchSize(configuration.GetValue<int>("MainDB:BatchSize", 25));
        }
    }
}
