using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public static RouteData ShouldMapTo<TController>(this string strRoute, Expression<Func<TController, ActionResult>> action, bool ignoreParameters) where TController : Controller
        {
            RouteData routeData = strRoute.Route();
            Assert.IsNotNull(routeData, "The URL did not match any route");

            //check controller
            routeData.ShouldMapTo<TController>();

            //check action
            var methodCall = (MethodCallExpression)action.Body;
            string actualAction = routeData.Values.GetValue("action").ToString();
            string expectedAction = methodCall.Method.Name;
            actualAction.AssertSameStringAs(expectedAction);

            //check parameters
            if (!ignoreParameters)
            {
                for (int i = 0; i < methodCall.Arguments.Count; i++)
                {
                    string name = methodCall.Method.GetParameters()[i].Name;

                    object value = ((ConstantExpression)methodCall.Arguments[i]).Value;

                    Assert.AreEqual(routeData.Values.GetValue(name), value.ToString());
                    //Assert.That(routeData.Values.GetValue(name), Is.EqualTo(value.ToString()));
                }
            }
            return routeData;
        }

        /// <summary>
        /// Validates that the route should map to a particular controller and action.
        /// This one uses actions without a return value.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="strRoute">The STR route.</param>
        /// <param name="action">The action.</param>
        /// <param name="ignoreParameters">if set to <c>true</c> [ignore parameters].</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this string strRoute, Expression<Action<TController>> action, bool ignoreParameters) where TController : Controller
        {
            var routeData = strRoute.Route();
            Assert.IsNotNull(routeData, "The URL did not match any route");

            //check controller
            routeData.ShouldMapTo<TController>();

            //check action
            var methodCall = (MethodCallExpression)action.Body;
            string actualAction = routeData.Values.GetValue("action").ToString();
            string expectedAction = methodCall.Method.Name;
            actualAction.AssertSameStringAs(expectedAction);

            //check parameters
            if (!ignoreParameters)
            {
                for (int i = 0; i < methodCall.Arguments.Count; i++)
                {
                    string name = methodCall.Method.GetParameters()[i].Name;

                    object value = ((ConstantExpression)methodCall.Arguments[i]).Value;

                    Assert.AreEqual(routeData.Values.GetValue(name), value.ToString());
                    //Assert.That(routeData.Values.GetValue(name), Is.EqualTo(value.ToString()));
                }
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
        /// Asserts that the route matches the expression specified.  Checks controller, action, and any method arguments
        /// into the action as route values.
        /// </summary>
        /// <typeparam name="TController">The controller.</typeparam>
        /// <param name="routeData">The routeData to check</param>
        /// <param name="action">The action to call on TController.</param>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Func<TController, ActionResult>> action)
            where TController : Controller
        {
            routeData.ShouldNotBeNull("The URL did not match any route");

            //check controller
            routeData.ShouldMapTo<TController>();

            //check action
            var methodCall = (MethodCallExpression)action.Body;
            string actualAction = routeData.Values.GetValue("action").ToString();
            string expectedAction = methodCall.Method.Name;
            actualAction.AssertSameStringAs(expectedAction);

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
        /// Converts the URL to matching RouteData and verifies that it will match a route with the values specified by the expression.
        /// </summary>
        /// <typeparam name="TController">The type of controller</typeparam>
        /// <param name="relativeUrl">The ~/ based url</param>
        /// <param name="action">The expression that defines what action gets called (and with which parameters)</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this string relativeUrl, Expression<Func<TController, ActionResult>> action) where TController : Controller
        {
            return relativeUrl.Route().ShouldMapTo(action);
        }

        /// <summary>
        /// Verifies the <see cref="RouteData">routeData</see> maps to the controller type specified.
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="routeData"></param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData) where TController : Controller
        {
            //strip out the word 'Controller' from the type
            string expected = typeof(TController).Name.Replace("Controller", "");

            //get the key (case insensitive)
            string actual = routeData.Values.GetValue("controller").ToString();


            expected.AssertSameStringAs(actual);
            return routeData;
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
