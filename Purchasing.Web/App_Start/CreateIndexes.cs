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
        }

        private static void CreateLookupIndexsJob(string indexRoot)
        {
            // create job
            var jobDetails = JobBuilder.Create<CreateLookupIndexsJob>().UsingJobData("indexRoot", indexRoot).Build();
            
            // create trigger
            //run monday->friday every day starting at 5AM and only running once
            //StartAt is 2 seconds from now, in order to give the more important jobs time to run quickly
            var dailyTrigger = TriggerBuilder.Create().WithSchedule(
                DailyTimeIntervalScheduleBuilder.Create()
                    .OnMondayThroughFriday()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 0))
                    .WithRepeatCount(0))
                .StartAt(DateTimeOffset.Now.AddSeconds(2))
                .Build();

            // get reference to scheduler (remote or local) and schedule job
            var sched = StdSchedulerFactory.GetDefaultScheduler();
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
    }
}