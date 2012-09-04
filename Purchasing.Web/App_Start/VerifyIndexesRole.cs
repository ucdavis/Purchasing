using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using Purchasing.Web.Services;

namespace Purchasing.Web.App_Start
{
    public class VerifyIndexesRole : RoleEntryPoint
    {
        private static readonly IIndexService IndexService = new IndexService(new DbService());
        public override void Run()
        {
            const string pathRoot = @"E:\sitesroot\0\Purchasing.Web\App_Data\Indexes";

            IndexService.SetIndexRoot(pathRoot);

            var mail = new System.Net.Mail.SmtpClient("smtp.ucdavis.edu");
            var message = new System.Net.Mail.MailMessage("srkirkland@ucdavis.edu", "srkirkland@ucdavis.edu",
                                                          "verify indexes role notificaiton", "");

            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(5));
                try
                {
                    var lastModified = IndexService.LastModified(Indexes.OrderHistory);
                    var sinceLastModified = DateTime.Now - lastModified;

                    message.Body = string.Format("last modified at {0}, which was {1} minutes and {2} seconds ago",
                                                 lastModified, sinceLastModified.Minutes, sinceLastModified.Seconds);

                    //recreate index if it hasn't been modified for at least 6 minutes
                    //this would mean the update timer isn't working
                    if (sinceLastModified > TimeSpan.FromMinutes(6))
                    {
                        IndexService.CreateHistoricalOrderIndex();   
                    }
                }
                catch (Exception ex)
                {
                    message.Body = "error checking/setting index: " + ex.Message;
                }

                mail.Send(message);
            }
        }
    }
}