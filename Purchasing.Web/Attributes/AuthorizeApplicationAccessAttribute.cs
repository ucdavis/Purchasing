using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Services;

namespace Purchasing.Web.Attributes
{
    /// <summary>
    /// Check for User access to any role
    /// </summary>
    public class AuthorizeApplicationAccessAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var roles = UserSecurityService.UserRoles(filterContext.HttpContext.User.Identity.Name);

            if (!roles.Any())
            {
                filterContext.Result = new HttpUnauthorizedResult(Resources.NoAccess_Application);
            }
            
            base.OnAuthorization(filterContext);
        }
    }
}