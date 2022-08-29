using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Serilog.Core;
using Serilog.Events;

namespace Purchasing.Mvc.Logging
{
    public class CustomHttpContextEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CustomHttpContextEnricher(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public CustomHttpContextEnricher()
        {
            _contextAccessor = new HttpContextAccessor();
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext == null)
                return;

            // var endpoint = httpContext.GetEndpoint();
            // var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            // User property not cached because it depends on whether event occurs before or after authentication
            AddOrUpdateProperty(httpContext, logEvent, propertyFactory,
                 "User", () => httpContext.User.Identity.Name ?? "anonymous");
        }

        private static void AddPropertyIfAbsent(HttpContext httpContext, LogEvent logEvent, ILogEventPropertyFactory factory,
            string propertyName, Func<object> getValue)
        {
            var itemKey = $"SERILOG_CUST_{propertyName}";
            if (httpContext.Items[itemKey] is LogEventProperty property)
            {
                logEvent.AddPropertyIfAbsent(property);
                return;
            }

            var newProperty = factory.CreateProperty(propertyName, getValue(), true);
            httpContext.Items[itemKey] = newProperty;
            logEvent.AddPropertyIfAbsent(newProperty);
        }

        private static void AddOrUpdateProperty(HttpContext httpContext, LogEvent logEvent, ILogEventPropertyFactory factory,
            string propertyName, Func<object> getValue)
        {
            var itemKey = $"SERILOG_CUST_{propertyName}";
            var newProperty = factory.CreateProperty(propertyName, getValue(), true);
            logEvent.AddOrUpdateProperty(newProperty);
        }
    }
}
