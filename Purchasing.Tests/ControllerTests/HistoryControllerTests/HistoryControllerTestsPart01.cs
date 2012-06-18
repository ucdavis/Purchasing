using System;
using System.Linq;
using Castle.Windsor;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Purchasing.Tests.ControllerTests.HistoryControllerTests
{
    public partial class HistoryControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsAll()
        {
            #region Arrange
            new FakeOrderHistory(3, OrderHistoryRepository);

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion Index Tests


        #region Method Tests

        [TestMethod]
        public void TestWriteMethodTests()
        {
            #region Arrange
            Assert.Inconclusive("Need to write these tests");
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert
        }
        #endregion Method Tests
    }
}
