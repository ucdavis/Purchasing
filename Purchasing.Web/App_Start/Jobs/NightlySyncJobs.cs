using System.Data;
using Dapper;
using Microsoft.Practices.ServiceLocation;
using Quartz;
using Purchasing.Web.Services;

namespace Purchasing.Web.App_Start.Jobs
{
    /// <summary>
    /// Runs sync jobs every night:
    /// usp_ProcessOrgDescendants
    /// usp_SyncWorkgroupAccounts
    /// </summary>
    public class NightlySyncJobs : IJob
    {
        private readonly IDbService _dbService;

        public NightlySyncJobs()
        {
            _dbService = ServiceLocator.Current.GetInstance<IDbService>();
        }

        public void Execute(IJobExecutionContext context)
        {
            var connectionString = context.MergedJobDataMap["connectionString"] as string;  

            using (var conn = _dbService.GetConnection(connectionString))
            {
                conn.Execute("usp_ProcessOrgDescendants", commandType: CommandType.StoredProcedure);
                conn.Execute("usp_SyncWorkgroupAccounts", commandType: CommandType.StoredProcedure);
            }
        }
    }
}