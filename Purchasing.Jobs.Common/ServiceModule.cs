using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Purchasing.Core.Services;

namespace Purchasing.Jobs.Common
{
    public class ServiceModule : NinjectModule
    {
        private readonly IConfiguration _configuration;

        public ServiceModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void Load()
        {
            Bind<IDbService>().To<DbService>().Named("dbService");
            // Indexing jobs resolve this at startup. Unrelated email jobs do not need search configuration.
            Bind<SearchBackendSet>().ToMethod(_ => new SearchBackendSet(_configuration)).InSingletonScope();
            Bind<IIndexService>().To<ElasticSearchIndexService>().Named("indexService");
            Bind<IConfiguration>().ToConstant(_configuration);
        }
    }
}
