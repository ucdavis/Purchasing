using System.Web.Mvc;
using Purchasing.Mvc.Services;
using Purchasing.Web.Services;

namespace Purchasing.Mvc.Attributes
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