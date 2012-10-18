using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Purchasing.Core.Domain;
using SendGridMail;
using SendGridMail.Transport;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Services
{
    public interface INotificationSender
    {
        void SendNotifications();
        void SendDailyWeeklyNotifications();
        void SetAuthentication(string userName, string password);
    }

    public class EmailNotificationSender : INotificationSender
    {
        private readonly IRepositoryWithTypedId<EmailQueue, Guid> _emailRepository;
        private static string _sendGridUserName;
        private static string _sendGridPassword;
        private const string SendGridFrom = "opp-noreply@ucdavis.edu";

        public EmailNotificationSender(IRepositoryWithTypedId<EmailQueue, Guid> emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public void SetAuthentication(string userName, string password)
        {
            _sendGridUserName = userName;
            _sendGridPassword = password;
        }

        public void SendNotifications()
        {
            // always trigger per event emails
            ProcessEmails(EmailPreferences.NotificationTypes.PerEvent);
        }

        public void SendDailyWeeklyNotifications()
        {
            ProcessEmails(EmailPreferences.NotificationTypes.Daily);

            // send weekly summaries
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                ProcessEmails(EmailPreferences.NotificationTypes.Weekly);
            }
        }

        private void ProcessEmails(EmailPreferences.NotificationTypes notificationType)
        {
            Check.Require(!string.IsNullOrWhiteSpace(_sendGridUserName));
            Check.Require(!string.IsNullOrWhiteSpace(_sendGridPassword));

            var pending = _emailRepository.Queryable.Where(a => a.Pending && a.NotificationType == notificationType).ToList();
            var users = pending.Select(a => a.User).Distinct();

            foreach (var user in users)
            {
                var email = user.Email;

                var message = new StringBuilder("<ul>");
                foreach (var eq in pending.Where(a => a.User == user))
                {
                    if (!string.IsNullOrEmpty(eq.Email)) email = eq.Email;

                    message.Append("<li>");
                    message.Append(eq.Text);
                    message.Append("</li>");
                }
                message.Append("</ul>");

                var sgMessage = SendGrid.GenerateInstance();
                sgMessage.From = new MailAddress(SendGridFrom, "OPP No Reply");
                sgMessage.Subject = "PrePurchasing Notifications";
                sgMessage.AddTo(email);
                sgMessage.Html = message.ToString();

                //Start a transaction and try to send an email to the user. if there are no issues, mark that user's emails as non-pending and commit
                _emailRepository.DbContext.BeginTransaction();

                var transport = REST.GetInstance(new NetworkCredential(_sendGridUserName, _sendGridPassword));
                transport.Deliver(sgMessage);

                foreach (var pendingForUser in pending.Where(u => u.User == user))
                {
                    pendingForUser.Pending = false;
                    _emailRepository.EnsurePersistent(pendingForUser);
                }

                _emailRepository.DbContext.CommitTransaction();
            }
        }
    }

    public class DevNotificationSender : INotificationSender
    {
        public void SendNotifications()
        {
            // do nothing
        }

        public void SendDailyWeeklyNotifications()
        {
            // do nothing
        }

        public void SetAuthentication(string userName, string password)
        {
            // do nothing
        }
    }
}