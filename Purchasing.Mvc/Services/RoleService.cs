using System;
using System.Configuration.Provider;
using System.Data.Odbc;
using System.Linq;
using Dapper;
using CommonServiceLocator;
using Purchasing.Core.Services;
using Purchasing.Mvc.Services;
using System.Data.Common;

namespace Purchasing.Mvc.Services
{

    // Was originally PurchasingRoleProvider
    public interface IRoleService
    {
        public bool IsUserInRole(string username, string roleName, bool showInactive = false);
        public string[] GetRolesForUser(string username);
        public void CreateRole(string roleName);
        public bool DeleteRole(string roleName, bool throwOnPopulatedRole);
        public bool RoleExists(string roleName);
        public void AddUsersToRoles(string[] usernames, string[] roleNames);
        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames);
        public string[] GetUsersInRole(string roleName);
        public string[] GetAllRoles();
        public string[] FindUsersInRole(string roleName, string usernameToMatch);
    }

    public class RoleService: IRoleService
    {
        private readonly IDbService _dbService;

        public RoleService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public bool IsUserInRole(string username, string roleName, bool showInactive = false)
        {
            using (var conn = _dbService.GetConnection())
            {
                if (showInactive)
                {
                    var result2 = conn.Query<int>(
                        @"select count(UserId) from Permissions inner join Users on Permissions.UserId = Users.Id 
                        where UserId = @username and RoleId = @rolename",
                    new { username, rolename = roleName });

                    return result2.Single() > 0; //Is there more than zero users associated with that role?
                }
                
                var result = conn.Query<int>(
                    @"select count(UserId) from Permissions inner join Users on Permissions.UserId = Users.Id 
                        where Users.IsActive = 1 and UserId = @username and RoleId = @rolename",
                    new { username, rolename = roleName });

                return result.Single() > 0; //Is there more than zero users associated with that role?
            }
        }

        public string[] GetRolesForUser(string username)
        {
            using (var conn = _dbService.GetConnection())
            {
                var result =
                    conn.Query<string>(
                        "select RoleId from Permissions inner join Users on Permissions.UserId = Users.Id where Users.IsActive = 1 and UserId = @username",
                        new {username});

                return result.ToArray();
            }
        }

        public void CreateRole(string roleName)
        {
            throw new InvalidOperationException("Cannot create roles through the role provider");
        }

        public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            using (var conn = _dbService.GetConnection())
            {
                if (throwOnPopulatedRole)
                {
                    //Need to check to see if the role is populated
                    var usersInRole = conn.Query<int>("select count(UserId) from Permissions where RoleId = @rolename",
                                    new {rolename = roleName});

                    if (usersInRole.Single() > 0) throw new ProviderException("The role cannot be deleted because it is currently populated");
                }

                try
                {
                    conn.Execute("delete from Permissions where RoleId = @rolename", new { rolename = roleName });//Delete users in role
                    conn.Execute("delete from Roles where RoleId = @rolename", new { rolename = roleName });//Delete the role
                }
                catch (DbException)
                {
                    return false;
                }

                return true;
            }
        }

        public bool RoleExists(string roleName)
        {
            using (var conn = _dbService.GetConnection())
            {
                var result = conn.Query<int>("select count(RoleId) from Roles where RoleId = @rolename",
                                             new {rolename = roleName});
                
                return result.Single() > 0;
            }
        }

        public void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (var conn = _dbService.GetConnection())
            {
                foreach (string username in usernames)
                {
                    foreach (string rolename in roleNames)
                    {
                        conn.Execute("insert into Permissions (UserId, RoleId) values (@username, @rolename)",
                                     new {username, rolename});

                    }
                }
            }
        }

        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (var conn = _dbService.GetConnection())
            {
                foreach (string username in usernames)
                {
                    foreach (string rolename in roleNames)
                    {
                        conn.Execute("delete from Permissions where UserId = @username and RoleId = @rolename",
                                     new {username, rolename});

                    }
                }
            }
        }

        public string[] GetUsersInRole(string roleName)
        {
            using (var conn = _dbService.GetConnection())
            {
                var result =
                    conn.Query<string>(
                        "select UserId from Permissions inner join Users on Permissions.UserId = Users.Id where Users.IsActive = 1 and RoleId = @rolename",
                        new {rolename = roleName});

                return result.ToArray();
            }
        }

        public string[] GetAllRoles()
        {
            using (var conn = _dbService.GetConnection())
            {
                var result = conn.Query<string>("select Id from Roles");

                return result.ToArray();
            }
        }

        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            using (var conn = _dbService.GetConnection())
            {
                var result =
                    conn.Query<string>(
                        "select UserId from Permissions inner join Users on Permissions.UserId = Users.Id where Users.IsActive = 1 and RoleId = @rolename and UserId like %@username%",
                        new {rolename = roleName, username = usernameToMatch});

                return result.ToArray();
            }
        }

        public string ApplicationName
        {
            get { return "Purchasing"; }
            set { throw new InvalidOperationException("You are not allowed to set the application name"); }
        }
    }
}