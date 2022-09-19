using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Purchasing.Tests.Extensions;


namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    public class HistoryAjaxControllerTests : ControllerTestBase<HistoryAjaxController>
    {
        private readonly Type _controllerClass = typeof(HistoryAjaxController);
        //public IQueryRepositoryFactory QueryRepositoryFactory;
        public ISearchService SearchService;
        public IAccessQueryService AccessQueryService;
        public IOrderService OrderService;
        public IDbService DbService;


        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            //QueryRepositoryFactory = Mock.Of<IQueryRepositoryFactory>();
            OrderService = Mock.Of<IOrderService>();
            SearchService = Mock.Of<ISearchService>();
            AccessQueryService = Mock.Of<IAccessQueryService>();
            //QueryRepositoryFactory.OrderTrackingHistoryRepository = Mock.Of<IRepository<OrderTrackingHistory>>();
            //QueryRepositoryFactory.CommentHistoryRepository = Mock.Of<IRepositoryWithTypedId<CommentHistory, Guid>>();
            //QueryRepositoryFactory.OrderHistoryRepository = Mock.Of<IRepository<OrderHistory>>();
            DbService = Mock.Of<IDbService>();

            Controller = new HistoryAjaxController(SearchService, AccessQueryService, OrderService, DbService);

        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());

            //Fixes problem where .Fetch is used in a query
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));

            base.RegisterAdditionalServices(container);
        }

        #endregion Init

        #region Mapping Tests
        [TestMethod]
        public void TestRecentActivityMapping()
        {
            "~/HistoryAjax/RecentActivity/".ShouldMapTo<HistoryAjaxController>(a => a.RecentActivity());
        }

        [TestMethod]
        public void TestRecentCommentsMapping()
        {
            "~/HistoryAjax/RecentComments/".ShouldMapTo<HistoryAjaxController>(a => a.RecentComments());
        }

        [TestMethod]
        public void TestRecentlyCompletedMapping()
        {
            "~/HistoryAjax/RecentlyCompleted/".ShouldMapTo<HistoryAjaxController>(a => a.RecentlyCompleted());
        }
        #endregion Mapping Tests

        #region Method Tests

        #region RecentActivity Tests


        [TestMethod, Ignore] //Not sure how to fake it out
        public void TestRecentActivityReturnsPartialView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderTrackingHistory = new List<OrderTrackingHistory>();
            for (int i = 0; i < 5; i++)
            {
                orderTrackingHistory.Add(CreateValidEntities.OrderTrackingHistory(i + 1));
                orderTrackingHistory[i].AccessUserId = "Me";
                orderTrackingHistory[i].DateCreated = DateTime.UtcNow.ToPacificTime().Date;
            }
            orderTrackingHistory[3].DateCreated = DateTime.UtcNow.ToPacificTime().AddDays(3);
            orderTrackingHistory[4].DateCreated = DateTime.UtcNow.ToPacificTime().AddDays(3);
            orderTrackingHistory[4].AccessUserId = "NotMe";

            //new FakeOrderTrackingHistory(0, QueryRepositoryFactory.OrderTrackingHistoryRepository, orderTrackingHistory);
            #endregion Arrange

            #region Act
            var results = Controller.RecentActivity()
                .AssertPartialViewRendered()
                .WithViewData<OrderTrackingHistory>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("CreatedBy4", results.CreatedBy);
            #endregion Assert
        }

        [TestMethod, Ignore] //Not sure how to fake it out
        public void TestRecentActivityReturnsPartialView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderTrackingHistory = new List<OrderTrackingHistory>();
            for (int i = 0; i < 5; i++)
            {
                orderTrackingHistory.Add(CreateValidEntities.OrderTrackingHistory(i + 1));
                orderTrackingHistory[i].AccessUserId = "Me";
                orderTrackingHistory[i].DateCreated = DateTime.UtcNow.ToPacificTime().Date;
            }
            orderTrackingHistory[3].DateCreated = DateTime.UtcNow.ToPacificTime().AddDays(3);
            orderTrackingHistory[4].DateCreated = DateTime.UtcNow.ToPacificTime().AddDays(3);
            //orderTrackingHistory[4].AccessUserId = "NotMe";

            //new FakeOrderTrackingHistory(0, QueryRepositoryFactory.OrderTrackingHistoryRepository, orderTrackingHistory);
            #endregion Arrange

            #region Act
            var results = Controller.RecentActivity()
                .AssertPartialViewRendered()
                .WithViewData<OrderTrackingHistory>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("CreatedBy4", results.CreatedBy);
            #endregion Assert
        }

        [TestMethod, Ignore] //Not sure how to fake it out
        public void TestRecentActivityReturnsPartialView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderTrackingHistory = new List<OrderTrackingHistory>();
            for (int i = 0; i < 5; i++)
            {
                orderTrackingHistory.Add(CreateValidEntities.OrderTrackingHistory(i + 1));
                orderTrackingHistory[i].AccessUserId = "Me";
                orderTrackingHistory[i].DateCreated = DateTime.UtcNow.ToPacificTime().Date;
            }
            orderTrackingHistory[3].DateCreated = DateTime.UtcNow.ToPacificTime().AddDays(3);
            orderTrackingHistory[4].DateCreated = DateTime.UtcNow.ToPacificTime().AddDays(4); //changed from other tests
            //orderTrackingHistory[4].AccessUserId = "NotMe";

            //new FakeOrderTrackingHistory(0, QueryRepositoryFactory.OrderTrackingHistoryRepository, orderTrackingHistory);
            #endregion Arrange

            #region Act
            var results = Controller.RecentActivity()
                .AssertPartialViewRendered()
                .WithViewData<OrderTrackingHistory>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("CreatedBy5", results.CreatedBy);
            #endregion Assert
        }
        #endregion RecentActivity Tests

        #region RecentComments Tests


        [TestMethod, Ignore]
        public void TestRecentCommentsReturnsPartialView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var commentHistory = new List<CommentHistory>();
            for (int i = 0; i < 10; i++)
            {
                commentHistory.Add(CreateValidEntities.CommentHistory(i + 1));
                commentHistory[i].AccessUserId = "Me";
                commentHistory[i].DateCreated = DateTime.UtcNow.ToPacificTime().Date.AddDays(i);
            }
            commentHistory[8].AccessUserId = "NotMe";

            //new FakeCommentHistory(0, QueryRepositoryFactory.CommentHistoryRepository, commentHistory, false);

            #endregion Arrange

            #region Act
            var results = Controller.RecentComments()
                .AssertPartialViewRendered()
                .WithViewData<IList<CommentHistory>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Count());
            Assert.AreEqual("Comment10", results[0].Comment);
            Assert.AreEqual("Comment8", results[1].Comment);
            Assert.AreEqual("Comment7", results[2].Comment);
            Assert.AreEqual("Comment6", results[3].Comment);
            Assert.AreEqual("Comment5", results[4].Comment);
            #endregion Assert
        }

        #endregion RecentComments Tests

        #region RecentlyCompleted Tests


        [TestMethod, Ignore]
        public void TestRecentlyCompleted()
        {
            #region Arrange

            //new FakeOrderHistory(10, QueryRepositoryFactory.OrderHistoryRepository);
            var rtValue1 = new IndexedList<OrderHistory>();
            rtValue1.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            //rtValue1.Results = QueryRepositoryFactory.OrderHistoryRepository.Queryable.Take(5).ToList();

            var rtValue2 = new IndexedList<OrderHistory>();
            rtValue2.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            //rtValue2.Results = QueryRepositoryFactory.OrderHistoryRepository.Queryable.Take(3).ToList();

            Mock.Get(OrderService).Setup(a => a.GetIndexedListofOrders(null, null, false, false, OrderStatusCode.Codes.Denied, null, null, true,
                                                       new DateTime(DateTime.UtcNow.ToPacificTime().Year, DateTime.UtcNow.ToPacificTime().Month, 1), null)).Returns(rtValue1);
            Mock.Get(OrderService).Setup(a => a.GetIndexedListofOrders(null, null, true, false, OrderStatusCode.Codes.Complete, null, null, true,
                                                       new DateTime(DateTime.UtcNow.ToPacificTime().Year, DateTime.UtcNow.ToPacificTime().Month, 1), null)).Returns(rtValue2);
            #endregion Arrange

            #region Act
            var results = Controller.RecentlyCompleted()
                .AssertResultIs<JsonNetResult>();
            #endregion Act
            

            #region Assert
            dynamic data = JObject.FromObject(results.Data);
            Assert.AreEqual(5, (int)data.deniedThisMonth);
            Assert.AreEqual(3, (int)data.completedThisMonth);
            Mock.Get(OrderService).Verify(a => a.GetIndexedListofOrders(null, null, false, false, OrderStatusCode.Codes.Denied, null, null, true,
                                                       new DateTime(DateTime.UtcNow.ToPacificTime().Year, DateTime.UtcNow.ToPacificTime().Month, 1), null));
            Mock.Get(OrderService).Verify(a => a.GetIndexedListofOrders(null, null, true, false, OrderStatusCode.Codes.Complete, null, null, true,
                                                       new DateTime(DateTime.UtcNow.ToPacificTime().Year, DateTime.UtcNow.ToPacificTime().Month, 1), null));
            #endregion Assert
        }

        #endregion RecentlyCompleted Tests
        #endregion Method Tests

        #region Reflection Tests

        #region Controller Class Tests
        /// <summary>
        /// Tests the controller inherits from application controller.
        /// </summary>
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
            var result = controllerClass.GetFilteredCustomAttributes(true);
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
            var result = controllerClass.GetFilteredCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseTransactionsByDefaultAttribute not found.");
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
            var result = controllerClass.GetFilteredCustomAttributes(true).OfType<AutoValidateAntiforgeryTokenAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "AutoValidateAntiforgeryTokenAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasVersionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetFilteredCustomAttributes(true).OfType<Mvc.Attributes.VersionAttribute>();
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
            var result = controllerClass.GetFilteredCustomAttributes(true).OfType<AuthorizeAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "AuthorizeAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasProfileAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetFilteredCustomAttributes(true).OfType<ProfileAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "ProfileAttribute not found.");
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
        public void TestControllerMethodRecentActivityContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("RecentActivity");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodRecentCommentsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("RecentComments");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodRecentlyCompletedContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("RecentlyCompleted");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
