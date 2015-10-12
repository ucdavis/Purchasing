using System.Web.Mvc;
using Serilog;

namespace Purchasing.Mvc.Attributes
{
    public class HandleAndLogErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Log.Error(filterContext.Exception, "Unhandled Exception Caught");

            base.OnException(filterContext);
        }
    }
}