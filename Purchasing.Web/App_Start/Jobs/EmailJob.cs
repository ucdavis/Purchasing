using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.Services;
using Quartz;

namespace Purchasing.Web.App_Start.Jobs
{
    public class EmailJob : IJob
    {
        private readonly INotificationSender _notificationSender;

        public EmailJob()
        {
            _notificationSender = ServiceLocator.Current.GetInstance<INotificationSender>(); ;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationSender.SendNotifications();
        }
    }

    public class DailyEmailJob : IJob
    {
        private readonly INotificationSender _notificationSender;

        public DailyEmailJob()
        {
            _notificationSender = ServiceLocator.Current.GetInstance<INotificationSender>(); ;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationSender.SendDailyWeeklyNotifications();
        } 
    }
}