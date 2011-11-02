using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Mapping;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;

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
        /// <param name="allActive"></param>
        /// <param name="all">Get all orders pending, completed, cancelled</param>
        /// <param name="owned"></param>
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


        public OrderAccessService(IUserIdentity userIdentity, IRepositoryWithTypedId<User, string> userRepository, IRepository<Order> orderRepository, IRepository<WorkgroupPermission> workgroupPermissionRepository, IRepository<Approval> approvalRepository, IRepository<OrderTracking> orderTrackingRepository )
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
            var approvals = order.Approvals.Where(a => a.StatusCode.Level == currentStatus.Level && !a.Completed).ToList();

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
                var levels = orderStatusCodes.Select(a => a.Level).ToList();
                results = results.Where(a => levels.Contains(a.StatusCode.Level));
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
