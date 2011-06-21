using System.Web.Mvc;

namespace OrAdmin.Web.Areas.System
{
    public class SystemAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "system";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "system_default",
                "system/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "OrAdmin.Web.Areas.System.Controllers" }
            );
        }
    }
}
