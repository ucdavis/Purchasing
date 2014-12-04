using System;
using System.Configuration;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Routing;

namespace Purchasing.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class IpFilter : FilterAttribute, IAuthorizationFilter
    {
        private string localIpFilter = ConfigurationManager.AppSettings["IpFilter"];

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var clientIp = filterContext.RequestContext.HttpContext.Request.UserHostAddress;

            // not authorized
            if (clientIp != localIpFilter)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "error", action = "notauthorized" }));
            }

        }
    }
}