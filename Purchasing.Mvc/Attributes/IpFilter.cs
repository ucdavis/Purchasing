using System;
using System.Configuration;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using UCDArch.Core;

namespace Purchasing.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class IpFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var localIpFilter = SmartServiceLocator<IConfiguration>.GetService().GetValue<string>("IpFilter");
            var clientIp = filterContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            // not authorized
            if (clientIp != localIpFilter)
            {
                filterContext.Result = new UnauthorizedObjectResult("You do not have access to this application");
            }

        }
    }
}