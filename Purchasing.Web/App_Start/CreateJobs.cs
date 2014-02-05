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

            CreateVerifyPermissionsJob(); // If the org descendants changes, it is possible the inherited workgroup permissions will change. This checks that and emails me. -JCS
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
            var createJobDetails = JobBuilder.Create<CreateOrderIndexsJob>().UsingJobData("indexRoot", indexRoot).Build();
            var updateJobDetails = JobBuilder.Create<UpdateOrderIndexsJob>().UsingJobData("indexRoot", indexRoot).Build();

            // create trigger
            var runCreateOnce =
                TriggerBuilder.Create().ForJob(createJobDetails).WithSchedule(
                    SimpleScheduleBuilder.RepeatMinutelyForTotalCount(1)).StartNow().Build();

            var runUpdateEveryFiveMinutes =
                TriggerBuilder.Create().ForJob(updateJobDetails).WithSchedule(
                    SimpleScheduleBuilder.RepeatMinutelyForever(5)).StartNow().Build();

            // get reference to scheduler (remote or local) and schedule job
            var sched = StdSchedulerFactory.GetDefaultScheduler();
            sched.ScheduleJob(createJobDetails, runCreateOnce);
            sched.ScheduleJob(updateJobDetails, runUpdateEveryFiveMinutes);
            sched.Start();
        }

        private static void CreateVerifyPermissionsJob()
        {
            var sendGridUser = WebConfigurationManager.AppSettings["SendGridUserName"];
            var sendGridPassword = WebConfigurationManager.AppSettings["SendGridPassword"];

            var jobDetails = JobBuilder.Create<VerifyPermissionsJob>().UsingJobData("sendGridUser", sendGridUser).UsingJobData("sendGridPassword", sendGridPassword).Build();

            var nightly =
                TriggerBuilder.Create().ForJob(jobDetails).WithSchedule(
                    CronScheduleBuilder.DailyAtHourAndMinute(6, 0)).StartNow().Build();
            var sched = StdSchedulerFactory.GetDefaultScheduler();
            sched.ScheduleJob(jobDetails, nightly);
            sched.Start();
        }
    }
}