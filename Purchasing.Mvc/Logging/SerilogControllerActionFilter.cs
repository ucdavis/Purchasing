using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            // ModelState saved for subsequent use by SerilogHttpContextEnricher
            context.HttpContext.Items[SerilogHttpContextEnricher.SERILOG_CUSTOM_MODEL_STATE] = context.ModelState;
            
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }

}
