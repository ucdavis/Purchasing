using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Purchasing.Mvc.Services;

namespace Purchasing.Mvc.Handlers
{
    public class VerifyRoleAccessHandler : AuthorizationHandler<RoleAccessRequirement>
    {
        private readonly IRoleService _roleService;

        public VerifyRoleAccessHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAccessRequirement requirement)
        {
            var userId = context.User.Identity.Name;

            if (requirement.RoleStrings.Any(role => _roleService.IsUserInRole(userId, role)))
            {
                context.Succeed(requirement);
            }
            else 
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }

    }
}