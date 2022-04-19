using System;
using System.Collections.Generic;
using System.Web;
using StackExchange.Profiling;
using Purchasing.Mvc;
using UCDArch.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Purchasing.Mvc.Helpers
{
    /// <summary>
    /// Instrumenting controller actions. See: http://samsaffron.com/archive/2011/07/25/Automatically+instrumenting+an+MVC3+app
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ProfileAttribute : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute
    {
        const string StackKey = "ProfilingActionFilterStack";

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext filterContext)
        {
            var mp = MiniProfiler.Current;
            if (mp != null)
            {
                var stack = SmartServiceLocator<IHttpContextAccessor>.GetService().HttpContext.Items[StackKey] as Stack<IDisposable>;
                if (stack == null)
                {
                    stack = new Stack<IDisposable>();
                    SmartServiceLocator<IHttpContextAccessor>.GetService().HttpContext.Items[StackKey] = stack;
                }

                var prof = MiniProfiler.Current.Step("Controller: " + filterContext.Controller + "." + (filterContext.ActionDescriptor as ControllerActionDescriptor).ActionName);
                stack.Push(prof);

            }
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var stack = SmartServiceLocator<IHttpContextAccessor>.GetService().HttpContext.Items[StackKey] as Stack<IDisposable>;
            if (stack != null && stack.Count > 0)
            {
                stack.Pop().Dispose();
            }
        }
    }
}