using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Testing.Fakes;

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
        public void TestDetailsRedirectsToIndexWhenAutoApprovalNotFound1()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(8, false)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDetailsRedirectsToIndexWhenAutoApprovalNotFound2()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(8)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsRedirectsToIndexWhenAutoApprovalNotFound3()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(8, true)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(true));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            #endregion Assert
        }


        [TestMethod]
        public void TestDetailsRedirectsToErrorIndexWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Details(3, false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("No Access", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDetailsRedirectsToErrorIndexWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Details(3, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("No Access", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestDetailsReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(3, false)
                .AssertViewRendered()
                .WithViewData<AutoApproval>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(Controller.ViewBag.ShowAll);
            Assert.AreEqual(3, result.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(3, true)
                .AssertViewRendered()
                .WithViewData<AutoApproval>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(Controller.ViewBag.ShowAll);
            Assert.AreEqual(3, result.Id);
            #endregion Assert
        }

        #endregion DetailsTests

    }
}
