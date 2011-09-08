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
        /// Creates the proper approval routing and attaches it to the order, depending on the parameters passed in.
        /// </summary>
        /// <param name="order">The order to affect</param>
        /// <param name="workgroupAccountId">Optional workgroupAccountId of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        void AddApprovalsWithNoSplits(Order order, int? workgroupAccountId = null, string approverId = null, string accountManagerId = null);

        /// <summary>
        /// Will add the proper approval levels to an order containing splits.
        /// </summary>
        /// <param name="order">The order with valid split objects</param>
        void AddApprovalsToOrderWithSplits(Order order);
    }

    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepository<WorkgroupAccount> _workgroupAccountRepository;
        private readonly IRepositoryWithTypedId<Account, string> _accountRepository;
        private readonly IRepositoryWithTypedId<OrderStatusCode, string> _orderStatusCodeRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;

        public OrderService(IRepository<Order> orderRepository,
            IRepository<Workgroup> workgroupRepository,
            IRepository<WorkgroupAccount> workgroupAccountRepository,
            IRepositoryWithTypedId<Account,string> accountRepository,
            IRepositoryWithTypedId<OrderStatusCode, string> orderStatusCodeRepository,
            IRepositoryWithTypedId<User, string> userRepository)
        {
            _orderRepository = orderRepository;
            _workgroupRepository = workgroupRepository;
            _workgroupAccountRepository = workgroupAccountRepository;
            _accountRepository = accountRepository;
            _orderStatusCodeRepository = orderStatusCodeRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Will add the proper approval levels to an order containing splits.
        /// </summary>
        /// <param name="order">The order with valid split objects</param>
        public void AddApprovalsToOrderWithSplits(Order order)
        {
            Check.Require(order.Splits.Count() > 0, "Supplied order must contain splits");

            foreach (var split in order.Splits)
            {
                //Add in approvals for selected options
                User approver = null, acctManager = null, purchaser = null;

                //Try to find the account in the workgroup so we can route it by users
                var account = _workgroupAccountRepository.Queryable.Where(x => x.Account.Id == split.Account.Id).FirstOrDefault();

                if (account != null)
                {
                    approver = account.Approver;
                    acctManager = account.AccountManager;
                    purchaser = account.Purchaser;
                }

                //Add in approval steps
                var approverApproval = new Approval
                {
                    Approved = false,
                    Level = 2, //TODO: is this redundant with status code?
                    User = approver,
                    StatusCode =
                        _orderStatusCodeRepository.Queryable.Where(
                            x => x.Id == OrderStatusCodeId.Approver).Single()
                };

                var acctManagerApproval = new Approval
                {
                    Approved = false,
                    Level = 3, //TODO: is this redundant with status code?
                    User = acctManager,
                    StatusCode =
                        _orderStatusCodeRepository.Queryable.Where(
                            x => x.Id == OrderStatusCodeId.AccountManager).Single()
                };

                var purchaserApproval = new Approval
                {
                    Approved = false,
                    Level = 4, //TODO: is this redundant with status code?
                    User = purchaser,
                    StatusCode =
                        _orderStatusCodeRepository.Queryable.Where(
                            x => x.Id == OrderStatusCodeId.Purchaser).Single()
                };

                split.AddApproval(approverApproval);
                split.AddApproval(acctManagerApproval);
                split.AddApproval(purchaserApproval);
            }
        }

        /// <summary>
        /// Creates the proper approval routing and attaches it to the order, depending on the parameters passed in.
        /// </summary>
        /// <param name="order">The order to affect</param>
        /// <param name="workgroupAccountId">Optional workgroupAccountId of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        public void AddApprovalsWithNoSplits(Order order, int? workgroupAccountId = null, string approverId = null, string accountManagerId = null)
        {
            Check.Require(workgroupAccountId.HasValue || !string.IsNullOrWhiteSpace(accountManagerId),
                          "You must either supply the ID of a valid workgroup account or provide the userId for an account manager");

            //Add in approvals for selected options
            User approver, acctManager, purchaser = null;

            if (workgroupAccountId.HasValue) //if we route by account, use that for info
            {
                var account = _workgroupAccountRepository.GetById(workgroupAccountId.Value);

                approver = account.Approver;
                acctManager = account.AccountManager;
                purchaser = account.Purchaser;
            }
            else //else stick with user provided values
            {
                approver = string.IsNullOrWhiteSpace(approverId) ? null : _userRepository.GetById(approverId);
                acctManager = _userRepository.GetById(accountManagerId);
            }

            //Add in approval steps
            var approverApproval = new Approval
            {
                Approved = false,
                Level = 2, //TODO: is this redundant with status code?
                User = approver,
                StatusCode =
                    _orderStatusCodeRepository.Queryable.Where(
                        x => x.Id == OrderStatusCodeId.Approver).Single()
            };

            var acctManagerApproval = new Approval
            {
                Approved = false,
                Level = 3, //TODO: is this redundant with status code?
                User = acctManager,
                StatusCode =
                    _orderStatusCodeRepository.Queryable.Where(
                        x => x.Id == OrderStatusCodeId.AccountManager).Single()
            };

            var purchaserApproval = new Approval
            {
                Approved = false,
                Level = 4, //TODO: is this redundant with status code?
                User = purchaser,
                StatusCode =
                    _orderStatusCodeRepository.Queryable.Where(
                        x => x.Id == OrderStatusCodeId.Purchaser).Single()
            };

            order.AddApproval(approverApproval);
            order.AddApproval(acctManagerApproval);
            order.AddApproval(purchaserApproval);
        }
    }
}