﻿using System;
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
        #region Requestor Tests
        [TestMethod]
        public void TestOrdersForRequestor1()
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
            foreach(var result in results)
            {
                Assert.AreEqual("bender", result.CreatedBy.Id);
            }
            #endregion Assert
        }

        /// <summary>
        /// This one is the same as above but it uses the new way to fake the data
        /// </summary>
        [TestMethod]
        public void TestOrdersForRequestor2()
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
        
        #endregion Requestor Tests


    }
}
