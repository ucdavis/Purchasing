using Dapper;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAggieEnterpriseService _aggieEnterpriseService;

        public AePurchasingCategoryService(IDbService dbService, IRepositoryFactory repositoryFactory, IAggieEnterpriseService aggieEnterpriseService)
        {
            _dbService = dbService;
            _repositoryFactory = repositoryFactory;
            _aggieEnterpriseService = aggieEnterpriseService;
        }

        public async Task UpdateCategories(bool resetAll = false)
        {

            if (resetAll)
            {
                //Inactivate all categories
                using (var connection = _dbService.GetConnection())
                {
                    using (var ts = connection.BeginTransaction())
                    {
                        //Testing just one.
                        connection.Execute("update vCommodities set IsActive = 0 where IsActive = 1", null, ts);
                        ts.Commit();
                    }
                }
            }
            var categories = await _aggieEnterpriseService.GetPurchasingCategories(); 

            var activeCategories = categories.Where(x => x.IsActive).ToArray();
            if (activeCategories.Any())
            {
                foreach (var category in activeCategories)
                {
                    var newCatgory = _repositoryFactory.CommodityRepository.Queryable.Where(x => x.Id == category.Id).SingleOrDefault();
                    if (newCatgory == null)
                    {
                        newCatgory = new Commodity();
                        newCatgory.Id = category.Id;
                        newCatgory.Name = category.Name;
                        newCatgory.IsActive = category.IsActive;
                        _repositoryFactory.CommodityRepository.EnsurePersistent(newCatgory);
                    }
                    else if(newCatgory.Name != category.Name || newCatgory.IsActive != category.IsActive)
                    {
                        newCatgory.Name = category.Name;
                        newCatgory.IsActive = category.IsActive;
                        _repositoryFactory.CommodityRepository.EnsurePersistent(newCatgory);
                    }
                    
                }
            }
            var inactiveCategories = categories.Where(x => !x.IsActive).ToArray();
            if (inactiveCategories.Any())
            {
                foreach (var category in inactiveCategories)
                {
                    var newCatgory = _repositoryFactory.CommodityRepository.Queryable.Where(x => x.Id == category.Id).SingleOrDefault();
                    if (newCatgory != null && newCatgory.IsActive != category.IsActive)
                    {
                        newCatgory.Name = category.Name;
                        newCatgory.IsActive = category.IsActive;
                        _repositoryFactory.CommodityRepository.EnsurePersistent(newCatgory);
                    }                    
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
