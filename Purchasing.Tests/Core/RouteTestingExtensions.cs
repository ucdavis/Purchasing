using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.Core
{
    public static class RouteTestingExtensions
    {
        /// <summary>
        /// Validates that the route should map to a particular controller and action.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="strRoute">The STR route.</param>
        /// <param name="action">The action.</param>
        /// <param name="ignoreParameters">if set to <c>true</c> [ignore parameters].</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(
            this string strRoute, 
            Expression<Func<TController, ActionResult>> action, 
            bool ignoreParameters = false)
            where TController : Controller
        {
            return ShouldMapTo(strRoute, typeof(TController), (MethodCallExpression)action.Body, ignoreParameters);
        }

        /// <summary>
        /// Validates that the route should map to a particular controller and action.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="strRoute">The STR route.</param>
        /// <param name="action">The action.</param>
        /// <param name="ignoreParameters">if set to <c>true</c> [ignore parameters].</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(
            this string strRoute, 
            Expression<Func<TController, Task<ActionResult>>> action, 
            bool ignoreParameters = false)
            where TController : Controller
        {
            return ShouldMapTo(strRoute, typeof(TController), (MethodCallExpression)action.Body, ignoreParameters);
        }

        /// <summary>
        /// Validates that the route should map to a particular controller and action.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="strRoute">The STR route.</param>
        /// <param name="action">The action.</param>
        /// <param name="ignoreParameters">if set to <c>true</c> [ignore parameters].</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(
            this string strRoute, Expression<Func<TController, Task>> action, 
            bool ignoreParameters) 
            where TController : Controller
        {
            return ShouldMapTo(strRoute, typeof(TController), (MethodCallExpression)action.Body, ignoreParameters);
        }

        private static RouteData ShouldMapTo(
            string strRoute, 
            Type controllerType, 
            MethodCallExpression methodCall = null, 
            bool ignoreParameters = false)
        {
            RouteData routeData = strRoute.Route();
            Assert.IsNotNull(routeData, "The URL did not match any route");

            //check controller
            Assert.IsTrue(controllerType.IsAssignableFrom(typeof(Controller)), "The controller type must be a subclass of Controller");

            //strip out the word 'Controller' from the type
            string expected = controllerType.Name.Replace("Controller", "");

            //get the key (case insensitive)
            string actual = routeData.Values.GetValue("controller").ToString();

            expected.AssertSameStringAs(actual);

            if (methodCall == null)
            {
                return routeData;
            }

            //check action
            string actualAction = routeData.Values.GetValue("action").ToString();
            string expectedAction = methodCall.Method.Name;
            actualAction.AssertSameStringAs(expectedAction);

            if (ignoreParameters)
            {
                return routeData;
            }

            //check parameters
            for (int i = 0; i < methodCall.Arguments.Count; i++)
            {
                string name = methodCall.Method.GetParameters()[i].Name;
                object value = null;

                switch (methodCall.Arguments[i].NodeType)
                {
                    case ExpressionType.Constant:
                        value = ((ConstantExpression)methodCall.Arguments[i]).Value;
                        break;

                    case ExpressionType.MemberAccess:
                        value = Expression.Lambda(methodCall.Arguments[i]).Compile().DynamicInvoke();
                        break;

                }

                value = (value == null ? value : value.ToString());
                routeData.Values.GetValue(name).ShouldEqual(value, "Value for parameter did not match");
            }

            return routeData;
        }

        /// <summary>
        /// Returns the corresponding route for the URL.  Returns null if no route was found.
        /// </summary>
        /// <param name="url">The app relative url to test.</param>
        /// <returns>A matching <see cref="RouteData" />, or null.</returns>
        public static RouteData Route(this string url, string pattern = "{controller=Home}/{action=Index}/{id?}")
        {
            var values = Match(pattern, url);
            return new RouteData(values);
        }

        /// <summary>
        /// Gets a value from the <see cref="RouteValueDictionary" /> by key.  Does a
        /// case-insensitive search on the keys.
        /// </summary>
        /// <param name="routeValues"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValue(this RouteValueDictionary routeValues, string key)
        {
            foreach (var routeValueKey in routeValues.Keys)
            {
                if (string.Equals(routeValueKey, key, StringComparison.InvariantCultureIgnoreCase))
                    return routeValues[routeValueKey] as string;
            }

            return null;
        }

        public static RouteValueDictionary Match(string routeTemplate, string requestPath)
        {
            var regex = new Regex(@"(.*)(\?[^{}]*$)");
            var match = regex.Match(routeTemplate);
            if (match.Success)
            {
                routeTemplate = match.Groups[1].Value;
            }
            else
            {
                return null;
            }

            var template = TemplateParser.Parse(routeTemplate);

            var matcher = new TemplateMatcher(template, GetDefaults(template));

            var values = new RouteValueDictionary();

            return matcher.TryMatch(requestPath, values) ? values : null;
        }

        // This method extracts the default argument values from the template.
        private static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }

            return result;
        }
    }
}
