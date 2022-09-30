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
        Task ResetCategories();
    }

    public class AePurchasingCategoryService : IAePurchasingCategoryService
    {
        private readonly IDbService _dbService ;
        public AePurchasingCategoryService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task ResetCategories()
        {
            //throw new NotImplementedException();

            using (var connection = _dbService.GetConnection())
            {
                using (var ts = connection.BeginTransaction())
                {
                    connection.Execute("update vCommodities set IsActive = 0 where id = @id", new { now = DateTime.UtcNow.ToPacificTime(), id = "10101501" }, ts);
                    ts.Commit();
                }
            }
            
            
        }
        //Example of an update:
        //using (var ts = connection.BeginTransaction())
        //connection.Execute("update EmailQueueV2 set Pending = 0, DateTimeSent = @now where id = @id",        new { now = DateTime.UtcNow.ToPacificTime(), id = emailQueue.Id    }, ts);
        //ts.Commit();

        //Set all vCommodity records to inactive.
        //Read all Purchasing categories
        //see if it exists, if it does, activate it and update the name
        //if it doesn't exist, create it and activate it
        //query graphQl for ones that the end date has just past to deactivate them?
    }
}
