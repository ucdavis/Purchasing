using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using Purchasing.Web.App_Start.Jobs;
using Quartz;
using Quartz.Impl;
using System.Web;
using System.Web.Configuration;
using SendGridMail;
using SendGridMail.Transport;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Purchasing.Web.App_Start.CreateJobs), "ScheduleJobs")]
namespace Purchasing.Web.App_Start
{
    public static class CreateJobs
    {
        private static void ScheduleJobs()
        {
            var indexRoot = HttpContext.Current.Server.MapPath("~/App_Data/Indexes");

            CreateOrderIndexesJob(indexRoot);
            CreateLookupIndexsJob(indexRoot);

            CreateNightlySyncJobs();
            CreateEmailJob();

            CreateDatabaseBackupJob();
        }

        private static void CreateNightlySyncJobs()
        {
            var connectionString = WebConfigurationManager.ConnectionStrings["MainDb"].ConnectionString;

            // create job
            var jobDetails = JobBuilder.Create<NightlySyncJobs>().UsingJobData("connectionString", connectionString).Build();

            // create trigger
            var nightly =
                TriggerBuilder.Create().ForJob(jobDetails).WithSchedule(
                    CronScheduleBuilder.DailyAtHourAndMinute(4, 0)).StartNow().Build();

            // get reference to scheduler (remote or local) and schedule job
            var sched = StdSchedulerFactory.GetDefaultScheduler();
            sched.ScheduleJob(jobDetails, nightly);
            sched.Start();
        }

        private static void CreateLookupIndexsJob(string indexRoot)
        {
            // create job
            var jobDetails = JobBuilder.Create<CreateLookupIndexsJob>().UsingJobData("indexRoot", indexRoot).Build();
            var runOnce = JobBuilder.Create<CreateLookupIndexsJob>().UsingJobData("indexRoot", indexRoot).Build();
            
            // create trigger-- run every day starting at 5AM
            var dailyTrigger = TriggerBuilder.Create().ForJob(jobDetails).WithSchedule(
                    CronScheduleBuilder.DailyAtHourAndMinute(5, 0)
                )
                .StartNow()
                .Build();

            //start one run in 2 seconds from now to prime the cache, in order to give the more important jobs time to run quickly
            var primeCache =
                TriggerBuilder.Create().ForJob(runOnce).WithSchedule(
                        SimpleScheduleBuilder.RepeatMinutelyForTotalCount(1)
                    )
                    .StartAt(DateTimeOffset.Now.AddSeconds(2))
                    .Build();
            
            // schedule the jobs, first prime cache and then daily will run
            var sched = StdSchedulerFactory.GetDefaultScheduler();
            sched.ScheduleJob(runOnce, primeCache);
            sched.ScheduleJob(jobDetails, dailyTrigger);
            sched.Start();
        }

        private static void CreateOrderIndexesJob(string indexRoot)
        {
            // create job
            var jobDetails = JobBuilder.Create<CreateOrderIndexsJob>().UsingJobData("indexRoot", indexRoot).Build();

            // create trigger
            var everyFiveMinutes =
                TriggerBuilder.Create().ForJob(jobDetails).WithSchedule(
                    SimpleScheduleBuilder.RepeatMinutelyForever(5)).StartNow().Build();

            // get reference to scheduler (remote or local) and schedule job
            var sched = StdSchedulerFactory.GetDefaultScheduler();
            sched.ScheduleJob(jobDetails, everyFiveMinutes);
            sched.Start();
        }

        private static void CreateEmailJob()
        {
            //only create the email job if we explicitly set sendNotifications
            if (WebConfigurationManager.AppSettings["SendNotifications"] == "true")
            {
                var job = JobBuilder.Create<EmailJob>().Build();
                var dailyjob = JobBuilder.Create<DailyEmailJob>().Build();

                //run daily trigger every 5 minutes after inital 30 second delay to give priority to warmup
                var trigger = TriggerBuilder.Create().ForJob(job)
                                .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(5))
                                .StartAt(DateTimeOffset.Now.AddSeconds(30))
                                .Build();

                var dailyTrigger = TriggerBuilder.Create().ForJob(dailyjob)
                                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(17, 0))
                                    .StartNow().Build();

                var sched = StdSchedulerFactory.GetDefaultScheduler();
                sched.ScheduleJob(job, trigger);
                sched.ScheduleJob(dailyjob, dailyTrigger);
                sched.Start();
            }
        }

        private static void CreateDatabaseBackupJob()
        {
            var storageAccountName = WebConfigurationManager.AppSettings["AzureStorageAccountName"];
            var serverName = WebConfigurationManager.AppSettings["AzureServerName"];
            var username = WebConfigurationManager.AppSettings["AzureUserName"];
            var password = WebConfigurationManager.AppSettings["AzurePassword"];
            var storageKey = WebConfigurationManager.AppSettings["AzureStorageKey"];
            var blobContainer = WebConfigurationManager.AppSettings["AzureBlobContainer"];

            var jobDetails = JobBuilder.Create<DatabaseBackupJob>().UsingJobData("storageAccountName", storageAccountName).UsingJobData("serverName", serverName).UsingJobData("username", username).UsingJobData("password", password).UsingJobData("storageKey", storageKey).UsingJobData("blobContainer", blobContainer).Build();

            var nightly = TriggerBuilder.Create().ForJob(jobDetails).WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(9, 42)).StartNow().Build();
            var sched = StdSchedulerFactory.GetDefaultScheduler();
            sched.ScheduleJob(jobDetails, nightly);
            sched.Start();
        }
    }
}