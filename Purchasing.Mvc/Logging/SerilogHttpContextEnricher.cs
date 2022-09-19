using System;
using System.Collections.Concurrent;
using J2N.Collections.Generic.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog.Core;
using Serilog.Events;

namespace Purchasing.Mvc.Logging
{
    public class SerilogHttpContextEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private bool _enriching = false;

        public SerilogHttpContextEnricher(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public SerilogHttpContextEnricher()
        {
            _contextAccessor = new HttpContextAccessor();
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_enriching)
            {
                // Prevent infinite recursion that can happen when accessing certain properties, like ISession.Id
                return;
            }
            try
            {
                _enriching = true;
                var httpContext = _contextAccessor.HttpContext;
                if (httpContext == null)
                    return;

                // Set HttpContext properties
                AddOrUpdateProperty(httpContext, logEvent, propertyFactory, "User", () => httpContext.User?.Identity?.Name);
                AddOrUpdateProperty(httpContext, logEvent, propertyFactory, "SessionId", () => httpContext.SafeSession()?.Id);
                AddOrUpdateProperty(httpContext, logEvent, propertyFactory, "TraceId", () => httpContext.TraceIdentifier);
                AddOrUpdateProperty(httpContext, logEvent, propertyFactory, "EndpointName", () => httpContext.GetEndpoint()?.DisplayName);
                AddOrUpdateProperty(httpContext, logEvent, propertyFactory, "ResponseContentType", () => httpContext.Response?.ContentType?.ToString());
            }
            finally
            {
                _enriching = false;
            }
        }

        private static void AddOrUpdateProperty(HttpContext httpContext, LogEvent logEvent, ILogEventPropertyFactory factory,
            string propertyName, Func<object> getValue, string defaultValue = "unknown", bool destructure = true)
        {
            // Values retrieved from httpContext can go into and out of scope depending on where in the execution
            // pipeline logging occurs. We want to hold onto the most up-to-date value that is not defaultValue.
            var itemKey = $"SERILOG_CUSTOM_{propertyName}";
            var value = getValue();
            if (httpContext.Items[itemKey] is ValueTuple<object, LogEventProperty> item)
            {
                if (value == null || item.Item1 == value)
                {
                    logEvent.AddOrUpdateProperty(item.Item2);
                    return;
                }
            }

            var newValue = value ?? defaultValue;
            var newProperty = factory.CreateProperty(propertyName, newValue, destructure);
            httpContext.Items[itemKey] = (newValue, newProperty);
            logEvent.AddOrUpdateProperty(newProperty);
        }
    }
}
