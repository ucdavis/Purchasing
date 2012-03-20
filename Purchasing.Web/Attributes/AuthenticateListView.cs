using System;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Purchasing.Web.Attributes
{
    public class AuthenticateListView : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            if (String.IsNullOrEmpty(req.Headers["Authorization"]))
            {
                //Basically send a 401 w/ WWW-Authenticate for basic auth, 
                //but we have to get tricky and skip/clear errors because IIS tries to redirect 401 as 302 
                filterContext.HttpContext.SkipAuthorization = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                filterContext.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic realm=\"ucdavis\"");
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.Result = new HttpUnauthorizedResult("Unauthorized");
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                filterContext.HttpContext.Response.End();
                filterContext.HttpContext.ClearError();
            }
            else
            {
                var cred =
                    Encoding.ASCII.GetString(
                        Convert.FromBase64String(
                            filterContext.RequestContext.HttpContext.Request.Headers["Authorization"].Substring(6))).
                        Split(':');
                var user = new { Name = cred[0], Pass = cred[1] };

                if (user.Pass != WebConfigurationManager.AppSettings["userlistpassword"])
                {
                    filterContext.Result =
                        new HttpUnauthorizedResult("Password incorrect. Please check the user list password settings");
                }
            }
        }
    }
}