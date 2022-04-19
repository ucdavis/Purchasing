using System.Security.Principal;
using System.Web;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Purchasing.Mvc;
using UCDArch.Core;

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
        public string Current { get { return SmartServiceLocator<IHttpContextAccessor>.GetService().HttpContext.User.Identity.Name; } }
        public IPrincipal CurrentPrincipal { get { return SmartServiceLocator<IHttpContextAccessor>.GetService().HttpContext.User; } }
        public bool IsUserInRole(string userId, string roleId)
        {
            return SmartServiceLocator<IRoleService>.GetService().IsUserInRole(userId, roleId);
        }

        public void RemoveUserRoleFromCache(string roleCacheId, string userId)
        {
            SmartServiceLocator<IHttpContextAccessor>.GetService().HttpContext.Session.Remove(string.Format(roleCacheId, userId));
        }

    }
}