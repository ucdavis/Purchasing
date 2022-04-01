using System;
using System.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using CommonServiceLocator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Purchasing.Mvc.Attributes
{
    public class AuthorizeRequesterInOrderWorkgroupAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IRepositoryFactory _repositoryFactory;
        
        public AuthorizeRequesterInOrderWorkgroupAttribute()
        {
            _repositoryFactory = ServiceLocator.Current.GetInstance<IRepositoryFactory>();
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var orderId = int.Parse((string)filterContext.RouteData.Values["id"]);
            var user = filterContext.HttpContext.User.Identity.Name;

            bool hasRequestorPermission = false;

            using (var ts = new TransactionScope())
            {
                var permissions = from o in _repositoryFactory.OrderRepository.Queryable
                                  where o.Id == orderId
                                  join p in _repositoryFactory.WorkgroupPermissionRepository.Queryable on o.Workgroup.Id equals p.Workgroup.Id
                                  where p.User.Id == user && p.Role.Id == Role.Codes.Requester
                                  select p;

                hasRequestorPermission = permissions.Any();

                ts.CommitTransaction();
            }

            if (!hasRequestorPermission)
            {
                filterContext.Result = new UnauthorizedObjectResult("You do not have access to copy this order");
            }
        }
    }
}