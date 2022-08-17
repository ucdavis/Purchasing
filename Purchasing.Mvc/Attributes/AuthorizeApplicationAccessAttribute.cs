using System;
using System.Linq;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Purchasing.Mvc.Helpers;

namespace Purchasing.Mvc.Attributes
{
    /// <summary>
    /// Check for User access to any role
    /// </summary>
    public class AuthorizeApplicationAccessAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var roles = UserSecurityService.UserRoles(filterContext.HttpContext.User.Identity.Name);

            if (!roles.Any())
            {
                filterContext.Result = ViewHelper.NotAuthorized(Resources.NoAccess_Application);
            }
        }
    }
}