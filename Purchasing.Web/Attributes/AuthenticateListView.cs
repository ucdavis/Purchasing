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
                var res = filterContext.HttpContext.Response;
                res.StatusCode = 401;
                res.AddHeader("WWW-Authenticate", "Basic realm=\"ucdavis\"");
                res.End();
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