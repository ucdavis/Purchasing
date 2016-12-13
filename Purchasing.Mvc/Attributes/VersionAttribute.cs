using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Caching;

namespace Purchasing.Mvc.Attributes
{
    /// <summary>
    /// Sets the ViewData["Version"] to a version number corresponding to the last build date.
    /// Format: {MajorVersion}.{year}.{month}.{day}
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class VersionAttribute : ActionFilterAttribute
    {
        public int MajorVersion { get; set; }
        public string VersionKey { get; set; }

        public VersionAttribute()
        {
            MajorVersion = 2;
            VersionKey = "Version";
        }

        /// <summary>
        /// Grabs the date time stamp and places the version in Cache if it does not exist
        /// and places the version in ViewData
        /// </summary>
        /// <param name="filterContext"></param>
        private void LoadAssemblyVersion(ActionExecutingContext filterContext)
        {
            var version = filterContext.HttpContext.Cache[VersionKey] as string;

            if (string.IsNullOrEmpty(version))
            {
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString(); //Version from AppVeyor.

                //Insert version into the cache until tomorrow (Today + 1 day)
                filterContext.HttpContext.Cache.Insert(VersionKey, version, null, DateTime.Today.AddDays(1), Cache.NoSlidingExpiration);
            }

            filterContext.Controller.ViewData[VersionKey] = version;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadAssemblyVersion(filterContext);

            base.OnActionExecuting(filterContext);
        }
    }
}