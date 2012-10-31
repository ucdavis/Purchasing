using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;
using Purchasing.WS;
using Quartz;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.App_Start.Jobs
{
    public class DatabaseBackupJob : IJob
    {
        private readonly IRepository<BackupLog> _backupLogRespoitory;

        public DatabaseBackupJob(IRepository<BackupLog> backupLogRespoitory)
        {
            _backupLogRespoitory = backupLogRespoitory;
        }

        public void Execute(IJobExecutionContext context)
        {
            var storageUrl = context.MergedJobDataMap["storageUrl"] as string;
            var serverName = context.MergedJobDataMap["serverName"] as string;
            var username = context.MergedJobDataMap["username"] as string;
            var password = context.MergedJobDataMap["password"] as string;
            var storageKey = context.MergedJobDataMap["storageKey"] as string;

            // initialize the service
            var azureService = new AzureStorageService(storageUrl, serverName, username, password);

            var flag = false;

            // check for an "outstanding" job
            var bl = _backupLogRespoitory.Queryable.Where(a => !a.Completed).OrderByDescending(a => a.DateTimeCreated).FirstOrDefault();
            if (bl != null)
            {
                // check the status of the job
                var result = azureService.GetStatus(bl.RequestId);

                if (result == "Completed")
                {
                    bl.Completed = true;
                    _backupLogRespoitory.EnsurePersistent(bl);
                }
                else
                {
                    // job still running, skip this time around
                    flag = true;
                }

            }

            if (!flag)
            {
                // record the request
                var requestId = azureService.Backup("PrePurchasing", storageKey);
                var backupLog = new BackupLog() { RequestId = requestId };
                _backupLogRespoitory.EnsurePersistent(backupLog);    
            }
        }
    }
}