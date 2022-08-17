using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using UCDArch.Core;

namespace Purchasing.Mvc.Helpers
{
    public static class ViewHelper
    {
        public static ViewResult NotAuthorized(string errorMessage = null)
        {
            var result = new ViewResult
            {
                ViewName = "/Views/Error/NotAuthorized.cshtml",
            };

            if (!string.IsNullOrEmpty(errorMessage))
            {
                var httpContext = SmartServiceLocator<IHttpContextAccessor>.GetService().HttpContext;
                var tempDataFactory = SmartServiceLocator<ITempDataDictionaryFactory>.GetService();
                var tempData = tempDataFactory.GetTempData(httpContext);
                tempData["ErrorMessage"] = errorMessage;
            }

            return result;
        }
    }
}
