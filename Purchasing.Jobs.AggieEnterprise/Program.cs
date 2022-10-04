using System;
using Ninject;
using Purchasing.Core.Helpers;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Purchasing.Jobs.Common.Logging;
using Serilog;

class Program : WebJobBase
{
    static void Main(string[] args)
    {
        var kernel = ConfigureServices();


        try
        {
            Log.Information("Updating Aggie Enterprise Purchasing Categories (Commodities)");



            Log.Information("Aggie Enterprise Purchasing Categories updated successfully at {0}", DateTime.UtcNow.ToPacificTime());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "FAILED: Aggie Enterprise Purchasing Categories failed because {0}", ex.Message);
        }
    }
}