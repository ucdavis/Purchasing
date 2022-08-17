using System;
using Microsoft.Azure.WebJobs;
using Ninject;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Purchasing.Jobs.Common.Logging;
using Purchasing.Jobs.NotificationsCommon;
using Serilog;

namespace Purchasing.Jobs.DailyEmailNotifications
{
    public class Program : WebJobBase
    {
        private static IDbService _dbService;

        static void Main(string[] args)
        {
            var kernel = ConfigureServices();
            _dbService = kernel.Get<IDbService>();
            EmailNotifications();
        }

        public static void EmailNotifications()
        {
            try
            {
                Log.Information("Processing daily email notifications");
                ProcessNotifications.ProcessEmails(_dbService, EmailPreferences.NotificationTypes.Daily);

                // send weekly summaries
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                {
                    Log.Information("Processing weekly email notifications");
                    ProcessNotifications.ProcessEmails(_dbService, EmailPreferences.NotificationTypes.Weekly);
                }

                Log.Information("Daily email notifications complete");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FAILED: Daily email notifications failed because {0}", ex.Message);
            }
        }
    }
}
