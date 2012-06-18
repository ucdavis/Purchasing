//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Purchasing.Core.Domain;
//using Purchasing.Tests.Core;
//using Rhino.Mocks;
//using UCDArch.Testing;

//namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
//{

//    public partial class OrderAccessServiceTests
//    {
//        #region Purchaser Tests

//        /// <summary>
//        /// awong is a purchaser for wg1
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForPurchaser1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("awong").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //Doesn't see this one. Good.
//            // Remove awong from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "awong").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(6, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// Not all orders are at purchaser stage.
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForPurchaser2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("awong").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2); //Doesn't see this one. Good.
//            // Remove awong from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "awong").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(2, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// grimes is a purchaser for wg1, but not for these orders
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForPurchaser3()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("grimes").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(0, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// awong is away, so grimes can see purchase level for wg 1
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForPurchaser4()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("grimes").Repeat.Any();
//            SetupUsers1(null, false);
//            var user = CreateValidEntities.User(4);
//            user.FirstName = "Amy";
//            user.LastName = "Wong";
//            user.IsAway = true;
//            user.SetIdTo("awong");
//            SetupUsers1(user);
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(6, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// user is null, so grimes can see them
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForPurchaser5()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("grimes").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            var orderStatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode == orderStatusCode)
//                {
//                    approval.User = null;
//                }
//            }
//            SetupOrders(orders, approvals, 0, null, null, 0, true);
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(6, results.Count);
//            #endregion Assert
//        }


//        /// <summary>
//        /// grimes is secondary user so can see them.
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForPurchaser6()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("grimes").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
//            // Remove grimes from all approvals for wg 2 
//            var lastOrder = orders.Count();

//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            var orderStatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode == orderStatusCode)
//                {
//                    approval.User = UserRepository.GetNullableById("grimes");
//                }
//            }
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "grimes").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 0, null, null, 0, true);
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(6, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// grimes is secondary user, but not for purchase status, so can't see them.
//        /// don't need to do this test for other status, cause if it works for this one, true for all
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForPurchaser7()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("grimes").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            var orderStatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode == orderStatusCode)
//                {
//                    approval.User = UserRepository.GetNullableById("grimes");
//                }
//            }
//            SetupOrders(orders, approvals, 0, null, null, 0, true);
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(0, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// other status is null, but grimes still can't see them
//        /// don't need to do this test for other status, cause if it works for this one, true for all
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForPurchaser8()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("grimes").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            var orderStatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode == orderStatusCode)
//                {
//                    approval.User = null;
//                }
//            }
//            SetupOrders(orders, approvals, 0, null, null, 0, true);
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(0, results.Count);
//            #endregion Assert
//        } 
//        #endregion Purchaser Tests
//    }
//}
