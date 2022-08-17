using Serilog;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Purchasing.Mvc.Attributes
{
    public class HandleAndLogErrorAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Log.Error(filterContext.Exception, "Exception: {message} of type {type}", filterContext.Exception.Message,
                filterContext.Exception.GetType().Name);

            base.OnException(filterContext);
        }
    }
}