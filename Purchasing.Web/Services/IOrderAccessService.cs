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
    }

    public class OrderAccessService : IOrderAccessService
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;

        public OrderAccessService(IUserIdentity userIdentity, IRepositoryWithTypedId<User, string> userRepository )
        {
            _userIdentity = userIdentity;
            _userRepository = userRepository;
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
