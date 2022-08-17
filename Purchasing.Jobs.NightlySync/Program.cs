using System;
using Dapper;
using Microsoft.Azure.WebJobs;
using Ninject;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Purchasing.Jobs.Common.Logging;
using Serilog;
using System.Data;

namespace Purchasing.Jobs.NightlySync
{
    public class Program : WebJobBase
    {
        private static IDbService _dbService;

        static void Main(string[] args)
        {
            var kernel = ConfigureServices();
            _dbService = kernel.Get<IDbService>();
            NightlySync();
        }

        private static void NightlySync()
        {
            try
            {
                Log.Information("Processing nightly sync");
                using (var db = _dbService.GetConnection())
                {
                    try
                    {
                        var rows = db.Execute("usp_ProcessOrgDescendants", commandType: CommandType.StoredProcedure, commandTimeout: 300);
                        Log.Information("usp_ProcessOrgDescendants: {0} rows affected", rows);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error running Org Descendants");
                    }

                    try
                    {
                        var rows = db.Execute("usp_SyncWorkgroupAccounts", commandType: CommandType.StoredProcedure, commandTimeout: 300);
                        Log.Information("usp_SyncWorkgroupAccounts: {0} rows affected", rows);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error running Sync Workgroup Accounts");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FAILED: Nightly sync failed because {0}", ex.Message);
            }
        }
    }
}
