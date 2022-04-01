using System;
using System.Configuration;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Purchasing.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class IpFilter : Attribute, IAuthorizationFilter
    {
        private string localIpFilter = ConfigurationManager.AppSettings["IpFilter"];

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var clientIp = filterContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            // not authorized
            if (clientIp != localIpFilter)
            {
                filterContext.Result = new UnauthorizedObjectResult("You do not have access to this application");
            }

        }
    }
}