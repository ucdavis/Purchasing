using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using Org.BouncyCastle.Security;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.App_GlobalResources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Services
{
    public struct RolesAndAccessLevel
    {
        public OrderAccessLevel OrderAccessLevel { get; set; }
        public HashSet<string> Roles { get; set; }
    }

    [Flags]
    public enum OrderAccessLevel
    {
        None = 1,
        Readonly = 2,
        Edit = 4
    }

    public interface ISecurityService
    {
        /// <summary>
        /// Checks if the current user has access to the workgroup or the organization.
        /// </summary>
        /// <param name="workgroup"></param>
        /// <param name="organization"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool HasWorkgroupOrOrganizationAccess(Workgroup workgroup, Organization organization, out string message);

        /// <summary>
        /// Checks if the current user has access to the workgroup
        /// </summary>
        /// <param name="workgroup"></param>
        /// <returns></returns>
        bool HasWorkgroupAccess(Workgroup workgroup);

        bool HasWorkgroupEditAccess(int id, out string message);

        /// <summary>
        /// Checks if a user is in a particular role for a workgroup
        /// </summary>
        /// <param name="roleCode">Role Id</param>
        /// <param name="workgroupId">Workgroup Id</param>
        /// <returns>True, if in role, False, if not.</returns>
        bool IsInRole(string roleCode, int workgroupId);

        /// <summary>
        /// Checks if a user is in a particular role for a workgroup
        /// </summary>
        /// <param name="role">Role</param>
        /// <param name="workgroup">Workgroup</param>
        /// <returns>True, if in role, False, if not.</returns>
        bool IsInRole(Role role, Workgroup workgroup);

        /// <summary>
        /// Checks if a user is in a particular role for a workgroup, includes check for admin workgroups
        /// </summary>
        /// <param name="roleCode"></param>
        /// <param name="workgroupId"></param>
        /// <returns></returns>
        bool hasWorkgroupRole(string roleCode, int workgroupId);
        
        /// <summary>
        /// Checks if a user is an admin for that level.  
        /// </summary>
        /// <param name="roleCode"></param>
        /// <param name="workgroupId"></param>
        /// <returns></returns>
        bool hasAdminWorkgroupRole(string roleCode, int workgroupId);
        

        /// <summary>
        /// Get the current user's access to the order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        OrderAccessLevel GetAccessLevel(Order order, bool? closed = null);
        /// <summary>
        /// Get the current user's access to the order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        OrderAccessLevel GetAccessLevel(int orderId, bool? closed = null);

        /// <summary>
        /// Finds or creates a user object as necessary
        /// </summary>
        /// <param name="kerb"></param>
        /// <returns>Null, if kerb does not return result from ldap or db</returns>
        User GetUser(string kerb);

        RolesAndAccessLevel GetAccessRoleAndLevel(Order order);

        /// <summary>
        /// Checks only read access for orders.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool HasReadAccess(int orderId);
        /// <summary>
        /// Checks only edit access for oders.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool HasEditAccess(int orderId);
    }

    public class SecurityService :  ISecurityService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IUserIdentity _userIdentity;
        private readonly IDirectorySearchService _directorySearchService;

        public SecurityService(IRepositoryFactory repositoryFactory, IUserIdentity userIdentity, IDirectorySearchService directorySearchService, IQueryRepositoryFactory queryRepositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _userIdentity = userIdentity;
            _directorySearchService = directorySearchService;
            _queryRepositoryFactory = queryRepositoryFactory;
        }

        // ===================================================
        // Workgroup Access Functions
        // ===================================================
        public bool HasWorkgroupOrOrganizationAccess(Workgroup workgroup, Organization organization, out string message)
        {
            if(workgroup == null && organization == null)
            {
                message = "Workgroup and Organization not found.";
                return false;
            }

            var query = _repositoryFactory.UserRepository.Queryable.Where(a => a.Id == _userIdentity.Current);
            if (workgroup != null) query = query.Fetch(a => a.WorkgroupPermissions);
            if (organization != null) query = query.Fetch(a => a.Organizations);

            var user = query.Single();
            var isAdmin = user.Roles.Any(a => a.Id == Role.Codes.DepartmentalAdmin);

            if(workgroup != null)
            {
                // workgroups the user just has access to
                var workgroupIds = user.WorkgroupPermissions.Select(a => a.Workgroup.Id).ToList();

                // workgroups allowed through admin
                if (isAdmin)
                {
                    var orgs = user.Organizations.Select(a => a.Id).ToList();

                    var adminOrgs = _queryRepositoryFactory.AdminWorkgroupRepository.Queryable
                            .Where(a => orgs.Contains(a.RollupParentId));

                    var temp = adminOrgs.Select(a => a.WorkgroupId).ToList();

                    workgroupIds.AddRange(temp);
                }

                if(!workgroupIds.Contains(workgroup.Id))
                {
                    message = Resources.NoAccess_Workgroup;
                    return false;
                }
            }
            else
            {
                var orgIds = user.Organizations.Select(x => x.Id).ToList();

                if (isAdmin)
                {
                    var adminOrgs = _queryRepositoryFactory.OrganizationDescendantRepository.Queryable.Where(a => orgIds.Contains(a.RollupParentId));

                    orgIds.AddRange(adminOrgs.Select(a => a.OrgId).ToList());
                }

                if(!orgIds.Contains(organization.Id))
                {
                    message = Resources.NoAccess_Org;
                    return false;
                }
            }

            message = null;
            return true;
        }

        public bool HasWorkgroupAccess(Workgroup workgroup)
        {
            //return true; //TODO: do the actual check once GetWorkgroups is checked?
            string message;

            return HasWorkgroupOrOrganizationAccess(workgroup, null, out message);
        }

        /// <summary>
        /// This is based on the user's organizational access
        /// </summary>
        /// <param name="id">workgroup id</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HasWorkgroupEditAccess(int id, out string message)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                message = "Workgroup not found";
                return false;
            }
            return HasWorkgroupOrOrganizationAccess(null, workgroup.PrimaryOrganization, out message);
        }

        public bool IsInRole(string roleCode, int workgroupId)
        {
            var role = _repositoryFactory.RoleRepository.GetNullableById(roleCode);
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId);

            // invalid role code was provided
            if (role == null)
            {
                throw new ArgumentException("Role code not found.");
            }

            // invalid workgroup
            if (workgroup == null)
            {
                throw new ArgumentException("Workgroup was not found.");
            }

            return IsInRole(role, workgroup);
        }

        public bool IsInRole(Role role, Workgroup workgroup)
        {
            Check.Require(role != null, "role is required.");
            Check.Require(workgroup != null, "workgroup is required.");
            var user = _repositoryFactory.UserRepository.Queryable.Where(x => x.Id == _userIdentity.Current).Fetch(x => x.Organizations).Single();

            return workgroup.Permissions.Any(a => a.User == user && a.Role == role);
        }

        public bool hasWorkgroupRole(string roleCode, int workgroupId)
        {
            return _queryRepositoryFactory.WorkgroupRoleRepository.Queryable.Any(a => a.AccessUserId == _userIdentity.Current && a.RoleId == roleCode && a.WorkgroupId == workgroupId);
        }

        public bool hasAdminWorkgroupRole(string roleCode, int workgroupId)
        {
            return _queryRepositoryFactory.WorkgroupRoleRepository.Queryable.Any(a => a.AccessUserId == _userIdentity.Current && a.RoleId == roleCode && a.WorkgroupId == workgroupId && a.IsAdmin);
        }

        public RolesAndAccessLevel GetAccessRoleAndLevel(Order order)
        {
            Check.Require(order != null, "order is required.");

            var access = _queryRepositoryFactory.AccessRepository.Queryable.Where(a => a.OrderId == order.Id && a.AccessUserId == _userIdentity.Current).ToList();

            if (access.Any())
            {
                var roles = new HashSet<string>(access.Select(x => x.AccessLevel));
                
                if (access.Any(x => x.EditAccess))
                {
                    return new RolesAndAccessLevel {OrderAccessLevel = OrderAccessLevel.Edit, Roles = roles};
                }

                if (access.Any(x => x.ReadAccess))
                {
                    return new RolesAndAccessLevel { OrderAccessLevel = OrderAccessLevel.Readonly, Roles = roles };
                }
            }

            return new RolesAndAccessLevel { OrderAccessLevel = OrderAccessLevel.None, Roles = new HashSet<string>() };
        }

        // ===================================================
        // Order Access Functions
        // ===================================================
        public OrderAccessLevel GetAccessLevel(int orderId, bool? closed = null)
        {
            // closed orders can only have read access, never edit access.
            if (closed.HasValue && closed.Value)
            {
                if (HasReadAccess(orderId))
                {
                    return OrderAccessLevel.Readonly;
                }

                // if it's closed and you don't have read...i'm pretty sure there shouldn't be access
                return OrderAccessLevel.None;
            }

            var access = _queryRepositoryFactory.AccessRepository.Queryable.Where(a => a.OrderId == orderId && a.AccessUserId == _userIdentity.Current).ToList();

            if (access.Any())
            {
                if (access.Any(x => x.EditAccess))
                {
                    return OrderAccessLevel.Edit;
                }

                if (access.Any(x => x.ReadAccess))
                {
                    return OrderAccessLevel.Readonly;
                }
            }

            // default no access
            return OrderAccessLevel.None;

        }

        public OrderAccessLevel GetAccessLevel(Order order, bool? closed = null)
        {
            Check.Require(order != null, "order is required.");

            return GetAccessLevel(order.Id, closed);
        }

        public bool HasReadAccess(int orderId)
        {
            return _queryRepositoryFactory.ReadAccessRepository.Queryable.Any(a => a.OrderId == orderId && a.AccessUserId == _userIdentity.Current);
        }

        public bool HasEditAccess(int orderId)
        {
            return _queryRepositoryFactory.EditAccessRepository.Queryable.Any(a => a.OrderId == orderId && a.AccessUserId == _userIdentity.Current);
        }

        // ===================================================
        // User Functions
        // ===================================================
        public User GetUser(string kerb)
        {
            if (!string.IsNullOrWhiteSpace(kerb))
            {
                kerb = kerb.ToLower(); //So without this, if it finds the user, it has an upper case ID even if the found user id is lower case
            }
            var user = _repositoryFactory.UserRepository.GetNullableById(kerb);

            if (user == null)
            {
                var ldapUser = _directorySearchService.FindUser(kerb);

                if (ldapUser != null)
                {
                    user = new User(kerb);
                    user.FirstName = ldapUser.FirstName;
                    user.LastName = ldapUser.LastName;
                    user.Email = ldapUser.EmailAddress;

                    _repositoryFactory.UserRepository.EnsurePersistent(user, false);
                }
            }

            return user;
        }
    }

    public class UserSecurityService
    {
        /// <summary>
        /// Returns a list of user's roles (ignoring workgroup)
        /// </summary>
        /// <remarks>
        /// Used to help determine menu context
        /// </remarks>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<string> UserRoles(string userId)
        {
            var repositoryFactory = ServiceLocator.Current.GetInstance<IRepositoryFactory>();

            var context = HttpContext.Current;

            // cached value exists?
            var cacheId = string.Format(Resources.Role_CacheId, userId);
            var cRoles = (List<string>)context.Session[cacheId];
            if (cRoles != null)
            {
                return cRoles;
            }

            // no cached values, find the roles
            var roles = new List<string>();

            using (var ts = new TransactionScope())
            {
                var user = repositoryFactory.UserRepository.GetNullableById(userId);

                if (user == null)
                {
                    return roles;
                }

                // get the admin type roles
                roles.AddRange(user.Roles.Select(a => a.Id).Distinct().ToList());

                // load all the permissions
                var permissions = repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(a => a.User == user).Fetch(a => a.Workgroup).ToList();

                // get the regular roles
                roles.AddRange(permissions.Where(a => !a.Workgroup.Administrative).Select(a => a.Role.Id).Distinct().ToList());

                // has role in an administrative workgroup
                if (permissions.Any(a => a.Workgroup.Administrative))
                {
                    roles.Add(Role.Codes.AdminWorkgroup);
                }

                // is ad hoc account manager, used to give access to ldap search and other ajax functions that are being secured.  They have no other roles in the system
                if (!roles.Any() && repositoryFactory.ApprovalRepository.Queryable.Where(a => a.User == user || a.SecondaryUser == user).Any())
                {
                    roles.Add(Role.Codes.AdhocAccountManager);
                }

                ts.CommitTransaction();
            }

            // save the roles into the cache
            // var expiration = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day);
            context.Session.Add(cacheId, roles);
            context.Session.Add(cacheId,roles);

            return roles;
        }
    }

}