using System;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.Services;
using Quartz;

namespace Purchasing.Web.App_Start.Jobs
{
    /// <summary>
    /// Create lookup indexes, including buildings, commodities, accounts, and vendors
    /// </summary>
    public class CreateLookupIndexsJob : IJob
    {
        private readonly IIndexService _indexService;

        public CreateLookupIndexsJob()
        {
            _indexService = ServiceLocator.Current.GetInstance<IIndexService>();
        }

        public void Execute(IJobExecutionContext context)
        {
            var indexRoot = context.MergedJobDataMap["indexRoot"] as string;
            _indexService.SetIndexRoot(indexRoot);

            try
            {
                _indexService.CreateAccountsIndex();
                _indexService.CreateBuildingsIndex();
                _indexService.CreateCommoditiesIndex();
                _indexService.CreateVendorsIndex();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
    }
}