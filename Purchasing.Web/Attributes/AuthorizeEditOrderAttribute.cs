using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.Services;

namespace Purchasing.Web.Attributes
{
    public class AuthorizeEditOrderAttribute : AuthorizeAttribute
    {
        private readonly IOrderAccessService _orderAccessService;

        public AuthorizeEditOrderAttribute()
        {
            _orderAccessService = ServiceLocator.Current.GetInstance<IOrderAccessService>();
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var orderId = int.Parse((string)filterContext.RouteData.Values["id"]);
            var accessLevel = _orderAccessService.GetAccessLevel(orderId);

            if (accessLevel != OrderAccessLevel.Edit)
            {
                filterContext.Result = new HttpUnauthorizedResult("You do not have access to edit this order");
            }

            base.OnAuthorization(filterContext);
        }
    }
}