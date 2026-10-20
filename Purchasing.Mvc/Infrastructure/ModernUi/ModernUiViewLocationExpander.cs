using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Purchasing.Mvc.Infrastructure.ModernUi
{
    public sealed class ModernUiViewLocationExpander : IViewLocationExpander
    {
        private const string CacheKey = "modern-ui";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var httpContext = context.ActionContext.HttpContext;
            var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            var identity = httpContext.User.Identity;
            var modern = context.IsMainPage
                && string.IsNullOrEmpty(context.AreaName)
                && identity?.IsAuthenticated == true
                && !string.IsNullOrWhiteSpace(identity.Name)
                && configuration.GetValue<bool>("ModernUi:Enabled")
                && configuration.GetSection("ModernUi:AllowedLogins").GetChildren()
                    .Any(login => string.Equals(login.Value?.Trim(), identity.Name, StringComparison.OrdinalIgnoreCase));

            // Razor caches view locations. Separate the two interfaces, not individual users.
            context.Values[CacheKey] = modern ? "true" : "false";
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values[CacheKey] == "true")
                yield return "/Views/Modern/{1}/{0}.cshtml";

            foreach (var location in viewLocations)
                yield return location;
        }
    }
}
