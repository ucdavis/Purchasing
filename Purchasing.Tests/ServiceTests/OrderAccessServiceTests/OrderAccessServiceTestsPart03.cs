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
//        #region Account Manager Tests

//        /// <summary>
//        /// Flanders is an account manager for wg 1, but not wg 2. 
//        /// The only orders at the account manager stage are for wg 2
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForAccountManager1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
//            // Remove flanders from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "flanders").ToList();
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
//            Assert.AreEqual(0, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// The first 4 orders are at the account manager stage for wg1
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForAccountManager2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
//            // Remove flanders from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "flanders").ToList();
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
//            Assert.AreEqual(4, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// brannigan is an AccountManager, but not for these orders and wg
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForAccountManager3()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("brannigan").Repeat.Any(); 
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
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
//        /// brannigan is an AccountManager, but not for these orders but would have orders if flanders was away/null
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForAccountManager4()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("brannigan").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
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
//        /// brannigan is an AccountManager, but not for these orders But flanders is away.
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForAccountManager5()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("brannigan").Repeat.Any();
//            SetupUsers1(null, false);
//            var user = CreateValidEntities.User(8);
//            user.FirstName = "Ned";
//            user.LastName = "Flanders";
//            user.SetIdTo("flanders");
//            user.IsAway = true;
//            SetupUsers1(user);
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(4, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// brannigan is an AccountManager, but not for these orders But user is null.
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForAccountManager6()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("brannigan").Repeat.Any();
//            SetupUsers1(null, false);
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            var orderStatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
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
//            Assert.AreEqual(4, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// brannigan is an AccountManager, but for these orders as the secondary user.
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForAccountManager7()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("brannigan").Repeat.Any();
//            SetupUsers1(null, false);
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
//            // Remove brannigan from all approvals for wg 2 
//            var lastOrder = orders.Count();

//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            var orderStatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode == orderStatusCode)
//                {
//                    approval.User = UserRepository.GetNullableById("brannigan");
//                }
//            }

//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && (a.SecondaryUser != null && a.SecondaryUser.Id == "brannigan") || (a.User != null && a.User.Id == "brannigan")).ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.SecondaryUser = null;
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 0, null, null, 0, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(4, results.Count);
//            #endregion Assert
//        }
//        #endregion Account Manager Tests
//    }
//}
