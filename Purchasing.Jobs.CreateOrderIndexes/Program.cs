using System;
using Ninject;
using Purchasing.Core.Helpers;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Purchasing.Jobs.Common.Logging;
using Serilog;

namespace Purchasing.Jobs.CreateOrderIndexes
{
    class Program : WebJobBase
    {
        static void Main(string[] args)
        {
            var kernel = ConfigureServices();

            var indexService = kernel.Get<IIndexService>();

            try
            {
                Log.Information("Creating Order Indexes");

                indexService.CreateHistoricalOrderIndex();
                indexService.CreateLineItemsIndex();
                indexService.CreateCommentsIndex();
                indexService.CreateCustomAnswersIndex();
                indexService.CreateTrackingIndex();

                Log.Information("Order indexes created successfully at {0}", DateTime.UtcNow.ToPacificTime());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FAILED: Index creation failed because {0}", ex.Message);
            }
        }
    }
}
