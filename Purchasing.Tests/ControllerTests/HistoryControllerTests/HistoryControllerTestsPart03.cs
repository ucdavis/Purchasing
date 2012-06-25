using System;
using System.Linq;
using Castle.Windsor;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Web.Models;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using MvcContrib.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Purchasing.Core;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Purchasing.Tests.ControllerTests.HistoryControllerTests
{
    public partial class HistoryControllerTests
    {
        #region RecentActivity Tests


        [TestMethod]
        public void TestRecentActivityReturnsPartialView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            var orderTrackingHistory = new List<OrderTrackingHistory>();
            for (int i = 0; i < 5; i++)
            {
                orderTrackingHistory.Add(CreateValidEntities.OrderTrackingHistory(i+1));
                orderTrackingHistory[i].AccessUserId = "Me";
                orderTrackingHistory[i].DateCreated = DateTime.Now.Date;
            }
            orderTrackingHistory[3].DateCreated = DateTime.Now.AddDays(3);
            orderTrackingHistory[4].DateCreated = DateTime.Now.AddDays(3);
            orderTrackingHistory[4].AccessUserId = "NotMe";

            new FakeOrderTrackingHistory(0, OrderTrackingHistoryRepository, orderTrackingHistory);
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

        [TestMethod]
        public void TestRecentActivityReturnsPartialView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            var orderTrackingHistory = new List<OrderTrackingHistory>();
            for (int i = 0; i < 5; i++)
            {
                orderTrackingHistory.Add(CreateValidEntities.OrderTrackingHistory(i + 1));
                orderTrackingHistory[i].AccessUserId = "Me";
                orderTrackingHistory[i].DateCreated = DateTime.Now.Date;
            }
            orderTrackingHistory[3].DateCreated = DateTime.Now.AddDays(3);
            orderTrackingHistory[4].DateCreated = DateTime.Now.AddDays(3);
            //orderTrackingHistory[4].AccessUserId = "NotMe";

            new FakeOrderTrackingHistory(0, OrderTrackingHistoryRepository, orderTrackingHistory);
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

        [TestMethod]
        public void TestRecentActivityReturnsPartialView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            var orderTrackingHistory = new List<OrderTrackingHistory>();
            for (int i = 0; i < 5; i++)
            {
                orderTrackingHistory.Add(CreateValidEntities.OrderTrackingHistory(i + 1));
                orderTrackingHistory[i].AccessUserId = "Me";
                orderTrackingHistory[i].DateCreated = DateTime.Now.Date;
            }
            orderTrackingHistory[3].DateCreated = DateTime.Now.AddDays(3);
            orderTrackingHistory[4].DateCreated = DateTime.Now.AddDays(4); //changed from other tests
            //orderTrackingHistory[4].AccessUserId = "NotMe";

            new FakeOrderTrackingHistory(0, OrderTrackingHistoryRepository, orderTrackingHistory);
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


        [TestMethod]
        public void TestRecentCommentsReturnsPartialView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            var commentHistory = new List<CommentHistory>();
            for (int i = 0; i < 10; i++)
            {
                commentHistory.Add(CreateValidEntities.CommentHistory(i+1));
                commentHistory[i].AccessUserId = "Me";
                commentHistory[i].DateCreated = DateTime.Now.Date.AddDays(i);
            }
            commentHistory[8].AccessUserId = "NotMe";

            new FakeCommentHistory(0, CommentHistoryRepository, commentHistory, false);

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


        [TestMethod]
        public void TestRecentlyCompleted()
        {
            #region Arrange

            new FakeOrderHistory(10, OrderHistoryRepository);

            OrderService.Expect(a => a.GetListofOrders(false, false, OrderStatusCode.Codes.Denied, null, null, true,
                                                       new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), null))
                                                       .Return(OrderHistoryRepository.Queryable.Take(5).AsQueryable());
            OrderService.Expect(a => a.GetListofOrders(true, false, OrderStatusCode.Codes.Complete, null, null, true,
                                                       new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), null))
                                                       .Return(OrderHistoryRepository.Queryable.Take(3).AsQueryable());
            #endregion Arrange

            #region Act
            var results = Controller.RecentlyCompleted()
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = results.Data;
            Assert.AreEqual(5, data.deniedThisMonth);
            Assert.AreEqual(3, data.completedThisMonth);
            OrderService.AssertWasCalled(a => a.GetListofOrders(false, false, OrderStatusCode.Codes.Denied, null, null, true,
                                                       new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), null));
            OrderService.AssertWasCalled(a => a.GetListofOrders(true, false, OrderStatusCode.Codes.Complete, null, null, true,
                                                       new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), null));
            #endregion Assert		
        }

        #endregion RecentlyCompleted Tests

    }
}
