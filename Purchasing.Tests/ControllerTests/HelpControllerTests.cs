using System;
using System.Linq;
using System.Web.Mvc;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Newtonsoft.Json.Linq;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Web.Helpers;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;


namespace Purchasing.Tests.ControllerTests
{
    public class JsonFormat
    {
        public bool HasIssues;
        public int IssueCount;
        public Int64 TimeStamp;
    }
    [TestClass]
    public class HelpControllerTests : ControllerTestBase<HelpController>
    {
        private readonly Type _controllerClass = typeof(HelpController);
        public IUservoiceService UservoiceService;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            UservoiceService = MockRepository.GenerateStub<IUservoiceService>();
            Controller = new TestControllerBuilder().CreateController<HelpController>(UservoiceService);            
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
        public void TesIndexMapping1()
        {
            "~/Help/Index/".ShouldMapTo<HelpController>(a => a.Index());
        }

        [TestMethod]
        public void TesIndexMapping2()
        {
            "~/Help/".ShouldMapTo<HelpController>(a => a.Index());
        }

        [TestMethod]
        public void TestGetActiveIssuesCountMapping()
        {
            "~/Help/GetActiveIssuesCount/".ShouldMapTo<HelpController>(a => a.GetActiveIssuesCount());
        }

        [TestMethod]
        public void TestOpenIssuesMapping()
        {
            "~/Help/OpenIssues/".ShouldMapTo<HelpController>(a => a.OpenIssues());
        }

        [TestMethod]
        public void TestIssuesForUserMapping()
        {
            "~/Help/IssuesForUser/".ShouldMapTo<HelpController>(a => a.IssuesForUser(null));
        }
        #endregion Mapping Tests

        #region GetActiveIssuesCount Tests

        [TestMethod]
        public void TestGetActiveIssuesCountWhenNoIssues()
        {
            #region Arrange
            UservoiceService.Expect(a => a.GetActiveIssuesCount()).Return(0);
            #endregion Arrange

            #region Act
            var result = Controller.GetActiveIssuesCount()
                .AssertResultIs<JsonResult>();
            #endregion Act

            #region Assert
            dynamic data = result.Data;
            Assert.IsFalse(data.HasIssues);
            Assert.AreEqual(0, data.IssuesCount);
            Assert.AreEqual(DateTime.Now.Date, new DateTime(data.TimeStamp).Date);
            #endregion Assert		
        }

        [TestMethod]
        public void TestGetActiveIssuesCountWhenIssues()
        {
            #region Arrange
            UservoiceService.Expect(a => a.GetActiveIssuesCount()).Return(2);
            #endregion Arrange

            #region Act
            var result = Controller.GetActiveIssuesCount()
                .AssertResultIs<JsonResult>();
            #endregion Act

            #region Assert
            dynamic data = result.Data;
            Assert.IsTrue(data.HasIssues);
            Assert.AreEqual(2, data.IssuesCount);
            Assert.AreEqual(DateTime.Now.Date, new DateTime(data.TimeStamp).Date);
            #endregion Assert
        }
        #endregion GetActiveIssuesCount Tests

        #region OpenIssues Tests

        [TestMethod]
        public void TestOpenIssues()
        {
            #region Arrange
            var json = @"{
                CPU: 'Intel',
                Drives: [
                'DVD read/writer',
                ""500 gigabyte hard drive""
                ]
            }";

            var rtValue = JObject.Parse(json);


            UservoiceService.Expect(a => a.GetOpenIssues()).Return(rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.OpenIssues()
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("{\"CPU\":\"Intel\",\"Drives\":[\"DVD read/writer\",\"500 gigabyte hard drive\"]}", result.JsonResultString);
            UservoiceService.AssertWasCalled(a => a.GetOpenIssues());
            #endregion Assert		
        }
        #endregion OpenIssues Tests

        #region IssuesForUser Tests

        [TestMethod]
        public void TestIssuesForUser1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            var json = @"{
                CPU: 'Intel',
                Drives: [
                'DVD read/writer',
                ""500 gigabyte hard drive""
                ]
            }";

            var rtValue = JObject.Parse(json);
            UservoiceService.Expect(a => a.GetActiveIssuesForUser(Arg<string>.Is.Anything)).Return(rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.IssuesForUser(null)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("{\"CPU\":\"Intel\",\"Drives\":[\"DVD read/writer\",\"500 gigabyte hard drive\"]}", result.JsonResultString);
            UservoiceService.AssertWasCalled(a => a.GetActiveIssuesForUser("Me"));
            #endregion Assert
        }

        [TestMethod]
        public void TestIssuesForUser2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            UservoiceService.Expect(a => a.GetActiveIssuesForUser(Arg<string>.Is.Anything)).Return(new JObject());
            #endregion Arrange

            #region Act
            var result = Controller.IssuesForUser("test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);            
            UservoiceService.AssertWasCalled(a => a.GetActiveIssuesForUser("test"));
            #endregion Assert
        }
        #endregion IssuesForUser Tests

        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView()
        {
            Controller.Index()
                .AssertViewRendered();
        }

        #endregion Index Tests

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
        /// Tests the controller has 6 attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasSixAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(6, result.Count());
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

        [TestMethod]
        public void TestControllerHasHandleTransactionsManuallyAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<HandleTransactionsManuallyAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "HandleTransactionsManuallyAttribute not found.");
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
            Assert.AreEqual(4, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodGetActiveIssuesCountContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("GetActiveIssuesCount");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<OutputCacheAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "OutputCacheAttribute not found");
            Assert.AreEqual(60, expectedAttribute.ElementAt(0).Duration);
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodOpenIssuesCountContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("OpenIssues");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<OutputCacheAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "OutputCacheAttribute not found");
            Assert.AreEqual(60, expectedAttribute.ElementAt(0).Duration);
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodIssuesForUserContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("IssuesForUser");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<OutputCacheAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "OutputCacheAttribute not found");
            Assert.AreEqual(60, expectedAttribute.ElementAt(0).Duration);
            Assert.AreEqual(1, allAttributes.Count());
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
        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
