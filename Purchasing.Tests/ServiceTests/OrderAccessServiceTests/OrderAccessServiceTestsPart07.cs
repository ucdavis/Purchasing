//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Purchasing.Core.Domain;
//
//namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
//{
//    public partial class OrderAccessServiceTests
//    {
//        #region GetActiveOrders Tests

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWithoutPassedParm1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

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
//        public void TestGetActiveOrdersReturnsExpectedResultsWithPassedParm1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(false);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(0, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWithPassedParm2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenApprover1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            // Remove hsimpson from all approvals for wg 2 (order 22 added above)
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenApprover2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1, true);
//            // Remove hsimpson from all approvals for wg 2 (order 22 added above)
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenApprover3()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2); //not in this wg.
//            // Remove hsimpson from all approvals for wg 2 (order 22 added above)
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count()); //But these are active ones for Approver.
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenApprover4()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            // Remove hsimpson from all approvals for wg 2 (order 22 added above)
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(2, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenApprover5()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(2, results.Count());
//            #endregion Assert
//        }


//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenAccountManager1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            // Remove flanders from all approvals for wg 2 (order 22 added above)
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "flanders").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenAccountManager2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2); //not in this wg.
//            // Remove flanders from all approvals for wg 2 (order 22 added above)
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "flanders").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count()); //but these are active
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenAccountManager3()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(0, results.Count()); //zero because not active for this user yet, and not part of the past actions
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenAccountManager4()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            // Remove flanders from all approvals for wg 2 (order 22 added above)
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "flanders").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(2, results.Count());
//            #endregion Assert
//        }

//        public void TestGetActiveOrdersReturnsExpectedResultsWhenAccountManager5()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(2, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenPurchaser1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("awong").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            // Remove awong from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "awong").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(6, results.Count()); //because that is their current status
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenPurchaser2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("awong").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(0, results.Count()); 
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenPurchaser3()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("awong").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2); //not in this wg.
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(0, results.Count());
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenPurchaser4()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("awong").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            // Remove awong from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "awong").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(2, results.Count()); 
//            #endregion Assert
//        }
//        [TestMethod]
//        public void TestGetActiveOrdersReturnsExpectedResultsWhenPurchaser5()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("awong").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //not in this wg.
//            // Remove awong from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "awong").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            SetupOrderTracking();
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders(true);
//            #endregion Act

//            #region Assert
//            Assert.AreEqual(2, results.Count());
//            #endregion Assert
//        }
//        #endregion GetActiveOrders Tests
//    }
//}
