using System;
using System.Data;
using System.Net;
using System.Net.Mail;
using Dapper;
using Microsoft.Practices.ServiceLocation;
using Quartz;
using Purchasing.Web.Services;
using SendGridMail;
using SendGridMail.Transport;
using UCDArch.Core.Utils;

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

            var sendGridUserName = context.MergedJobDataMap["sendGridUser"] as string;
            var sendGridPassword = context.MergedJobDataMap["sendGridPassword"] as string;

            using (var conn = _dbService.GetConnection(connectionString))
            {
                try
                {
                    conn.Execute("usp_ProcessOrgDescendants", commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    SendSingleEmail("anlai@ucdavis.edu", "Error running Process Org Descendants", ex.Message, sendGridUserName, sendGridPassword);
                }

                try
                {
                    conn.Execute("usp_SyncWorkgroupAccounts", commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    SendSingleEmail("anlai@ucdavis.edu", "Error running Sync Workgroup Accounts", ex.Message, sendGridUserName, sendGridPassword);
                }
                
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