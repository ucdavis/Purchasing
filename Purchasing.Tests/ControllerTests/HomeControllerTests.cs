using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Purchasing.Core;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
//using Purchasing.Controllers.Filters;
using Purchasing.Core.Domain;
//using Purchasing.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Helpers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;


namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    public class HomeControllerTests : ControllerTestBase<HomeController>
    {
        private readonly Type _controllerClass = typeof(HomeController);
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IQueryRepositoryFactory QueryRepositoryFactory;
        public IRepository<PendingOrder> PendingOrderRepository;
        public IRepository<OpenOrderByUser> OpenOrderByUserRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();

            PendingOrderRepository = MockRepository.GenerateStub<IRepository<PendingOrder>>();
            OpenOrderByUserRepository = MockRepository.GenerateStub<IRepository<OpenOrderByUser>>();
            
            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
            QueryRepositoryFactory.PendingOrderRepository = PendingOrderRepository;
            QueryRepositoryFactory.OpenOrderByUserRepository = OpenOrderByUserRepository;

            Controller = new TestControllerBuilder().CreateController<HomeController>(UserRepository, QueryRepositoryFactory);
            
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

        //public HomeControllerTests()
        //{
        //    //    ExampleRepository = FakeRepository<Example>();
        //    //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();
        //}
        #endregion Init

        #region Mapping Tests
        [TestMethod]
        public void TestIndexMapping1()
        {
            "~/Home/Index/".ShouldMapTo<HomeController>(a => a.Index());
        }

        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/Home/".ShouldMapTo<HomeController>(a => a.Index());
        }

        [TestMethod]
        public void TestLandingMapping()
        {
            "~/Home/Landing/".ShouldMapTo<HomeController>(a => a.Landing());
        }
        #endregion Mapping Tests

        #region Method Tests

        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView()
        {
            Controller.Index().AssertViewRendered();
        }
        #endregion Index Tests

        #region Landing Tests

        [TestMethod]
        public void TestLandingRedirectsWhenUserNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            Controller.Landing()
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("You are currently not an active user for this program. If you believe this is incorrect contact your departmental administrator to add you.", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestLandingRedirectsWhenUserNotActive()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            var users = new List<User>();
            users.Add(CreateValidEntities.User(2));
            users[0].IsActive = false;
            users[0].SetIdTo("Me");
            new FakeUsers(0, UserRepository, users, true);
            #endregion Arrange

            #region Act
            Controller.Landing()
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("You are currently not an active user for this program. If you believe this is incorrect contact your departmental administrator to add you.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestLandReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            var users = new List<User>();
            users.Add(CreateValidEntities.User(2));
            users[0].IsActive = true;
            users[0].SetIdTo("Me");
            new FakeUsers(0, UserRepository, users, true);
            #endregion Arrange

            #region Act
            var result = Controller.Landing()
                .AssertViewRendered()
                .WithViewData<LandingViewModel>();
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        #endregion Landing Tests


        [TestMethod]
        public void TestWriteMethodTests()
        {
            #region Arrange
            Assert.Inconclusive("Need to write these tests");          
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert		
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
        public void TestControllerHas5Attributes()
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
            Assert.Inconclusive("Tests are still being written. When done, remove this line.");
            Assert.AreEqual(1, result.Count(), "It looks like a method was added or removed from the controller.");
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
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<HandleTransactionsManuallyAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HandleTransactionsManuallyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        //Examples

        //[TestMethod]
        //public void TestControllerMethodLogOnContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethod("LogOn");
        //    #endregion Arrange

        //    #region Act
        //    //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    //Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(0, allAttributes.Count());
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodLogOutContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethod("LogOut");
        //    #endregion Arrange

        //    #region Act
        //    //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    //Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(0, allAttributes.Count());
        //    #endregion Assert
        //}


        //[TestMethod]
        //public void TestControllerMethodCreateContainsExpectedAttributes1()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(1, allAttributes.Count());
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodCreateContainsExpectedAttributes2()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
        //    Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodCreateContainsExpectedAttributes3()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
        //    #endregion Assert
        //}


        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
