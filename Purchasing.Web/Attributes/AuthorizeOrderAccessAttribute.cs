using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Attributes
{
    public class AuthorizeOrderAccessAttribute : AuthorizeAttribute
    {
        private readonly OrderAccessLevel _accessLevel;
        private readonly ISecurityService _securityService;

        public AuthorizeOrderAccessAttribute(OrderAccessLevel accessLevel)
        {
            _accessLevel = accessLevel;
            _securityService = ServiceLocator.Current.GetInstance<ISecurityService>();
        }

        /// <summary>
        /// Authorize that the user has the desired access to the given order
        /// </summary>
        /// <remarks>
        /// First check access with vOpenAccess and then vClosedAccess if the user doesn't have access there
        /// </remarks>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var orderId = int.Parse((string)filterContext.RouteData.Values["id"]);

            OrderAccessLevel accessLevel;

            using (var ts = new TransactionScope())
            {
                accessLevel = _securityService.GetAccessLevel(orderId);
                ts.CommitTransaction();
            }

            if (!_accessLevel.HasFlag(accessLevel))
            {
                filterContext.Result = new HttpUnauthorizedResult(Resources.Authorization_PermissionDenied);
            }

            base.OnAuthorization(filterContext);
        }
    }

    public class AuthorizeEditOrderAttribute : AuthorizeOrderAccessAttribute
    {
        public AuthorizeEditOrderAttribute()
            : base(OrderAccessLevel.Edit)
        {

        }
    }

    public class AuthorizeReadOrderAttribute : AuthorizeOrderAccessAttribute
    {
        public AuthorizeReadOrderAttribute()
            : base(OrderAccessLevel.Readonly)
        {
            
        }
    }

    public class AuthorizeReadOrEditOrderAttribute : AuthorizeOrderAccessAttribute
    {
        public AuthorizeReadOrEditOrderAttribute()
            : base(OrderAccessLevel.Edit | OrderAccessLevel.Readonly)
        {

        }
    }
}