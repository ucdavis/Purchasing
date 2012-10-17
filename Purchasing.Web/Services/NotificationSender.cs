﻿using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Purchasing.Core.Domain;
using SendGridMail;
using SendGridMail.Transport;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Services
{
    public interface INotificationSender
    {
        void SendNotifications();
        void SendDailyWeeklyNotifications();
    }

    public class NotificationSender : INotificationSender
    {
        private readonly IRepositoryWithTypedId<EmailQueue, Guid> _emailRepository;
        private static readonly string _sendGridUserName = ConfigurationManager.AppSettings["SendGridUserName"];
        private static readonly string _sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"];
        private const string _sendGridFrom = "opp-noreply@ucdavis.edu";

        public NotificationSender(IRepositoryWithTypedId<EmailQueue, Guid> emailRepository)
        {
            _emailRepository = emailRepository;
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
                sgMessage.From = new MailAddress(_sendGridFrom, "OPP No Reply");
                sgMessage.Subject = "PrePurchasing Notifications";
                sgMessage.AddTo(email);
                sgMessage.Html = message.ToString();

                var transport = REST.GetInstance(new NetworkCredential(_sendGridUserName, _sendGridPassword));
                transport.Deliver(sgMessage);
            }

            // if we got here...assuming success!
            foreach (var eq in pending)
            {
                eq.Pending = false;
                _emailRepository.EnsurePersistent(eq);
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
    }
}