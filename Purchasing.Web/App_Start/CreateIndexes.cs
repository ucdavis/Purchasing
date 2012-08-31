using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.Services;
[assembly: WebActivator.PostApplicationStartMethod(typeof(Purchasing.Web.App_Start.CreateIndexes), "CreateHistoricalOrderIndex")]
namespace Purchasing.Web.App_Start
{
    public static class CreateIndexes
    {
        private static void CreateHistoricalOrderIndex()
        {
            var indexService = ServiceLocator.Current.GetInstance<IIndexService>();

            indexService.CreateHistoricalOrderIndex();
        }
    }

    
}