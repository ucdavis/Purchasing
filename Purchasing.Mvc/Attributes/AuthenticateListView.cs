using System;
using System.Threading.Tasks;
using System.Text;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Purchasing.Mvc.Attributes
{
    public class AuthenticateListView : ActionFilterAttribute
    {
        public const string BLOCK_401_TO_302_REDIRECT = "Block401To302Redirect";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var configuration = filterContext.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

            var req = filterContext.HttpContext.Request;
            if (String.IsNullOrEmpty(req.Headers["Authorization"]))
            {
                //Basically send a 401 w/ WWW-Authenticate for basic auth, 
                //but we have to get tricky and skip/clear errors because IIS tries to redirect 401 as 302 
                // filterContext.HttpContext.SkipAuthorization = true;
                // filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                filterContext.HttpContext.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"ucdavis\"");
                // filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.Result = new UnauthorizedResult();
                // filterContext.Result.ExecuteResult(Controller.ControllerContext);
                // filterContext.HttpContext.Response.End();
                // filterContext.HttpContext.ClearError();

                // This needs to be handled by a StatusCodePages middleware
                filterContext.HttpContext.Items[BLOCK_401_TO_302_REDIRECT] = true;
            }
            else
            {
                var cred =
                    Encoding.ASCII.GetString(
                        Convert.FromBase64String(
                            filterContext.HttpContext.Request.Headers["Authorization"].ToString().Substring(6))).
                        Split(':');
                var user = new { Name = cred[0], Pass = cred[1] };

                if (user.Pass != configuration["userlistpassword"])
                {
                    filterContext.Result = new UnauthorizedObjectResult("Password incorrect. Please check the user list password settings");
                }
            }
        }
    }
}