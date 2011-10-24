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
        #region Purchaser Tests

        /// <summary>
        /// awong is a purchaser for wg1
        /// </summary>
        [TestMethod]
        public void TestOrdersForPurchaser1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong").Repeat.Any();
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
            Assert.AreEqual(6, results.Count);
            #endregion Assert
        }

        /// <summary>
        /// grimes is a purchaser for wg1, but not for these orders
        /// </summary>
        [TestMethod]
        public void TestOrdersForPurchaser2()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("grimes").Repeat.Any();
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
        #endregion Purchaser Tests
    }
}
