using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core;
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
        void OrderCancelled(Order order, User actor, string cancelReason);
        void OrderDenied(Order order, User user, string comment);
        void OrderCompleted(Order order, User user);
        void OrderReRouted(Order order, int level, bool assigned = false);
        void OrderReceived(Order order, LineItem lineItem, User actor);
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

        private enum EventCode { Approval, Update, Cancelled, Arrival, Complete, Received }//, KualiUpdate }

        /* strings to be used in the messages */
        private const string ApprovalMessage = "Order request {0} for {1} has been approved by {2} at {3} review.";
        private const string CancellationMessage = "Order request {0} for {1} has been cancelled by {2} at {3} review with the following comment \"{4}\".";
        private const string UpdateInKualiMessage = "Order request {0} for {1} has been updated in Kuali to {2}.";
        private const string ChangeMessage = "Order request {0} for {1} has been changed by {2}.";
        private const string SubmissionMessage = "Order request {0} for {1} has been submitted.";
        private const string ArrivalMessage = "Order request {0} for {1} has arrived at your level ({2}) for review from {3}.";
        private const string CompleteMessage = "Order request {0} for {1} has been completed by {2}.  Order will be completed as a {3}.";
        private const string ReceiveMessage = "Order request {0} for {1} has had one or more items received.";
        private const string RerouteMessage = "Order request {0} for {1} has been rerouted to you.";

        public NotificationService(IRepositoryWithTypedId<EmailQueue, Guid> emailRepository, IRepositoryWithTypedId<EmailPreferences, string> emailPreferenceRepository, IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<OrderStatusCode, string> orderStatusCodeRepository, IUserIdentity userIdentity, IServerLink serverLink, IQueryRepositoryFactory queryRepositoryFactory, IRepositoryFactory repositoryFactory )
        {
            _emailRepository = emailRepository;
            _emailPreferenceRepository = emailPreferenceRepository;
            _userRepository = userRepository;
            _orderStatusCodeRepository = orderStatusCodeRepository;
            _userIdentity = userIdentity;
            _serverLink = serverLink;
            _queryRepositoryFactory = queryRepositoryFactory;
            _repositoryFactory = repositoryFactory;
        }

        public void OrderApproved(Order order, Approval approval)
        {
            var queues = new List<EmailQueue>();

            // go through all the tracking history
            foreach (var appr in order.OrderTrackings.Where(a => a.StatusCode.Level < approval.StatusCode.Level).Select(a => new {User = a.User, StatusCode = a.StatusCode}).Distinct())
            {
                var user = appr.User;
                var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                if (IsMailRequested(preference, appr.StatusCode, approval.StatusCode, EventCode.Approval))
                {
                    var currentUser = _userRepository.GetNullableById(_userIdentity.Current);
                    var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ApprovalMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, currentUser.FullName, approval.StatusCode.Name), user);                   
                    AddToQueue(queues, emailQueue);
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

        public void OrderDenied(Order order, User user, string comment)
        {
            var queues = new List<EmailQueue>();

            foreach (var appr in order.OrderTrackings.Select(a => new { User = a.User, StatusCode = a.StatusCode }).Distinct())
            {
                var target = appr.User;
                var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(CancellationMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, user.FullName, order.StatusCode.Name, comment), target);
                //order.AddEmailQueue(emailQueue);
                AddToQueue(queues, emailQueue);
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
            var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

            if(preference.RequesterOrderSubmission)
            {
                var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(SubmissionMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name), user);
                order.AddEmailQueue(emailQueue);
            }

            // add the approvers/account managers that are "next" to see if they want the "arrival" email
            // get the lowest level 
            var level = order.Approvals.Where(a => !a.Completed).Min(a => a.StatusCode.Level);

            ProcessArrival(order, null, level);
        }

        public void OrderCompleted(Order order, User user)
        {
            var queues = new List<EmailQueue>();

            foreach (var approval in order.OrderTrackings.Select(a => new { a.User, a.StatusCode }).Distinct())
            {
                var preference = _emailPreferenceRepository.GetNullableById(approval.User.Id) ?? new EmailPreferences(approval.User.Id);

                if (IsMailRequested(preference, approval.StatusCode, order.StatusCode, EventCode.Approval))
                {
                    var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(CompleteMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, user.FullName, order.OrderType.Name), user);
                    AddToQueue(queues, emailQueue);
                }
            }

            AddQueuesToOrder(order, queues);
        }

        public void OrderReRouted (Order order, int level, bool assigned = false)
        {
            ProcessArrival(order, null, level, assigned);
        }

        public void OrderReceived(Order order, LineItem lineItem, User actor)
        {
            var queues = new List<EmailQueue>();

            // get the order's purchaser
            var purchasers = order.OrderTrackings.Where(a => a.StatusCode.Id == OrderStatusCode.Codes.Complete).ToList();

            foreach (var p in purchasers)
            {
                var preference = _emailPreferenceRepository.GetNullableById(p.User.Id) ?? new EmailPreferences(p.User.Id);

                if (IsMailRequested(preference, p.StatusCode, order.StatusCode, EventCode.Received, order.OrderType))
                {
                    var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ReceiveMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name), p.User);
                    AddToQueue(queues, emailQueue);
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

            var queues = new List<EmailQueue>();

            // there is at least one that is workgroup permissions
            if (wrkgrp)
            {
                var workgroups = FindAdminWorkgroups(order);

                // get the workgroup and all the people at the level
                //var peeps = order.Workgroup.Permissions.Where(a => a.Role.Level == level).Select(a => a.User);

                var peeps = workgroups.SelectMany(a => a.Permissions).Where(a => a.Role.Level == level).Select(a => a.User);

                var apf = future.Where(a => a.User == null).First();

                foreach (var peep in peeps)
                {
                    var preference = _emailPreferenceRepository.GetNullableById(peep.Id) ?? new EmailPreferences(peep.Id);

                    if (IsMailRequested(preference, apf.StatusCode, approval != null ? approval.StatusCode : null, EventCode.Arrival))
                    {
                        var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ArrivalMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, apf.StatusCode.Name, currentUser.FullName), peep);
                        AddToQueue(queues, emailQueue);
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

        private void ProcessApprovalsEmailQueue(Order order, Approval approval, List<EmailQueue> queues, User currentUser, IEnumerable<Approval> aps, bool assigned = false)
        {
            foreach (var ap in aps)
            {
                // load the user and information
                var user = ap.User;

                var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                if (IsMailRequested(preference, ap.StatusCode, approval != null ? approval.StatusCode : null, EventCode.Arrival))
                {
                    var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(!assigned ? ArrivalMessage : RerouteMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, ap.StatusCode.Name, currentUser.FullName), ap.User);
                    AddToQueue(queues, emailQueue);
                }

                if (ap.SecondaryUser != null)
                {
                    if (IsMailRequested(preference, ap.StatusCode, approval != null ? approval.StatusCode : null, EventCode.Arrival))
                    {
                        var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(!assigned ? ArrivalMessage : RerouteMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, ap.StatusCode.Name, currentUser.FullName), ap.SecondaryUser);
                        AddToQueue(queues, emailQueue);
                    }
                }
            }
        }

        public void OrderEdited(Order order, User actor)
        {
            var queues = new List<EmailQueue>();

            // go through all the tracking history
            foreach (var appr in order.OrderTrackings.Where(a => a.StatusCode.Level <= order.StatusCode.Level).Select(a => new { User = a.User, StatusCode = a.StatusCode }).Distinct())
            {
                var user = appr.User;
                var preference = _emailPreferenceRepository.GetNullableById(user.Id) ?? new EmailPreferences(user.Id);

                if (IsMailRequested(preference, appr.StatusCode, order.StatusCode, EventCode.Update))
                {
                    var emailQueue = new EmailQueue(order, preference.NotificationType, string.Format(ChangeMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, actor.FullName), user);
                    //order.AddEmailQueue(emailQueue);
                    AddToQueue(queues, emailQueue);
                }

            }

            AddQueuesToOrder(order, queues);
        }

        public void OrderCancelled(Order order, User actor, string cancelReason)
        {
            var user = order.CreatedBy;
            var preference = _emailPreferenceRepository.GetNullableById(user.Id);
            var notificationType = EmailPreferences.NotificationTypes.PerEvent;

            if (preference != null) { notificationType = preference.NotificationType; }

            var emailQueue = new EmailQueue(order, notificationType, string.Format(CancellationMessage, GenerateLink(_serverLink.Address, order.OrderRequestNumber()), order.Vendor == null ? "Unspecified Vendor" : order.Vendor.Name, actor.FullName, order.StatusCode.Name, cancelReason), user);
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

                        case EventCode.Complete:

                            return preference.ApproverPurchaserProcessed;

                        //case EventCode.KualiUpdate:

                        //    //TODO: add in kuali stuff

                        //    break;

                        case EventCode.Arrival:

                            return preference.ApproverOrderArrive;

                        default: return false;
                    }

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
                                case "PC": return preference.PurchaserPCardItemReceived;
                                case "CS": return preference.PurchaserCampusServicesItemReceived;
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
        private void AddToQueue(List<EmailQueue> emailQueues, EmailQueue emailQueue)
        {
            if (!emailQueues.Any(a => a.User == emailQueue.User)) emailQueues.Add(emailQueue);
        }

        /// <summary>
        /// Copy emailqueues into the order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="emailQueues"></param>
        private void AddQueuesToOrder(Order order, List<EmailQueue> emailQueues )
        {
            foreach (var eq in emailQueues)
            {
                order.AddEmailQueue(eq);
            }
        }

        private string GenerateLink(string address, string orderRequestNumber)
        {
            return string.Format("<a href=\"{0}{1}\">{1}</a>", address, orderRequestNumber);
        }

        private List<Workgroup> FindAdminWorkgroups(Order order)
        {
            var workgroups = new List<Workgroup>();

            // find the rollup parents
            var rollupDepts = _queryRepositoryFactory.AdminWorkgroupRepository.Queryable.Where(a => a.WorkgroupId == order.Workgroup.Id).Select(a => a.RollupParentId).ToList();

            // get the proper workgroups for the rollup departments
            // needs to look at more than just hte primary org
            //var wrkgrps = _repositoryFactory.WorkgroupRepository.Queryable.Where(a => rollupDepts.Contains(a.PrimaryOrganization.Id) && a.Administrative && a.SharedOrCluster).ToList();
            //workgroups.AddRange(wrkgrps);

            foreach (var deptId in rollupDepts)
            {
                var dept = _repositoryFactory.OrganizationRepository.GetNullableById(deptId);

                if (dept != null)
                {
                    var wrkgrps = _repositoryFactory.WorkgroupRepository.Queryable.Where(a => a.Organizations.Contains(dept) && a.Administrative && a.SharedOrCluster).ToList();
                    workgroups.AddRange(wrkgrps);
                }
                
            }

            workgroups.Add(order.Workgroup);

            return workgroups.Distinct().ToList();
        }
    }
}