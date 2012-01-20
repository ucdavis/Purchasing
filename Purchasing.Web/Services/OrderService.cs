using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using AutoMapper;

namespace Purchasing.Web.Services
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
        void CreateApprovalsForNewOrder(Order order, int[] conditionalApprovalIds = null, string accountId = null, string approverId = null, string accountManagerId = null);
        
        /// <summary>
        /// Recreates approvals for the given order, removing all approvals at or above the current order level
        /// Should not affect conditional approvals?
        /// </summary>
        void ReRouteApprovalsForExistingOrder(Order order);

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
        void Approve(Order order);

        OrderStatusCode GetCurrentOrderStatus(int orderId);

        /// <summary>
        /// //get the lowest status code that still needs to be approved
        /// </summary>
        OrderStatusCode GetCurrentOrderStatus(Order order);

        /// <summary>
        /// Handle editing an existing order without any rerouting
        /// </summary>
        void EditExistingOrder(Order order);
    }

    public class OrderService : IOrderService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IEventService _eventService;
        private readonly IOrderAccessService _orderAccessService;
        private readonly IUserIdentity _userIdentity;

        public OrderService(IRepositoryFactory repositoryFactory, IEventService eventService, IOrderAccessService orderAccessService, IUserIdentity userIdentity)
        {
            _repositoryFactory = repositoryFactory;
            _eventService = eventService;
            _orderAccessService = orderAccessService;
            _userIdentity = userIdentity;
        }

        /// <summary>
        /// Will add the proper approval levels to an order.  If a workgroup account or approver/acctManager is passed in, a split is not possible
        /// </summary>
        /// <param name="order">The order.  If it does not contain splits, you must pass along either workgroupAccount or acctManager</param>
        /// <param name="conditionalApprovalIds">The Ids of required conditional approvals for this order (the ones answered "yes")</param>
        /// <param name="accountId">Optional id of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        public void CreateApprovalsForNewOrder(Order order, int[] conditionalApprovalIds = null, string accountId = null, string approverId = null, string accountManagerId = null)
        {
            var approvalInfo = new ApprovalInfo();

            if (order.Splits.Count() == 1) //Order has one split and can thus optionally not have accounts assigned
            {
                var split = order.Splits.Single();

                Check.Require(!string.IsNullOrWhiteSpace(accountId) || !string.IsNullOrWhiteSpace(accountManagerId),
                          "You must either supply the ID of a valid account or provide the userId for an account manager");

                if (!string.IsNullOrWhiteSpace(accountId)) //if we route by account, use that for info
                {
                    approvalInfo.AccountId = accountId;

                    var workgroupAccount =
                        _repositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Account.Id == accountId).FirstOrDefault();

                    if (workgroupAccount != null) //route to the people contained in the workgroup account info
                    {
                        approvalInfo.Approver = workgroupAccount.Approver;
                        approvalInfo.AcctManager = workgroupAccount.AccountManager;
                        approvalInfo.Purchaser = workgroupAccount.Purchaser;
                    }
                    
                    split.Account = accountId; //Assign the account to the split
                }
                else //else stick with user provided values
                {
                    approvalInfo.Approver = string.IsNullOrWhiteSpace(approverId) ? null : _repositoryFactory.UserRepository.GetById(approverId);
                    approvalInfo.AcctManager = _repositoryFactory.UserRepository.GetById(accountManagerId);
                }

                AddApprovalSteps(order, approvalInfo, split);
            }
            else //else order has multiple splits and each one needs an account
            {
                foreach (var split in order.Splits)
                {
                    //Try to find the account in the workgroup so we can route it by users
                    var account = _repositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Account.Id == split.Account).FirstOrDefault();

                    if (account != null)
                    {
                        approvalInfo.AccountId = account.Account.Id; //the underlying accountId
                        approvalInfo.Approver = account.Approver;
                        approvalInfo.AcctManager = account.AccountManager;
                        approvalInfo.Purchaser = account.Purchaser;
                    }

                    AddApprovalSteps(order, approvalInfo, split);
                }   
            }

            //If we were passed conditional approval info, go ahead and add them
            if (conditionalApprovalIds != null && conditionalApprovalIds.Count() > 0)
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
                                            secondaryApproverId = x.SecondaryApprover.Id
                                        }
                            ).Single();

                    var newApproval = new Approval //Add a new 'approver' level approval
                                          {
                                              Completed = false,
                                              User = _repositoryFactory.UserRepository.GetById(approverIds.primaryApproverId),
                                              SecondaryUser = approverIds.secondaryApproverId == null ? null : _repositoryFactory.UserRepository.GetById(approverIds.secondaryApproverId),
                                              StatusCode =
                                                  _repositoryFactory.OrderStatusCodeRepository.Queryable.Where(
                                                      x => x.Id == OrderStatusCode.Codes.ConditionalApprover).Single()
                                          };

                    order.AddApproval(newApproval);//Add directly to the order since conditional approvals never go against splits
                }
            }
            
            order.StatusCode = GetCurrentOrderStatus(order);

            _eventService.OrderCreated(order); //Creating approvals means the order is being created
        }

        /// <summary>
        /// Recreates approvals for the given order, removing all approvals at or above the current order level
        /// Should not affect conditional approvals?
        /// </summary>
        public void ReRouteApprovalsForExistingOrder(Order order)
        {
            var currentLevel = order.StatusCode.Level;

            //Remove all approvals at the current level or above
            foreach (var split in order.Splits)
            {
                var approvalsToRemove =
                    split.Approvals.Where(x =>
                        x.StatusCode.Level >= currentLevel &&
                        x.StatusCode.Id != OrderStatusCode.Codes.ConditionalApprover).ToList();
                
                foreach (var approval in approvalsToRemove)
                {
                    split.Approvals.Remove(approval);
                }
            }

            //recreate approvals
            foreach (var split in order.Splits)
            {
                var approvalInfo = new ApprovalInfo();

                //Try to find the account in the workgroup so we can route it by users
                var account = _repositoryFactory.WorkgroupAccountRepository.Queryable.FirstOrDefault(x => x.Account.Id == split.Account);

                if (account != null)
                {
                    approvalInfo.AccountId = account.Account.Id; //the underlying accountId
                    approvalInfo.Approver = account.Approver;
                    approvalInfo.AcctManager = account.AccountManager;
                    approvalInfo.Purchaser = account.Purchaser;
                }

                AddApprovalSteps(order, approvalInfo, split, currentLevel);
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

        /// <summary>
        /// //get the lowest status code that still needs to be approved
        /// </summary>
        public OrderStatusCode GetCurrentOrderStatus(Order order)
        {
            var lowestSplitApproval = (from o in order.Splits
                    let splitApprovals = o.Approvals
                    from a in splitApprovals
                    where !a.Completed
                    orderby a.StatusCode.Level
                    select a.StatusCode).First();

            var lowestDirectApproval =
                (from a in order.Approvals where !a.Completed orderby a.StatusCode.Level select a.StatusCode).
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
        public void Approve(Order order)
        {
            var currentApprovalLevel = order.StatusCode.Level;

            var hasRolesInThisOrdersWorkgroup = _orderAccessService.GetAccessLevel(order) == OrderAccessLevel.Edit;

            //If the approval is at the current level & directly associated with the user (primary or secondary), go ahead and approve it
            foreach (var approvalForUserDirectly in
                        order.Approvals.Where(
                            x => x.StatusCode.Level == currentApprovalLevel
                                && !x.Completed
                                && (
                                    (x.User != null && x.User.Id == _userIdentity.Current)
                                    || (x.SecondaryUser != null && x.SecondaryUser.Id == _userIdentity.Current)
                                    )
                                )
                    )
            {
                approvalForUserDirectly.Completed = true;
                _eventService.OrderApproved(order, approvalForUserDirectly);
            }

            if (hasRolesInThisOrdersWorkgroup)
            {
                //If the approval is at the current level and has no user is attached, it can be approved by this workgroup user
                foreach (
                    var approvalForWorkgroup in
                        order.Approvals.Where(x => x.StatusCode.Level == currentApprovalLevel && !x.Completed && x.User == null))
                {
                    approvalForWorkgroup.Completed = true;
                    _eventService.OrderApproved(order, approvalForWorkgroup);
                }

                //If the approval is at the current level, and the users are away, it can be approved by this workgroup user
                foreach (
                    var approvalForAway in
                        order.Approvals.Where(a =>
                            a.StatusCode.Level == currentApprovalLevel &&
                            !a.Completed &&
                            a.User != null &&
                            a.User.IsAway &&
                            (a.SecondaryUser == null || (a.SecondaryUser != null && a.SecondaryUser.IsAway))))
                {
                    approvalForAway.Completed = true;
                    _eventService.OrderApproved(order, approvalForAway);
                }
            }

            //Now if there are no more approvals pending at this level, move the order up a level or complete it
            if (order.Approvals.Any(x => x.StatusCode.Level == currentApprovalLevel && !x.Completed) == false)
            {
                var nextStatusCode =
                    _repositoryFactory.OrderStatusCodeRepository.Queryable.Single(x => x.Level == (currentApprovalLevel + 1));

                order.StatusCode = nextStatusCode;
                _eventService.OrderStatusChange(order, nextStatusCode);
            }
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
                                   PoNumber = order.PoNumber,
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
                                                _repositoryFactory.OrderStatusCodeRepository.GetById(OrderStatusCode.Codes.AccountManager)
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
                split.AssociateApproval(approval);

                if (approval.Completed)
                {
                    //already appoved means auto approval, so send that specific event
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
        /// <param name="split">The split this autoapproval will be associated with</param>
        /// <param name="approver">The assocaited approver</param>
        private bool AutoApprovable(Order order, Split split, User approver)
        {
            if (approver == null) return false; //Only auto approve when assigned to a specific approver

            if (approver.Id == _userIdentity.Current) return true; //Auto approved if the approver is the current user

            var total = split.Amount;
            var accountId = split.Account ?? string.Empty;

            //See if there are any automatic approvals for this user/account.
            var possibleAutomaticApprovals =
                _repositoryFactory.AutoApprovalRepository.Queryable
                    .Where(x => x.IsActive && x.Expiration > DateTime.Now) //valid only if it is active and isn't expired yet
                    .Where(x=>x.User.Id == approver.Id) //auto approval must have been created by the approver
                    .Where(x=>x.TargetUser.Id == order.CreatedBy.Id || x.Account.Id == accountId)//either applies to the order creator or account
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
        }
    }
}