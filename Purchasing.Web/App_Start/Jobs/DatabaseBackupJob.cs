using System;
using System.Linq;
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
            var storageAccountName = context.MergedJobDataMap["storageAccountName"] as string;
            var serverName = context.MergedJobDataMap["serverName"] as string;
            var username = context.MergedJobDataMap["username"] as string;
            var password = context.MergedJobDataMap["password"] as string;
            var storageKey = context.MergedJobDataMap["storageKey"] as string;
            var blobContainer = context.MergedJobDataMap["blobContainer"] as string;

            // initialize the service
            var azureService = new AzureStorageService(serverName, username, password, storageAccountName, storageKey, blobContainer);

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
                var filename = string.Empty;

                // record the request
                var requestId = azureService.Backup("PrePurchasing", out filename);
                var backupLog = new BackupLog() { RequestId = requestId, Filename = filename};
                _backupLogRespoitory.EnsurePersistent(backupLog);    

                // perform database cleanup
                var deleted = azureService.BlobCleanup();

                foreach (var blobName in deleted)
                {
                    var blob = _backupLogRespoitory.Queryable.FirstOrDefault(a => a.Filename == blobName);
                    if (blob != null)
                    {
                        blob.Deleted = true;
                        blob.DateTimeDeleted = DateTime.Now;
                        _backupLogRespoitory.EnsurePersistent(blob);
                    }

                }
            }
        }
    }
}