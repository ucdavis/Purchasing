using System;
using CommonServiceLocator;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UCDArch.Core;

namespace Purchasing.Mvc.Attributes
{
    public class AuthorizeWorkgroupAccessAttribute : Attribute, IAuthorizationFilter
    {
        private readonly ISecurityService _securityService;

        public AuthorizeWorkgroupAccessAttribute()
        {
            _securityService = SmartServiceLocator<ISecurityService>.GetService();
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var workgroupId = 0;
            try
            {
                if (filterContext.RouteData.Values.ContainsKey("id"))
                {
                    workgroupId = int.Parse((string) filterContext.RouteData.Values["id"]);
                }
                else if (filterContext.RouteData.Values.ContainsKey("workgroupId"))
                {
                    workgroupId = int.Parse((string)filterContext.RouteData.Values["workgroupId"]);
                }
            }
            catch (Exception)
            {
                workgroupId = 0;
            }
            if(workgroupId == 0) //We let this past because the workgroup has not been created and we will redirect within the methods
            {
                return;
            }            
            bool hasAccess;
            var message = string.Empty;

            using (var ts = new TransactionScope())
            {
                hasAccess = _securityService.HasWorkgroupEditAccess(workgroupId, out message);
                ts.CommitTransaction();                
            }
            if(!hasAccess)
            {
                if(string.IsNullOrWhiteSpace(message))
                {
                    message = "You do not have access to edit this workgroup";
                }
                filterContext.Result = new UnauthorizedObjectResult(message);
            }
        }
    }
}