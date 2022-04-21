using System;
using System.Web;
using Purchasing.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Purchasing.Mvc.Services
{
    public interface IServerLink
    {
        string Address { get; }
    }
    
    public class ServerLink : IServerLink
    {
        public ServerLink(IHttpContextAccessor contextAccessor, LinkGenerator linkGenerator)
        {
            Address = linkGenerator.GetUriByAction(contextAccessor.HttpContext, "Lookup", "Order");
        }

        public string Address { get; }
    }
}