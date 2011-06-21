using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using OrAdmin.Core.Enums.App;
using OrAdmin.Core.Extensions;
using Telerik.Web.Mvc;

namespace OrAdmin.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Ingnore URL containing .axd... it's used for loading scripts
            // and other app resources
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //
            // Define custom routes here
            //

            // Custom route used by Application_AuthenticateRequest to redirect users to profile page
            routes.MapRoute(
                "Profile",
                "system/profile/{action}",
                new { controller = "Profile", action = "Index", id = UrlParameter.Optional },
                new string[] { "OrAdmin.Web.Areas.System.Controllers" }
            );

            // Custom routes and overrides should be defined BEFORE this catch all route 
            // or they won't take effect
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "OrAdmin.Web.Areas.Home.Controllers" }
            );
        }

        protected void Application_Start()
        {
            // This registers all the "areas" of the app. It should come before RegisterRoutes
            AreaRegistration.RegisterAllAreas();

            // This registers all of the custom routes defined above
            RegisterRoutes(RouteTable.Routes);

            //// Uncomment this to enable route debugging
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);

            // Register the sitemap with telerik's site map manager
            SiteMapManager.SiteMaps.Register<XmlSiteMap>(GlobalProperty.App.SiteMap.ToString(), sitemap => sitemap.LoadFrom("~/Web.sitemap"));

            // I like to keep my master pages and partials in subsequent folders
            // rather than all thrown into a single folder.
            var engine = (WebFormViewEngine)ViewEngines.Engines.First();
            engine.MasterLocationFormats = new string[] { 
                "~/Shared/Master/{0}.master", 
                "~/Shared/{1}/{0}.master"
            };
            engine.PartialViewLocationFormats = new string[] { 
                "~/Shared/Partial/{0}.ascx",
                "~/Shared/{1}/Partial/{0}.ascx",
                "~/Shared/Partial/{0}.aspx",
                "~/Shared/{1}/Partial/{0}.aspx"
            };

            // These additions allow me to route default requests for "~/" to the home area
            // NOTE: see http://stackoverflow.com/questions/3197460/asp-net-mvc-possible-to-move-home-to-area
            engine.ViewLocationFormats = new string[] { 
                "~/Views/{1}/{0}.aspx",
                "~/Views/{1}/{0}.ascx",
                "~/Areas/{1}/Views/{1}/{0}.aspx", // new
                "~/Areas/{1}/Views/{1}/{0}.ascx", // new
                "~/Areas/{1}/Views/{0}.aspx", // new
                "~/Areas/{1}/Views/{0}.ascx", // new
                "~/Shared/Views/{0}.aspx", // changed
                "~/Shared/Views/{0}.ascx", // changed
                "~/Shared/Views/{1}/{0}.aspx", // new
                "~/Shared/Views/{1}/{0}.ascx" // new
            };
        }

        protected void Application_AuthenticateRequest()
        {
            if (
                HttpContext.Current.User != null && 
                !HttpContext.Current.Request.Path.Contains(".") // Don't include resources (css, js, etc.)
                )
            {
                // Update the last activity date and set user to "online"
                Membership.GetUser(true);

                // Redirect to profile page if the current user does not have a profile
                // TODO: see http://stackoverflow.com/questions/3515475/asp-net-mvc-redirect-dilemma-on-application-authenticaterequest
                if (
                    !HttpContext.Current.User.HasProfile() && 
                    HttpContext.Current.Request.Path != "/system/profile")
                    Response.RedirectToRoute("Profile", new { returnUrl = HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Path) });
            }
        }
    }
}