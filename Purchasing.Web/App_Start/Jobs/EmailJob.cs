using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.Services;
using Quartz;

namespace Purchasing.Web.App_Start.Jobs
{
    public class EmailJob : IJob
    {
        private readonly INotificationService _notificationService;

        public EmailJob()
        {
            _notificationService = ServiceLocator.Current.GetInstance<INotificationService>(); ;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationService.SendEmails();
        }
    }
}