﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AggieEnterpriseApi.Validation;
using Azure.Storage.Blobs.Models;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;
using Purchasing.Mvc.Services;
using Purchasing.WS;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Mvc.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Will add the proper approval levels to an order.  If a workgroup account or approver/acctManager is passed in, a split is not possible
        /// </summary>
        /// <param name="order">The order.  If it does not contain splits, you must pass along either workgroupAccount or acctManager</param>
        /// <param name="conditionalApprovalIds">The Ids of required conditional approvals for this order (the ones answered "yes")</param>
        /// <param name="accountId">Optional id of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        Task CreateApprovalsForNewOrder(Order order, int[] conditionalApprovalIds = null, string accountId = null, string approverId = null, string accountManagerId = null);
        
        /// <summary>
        /// Recreates approvals for the given order, removing all approvals at or above the current order level
        /// Should not affect conditional approvals?
        /// </summary>
        Task ReRouteApprovalsForExistingOrder(Order order, string approverId, string accountManagerId);

        /// <summary>
        /// Returns all of the approvals that need to be completed for the current approval status level
        /// </summary>
        /// <param name="orderId">Id of the order</param>
        IEnumerable<Approval> GetCurrentRequiredApprovals(int orderId);

        /// <summary>
        /// Modifies an order's approvals according to the permissions of the given userId
        /// </summary>
        /// <param name="order">The order</param>
        /// <param name="userId">Currently logged in user who clicked "I Approve"</param>
        bool Approve(Order order);

        OrderStatusCode GetCurrentOrderStatus(int orderId);

        /// <summary>
        /// //get the lowest status code that still needs to be approved
        /// </summary>
        OrderStatusCode GetCurrentOrderStatus(Order order);

        /// <summary>
        /// Handle editing an existing order without any rerouting
        /// </summary>
        void EditExistingOrder(Order order);

        void ReRouteSingleApprovalForExistingOrder(Approval approval, User user, bool notify = false);
        
        void Deny(Order order, string comment);
        void Cancel(Order order, string comment);

        bool WillOrderBeSentToKfs(OrderType newOrderType, string kfsDocType = null);
        /// <summary>
        /// Complete the last approval for an order, and return any errors that result
        /// </summary>
        /// <returns>String array of error messages, non-empty if completion didn't succeed</returns>
        Task<string[]> Complete(Order order, OrderType newOrderType, string kfsDocType = null);

        /// <summary>
        /// Get the current user's list of orders.
        /// </summary>
        /// <param name="allActive"></param>
        /// <param name="all">Get all orders pending, completed, cancelled</param>
        /// <param name="owned"></param>
        /// <param name="notOwned">Don't show orders you created </param>
        /// <param name="orderStatusCodes">Get all orders with current status codes in this list</param>
        /// <param name="startDate">Get all orders after this date</param>
        /// <param name="endDate">Get all orders before this date</param>
        /// <returns>List of orders according to the criteria</returns>
        IQueryable<OrderHistory> GetListofOrders(bool isComplete = false, bool showPending = false, string orderStatusCode = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), bool showCreated = false, DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?());

        /// <summary>
        /// Returns a list of orders that the current user has administrative access to
        /// </summary>
        /// <returns></returns>
        IQueryable<OrderHistory> GetAdministrativeListofOrders(bool isComplete = false, bool showPending = false, string orderStatusCode = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?());

        /// <summary>
        /// Looks for existing saved forms and associates them with the current order
        /// </summary>
        void HandleSavedForm(Order order, Guid formSaveId);

        IndexedList<OrderHistory> GetIndexedListofOrders(string received, string paid,bool isComplete = false, bool showPending = false, string orderStatusCode = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), bool showCreated = false, DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?());

        /// <summary>
        /// Returns a list of orders that the current user has administrative access to
        /// </summary>
        /// <returns></returns>
        IndexedList<OrderHistory> GetAdministrativeIndexedListofOrders(string received, string paid, bool isComplete = false, bool showPending = false, string orderStatusCode = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?());

        IndexedList<OrderHistory> GetAccountsPayableIndexedListofOrders(string received, string paid, string orderStatusCode = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?());

        IndexedList<OrderHistory> GetIndexedListofOrders(Access[] accessibleOrders, string received, string paid, bool isComplete = false,
            bool showPending = false, string orderStatusCode = null, DateTime? startDate = new DateTime?(),
            DateTime? endDate = new DateTime?(), bool showCreated = false,
            DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?());
    }

    public class OrderService : IOrderService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IAccessQueryService _accessQueryService;
        private readonly IFinancialSystemService _financialSystemService;
        private readonly IIndexService _indexService;
        private readonly IEventService _eventService;
        private readonly IUserIdentity _userIdentity;
        private readonly ISecurityService _securityService;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly IRepository<Approval> _approvalRepository;
        private readonly IRepository<OrderTracking> _orderTrackingRepository;
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository;
        //private readonly IRepositoryWithTypedId<User, string> _userRepository; // UserRepository is in the RepositoryFactory
        private readonly IRepository<Order> _orderRepository;

        private readonly IAggieEnterpriseService _aggieEnterpriseService;

        public OrderService(IRepositoryFactory repositoryFactory, 
                            IEventService eventService, 
                            IUserIdentity userIdentity, 
                            ISecurityService securityService, 
                            IRepository<WorkgroupPermission> workgroupPermissionRepository, 
                            IRepository<Approval> approvalRepository, 
                            IRepository<OrderTracking> orderTrackingRepository, 
                            IRepositoryWithTypedId<Organization, string> organizationRepository, 
                            //IRepositoryWithTypedId<User, string> userRepository, 
                            IRepository<Order> orderRepository, 
                            IQueryRepositoryFactory queryRepositoryFactory, 
                            IAccessQueryService accessQueryService,
                            IFinancialSystemService financialSystemService,
                            IIndexService indexService,
                            IAggieEnterpriseService aggieEnterpriseService)
        {
            _repositoryFactory = repositoryFactory;
            _eventService = eventService;
            _userIdentity = userIdentity;
            _securityService = securityService;
            _workgroupPermissionRepository = workgroupPermissionRepository;
            _approvalRepository = approvalRepository;
            _orderTrackingRepository = orderTrackingRepository;
            _organizationRepository = organizationRepository;
            //_userRepository = userRepository;
            _orderRepository = orderRepository;
            _queryRepositoryFactory = queryRepositoryFactory;
            _accessQueryService = accessQueryService;
            _financialSystemService = financialSystemService;
            _indexService = indexService;
            _aggieEnterpriseService = aggieEnterpriseService;
        }

        /// <summary>
        /// Will add the proper approval levels to an order.  If a workgroup account or approver/acctManager is passed in, a split is not possible
        /// </summary>
        /// <param name="order">The order.  If it does not contain splits, you must pass along either workgroupAccount or acctManager</param>
        /// <param name="conditionalApprovalIds">The Ids of required conditional approvals for this order (the ones answered "yes")</param>
        /// <param name="accountId">Optional id of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        public async Task CreateApprovalsForNewOrder(Order order, int[] conditionalApprovalIds = null, string accountId = null, string approverId = null, string accountManagerId = null)
        {
            var approvalInfo = new ApprovalInfo();

            if (order.Splits.Count() == 1) //Order has one split and can thus optionally not have accounts assigned
            {
                var split = order.Splits.Single();

                Check.Require(!string.IsNullOrWhiteSpace(accountId) || !string.IsNullOrWhiteSpace(accountManagerId),
                          "You must either supply the ID of a valid account or provide the userId for an account manager");

                if (!string.IsNullOrWhiteSpace(accountId)) //if we route by account, use that for info
                {
                    //TODO: move this code to a private methods as very similar code is used elsewhere here.                   
                    var workgroupAccount =
                        _repositoryFactory.WorkgroupAccountRepository.Queryable.FirstOrDefault(x => x.Workgroup.Id == order.Workgroup.Id && 
                        ((x.Account != null && x.Account.Id == accountId) || (x.FinancialSegmentString != null && x.FinancialSegmentString == accountId)) );

                    approvalInfo.AccountId = accountId;
                    approvalInfo.IsExternal = false; //always false now? (workgroupAccount == null); //if we can't find the account in the workgroup it is external
                    
                    if (workgroupAccount != null) //route to the people contained in the workgroup account info
                    {
                        approvalInfo.Approver = workgroupAccount.Approver;
                        approvalInfo.AcctManager = workgroupAccount.AccountManager;
                        approvalInfo.Purchaser = workgroupAccount.Purchaser;
                    }
                    else //account is not in the workgroup, even if we don't find the account, we will still use it
                    {
                        if (FinancialChartValidation.GetFinancialChartStringType(accountId) == FinancialChartStringType.Invalid)
                        {
                            //This was doing some stuff to deal with KFS external accounts which we no longer support
                            
                            approvalInfo.Approver = null;
                            approvalInfo.AcctManager = null;
                            approvalInfo.Purchaser = null;
                        }
                        else
                        {
                            var aggieApproval = await _aggieEnterpriseService.GetFinancialOfficer(accountId);
                            //TODO: Fix if we can get info from AE
                            approvalInfo.IsExternal = aggieApproval.IsExternal; //This is currently always false in the GetFinancialOfficer method
                            approvalInfo.Approver = null;
                            approvalInfo.AcctManager = aggieApproval.FinancialOfficerId != null ? _securityService.GetUser(aggieApproval.FinancialOfficerId) : null;
                            approvalInfo.Purchaser = null;
                        }
                    }

                    //Ok this is causing issues... May need more debugging
                    //split.Account = accountId; //Assign the account to the split
                    //Try doing this instead:
                    var segmentStringType = FinancialChartValidation.GetFinancialChartStringType(accountId);
                    if(segmentStringType == FinancialChartStringType.Invalid)
                    {
                        split.Account = accountId;
                    }
                    else if(string.IsNullOrWhiteSpace(split.FinancialSegmentString))
                    {
                        split.FinancialSegmentString = accountId;
                    }
                }
                else //else stick with user provided values
                {
                    approvalInfo.Approver = string.IsNullOrWhiteSpace(approverId) ? null : _repositoryFactory.UserRepository.GetById(approverId);
                    approvalInfo.AcctManager = GetManager(accountManagerId);
                }

                AddApprovalSteps(order, approvalInfo, split);
            }
            else //else order has multiple splits and each one needs an account
            {
                foreach (var split in order.Splits)
                {
                    //Try to find the account in the workgroup so we can route it by users
                    var workgroupAccount = _repositoryFactory.WorkgroupAccountRepository.Queryable.FirstOrDefault(x => x.Workgroup.Id == order.Workgroup.Id &&
                        ((x.Account != null && x.Account.Id == split.Account) || (x.FinancialSegmentString != null && x.FinancialSegmentString == split.FinancialSegmentString)));
                    approvalInfo.AccountId = split.Account;
                    approvalInfo.IsExternal = false; // always false now  workgroupAccount == null; //if we can't find the account in the workgroup it is external

                    if (workgroupAccount != null) //route to the people contained in the workgroup account info
                    {
                        approvalInfo.Approver = workgroupAccount.Approver;
                        approvalInfo.AcctManager = workgroupAccount.AccountManager;
                        approvalInfo.Purchaser = workgroupAccount.Purchaser;
                    }
                    else
                    { //account is not in the workgroup
                        if (split.Account != null)
                        {
                            //This was doing some stuff to deal with KFS external accounts which we no longer support

                            approvalInfo.Approver = null;
                            approvalInfo.AcctManager = null;
                            approvalInfo.Purchaser = null;
                        }
                        else
                        {
                            var aggieApproval = await _aggieEnterpriseService.GetFinancialOfficer(split.FinancialSegmentString);
                            //TODO: Fix if we can get info from AE
                            approvalInfo.IsExternal = aggieApproval.IsExternal;
                            approvalInfo.Approver = null;
                            approvalInfo.AcctManager = aggieApproval.FinancialOfficerId != null ? _securityService.GetUser(aggieApproval.FinancialOfficerId) : null;
                            approvalInfo.Purchaser = null;
                        }
                    }
                    
                    AddApprovalSteps(order, approvalInfo, split);
                }   
            }

            //If we were passed conditional approval info, go ahead and add them
            if (conditionalApprovalIds != null && conditionalApprovalIds.Any())
            {
                foreach (var conditionalApprovalId in conditionalApprovalIds)
                {
                    var id = conditionalApprovalId;
                    var approverIds =
                        _repositoryFactory.ConditionalApprovalRepository.Queryable.Where(x => x.Id == id)
                            .Select(x =>
                                    new
                                        {
                                            primaryApproverId = x.PrimaryApprover.Id,
                                            secondaryApproverId = x.SecondaryApprover != null ? x.SecondaryApprover.Id : null
                                        }
                            ).Single();

                    var newApproval = new Approval //Add a new 'approver' level approval
                                          {
                                              Completed = false,
                                              User = _repositoryFactory.UserRepository.GetById(approverIds.primaryApproverId),
                                              SecondaryUser = approverIds.secondaryApproverId == null ? null : _repositoryFactory.UserRepository.GetById(approverIds.secondaryApproverId),
                                              StatusCode =
                                                  _repositoryFactory.OrderStatusCodeRepository.Queryable.Single(x => x.Id == OrderStatusCode.Codes.ConditionalApprover)
                                          };

                    order.AddApproval(newApproval);//Add directly to the order since conditional approvals never go against splits
                }
            }
            if (order.Workgroup.RequireApproval && !order.Approvals.Any(a => a.StatusCode.Id == OrderStatusCode.Codes.Approver))
            {
                // Ok, we don't have any approvals (because an external account was used), so we want to add one.
                var missingApproval = new Approval
                                          {
                                              Completed = false,
                                              User = null,
                                              SecondaryUser = null,
                                              StatusCode =
                                                  _repositoryFactory.OrderStatusCodeRepository.Queryable.Single(
                                                      x => x.Id == OrderStatusCode.Codes.Approver)
                                          };
                order.AddApproval(missingApproval);
            }
            
            order.StatusCode = GetCurrentOrderStatus(order);

            _eventService.OrderCreated(order); //Creating approvals means the order is being created
        }

        /// <summary>
        /// Recreates approvals for the given order, removing all approvals at or above the current order level
        /// Should not affect conditional approvals?
        /// </summary>
        public async Task ReRouteApprovalsForExistingOrder(Order order, string approverId = null, string accountManagerId = null)
        {
            var currentLevel = order.StatusCode.Level;

            //Remove all approvals at the current level or above
            var approvalsToRemove =
                    order.Approvals.Where(x =>
                        x.StatusCode.Level >= currentLevel &&
                        x.StatusCode.Id != OrderStatusCode.Codes.ConditionalApprover).ToList();
                
            foreach (var approval in approvalsToRemove)
            {
                order.Approvals.Remove(approval);
            }

            //recreate approvals
            if (!string.IsNullOrWhiteSpace(accountManagerId) && order.Splits.Count == 1)
            {
                //If we have an account manager assigned directly and only one split, use direct assigning for creating approvals
                var approvalInfo = new ApprovalInfo();
                var split = order.Splits.Single();

                approvalInfo.Approver = string.IsNullOrWhiteSpace(approverId) ? null : _repositoryFactory.UserRepository.GetById(approverId);
                approvalInfo.AcctManager = GetManager(accountManagerId);
                
                AddApprovalSteps(order, approvalInfo, split, currentLevel);
            }
            else
            {
                foreach (var split in order.Splits)
                {
                    var approvalInfo = new ApprovalInfo();
                    
                    //Try to find the account in the workgroup so we can route it by users
                    var workgroupAccount = _repositoryFactory.WorkgroupAccountRepository.Queryable.FirstOrDefault(x => x.Workgroup.Id == order.Workgroup.Id && 
                    ((x.Account != null && x.Account.Id == split.Account) || (x.FinancialSegmentString != null && x.FinancialSegmentString == split.FinancialSegmentString)));

                    approvalInfo.AccountId = split.Account ?? split.FinancialSegmentString;
                    approvalInfo.IsExternal = false; // always false now (workgroupAccount == null); //if we can't find the account in the workgroup it is external

                    if (workgroupAccount != null) //route to the people contained in the workgroup account info
                    {
                        approvalInfo.Approver = workgroupAccount.Approver;
                        approvalInfo.AcctManager = workgroupAccount.AccountManager;
                        approvalInfo.Purchaser = workgroupAccount.Purchaser;
                    }
                    else
                    { //account is not in the workgroup
                        if(split.Account == null) //Workaround for CoA
                        {
                            var aggieApproval = await _aggieEnterpriseService.GetFinancialOfficer(split.FinancialSegmentString);
                            //TODO: Fix if we can get info from AE
                            approvalInfo.IsExternal = aggieApproval.IsExternal;
                            approvalInfo.Approver = null;
                            approvalInfo.AcctManager = aggieApproval.FinancialOfficerId != null ? _securityService.GetUser(aggieApproval.FinancialOfficerId) : null;
                            approvalInfo.Purchaser = null;
                        }
                        else 
                        {
                            //This was doing some stuff to deal with KFS external accounts which we no longer support

                            approvalInfo.Approver = null;
                            approvalInfo.AcctManager =null;
                            approvalInfo.Purchaser = null;


                        }

                    }
                    
                    AddApprovalSteps(order, approvalInfo, split, currentLevel);
                }
            }

            order.StatusCode = GetCurrentOrderStatus(order);

            _eventService.OrderReRouted(order);
        }

        /// <summary>
        /// Handle editing an existing order without any rerouting
        /// </summary>
        public void EditExistingOrder(Order order)
        {
            _eventService.OrderEdited(order);
        }

        public void ReRouteSingleApprovalForExistingOrder(Approval approval, User user, bool notify = false)
        {
            approval.SecondaryUser = null;
            approval.User = user;

            _eventService.OrderApprovalAdded(approval.Order, approval, notify);            
        }

        /// <summary>
        /// //get the lowest status code that still needs to be approved
        /// </summary>
        public OrderStatusCode GetCurrentOrderStatus(Order order)
        {
            var lowestSplitApproval = (from a in order.Approvals
                                       where a.Split != null
                                             && !a.Completed
                                       orderby a.StatusCode.Level
                                       select a.StatusCode).First();

            var lowestDirectApproval =
                (from a in order.Approvals
                 where a.Split == null && !a.Completed
                 orderby a.StatusCode.Level
                 select a.StatusCode).
                    FirstOrDefault();

            if (lowestDirectApproval == null) return lowestSplitApproval;

            return lowestDirectApproval.Level < lowestSplitApproval.Level ? lowestDirectApproval : lowestSplitApproval;
        }

        /// <summary>
        /// Returns the current approval level that needs to be completed, or null if there are no approval steps pending
        /// </summary>
        /// <param name="orderId">Id of the order</param>
        /// <remarks>TODO: I think we can get rid of this and just query order.StatusCode as long as it is up to date</remarks>
        public OrderStatusCode GetCurrentOrderStatus(int orderId)
        {
            var currentApprovalLevel = (from approval in _repositoryFactory.ApprovalRepository.Queryable
                                        where approval.Order.Id == orderId && !approval.Completed
                                        orderby approval.StatusCode.Level
                                        select approval.StatusCode).FirstOrDefault();
            return currentApprovalLevel;
        }

        /// <summary>
        /// Returns all of the approvals that need to be completed for the current approval status level
        /// </summary>
        /// <param name="orderId">Id of the order</param>
        public IEnumerable<Approval> GetCurrentRequiredApprovals(int orderId)
        {
            var currentOrderStatus = GetCurrentOrderStatus(orderId);

            return
                _repositoryFactory.ApprovalRepository.Queryable.Where(
                    x => x.Order.Id == orderId && x.StatusCode.Id == currentOrderStatus.Id);
        }

        /// <summary>
        /// Modifies an order's approvals according to the permissions of the given userId
        /// </summary>
        /// <param name="order">The order</param>
        public bool Approve(Order order)
        {
            var didApprovalHappen = false;
            var currentApprovalLevel = order.StatusCode.Level;

            var hasWorkgroupRole = _securityService.hasWorkgroupRole(order.StatusCode.Id == OrderStatusCode.Codes.ConditionalApprover ? OrderStatusCode.Codes.Approver : order.StatusCode.Id, order.Workgroup.Id);

            //If the approval is at the current level & directly associated with the user (primary or secondary), go ahead and approve it
            foreach (var approvalForUserDirectly in
                        order.Approvals.Where(
                            x => x.StatusCode.Level == currentApprovalLevel
                                && !x.Completed
                                && (
                                    (x.User != null && string.Equals(x.User.Id, _userIdentity.Current, StringComparison.OrdinalIgnoreCase))
                                    || (x.SecondaryUser != null && string.Equals(x.SecondaryUser.Id, _userIdentity.Current, StringComparison.OrdinalIgnoreCase))
                                    )
                                )
                    )
            {
                approvalForUserDirectly.Completed = true;
                _eventService.OrderApproved(order, approvalForUserDirectly);
                didApprovalHappen = true;
            }

            if (hasWorkgroupRole)
            {
                //If the approval is at the current level and has no user is attached, it can be approved by this workgroup user
                foreach (
                    var approvalForWorkgroup in
                        order.Approvals.Where(x => x.StatusCode.Level == currentApprovalLevel && !x.Completed && x.User == null))
                {
                    approvalForWorkgroup.Completed = true;
                    _eventService.OrderApproved(order, approvalForWorkgroup);
                    didApprovalHappen = true;
                }

                //If the approval is at the current level, and the users are away, it can be approved by this workgroup user
                foreach (
                    var approvalForAway in
                        order.Approvals.Where(a =>
                            a.StatusCode.Level == currentApprovalLevel && 
                            a.StatusCode.Id != OrderStatusCode.Codes.ConditionalApprover && //These can never be bypassed
                            !a.Completed &&
                            a.User != null &&
                            a.User.IsAway &&
                            (a.SecondaryUser == null || (a.SecondaryUser != null && a.SecondaryUser.IsAway))))
                {
                    approvalForAway.Completed = true;
                    _eventService.OrderApproved(order, approvalForAway);
                    didApprovalHappen = true;
                }

                if (!didApprovalHappen && _securityService.hasAdminWorkgroupRole(order.StatusCode.Id, order.Workgroup.Id))
                {
                    //Ok, so the approval didn't happen. Now check to see if the current user is an admin for that level.
                    foreach (
                    var approvalForAdmin in
                        order.Approvals.Where(a => a.StatusCode.Level == currentApprovalLevel && a.StatusCode.Id != OrderStatusCode.Codes.ConditionalApprover && !a.Completed && a.User != null))
                    {
                        approvalForAdmin.Completed = true;
                        _eventService.OrderApprovedByAdmin(order, approvalForAdmin);
                        didApprovalHappen = true;
                    }
                }
            }

            //Now if there are no more approvals pending at this level, move the order up a level
            if (order.Approvals.Any(x => x.StatusCode.Level == currentApprovalLevel && !x.Completed) == false)
            {
                var nextStatusCode =
                    _repositoryFactory.OrderStatusCodeRepository.Queryable.Single(x => x.Level == (currentApprovalLevel + 1));

                order.StatusCode = nextStatusCode;
                _eventService.OrderStatusChange(order, nextStatusCode);
            }

            return didApprovalHappen;
        }

        /// <summary>
        /// The order is denied by an actor. This requires a comment
        /// </summary>
        public void Deny(Order order, string comment)
        {
            var previousStatus = order.StatusCode;
            order.StatusCode = _repositoryFactory.OrderStatusCodeRepository.GetById(OrderStatusCode.Codes.Denied);

            _eventService.OrderDenied(order, comment, previousStatus);
        }

        /// <summary>
        /// The original creator of an order can cancel that order at any time
        /// </summary>
        public void Cancel(Order order, string comment)
        {
            var previousStatus = order.StatusCode;
            order.StatusCode = _repositoryFactory.OrderStatusCodeRepository.GetById(OrderStatusCode.Codes.Cancelled);

            _eventService.OrderCancelled(order, comment, previousStatus);
        }

        public bool WillOrderBeSentToKfs(OrderType newOrderType, string kfsDocType = null)
        {
            if (newOrderType.Id == OrderType.Types.KfsDocument && _financialSystemService.AllowedType(kfsDocType))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Complete the last approval for an order, and return any errors that result
        /// </summary>
        /// <returns>String array of error messages, non-empty if completion didn't succeed</returns>
        public async Task<string[]> Complete(Order order, OrderType newOrderType, string kfsDocType = null)
        {
            order.StatusCode = _repositoryFactory.OrderStatusCodeRepository.GetById(OrderStatusCode.Codes.Complete);
            order.OrderType = newOrderType;
            order.KfsDocType = kfsDocType;

            //TODO: Replace this if with the new method.
            if (newOrderType.Id == OrderType.Types.KfsDocument && _financialSystemService.AllowedType(kfsDocType))
            {
                //Note in this case newOrderType.DocType should be either PR or DPO
                var result = _financialSystemService.SubmitOrder(order, _userIdentity.Current, kfsDocType);
                
                if (result.Success)
                {
                    order.ReferenceNumber = result.DocNumber;    
                }
                else
                {
                    return result.Messages.ToArray();
                }
            }

            if(newOrderType.Id.Trim() == OrderType.Types.AggieEnterprise)
            {
                var user = _repositoryFactory.UserRepository.Queryable.Single(x => x.Id == _userIdentity.Current);

                var result = await _aggieEnterpriseService.UploadOrder(order, user.Email, user.Id); //TODO: Get an IAM email with a new field on the user?
                if (result.Success)
                {
                    order.ReferenceNumber = result.DocNumber; //TODO: Replace?
                    _eventService.AeOrderCompleted(order);
                }
                else
                {
                    return result.Messages.ToArray();
                }
            }

            if (order.KfsDocType == "PR2")
            {
                order.KfsDocType = "PR";
            }

            //Mark complete the final approval
            var purchaserApproval = order.Approvals.Single(x => !x.Completed);
            purchaserApproval.Completed = true;

            _eventService.OrderCompleted(order);

            return new string[0]; //return no errors
        }

        /// <summary>
        /// Duplicates the given order info a new order, one that doesn't include the splits, approvals, or history of the given order
        /// </summary>
        /// <remarks>
        /// //TODO: should a duplicate order retain the same splits?
        /// //TODO: should approvals/create events be called automatically?
        /// </remarks>
        public Order Duplicate(Order order)
        {
            var newOrder = new Order
                               {
                                   Address = order.Address,
                                   AllowBackorder = order.AllowBackorder,
                                   DateNeeded = order.DateNeeded,
                                   EstimatedTax = order.EstimatedTax,
                                   OrderType = order.OrderType,
                                   Organization = order.Organization,
                                   ReferenceNumber = order.ReferenceNumber,
                                   ShippingAmount = order.ShippingAmount,
                                   ShippingType = order.ShippingType,
                                   Vendor = order.Vendor,
                                   Workgroup = order.Workgroup
                               };

            //Now add in the line items
            foreach (var lineItem in order.LineItems)
            {
                var newLineItem = new LineItem
                                      {
                                          CatalogNumber = lineItem.CatalogNumber,
                                          Commodity = lineItem.Commodity,
                                          Description = lineItem.Description,
                                          Notes = lineItem.Notes,
                                          Quantity = lineItem.Quantity,
                                          Unit = lineItem.Unit,
                                          UnitPrice = lineItem.UnitPrice,
                                          Url = lineItem.Url
                                      };

                newOrder.AddLineItem(newLineItem);
            }

            return newOrder;
        }

        /// <summary>
        /// Looks for existing saved forms and associates them with the current order
        /// </summary>
        public void HandleSavedForm(Order order, Guid formSaveId)
        {
            var savedForm =
                _repositoryFactory.OrderRequestSaveRepository.Queryable.SingleOrDefault(x => x.Id == formSaveId);

            if (savedForm != null)
            {
                if (savedForm.PreparedBy.Id != _userIdentity.Current)
                {
                    //Add prepared by comment if the current user didn't do the inital save
                    order.AddComment(new OrderComment
                    {
                        Text = "Prepared By " + savedForm.PreparedBy.FullNameAndId,
                        User = savedForm.PreparedBy,
                        DateCreated = DateTime.UtcNow.ToPacificTime()
                    });   
                }

                _repositoryFactory.OrderRequestSaveRepository.Remove(savedForm, flushChanges: false); //now remove the saved form since it's no longer needed
            }
        }

        /// <summary>
        /// Add in approval steps to either the order or split, depending on what is provided
        /// </summary>
        /// <param name="order">The order</param>
        /// <param name="approvalInfo">list of approval people (or null) to route to</param>
        /// <param name="split">optional split to approve against instead of the order</param>
        /// <param name="minLevel">Min level only adds approvals at or above the provided level</param>
        private void AddApprovalSteps(Order order, ApprovalInfo approvalInfo, Split split, int minLevel = 0)
        {
            var approvals = new List<Approval>
                                {
                                    new Approval
                                        {
                                            Completed = AutoApprovable(order, split, approvalInfo.Approver), 
                                            //If this is auto approvable just include it but mark it as approval already
                                            User = approvalInfo.Approver,
                                            StatusCode =
                                                _repositoryFactory.OrderStatusCodeRepository.GetById(OrderStatusCode.Codes.Approver)
                                        },
                                    new Approval
                                        {
                                            Completed = false,
                                            User = approvalInfo.AcctManager,
                                            StatusCode =
                                                _repositoryFactory.OrderStatusCodeRepository.GetById(OrderStatusCode.Codes.AccountManager),
                                                IsExternal = approvalInfo.IsExternal
                                        },
                                    new Approval
                                        {
                                            Completed = false,
                                            User = approvalInfo.Purchaser,
                                            StatusCode =
                                                _repositoryFactory.OrderStatusCodeRepository.GetById(OrderStatusCode.Codes.Purchaser)
                                        }
                                };

            if (minLevel > 0)
            {
                approvals = approvals.Where(x => x.StatusCode.Level >= minLevel).ToList();
            }

            foreach (var approval in approvals)
            {
                if (approval.StatusCode.Id == OrderStatusCode.Codes.Purchaser)
                {
                    //Make sure to only add one purchaser approval
                    if (order.Approvals.Any(x => x.StatusCode.Id == OrderStatusCode.Codes.Purchaser)) continue;
                }

                if (approval.StatusCode.Id == OrderStatusCode.Codes.Approver && approvalInfo.IsExternal)
                {
                    continue; //Do not add approvals at the AP level for external accounts
                }
                
                split.AssociateApproval(approval);

                if (approval.Completed)
                {
                    //already approved means auto approval, so send that specific event
                    _eventService.OrderAutoApprovalAdded(order, approval);
                }
                else
                {
                    _eventService.OrderApprovalAdded(order, approval);   
                }
            }
        }

        /// <summary>
        /// Calculate the automatic approvals-- if any apply mark that approval level as complete
        /// </summary>
        /// <param name="order">The order</param>
        /// <param name="split">The split this autoApproval will be associated with</param>
        /// <param name="approver">The associated approver</param>
        private bool AutoApprovable(Order order, Split split, User approver)
        {
            if (approver == null) return false; //Only auto approve when assigned to a specific approver

            var total = split.Amount;
            var accountId = split.Account ?? string.Empty;

            if (order.Workgroup.ForceAccountApprover && string.IsNullOrWhiteSpace(accountId))
            {
                return false; //If the workgroup forces account approver decision and this split doesn't have an account, then it can't be auto approved
            }

            if (string.Equals(approver.Id, _userIdentity.Current, StringComparison.OrdinalIgnoreCase) && string.Equals(approver.Id, order.CreatedBy.Id, StringComparison.OrdinalIgnoreCase))
            {
                return true; //Auto approved if the approver is the current user
            }
            
            //See if there are any automatic approvals for this user/account.
            var possibleAutomaticApprovals =
                _repositoryFactory.AutoApprovalRepository.Queryable
                    .Where(x => x.IsActive && (x.Expiration == null || x.Expiration > DateTime.UtcNow.ToPacificTime())) //valid only if it is active and isn't expired yet
                    .Where(x => x.User.Id == approver.Id) //auto approval must have been created by the approver
                    .Where(x => (x.TargetUser != null && x.TargetUser.Id == order.CreatedBy.Id) || x.Account.Id == accountId)//either applies to the order creator or account
                    .ToList();

            foreach (var autoApproval in possibleAutomaticApprovals) //for each autoapproval, check if they apply.  If any do, return true
            {
                if (autoApproval.Equal)
                {
                    if (total == autoApproval.MaxAmount)
                    {
                        return true;
                    }
                }
                else //less than
                {
                    if (total < autoApproval.MaxAmount)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the manager given by account id, unless we want any manager which just returns null (goes to workgroup)
        /// </summary>
        private User GetManager(string accountManagerId)
        {
            return string.Equals(accountManagerId, "anymanager", StringComparison.OrdinalIgnoreCase)
                       ? null
                       : _repositoryFactory.UserRepository.GetById(accountManagerId);
        }

        private class ApprovalInfo
        {
            public ApprovalInfo()
            {
                Approver = null;
                AcctManager = null;
                Purchaser = null;
                AccountId = null;
            }

            public string AccountId { get; set; }
            public User Approver { get; set; }
            public User AcctManager { get; set; }
            public User Purchaser { get; set; }

            public bool IsExternal { get; set; }
        }


        public IQueryable<OrderHistory> GetListofOrders(bool isComplete = false, bool showPending = false, string orderStatusCode = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), bool showCreated = false, DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?())
        {
            // get orderids accessible by user
            var orderIds = _accessQueryService.GetOrderAccessByAdminStatus(_userIdentity.Current, isAdmin: false);

            if (orderIds.Count() == 0) // no results if you aren't allowed to see anything
            {
                return new List<OrderHistory>().AsQueryable();
            }

            // only show "pending" aka has edit rights
            if (showPending) orderIds = orderIds.Where(a => a.EditAccess);

            //var ids = orderIds.Select(a => a.OrderId).ToList();
    
            // filter for accessible orders
            var ordersQuery = _queryRepositoryFactory.OrderHistoryRepository.Queryable.Where(o => orderIds.Select(a => a.OrderId).Contains(o.OrderId)) ;
            
            // filter for selected status
            ordersQuery = GetOrdersByStatus(ordersQuery, isComplete, orderStatusCode);
            
            // filter for selected dates            
            ordersQuery = GetOrdersByDate(ordersQuery, startDate, endDate, startLastActionDate, endLastActionDate);

            // filter for created
            if (showCreated)
            {
                ordersQuery = ordersQuery.Where(a => a.CreatorId == _userIdentity.Current);
            }

            return ordersQuery;
        }

        public IndexedList<OrderHistory> GetIndexedListofOrders(Access[] accessibleOrders, string received, string paid, bool isComplete = false,
            bool showPending = false, string orderStatusCode = null, DateTime? startDate = new DateTime?(),
            DateTime? endDate = new DateTime?(), bool showCreated = false,
            DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?())
        {
            if (accessibleOrders.Length == 0) // no results if you aren't allowed to see anything
            {
                return new IndexedList<OrderHistory> { LastModified = DateTime.UtcNow.ToPacificTime(), Results = new List<OrderHistory>() };
            }

            // only show "pending" aka has edit rights
            if (showPending) accessibleOrders = accessibleOrders.Where(a => a.EditAccess).ToArray();

            //var ids = orderIds.Select(a => a.OrderId).ToList();

            // filter for accessible orders
            var ordersIndexQuery = _indexService.GetOrderHistory(accessibleOrders.Select(x => x.OrderId).ToArray(), startDate, endDate, startLastActionDate, endLastActionDate, orderStatusCode);
            var ordersQuery = ordersIndexQuery.Results.AsQueryable();

            // filter for selected status
            ordersQuery = GetOrdersByStatus(ordersQuery, isComplete, orderStatusCode, received, paid);

            // filter for selected dates            
            ordersQuery = GetOrdersByDate(ordersQuery, startDate, endDate, startLastActionDate, endLastActionDate);

            // filter for created
            if (showCreated)
            {
                ordersQuery = ordersQuery.Where(a => a.CreatorId == _userIdentity.Current);
            }

            ordersIndexQuery.Results = ordersQuery.OrderByDescending(a => a.LastActionDate).Take(1000).ToList();

            return ordersIndexQuery;

        }


        public IndexedList<OrderHistory> GetIndexedListofOrders(string received, string paid,bool isComplete = false, bool showPending = false, string orderStatusCode = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), bool showCreated = false, DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?())
        {
            // get orderids accessible by user
            var orders = _accessQueryService.GetOrderAccessByAdminStatus(_userIdentity.Current, isAdmin: false).ToArray();

            if (orders.Length == 0) // no results if you aren't allowed to see anything
            {
                return new IndexedList<OrderHistory> { LastModified = DateTime.UtcNow.ToPacificTime(), Results = new List<OrderHistory>() };
            }

            return GetIndexedListofOrders(orders, received, paid, isComplete, showPending, orderStatusCode, startDate, endDate, showCreated, startLastActionDate, endLastActionDate);
        }

        public IQueryable<OrderHistory> GetAdministrativeListofOrders(bool isComplete = false, bool showPending = false, string orderStatusCode = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?())
        {
            // get orderids accessible by user
            var orderIds = _accessQueryService.GetOrderAccessByAdminStatus(_userIdentity.Current, isAdmin: true);

            if (orderIds.Count() == 0) // no results if you aren't allowed to see anything
            {
                return new List<OrderHistory>().AsQueryable();
            }

            // only show "pending" aka has edit rights
            if (showPending) orderIds = orderIds.Where(a => a.EditAccess);

            //var ids = orderIds.Select(a => a.OrderId).ToList();

            // filter for accessible orders
            var ordersQuery = _queryRepositoryFactory.OrderHistoryRepository.Queryable.Where(o => orderIds.Select(a => a.OrderId).Contains(o.OrderId));

            // filter for selected status
            ordersQuery = GetOrdersByStatus(ordersQuery, isComplete, orderStatusCode);

            // filter for selected dates            
            ordersQuery = GetOrdersByDate(ordersQuery, startDate, endDate, startLastActionDate, endLastActionDate);

            return ordersQuery;
        }

        public IndexedList<OrderHistory> GetAdministrativeIndexedListofOrders(string received, string paid,bool isComplete = false, bool showPending = false, string orderStatusCode = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?())
        {
            // get orderids accessible by user
            var orderIds = _accessQueryService.GetOrderAccessByAdminStatus(_userIdentity.Current, isAdmin: true);

            if (orderIds.Count() == 0) // no results if you aren't allowed to see anything
            {
                return new IndexedList<OrderHistory> { LastModified = DateTime.UtcNow.ToPacificTime(), Results = new List<OrderHistory>() };
            }

            // only show "pending" aka has edit rights
            if (showPending) orderIds = orderIds.Where(a => a.EditAccess);

            //var ids = orderIds.Select(a => a.OrderId).ToList();

            // filter for accessible orders
            var ordersIndexQuery = _indexService.GetOrderHistory(orderIds.Select(x => x.OrderId).ToArray(), startDate, endDate, startLastActionDate, endLastActionDate, orderStatusCode);
            var ordersQuery = ordersIndexQuery.Results.AsQueryable();

            // filter for selected status
            ordersQuery = GetOrdersByStatus(ordersQuery, isComplete, orderStatusCode, received, paid);

            // filter for selected dates            
            ordersQuery = GetOrdersByDate(ordersQuery, startDate, endDate, startLastActionDate, endLastActionDate);

            ordersIndexQuery.Results = ordersQuery.OrderByDescending(a => a.LastActionDate).Take(1000).ToList();
            
            return ordersIndexQuery;
        }

        public IndexedList<OrderHistory> GetAccountsPayableIndexedListofOrders(string received, string paid, string orderStatusCode = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?())
        {
            // get orderids accessible by user
            //var orderIds = _accessQueryService.GetOrderAccessByAdminStatus(_userIdentity.Current, isAdmin: true);
            var orderIds = _repositoryFactory.OrderRepository.Queryable.Where(a => a.ApUser != null && a.ApUser.Id == _userIdentity.Current).Select(s => s.Id).ToArray();

            if (orderIds.Count() == 0) // no results if you aren't allowed to see anything
            {
                return new IndexedList<OrderHistory> { LastModified = DateTime.UtcNow.ToPacificTime(), Results = new List<OrderHistory>() };
            }

            // filter for accessible orders
            var ordersIndexQuery = _indexService.GetOrderHistory(orderIds, startDate, endDate, startLastActionDate, endLastActionDate, orderStatusCode);
            var ordersQuery = ordersIndexQuery.Results.AsQueryable();

            // filter for selected status
            ordersQuery = GetOrdersByStatus(ordersQuery, true, orderStatusCode, received, paid);

            // filter for selected dates            
            ordersQuery = GetOrdersByDate(ordersQuery, startDate, endDate, startLastActionDate, endLastActionDate);

            ordersIndexQuery.Results = ordersQuery.OrderByDescending(a => a.LastActionDate).Take(1000).ToList();

            return ordersIndexQuery;
        }


        #region Depricated
        /// <summary>
        /// Traverse down (recursively) the organization and gather all the workgroups
        /// </summary>
        /// <param name="organization"> </param>
        /// <param name="maxLevel"> Loop breaker. Max depth of 99</param>
        /// <returns></returns>
        public List<Workgroup> TraverseOrgs(Organization organization, int maxLevel)
        {
            Check.Require(maxLevel <= 99, string.Format("Possible infinite regression for {0}", organization.Name));
            maxLevel++;
            var results = new List<Workgroup>();

            // get the children of this particular org
            var children = _organizationRepository.Queryable.Where(a => a.Parent == organization).ToList();

            foreach (var org in children)
            {
                results.AddRange(TraverseOrgs(org, maxLevel));
            }

            results.AddRange(organization.Workgroups);

            return results.Distinct().ToList();
        }

        /// <summary>
        /// Get the list of "pending" orders
        /// </summary>
        /// <remarks>List of orders pending at the user's status as well as one's they have requested</remarks>
        /// <param name="user"></param>
        /// <param name="workgroups"></param>
        /// <returns></returns>
        private List<Order> GetPendingOrders(User user, List<Workgroup> workgroups)
        {
            //            /* SQL Query */
            //            var sql =@"select ord.id
            //                    from approvals ap
            //	                    inner join orders ord on ap.orderid = ord.id and ap.orderstatuscodeid = ord.orderstatuscodeid
            //	                    left outer join users u on ap.userid = u.id
            //                    where approved is null
            //	                    and
            //	                    (  
            //		                    ap.userid is null
            //	                     or (ap.userid = @user or ap.secondaryuserid = @user)
            //	                     or (ap.orderstatuscodeid <> 'CA' and u.isaway = 1)
            //	                     )
            //	                     and
            //	                     ord.id in (
            //			                    select ap.orderid
            //			                    from (
            //				                    -- get approvals
            //				                    select ap.*, ord.workgroupid, osc.level
            //				                    from approvals ap
            //					                    inner join orders ord on ap.orderid = ord.id
            //					                    inner join orderstatuscodes osc on ap.orderstatuscodeid = osc.id
            //			                    ) ap
            //			                    inner join (
            //				                    -- get workgroup permissions and levels
            //				                    select workgroupid, level
            //				                    from [workgrouppermissions] wp
            //					                    inner join roles on roles.id = wp.roleid
            //				                    where userid = @user
            //			                    ) perm on ap.workgroupid = perm.workgroupid and ap.level = perm.level
            //	                     )";

            var results = new List<Order>();

            //            using (var conn = _dbService.GetConnection())
            //            {

            //                var orders = conn.Query<int>(sql, new {user=_userIdentity.Current});
            //                results.AddRange(_orderRepository.Queryable.Where(a=> orders.Contains(a.Id)).ToList());

            //            }

            var permissions = _workgroupPermissionRepository.Queryable.Where(a => a.User == user).ToList();

            //// get all approvals that are applicable
            // //var levels = permissions.Select(a => a.Role.Level).ToList();

            foreach (var perm in permissions)
            {
                //TODO: Fix for Conditional Approvals: If Approver has approved, but there are still pending Conditional Approvals, they will see the one they have already approved.
                var result = from a in _approvalRepository.Queryable
                             where a.Order.Workgroup == perm.Workgroup && a.StatusCode.Level == perm.Role.Level
                                && a.StatusCode.Level == a.Order.StatusCode.Level && !a.Completed
                                && (
                                    (a.User == user || a.SecondaryUser == user) // user is assigned
                                    ||
                                    (a.StatusCode.Id != OrderStatusCode.Codes.ConditionalApprover && a.User != null && a.User.IsAway)  // in standard approval, is user away
                                    )
                             select a.Order;

                results.AddRange(result.ToList());

                // deal with the ones that are just flat out workgroup permissions
                result = from a in _approvalRepository.Queryable
                         where a.Order.Workgroup == perm.Workgroup && a.StatusCode.Level == perm.Role.Level
                            && a.StatusCode.Level == a.Order.StatusCode.Level && !a.Completed
                            && a.User == null
                         select a.Order;

                results.AddRange(result.ToList());

            }

            // get the orders directly assigned, outside of their workgroup permissions
            var directApprovals = from a in _approvalRepository.Queryable
                                  where a.StatusCode.Level == a.Order.StatusCode.Level && !a.Completed
                                     && (a.User == user || a.SecondaryUser == user) // user is assigned
                                  select a.Order;

            results.AddRange(directApprovals.ToList());

            // var approvals = (
            //                     from a in _approvalRepository.Queryable
            //                     where permissions.Select(b => b.Workgroup).Contains(a.Order.Workgroup)
            //                         && permissions.Where(b => b.Workgroup == a.Order.Workgroup).Select(b => b.Role.Level).Contains(a.StatusCode.Level)
            //                         && a.StatusCode == a.Order.StatusCode && !a.Approved.HasValue
            //                         && (
            //                             (a.User == null)    // not assigned, use workgroup
            //                             ||
            //                             (a.User == user || a.SecondaryUser == user) // user is assigned
            //                             ||
            //                             (a.StatusCode.Id != OrderStatusCode.Codes.ConditionalApprover && a.User.IsAway)  // in standard approval, is user away
            //                         )
            //                     select a.Order
            //                 ).ToList();

            // var test = from a in _approvalRepository.Queryable
            //            join p in permissions on new {a.Order.Workgroup, a.StatusCode.Level} equals new {p.Workgroup, p.Role.Level}
            //            select a;

            // var test2 = test.ToList();

            var requestedOrders = _orderRepository.Queryable.Where(a => !a.StatusCode.IsComplete && a.CreatedBy == user).ToList();
            results.AddRange(requestedOrders);

            return results.Distinct().ToList();

            // var orders = new List<Order>();
            // orders.AddRange(approvals);
            // orders.AddRange(requestedOrders);
            // return orders.Distinct().ToList();
        }

        /// <summary>
        /// Gets all orders for which the user has already acted on, but are not yet complete.
        /// </summary>
        /// <returns></returns>
        private List<Order> GetActiveOrders(User user, List<Workgroup> workgroups)
        {
            var tracking = _orderTrackingRepository.Queryable.Where(a => a.User == user).Select(a => a.Order).ToList();
            var orders = _orderRepository.Queryable.Where(a => tracking.Contains(a) && !a.StatusCode.IsComplete);

            return orders.ToList();
        }

        /// <summary>
        /// Gets the archive of all orders the user is in the tracking chain for including complete
        /// </summary>
        /// <param name="user"></param>
        /// <param name="owned">Only return orders that are owned by the user</param>
        /// <returns></returns>
        private List<Order> GetCompletedOrders(User user, List<Workgroup> workgroups)
        {
            var tracking = _orderTrackingRepository.Queryable.Where(a => a.User == user).Select(a => a.Order).ToList();
            var orders = _orderRepository.Queryable.Where(a => tracking.Contains(a) && a.StatusCode.IsComplete);

            return orders.ToList();
        }
        #endregion

        /// <summary>
        /// Adds where clause to linq query to filter orders by status
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="isComplete"></param>
        /// <param name="orderStatusCode"></param>
        /// <param name="received">yes or no or null </param>
        /// <param name="paid">yes or no or null </param>
        /// <returns></returns>
        private IQueryable<OrderHistory> GetOrdersByStatus(IQueryable<OrderHistory> orders, bool isComplete = false, string orderStatusCode = null, string received = null, string paid = null)
        {
            if (orderStatusCode != null)
            {
                if (received != null && received.ToLower() == "yes")
                {
                    orders = orders.Where(o => o.StatusId == OrderStatusCode.Codes.Complete && o.Received == "Yes");
                }
                else if (received != null && received.ToLower() == "no")
                {
                    orders = orders.Where(o => o.StatusId == OrderStatusCode.Codes.Complete && o.Received == "No");
                }
                else if (paid != null && paid.ToLower() == "yes")
                {
                    orders = orders.Where(o => o.StatusId == OrderStatusCode.Codes.Complete && o.Paid == "Yes");
                }
                else if (paid != null && paid.ToLower() == "no")
                {
                    orders = orders.Where(o => o.StatusId == OrderStatusCode.Codes.Complete && o.Paid == "No");
                }
                else
                {
                    //just in case...
                    orders = orders.Where(o => o.StatusId == orderStatusCode);
                }
            }
            else if (isComplete)
            {
                orders = orders.Where(o => o.IsComplete);
            }

            return orders;
        }

        /// <summary>
        /// Adds where clause to linq query to filter by date
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private IQueryable<OrderHistory> GetOrdersByDate(IQueryable<OrderHistory> orders, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?(), DateTime? startLastActionDate = new DateTime?(), DateTime? endLastActionDate = new DateTime?())
        {
            if (startDate.HasValue)
            {
                orders = orders.Where(o => o.DateCreated > startDate.Value);
            }

            if (endDate.HasValue)
            {
                orders = orders.Where(o => o.DateCreated < endDate.Value);
            }

            if (startLastActionDate.HasValue)
            {
                orders = orders.Where(o => o.LastActionDate > startLastActionDate.Value);
            }

            if (endLastActionDate.HasValue)
            {
                orders = orders.Where(o => o.LastActionDate < endLastActionDate.Value);
            }

            return orders;
        }
    }
}