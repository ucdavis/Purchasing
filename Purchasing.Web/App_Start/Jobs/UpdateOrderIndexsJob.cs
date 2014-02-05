using System;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.Services;
using Quartz;

namespace Purchasing.Web.App_Start.Jobs
{
    /// <summary>
    /// update the order indexes based on the last time the job ran. Includes HistoricalOrder, LineItems, Comments, and CustomAnswers
    /// </summary>
    public class UpdateOrderIndexsJob : IJob
    {
        private readonly IIndexService _indexService;

        public UpdateOrderIndexsJob()
        {
            _indexService = ServiceLocator.Current.GetInstance<IIndexService>();
        }

        public void Execute(IJobExecutionContext context)
        {
            var indexRoot = context.MergedJobDataMap["indexRoot"] as string;
            _indexService.SetIndexRoot(indexRoot);

            try
            {
                _indexService.UpdateHistoricalOrderIndex();
                //TODO: this still recreates these other indexes from scratch
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