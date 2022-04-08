using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Purchasing.Mvc.Logging
{
    public class SerilogControllerActionFilter : IActionFilter
    {
        private readonly IDiagnosticContext _diagnosticContext;
        public SerilogControllerActionFilter(IDiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _diagnosticContext.Set("RouteData", context.ActionDescriptor.RouteValues);
            _diagnosticContext.Set("ActionName", context.ActionDescriptor.DisplayName);
            _diagnosticContext.Set("ActionId", context.ActionDescriptor.Id);
            _diagnosticContext.Set("ValidationState", context.ModelState.IsValid);
            
            var httpContext = context.HttpContext;
            var request = httpContext.Request;

            // Set all the common properties available for every request
            _diagnosticContext.Set("Host", request.Host);
            _diagnosticContext.Set("Protocol", request.Protocol);
            _diagnosticContext.Set("Scheme", request.Scheme);

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                _diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set User
            var user = httpContext.User.Identity.Name ?? "anonymous";
            _diagnosticContext.Set("User", user);

            // TODO: enable after upgrade to aspnetcore 3.x or higher
            //// Retrieve the IEndpointFeature selected for the request
            //var endpoint = httpContext.GetEndpoint();
            //if (endpoint is object) // endpoint != null
            //{
            //  _diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            //}
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Set the content-type of the Response at this point
            _diagnosticContext.Set("ResponseContentType", context.HttpContext.Response.ContentType);
        }
    }

}
