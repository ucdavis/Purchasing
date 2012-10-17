using System;
using Purchasing.Web.App_Start.Jobs;
using Quartz;
using Quartz.Impl;
using System.Web;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Purchasing.Web.App_Start.CreateIndexes), "ScheduleJobs")]
namespace Purchasing.Web.App_Start
{
    public static class CreateIndexes
    {
        private static void ScheduleJobs()
        {
            var indexRoot = HttpContext.Current.Server.MapPath("~/App_Data/Indexes");

            CreateOrderIndexesJob(indexRoot);
            CreateLookupIndexsJob(indexRoot);

            //CreateEmailJob();
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
            //var job = JobBuilder.Create<EmailJob>().Build();
            //var dailyjob = JobBuilder.Create<DailyEmailJob>().Build();

            //// 5 minutes minus current time into 5 minute interval = how much time to wait before starting
            //// so the job runs every 0/5 minute interval
            //// 300 seconds = 5 minutes
            //var offset = 300 - (((DateTime.Now.Minute * 60) + DateTime.Now.Second) % 300);

            //var trigger = TriggerBuilder.Create().ForJob(job)
            //                .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(5))
            //                .StartAt(DateTimeOffset.Now.AddSeconds(offset))
            //                .Build();

            //var dailyTrigger = TriggerBuilder.Create().ForJob(dailyjob)
            //                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(17, 0))
            //                    .StartNow().Build();

            //var sched = StdSchedulerFactory.GetDefaultScheduler();
            //sched.ScheduleJob(job, trigger);
            //sched.ScheduleJob(dailyjob, dailyTrigger);
            //sched.Start();
        }
    }
}