using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Attributes
{
    public class AuthorizeOrderAccessAttribute : AuthorizeAttribute
    {
        private readonly OrderAccessLevel _accessLevel;
        private readonly IOrderAccessService _orderAccessService;

        public AuthorizeOrderAccessAttribute(OrderAccessLevel accessLevel)
        {
            _accessLevel = accessLevel;
            _orderAccessService = ServiceLocator.Current.GetInstance<IOrderAccessService>();
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var orderId = int.Parse((string)filterContext.RouteData.Values["id"]);

            OrderAccessLevel accessLevel;

            using (var ts = new TransactionScope())
            {
                accessLevel = _orderAccessService.GetAccessLevel(orderId);
                ts.CommitTransaction();
            }

            if (!_accessLevel.HasFlag(accessLevel))
            {
                filterContext.Result = new HttpUnauthorizedResult("You do not have access to edit this order");
            }

            base.OnAuthorization(filterContext);
        }
    }

    public class AuthorizeEditOrder : AuthorizeOrderAccessAttribute
    {
        public AuthorizeEditOrder()
            : base(OrderAccessLevel.Edit)
        {

        }
    }

    public class AuthorizeReadOrder : AuthorizeOrderAccessAttribute
    {
        public AuthorizeReadOrder() : base(OrderAccessLevel.Readonly)
        {
            
        }
    }

    public class AuthorizeReadOrEditOrder : AuthorizeOrderAccessAttribute
    {
        public AuthorizeReadOrEditOrder() : base(OrderAccessLevel.Edit | OrderAccessLevel.Readonly)
        {

        }
    }
}