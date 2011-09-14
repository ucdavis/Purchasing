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
    public interface IOrderService
    {
        OrderStatusCode GetCurrentOrderStatus(int orderId);

        /// <summary>
        /// Will add the proper approval levels to an order.  If a workgroup account or approver/acctManager is passed in, a split is not possible
        /// </summary>
        /// <param name="order">The order.  If it does not contain splits, you must pass along either workgroupAccount or acctManager</param>
        /// <param name="conditionalApprovalIds">The Ids of required conditional approvals for this order (the ones answered "yes")</param>
        /// <param name="workgroupAccountId">Optional workgroupAccountId of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        void AddApprovals(Order order, int[] conditionalApprovalIds = null, int? workgroupAccountId = null, string approverId = null, string accountManagerId = null);

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
        void Approve(Order order, string userId);
    }

    public class OrderService : IOrderService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IEventService _eventService;

        public OrderService(IRepositoryFactory repositoryFactory, IEventService eventService)
        {
            _repositoryFactory = repositoryFactory;
            _eventService = eventService;
        }

        /// <summary>
        /// Will add the proper approval levels to an order.  If a workgroup account or approver/acctManager is passed in, a split is not possible
        /// </summary>
        /// <param name="order">The order.  If it does not contain splits, you must pass along either workgroupAccount or acctManager</param>
        /// <param name="conditionalApprovalIds">The Ids of required conditional approvals for this order (the ones answered "yes")</param>
        /// <param name="workgroupAccountId">Optional workgroupAccountId of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        public void AddApprovals(Order order, int[] conditionalApprovalIds = null, int? workgroupAccountId = null, string approverId = null, string accountManagerId = null)
        {
            var approvalInfo = new ApprovalInfo();

            var hasSplits = order.Splits.Count > 0;

            if (!hasSplits)
            {
                Check.Require(workgroupAccountId.HasValue || !string.IsNullOrWhiteSpace(accountManagerId),
                          "You must either supply the ID of a valid workgroup account or provide the userId for an account manager");

                if (workgroupAccountId.HasValue) //if we route by account, use that for info
                {
                    var account = _repositoryFactory.WorkgroupAccountRepository.GetById(workgroupAccountId.Value);

                    approvalInfo.AccountId = account.Account.Id; //the underlying accountId
                    approvalInfo.Approver = account.Approver;
                    approvalInfo.AcctManager = account.AccountManager;
                    approvalInfo.Purchaser = account.Purchaser;
                }
                else //else stick with user provided values
                {
                    approvalInfo.Approver = string.IsNullOrWhiteSpace(approverId) ? null : _repositoryFactory.UserRepository.GetById(approverId);
                    approvalInfo.AcctManager = _repositoryFactory.UserRepository.GetById(accountManagerId);
                }

                AddApprovalSteps(order, approvalInfo);
            }
            
            foreach (var split in order.Splits)
            {
                //Try to find the account in the workgroup so we can route it by users
                var account = _repositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Account.Id == split.Account.Id).FirstOrDefault();

                if (account != null)
                {
                    approvalInfo.AccountId = account.Account.Id; //the underlying accountId
                    approvalInfo.Approver = account.Approver;
                    approvalInfo.AcctManager = account.AccountManager;
                    approvalInfo.Purchaser = account.Purchaser;
                }

                AddApprovalSteps(order, approvalInfo, split);
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
                                              Approved = false,
                                              Level = 2,
                                              //TODO: is this redundant with status code?
                                              User = _repositoryFactory.UserRepository.GetById(approverIds.primaryApproverId),
                                              StatusCode =
                                                  _repositoryFactory.OrderStatusCodeRepository.Queryable.Where(
                                                      x => x.Id == OrderStatusCodeId.Approver).Single()
                                          };

                    order.AddApproval(newApproval);//Add directly to the order since conditional approvals never go against splits
                }
            }
        }

        /// <summary>
        /// Returns the current approval level that needs to be completed, or null if there are no approval steps pending
        /// </summary>
        /// <param name="orderId">Id of the order</param>
        /// <remarks>TODO: I think we can get rid of this and just query order.StatusCode as long as it is up to date</remarks>
        public OrderStatusCode GetCurrentOrderStatus(int orderId)
        {
            var currentApprovalLevel = (from approval in _repositoryFactory.ApprovalRepository.Queryable
                                        where approval.Order.Id == orderId && !approval.Approved
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
        /// <param name="userId">Currently logged in user who clicked "I Approve"</param>
        public void Approve(Order order, string userId)
        {
            var currentApprovalLevel = order.StatusCode.Level;

            //TODO: check to make sure we aren't already at the highest approval level

            //TODO: would it be easier to "level" the roles, like approver = 2, acctManager = 3???
            //First find out if the user has access to the order's workgroup (TODO: AT THE CURRENT LEVEL)
            var hasRolesInThisOrdersWorkgroup =
                _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
                    x => x.Workgroup.Id == order.Workgroup.Id && x.User.Id == userId).Any();

            //If the approval is at the current level & directly associated with the user, go ahead and approve it
            foreach (var approvalForUserDirectly in order.Approvals.Where(x => x.StatusCode.Level == currentApprovalLevel && (x.User != null && x.User.Id == userId)))
            {
                approvalForUserDirectly.Approved = true;
                _eventService.OrderApproved(order, approvalForUserDirectly);
            }

            if (hasRolesInThisOrdersWorkgroup)
            {
                //If the approval is at the current level and has no user is attached, it can be approve by this workgroup user
                foreach (var approvalForWorkgroup in order.Approvals.Where(x => x.StatusCode.Level == currentApprovalLevel && x.User == null))
                {
                    approvalForWorkgroup.Approved = true;
                    _eventService.OrderApproved(order, approvalForWorkgroup);
                }
            }

            //Now if there are no more approvals pending at this level, move the order up a level or complete it
            if (order.Approvals.Where(x => x.StatusCode.Level == currentApprovalLevel && x.Approved == false).Any() == false)
            {
                var nextStatusCode =
                    _repositoryFactory.OrderStatusCodeRepository.Queryable.Where(
                        x => x.Level == (currentApprovalLevel + 1)).Single();

                order.StatusCode = nextStatusCode;
                _eventService.OrderStatusChange(order, nextStatusCode);
            }
        }

        /// <summary>
        /// Add in approval steps to either the order or split, depending on what is provided
        /// </summary>
        /// <param name="order">The order</param>
        /// <param name="approvalInfo">list of approval people (or null) to route to</param>
        /// <param name="split">optional split to approve against instead of the order</param>
        private void AddApprovalSteps(Order order, ApprovalInfo approvalInfo, Split split = null)
        {
            var approvals = new List<Approval>
                                {
                                    new Approval
                                        {
                                            Approved = AutoApprovable(order, approvalInfo.AccountId), 
                                            //If this is auto approvable just include it but mark it as approval already
                                            Level = 2,
                                            //TODO: is this redundant with status code?
                                            User = approvalInfo.Approver,
                                            StatusCode =
                                                _repositoryFactory.OrderStatusCodeRepository.Queryable.Where(
                                                    x => x.Id == OrderStatusCodeId.Approver).Single()
                                        },
                                    new Approval
                                        {
                                            Approved = false,
                                            Level = 3,
                                            //TODO: is this redundant with status code?
                                            User = approvalInfo.AcctManager,
                                            StatusCode =
                                                _repositoryFactory.OrderStatusCodeRepository.Queryable.Where(
                                                    x => x.Id == OrderStatusCodeId.AccountManager).Single()
                                        },
                                    new Approval
                                        {
                                            Approved = false,
                                            Level = 4,
                                            //TODO: is this redundant with status code?
                                            User = approvalInfo.Purchaser,
                                            StatusCode =
                                                _repositoryFactory.OrderStatusCodeRepository.Queryable.Where(
                                                    x => x.Id == OrderStatusCodeId.Purchaser).Single()
                                        }
                                };

            foreach (var approval in approvals)
            {
                if (split != null)
                {
                    split.AddApproval(approval);
                }
                else
                {
                    order.AddApproval(approval);
                }

                _eventService.OrderApprovalAdded(order, approval);
            }
        }

        /// <summary>
        /// Calculate the automatic approvals-- if any apply mark that approval level as complete
        /// </summary>
        /// <param name="order">The order</param>
        /// <param name="accountId">optional accountId</param>
        private bool AutoApprovable(Order order, string accountId = null)
        {
            var orderTotal = order.Total();

            //TODO: I think we need to know what user created an order, like order.InitiatedBy, non nullable
            var userForNow = "postit"; //TODO: Changed once we associate a user with an order

            //See if there are any automatic approvals for this user/account
            var possibleAutomaticApprovals =
                _repositoryFactory.AutoApprovalRepository.Queryable.Where(x => x.TargetUser.Id == userForNow || x.Account.Id == accountId).ToList();

            foreach (var autoApproval in possibleAutomaticApprovals) //for each autoapproval, check if they apply.  If any do, return true
            {
                if (autoApproval.Equal)
                {
                    if (orderTotal == autoApproval.MaxAmount)
                    {
                        return true;
                    }
                }
                else //less than
                {
                    if (orderTotal < autoApproval.MaxAmount)
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