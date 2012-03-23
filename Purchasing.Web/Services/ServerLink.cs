using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Purchasing.Web.Services
{
    public interface IServerLink
    {
        string Address { get; }
    }
    
    public class ServerLink : IServerLink
    {
        public string Address
        {
            get { return "<a href=\"http://" + HttpContext.Current.Request.Url.Host + "/Order/Lookup/{0}\">{1}</a>"; }
        }
    }
}