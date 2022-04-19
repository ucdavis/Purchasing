using System;
using CommonServiceLocator;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UCDArch.Core;

namespace Purchasing.Mvc.Attributes
{
    public class AuthorizeOrderAccessAttribute : Attribute, IAuthorizationFilter
    {
        private readonly OrderAccessLevel _accessLevel;
        private readonly ISecurityService _securityService;

        public AuthorizeOrderAccessAttribute(OrderAccessLevel accessLevel)
        {
            _accessLevel = accessLevel;
            _securityService = SmartServiceLocator<ISecurityService>.GetService();
        }

        /// <summary>
        /// Authorize that the user has the desired access to the given order
        /// </summary>
        /// <remarks>
        /// First check access with vOpenAccess and then vClosedAccess if the user doesn't have access there
        /// </remarks>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationFilterContext filterContext)
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
                filterContext.Result = new UnauthorizedObjectResult(Resources.Authorization_PermissionDenied);
            }
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