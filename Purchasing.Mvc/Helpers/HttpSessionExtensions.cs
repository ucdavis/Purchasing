using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Http
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static ISession SafeSession(this HttpContext httpContext)
        {
            // ensure the SessionFeature exists before attmepting to reference httpContext.Session
            var sessionFeature = httpContext.Features.Get<ISessionFeature>();
            return sessionFeature == null 
                ? null 
                : !httpContext.Session.IsAvailable 
                    ? null 
                    : httpContext.Session;
        }
    }
}