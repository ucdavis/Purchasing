using Dapper;
using Purchasing.Core.Domain;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

namespace Purchasing.Core.Services
{
    public interface IAeLookupsService
    {
        Task UpdateCategories(bool resetAll = false);
        Task UpdateUnitOfMeasure();
    }

    public class AeLookupsService : IAeLookupsService
    {
        private readonly IDbService _dbService ;
        private readonly IAggieEnterpriseService _aggieEnterpriseService;

        public AeLookupsService(IDbService dbService, IAggieEnterpriseService aggieEnterpriseService)
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

        public async Task UpdateUnitOfMeasure()
        {
            //Get new unit of measure values
            //Confirm there are more than 10

            //Clear out the unit of measure table

            var units = await _aggieEnterpriseService.GetUnitOfMeasures();
            if (units == null || units.Count() < 10)
            {
                Log.Error("Unit of measure count is less than 10. Not updating");
                throw new System.Exception("Unit of measure count is less than 10. Not updating");
            }

            using (var connection = _dbService.GetConnection())
            {
                using (var ts = connection.BeginTransaction())
                {
                    await connection.ExecuteAsync("delete from UnitOfMeasures", null, ts);
                    await ts.CommitAsync();
                    Log.Information("Unit of measure table cleared");
                }
                //Add all the new units
                var count = 0;
                using (var ts = connection.BeginTransaction())
                {
                    foreach (var unit in units)
                    {
                        await connection.ExecuteAsync("insert into UnitOfMeasures (Id, Name) values (@id, @name)", new { unit.Id, unit.Name }, ts);
                        count++;
                    }
                    await ts.CommitAsync();
                    Log.Information("Unit of measure table updated with {count} units", count);
                }
            }
            return;
        }

    }
}
