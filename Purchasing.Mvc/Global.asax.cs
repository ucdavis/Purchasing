using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;

namespace Purchasing.Mvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AutomapperConfig.Configure();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            LogConfig.ConfigureLogging();

            TelemetryConfiguration.Active.InstrumentationKey =
                CloudConfigurationManager.GetSetting("ApplicationInsightsKey");
        }
    }
}
