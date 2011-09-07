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
        /// <param name="accountId">Optional workgroupAccountId of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        void AddApprovalsWithNoSplits(Order order, int? accountId = null, string approverId = null, string accountManagerId = null);
    }

    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepository<WorkgroupAccount> _workgroupAccountRepository;
        private readonly IRepositoryWithTypedId<OrderStatusCode, string> _orderStatusCodeRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;

        public OrderService(IRepository<Order> orderRepository,
            IRepository<Workgroup> workgroupRepository,
            IRepository<WorkgroupAccount> workgroupAccountRepository,
            IRepositoryWithTypedId<OrderStatusCode, string> orderStatusCodeRepository,
            IRepositoryWithTypedId<User, string> userRepository)
        {
            _orderRepository = orderRepository;
            _workgroupRepository = workgroupRepository;
            _workgroupAccountRepository = workgroupAccountRepository;
            _orderStatusCodeRepository = orderStatusCodeRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Creates the proper approval routing and attaches it to the order, depending on the parameters passed in.
        /// </summary>
        /// <param name="order">The order to affect</param>
        /// <param name="accountId">Optional workgroupAccountId of an account to use for routing</param>
        /// <param name="approverId">Optional approver userID</param>
        /// <param name="accountManagerId">AccountManager userID, required if account is not supplied</param>
        public void AddApprovalsWithNoSplits(Order order, int? accountId = null, string approverId = null, string accountManagerId = null)
        {
            Check.Require(accountId.HasValue || !string.IsNullOrWhiteSpace(accountManagerId),
                          "You must either supply the ID of a valid workgroup account or provide the userId for an account manager");

            //Add in approvals for selected options
            User approver, acctManager, purchaser = null;

            if (accountId.HasValue) //if we route by account, use that for info
            {
                var account = _workgroupAccountRepository.GetById(accountId.Value);

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