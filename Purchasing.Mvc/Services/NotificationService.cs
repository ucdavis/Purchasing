﻿using System;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Services;
using Serilog;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Mvc.Services
{
    public interface INotificationService
    {
        void OrderApproved(Order order, Approval approval);
        void OrderCreated(Order order);
        void OrderEdited(Order order, User actor);
        void OrderCancelled(Order order, User actor, string cancelReason, OrderStatusCode previousStatus);
        void OrderDenied(Order order, User user, string comment, OrderStatusCode previousStatus);
        void OrderCompleted(Order order, User user);
        void OrderReRouted(Order order, int level, bool assigned = false);
        void OrderReceived(Order order, LineItem lineItem, User actor, decimal quantity, string overrideDescription = null);
        void OrderPaid(Order order, LineItem lineItem, User actor, decimal quantity, string overrideDescription = null);

        void OrderAddAttachment(Order order, User actor);
        void OrderAddNote(Order order, User actor, string comment);


    }

    public class NotificationService : INotificationService
    {
        private readonly IRepositoryWithTypedId<EmailQueue, Guid> _emailRepository;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferenceRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<OrderStatusCode, string> _orderStatusCodeRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly IServerLink _serverLink;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IRepositoryFactory _repositoryFactory;

        private enum EventCode { Approval, Update, Cancelled, Arrival, Complete, Received, Paid }//, KualiUpdate }

        /* strings to be used in the messages */
        //private const string ApprovalMessage = "Order request {0} for {1} has been approved by {2} at {3} review.";
        //private const string CancellationMessage = "Order request {0} for {1} has been cancelled by {2} at {3} review with the following comment \"{4}\".";
        //private const string UpdateInKualiMessage = "Order request {0} for {1} has been updated in Kuali to {2}.";
        //private const string ChangeMessage = "Order request {0} for {1} has been changed by {2}.";
        //private const string SubmissionMessage = "Order request {0} for {1} has been submitted.";
        //private const string ArrivalMessage = "Order request {0} for {1} has arrived at your level ({2}) for review from {3}{4}.";
        //private const string CompleteMessage = "Order request {0} for {1} has been completed by {2}.  Order will be completed as a {3}.";
        //private const string ReceiveMessage = "Order request {0} for {1} has {2} item(s) received.";
        //private const string RerouteMessage = "Order request {0} for {1} has been rerouted to you.";
        //private const string AddAttachmentMessage = "Order request {0} for {1} has a new attachment added by {2}";
        //private const string AddNoteMessage = "Order request {0} for {1} has a new note added by {2}";

        public NotificationService(IRepositoryWithTypedId<EmailQueue, Guid> emailRepository, IRepositoryWithTypedId<EmailPreferences, string> emailPreferenceRepository, IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<OrderStatusCode, string> orderStatusCodeRepository, IUserIdentity userIdentity, IServerLink serverLink, IQueryRepositoryFactory queryRepositoryFactory, IRepositoryFactory repositoryFactory )
        {
            _emailRepository = emailRepository;
            _emailPreferenceRepository = emailPreferenceRepository;
            _userRepository = userRepository;
            _orderStatusCodeRepository = orderStatusCodeRepository;
            _userIdentity = userIdentity;
            _serverLink = serverLink;
            //_serverLink = "prepurchasing.ucdavis.edu";
            _queryRepositoryFactory = queryRepositoryFactory;
            _repositoryFactory = repositoryFactory;
        }

        private EmailPreferences GetEmailPreferences(string userId)
        {
            var pref = _emailPreferenceRepository.GetNullableById(userId);
            if (pref == null)
            {
                var log = Log.ForContext("userId", userId);
                //Log email pref not found
                log.Information("Email Preference not found with GetNullable");                
                pref = _emailPreferenceRepository.Queryable.FirstOrDefault(a => a.Id == userId.Trim().ToLower());
                if (pref == null)
                {
                    //Log that this failed too
                    log.Information("Email Preference not found with FirstOrDefault and Trim.ToLower");                    
                    pref = new EmailPreferences(userId);
                }
            }

            return pref;
        }

        public void OrderApproved(Order order, Approval approval)
        {
            var queues = new List<EmailQueueV2>();

            // go through all the tracking history
            foreach (var appr in order.OrderTrackings.Where(a => a.StatusCode.Level < approval.StatusCode.Level).Select(a => new {User = a.User, StatusCode = a.StatusCode}).Distinct())
            {
                var user = appr.User;
                var preference = GetEmailPreferences(user.Id);

                if (IsMailRequested(preference, appr.StatusCode, approval.StatusCode, EventCode.Approval))
                {
                    var currentUser = _userRepository.GetNullableById(_userIdentity.Current);
                    //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ApprovalMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, currentUser.FullName, approval.StatusCode.Name), user);
                    var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Approved", string.Format("By {0} at {1} review.", currentUser.FullName, approval.StatusCode.Name), user);
                    AddToQueue(queues, emailQueue2);
                }

            }

            AddQueuesToOrder(order, queues);

            // is the current level complete?
            if (!order.Approvals.Where(a => a.StatusCode.Level == order.StatusCode.Level && !a.Completed).Any())
            {
                // look forward to the next level
                var level = order.StatusCode.Level + 1; 

                ProcessArrival(order, approval, level);
            }

        }

        public void OrderDenied(Order order, User user, string comment, OrderStatusCode previousStatus)
        {
            var queues = new List<EmailQueueV2>();

            foreach (var appr in order.OrderTrackings.Select(a => new { User = a.User, StatusCode = a.StatusCode }).Distinct())
            {
                var target = appr.User;
                var preference = GetEmailPreferences(user.Id);

                //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(CancellationMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, user.FullName, order.StatusCode.Name, comment), target);
                var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Denied", string.Format("By {0} at {1} review with the following comment \"{2}\".", user.FullName, previousStatus.Name, comment), target);
                //order.AddEmailQueue(emailQueue);
                AddToQueue(queues, emailQueue2);
            }

            AddQueuesToOrder(order, queues);
        }

        /// <summary>
        /// Tested Feb 17, 2012
        /// </summary>
        /// <param name="order"></param>
        public void OrderCreated(Order order)
        {
            var user = order.CreatedBy;
            var preference = GetEmailPreferences(user.Id);

            if(preference.RequesterOrderSubmission)
            {
                //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(SubmissionMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name), user);
                var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Submitted", null, user);
                order.AddEmailQueue(emailQueue2);
            }

            // add the approvers/account managers that are "next" to see if they want the "arrival" email
            // get the lowest level 
            var level = order.Approvals.Where(a => !a.Completed).Min(a => a.StatusCode.Level);

            ProcessArrival(order, null, level);
        }

        public void OrderCompleted(Order order, User user)
        {
            var queues = new List<EmailQueueV2>();

            foreach (var approval in order.OrderTrackings.Select(a => new { a.User, a.StatusCode }).Distinct())
            {
                var preference = GetEmailPreferences(approval.User.Id);

                if (IsMailRequested(preference, approval.StatusCode, order.StatusCode, EventCode.Approval))
                {
                    //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(CompleteMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, user.FullName, order.OrderType.Name), approval.User);
                    var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Completed", string.Format("By {0} as a {1}.", user.FullName, order.OrderType.Name), approval.User);
                    AddToQueue(queues, emailQueue2);
                }
            }

            AddQueuesToOrder(order, queues);
        }

        public void OrderReRouted (Order order, int level, bool assigned = false)
        {
            ProcessArrival(order, null, level, assigned);
        }

        public void OrderReceived(Order order, LineItem lineItem, User actor, decimal quantity, string overrideDescription = null)
        {
            var queues = new List<EmailQueueV2>();

            var description = overrideDescription;
            if (string.IsNullOrWhiteSpace(description))
            {
                description = string.Format("{0} item(s) by {1}.", quantity, actor.FullName);
            }

            // get the order's purchaser
            // var purchasers = order.OrderTrackings.Where(a => a.StatusCode.Id == OrderStatusCode.Codes.Complete).ToList();

            if (!string.IsNullOrEmpty(order.Workgroup.NotificationEmailList))
            {
                //var emailQueue = new EmailQueue(order, EmailPreferences.NotificationTypes.PerEvent, string.Format(ReceiveMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, quantity), actor, order.Workgroup.NotificationEmailList);
                var emailQueue2 = new EmailQueueV2(order, EmailPreferences.NotificationTypes.PerEvent, "Received", description, null, order.Workgroup.NotificationEmailList);
                AddToQueue(queues, emailQueue2);
            }
            
            foreach (var approval in order.OrderTrackings.Select(a => new { a.User, a.StatusCode }).Distinct())
            {
                var preference = GetEmailPreferences(approval.User.Id);

                if (IsMailRequested(preference, approval.StatusCode, order.StatusCode, EventCode.Received, order.OrderType))
                {
                    //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ReceiveMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, quantity), approval.User);
                    var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Received", description, approval.User);
                    AddToQueue(queues, emailQueue2);
                }
            }

            AddQueuesToOrder(order, queues);
        }

        public void OrderPaid(Order order, LineItem lineItem, User actor, decimal quantity, string overrideDescription = null)
        {
            var queues = new List<EmailQueueV2>();

            var description = overrideDescription;
            if (string.IsNullOrWhiteSpace(description))
            {
                description = string.Format("{0} item(s) by {1}.", quantity, actor.FullName);
            }

            if (!string.IsNullOrEmpty(order.Workgroup.NotificationEmailList))
            {                
                var emailQueue2 = new EmailQueueV2(order, EmailPreferences.NotificationTypes.PerEvent, "Paid", description, null, order.Workgroup.NotificationEmailList);
                AddToQueue(queues, emailQueue2);
            }

            foreach (var approval in order.OrderTrackings.Select(a => new { a.User, a.StatusCode }).Distinct())
            {
                var preference = GetEmailPreferences(approval.User.Id);

                if (IsMailRequested(preference, approval.StatusCode, order.StatusCode, EventCode.Paid, order.OrderType))
                {                    
                    var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Paid", description, approval.User);
                    AddToQueue(queues, emailQueue2);
                }
            }

            AddQueuesToOrder(order, queues);
        }



        public void ProcessArrival(Order order, Approval approval, int level, bool assigned = false)
        {
            // find all the approvals at the next level
            var future = order.Approvals.Where(a => a.StatusCode.Level == level);

            // check for any approvals not specifically assigned to anyone
            var wrkgrp = future.Any(a => a.User == null);

            // get the current user
            var currentUser = _userRepository.GetNullableById(_userIdentity.Current);

            var queues = new List<EmailQueueV2>();

            // there is at least one that is workgroup permissions
            if (wrkgrp)
            {

                var peeps = order.Workgroup.Permissions.Where(a => a.Role.Level == level && (!a.IsAdmin || (a.IsAdmin && a.IsFullFeatured))).Select(a => a.User);

                var apf = future.First(a => a.User == null);

                foreach (var peep in peeps)
                {
                    if (!peep.IsActive) //Not active, we don't want to email them
                    {
                        continue;
                    }
                    var preference = GetEmailPreferences(peep.Id);

                    if (IsMailRequested(preference, apf.StatusCode, approval != null ? approval.StatusCode : null, EventCode.Arrival))
                    {
                        var extraInfo = string.Empty;
                        if (preference.ShowAccountInEmail && apf.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
                        {
                            extraInfo = string.Format(" with accounts {0}", order.AccountNumbers);
                        }
                        //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ArrivalMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, apf.StatusCode.Name, currentUser.FullName, extraInfo), peep);
                        var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Arrived", string.Format("At your level ({0}) for review from {1}{2}.", apf.StatusCode.Name, currentUser.FullName, extraInfo), peep);
                        AddToQueue(queues, emailQueue2);
                    }
                }

                // find any that still need to be sent regardless (outside of workgroup)
                var aps = future.Where(a => a.User != null && !peeps.Contains(a.User));

                ProcessApprovalsEmailQueue(order, approval, queues, currentUser, aps, assigned);
            }
            else
            {
                // check each of the approvals
                ProcessApprovalsEmailQueue(order, approval, queues, currentUser, future, assigned);               
            }

            AddQueuesToOrder(order, queues);
        }

        private void ProcessApprovalsEmailQueue(Order order, Approval approval, List<EmailQueueV2> queues, User currentUser, IEnumerable<Approval> aps, bool assigned = false)
        {
            foreach (var ap in aps)
            {
                // load the user and information
                var user = ap.User;

                var preference = GetEmailPreferences(user.Id);

                if (IsMailRequested(preference, ap.StatusCode, approval != null ? approval.StatusCode : null, EventCode.Arrival))
                {
                    var extraInfo = string.Empty;
                    if (preference.ShowAccountInEmail && ap.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
                    {
                        extraInfo = string.Format(" with accounts {0}", order.AccountNumbers);
                    }
                    //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(!assigned ? ArrivalMessage : RerouteMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, ap.StatusCode.Name, currentUser.FullName, extraInfo), ap.User);
                    if (!assigned)
                    {
                        var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Arrived", string.Format("At your level ({0}) for review from {1}{2}.", ap.StatusCode.Name, currentUser.FullName, extraInfo), ap.User);
                        AddToQueue(queues, emailQueue2);
                    }
                    else
                    {
                        var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Rerouted", string.Format("To you at your level ({0}) for review from {1}{2}.", ap.StatusCode.Name, currentUser.FullName, extraInfo), ap.User);
                        AddToQueue(queues, emailQueue2);
                    }

                }

                if (ap.SecondaryUser != null)
                {
                    if (IsMailRequested(preference, ap.StatusCode, approval != null ? approval.StatusCode : null, EventCode.Arrival))
                    {
                        var extraInfo = string.Empty;
                        if (preference.ShowAccountInEmail && ap.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
                        {
                            extraInfo = string.Format(" with accounts {0}", order.AccountNumbers);
                        }
                        //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(!assigned ? ArrivalMessage : RerouteMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, ap.StatusCode.Name, currentUser.FullName, extraInfo), ap.SecondaryUser);
                        if (!assigned)
                        {
                            var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Arrived", string.Format("At your level ({0}) for review from {1}{2}.", ap.StatusCode.Name, currentUser.FullName, extraInfo), ap.SecondaryUser);
                            AddToQueue(queues, emailQueue2);
                        }
                        else
                        {
                            var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Rerouted", string.Format("To you at your level ({0}) for review from {1}{2}.", ap.StatusCode.Name, currentUser.FullName, extraInfo), ap.SecondaryUser);
                            AddToQueue(queues, emailQueue2);
                        }
                    }
                }
            }
        }

        public void OrderEdited(Order order, User actor)
        {
            var queues = new List<EmailQueueV2>();

            // go through all the tracking history
            foreach (var appr in order.OrderTrackings.Where(a => a.StatusCode.Level <= order.StatusCode.Level).Select(a => new { User = a.User, StatusCode = a.StatusCode }).Distinct())
            {
                var user = appr.User;
                var preference = GetEmailPreferences(user.Id);

                if (IsMailRequested(preference, appr.StatusCode, order.StatusCode, EventCode.Update))
                {
                    //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ChangeMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, actor.FullName), user);
                    var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Changed", string.Format("By {0} at {1} review.", actor.FullName, order.StatusCode.Name), user);
                    //order.AddEmailQueue(emailQueue);
                    AddToQueue(queues, emailQueue2);
                }

            }

            AddQueuesToOrder(order, queues);
        }

        public void OrderCancelled(Order order, User actor, string cancelReason, OrderStatusCode previousStatus)
        {
            var user = order.CreatedBy;
            var preference = _emailPreferenceRepository.GetNullableById(user.Id);
            var notificationType = EmailPreferences.NotificationTypes.PerEvent;

            if (preference != null) { notificationType = preference.NotificationType; }

            //var emailQueue = new EmailQueue(order, notificationType, string.Format(CancellationMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, actor.FullName, order.StatusCode.Name, cancelReason), user);
            var emailQueue2 = new EmailQueueV2(order, notificationType, "Cancelled", string.Format("By {0} at {1} review with the comment  \"{2}\".", actor.FullName, previousStatus.Name, cancelReason), user);
            order.AddEmailQueue(emailQueue2);
        }

        public void OrderAddAttachment(Order order, User actor)
        {
            var users = order.OrderTrackings.Select(a => a.User).Distinct().ToList();
            foreach (var ot in users)
            {
                if (!ot.IsActive) //Not active, we don't want to email them
                {
                    continue;
                }
                var preference = GetEmailPreferences(ot.Id);

                if (preference.AddAttachment)
                {
                    //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(AddAttachmentMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, actor.FullName), ot);
                    var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Attachment Added", string.Format("By {0}.", actor.FullName), ot);
                    order.AddEmailQueue(emailQueue2);
                }
            }
        }

        public void OrderAddNote(Order order, User actor, string comment)
        {
            var users = order.OrderTrackings.Select(a => a.User).Distinct().ToList();
            //The above gets users who have acted on the order, but adding a note doesn't add to this list.
            //So, we also need to grab the user who may be currently assigned to this order and check the level to see if it matches.
            //Do we want to only grab it if a specific user is assigned, or do we want to grab all users who are at the same level if it is anyone?           

            var approval = order.Approvals.Where(a => a.StatusCode.Level == order.StatusCode.Level);
            users.AddRange(approval.Where(a => a.User != null).Select(a => a.User));
            //Remove duplicates
            users = users.Distinct().ToList();

            var peeps = new List<User>();
            if (approval.Any(a => a.User == null))
            {
                //Ok, there was an approval with no one assigned, so lets get all the users that can act on the order.
                peeps = order.Workgroup.Permissions.Where(a => a.Role.Level == order.StatusCode.Level && (!a.IsAdmin || (a.IsAdmin && a.IsFullFeatured))).Select(a => a.User).ToList();
                //remove any peeps that are in users  or are null
                peeps = peeps.Where(a => !users.Contains(a) && a != null).ToList();
            }

            //peeps should be empty or only contain users that are not in users

            users.AddRange(peeps);

            //var count = users.Count();
            //users = users.Distinct().ToList();
            //if(users.Count() != count)
            //{
            //    //Log that we had duplicates
            //    var log = Log.ForContext("userId", actor.Id);
            //    log.Information("Duplicate users found in OrderAddNote");
            //}


            var shortComment = comment ?? string.Empty;
            if (comment != null && comment.Length > 100)
            {
                shortComment = comment.Substring(0, 100) + "...";
            }


            foreach (var ot in users)
            {
                if (!ot.IsActive) //Not active, we don't want to email them
                {
                    continue;
                }
                var preference = GetEmailPreferences(ot.Id);

                if (preference.AddNote)
                {
                    if(peeps.Contains(ot))
                    {
                        //This user is in the workgroup, but not assigned to the order.  We need to check if they want to see notes for orders they are not assigned to.
                        if (!preference.IncludeNotesNotAssigned)
                        {
                            continue;
                        }
                    }

                    //var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(AddNoteMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, actor.FullName), ot);
                    var emailQueue2 = new EmailQueueV2(order, preference.NotificationType, "Note Added", string.Format("By {0} with the note \"{1}\".", actor.FullName, shortComment), ot);
                    order.AddEmailQueue(emailQueue2);
                }
            }
        }

        /// <summary>
        /// Determines if user has opted out of a selected email
        /// </summary>
        /// <param name="preference"></param>
        /// <param name="role"></param>
        /// <param name="currentStatus"></param>
        /// <param name="eventCode"></param>
        /// <returns>True if should receive email, False if should not receive email</returns>
        private bool IsMailRequested(EmailPreferences preference, OrderStatusCode role, OrderStatusCode currentStatus, EventCode eventCode, OrderType orderType = null)
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

                                // this technically doesn't exist, gets completed at purchaser level
                                //case OrderStatusCode.Codes.Purchaser: return preference.RequesterPurchaserAction;

                                //case OrderStatusCode.Codes.Complete: return preference.RequesterKualiApproved;  //Done: OrderStatusCode.Codes.Complete (Kuali Approved) 

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

                        case EventCode.Cancelled:

                            // there is no option, user always receives this event
                            return true;

                        case EventCode.Complete:

                            return preference.RequesterPurchaserAction;

                            //case EventCode.KualiUpdate:

                            //    //TODO: add in kuali stuff

                            //    break;

                        case EventCode.Received: 
                            return preference.RequesterReceived;
                        case EventCode.Paid:
                            return preference.RequesterPaid;
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
                                //case OrderStatusCode.Codes.Complete: return preference.ApproverKualiApproved; //Done: OrderStatusCode.Codes.Complete (Kuali Approved) or Request Completed (Look at Email Preferences Page) ?
                                case OrderStatusCode.Codes.Complete: return preference.ApproverPurchaserProcessed;

                                default: return false;
                            }

                        case EventCode.Update:

                            // this email is turned off, no email exists
                            return false;

                        case EventCode.Cancelled:

                            // this email is turned off, no email exists
                            return false;

                        // this technically doesn't exist, since a "complete" order is an approval at purchaser level, see switch statement in approval event.
                        case EventCode.Complete:

                            return preference.ApproverPurchaserProcessed;

                        //case EventCode.KualiUpdate:

                        //    //TODO: add in kuali stuff

                        //    break;

                        case EventCode.Arrival:

                            return preference.ApproverOrderArrive;

                        // no received status for approver
                        case EventCode.Received:
                            return false;
                        case EventCode.Paid:
                            return false;

                        default: return false;
                    }

                case OrderStatusCode.Codes.ConditionalApprover:
                    //Copied code from Approver above... Otherwise this was always returning true...
                    // evaluate the event
                    switch (eventCode)
                    {
                        case EventCode.Approval:

                            switch (currentStatus.Id)
                            {
                                case OrderStatusCode.Codes.AccountManager: return preference.ApproverAccountManagerApproved;
                                case OrderStatusCode.Codes.Purchaser: return preference.ApproverPurchaserProcessed;
                                //case OrderStatusCode.Codes.Complete: return preference.ApproverKualiApproved; //Done: OrderStatusCode.Codes.Complete (Kuali Approved) or Request Completed (Look at Email Preferences Page) ?
                                case OrderStatusCode.Codes.Complete: return preference.ApproverPurchaserProcessed;

                                default: return false;
                            }

                        case EventCode.Update:

                            // this email is turned off, no email exists
                            return false;

                        case EventCode.Cancelled:

                            // this email is turned off, no email exists
                            return false;

                        // this technically doesn't exist, since a "complete" order is an approval at purchaser level, see switch statement in approval event.
                        case EventCode.Complete:

                            return preference.ApproverPurchaserProcessed;

                        //case EventCode.KualiUpdate:

                        //    //TODO: add in kuali stuff

                        //    break;

                        case EventCode.Arrival:

                            return preference.ApproverOrderArrive;

                        // no received status for approver
                        case EventCode.Received:
                            return false;
                        case EventCode.Paid:
                            return false;

                        default: return false;
                    }

                case OrderStatusCode.Codes.AccountManager:

                    switch (eventCode)
                    {
                        case EventCode.Approval:

                            switch (currentStatus.Id)
                            {
                                case OrderStatusCode.Codes.Purchaser: return preference.AccountManagerPurchaserProcessed;
                                case OrderStatusCode.Codes.Complete: return preference.AccountManagerPurchaserProcessed; //Done: OrderStatusCode.Codes.Complete (Kuali Approved) or Request Completed (Look at Email Preferences Page) ?
                                default: return true;
                            }

                        case EventCode.Update:

                            // no email exists
                            return false;

                        case EventCode.Cancelled:

                            // no email exists
                            return false;

                        case EventCode.Complete:

                            return preference.AccountManagerPurchaserProcessed;

                        //case EventCode.KualiUpdate:

                        //    //TODO: Add in kuali stuff

                        //    break;

                        case EventCode.Arrival:

                            return preference.AccountManagerOrderArrive;

                        // account manager doesn't have any emails fror received.
                        case EventCode.Received:
                            return false;
                        case EventCode.Paid:
                            return false;

                        default: return false;
                    }

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

                        //case EventCode.KualiUpdate:
                        //    //TODO: Add in Kuali Stuff
                        //    break;

                        case EventCode.Arrival:

                            return preference.PurchaserOrderArrive;

                        case EventCode.Received:

                            switch (orderType.Id)
                            {
                                case "KFS": return preference.PurchaserKfsItemReceived;
                                case "AE": return preference.PurchaserKfsItemReceived;
                                case "PC": return preference.PurchaserPCardItemReceived;
                                case "CS": return preference.PurchaserCampusServicesItemReceived;
                                default: return false;
                            }
                        case EventCode.Paid:

                            switch (orderType.Id)
                            {
                                case "KFS": return preference.PurchaserKfsItemPaid;
                                case "AE": return preference.PurchaserKfsItemPaid;
                                case "PC": return preference.PurchaserPCardItemPaid;
                                case "CS": return preference.PurchaserCampusServicesItemPaid;
                                default: return false;
                            }


                        default: return false;
                    }
            }

            // default receive email
            return true;
        }

        /// <summary>
        /// Add email queue to a list, but check to ensure no duplicates
        /// </summary>
        /// <param name="emailQueues"></param>
        /// <param name="emailQueue"></param>
        private void AddToQueue(List<EmailQueueV2> emailQueues, EmailQueueV2 emailQueue)
        {
            if (emailQueue.User != null && !emailQueue.User.IsActive)
            {
                return;                
            }
            if (!emailQueues.Any(a => a.User == emailQueue.User)) emailQueues.Add(emailQueue);
        }

        /// <summary>
        /// Copy emailqueues into the order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="emailQueues"></param>
        private void AddQueuesToOrder(Order order, List<EmailQueueV2> emailQueues )
        {
            foreach (var eq in emailQueues)
            {
                order.AddEmailQueue(eq);
            }
        }

        private string GenerateLink(string address, string orderRequestNumber)
        {
            return string.Format("<a href=\"{0}{1}\">{1}</a>", "https://prepurchasing.ucdavis.edu/Order/Lookup/", orderRequestNumber);
        }
    }
}