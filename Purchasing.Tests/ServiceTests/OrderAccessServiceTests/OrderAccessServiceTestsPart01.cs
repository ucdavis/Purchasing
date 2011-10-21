using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            #endregion Assert
        }

        [TestMethod]
        public void TestUnCompletedOrdersForApprover()
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
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(2, results[0].Id);
            #endregion Assert
        }
    }
}
