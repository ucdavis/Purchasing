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
        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.Create(false)
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

            Assert.IsFalse(Controller.ViewBag.ShowAll);
            Assert.IsTrue(Controller.ViewBag.IsCreate);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.Create()
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

            Assert.IsFalse(Controller.ViewBag.ShowAll);
            Assert.IsTrue(Controller.ViewBag.IsCreate);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.Create(true)
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

            Assert.IsTrue(Controller.ViewBag.ShowAll);
            Assert.IsTrue(Controller.ViewBag.IsCreate);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NoOneMe");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.Create(false)
                .AssertViewRendered()
                .WithViewData<AutoApprovalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Users.Count);
            Assert.AreEqual(0, result.Accounts.Count);

            Assert.IsFalse(Controller.ViewBag.ShowAll);
            Assert.IsTrue(Controller.ViewBag.IsCreate);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetReturnsView5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.Create(false)
                .AssertViewRendered()
                .WithViewData<AutoApprovalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Users.Count);
            Assert.AreEqual(1, result.Accounts.Count);

            Assert.IsFalse(Controller.ViewBag.ShowAll);
            Assert.IsTrue(Controller.ViewBag.IsCreate);
            #endregion Assert
        }
        
        #endregion Create Get Tests
    }
}
