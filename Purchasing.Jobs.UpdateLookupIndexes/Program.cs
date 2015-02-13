using System;
using Ninject;
using Purchasing.Core.Helpers;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;

namespace Purchasing.Jobs.UpdateLookupIndexes
{
    class Program : WebJobBase
    {
        static void Main(string[] args)
        {
            var kernel = ConfigureServices();

            var indexService = kernel.Get<IIndexService>();

            try
            {
                indexService.CreateAccountsIndex();
                indexService.CreateBuildingsIndex();
                indexService.CreateCommoditiesIndex();
                indexService.CreateVendorsIndex();

                Console.WriteLine("Lookup indexes updated successfully at {0}", DateTime.UtcNow.ToPacificTime());
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED: Indexes failed because {0}", ex.Message);
            }
        }
    }
}
