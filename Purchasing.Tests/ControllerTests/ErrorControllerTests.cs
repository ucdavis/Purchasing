using System;
using System.Linq;
using System.Web.Mvc;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Web.Helpers;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    public class ErrorControllerTests : ControllerTestBase<ErrorController>
    {
        private readonly Type _controllerClass = typeof(ErrorController);

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            Controller = new TestControllerBuilder().CreateController<ErrorController>();
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes(); 
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();
            base.RegisterAdditionalServices(container);
        }

        #endregion Init

        #region Mapping Tests
        [TestMethod]
        public void TestIndexMapping1()
        {
            "~/Error/Index/".ShouldMapTo<ErrorController>(a => a.Index());
        }
        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/Error/".ShouldMapTo<ErrorController>(a => a.Index());
        }

        [TestMethod]
        public void TestNotAuthorizedMapping1()
        {
            "~/Error/NotAuthorized/".ShouldMapTo<ErrorController>(a => a.NotAuthorized());
        }

        [TestMethod]
        public void TestFileNotFoundMapping1()
        {
            "~/Error/FileNotFound/".ShouldMapTo<ErrorController>(a => a.FileNotFound());
        }
        #endregion Mapping Tests

        #region Method Tests


        [TestMethod]
        public void TestIndexRetuensView()
        {
            Controller.Index().AssertViewRendered();
        }

        [TestMethod]
        public void TestNotAuthorizedRetuensView()
        {
            Controller.NotAuthorized().AssertViewRendered();
        }

        [TestMethod]
        public void TestFileNotFoundRetuensView()
        {
            Controller.FileNotFound().AssertViewRendered();
        }

        #endregion Method Tests

        #region Reflection Tests

        #region Controller Class Tests
        [TestMethod]
        public void TestControllerInheritsFromApplicationController()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            Assert.IsNotNull(controllerClass.BaseType);
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("ApplicationController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has 5 attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasFiveAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "UseTransactionsByDefaultAttribute not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has anti forgery token attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAntiForgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "UseAntiForgeryTokenOnPostByDefault not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasVersionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<Web.Attributes.VersionAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "VersionAttribute not found.");
            #endregion Assert
        }


        [TestMethod]
        public void TestControllerHasAuthorizeAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "AuthorizeAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasProfileAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<ProfileAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "ProfileAttribute not found.");
            #endregion Assert
        }

        #endregion Controller Class Tests

        #region Controller Method Tests

        [TestMethod]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodNotAuthorizedContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("NotAuthorized");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodFileNotFoundContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("FileNotFound");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }


        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
