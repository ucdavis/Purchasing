using System;
using System.Web;
using System.Web.Routing;

namespace OrAdmin.Core.RouteConstraints
{
    public class GuidConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.ContainsKey(parameterName))
            {
                if (values[parameterName] is Guid)
                    return true;

                string stringValue = values[parameterName].ToString();

                if (!string.IsNullOrEmpty(stringValue))
                {
                    try
                    {
                        Guid guid = new Guid(stringValue);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
