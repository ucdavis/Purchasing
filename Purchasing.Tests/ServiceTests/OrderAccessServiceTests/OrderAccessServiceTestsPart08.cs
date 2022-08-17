//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Purchasing.Core.Domain;
//
//namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
//{
//    public partial class OrderAccessServiceTests
//    {
//        #region GetCompletedOrders Tests
//        [TestMethod]
//        public void TestGetCompletedOrdersReturnsExpectedResultsWithoutPassedParm1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(0, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetCompletedOrdersReturnsExpectedResultsWithoutPassedParm2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(0, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TesGetCompletedOrdersReturnsExpectedResultsWithPassedParm1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: false);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(0, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TesGetCompletedOrdersReturnsExpectedResultsWithPassedParm2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: false);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(0, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TesGetCompletedOrdersReturnsExpectedResultsWithPassedParm3()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1, true);

//            var apps = approvals.Where(a => a.Order.Workgroup.Id == 2 && a.User.Id == "hsimpson"); //Remove Homer from WG 2 tracking
//            foreach(var approval in apps)
//            {
//                approval.User = UserRepository.Queryable.Single(a => a.Id == "awong");
//            }

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TesGetCompletedOrdersReturnsExpectedResultsWithPassedParm4()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1, true);

//            var apps = approvals.Where(a => a.Order.Workgroup.Id == 2 && a.User.Id == "hsimpson"); //Remove Homer from WG 2 tracking
//            foreach (var approval in apps)
//            {
//                approval.User = UserRepository.Queryable.Single(a => a.Id == "awong");
//            }

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetCompletedOrdersReturnsExpectedResultsWhenRequester1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("moe").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 10, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(allActive: true, all: true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(12, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetCompletedOrdersReturnsExpectedResultsWhenRequester2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("bender").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 10, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(allActive: true, all: true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(5, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetCompletedOrdersReturnsExpectedResultsWhenApprover1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 2); //not in this wg.
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 10, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetCompletedOrdersReturnsExpectedResultsWhenApprover2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 2); //not in this wg.
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 10, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(allActive:true, all: true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(16, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetCompletedOrdersReturnsExpectedResultsWhenAccountManager1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 2); //not in this wg.
//            // Remove flanders from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "flanders").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 10, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetCompletedOrdersReturnsExpectedResultsWhenAccountManager2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 2); //not in this wg.
//            // Remove flanders from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "flanders").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 10, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(allActive: true, all: true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(16, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetCompletedOrdersReturnsExpectedResultsWhenPurchaser1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("awong").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 2); //not in this wg.
//            // Remove awong from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "awong").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 10, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(all: true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count());
//            #endregion Assert
//        }

//        #endregion GetCompletedOrders Tests
//    }
//}
