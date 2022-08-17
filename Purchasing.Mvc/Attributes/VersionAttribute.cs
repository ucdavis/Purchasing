using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

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
        private void LoadAssemblyVersion(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext filterContext)
        {
            var cache = filterContext.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

            var version = cache.GetOrCreate(VersionKey, entry =>
            {
                entry.AbsoluteExpiration = DateTime.Today.AddDays(1);
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();;
            });

            var controller = filterContext.Controller as Controller;
            controller.ViewData[VersionKey] = version;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext filterContext)
        {
            LoadAssemblyVersion(filterContext);
        }
    }
}