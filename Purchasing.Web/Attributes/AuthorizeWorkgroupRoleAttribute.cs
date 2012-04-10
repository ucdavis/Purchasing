using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Services;

namespace Purchasing.Web.Attributes
{
    public class AuthorizeWorkgroupRoleAttribute : AuthorizeAttribute
    {
        private readonly string _role; //TODO: Review making this a public readonly var so I can test for it with reflection

        public AuthorizeWorkgroupRoleAttribute(string Role)
        {
            _role = Role;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var roles = UserSecurityService.UserRoles(filterContext.HttpContext.User.Identity.Name);

            if (!roles.Contains(_role))
            {
                filterContext.Result = new HttpUnauthorizedResult(Resources.NoAccess);
            }

            base.OnAuthorization(filterContext);
        }
    }
}