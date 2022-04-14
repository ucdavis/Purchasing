using System;
using System.Web;
using Purchasing.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Mvc.Services
{
    public interface IServerLink
    {
        string Address { get; }
    }
    
    public class ServerLink : IServerLink
    {
        public ServerLink(IUrlHelper urlHelper)
        {
            var scheme = urlHelper.ActionContext.HttpContext.Request.Scheme;
            Address = urlHelper.Action("Lookup", "Order", null, scheme);
        }

        public string Address { get; }
    }
}