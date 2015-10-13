using System.Web.Mvc;
using Serilog;

namespace Purchasing.Mvc.Attributes
{
    public class HandleAndLogErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Log.Error(filterContext.Exception, "Exception: {message} of type {type}", filterContext.Exception.Message,
                filterContext.Exception.GetType().Name);

            base.OnException(filterContext);
        }
    }
}