using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Elmah;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.Controllers;
using StackExchange.Profiling;
using UCDArch.Data.NHibernate;
using UCDArch.Web.IoC;
using UCDArch.Web.ModelBinder;
using Purchasing.Core.Domain;

namespace Purchasing.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
#if DEBUG
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
#endif

           // RegisterRoutes(RouteTable.Routes);
            new RouteConfigurator().RegisterRoutes();

            ModelBinders.Binders.DefaultBinder = new UCDArchModelBinder();

            AutomapperConfig.Configure();

            NHibernateSessionConfiguration.Mappings.UseFluentMappings(typeof(Approval).Assembly);

            IWindsorContainer container = InitializeServiceLocator();

            //TODO: Uncomment to enable the audit interceptors
            //NHibernateSessionManager.Instance.RegisterInterceptor(container.Resolve<IInterceptor>());

//#if DEBUG
//            if (WebConfigurationManager.AppSettings["ResetDb"] != "false")
//            {
//                DbHelper.ResetDatabase(WebConfigurationManager.AppSettings["DemoMode"] == "true", WebConfigurationManager.AppSettings["BlankDb"] == "true"); //TODO: Only reset db on debug
//            }
//#endif

            InitProfilerSettings();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            //Removes "Session state has created a session id" httpException
            string sessionId = Session.SessionID;
        }

        private static IWindsorContainer InitializeServiceLocator()
        {
            IWindsorContainer container = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            container.RegisterControllers(typeof(HomeController).Assembly);
            ComponentRegistrar.AddComponentsTo(container);

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return container;
        }

        private static void InitProfilerSettings()
        {
            //Don't profile any resource files 
            MiniProfiler.Settings.IgnoredPaths = new[] { "/mini-profiler-", "/css/", "/scripts/", "/images/", "/favicon.ico" };

            //Clean up the nhibernate stack trace
            MiniProfiler.Settings.ExcludeAssembly("mscorlib");
            MiniProfiler.Settings.ExcludeAssembly("NHibernate");
            MiniProfiler.Settings.ExcludeAssembly("System.Web.Extensions");
            MiniProfiler.Settings.ExcludeType("DbCommandProxy");
            
            MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();
        }

        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)
            {
                MiniProfiler.Start();
            }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

        /// <summary>
        /// ELMAH filtering for the error log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ErrorLog_Filtering(object sender, ExceptionFilterEventArgs e)
        {

        }

        /// <summary>
        /// ELMAH filtering for the mail log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            if (e.Exception.GetBaseException() is HttpException)
            {
                e.Dismiss();
            }            
        }
    }
}