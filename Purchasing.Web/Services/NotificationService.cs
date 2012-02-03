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
        void OrderDenied(Order order, User user, string comment);
    }

    public class NotificationService : INotificationService
    {
        private readonly IRepositoryWithTypedId<EmailQueue, Guid> _emailRepository;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferenceRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<OrderStatusCode, string> _orderStatusCodeRepository;
        private readonly IUserIdentity _userIdentity;

        private enum EventCode { Approval, Update, Cancelled, KualiUpdate, Arrival }

        /* strings to be used in the messages */
        private const string ApprovalMessage = "Order request #{0}, has been approved by {1} at {2} review.";
        private const string CancellationMessage = "Order request #{0} has been cancelled by {1} at {2} review.";
        private const string UpdateInKualiMessage = "Order request #{0} has been updated in Kuali to {1}.";
        private const string ChangeMessage = "Order request #{0} has been changed by {1}.";
        private const string SubmissionMessage = "Order request #{0} has been submitted.";
        private const string ArrivalMessage = "Order request #{0} has arrived at your level for review from {1}.";

        public NotificationService(IRepositoryWithTypedId<EmailQueue, Guid> emailRepository, IRepositoryWithTypedId<EmailPreferences, string> emailPreferenceRepository, IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<OrderStatusCode, string> orderStatusCodeRepository, IUserIdentity userIdentity )
        {
            _emailRepository = emailRepository;
            _emailPreferenceRepository = emailPreferenceRepository;
            _userRepository = userRepository;
            _orderStatusCodeRepository = orderStatusCodeRepository;
            _userIdentity = userIdentity;
        }

        public void OrderApproved(Order order, Approval approval)
        {
            // go through all the tracking history
            foreach (var appr in order.OrderTrackings.Where(a => a.StatusCode.Level <= approval.StatusCode.Level))
            {
                var user = appr.User;
                var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                if (IsMailRequested(preference, appr.StatusCode, approval.StatusCode, EventCode.Approval))
                {
                    var currentUser = _userRepository.GetNullableById(_userIdentity.Current);
                    var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ApprovalMessage, order.OrderRequestNumber(), currentUser.FullName, approval.StatusCode.Name), user);
                    order.AddEmailQueue(emailQueue);
                }

            }
            
            // is the current level complete?
            if (!order.Approvals.Where(a => a.StatusCode.Level == order.StatusCode.Level && !a.Completed).Any())
            {
                // look forward to the next level
                var level = order.StatusCode.Level + 1; 

                ProcessArrival(order, approval, level);

                //// find all the approvals at the next level
                //var future = order.Approvals.Where(a => a.StatusCode.Level == level);

                //// check for any approvals not specifically assigned to anyone
                //var wrkgrp = future.Any(a => a.User == null);

                //// there is at least one that is workgroup permissions
                //if (wrkgrp)
                //{
                //    // get the workgroup and all the people at the level
                //    var peeps = order.Workgroup.Permissions.Where(a => a.Role.Level == order.StatusCode.Level).Select(a => a.User);

                //    var apf = future.Where(a => a.User == null).First();

                //    foreach (var peep in peeps)
                //    {
                //        var preference = _emailPreferenceRepository.GetNullableById(peep.Id) ?? new EmailPreferences(peep.Id);

                //        if (IsMailRequested(preference, apf.StatusCode, approval.StatusCode, EventCode.Arrival))
                //        {
                //            var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ArrivalMessage, order.OrderRequestNumber(), order.CreatedBy.FullName), peep);
                //            order.AddEmailQueue(emailQueue);
                //        }
                //    }

                //    // find any that still need to be sent regardless (outside of workgroup)
                //    var aps = future.Where(a => a.User != null && !peeps.Contains(a.User));

                //    foreach (var ap in future)
                //    {
                //        // load the user and information
                //        var user = ap.User;

                //        var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                //        if (IsMailRequested(preference, ap.StatusCode, approval.StatusCode, EventCode.Arrival))
                //        {
                //            var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ArrivalMessage, order.OrderRequestNumber(), order.CreatedBy.FullName), ap.User);
                //            order.AddEmailQueue(emailQueue);
                //        }
                //    }
                //}
                //else
                //{
                //    // check each of the approvals
                //    foreach (var ap in future)
                //    {
                //        // load the user and information
                //        var user = ap.User;

                //        if (user != null)
                //        {
                //            var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                //            if (IsMailRequested(preference, ap.StatusCode, approval.StatusCode, EventCode.Arrival))
                //            {
                //                var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ArrivalMessage, order.OrderRequestNumber(), order.CreatedBy.FullName), ap.User);
                //                order.AddEmailQueue(emailQueue);
                //            }
                //        }
                //    }    
                //}
            }

        }

        public void OrderDenied(Order order, User user, string comment)
        {
            //TODO: impl order denied notification
        }

        public void OrderCreated(Order order)
        {
            var user = order.CreatedBy;
            var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

            if(preference.RequesterOrderSubmission)
            {
                var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(SubmissionMessage, order.OrderRequestNumber()), user);
                order.AddEmailQueue(emailQueue);
            }

            // add the approvers/account managers that are "next" to see if they want the "arrival" email
            // get the lowest level 
            var level = order.Approvals.Where(a => !a.Completed).Min(a => a.StatusCode.Level);

            ProcessArrival(order, null, level);

            //var future = order.Approvals.Where(a => a.StatusCode.Level == level);

            //foreach (var ap in future)
            //{
            //    // load the user and information
            //    var auser = ap.User;

            //    if (auser != null)
            //    {
            //        var apreference = _emailPreferenceRepository.GetNullableById(auser.Id) ?? new EmailPreferences(auser.Id);

            //        if (IsMailRequested(apreference, ap.StatusCode, null, EventCode.Arrival))
            //        {
            //            var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ArrivalMessage, order.OrderRequestNumber(), order.CreatedBy.FullName), ap.User);
            //            order.AddEmailQueue(emailQueue);
            //        }    
            //    }
            //}
        }

        public void ProcessArrival(Order order, Approval approval, int level)
        {
            // find all the approvals at the next level
            var future = order.Approvals.Where(a => a.StatusCode.Level == level);

            // check for any approvals not specifically assigned to anyone
            var wrkgrp = future.Any(a => a.User == null);

            // there is at least one that is workgroup permissions
            if (wrkgrp)
            {
                // get the workgroup and all the people at the level
                var peeps = order.Workgroup.Permissions.Where(a => a.Role.Level == order.StatusCode.Level).Select(a => a.User);

                var apf = future.Where(a => a.User == null).First();

                foreach (var peep in peeps)
                {
                    var preference = _emailPreferenceRepository.GetNullableById(peep.Id) ?? new EmailPreferences(peep.Id);

                    if (IsMailRequested(preference, apf.StatusCode, approval != null ? approval.StatusCode : null, EventCode.Arrival))
                    {
                        var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ArrivalMessage, order.OrderRequestNumber(), order.CreatedBy.FullName), peep);
                        order.AddEmailQueue(emailQueue);
                    }
                }

                // find any that still need to be sent regardless (outside of workgroup)
                var aps = future.Where(a => a.User != null && !peeps.Contains(a.User));

                foreach (var ap in aps)
                {
                    // load the user and information
                    var user = ap.User;

                    var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                    if (IsMailRequested(preference, ap.StatusCode, approval != null ? approval.StatusCode : null, EventCode.Arrival))
                    {
                        var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ArrivalMessage, order.OrderRequestNumber(), order.CreatedBy.FullName), ap.User);
                        order.AddEmailQueue(emailQueue);
                    }
                }
            }
            else
            {
                // check each of the approvals
                foreach (var ap in future)
                {
                    // load the user and information
                    var user = ap.User;

                    if (user != null)
                    {
                        var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                        if (IsMailRequested(preference, ap.StatusCode, approval != null ? approval.StatusCode : null, EventCode.Arrival))
                        {
                            var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ArrivalMessage, order.OrderRequestNumber(), order.CreatedBy.FullName), ap.User);
                            order.AddEmailQueue(emailQueue);
                        }
                    }
                }
            }
        }

        public void OrderEdited(Order order, User actor)
        {
            // go through all the tracking history
            foreach (var appr in order.OrderTrackings.Where(a => a.StatusCode.Level <= order.StatusCode.Level))
            {
                var user = appr.User;
                var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                if (IsMailRequested(preference, appr.StatusCode, order.StatusCode, EventCode.Update))
                {
                    var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ChangeMessage, order.OrderRequestNumber(), actor.FullName), user);
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
        /// <returns>True if should receive email, False if should not receive email</returns>
        private bool IsMailRequested(EmailPreferences preference, OrderStatusCode role, OrderStatusCode currentStatus, EventCode eventCode)
        {
            // no preference, automatically gets emails
            if (preference == null) return true;

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
                                case OrderStatusCode.Codes.Approver: return preference.RequesterApproverApproved;

                                case OrderStatusCode.Codes.ConditionalApprover: return preference.RequesterApproverApproved;

                                case OrderStatusCode.Codes.AccountManager: return preference.RequesterAccountManagerApproved;

                                case OrderStatusCode.Codes.Purchaser: return preference.RequesterPurchaserAction;

                                case OrderStatusCode.Codes.Complete: return preference.RequesterKualiApproved;  //Done: OrderStatusCode.Codes.Complete (Kuali Approved) 
                              

                                default: return true;
                            }


                        case EventCode.Update:

                            switch (currentStatus.Id)
                            {
                                case OrderStatusCode.Codes.Approver: return preference.RequesterApproverChanged;

                                case OrderStatusCode.Codes.ConditionalApprover: return preference.RequesterApproverChanged;

                                case OrderStatusCode.Codes.AccountManager: return preference.RequesterAccountManagerChanged;

                                case OrderStatusCode.Codes.Purchaser: return preference.RequesterPurchaserChanged;

                                default: return true;
                            }

                            break;
                        case EventCode.Cancelled:

                            // there is no option, user always receives this event
                            return true;

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
                                case OrderStatusCode.Codes.AccountManager: return preference.ApproverAccountManagerApproved;
                                case OrderStatusCode.Codes.Purchaser: return preference.ApproverPurchaserProcessed;
                                case OrderStatusCode.Codes.Complete: return preference.ApproverKualiApproved; //Done: OrderStatusCode.Codes.Complete (Kuali Approved) or Request Completed (Look at Email Preferences Page) ?

                                default: return false;
                            }

                        case EventCode.Update:

                            // this email is turned off, no email exists
                            return false;

                        case EventCode.Cancelled:

                            // this email is turned off, no email exists
                            return false;

                        case EventCode.KualiUpdate:

                            //TODO: add in kuali stuff

                            break;

                        case EventCode.Arrival:

                            return preference.ApproverOrderArrive;

                        default: return false;
                    }

                    break;
                case OrderStatusCode.Codes.ConditionalApprover:

                    // is this supposed to be the same as approver?

                    break;
                case OrderStatusCode.Codes.AccountManager:

                    switch (eventCode)
                    {
                        case EventCode.Approval:

                            switch (currentStatus.Id)
                            {
                                case OrderStatusCode.Codes.Purchaser: return preference.AccountManagerPurchaserProcessed;
                                case OrderStatusCode.Codes.Complete: return preference.AccountManagerKualiApproved; //Done: OrderStatusCode.Codes.Complete (Kuali Approved) or Request Completed (Look at Email Preferences Page) ?
                                default: return true;
                            }

                            break;
                        case EventCode.Update:

                            // no email exists
                            return false;

                            break;
                        case EventCode.Cancelled:

                            // no email exists
                            return false;

                            break;
                        case EventCode.KualiUpdate:

                            //TODO: Add in kuali stuff

                            break;

                        case EventCode.Arrival:

                            return preference.AccountManagerOrderArrive;

                        default: return false;
                    }

                    break;
                case OrderStatusCode.Codes.Purchaser:

                    switch (eventCode)
                    {
                        case EventCode.Approval:
                            switch (currentStatus.Id)
                            {
                                case OrderStatusCode.Codes.Complete: return preference.PurchaserKualiApproved;  //Done: OrderStatusCode.Codes.Complete (Kuali Approved) or Request Completed (Look at Email Preferences Page) ?
                            }
                           
                            // no email exists
                            return false;

                        case EventCode.Update:

                            // no email exists
                            return false;

                        case EventCode.Cancelled:

                            // no email exists
                            return false;

                        case EventCode.KualiUpdate:
                            //TODO: Add in Kuali Stuff
                            break;

                        case EventCode.Arrival:

                            return preference.PurchaserOrderArrive;

                        default: return false;
                    }

                    break;
            }

            // default receive email
            return true;
        }

    }

}