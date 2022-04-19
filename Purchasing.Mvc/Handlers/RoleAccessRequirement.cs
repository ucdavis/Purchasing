using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Purchasing.Mvc.Handlers
{
    public class RoleAccessRequirement : IAuthorizationRequirement
    {
        public readonly string[] RoleStrings;

        public RoleAccessRequirement(params string[] roleStrings)
        {
            RoleStrings = roleStrings;
        }
    }
}
