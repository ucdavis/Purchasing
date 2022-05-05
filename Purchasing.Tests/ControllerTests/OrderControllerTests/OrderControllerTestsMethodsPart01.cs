using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Tests.ControllerTests.OrderControllerTests
{
    public partial class OrderControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexRedirectsToHistoryIndex()
        {
            Controller.Index()
                .AssertActionRedirect();
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
                    users[i].Id = (i + 1).ToString();
                }
                Moq.Mock.Get(UserRepository2).SetupGet(a => a.Queryable).Returns(users.AsQueryable());
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
                users[i].Id = (i + 1).ToString();
                users[i].WorkgroupPermissions = new List<WorkgroupPermission>();
            }
            Moq.Mock.Get(UserRepository2).SetupGet(a => a.Queryable).Returns(users.AsQueryable());
            
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
            users[0].Id = "Me";
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
            Moq.Mock.Get(UserRepository2).SetupGet(a => a.Queryable).Returns(users.AsQueryable());
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
            users[0].Id = "Me";
            var permissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 5; i++)
            {
                permissions.Add(CreateValidEntities.WorkgroupPermission(i + 1));
                permissions[i].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);
                permissions[i].Workgroup = CreateValidEntities.Workgroup(i + 1);
                permissions[i].Workgroup.Id = i + 1;
            }
            permissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Admin);
            permissions[2].Workgroup.IsActive = false; //Don't use
            permissions[3].Workgroup.Administrative = true; //Can't request
            permissions[4].Workgroup.IsActive = false;
            users[0].WorkgroupPermissions = permissions;
            Moq.Mock.Get(UserRepository2).SetupGet(a => a.Queryable).Returns(users.AsQueryable());
            #endregion Arrange

            #region Act
            var result = Controller.SelectWorkgroup()
                .AssertActionRedirect();
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
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Approver;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect();
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
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Cancelled;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect();
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
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Complete;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect();
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
            orders[0].StatusCode.Id = OrderStatusCode.Codes.CompleteNotUploadedKfs;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect();
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
            orders[0].StatusCode.Id = OrderStatusCode.Codes.ConditionalApprover;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect();
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
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Denied;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect();
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
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Requester;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            #endregion Assert
        }

        //[TestMethod]
        //public void TestReroutePurchaserGetRedirectsWhenAlreadyAssigned1()
        //{
        //    #region Arrange
        //    var orders = new List<Order>();
        //    orders.Add(CreateValidEntities.Order(1));
        //    orders[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
        //    orders[0].Approvals.Add(CreateValidEntities.Approval(1));
        //    orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.Purchaser;
        //    orders[0].Approvals[0].User = CreateValidEntities.User(99);
        //    new FakeOrders(0, OrderRepository, orders);
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.ReroutePurchaser(1)
        //        .AssertActionRedirect()
        //        .ToAction<OrderController>(a => a.Review(1));
        //    #endregion Act

        //    #region Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, result.RouteValues["id"]);
        //    Assert.AreEqual("Order purchaser can not already be assigned to change purchaser.", Controller.ErrorMessage);
        //    #endregion Assert
        //}


        [TestMethod]
        public void TestReroutePurchaserGetReturnsView1()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
            orders[0].Approvals[0].User = CreateValidEntities.User(99);
            new FakeOrders(0, OrderRepository, orders);
            new FakeUsers(3, UserRepository);
            //new FakeOrderPeeps(3, OrderPeepRepository);
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
            new FakeUsers(12, UserRepository);
            SetupRoles();
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
            orders[0].Approvals[0].User = CreateValidEntities.User(99);
            orders[0].Workgroup.Id = 2;
            orders[0].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(1));
            orders[0].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(12));
            orders[0].Workgroup.Permissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            orders[0].Workgroup.Permissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            orders[0].Workgroup.Permissions[1].User = UserRepository.Queryable.Single(a => a.Id == "12");
            orders[0].Workgroup.Permissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            orders[0].Workgroup.Permissions[1].IsAdmin = true;
            new FakeOrders(0, OrderRepository, orders);
            

            //var orderPeeps = new List<OrderPeep>();
            //for (int i = 0; i < 12; i++)
            //{
            //    orderPeeps.Add(CreateValidEntities.OrderPeep(i+1));
            //    orderPeeps[i].OrderId = 1;
            //    orderPeeps[i].WorkgroupId = 2;
            //    orderPeeps[i].OrderStatusCodeId = OrderStatusCode.Codes.Purchaser;
            //    orderPeeps[i].UserId = (i+1).ToString();
            //}
            //orderPeeps[1].UserId = "99";
            //orderPeeps[2].OrderStatusCodeId = OrderStatusCode.Codes.AccountManager;
            //orderPeeps[3].OrderId = 9;
            //orderPeeps[4].WorkgroupId = 1;
            //orderPeeps[5].OrderStatusCodeId = OrderStatusCode.Codes.Approver;
            //orderPeeps[6].OrderStatusCodeId = OrderStatusCode.Codes.Requester;
            //orderPeeps[7].OrderStatusCodeId = OrderStatusCode.Codes.Cancelled;
            //orderPeeps[8].OrderStatusCodeId = OrderStatusCode.Codes.Complete;
            //orderPeeps[9].UserId = "1";
            //orderPeeps[10].UserId = "1";

            //new FakeOrderPeeps(0, OrderPeepRepository, orderPeeps);

            //var peeps = new List<OrderPeep>();
            //for (int i = 0; i < 10; i++)
            //{
            //    peeps.Add(CreateValidEntities.OrderPeep(i+1));
            //    peeps[i].OrderId = 1;
            //    peeps[i].WorkgroupId = 2;
            //}

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

        /// <summary>
        /// When already assigned
        /// </summary>
        [TestMethod]
        public void TestReroutePurchaserGetReturnsView3()
        {
            #region Arrange
            SetupRoles();
            new FakeUsers(12, UserRepository);
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.Purchaser;
            orders[0].Approvals[0].User = CreateValidEntities.User(99);
            orders[0].Workgroup.Id = 2;
            orders[0].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(1));
            orders[0].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(12));
            orders[0].Workgroup.Permissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            orders[0].Workgroup.Permissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            orders[0].Workgroup.Permissions[1].User = UserRepository.Queryable.Single(a => a.Id == "12");
            orders[0].Workgroup.Permissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            orders[0].Workgroup.Permissions[1].IsAdmin = true;
            new FakeOrders(0, OrderRepository, orders);
            

            //var orderPeeps = new List<OrderPeep>();
            //for (int i = 0; i < 12; i++)
            //{
            //    orderPeeps.Add(CreateValidEntities.OrderPeep(i + 1));
            //    orderPeeps[i].OrderId = 1;
            //    orderPeeps[i].WorkgroupId = 2;
            //    orderPeeps[i].OrderStatusCodeId = OrderStatusCode.Codes.Purchaser;
            //    orderPeeps[i].UserId = (i + 1).ToString();
            //}
            //orderPeeps[1].UserId = "99";
            //orderPeeps[2].OrderStatusCodeId = OrderStatusCode.Codes.AccountManager;
            //orderPeeps[3].OrderId = 9;
            //orderPeeps[4].WorkgroupId = 1;
            //orderPeeps[5].OrderStatusCodeId = OrderStatusCode.Codes.Approver;
            //orderPeeps[6].OrderStatusCodeId = OrderStatusCode.Codes.Requester;
            //orderPeeps[7].OrderStatusCodeId = OrderStatusCode.Codes.Cancelled;
            //orderPeeps[8].OrderStatusCodeId = OrderStatusCode.Codes.Complete;
            //orderPeeps[9].UserId = "1";
            //orderPeeps[10].UserId = "1";

            //new FakeOrderPeeps(0, OrderPeepRepository, orderPeeps);

            //var peeps = new List<OrderPeep>();
            //for (int i = 0; i < 10; i++)
            //{
            //    peeps.Add(CreateValidEntities.OrderPeep(i + 1));
            //    peeps[i].OrderId = 1;
            //    peeps[i].WorkgroupId = 2;
            //}

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

        #region ReroutePurchaser Post Tests
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestReroutePurchaserPostWhenOrderNotFound()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeOrders(3, OrderRepository);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.ReroutePurchaser(4, "test");
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
        public void TestReroutePurchaserPostRedirectsWhenWrongStatus1()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Approver;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1, "test")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Approval>()), Moq.Times.Never());
            Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(Moq.It.IsAny<Approval>(), Moq.It.IsAny<User>(), Moq.It.IsAny<bool>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserPostRedirectsWhenWrongStatus2()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Cancelled;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1, "test")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Approval>()), Moq.Times.Never());
            Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(Moq.It.IsAny<Approval>(), Moq.It.IsAny<User>(), Moq.It.IsAny<bool>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserPostRedirectsWhenWrongStatus3()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Complete;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1, "test")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Approval>()), Moq.Times.Never());
            Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(Moq.It.IsAny<Approval>(), Moq.It.IsAny<User>(), Moq.It.IsAny<bool>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserPostRedirectsWhenWrongStatus4()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.CompleteNotUploadedKfs;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1, "test")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Approval>()), Moq.Times.Never());
            Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(Moq.It.IsAny<Approval>(), Moq.It.IsAny<User>(), Moq.It.IsAny<bool>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserPostRedirectsWhenWrongStatus5()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.ConditionalApprover;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1, "test")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Approval>()), Moq.Times.Never());
            Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(Moq.It.IsAny<Approval>(), Moq.It.IsAny<User>(), Moq.It.IsAny<bool>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserPostRedirectsWhenWrongStatus6()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Denied;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1, "test")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Approval>()), Moq.Times.Never());
            Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(Moq.It.IsAny<Approval>(), Moq.It.IsAny<User>(), Moq.It.IsAny<bool>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserPostRedirectsWhenWrongStatus7()
        {
            #region Arrange
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Requester;
            new FakeOrders(0, OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1, "test")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Order Status must be at account manager or purchaser to change purchaser.", Controller.ErrorMessage);
            Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Approval>()), Moq.Times.Never());
            Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(Moq.It.IsAny<Approval>(), Moq.It.IsAny<User>(), Moq.It.IsAny<bool>()), Moq.Times.Never());
            #endregion Assert
        }

        //[TestMethod]
        //public void TestReroutePurchaserPostRedirectsWhenAlreadyAssigned1()
        //{
        //    #region Arrange
        //    var orders = new List<Order>();
        //    orders.Add(CreateValidEntities.Order(1));
        //    orders[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
        //    orders[0].Approvals.Add(CreateValidEntities.Approval(1));
        //    orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.Purchaser;
        //    orders[0].Approvals[0].User = CreateValidEntities.User(99);
        //    new FakeOrders(0, OrderRepository, orders);
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.ReroutePurchaser(1, "test")
        //        .AssertActionRedirect()
        //        .ToAction<OrderController>(a => a.Review(1));
        //    #endregion Act

        //    #region Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, result.RouteValues["id"]);
        //    Assert.AreEqual("Order purchaser can not already be assigned to change purchaser.", Controller.ErrorMessage);
        //    Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Approval>(), Moq.Times.Never()));
        //    Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(Moq.It.IsAny<Approval>(), Moq.It.IsAny<User>(), Moq.It.IsAny<bool>(), Moq.Times.Never()));
        //    #endregion Assert
        //}

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestReroutePurchaserThrowsExceptionIfUserDoesNotExist()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var orders = new List<Order>();
                orders.Add(CreateValidEntities.Order(1));
                orders[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
                orders[0].Approvals.Add(CreateValidEntities.Approval(1));
                orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
                orders[0].Approvals[0].User = CreateValidEntities.User(99);
                new FakeOrders(0, OrderRepository, orders);
                new FakeUsers(3, UserRepository);
                //new FakeOrderPeeps(3, OrderPeepRepository);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.ReroutePurchaser(1, "NotMe");
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
        [ExpectedException(typeof(PreconditionException))]
        public void TestReroutePurchaserThrowsExceptionIfUserNotPurchaser()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var orders = new List<Order>();
                orders.Add(CreateValidEntities.Order(1));
                orders[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
                orders[0].Approvals.Add(CreateValidEntities.Approval(1));
                orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
                orders[0].Approvals[0].User = CreateValidEntities.User(99);
                orders[0].Workgroup.Id = 2;
                orders[0].Approvals.Add(CreateValidEntities.Approval(1));
                orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.Purchaser;
                orders[0].Approvals[0].User = null;
                new FakeOrders(0, OrderRepository, orders);
                new FakeUsers(12, UserRepository);

                //var orderPeeps = new List<OrderPeep>();
                //for (int i = 0; i < 12; i++)
                //{
                //    orderPeeps.Add(CreateValidEntities.OrderPeep(i + 1));
                //    orderPeeps[i].OrderId = 1;
                //    orderPeeps[i].WorkgroupId = 2;
                //    orderPeeps[i].OrderStatusCodeId = OrderStatusCode.Codes.Purchaser;
                //    orderPeeps[i].UserId = (i + 1).ToString();
                //}
                //orderPeeps[1].UserId = "99";
                //orderPeeps[2].OrderStatusCodeId = OrderStatusCode.Codes.AccountManager;
                //orderPeeps[3].OrderId = 9;
                //orderPeeps[4].WorkgroupId = 1;
                //orderPeeps[5].OrderStatusCodeId = OrderStatusCode.Codes.Approver;
                //orderPeeps[6].OrderStatusCodeId = OrderStatusCode.Codes.Requester;
                //orderPeeps[7].OrderStatusCodeId = OrderStatusCode.Codes.Cancelled;
                //orderPeeps[8].OrderStatusCodeId = OrderStatusCode.Codes.Complete;
                //orderPeeps[9].UserId = "1";
                //orderPeeps[10].UserId = "1";

                //new FakeOrderPeeps(0, OrderPeepRepository, orderPeeps);

                //var peeps = new List<OrderPeep>();
                //for (int i = 0; i < 10; i++)
                //{
                //    peeps.Add(CreateValidEntities.OrderPeep(i + 1));
                //    peeps[i].OrderId = 1;
                //    peeps[i].WorkgroupId = 2;
                //}
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.ReroutePurchaser(1, "3");
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Precondition failed.", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestReroutePurchaserWhenValid1()
        {
            #region Arrange
            new FakeUsers(12, UserRepository);
            SetupRoles();
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
            orders[0].Approvals[0].User = CreateValidEntities.User(99);
            orders[0].Workgroup.Id = 2;
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[1].StatusCode.Id = OrderStatusCode.Codes.Purchaser;
            orders[0].Approvals[1].User = null;
            orders[0].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(1));
            orders[0].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(12));
            orders[0].Workgroup.Permissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            orders[0].Workgroup.Permissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            orders[0].Workgroup.Permissions[1].User = UserRepository.Queryable.Single(a => a.Id == "12");
            orders[0].Workgroup.Permissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            orders[0].Workgroup.Permissions[1].IsAdmin = true;
            new FakeOrders(0, OrderRepository, orders);
            

            //var orderPeeps = new List<OrderPeep>();
            //for (int i = 0; i < 12; i++)
            //{
            //    orderPeeps.Add(CreateValidEntities.OrderPeep(i + 1));
            //    orderPeeps[i].OrderId = 1;
            //    orderPeeps[i].WorkgroupId = 2;
            //    orderPeeps[i].OrderStatusCodeId = OrderStatusCode.Codes.Purchaser;
            //    orderPeeps[i].UserId = (i + 1).ToString();
            //}
            //orderPeeps[1].UserId = "99";
            //orderPeeps[2].OrderStatusCodeId = OrderStatusCode.Codes.AccountManager;
            //orderPeeps[3].OrderId = 9;
            //orderPeeps[4].WorkgroupId = 1;
            //orderPeeps[5].OrderStatusCodeId = OrderStatusCode.Codes.Approver;
            //orderPeeps[6].OrderStatusCodeId = OrderStatusCode.Codes.Requester;
            //orderPeeps[7].OrderStatusCodeId = OrderStatusCode.Codes.Cancelled;
            //orderPeeps[8].OrderStatusCodeId = OrderStatusCode.Codes.Complete;
            //orderPeeps[9].UserId = "1";
            //orderPeeps[10].UserId = "1";

            //new FakeOrderPeeps(0, OrderPeepRepository, orderPeeps);

            //var peeps = new List<OrderPeep>();
            //for (int i = 0; i < 10; i++)
            //{
            //    peeps.Add(CreateValidEntities.OrderPeep(i + 1));
            //    peeps[i].OrderId = 1;
            //    peeps[i].WorkgroupId = 2;
            //}
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1, "1")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Order  rerouted to purchaser FirstName1 LastName1", Controller.Message);
            Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(orders[0].Approvals[1]));
            Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(orders[0].Approvals[1], UserRepository.Queryable.Single(b => b.Id == "1"), false));

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            #endregion Assert
        }
        [TestMethod]
        public void TestReroutePurchaserWhenValid2()
        {
            #region Arrange
            SetupRoles();
            new FakeUsers(12, UserRepository);
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
            orders[0].Approvals[0].User = CreateValidEntities.User(99);
            orders[0].Workgroup.Id = 2;
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.Purchaser;
            orders[0].Approvals[0].User = CreateValidEntities.User(99);
            orders[0].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(1));
            orders[0].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(12));
            orders[0].Workgroup.Permissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            orders[0].Workgroup.Permissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            orders[0].Workgroup.Permissions[1].User = UserRepository.Queryable.Single(a => a.Id == "12");
            orders[0].Workgroup.Permissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            orders[0].Workgroup.Permissions[1].IsAdmin = true;
            new FakeOrders(0, OrderRepository, orders);
            

            //var orderPeeps = new List<OrderPeep>();
            //for (int i = 0; i < 12; i++)
            //{
            //    orderPeeps.Add(CreateValidEntities.OrderPeep(i + 1));
            //    orderPeeps[i].OrderId = 1;
            //    orderPeeps[i].WorkgroupId = 2;
            //    orderPeeps[i].OrderStatusCodeId = OrderStatusCode.Codes.Purchaser;
            //    orderPeeps[i].UserId = (i + 1).ToString();
            //}
            //orderPeeps[1].UserId = "99";
            //orderPeeps[2].OrderStatusCodeId = OrderStatusCode.Codes.AccountManager;
            //orderPeeps[3].OrderId = 9;
            //orderPeeps[4].WorkgroupId = 1;
            //orderPeeps[5].OrderStatusCodeId = OrderStatusCode.Codes.Approver;
            //orderPeeps[6].OrderStatusCodeId = OrderStatusCode.Codes.Requester;
            //orderPeeps[7].OrderStatusCodeId = OrderStatusCode.Codes.Cancelled;
            //orderPeeps[8].OrderStatusCodeId = OrderStatusCode.Codes.Complete;
            //orderPeeps[9].UserId = "1";
            //orderPeeps[10].UserId = "1";

            //new FakeOrderPeeps(0, OrderPeepRepository, orderPeeps);

            //var peeps = new List<OrderPeep>();
            //for (int i = 0; i < 10; i++)
            //{
            //    peeps.Add(CreateValidEntities.OrderPeep(i + 1));
            //    peeps[i].OrderId = 1;
            //    peeps[i].WorkgroupId = 2;
            //}
            #endregion Arrange

            #region Act
            var result = Controller.ReroutePurchaser(1, "1")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Order  rerouted to purchaser FirstName1 LastName1", Controller.Message);
            Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(orders[0].Approvals[0]));
            Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(orders[0].Approvals[0], UserRepository.Queryable.Single(b => b.Id == "1"), false));

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestReroutePurchaserWhenValid3()
        {
            #region Arrange
            new FakeUsers(12, UserRepository);
            SetupRoles();
            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].StatusCode.Id = OrderStatusCode.Codes.Purchaser;
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[0].StatusCode.Id = OrderStatusCode.Codes.AccountManager;
            orders[0].Approvals[0].User = CreateValidEntities.User(99);
            orders[0].Workgroup.Id = 2;
            orders[0].Approvals.Add(CreateValidEntities.Approval(1));
            orders[0].Approvals[1].StatusCode.Id = OrderStatusCode.Codes.Purchaser;
            orders[0].Approvals[1].User = null;
            orders[0].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(1));
            orders[0].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(12));
            orders[0].Workgroup.Permissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            orders[0].Workgroup.Permissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            orders[0].Workgroup.Permissions[1].User = UserRepository.Queryable.Single(a => a.Id == "12");
            orders[0].Workgroup.Permissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            orders[0].Workgroup.Permissions[1].IsAdmin = true;
            new FakeOrders(0, OrderRepository, orders);
            

            //var orderPeeps = new List<OrderPeep>();
            //for (int i = 0; i < 12; i++)
            //{
            //    orderPeeps.Add(CreateValidEntities.OrderPeep(i + 1));
            //    orderPeeps[i].OrderId = 1;
            //    orderPeeps[i].WorkgroupId = 2;
            //    orderPeeps[i].OrderStatusCodeId = OrderStatusCode.Codes.Purchaser;
            //    orderPeeps[i].UserId = (i + 1).ToString();
            //}
            //orderPeeps[1].UserId = "99";
            //orderPeeps[2].OrderStatusCodeId = OrderStatusCode.Codes.AccountManager;
            //orderPeeps[3].OrderId = 9;
            //orderPeeps[4].WorkgroupId = 1;
            //orderPeeps[5].OrderStatusCodeId = OrderStatusCode.Codes.Approver;
            //orderPeeps[6].OrderStatusCodeId = OrderStatusCode.Codes.Requester;
            //orderPeeps[7].OrderStatusCodeId = OrderStatusCode.Codes.Cancelled;
            //orderPeeps[8].OrderStatusCodeId = OrderStatusCode.Codes.Complete;
            //orderPeeps[9].UserId = "1";
            //orderPeeps[10].UserId = "1";

            //new FakeOrderPeeps(0, OrderPeepRepository, orderPeeps);

            //var peeps = new List<OrderPeep>();
            //for (int i = 0; i < 10; i++)
            //{
            //    peeps.Add(CreateValidEntities.OrderPeep(i + 1));
            //    peeps[i].OrderId = 1;
            //    peeps[i].WorkgroupId = 2;
            //}
            #endregion Arrange

            #region Act
            Controller.ReroutePurchaser(1, "1")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Order  rerouted to purchaser FirstName1 LastName1", Controller.Message);
            Moq.Mock.Get(ApprovalRepository).Verify(a => a.EnsurePersistent(orders[0].Approvals[1]));
            Moq.Mock.Get(OrderService).Verify(a => a.ReRouteSingleApprovalForExistingOrder(orders[0].Approvals[1], UserRepository.Queryable.Single(b => b.Id == "1"), true));

            #endregion Assert
        }
        #endregion ReroutePurchaser Post Tests

    }
}
