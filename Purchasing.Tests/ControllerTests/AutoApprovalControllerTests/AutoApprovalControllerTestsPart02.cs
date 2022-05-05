using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Testing.Fakes;
using UCDArch.Testing.Extensions;
using Microsoft.AspNetCore.Mvc;

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

        #region Create Post Tests

        [TestMethod]
        public void TestCreatePostReturnsViewWhenInvalid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = null;
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal) 765.32;
            autoApprovalToCreate.LessThan = true;
            autoApprovalToCreate.Equal = true;
            
            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate, false)
                .AssertViewRendered()
                .WithViewData<AutoApprovalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("An account OR user must be selected, not both.");
            Assert.IsNotNull(result);
            Assert.AreEqual("Me", result.AutoApproval.User.Id);
            Assert.AreEqual((decimal)765.32, result.AutoApproval.MaxAmount);
            Assert.IsTrue(Controller.ViewBag.IsCreate);
            Assert.IsFalse(Controller.ViewBag.ShowAll);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostReturnsViewWhenInvalid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = null;
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal)765.32;
            autoApprovalToCreate.LessThan = true;
            autoApprovalToCreate.Equal = true;

            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate)
                .AssertViewRendered()
                .WithViewData<AutoApprovalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("An account OR user must be selected, not both.");
            Assert.IsNotNull(result);
            Assert.AreEqual("Me", result.AutoApproval.User.Id);
            Assert.AreEqual((decimal)765.32, result.AutoApproval.MaxAmount);
            Assert.IsTrue(Controller.ViewBag.IsCreate);
            Assert.IsFalse(Controller.ViewBag.ShowAll);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostReturnsViewWhenInvalid3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = null;
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal)765.32;
            autoApprovalToCreate.LessThan = true;
            autoApprovalToCreate.Equal = true;
            autoApprovalToCreate.IsActive = false;
            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate, true)
                .AssertViewRendered()
                .WithViewData<AutoApprovalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("An account OR user must be selected, not both.");
            Assert.IsNotNull(result);
            Assert.AreEqual("Me", result.AutoApproval.User.Id);
            Assert.AreEqual((decimal)765.32, result.AutoApproval.MaxAmount);
            Assert.IsTrue(result.AutoApproval.IsActive);            
            Assert.IsTrue(Controller.ViewBag.IsCreate);
            Assert.IsTrue(Controller.ViewBag.ShowAll);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostReturnsViewWhenInvalid4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = CreateValidEntities.User(66);
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal)765.32;
            autoApprovalToCreate.LessThan = true;
            autoApprovalToCreate.Equal = true;
            autoApprovalToCreate.Expiration = DateTime.UtcNow.ToPacificTime().Date;
            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate, true)
                .AssertViewRendered()
                .WithViewData<AutoApprovalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Expiration date has already passed");
            Assert.IsNotNull(result);
            Assert.AreEqual("Me", result.AutoApproval.User.Id);
            Assert.AreEqual((decimal)765.32, result.AutoApproval.MaxAmount);
            Assert.IsTrue(Controller.ViewBag.IsCreate);
            Assert.IsTrue(Controller.ViewBag.ShowAll);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = CreateValidEntities.User(66);
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal)765.32;
            autoApprovalToCreate.LessThan = true;
            autoApprovalToCreate.Equal = true;
            autoApprovalToCreate.Expiration = null;
            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate, true)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            Moq.Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()));
//TODO: Arrange
            AutoApproval args = default;
            Moq.Mock.Get( AutoApprovalRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()))
                .Callback<AutoApproval>(x => args = x);
//ENDTODO 
            Assert.IsNotNull(args);
            Assert.AreEqual("Me", args.User.Id);
            Assert.AreEqual((decimal)765.32, args.MaxAmount);
            Assert.IsTrue(args.LessThan);
            Assert.IsFalse(args.Equal);
            Assert.AreEqual("LastName66", args.TargetUser.LastName);
            Assert.IsNull(args.Account);
            Assert.IsNull(args.Expiration);
            Assert.AreEqual("AutoApproval Created Successfully", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = CreateValidEntities.User(66);
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal)765.32;
            autoApprovalToCreate.LessThan = true;
            autoApprovalToCreate.Equal = true;
            autoApprovalToCreate.Expiration = null;
            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate, false)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            Moq.Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()));
//TODO: Arrange
            AutoApproval args = default;
            Moq.Mock.Get(AutoApprovalRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()))
                .Callback<AutoApproval>(x => args = x);
//ENDTODO
            Assert.IsNotNull(args);
            Assert.AreEqual("Me", args.User.Id);
            Assert.AreEqual((decimal)765.32, args.MaxAmount);
            Assert.IsTrue(args.LessThan);
            Assert.IsFalse(args.Equal);
            Assert.AreEqual("LastName66", args.TargetUser.LastName);
            Assert.IsNull(args.Account);
            Assert.IsNull(args.Expiration);
            Assert.AreEqual("AutoApproval Created Successfully", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostRedirectsWhenValid3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = CreateValidEntities.User(66);
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal)765.32;
            autoApprovalToCreate.LessThan = false;
            autoApprovalToCreate.Equal = false;
            autoApprovalToCreate.Expiration = null;
            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate, true)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            Moq.Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()));
