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
        /// <remarks>
        /// Includes approval actions by approvers and account managers as well as processing by Purchaser
        /// </remarks>
        /// <param name="order">Order being updated</param>
        /// <param name="user">User performing the approval action</param>
        /// <param name="orderStatusCode">The status code that is being completed</param>
        /// <param name="approved">Whether to send a approved or denied message</param>
        void ApprovalDecision(Order order, User user, OrderStatusCode orderStatusCode, bool approved);

        /// <summary>
        /// Order is cancelled
        /// </summary>
        /// <remarks>User does not have an option to opt-out of these emails, this one is mandatory</remarks>
        /// <param name="order">Order being cancelled</param>
        /// <param name="user">User performing the cancel action</param>
        /// <param name="orderStatusCode">The current status code of the order, the level the order is being cancelled.</param>
        void OrderCancelled(Order order, User user, OrderStatusCode orderStatusCode);

        /// <summary>
        /// Order has been updated from Kuali with a new status there
        /// </summary>
        /// <param name="order"></param>
        void KualiUpdate(Order order);

        /// <summary>
        /// Order has been changed by a user.
        /// </summary>
        /// <param name="order">Order that has been changed</param>
        /// <param name="user">User performing the change</param>
        void OrderChanged (Order order, User user);

        /// <summary>
        /// Order submission confirmation message, should only be called when user submits an order
        /// </summary>
        /// <remarks>
        /// Users do not get the option to opt out of this email
        /// </remarks>
        /// <param name="order"></param>
        void OrderSubmitted(Order order);
    }

    public class NotificationService : INotificationService
    {
        /* strings to be used in the messages */
        private const string ApprovalMessage = "Order request #{0}, has been {2} by {3} for {4} review.";
        private const string CancellationMessage = "Order request #{0} has been cancelled by {1} at {2} review.";
        private const string UpdateInKualiMessage = "Order request #{0} has been updated in Kuali to {1}";
        private const string ChangeMessage = "Order request #{0} has been changed by {1}";
        private const string SubmissionMessage = "Order request #{0} has been submitted.";

        private enum EventCode
        {
            Approval, Update, Cancelled, KualiUpdate
        }

        /* private helper properties */
        private readonly IRepository<EmailQueue> _emailQueueRepository;
        private readonly IRepository<Approval> _approvalRepository;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferencesRepository;

        public NotificationService(IRepository<EmailQueue> emailQueueRepository, IRepository<Approval> approvalRepository, IRepositoryWithTypedId<EmailPreferences, string> emailPreferencesRepository  )
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

            QueueEmails(order, EventCode.Approval, user, orderStatusCode, approved);
        }

        public void OrderCancelled(Order order, User user, OrderStatusCode orderStatusCode)
        {
            Check.Require(order != null, "order is required.");
            Check.Require(user != null, "user is required.");
            Check.Require(orderStatusCode != null, "orderStatusCode is required.");

            QueueEmails(order, EventCode.Cancelled, user, orderStatusCode);
        }

        public void KualiUpdate(Order order)
        {
            Check.Require(order != null, "order is required.");

            

            throw new NotImplementedException();
        }

        public void OrderChanged(Order order, User user)
        {
            throw new NotImplementedException();
        }

        public void OrderSubmitted(Order order)
        {
            Check.Require(order != null, "order is required.");

            // get the user who submitted the order
            var target = order.CreatedBy;

            // get the email preferences
            var preference = _emailPreferencesRepository.GetNullableById(target.Id);

            // build the text
            var txt = string.Format(SubmissionMessage, order.Id);

            // create the email queue object
            var emailQueue = new EmailQueue(order, preference.NotificationType, txt, target);
            _emailQueueRepository.EnsurePersistent(emailQueue);
        }

        private void QueueEmails (Order order, EventCode eventCode, User user, OrderStatusCode currentStatus, bool? approved = null)
        {
            Check.Require(order != null, "order is required.");
            Check.Require(eventCode != null, "eventCode is required.");
            Check.Require(user != null, "user is required.");

            // list of people to notify
            var peopleToNotify = new List<KeyValuePair<User, EmailPreferences>>();

            // go through anyone who has "approved" a document
            foreach (var approval in order.Approvals.Where(a => a.Approved))
            {
                // person we intend to email
                var target = approval.User;

                // get their email preferences
                var preference = _emailPreferencesRepository.GetNullableById(target.Id);

                // check their preferences
                if (!HasOptedOut(user, preference, approval.StatusCode, currentStatus, eventCode, approved))
                {
                    peopleToNotify.Add(new KeyValuePair<User, EmailPreferences>(target, preference));
                }
            }

            string txt = string.Empty;

            switch(eventCode)
            {
                case EventCode.Approval:

                    Check.Require(approved.HasValue, "approved is required.");

                    txt = string.Format(ApprovalMessage, order.Id, approved.Value ? "approved" : "denied", user.FullName, currentStatus.Name);
                    break;
                case EventCode.Cancelled:
                    txt = string.Format(CancellationMessage, order.Id, user.FullName, currentStatus.Name);
                    break;
                case EventCode.Update:
                    break;
                case EventCode.KualiUpdate:
                    break;
            }

            foreach (var person in peopleToNotify)
            {
                var emailQueue = new EmailQueue(order, person.Value.NotificationType, txt, person.Key);
                _emailQueueRepository.EnsurePersistent(emailQueue);
            }
        }

        /// <summary>
        /// Determines if the user has opted out for the current event, taking into cosnideration where they approved at
        /// </summary>
        /// <param name="user">User we are evaluating for the message</param>
        /// <param name="preference">Preferences of the user</param>
        /// <param name="approvalLevel">The User's level in the order's review process</param>
        /// <param name="currentLevel">The level of the event we are looking at</param>
        /// <param name="eventCode">Code for what type of event</param>
        /// <param name="approved">(Optional) for the approval steps</param>
        /// <returns></returns>
        private bool HasOptedOut(User user, EmailPreferences preference, OrderStatusCode approvalLevel, OrderStatusCode currentLevel, EventCode eventCode, bool? approved = null)
        {
            // default to a false, meaning they will get the email
            if (preference == null) return false;

            // approval event or update event
            if (eventCode == EventCode.Approval)
            {
                Check.Require(approved.HasValue, "approved is required.");

                switch (approvalLevel.Id)
                {
                        // user is a requester
                    case OrderStatusCode.Codes.Requester:

                        if (approved.Value)
                        {
                            if (currentLevel.Id == OrderStatusCode.Codes.Approver) return !preference.RequesterApproverApproved;
                            if (currentLevel.Id == OrderStatusCode.Codes.AccountManager) return !preference.RequesterAccountManagerApproved;
                            if (currentLevel.Id == OrderStatusCode.Codes.Purchaser) return !preference.RequesterPurchaserAction;
                        }

                        break;
                        // user is an approver
                    case OrderStatusCode.Codes.Approver:

                        if (approved.Value)
                        {
                            if (currentLevel.Id == OrderStatusCode.Codes.AccountManager) return !preference.ApproverAccountManagerApproved;
                            if (currentLevel.Id == OrderStatusCode.Codes.Purchaser) return !preference.ApproverPurchaserProcessed;
                        }
                        else
                        {
                            if (currentLevel.Id == OrderStatusCode.Codes.AccountManager) return !preference.ApproverAccountManagerDenied;
                        }

                        break;
                    case OrderStatusCode.Codes.AccountManager:

                        if (approved.Value)
                        {
                            if (currentLevel.Id == OrderStatusCode.Codes.Purchaser) return !preference.AccountManagerPurchaserProcessed;
                        }

                        break;
                }

                // default value, opt-in
                return false;
            }
            // change or update of order event
            else if (eventCode == EventCode.Update)
            {
                
            }
            // update from kuali to an order
            else if (eventCode == EventCode.KualiUpdate)
            {
                
            }
           
            // fail-safe, send this email event
            return false;
        }
    }
}