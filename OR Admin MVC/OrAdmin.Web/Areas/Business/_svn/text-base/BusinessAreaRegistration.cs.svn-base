using System.Web.Mvc;
using OrAdmin.Core.RouteConstraints;

namespace OrAdmin.Web.Areas.Business
{
    public class BusinessAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "business";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // Admin filters
            context.MapRoute(
                "Admin Filters",
                "business/purchasing/admin/{filter}",
                new { controller = "Purchasing", action = "AdminFiltered" },
                new string[] { "OrAdmin.Web.Areas.Business.Controllers" }
            );

            // Request details
            context.MapRoute(
                "Request Details",
                "business/purchasing/my-requests/{requestUniqueId}",
                new { controller = "Purchasing", action = "MyRequestDetails" },
                new { requestUniqueId = new GuidConstraint() },
                new string[] { "OrAdmin.Web.Areas.Business.Controllers" }
            );

            // Request filters
            context.MapRoute(
                "Request Filters",
                "business/purchasing/my-requests/{filter}",
                new { controller = "Purchasing", action = "MyRequestsFiltered" },
                new string[] { "OrAdmin.Web.Areas.Business.Controllers" }
            );

            // Dpo details
            context.MapRoute(
                "Dpo Details",
                "business/purchasing/request/dpo/{requestUniqueId}/{mode}",
                new { controller = "Purchasing", action = "Dpo", mode = UrlParameter.Optional, requestUniqueId = UrlParameter.Optional },
                new { requestUniqueId = new GuidConstraint() },
                new string[] { "OrAdmin.Web.Areas.Business.Controllers" }
            );

            // Dro
            context.MapRoute(
                "Dro",
                "business/purchasing/request/dro/",
                new { controller = "Purchasing", action = "Dro" },
                new string[] { "OrAdmin.Web.Areas.Business.Controllers" }
            );

            // Ba
            context.MapRoute(
                "Ba",
                "business/purchasing/request/ba/",
                new { controller = "Purchasing", action = "Ba" },
                new string[] { "OrAdmin.Web.Areas.Business.Controllers" }
            );

            context.MapRoute(
                "Business Default",
                "business/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "OrAdmin.Web.Areas.Business.Controllers" }
            );
        }
    }
}
