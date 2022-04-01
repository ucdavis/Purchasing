using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Models;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;


namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    public class HomeControllerTests : ControllerTestBase<HomeController>
    {
        //private readonly Type _controllerClass = typeof(HomeController);
        //public IRepositoryWithTypedId<User, string> UserRepository;
        //public IQueryRepositoryFactory QueryRepositoryFactory;
        //public IRepository<PendingOrder> PendingOrderRepository;
        //public IRepository<OpenOrderByUser> OpenOrderByUserRepository;

        //#region Init
        ///// <summary>
        ///// Setups the controller.
        ///// </summary>
        //protected override void SetupController()
        //{
        //    UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();

        //    PendingOrderRepository = MockRepository.GenerateStub<IRepository<PendingOrder>>();
        //    OpenOrderByUserRepository = MockRepository.GenerateStub<IRepository<OpenOrderByUser>>();
            
        //    QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
        //    QueryRepositoryFactory.PendingOrderRepository = PendingOrderRepository;
        //    QueryRepositoryFactory.OpenOrderByUserRepository = OpenOrderByUserRepository;

        //    Controller = new TestControllerBuilder().CreateController<HomeController>(UserRepository, QueryRepositoryFactory);
            
        //}

        //protected override void RegisterRoutes()
        //{
        //    RouteConfig.RegisterRoutes(RouteTable.Routes);
        //}

        //protected override void RegisterAdditionalServices(IWindsorContainer container)
        //{
        //    AutomapperConfig.Configure();
        //    container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));
        //    base.RegisterAdditionalServices(container);
        //}

        //#endregion Init

        //#region Mapping Tests
        //[TestMethod]
        //public void TestIndexMapping1()
        //{
        //    "~/Home/Index/".ShouldMapTo<HomeController>(a => a.Index());
        //}

        //[TestMethod]
        //public void TestIndexMapping2()
        //{
        //    "~/Home/".ShouldMapTo<HomeController>(a => a.Index());
        //}

        //[TestMethod]
        //public void TestLandingMapping()
        //{
        //    "~/Home/Landing/".ShouldMapTo<HomeController>(a => a.Landing());
        //}
        //#endregion Mapping Tests

        //#region Method Tests

        //#region Index Tests

        //[TestMethod]
        //public void TestIndexReturnsView()
        //{
        //    Controller.Index().AssertViewRendered();
        //}
        //#endregion Index Tests

        //#region Landing Tests

        //[TestMethod]
        //public void TestLandingRedirectsWhenUserNotFound()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
        //    new FakeUsers(3, UserRepository);
        //    #endregion Arrange

        //    #region Act
        //    Controller.Landing()
        //        .AssertActionRedirect()
        //        .ToAction<ErrorController>(a => a.NotAuthorized());
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual("You are currently not an active user for this program. If you believe this is incorrect contact your departmental administrator to add you.", Controller.Message);
        //    #endregion Assert		
        //}


        //[TestMethod]
        //public void TestLandingRedirectsWhenUserNotActive()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
        //    var users = new List<User>();
        //    users.Add(CreateValidEntities.User(2));
        //    users[0].IsActive = false;
        //    users[0].SetIdTo("Me");
        //    new FakeUsers(0, UserRepository, users, true);
        //    #endregion Arrange

        //    #region Act
        //    Controller.Landing()
        //        .AssertActionRedirect()
        //        .ToAction<ErrorController>(a => a.NotAuthorized());
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual("You are currently not an active user for this program. If you believe this is incorrect contact your departmental administrator to add you.", Controller.Message);
        //    #endregion Assert
        //}


        //[TestMethod]
        //public void TestLandReturnsView1()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
        //    var users = new List<User>();
        //    users.Add(CreateValidEntities.User(2));
        //    users[0].IsActive = true;
        //    users[0].SetIdTo("Me");
        //    new FakeUsers(0, UserRepository, users, true);

        //    new FakePendingOrders(3, PendingOrderRepository);
        //    new FakeOpenOrderByUser(3, OpenOrderByUserRepository);
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Landing()
        //        .AssertViewRendered()
        //        .WithViewData<LandingViewModel>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsNotNull(result);
        //    #endregion Assert		
        //}

        //[TestMethod]
        //public void TestLandReturnsView2()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
        //    var users = new List<User>();
        //    users.Add(CreateValidEntities.User(2));
        //    users[0].IsActive = true;
        //    users[0].SetIdTo("Me");
        //    new FakeUsers(0, UserRepository, users, true);

        //    var pendingOrders = new List<PendingOrder>();
        //    var openOrders = new List<OpenOrderByUser>();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        pendingOrders.Add(CreateValidEntities.PendingOrder(i+1));
        //        openOrders.Add(CreateValidEntities.OpenOrderByUser(i+11));
        //        pendingOrders[i].AccessUserId = "Me";
        //        openOrders[i].AccessUserId = "Me";
        //        pendingOrders[i].LastActionDate = DateTime.UtcNow.ToPacificTime().Date.AddDays(i + 1);
        //        openOrders[i].LastActionDate = DateTime.UtcNow.ToPacificTime().Date.AddDays(i + 1);
        //    }

        //    pendingOrders[3].AccessUserId = "NotMe";
        //    openOrders[3].AccessUserId = "NotMe";

        //    new FakePendingOrders(0, PendingOrderRepository, pendingOrders);
        //    new FakeOpenOrderByUser(0, OpenOrderByUserRepository, openOrders);
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Landing()
        //        .AssertViewRendered()
        //        .WithViewData<LandingViewModel>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(4, result.PendingOrders.Count());
        //    Assert.AreEqual(4, result.YourOpenOrders.Count());
        //    Assert.AreEqual("Creator5", result.PendingOrders.ToList()[0].Creator);
        //    Assert.AreEqual("Creator3", result.PendingOrders.ToList()[1].Creator);
        //    Assert.AreEqual("Creator2", result.PendingOrders.ToList()[2].Creator);
        //    Assert.AreEqual("Creator1", result.PendingOrders.ToList()[3].Creator);
        //    Assert.AreEqual("Creator15", result.YourOpenOrders.ToList()[0].Creator);
        //    Assert.AreEqual("Creator13", result.YourOpenOrders.ToList()[1].Creator);
        //    Assert.AreEqual("Creator12", result.YourOpenOrders.ToList()[2].Creator);
        //    Assert.AreEqual("Creator11", result.YourOpenOrders.ToList()[3].Creator);
        //    #endregion Assert
        //}

        //#endregion Landing Tests

        //#endregion Method Tests

        //#region Reflection Tests

        //#region Controller Class Tests
        //[TestMethod]
        //public void TestControllerInheritsFromApplicationController()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    #endregion Arrange

        //    #region Act
        //    Assert.IsNotNull(controllerClass.BaseType);
        //    var result = controllerClass.BaseType.Name;
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual("ApplicationController", result);
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the controller has 5 attributes.
        ///// </summary>
        //[TestMethod]
        //public void TestControllerHas5Attributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    #endregion Arrange

        //    #region Act
        //    var result = controllerClass.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(5, result.Count());
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the controller has transaction attribute.
        ///// </summary>
        //[TestMethod]
        //public void TestControllerHasTransactionAttribute()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    #endregion Arrange

        //    #region Act
        //    var result = controllerClass.GetCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(result.Any(), "UseTransactionsByDefaultAttribute not found.");
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the controller has anti forgery token attribute.
        ///// </summary>
        //[TestMethod]
        //public void TestControllerHasAntiForgeryTokenAttribute()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    #endregion Arrange

        //    #region Act
        //    var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(result.Any(), "UseAntiForgeryTokenOnPostByDefault not found.");
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerHasVersionAttribute()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    #endregion Arrange

        //    #region Act
        //    var result = controllerClass.GetCustomAttributes(true).OfType<Mvc.Attributes.VersionAttribute>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(result.Any(), "VersionAttribute not found.");
        //    #endregion Assert
        //}


        //[TestMethod]
        //public void TestControllerHasAuthorizeAttribute()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    #endregion Arrange

        //    #region Act
        //    var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(result.Any(), "AuthorizeAttribute not found.");
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerHasProfileAttribute()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    #endregion Arrange

        //    #region Act
        //    var result = controllerClass.GetCustomAttributes(true).OfType<ProfileAttribute>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(result.Any(), "ProfileAttribute not found.");
        //    #endregion Assert
        //}

        //#endregion Controller Class Tests

        //#region Controller Method Tests

        //[TestMethod]
        //public void TestControllerContainsExpectedNumberOfPublicMethods()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    #endregion Arrange

        //    #region Act
        //    var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(2, result.Count(), "It looks like a method was added or removed from the controller.");
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodIndexContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethod("Index");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<HandleTransactionsManuallyAttribute>();
        //    var allAttributes = controllerMethod.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "HandleTransactionsManuallyAttribute not found");
        //    Assert.AreEqual(1, allAttributes.Count());
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodLandingContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethod("Landing");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
        //    var allAttributes = controllerMethod.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
        //    Assert.AreEqual(1, allAttributes.Count());
        //    #endregion Assert
        //}


        //#endregion Controller Method Tests

        //#endregion Reflection Tests

        protected override void SetupController()
        {
            //throw new NotImplementedException();
        }
    }
}
