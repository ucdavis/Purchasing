//using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Purchasing.Core.Domain;
//using Rhino.Mocks;

//namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
//{

//    public partial class OrderAccessServiceTests
//    {
//        #region Requestor Tests
//        [TestMethod]
//        public void TestOrdersForRequestor2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("bender").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(5, results.Count);
//            foreach(var result in results)
//            {
//                Assert.AreEqual("bender", result.CreatedBy.Id);
//            }
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestOrdersForRequestor3()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("bender").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(5, results.Count);
//            foreach(var result in results)
//            {
//                Assert.AreEqual("bender", result.CreatedBy.Id);
//            }
//            #endregion Assert
//        }

//        [TestMethod]
//        public void TestOrdersForRequestor4()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("bender").Repeat.Any();
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
//            Assert.AreEqual(5, results.Count);
//            foreach(var result in results)
//            {
//                Assert.AreEqual("bender", result.CreatedBy.Id);
//            }
//            #endregion Assert
//        }


//        [TestMethod]
//        public void TestOrdersForRequestorWhenTheyAreCompleted1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("bender").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            orders[0].StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete);
//            orders[1].StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs);
//            orders[2].StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete);

//            SetupOrders(orders, approvals, 0, null, null, 0, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(2, results.Count);
//            foreach(var result in results)
//            {
//                Assert.AreEqual("bender", result.CreatedBy.Id);
//            }
//            #endregion Assert
//        }
//        #endregion Requestor Tests


//    }
//}
