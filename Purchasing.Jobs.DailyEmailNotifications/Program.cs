using System;
using Microsoft.Azure.WebJobs;
using Ninject;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Purchasing.Jobs.Common.Logging;
using Purchasing.Jobs.NotificationsCommon;

namespace Purchasing.Jobs.DailyEmailNotifications
{
    public class Program : WebJobBase
    {
        private static IDbService _dbService;

        static void Main(string[] args)
        {
            LogHelper.ConfigureLogging();

            Console.WriteLine("Build Number: {0}", typeof(Program).Assembly.GetName().Version);

            var kernel = ConfigureServices();
            _dbService = kernel.Get<IDbService>();
            // var jobHost = new JobHost();
            // jobHost.Call(typeof(Program).GetMethod("EmailNotifications"));
            
        }

        [NoAutomaticTrigger]
        public static void EmailNotifications()
        {
            ProcessNotifications.ProcessEmails(_dbService, EmailPreferences.NotificationTypes.Daily);

            // send weekly summaries
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                ProcessNotifications.ProcessEmails(_dbService, EmailPreferences.NotificationTypes.Weekly);
            }
        }
    }
}
