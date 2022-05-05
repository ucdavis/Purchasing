using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;

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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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

        #region Edit Post Tests
        [TestMethod]
        public void TestEditPostRedirectsToIndexWhenAutoApprovalNotFound1()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, new AutoApproval(), true)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToIndexWhenAutoApprovalNotFound2()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, new AutoApproval(), false)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToIndexWhenAutoApprovalNotFound3()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, new AutoApproval())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToErrorWhenCurrentIdDifferent()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Edit(3, new AutoApproval())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("No Access", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            SetupData3();
            var autoApprovalToEdit = CreateValidEntities.AutoApproval(99);
            autoApprovalToEdit.Id = 99;
            autoApprovalToEdit.MaxAmount = (decimal) 12.44;
            autoApprovalToEdit.TargetUser = null;
            autoApprovalToEdit.Account = null;
            autoApprovalToEdit.User = null;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, autoApprovalToEdit, false)
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
            Assert.AreEqual((decimal)12.44, result.AutoApproval.MaxAmount);
            Assert.IsNotNull(result.AutoApproval.User);

            Assert.IsFalse(Controller.ViewBag.ShowAll);
            Assert.IsFalse(Controller.ViewBag.IsCreate);

            Controller.ModelState.AssertErrorsAre("An account OR user must be selected, not both.");
            Moq.Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()), Moq.Times.Never());
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            SetupData3();
            var autoApprovalToEdit = CreateValidEntities.AutoApproval(99);
            autoApprovalToEdit.Id = 99;
            autoApprovalToEdit.MaxAmount = (decimal)12.44;
            autoApprovalToEdit.TargetUser = null;
            autoApprovalToEdit.Account = null;
            autoApprovalToEdit.User = null;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, autoApprovalToEdit, true)
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
            Assert.AreEqual((decimal)12.44, result.AutoApproval.MaxAmount);
            Assert.IsNotNull(result.AutoApproval.User);

            Assert.IsTrue(Controller.ViewBag.ShowAll);
            Assert.IsFalse(Controller.ViewBag.IsCreate);

            Controller.ModelState.AssertErrorsAre("An account OR user must be selected, not both.");
            Moq.Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            SetupData3();
            var autoApprovalToEdit = CreateValidEntities.AutoApproval(99);
            autoApprovalToEdit.Id = 99;
            autoApprovalToEdit.MaxAmount = (decimal)12.44;
            autoApprovalToEdit.TargetUser = CreateValidEntities.User(88);
            autoApprovalToEdit.Account = CreateValidEntities.Account(87);
            autoApprovalToEdit.User = null;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, autoApprovalToEdit, true)
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
            Assert.AreEqual((decimal)12.44, result.AutoApproval.MaxAmount);
            Assert.IsNotNull(result.AutoApproval.User);

            Assert.IsTrue(Controller.ViewBag.ShowAll);
            Assert.IsFalse(Controller.ViewBag.IsCreate);

            Controller.ModelState.AssertErrorsAre("An account OR user must be selected, not both.");
            Moq.Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<AutoApproval>()), Moq.Times.Never());
            #endregion Assert
        }
        [TestMethod]
        public void TestEditPostSavesWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData2();
            SetupData3();
            var autoApprovalToEdit = CreateValidEntities.AutoApproval(99);
            autoApprovalToEdit.Id = 99;
            autoApprovalToEdit.MaxAmount = (decimal)12.44;
            autoApprovalToEdit.TargetUser = null;
            autoApprovalToEdit.Account = CreateValidEntities.Account(9);
            autoApprovalToEdit.User = null;
            var saveLessThan = AutoApprovalRepository.GetNullableById(3).LessThan;
            autoApprovalToEdit.LessThan = !saveLessThan;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, autoApprovalToEdit, true)
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
            Assert.AreEqual(3, args.Id);
            Assert.AreEqual((decimal)12.44, args.MaxAmount);
            Assert.IsNotNull(args.User);
            Assert.AreNotEqual(args.LessThan, saveLessThan);
            Assert.AreEqual("AutoApproval Edited Successfully", Controller.Message);
            #endregion Assert
        }
        #endregion Edit Post Tests
    }
}
