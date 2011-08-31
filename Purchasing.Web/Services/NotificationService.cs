using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Services
{
    public interface INotificationService
    {
        /// <summary>
        /// Create an email queue object for an approval decision
        /// </summary>
        /// <param name="order">Order being updated</param>
        /// <param name="user">User performing the approval action</param>
        /// <param name="orderStatusCode">The status code that is being completed</param>
        /// <param name="approved">Whether to send a approved or denied message</param>
        void ApprovalDecision(Order order, User user, OrderStatusCode orderStatusCode, bool approved);

        //void KualiUpdate(Order order);
    }

    public class NotificationService : INotificationService
    {
        /* strings to be used in the messages */
        private const string ApprovalMessage = "Order request #{0}, has been {2} by {3} for {4} review.";

        /* private helper properties */
        private readonly IRepository<EmailQueue> _emailQueueRepository;
        private readonly IRepository<Approval> _approvalRepository;
        private readonly IRepository<EmailPreferences> _emailPreferencesRepository;

        public NotificationService(IRepository<EmailQueue> emailQueueRepository, IRepository<Approval> approvalRepository, IRepository<EmailPreferences> emailPreferencesRepository  )
        {
            _emailQueueRepository = emailQueueRepository;
            _approvalRepository = approvalRepository;
            _emailPreferencesRepository = emailPreferencesRepository;
        }

        public void ApprovalDecision(Order order, User user, OrderStatusCode orderStatusCode, bool approved)
        {
            Check.Require(order != null, "order is required.");
            Check.Require(user != null, "user is required.");
            Check.Require(orderStatusCode != null, "orderStatusCode is required.");

            // determine if the user has opted to receive this notification and get the type
            var peopleToNotify = new List<KeyValuePair<User, EmailPreferences>>();

            // take the level we're at and go down
            foreach (var approval in order.Approvals.Where(a => a.Approved).OrderByDescending(a => a.Level))
            {
                // get the person
                var target = approval.User;

                var preference = _emailPreferencesRepository.Queryable.Where(a => a.User == target).FirstOrDefault();

                // check their preferences on this event
                if (HasOptedIn(target, preference, approval.StatusCode, orderStatusCode)) peopleToNotify.Add(new KeyValuePair<User, EmailPreferences>(user, preference));
            }

            var txt = string.Format(ApprovalMessage, order.Id, approved ? "approved" : "denied", user.FullName, orderStatusCode.Name);

            foreach (var person in peopleToNotify)
            {
                var emailQueue = new EmailQueue(order, person.Value.NotificationType, txt, person.Key);
                _emailQueueRepository.EnsurePersistent(emailQueue);
            }
        }

        /// <summary>
        /// Determines if the user has opted in for the current event, taking into cosnideration where they approved at
        /// </summary>
        /// <param name="user"></param>
        /// <param name="preference"></param>
        /// <param name="approvalLevel"></param>
        /// <param name="currentLevel"></param>
        /// <returns></returns>
        private bool HasOptedIn(User user, EmailPreferences preference, OrderStatusCode approvalLevel, OrderStatusCode currentLevel)
        {
            if (preference == null) return false;

            // evaluate their preferences here

            return true;
        }
    }
}