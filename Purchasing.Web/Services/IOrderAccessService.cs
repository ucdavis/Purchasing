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
        IList<Order> GetListofOrders(bool allActive = false, bool all = false, bool owned = false, List<OrderStatusCode> orderStatusCodes = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?());
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

        public IList<Order> GetListofOrders(bool allActive = false, bool all = false, bool owned = false, List<OrderStatusCode> orderStatusCodes = null, DateTime? startDate = new DateTime?(), DateTime? endDate = new DateTime?())
        {
            // get the user
            var user = _userRepository.GetNullableById(_userIdentity.Current);

            // get the user's workgroups
            var workgroups = _workgroupPermissionRepository.Queryable.Where(a=>a.User == user).Select(a => a.Workgroup).Distinct().ToList();

            // always get the pending orders
            var orders = GetPendingOrders(user, workgroups);

            // get the active orders
            if (allActive)
            {
                orders.AddRange(GetActiveOrders(user, workgroups));
            }

            // get the completed
            if (all)
            {
                orders.AddRange(GetCompletedOrders(user, workgroups));
            }

            IEnumerable<Order> results = orders.Select(a => a);

            if (owned)
            {
                results = orders.Where(a => a.CreatedBy == user);
            }

            // apply the user's filters
            if (!all)
            {
                // only show non-complete orders
                results = results.Where(a => !a.StatusCode.IsComplete);
            }

            // apply status codes filter
            if (orderStatusCodes != null && orderStatusCodes.Count > 0)
            {
                results = results.Where(a => orderStatusCodes.Contains(a.StatusCode));
            }

            // begin date filter
            if (startDate.HasValue)
            {
                results = results.Where(a => a.DateCreated > startDate.Value);
            }

            // end date filter
            if (endDate.HasValue)
            {
                results = results.Where(a => a.DateCreated < endDate);
            }

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
            var permissions = _workgroupPermissionRepository.Queryable.Where(a => a.User == user);

            // get all approvals that are applicable
            var levels = permissions.Select(a => a.Role.Level).ToList();

            var approvals = (
                                from a in _approvalRepository.Queryable
                                where workgroups.Contains(a.Order.Workgroup)
                                    && levels.Contains(a.StatusCode.Level)
                                    && a.StatusCode == a.Order.StatusCode && !a.Approved.HasValue
                                    && (
                                        (a.User == null)    // not assigned, use workgroup
                                        ||
                                        (a.User == user || a.SecondaryUser == user) // user is assigned
                                        ||
                                        (a.StatusCode.Id != "CN" && a.User.IsAway)  // in standard approval, is user away
                                    )
                                select a.Order
                            ).ToList();

            var requestedOrders = _orderRepository.Queryable.Where(a => !a.StatusCode.IsComplete && a.CreatedBy == user).ToList();

            var orders = new List<Order>();
            orders.AddRange(approvals);
            orders.AddRange(requestedOrders);
            return orders.Distinct().ToList();
        }

        /// <summary>
        /// Gets all orders for which the user is in the tracking chain not including completed
        /// </summary>
        /// <returns></returns>
        private List<Order> GetActiveOrders(User user, List<Workgroup> workgroups)
        {
            var tracking = _orderTrackingRepository.Queryable.Where(a => a.User == user).Select(a => a.Order).ToList();
            var orders = _orderRepository.Queryable.Where(a => workgroups.Contains(a.Workgroup) && tracking.Contains(a) && !a.StatusCode.IsComplete);

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
            var orders = _orderRepository.Queryable.Where(a => workgroups.Contains(a.Workgroup) && tracking.Contains(a) && a.StatusCode.IsComplete);

            return orders.ToList();
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
