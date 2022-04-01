using System;
using System.Configuration.Provider;
using System.Data.Odbc;
using System.Linq;
using System.Web.Security;
using Dapper;
using CommonServiceLocator;
using Purchasing.Core.Services;
using Purchasing.Mvc.Services;

namespace Purchasing.Mvc.Providers
{
    public class PurchasingRoleProvider : RoleProvider
    {
        protected IDbService DbService { get; set; }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            DbService = ServiceLocator.Current.GetInstance<IDbService>();
            base.Initialize(name, config);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (var conn = DbService.GetConnection())
            {
                var result = conn.Query<int>(
                    @"select count(UserId) from Permissions inner join Users on Permissions.UserId = Users.Id 
                        where Users.IsActive = 1 and UserId = @username and RoleId = @rolename",
                    new {username, rolename = roleName});

                return result.Single() > 0; //Is there more than zero users associated with that role?
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var conn = DbService.GetConnection())
            {
                var result =
                    conn.Query<string>(
                        "select RoleId from Permissions inner join Users on Permissions.UserId = Users.Id where Users.IsActive = 1 and UserId = @username",
                        new {username});

                return result.ToArray();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new InvalidOperationException("Cannot create roles through the role provider");
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            using (var conn = DbService.GetConnection())
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
                catch (OdbcException)
                {
                    return false;
                }

                return true;
            }
        }

        public override bool RoleExists(string roleName)
        {
            using (var conn = DbService.GetConnection())
            {
                var result = conn.Query<int>("select count(RoleId) from Roles where RoleId = @rolename",
                                             new {rolename = roleName});
                
                return result.Single() > 0;
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (var conn = DbService.GetConnection())
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

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (var conn = DbService.GetConnection())
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

        public override string[] GetUsersInRole(string roleName)
        {
            using (var conn = DbService.GetConnection())
            {
                var result =
                    conn.Query<string>(
                        "select UserId from Permissions inner join Users on Permissions.UserId = Users.Id where Users.IsActive = 1 and RoleId = @rolename",
                        new {rolename = roleName});

                return result.ToArray();
            }
        }

        public override string[] GetAllRoles()
        {
            using (var conn = DbService.GetConnection())
            {
                var result = conn.Query<string>("select Id from Roles");

                return result.ToArray();
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            using (var conn = DbService.GetConnection())
            {
                var result =
                    conn.Query<string>(
                        "select UserId from Permissions inner join Users on Permissions.UserId = Users.Id where Users.IsActive = 1 and RoleId = @rolename and UserId like %@username%",
                        new {rolename = roleName, username = usernameToMatch});

                return result.ToArray();
            }
        }

        public override string ApplicationName
        {
            get { return "Purchasing"; }
            set { throw new InvalidOperationException("You are not allowed to set the application name"); }
        }
    }
}