using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Purchasing.Mvc;

namespace Purchasing.Mvc.Services
{
    public interface IUserIdentity
    {
        string Current { get; }
        IPrincipal CurrentPrincipal { get; }
        bool IsUserInRole(string userId, string roleId);
        void RemoveUserRoleFromCache(string roleCacheId, string userId);
    }

    public class UserIdentity : IUserIdentity
    {
        public string Current { get { return HttpContextHelper.Current.User.Identity.Name; } }
        public IPrincipal CurrentPrincipal { get { return HttpContextHelper.Current.User; } }
        public bool IsUserInRole(string userId, string roleId)
        {
            return Roles.IsUserInRole(userId, roleId);
        }

        public void RemoveUserRoleFromCache(string roleCacheId, string userId)
        {
            HttpContextHelper.Current.Session.Remove(string.Format(roleCacheId, userId));
        }

    }
}