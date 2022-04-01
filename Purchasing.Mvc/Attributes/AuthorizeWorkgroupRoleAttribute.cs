using System;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Purchasing.Mvc.Attributes
{
    public class AuthorizeWorkgroupRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _role; //TODO: Review making this a public readonly var so I can test for it with reflection

        public AuthorizeWorkgroupRoleAttribute(string Role)
        {
            _role = Role;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var roles = UserSecurityService.UserRoles(filterContext.HttpContext.User.Identity.Name);

            if (!roles.Contains(_role))
            {
                filterContext.Result = new UnauthorizedObjectResult(Resources.NoAccess);
            }
        }
    }
}