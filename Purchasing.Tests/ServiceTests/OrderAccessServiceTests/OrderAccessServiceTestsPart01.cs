using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Rhino.Mocks;

namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
{

    public partial class OrderAccessServiceTests
    {
        [TestMethod]
        public void TestUnCompletedOrdersForRequestor1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            SetupOrders1("bender");
            SetupApprovals1();

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Count);
            foreach (var result in results)
            {
                Assert.AreEqual("bender", result.CreatedBy.Id);
            }
            #endregion Assert
        }

        /// <summary>
        /// This one is the same as above but it uses the new way to fake the data
        /// </summary>
        [TestMethod]
        public void TestUnCompletedOrdersForRequestor2()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Count);
            foreach(var result in results)
            {
                Assert.AreEqual("bender", result.CreatedBy.Id);
            }
            #endregion Assert
        }

        [TestMethod]
        public void TestUnCompletedOrdersForApprover1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            SetupOrders1("bender");
            SetupApprovals1();

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("bender", results[0].CreatedBy.Id);
            Assert.AreEqual("moe", results[5].CreatedBy.Id);
            #endregion Assert
        }

        /// <summary>
        /// Same as the one above but new way of setting data
        /// </summary>
        [TestMethod]
        public void TestUnCompletedOrdersForApprover2()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("bender", results[0].CreatedBy.Id);
            Assert.AreEqual("moe", results[5].CreatedBy.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestUnCompletedOrdersForApprover3()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestUnCompletedOrdersForApprover4()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestUnCompletedOrdersForApprover5()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);
            foreach (var result in results)
            {
                Assert.AreEqual("moe", result.CreatedBy.Id);
            }
            #endregion Assert
        }

        [TestMethod]
        public void TestUnCompletedOrdersForApprover6()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
            #endregion Assert
        }


        [TestMethod]
        public void TestUnCompletedOrdersForApprover7()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("flanders").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(4, results.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestUnCompletedOrdersForApprover8()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("brannigan").Repeat.Any(); //brannigan is an AccountManager, but not for these orders and wg
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
            #endregion Assert
        }


        [TestMethod]
        public void TestUnCompletedOrdersForApprover9()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("brannigan").Repeat.Any(); //brannigan is an AccountManager, but not for these orders but would have orders if flanders was away/null?
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
            #endregion Assert
        }

        /// <summary>
        /// Test when user is null in approval
        /// Test when user is away
        /// tests for other parameters
        /// </summary>
        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("More tests");
            
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
    }
}
