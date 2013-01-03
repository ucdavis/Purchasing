using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Core.Domain;
using Purchasing.WS;
using Quartz;
using SendGridMail;
using SendGridMail.Transport;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.App_Start.Jobs
{
    public class DatabaseBackupJob : IJob
    {
        private readonly IRepository<BackupLog> _backupLogRespoitory;

        public DatabaseBackupJob()
        {
            _backupLogRespoitory = ServiceLocator.Current.GetInstance<IRepository<BackupLog>>();
        }

        public void Execute(IJobExecutionContext context)
        {
            var storageAccountName = context.MergedJobDataMap["storageAccountName"] as string;
            var serverName = context.MergedJobDataMap["serverName"] as string;
            var username = context.MergedJobDataMap["username"] as string;
            var password = context.MergedJobDataMap["password"] as string;
            var storageKey = context.MergedJobDataMap["storageKey"] as string;
            var blobContainer = context.MergedJobDataMap["blobContainer"] as string;

            var sendGridUserName = context.MergedJobDataMap["sendGridUser"] as string;
            var sendGridPassword = context.MergedJobDataMap["sendGridPassword"] as string;

            // initialize the service
            var azureService = new AzureStorageService(serverName, username, password, storageAccountName, storageKey, blobContainer);


            try
            {
                // make the commands for backup
                string filename;
                var reqId = azureService.BackupDataSync("PrePurchasing", out filename);

                // save a record of this backup job
                var backupLog = new BackupLog() {RequestId = reqId, Filename = filename};
                _backupLogRespoitory.EnsurePersistent(backupLog);

                // clean up the blob
                azureService.BlobCleanup();
            }
            catch (Exception ex)
            {
                SendSingleEmail("anlai@ucdavis.edu", "PrePurchasing - Error Backup Job", ex.Message, sendGridUserName, sendGridPassword);
            }
        }

        private void SendSingleEmail(string email, string subject, string body, string sendGridUserName, string sendGridPassword)
        {
            Check.Require(!string.IsNullOrWhiteSpace(sendGridUserName));
            Check.Require(!string.IsNullOrWhiteSpace(sendGridPassword));

            var sgMessage = SendGrid.GenerateInstance();
            sgMessage.From = new MailAddress("opp-noreply@ucdavis.edu", "OPP No Reply");
            sgMessage.Subject = subject;
            sgMessage.AddTo(email);
            sgMessage.Html = body;

            var transport = SMTP.GenerateInstance(new NetworkCredential(sendGridUserName, sendGridPassword));
            transport.Deliver(sgMessage);
        }
    }
}