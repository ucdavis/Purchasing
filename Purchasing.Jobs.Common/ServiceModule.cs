using Ninject.Modules;
using Purchasing.Core.Services;

namespace Purchasing.Jobs.Common
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDbService>().To<DbService>().Named("dbService");
            Bind<IIndexService>().To<ElasticSearchIndexService>().Named("indexService");
        }
    }
}
