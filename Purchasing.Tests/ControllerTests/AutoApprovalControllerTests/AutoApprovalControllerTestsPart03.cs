using System;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Controllers;
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
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsToIndexWhenAutoApprovalNotFound1()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, true)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(true));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditGetRedirectsToIndexWhenAutoApprovalNotFound2()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, false)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToIndexWhenAutoApprovalNotFound3()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToErrorWhenCurrentIdDifferent()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "NotMe");
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Edit(3)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("No Access", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3)
                .AssertViewRendered()
                .WithViewData<AutoApprovalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Users.Count);
            Assert.AreEqual(7, result.Accounts.Count);
            Assert.AreEqual("Someone55", result.Users[0].Id);
            Assert.AreEqual("Someone2", result.Users[1].Id);
            Assert.AreEqual("Someone3", result.Users[2].Id);
            Assert.AreEqual("Someone4", result.Users[3].Id);
            Assert.AreEqual("AccountName2", result.Accounts[0].Name);
            Assert.AreEqual("AccountName3", result.Accounts[1].Name);
            Assert.AreEqual("AccountName5", result.Accounts[2].Name);
            Assert.AreEqual("AccountName6", result.Accounts[3].Name);
            Assert.AreEqual("AccountName7", result.Accounts[4].Name);
            Assert.AreEqual("AccountName8", result.Accounts[5].Name);
            Assert.AreEqual("AccountName9", result.Accounts[6].Name);
            Assert.AreEqual(3, result.AutoApproval.Id);

            Assert.IsFalse(Controller.ViewBag.ShowAll);
            Assert.IsFalse(Controller.ViewBag.IsCreate);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, true)
                .AssertViewRendered()
                .WithViewData<AutoApprovalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Users.Count);
            Assert.AreEqual(7, result.Accounts.Count);
            Assert.AreEqual("Someone55", result.Users[0].Id);
            Assert.AreEqual("Someone2", result.Users[1].Id);
            Assert.AreEqual("Someone3", result.Users[2].Id);
            Assert.AreEqual("Someone4", result.Users[3].Id);
            Assert.AreEqual("AccountName2", result.Accounts[0].Name);
            Assert.AreEqual("AccountName3", result.Accounts[1].Name);
            Assert.AreEqual("AccountName5", result.Accounts[2].Name);
            Assert.AreEqual("AccountName6", result.Accounts[3].Name);
            Assert.AreEqual("AccountName7", result.Accounts[4].Name);
            Assert.AreEqual("AccountName8", result.Accounts[5].Name);
            Assert.AreEqual("AccountName9", result.Accounts[6].Name);
            Assert.AreEqual(3, result.AutoApproval.Id);

            Assert.IsTrue(Controller.ViewBag.ShowAll);
            Assert.IsFalse(Controller.ViewBag.IsCreate);
            #endregion Assert
        }

        #endregion Edit Get Tests
    }
}
