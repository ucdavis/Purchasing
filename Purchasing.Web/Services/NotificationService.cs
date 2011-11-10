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
        void OrderApproved(Order order, Approval approval);
        void OrderCreated(Order order);
        void OrderEdited(Order order, User actor);
        void OrderCancelled(Order order, User actor);
    }

    public class NotificationService : INotificationService
    {
        private readonly IRepositoryWithTypedId<EmailQueue, Guid> _emailRepository;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferenceRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IUserIdentity _userIdentity;

        private enum EventCode { Approval, Update, Cancelled, KualiUpdate }

        /* strings to be used in the messages */
        private const string ApprovalMessage = "Order request #{0}, has been approved by {1} at {2} review.";
        private const string CancellationMessage = "Order request #{0} has been cancelled by {1} at {2} review.";
        private const string UpdateInKualiMessage = "Order request #{0} has been updated in Kuali to {1}";
        private const string ChangeMessage = "Order request #{0} has been changed by {1}";
        private const string SubmissionMessage = "Order request #{0} has been submitted.";

        public NotificationService(IRepositoryWithTypedId<EmailQueue, Guid> emailRepository, IRepositoryWithTypedId<EmailPreferences, string> emailPreferenceRepository, IRepositoryWithTypedId<User, string> userRepository , IUserIdentity userIdentity )
        {
            _emailRepository = emailRepository;
            _emailPreferenceRepository = emailPreferenceRepository;
            _userRepository = userRepository;
            _userIdentity = userIdentity;
        }

        public void OrderApproved(Order order, Approval approval)
        {
            // go through all the tracking history
            foreach (var appr in order.OrderTrackings.Where(a => a.StatusCode.Level <= approval.StatusCode.Level))
            {
                var user = appr.User;
                var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                if (!HasOptedOut(preference, appr.StatusCode, approval.StatusCode, EventCode.Approval))
                {
                    var currentUser = _userRepository.GetNullableById(_userIdentity.Current);
                    var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ApprovalMessage, order.OrderRequestNumber(), currentUser.FullName, approval.StatusCode.Name), user);
                    order.AddEmailQueue(emailQueue);
                }

            }
        }

        public void OrderCreated(Order order)
        {
            //TODO: Check to see if the requestor has opted out.
            var user = order.CreatedBy;
            var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

            if(preference.RequesterOrderSubmission)
            {
                var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(SubmissionMessage, order.OrderRequestNumber()), user);
                order.AddEmailQueue(emailQueue);
            }
        }

        public void OrderEdited(Order order, User actor)
        {
            // go through all the tracking history
            foreach (var appr in order.OrderTrackings.Where(a => a.StatusCode.Level <= order.StatusCode.Level))
            {

                var user = appr.User;
                var preference = _emailPreferenceRepository.GetNullableById(user.Id);
                var notificationType = EmailPreferences.NotificationTypes.PerEvent;

                if (preference != null) { notificationType = preference.NotificationType; }

                if (!HasOptedOut(preference, appr.StatusCode, order.StatusCode, EventCode.Approval))
                {
                    var emailQueue = new EmailQueue(order, notificationType, string.Format(ApprovalMessage, order.OrderRequestNumber(), actor.FullName, order.StatusCode.Name));
                    order.AddEmailQueue(emailQueue);
                }

            }
        }

        public void OrderCancelled(Order order, User actor)
        {
            var user = order.CreatedBy;
            var preference = _emailPreferenceRepository.GetNullableById(user.Id);
            var notificationType = EmailPreferences.NotificationTypes.PerEvent;

            if (preference != null) { notificationType = preference.NotificationType; }

            var emailQueue = new EmailQueue(order, notificationType, string.Format(CancellationMessage, order.OrderRequestNumber(), actor.FullName, order.StatusCode.Name), user);
            order.AddEmailQueue(emailQueue);
        }

        /// <summary>
        /// Determines if user has opted out of a selected email
        /// </summary>
        /// <param name="preference"></param>
        /// <param name="role"></param>
        /// <param name="currentStatus"></param>
        /// <param name="eventCode"></param>
        /// <returns>True if should not receive email, False if should receive email</returns>
        private bool HasOptedOut(EmailPreferences preference, OrderStatusCode role, OrderStatusCode currentStatus, EventCode eventCode)
        {
            // no preference, automatically gets emails
            if (preference == null) return false;

            // what is the role of the user we are inspecting
            switch (role.Id)
            {
                case OrderStatusCode.Codes.Requester:

                    // what event is happening
                    switch (eventCode)
                    {
                        case EventCode.Approval:

                            // evaluate the level that is being handled
                            switch (currentStatus.Id)
                            {
                                case OrderStatusCode.Codes.Approver: return !preference.RequesterApproverApproved;

                                case OrderStatusCode.Codes.ConditionalApprover: return !preference.RequesterApproverApproved;

                                case OrderStatusCode.Codes.AccountManager: return !preference.RequesterAccountManagerApproved;

                                case OrderStatusCode.Codes.Purchaser: return !preference.RequesterPurchaserAction;

                                //TODO: OrderStatusCode.Codes.Complete (Kuali Approved) 

                                default: return false;
                            }


                        case EventCode.Update:

                            switch (currentStatus.Id)
                            {
                                case OrderStatusCode.Codes.Approver: return !preference.RequesterApproverChanged;

                                case OrderStatusCode.Codes.ConditionalApprover: return !preference.RequesterApproverChanged;

                                case OrderStatusCode.Codes.AccountManager: return !preference.RequesterAccountManagerChanged;

                                case OrderStatusCode.Codes.Purchaser: return !preference.RequesterPurchaserChanged;

                                default: return false;
                            }

                            break;
                        case EventCode.Cancelled:

                            // there is no option, user always receives this event
                            return false;

                            break;
                        case EventCode.KualiUpdate:

                            //TODO: add in kuali stuff

                            break;
                    }


                    break;
                case OrderStatusCode.Codes.Approver:

                    // evaluate the event
                    switch (eventCode)
                    {
                        case EventCode.Approval:

                            switch (currentStatus.Id)
                            {
                                case OrderStatusCode.Codes.AccountManager: return !preference.ApproverAccountManagerApproved;

                                case OrderStatusCode.Codes.Purchaser: return !preference.ApproverPurchaserProcessed;

                                //TODO: OrderStatusCode.Codes.Complete (Kuali Approved) or Request Completed (Look at Email Preferences Page) ?

                                default: return false;
                            }

                        case EventCode.Update:

                            // this email is turned off, no email exists
                            return true;

                        case EventCode.Cancelled:

                            // this email is turned off, no email exists
                            return false;

                        case EventCode.KualiUpdate:

                            //TODO: add in kuali stuff

                            break;

                        default: return false;
                    }

                    break;
                case OrderStatusCode.Codes.ConditionalApprover:
                    break;
                case OrderStatusCode.Codes.AccountManager:

                    switch (eventCode)
                    {
                        case EventCode.Approval:

                            switch (currentStatus.Id)
                            {
                                case OrderStatusCode.Codes.Purchaser: return !preference.AccountManagerPurchaserProcessed;
                                //TODO: OrderStatusCode.Codes.Complete (Kuali Approved) or Request Completed (Look at Email Preferences Page) ?
                                default: return false;
                            }

                            break;
                        case EventCode.Update:

                            // no email exists
                            return true;

                            break;
                        case EventCode.Cancelled:

                            // no email exists
                            return true;

                            break;
                        case EventCode.KualiUpdate:

                            //TODO: Add in kuali stuff

                            break;
                        default: return false;
                    }

                    break;
                case OrderStatusCode.Codes.Purchaser:

                    switch (eventCode)
                    {
                        case EventCode.Approval:
                            //TODO: OrderStatusCode.Codes.Complete (Kuali Approved) or Request Completed (Look at Email Preferences Page) ?
                            // no email exists
                            return true;

                        case EventCode.Update:

                            // no email exists
                            return true;

                        case EventCode.Cancelled:

                            // no email exists
                            return true;

                        case EventCode.KualiUpdate:
                            //TODO: Add in Kuali Stuff
                            break;
                        default: return false;
                    }

                    break;
            }



            // default receive email
            return false;
        }

    }

}