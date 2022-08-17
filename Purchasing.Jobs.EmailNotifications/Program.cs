﻿using System;
using Ninject;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Purchasing.Jobs.Common.Logging;
using Purchasing.Jobs.NotificationsCommon;
using Serilog;

namespace Purchasing.Jobs.EmailNotifications
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

        private static void EmailNotifications()
        {
            try
            {
                Log.Information("Processing per-event email notifications");
                ProcessNotifications.ProcessEmails(_dbService, EmailPreferences.NotificationTypes.PerEvent);
                Log.Information("Per-event email notifications complete");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FAILED: Per-event email notifications failed because {0}", ex.Message);
            }
        }
    }
}