//TODO: Arrange
            AutoApproval args = default;
            Moq.Mock.Get(AutoApprovalRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()))
                .Callback<AutoApproval>(x => args = x);
//ENDTODO
            Assert.IsNotNull(args);
            Assert.AreEqual("Me", args.User.Id);
            Assert.AreEqual((decimal)765.32, args.MaxAmount);
            Assert.IsFalse(args.LessThan);
            Assert.IsTrue(args.Equal);
            Assert.AreEqual("LastName66", args.TargetUser.LastName);
            Assert.IsNull(args.Account);
            Assert.IsNull(args.Expiration);
            Assert.AreEqual("AutoApproval Created Successfully", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenValid4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = CreateValidEntities.User(66);
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal)765.32;
            autoApprovalToCreate.LessThan = false;
            autoApprovalToCreate.Equal = true;
            autoApprovalToCreate.Expiration = null;
            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate, true)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            Moq.Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()));
//TODO: Arrange
            AutoApproval args = default;
            Moq.Mock.Get(AutoApprovalRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()))
                .Callback<AutoApproval>(x => args = x);
//ENDTODO
            Assert.IsNotNull(args);
            Assert.AreEqual("Me", args.User.Id);
            Assert.AreEqual((decimal)765.32, args.MaxAmount);
            Assert.IsFalse(args.LessThan);
            Assert.IsTrue(args.Equal);
            Assert.AreEqual("LastName66", args.TargetUser.LastName);
            Assert.IsNull(args.Account);
            Assert.IsNull(args.Expiration);
            Assert.AreEqual("AutoApproval Created Successfully", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenValid5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = CreateValidEntities.User(66);
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal)765.32;
            autoApprovalToCreate.LessThan = false;
            autoApprovalToCreate.Equal = true;
            autoApprovalToCreate.Expiration = DateTime.UtcNow.ToPacificTime().Date.AddDays(6);
            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate, true)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            Moq.Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()));
//TODO: Arrange
            AutoApproval args = default;
            Moq.Mock.Get(AutoApprovalRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()))
                .Callback<AutoApproval>(x => args = x);
//ENDTODO
            Assert.IsNotNull(args);
            Assert.AreEqual("Me", args.User.Id);
            Assert.AreEqual((decimal)765.32, args.MaxAmount);
            Assert.IsFalse(args.LessThan);
            Assert.IsTrue(args.Equal);
            Assert.AreEqual("LastName66", args.TargetUser.LastName);
            Assert.IsNull(args.Account);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(6), args.Expiration);
            Assert.AreEqual("AutoApproval Created Successfully", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenValid6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = CreateValidEntities.User(66);
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal)765.32;
            autoApprovalToCreate.LessThan = false;
            autoApprovalToCreate.Equal = true;
            autoApprovalToCreate.Expiration = DateTime.UtcNow.ToPacificTime().Date.AddDays(5);
            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate, true)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            Moq.Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()));
//TODO: Arrange
            AutoApproval args = default;
            Moq.Mock.Get(AutoApprovalRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()))
                .Callback<AutoApproval>(x => args = x);
//ENDTODO
            Assert.IsNotNull(args);
            Assert.AreEqual("Me", args.User.Id);
            Assert.AreEqual((decimal)765.32, args.MaxAmount);
            Assert.IsFalse(args.LessThan);
            Assert.IsTrue(args.Equal);
            Assert.AreEqual("LastName66", args.TargetUser.LastName);
            Assert.IsNull(args.Account);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(5), args.Expiration);
            Assert.AreEqual("AutoApproval Created Successfully Warning, will expire in 5 days or less", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenValid7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            var autoApprovalToCreate = CreateValidEntities.AutoApproval(9);
            autoApprovalToCreate.User = null;
            autoApprovalToCreate.TargetUser = CreateValidEntities.User(66);
            autoApprovalToCreate.Account = null;
            autoApprovalToCreate.MaxAmount = (decimal)765.32;
            autoApprovalToCreate.LessThan = false;
            autoApprovalToCreate.Equal = true;
            autoApprovalToCreate.Expiration = DateTime.UtcNow.ToPacificTime().Date.AddDays(1);
            #endregion Arrange

            #region Act
            var result = Controller.Create(autoApprovalToCreate, true)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            Moq.Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()));
//TODO: Arrange
            AutoApproval args = default;
            Moq.Mock.Get(AutoApprovalRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()))
                .Callback<AutoApproval>(x => args = x);
//ENDTODO
            Assert.IsNotNull(args);
            Assert.AreEqual("Me", args.User.Id);
            Assert.AreEqual((decimal)765.32, args.MaxAmount);
            Assert.IsFalse(args.LessThan);
            Assert.IsTrue(args.Equal);
            Assert.AreEqual("LastName66", args.TargetUser.LastName);
            Assert.IsNull(args.Account);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(1), args.Expiration);
            Assert.AreEqual("AutoApproval Created Successfully Warning, will expire in 5 days or less", Controller.Message);
            #endregion Assert
        }
        #endregion Create Post Tests
    }
}
