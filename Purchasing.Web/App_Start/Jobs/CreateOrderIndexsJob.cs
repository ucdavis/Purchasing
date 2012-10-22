using System;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.Services;
using Quartz;

namespace Purchasing.Web.App_Start.Jobs
{
    /// <summary>
    /// Create the order indexes, which include HistoricalOrder, LineItems, Comments, and CustomAnswers
    /// </summary>
    public class CreateOrderIndexsJob : IJob
    {
        private readonly IIndexService _indexService;

        public CreateOrderIndexsJob()
        {
            _indexService = ServiceLocator.Current.GetInstance<IIndexService>();
        }

        public void Execute(IJobExecutionContext context)
        {
            var indexRoot = context.MergedJobDataMap["indexRoot"] as string;
            _indexService.SetIndexRoot(indexRoot);

            try
            {
                _indexService.CreateHistoricalOrderIndex();
                _indexService.CreateLineItemsIndex();
                _indexService.CreateCommentsIndex();
                _indexService.CreateCustomAnswersIndex();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
    }
}