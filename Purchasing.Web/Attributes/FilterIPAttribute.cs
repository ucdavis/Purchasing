using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Purchasing.Web.Attributes
{
    public class FilterIPAttribute : AuthorizeAttribute
    {
        //List of the IP Addresses allow. Delimited by semicolon
        public string AllowedIPs { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            if (string.IsNullOrWhiteSpace(AllowedIPs))
            {
                throw new ArgumentException("No Allowed IP Addresses Configured-- you must allowed someone!");
            }

            return AllowedIPs.Split(';').Contains(httpContext.Request.UserHostAddress);
        }
    }
}