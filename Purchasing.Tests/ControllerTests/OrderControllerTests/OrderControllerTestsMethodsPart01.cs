using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.Windsor;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Core.Domain;
using Purchasing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;
using Purchasing.WS;

namespace Purchasing.Tests.ControllerTests.OrderControllerTests
{
    public partial class OrderControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexRedirectsToHistoryIndex()
        {
            Controller.Index()
                .AssertActionRedirect()
                .ToAction<HistoryController>(a => a.Index(null, null, null, null, null, false, false));
        }
        #endregion Index Tests

        #region SelectWorkgroup Tests

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestSelectWorkgroupThrowsExceptionIfCurrentUserNotFound()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
                var users = new List<User>();
                for (int i = 0; i < 3; i++)
                {
                    users.Add(CreateValidEntities.User(i+1));
                    users[i].SetIdTo((i + 1).ToString());
                }
                UserRepository2.Expect(a => a.Queryable).Return(users.AsQueryable()).Repeat.Any();
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.SelectWorkgroup();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains no matching element", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestSelectWorkgroupReturnsViewWhenNoWorkgroupsFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "2");
            SetupRoles();
            var users = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].SetIdTo((i + 1).ToString());
                users[i].WorkgroupPermissions = new List<WorkgroupPermission>();
            }
            UserRepository2.Expect(a => a.Queryable).Return(users.AsQueryable()).Repeat.Any();
            
            #endregion Arrange

            #region Act
            var result = Controller.SelectWorkgroup()
                .AssertViewRendered()
                .WithViewData<IEnumerable<Workgroup>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            #endregion Assert		
        }


        [TestMethod]
        public void TestSelectWorkgroupReturnsViewWhenMultipleWorkgroupsForUserFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupRoles();

            var users = new List<User>();
            users.Add(CreateValidEntities.User(2));
            users[0].SetIdTo("Me");
            var permissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 5; i++)
            {
                permissions.Add(CreateValidEntities.WorkgroupPermission(i+1));
                permissions[i].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);
                permissions[i].Workgroup = CreateValidEntities.Workgroup(i + 1);
            }
            permissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Admin);
            permissions[2].Workgroup.IsActive = false; //Don't use
            permissions[3].Workgroup.Administrative = true; //Can't request
            users[0].WorkgroupPermissions = permissions;
            UserRepository2.Expect(a => a.Queryable).Return(users.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.SelectWorkgroup()
                .AssertViewRendered()
                .WithViewData<IEnumerable<Workgroup>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Name2", result.ElementAt(0).Name);
            Assert.AreEqual("Name5", result.ElementAt(1).Name);
            #endregion Assert		
        }


        [TestMethod]
        public void TestSelectWorkgroupRedirectsWhenExactlyOneWorkgroup()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupRoles();

            var users = new List<User>();
            users.Add(CreateValidEntities.User(2));
            users[0].SetIdTo("Me");
            var permissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 5; i++)
            {
                permissions.Add(CreateValidEntities.WorkgroupPermission(i + 1));
                permissions[i].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);
                permissions[i].Workgroup = CreateValidEntities.Workgroup(i + 1);
                permissions[i].Workgroup.SetIdTo(i + 1);
            }
            permissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Admin);
            permissions[2].Workgroup.IsActive = false; //Don't use
            permissions[3].Workgroup.Administrative = true; //Can't request
            permissions[4].Workgroup.IsActive = false;
            users[0].WorkgroupPermissions = permissions;
            UserRepository2.Expect(a => a.Queryable).Return(users.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.SelectWorkgroup()
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.Request(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            #endregion Assert		
        }

        #endregion SelectWorkgroup Tests

        #region ReroutePurchaser Get Tests

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestReroutePurchaserGetWhenOrderNotFound()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeOrders(3, OrderRepository);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.ReroutePurchaser(4);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains no matching element", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestReroutePurchaserGetRedirectsWhenWrongStatus1()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.SetIdTo(OrderStatusCode.Codes.Approver);
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.Review(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestReroutePurchaserGetRedirectsWhenWrongStatus2()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.SetIdTo(OrderStatusCode.Codes.Cancelled);
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.Review(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserGetRedirectsWhenWrongStatus3()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.SetIdTo(OrderStatusCode.Codes.Complete);
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.Review(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserGetRedirectsWhenWrongStatus4()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.SetIdTo(OrderStatusCode.Codes.CompleteNotUploadedKfs);
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.Review(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserGetRedirectsWhenWrongStatus5()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.SetIdTo(OrderStatusCode.Codes.ConditionalApprover);
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.Review(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserGetRedirectsWhenWrongStatus6()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.SetIdTo(OrderStatusCode.Codes.Denied);
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.Review(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserGetRedirectsWhenWrongStatus7()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.SetIdTo(OrderStatusCode.Codes.Requester);
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.Review(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserGetRedirectsWhenAlreadyAssigned1()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.SetIdTo(OrderStatusCode.Codes.AccountManager);
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[0].StatusCode.SetIdTo(OrderStatusCode.Codes.Purchaser);
            orders[0].Approvals[0].User = CreateValidEntities.User(99);
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.Review(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order purchaser can not already be assigned to change purchaser.", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestReroutePurchaserGetReturnsView1()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.SetIdTo(OrderStatusCode.Codes.AccountManager);
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[0].StatusCode.SetIdTo(OrderStatusCode.Codes.AccountManager);
            orders[0].Approvals[0].User = CreateValidEntities.User(99);
            new FakeOrders(0, OrderRepository, orders);
            new FakeUsers(3, UserRepository);
            new FakeOrderPeeps(3, OrderPeepRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertViewRendered()
                .WithViewData<OrderReRoutePurchaserModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.PurchaserPeeps.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestReroutePurchaserGetReturnsView2()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.SetIdTo(OrderStatusCode.Codes.AccountManager);
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[0].StatusCode.SetIdTo(OrderStatusCode.Codes.AccountManager);
            orders[0].Approvals[0].User = CreateValidEntities.User(99);
            orders[0].Workgroup.SetIdTo(2);
            new FakeOrders(0, OrderRepository, orders);
            new FakeUsers(12, UserRepository);

            var orderPeeps = new List<OrderPeep>();
            for (int i = 0; i < 12; i++)
            {
                orderPeeps.Add(CreateValidEntities.OrderPeep(i+1));
                orderPeeps[i].OrderId = 1;
                orderPeeps[i].WorkgroupId = 2;
                orderPeeps[i].OrderStatusCodeId = OrderStatusCode.Codes.Purchaser;
                orderPeeps[i].UserId = (i+1).ToString();
            }
            orderPeeps[1].UserId = "99";
            orderPeeps[2].OrderStatusCodeId = OrderStatusCode.Codes.AccountManager;
            orderPeeps[3].OrderId = 9;
            orderPeeps[4].WorkgroupId = 1;
            orderPeeps[5].OrderStatusCodeId = OrderStatusCode.Codes.Approver;
            orderPeeps[6].OrderStatusCodeId = OrderStatusCode.Codes.Requester;
            orderPeeps[7].OrderStatusCodeId = OrderStatusCode.Codes.Cancelled;
            orderPeeps[8].OrderStatusCodeId = OrderStatusCode.Codes.Complete;
            orderPeeps[9].UserId = "1";
            orderPeeps[10].UserId = "1";

            new FakeOrderPeeps(0, OrderPeepRepository, orderPeeps);

            var peeps = new List<OrderPeep>();
            for (int i = 0; i < 10; i++)
            {
                peeps.Add(CreateValidEntities.OrderPeep(i+1));
                peeps[i].OrderId = 1;
                peeps[i].WorkgroupId = 2;
            }

            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertViewRendered()
                .WithViewData<OrderReRoutePurchaserModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.PurchaserPeeps.Count());
            Assert.AreEqual("1", result.PurchaserPeeps[0].Id);
            Assert.AreEqual("12", result.PurchaserPeeps[1].Id);
            #endregion Assert
        }
        #endregion ReroutePurchaser Get Tests

        #region Method Tests

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
    }
}
