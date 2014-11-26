using System.Web;
using System.Web.Optimization;

namespace Purchasing.Mvc
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //TODO: combine CSS into a bundle
            //bundles.AddPerSubDirectory<StylesheetBundle>("Css", new FileSearch { Exclude = new Regex("/single/") });
            //bundles.AddPerIndividualFile<StylesheetBundle>("Css/single");

            //bundles.AddPerSubDirectory<ScriptBundle>("Scripts/external"); //just the CDN hosted stuff
            //bundles.AddPerSubDirectory<ScriptBundle>("Scripts/public", new FileSearch { Exclude = new Regex("/single/") });
            //bundles.AddPerIndividualFile<ScriptBundle>("Scripts/public/single"); //stuff in single is shared and can be referenced individually

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui")
                .Include("~/Scripts/jquery-1.7.1.min.js")
                .Include("~/Scripts/jquery-ui-1.8.17.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/head")
                .IncludeDirectory("~/Scripts/public/head", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/common")
                .IncludeDirectory("~/Scripts/public/common", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/landing")
                .Include("~/Scripts/public/single/Landing.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Css/main").Include(
                      "~/Css/Site.css",
                      "~/Css/jquery-ui-1.8.16.caes.css",
                      "~/Css/jquery.qtip.min.css",
                      "~/Css/jquery.tzCheckbox.css",
                      "~/Css/Datatables.css",
                      "~/Css/custom.css",
                      "~/Css/icons.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}
