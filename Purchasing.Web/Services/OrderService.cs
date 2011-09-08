using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Will add the proper approval levels to an order.  If a workgroup account or approver/acctManager is passed in, a split is not possible
        /// </summary>
        /// <param name="order">The order.  If it does not contain splits, you must pass along either workgroupAccount or acctManager</param>
        /// <param name="workgroupAccountId">Optional workgroupAccountId of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        void AddApprovals(Order order, int? workgroupAccountId = null, string approverId = null, string accountManagerId = null);

        OrderStatusCode GetCurrentOrderStatus(int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepository<WorkgroupAccount> _workgroupAccountRepository;
        private readonly IRepositoryWithTypedId<Account, string> _accountRepository;
        private readonly IRepository<Approval> _approvalRepository;
        private readonly IRepositoryWithTypedId<OrderStatusCode, string> _orderStatusCodeRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;

        public OrderService(IRepository<Order> orderRepository,
            IRepository<Workgroup> workgroupRepository,
            IRepository<WorkgroupAccount> workgroupAccountRepository,
            IRepositoryWithTypedId<Account,string> accountRepository,
            IRepository<Approval> approvalRepository,
            IRepositoryWithTypedId<OrderStatusCode, string> orderStatusCodeRepository,
            IRepositoryWithTypedId<User, string> userRepository)
        {
            _orderRepository = orderRepository;
            _workgroupRepository = workgroupRepository;
            _workgroupAccountRepository = workgroupAccountRepository;
            _accountRepository = accountRepository;
            _approvalRepository = approvalRepository;
            _orderStatusCodeRepository = orderStatusCodeRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Will add the proper approval levels to an order.  If a workgroup account or approver/acctManager is passed in, a split is not possible
        /// </summary>
        /// <param name="order">The order.  If it does not contain splits, you must pass along either workgroupAccount or acctManager</param>
        /// <param name="workgroupAccountId">Optional workgroupAccountId of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        public void AddApprovals(Order order, int? workgroupAccountId = null, string approverId = null, string accountManagerId = null)
        {
            var approvalPeople = new ApprovalPeople();

            var hasSplits = order.Splits.Count > 0;

            if (!hasSplits)
            {
                Check.Require(workgroupAccountId.HasValue || !string.IsNullOrWhiteSpace(accountManagerId),
                          "You must either supply the ID of a valid workgroup account or provide the userId for an account manager");

                if (workgroupAccountId.HasValue) //if we route by account, use that for info
                {
                    var account = _workgroupAccountRepository.GetById(workgroupAccountId.Value);

                    approvalPeople.Approver = account.Approver;
                    approvalPeople.AcctManager = account.AccountManager;
                    approvalPeople.Purchaser = account.Purchaser;
                }
                else //else stick with user provided values
                {
                    approvalPeople.Approver = string.IsNullOrWhiteSpace(approverId) ? null : _userRepository.GetById(approverId);
                    approvalPeople.AcctManager = _userRepository.GetById(accountManagerId);
                }

                AddApprovalSteps(order, approvalPeople);
            }
            
            foreach (var split in order.Splits)
            {
                //Try to find the account in the workgroup so we can route it by users
                var account = _workgroupAccountRepository.Queryable.Where(x => x.Account.Id == split.Account.Id).FirstOrDefault();

                if (account != null)
                {
                    approvalPeople.Approver = account.Approver;
                    approvalPeople.AcctManager = account.AccountManager;
                    approvalPeople.Purchaser = account.Purchaser;
                }

                AddApprovalSteps(order, approvalPeople, split);
            }
        }

        /// <summary>
        /// Returns the current approval level that needs to be completed, or null if there are no approval steps pending
        /// </summary>
        /// <param name="orderId">Id of the order</param>
        public OrderStatusCode GetCurrentOrderStatus(int orderId)
        {
            var currentApprovalLevel = (from approval in _approvalRepository.Queryable
                                        where approval.Order.Id == orderId && !approval.Approved
                                        orderby approval.StatusCode.Level
                                        select approval.StatusCode).FirstOrDefault();
            return currentApprovalLevel;
        }

        /// <summary>
        /// Add in approval steps to either the order or split, depending on what is provided
        /// </summary>
        /// <param name="order">The order</param>
        /// <param name="approvalPeople">list of approval people (or null) to route to</param>
        /// <param name="split">optional split to approve against instead of the order</param>
        private void AddApprovalSteps(Order order, ApprovalPeople approvalPeople, Split split = null)
        {
            //Add in approval steps
            var approverApproval = new Approval
            {
                Approved = false,
                Level = 2, //TODO: is this redundant with status code?
                User = approvalPeople.Approver,
                StatusCode =
                    _orderStatusCodeRepository.Queryable.Where(
                        x => x.Id == OrderStatusCodeId.Approver).Single()
            };

            var acctManagerApproval = new Approval
            {
                Approved = false,
                Level = 3, //TODO: is this redundant with status code?
                User = approvalPeople.AcctManager,
                StatusCode =
                    _orderStatusCodeRepository.Queryable.Where(
                        x => x.Id == OrderStatusCodeId.AccountManager).Single()
            };

            var purchaserApproval = new Approval
            {
                Approved = false,
                Level = 4, //TODO: is this redundant with status code?
                User = approvalPeople.Purchaser,
                StatusCode =
                    _orderStatusCodeRepository.Queryable.Where(
                        x => x.Id == OrderStatusCodeId.Purchaser).Single()
            };

            if (split != null)
            {
                split.AddApproval(approverApproval);
                split.AddApproval(acctManagerApproval);
                split.AddApproval(purchaserApproval);
            }
            else
            {
                order.AddApproval(approverApproval);
                order.AddApproval(acctManagerApproval);
                order.AddApproval(purchaserApproval);
            }
        }
        
        private class ApprovalPeople
        {
            public ApprovalPeople()
            {
                Approver = null;
                AcctManager = null;
                Purchaser = null;
            }

            public User Approver { get; set; }
            public User AcctManager { get; set; }
            public User Purchaser { get; set; }
        }
    }
}