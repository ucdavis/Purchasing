using System;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Controllers.Dev;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;

namespace Purchasing.Tests.ControllerTests.AutoApprovalControllerTests
{
    public partial class AutoApprovalControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(true)
                .AssertViewRendered()
                .WithViewData<AutoApprovalListModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.AutoApprovals.Count());
            Assert.AreEqual(2, result.AutoApprovals[0].Id);
            Assert.IsTrue(result.ShowAll);
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(true)
                .AssertViewRendered()
                .WithViewData<AutoApprovalListModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, result.AutoApprovals.Count());
            Assert.AreEqual(1, result.AutoApprovals[0].Id);
            Assert.IsTrue(result.ShowAll);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(true)
                .AssertViewRendered()
                .WithViewData<AutoApprovalListModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, result.AutoApprovals.Count());
            Assert.AreEqual(1, result.AutoApprovals[0].Id);
            Assert.IsTrue(result.ShowAll);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NoOne");
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(true)
                .AssertViewRendered()
                .WithViewData<AutoApprovalListModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.AutoApprovals.Count());
            Assert.IsTrue(result.ShowAll);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(false)
                .AssertViewRendered()
                .WithViewData<AutoApprovalListModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.AutoApprovals.Count());
            Assert.AreEqual(3, result.AutoApprovals[0].Id);
            Assert.AreEqual(4, result.AutoApprovals[1].Id);
            Assert.AreEqual(6, result.AutoApprovals[2].Id);
            Assert.IsFalse(result.ShowAll);
            #endregion Assert
        }
        #endregion Index Tests

        #region DetailsTests


        [TestMethod]
        public void TestDetails()
        {
            #region Arrange
            Assert.Inconclusive("write tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        } 
        #endregion DetailsTests

    }
}
