using System;
using Ninject;
using Purchasing.Core.Helpers;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Purchasing.Jobs.Common.Logging;
using Serilog;

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
                Log.Information("Updating Lookup Indexes");
                indexService.CreateAccountsIndex();
                indexService.CreateBuildingsIndex();
                indexService.CreateCommoditiesIndex();
                indexService.CreateVendorsIndex();

                Log.Information("Lookup indexes updated successfully at {0}", DateTime.UtcNow.ToPacificTime());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FAILED: Indexes failed because {0}", ex.Message);
            }
        }
    }
}
