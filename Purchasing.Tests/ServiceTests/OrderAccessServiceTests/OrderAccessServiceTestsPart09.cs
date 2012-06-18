//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Purchasing.Core.Domain;
//using Rhino.Mocks;

//namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
//{
//    public partial class OrderAccessServiceTests
//    {
//        #region Filter Tests

//        [TestMethod]
//        public void TestOwnedFilter1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 10, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results1 = OrderService.GetListofOrders();
//            var results2 = OrderService.GetListofOrders(owned:true);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results1);
//            Assert.AreEqual(16, results1.Count);
//            Assert.IsNotNull(results2);
//            Assert.AreEqual(10, results2.Count);
//            foreach (var order in results2)
//            {
//                Assert.AreEqual("hsimpson", order.CreatedBy.Id);
//            }
//            #endregion Assert	
//        }

//        [TestMethod]
//        public void TestOwnedFilter2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();            
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act

//            var results = OrderService.GetListofOrders(all: true, allActive:true, owned: true);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(6, results.Count);
//            foreach(var order in results)
//            {
//                Assert.AreEqual("hsimpson", order.CreatedBy.Id);
//            }
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestAllFilter1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 5, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 5, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act

//            var results = OrderService.GetListofOrders(all: false, allActive: true, owned: true);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(4, results.Count);
//            #endregion Assert
//        }


//        [TestMethod]
//        public void TestAllFilter2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 5, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 5, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(14, results.Count);
//            #endregion Assert
//        }


//        [TestMethod]
//        public void TestOrderStatusCodes1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);
//            SetupOrderTracking();
//            List<OrderStatusCode> orderStatusCodes = null;
//            #endregion Arrange

//            #region Act
//// ReSharper disable ConditionIsAlwaysTrueOrFalse
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, orderStatusCodes: orderStatusCodes);
//// ReSharper restore ConditionIsAlwaysTrueOrFalse
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(6, results.Count);
//            #endregion Assert	
//        }


//        [TestMethod]
//        public void TestOrderStatusCodes2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);
//            SetupOrderTracking();
//            var orderStatusCodes = new List<OrderStatusCode>();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, orderStatusCodes: orderStatusCodes);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(6, results.Count);
//            #endregion Assert
//        }
//        [TestMethod]
//        public void TestOrderStatusCodes3()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);
//            SetupOrderTracking();
//            var orderStatusCodes = new List<OrderStatusCode>();
//            orderStatusCodes.Add(OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, orderStatusCodes: orderStatusCodes);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(2, results.Count);
//            foreach (var result in results)
//            {
//                Assert.AreEqual(2, result.StatusCode.Level);
//            }
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestOrderStatusCodes4()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);
//            SetupOrderTracking();
//            var orderStatusCodes = new List<OrderStatusCode>();
//            orderStatusCodes.Add(OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, orderStatusCodes: orderStatusCodes);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(1, results.Count);
//            foreach(var result in results)
//            {
//                Assert.AreEqual(3, result.StatusCode.Level);
//            }
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestOrderStatusCodes5()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);
//            SetupOrderTracking();
//            var orderStatusCodes = new List<OrderStatusCode>();
//            orderStatusCodes.Add(OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser));
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, orderStatusCodes: orderStatusCodes);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(1, results.Count);
//            foreach(var result in results)
//            {
//                Assert.AreEqual(4, result.StatusCode.Level);
//            }
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestOrderStatusCodes6()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);
//            SetupOrderTracking();
//            var orderStatusCodes = new List<OrderStatusCode>();
//            orderStatusCodes.Add(OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete));
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, orderStatusCodes: orderStatusCodes);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(2, results.Count);
//            foreach(var result in results)
//            {
//                Assert.AreEqual(5, result.StatusCode.Level);
//            }
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestOrderStatusCodes7()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);
//            SetupOrderTracking();
//            var orderStatusCodes = new List<OrderStatusCode>();
//            orderStatusCodes.Add(OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete));
//            orderStatusCodes.Add(OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, orderStatusCodes: orderStatusCodes);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(4, results.Count);
//            foreach(var result in results)
//            {
//                Assert.IsTrue(result.StatusCode.Level == 2 || result.StatusCode.Level == 5);
//            }
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates1()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach (var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, startDate: dateSeed.AddDays(-1));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(6, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates2()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, startDate: dateSeed);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(5, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates3()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, startDate: dateSeed.AddDays(1));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(4, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates4()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, startDate: dateSeed.AddDays(2));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(3, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates5()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, startDate: dateSeed.AddDays(3));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(2, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates6()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, startDate: dateSeed.AddDays(4));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(1, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates7()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, startDate: dateSeed.AddDays(5));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(0, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates8()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, endDate: dateSeed.AddDays(6));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(6, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates9()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, endDate: dateSeed.AddDays(5));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(5, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates10()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, endDate: dateSeed.AddDays(4));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(4, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates11()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, endDate: dateSeed.AddDays(3));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(3, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates12()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, endDate: dateSeed.AddDays(2));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(2, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates13()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, endDate: dateSeed.AddDays(1));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(1, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates14()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, endDate: dateSeed);
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(0, results.Count);
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestDates15()
//        {
//            #region Arrange
//            var dateSeed = DateTime.Now.Date;
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            var i = 0;
//            foreach(var order in orders)
//            {
//                order.DateCreated = dateSeed.AddDays(i++);
//            }
//            SetupOrders(orders, approvals, 0, "hsimpson", null, 0, true);
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true, allActive: true, owned: true, endDate: dateSeed.AddDays(4), startDate:dateSeed.AddDays(2));
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(1, results.Count);
//            #endregion Assert
//        }
//        #endregion Filter Tests
//    }
//}
