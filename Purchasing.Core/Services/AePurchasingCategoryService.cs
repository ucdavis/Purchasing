using Dapper;
using Purchasing.Core.Domain;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

namespace Purchasing.Core.Services
{
    public interface IAePurchasingCategoryService
    {
        Task UpdateCategories(bool resetAll = false);
    }

    public class AePurchasingCategoryService : IAePurchasingCategoryService
    {
        private readonly IDbService _dbService ;
        private readonly IAggieEnterpriseService _aggieEnterpriseService;

        public AePurchasingCategoryService(IDbService dbService, IAggieEnterpriseService aggieEnterpriseService)
        {
            _dbService = dbService;
            _aggieEnterpriseService = aggieEnterpriseService;
        }

        public async Task UpdateCategories(bool resetAll = false)
        {
            var categories = await _aggieEnterpriseService.GetPurchasingCategories(); // get any categories before any reset. This way if this throws an exception at least we have the old categories

            if (resetAll)
            {
                //Inactivate all categories
                using (var connection = _dbService.GetConnection())
                {
                    using (var ts = connection.BeginTransaction())
                    {
                        //Testing just one.
                        await connection.ExecuteAsync("update vCommodities set IsActive = 0 where IsActive = 1", null, ts);
                        ts.Commit();
                    }
                }
            } 

            using (var connection = _dbService.GetConnection())
            {
                using (var ts = connection.BeginTransaction())
                {
                    var updated = 0;
                    var added = 0;
                    var activeCategories = categories.Where(x => x.IsActive).ToArray();
                    if (activeCategories.Any())
                    {
                        foreach (var category in activeCategories)
                        {
                            var newCatgory = await connection.QueryFirstOrDefaultAsync<Commodity>("Select * from vCommodities where id = @id", new { category.Id }, ts);

                            if (newCatgory == null)
                            {
                                await connection.ExecuteAsync("insert into vCommodities (Id, Name, IsActive) values (@id, @name, @isactive)", new { category.Id, category.Name, category.IsActive }, ts);

                                added++;
                            }
                            else if (newCatgory.Name != category.Name || newCatgory.IsActive != category.IsActive)
                            {
                                await connection.ExecuteAsync("update vCommodities set Name = @name, IsActive = @isactive where Id = @id", new { category.Id, category.Name, category.IsActive }, ts);
                                updated++;
                            }
                        }
                        await ts.CommitAsync();
                    }
                    Log.Information("Categories: Updated {updatedCount} Added: {addedCount}", updated, added);
                }

                using (var ts = connection.BeginTransaction())
                {
                    var count = 0;
                    var inactiveCategories = categories.Where(x => !x.IsActive).ToArray();
                    if (inactiveCategories.Any())
                    {
                        foreach (var category in inactiveCategories)
                        {
                            var newCatgory = await connection.QueryFirstOrDefaultAsync<Commodity>("Select * from vCommodities where id = @id", new { category.Id }, ts);
                            if (newCatgory != null && newCatgory.IsActive != category.IsActive)
                            {
                                await connection.ExecuteAsync("update vCommodities set Name = @name, IsActive = @isactive where Id = @id", new { category.Id, category.Name, category.IsActive }, ts);
                                count++;
                            }
                        }
                        await ts.CommitAsync();
                    }
                    Log.Information("Deactivated Categories: {count}", count);
                }
            }

            return;
        }

        //Set all vCommodity records to inactive.
        //Read all Purchasing categories
        //see if it exists, if it does, activate it and update the name
        //if it doesn't exist, create it and activate it
        //query graphQl for ones that the end date has just past to deactivate them?
    }
}
