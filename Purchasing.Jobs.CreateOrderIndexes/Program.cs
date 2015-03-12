using System;
using Ninject;
using Purchasing.Core.Helpers;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;

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
                indexService.CreateHistoricalOrderIndex();
                indexService.CreateLineItemsIndex();
                indexService.CreateCommentsIndex();
                indexService.CreateCustomAnswersIndex();
                indexService.CreateTrackingIndex();

                Console.WriteLine("Order indexes created successfully at {0}", DateTime.UtcNow.ToPacificTime());
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED: Index creation failed because {0}", ex.Message);
            }
        }
    }
}
