using System;
using Ninject;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Purchasing.Jobs.Common.Logging;
using Purchasing.Jobs.NotificationsCommon;

namespace Purchasing.Jobs.EmailNotifications
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

            EmailNotifications();
        }

        private static void EmailNotifications()
        {
            ProcessNotifications.ProcessEmails(_dbService, EmailPreferences.NotificationTypes.PerEvent);
        }
    }
}