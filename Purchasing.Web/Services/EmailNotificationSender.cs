using System;
using System.Collections.Generic;
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
        private readonly IRepositoryWithTypedId<EmailQueueV2, Guid> _emailQueueV2Repository;
        private static string _sendGridUserName;
        private static string _sendGridPassword;
        private const string SendGridFrom = "opp-noreply@ucdavis.edu";

        public EmailNotificationSender(IRepositoryWithTypedId<EmailQueue, Guid> emailRepository, IRepositoryWithTypedId<EmailQueueV2, Guid> emailQueueV2Repository)
        {
            _emailRepository = emailRepository;
            _emailQueueV2Repository = emailQueueV2Repository;
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
            ProcessEmailsV2(EmailPreferences.NotificationTypes.PerEvent);
        }

        public void SendDailyWeeklyNotifications()
        {
            ProcessEmails(EmailPreferences.NotificationTypes.Daily);
            ProcessEmailsV2(EmailPreferences.NotificationTypes.Daily);

            // send weekly summaries
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                ProcessEmails(EmailPreferences.NotificationTypes.Weekly);
                ProcessEmailsV2(EmailPreferences.NotificationTypes.Weekly);
            }
        }

        private void ProcessEmailsV2(EmailPreferences.NotificationTypes notificationType)
        {
            Check.Require(!string.IsNullOrWhiteSpace(_sendGridUserName));
            Check.Require(!string.IsNullOrWhiteSpace(_sendGridPassword));

            var pending = _emailQueueV2Repository.Queryable.Where(a => a.Pending && a.NotificationType == notificationType).ToList();
            var users = pending.Where(b => b.User != null).Select(a => a.User).Distinct();

            #region Workgroup Notifications have a null User
            var workgroupNotifications = pending.Where(b => b.User == null).Select(a => a.Email).Distinct();
            foreach (var wEmail in workgroupNotifications)
            {
                var pendingForUser = pending.Where(e => e.Email == wEmail).ToList();

                var email = wEmail;

                BatchEmail(email, pendingForUser);
            }
            #endregion Workgroup Notifications have a null User

            #region Normal Email Notification, user will never be null

            foreach (var user in users)
            {
                var pendingForUser = pending.Where(e => e.User == user).ToList();

                var email = user.Email;
                BatchEmail(email, pendingForUser);
            }
            #endregion Normal Email Notification, user will never be null

            return;
        }

        private void ProcessEmails(EmailPreferences.NotificationTypes notificationType)
        {
            Check.Require(!string.IsNullOrWhiteSpace(_sendGridUserName));
            Check.Require(!string.IsNullOrWhiteSpace(_sendGridPassword));

            var pending = _emailRepository.Queryable.Where(a => a.Pending && a.NotificationType == notificationType).ToList();
            var users = pending.Where(b => b.User != null).Select(a => a.User).Distinct();

            #region Workgroup Notifications have a null User            
            var workgroupNotifications = pending.Where(b => b.User == null).Select(a => a.Email).Distinct();
            foreach (var wEmail in workgroupNotifications)
            {
                var pendingForUser = pending.Where(e => e.Email == wEmail).ToList();

                var email = wEmail;

                BatchEmail(email, pendingForUser);
            }
            #endregion Workgroup Notifications have a null User

            #region Normal Email Notification, user will never be null

            foreach (var user in users)
            {
                var pendingForUser = pending.Where(e => e.User == user).ToList();

                var email = user.Email;
                BatchEmail(email, pendingForUser);
            }
            #endregion Normal Email Notification, user will never be null
        }

        private void BatchEmail(string email, List<EmailQueueV2> pendingForUser)
        {
            _emailQueueV2Repository.DbContext.BeginTransaction();            
            var orders = pendingForUser.Select(a => a.Order).Distinct().ToList();

            var message = new StringBuilder("<ul>");
            foreach (var order in orders)
            {
                message.Append("<li>");
                message.Append("<div>");
                message.Append(string.Format("Order Request {0}", GenerateLink(order.RequestNumber)));

                message.Append("<ul>");
                message.Append(string.Format("<li><strong>Created By:</strong>{0}</li>", order.CreatedBy.FullName));
                message.Append(string.Format("<li><strong>Status:</strong>{0}</li>", order.StatusCode.Name));
                message.Append(string.Format("<li><strong>Vendor:</strong>{0}</li>", order.VendorName));
                message.Append("</ul>");

                message.Append("<table>");
                message.Append("<tbody>");

                foreach (var emailQueue in pendingForUser.Where(a => a.Order == order).OrderByDescending(b => b.DateTimeCreated))
                {
                    message.Append("<tr>");
                    message.Append(string.Format("<td>{0}</td>", emailQueue.DateTimeCreated));
                    message.Append(string.Format("<td>{0}</td>", emailQueue.Action));
                    message.Append(string.Format("<td>{0}</td>", emailQueue.Details));
                    message.Append("</tr>");

                    emailQueue.Pending = false;
                    emailQueue.DateTimeSent = DateTime.Now;
                    _emailQueueV2Repository.EnsurePersistent(emailQueue);
                }

                message.Append("</tbody>");
                message.Append("</table>");

                message.Append("</div>");
                message.Append("</li>");
            }
            message.Append("</ul>");

            var sgMessage = SendGrid.GenerateInstance();
            sgMessage.From = new MailAddress(SendGridFrom, "UCD PrePurchasing No Reply");

            sgMessage.Subject = pendingForUser.Count == 1
                        ? string.Format("PrePurchasing Notification for Order #{0}",
                                        pendingForUser.Single().Order.RequestNumber)
                        : "PrePurchasing Notifications";
           
            sgMessage.AddTo(email);
            sgMessage.Html = message.ToString();

            var transport = SMTP.GenerateInstance(new NetworkCredential(_sendGridUserName, _sendGridPassword));
            transport.Deliver(sgMessage);

            _emailQueueV2Repository.DbContext.CommitTransaction();
        }

        private string GenerateLink(string orderRequestNumber)
        {
            return string.Format("<a href=\"{0}{1}\">{1}</a>", "http://prepurchasing.ucdavis.edu/Order/Lookup/", orderRequestNumber);
        }

        private void BatchEmail(string email, List<EmailQueue> pendingForUser)
        {
            //Start a transaction and try to send an email to the user. if there are no issues, mark that user's emails as non-pending and commit
            _emailRepository.DbContext.BeginTransaction();

            var message = new StringBuilder("<ul>");
            foreach (var eq in pendingForUser)
            {
                message.Append("<li>");
                message.Append(eq.Text);
                message.Append("</li>");

                eq.Pending = false;
                eq.DateTimeSent = DateTime.Now;
                _emailRepository.EnsurePersistent(eq);
            }
            message.Append("</ul>");

            var sgMessage = SendGrid.GenerateInstance();
            sgMessage.From = new MailAddress(SendGridFrom, "UCD PrePurchasing No Reply");

            sgMessage.Subject = pendingForUser.Count == 1
                                    ? string.Format("PrePurchasing Notification for Order #{0}",
                                                    pendingForUser.Single().Order.RequestNumber)
                                    : "PrePurchasing Notifications";

            sgMessage.AddTo(email);
            sgMessage.Html = message.ToString();

            var transport = SMTP.GenerateInstance(new NetworkCredential(_sendGridUserName, _sendGridPassword));
            transport.Deliver(sgMessage);

            _emailRepository.DbContext.CommitTransaction();
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
