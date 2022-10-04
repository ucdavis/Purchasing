using Microsoft.Extensions.DependencyInjection;
using Purchasing.Core;
using Purchasing.Core.Helpers;
using Purchasing.Core.Models.Configuration;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Serilog;

class Program : WebJobBase
{
    static void Main(string[] args)
    {
        var kernel = ConfigureServices();

        var provider = ConfigureServicesLocal();

        try
        {
            Log.Information("Updating Aggie Enterprise Purchasing Categories (Commodities)");
            var aeCommodityCodeService = provider.GetService<IAePurchasingCategoryService>();

            aeCommodityCodeService?.UpdateCategories().GetAwaiter().GetResult();

            Log.Information("Aggie Enterprise Purchasing Categories updated successfully at {0}", DateTime.UtcNow.ToPacificTime());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "FAILED: Aggie Enterprise Purchasing Categories failed because {0}", ex.Message);
        }
    }

    private static ServiceProvider ConfigureServicesLocal()
    {
        IServiceCollection services = new ServiceCollection();

        // required services
        services.Configure<AggieEnterpriseOptions>(configuration.GetSection("AggieEnterprise"));
        services.AddSingleton<IDbService, DbService>();
        services.AddSingleton<IRepositoryFactory, RepositoryFactory>(); //Can't really use this here, but it's required for the service
        services.AddSingleton<IAggieEnterpriseService, AggieEnterpriseService>();
        services.AddSingleton<IAePurchasingCategoryService, AePurchasingCategoryService>();





        return services.BuildServiceProvider();
    }
}