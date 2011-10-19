using System;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Services
{
    public interface IOrderAccessService
    {
        /// <summary>
        /// Get the current user's access to the order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        OrderAccessLevel GetAccessLevel(Order order);

        /// <summary>
        /// Get the current user's list of orders.
        /// </summary>
        /// <param name="all">Get all orders pending, completed, cancelled</param>
        /// <param name="orderStatusCodes">Get all orders with current status codes in this list</param>
        /// <param name="startDate">Get all orders after this date</param>
        /// <param name="endDate">Get all orders before this date</param>
        /// <returns>List of orders according to the criteria</returns>
        IList<Order> GetListofOrders(bool all = false, List<OrderStatusCode> orderStatusCodes = null, DateTime? startDate = null, DateTime? endDate = null );
    }

    public class OrderAccessService : IOrderAccessService
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly IRepository<Approval> _approvalRepository;
        private readonly IRepository<OrderTracking> _orderTrackingRepository;

        public OrderAccessService(IUserIdentity userIdentity, IRepositoryWithTypedId<User, string> userRepository, IRepository<Order> orderRepository, IRepository<WorkgroupPermission> workgroupPermissionRepository, IRepository<Approval> approvalRepository, IRepository<OrderTracking> orderTrackingRepository  )
        {
            _userIdentity = userIdentity;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _workgroupPermissionRepository = workgroupPermissionRepository;
            _approvalRepository = approvalRepository;
            _orderTrackingRepository = orderTrackingRepository;
        }

        public OrderAccessLevel GetAccessLevel(Order order)
        {
            Check.Require(order != null, "order is required.");

            var workgroup = order.Workgroup;
            var user = _userRepository.GetNullableById(_userIdentity.Current);

            // current order status
            var currentStatus = order.StatusCode;

            // get the user's role in the workgroup
            var permissions = workgroup.Permissions.Where(a => a.User == user).ToList();

            // current approvals
            var approvals = order.Approvals.Where(a => a.StatusCode == currentStatus && !a.Approved.HasValue).ToList();

            // check for edit access
            if (HasEditAccess(order, approvals, permissions, currentStatus, user))
            {
                return OrderAccessLevel.Edit;
            }

            // check for read access
            if (HasReadAccess(order, order.OrderTrackings, permissions, user))
            {
                return OrderAccessLevel.Readonly;
            }

            // default no access
            return OrderAccessLevel.None;
        }

        public IList<Order> GetListofOrders(bool all = false, List<OrderStatusCode> orderStatusCodes = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?())
        {
            // get the user
            var user = _userRepository.GetNullableById(_userIdentity.Current);

            // get the user's workgroups
            var permissions = _workgroupPermissionRepository.Queryable.Where(a => a.User == user);
            var workgroups = permissions.Select(a => a.Workgroup).ToList();

            // get all approvals that are applicable
            var levels = permissions.Select(a => a.Role.Level).ToList();
            var approvals = _approvalRepository.Queryable.Where(a => workgroups.Contains(a.Order.Workgroup) && levels.Contains(a.StatusCode.Level) && a.StatusCode == a.Order.StatusCode);

            // approvals with no one assigned
            approvals = approvals.Where(a => a.User == null && a.SecondaryUser == null);
            // approvals that are marked as away
            approvals = approvals.Where(a => (a.User != null && a.User != user && a.User.IsAway) && (a.SecondaryUser != null && a.SecondaryUser != user && a.SecondaryUser.IsAway));
            // approvals assigned specifically to our user
            approvals = approvals.Where(a => a.User == user || a.SecondaryUser == user);

            var approvalList = approvals.ToList();

            var ordersByApproval = _orderRepository.Queryable.Where(a => approvalList.Select(b => b.Order).Contains(a)).ToList();
            
            // get orders that are not explicitely by approvals
            var tracking = _orderTrackingRepository.Queryable.Where(a => a.User == user).Select(a => a.Order).ToList();
            var orders = _orderRepository.Queryable.Where(a=> workgroups.Contains(a.Workgroup) && tracking.Contains(a));

            // apply the user's filters
            if (!all)
            {
                // only show non-complete orders
                orders = orders.Where(a => !a.StatusCode.IsComplete);
            }

            // apply status codes filter
            if (orderStatusCodes != null && orderStatusCodes.Count > 0)
            {
                orders = orders.Where(a => orderStatusCodes.Contains(a.StatusCode));
            }

            // begin date filter
            if (startDate.HasValue)
            {
                orders = orders.Where(a => a.DateCreated > startDate.Value);
            }

            // end date filter
            if (endDate.HasValue)
            {
                orders = orders.Where(a => a.DateCreated < endDate);
            }

            var result = orders.ToList();
            result.AddRange(ordersByApproval);

            return result.Distinct().ToList();
        }
        
        // checks if the user is the current person to review the order
        private bool HasEditAccess(Order order, IEnumerable<Approval> approvals, IEnumerable<WorkgroupPermission> permissions, OrderStatusCode currentStatus, User user )
        {
            // there exists at least one at the current level that is not tied to a user
            if (approvals.Any(a => a.User == null && a.SecondaryUser == null))
            {
                // the user has a matching role level to the current one and qualitfies for workgroup permissions
                if (permissions.Any(a => a.Role.Level == currentStatus.Level))
                {
                    return true;
                }
            }

            // there exists at least one at the current level that is tied to a user
            if (approvals.Any(a => a.User != null || a.SecondaryUser != null))
            {
                // is one the current user?
                if (approvals.Any(a => a.User == user || a.SecondaryUser == user)) return true;

                // is the user away? and not the current person
                if (approvals.Any(a => a.User.IsAway && (a.SecondaryUser == null || (a.SecondaryUser != null && a.SecondaryUser.IsAway))))
                {
                    // user is away for an approval, check workgroup permissions
                    if (permissions.Any(a => a.Role.Level == currentStatus.Level))
                    {
                        return true;
                    }
                }
            }

            // is the user explicitely defined at the current level of approval
            if (approvals.Any(a => a.User == user || a.SecondaryUser == user))
            {
                return true;
            }

            return false;
        }

        // checks if the user has access to the permissions to the workgroup or performed something in the order
        private bool HasReadAccess(Order order, IEnumerable<OrderTracking> trackings, IEnumerable<WorkgroupPermission> permissions, User user)
        {
            return permissions.Count() > 0 || trackings.Any(a => a.User == user);
        }
    }

    public enum OrderAccessLevel { None, Readonly, Edit }
}
