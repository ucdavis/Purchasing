using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using StackExchange.Profiling;

namespace Purchasing.Mvc.Helpers
{
    /// <summary>
    /// Instrumenting controller actions. See: http://samsaffron.com/archive/2011/07/25/Automatically+instrumenting+an+MVC3+app
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ProfileAttribute : ActionFilterAttribute
    {
        const string StackKey = "ProfilingActionFilterStack";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var mp = MiniProfiler.Current;
            if (mp != null)
            {
                var stack = HttpContext.Current.Items[StackKey] as Stack<IDisposable>;
                if (stack == null)
                {
                    stack = new Stack<IDisposable>();
                    HttpContext.Current.Items[StackKey] = stack;
                }

                var prof = MiniProfiler.Current.Step("Controller: " + filterContext.Controller + "." + filterContext.ActionDescriptor.ActionName);
                stack.Push(prof);

            }
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var stack = HttpContext.Current.Items[StackKey] as Stack<IDisposable>;
            if (stack != null && stack.Count > 0)
            {
                stack.Pop().Dispose();
            }
        }
    }
}