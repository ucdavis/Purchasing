using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

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
            if(!ignoreParameters)
            {
                for(int i = 0; i < methodCall.Arguments.Count; i++)
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
            if(!ignoreParameters)
            {
                for(int i = 0; i < methodCall.Arguments.Count; i++)
                {
                    string name = methodCall.Method.GetParameters()[i].Name;

                    object value = ((ConstantExpression)methodCall.Arguments[i]).Value;

                    Assert.AreEqual(routeData.Values.GetValue(name), value.ToString());
                    //Assert.That(routeData.Values.GetValue(name), Is.EqualTo(value.ToString()));
                }
            }
            return routeData;
        }
    }
}
