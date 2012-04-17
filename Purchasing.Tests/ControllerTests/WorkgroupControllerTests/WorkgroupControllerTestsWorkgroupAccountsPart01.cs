using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Rhino.Mocks;
using UCDArch.Testing;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region Accounts Tests
        
        [TestMethod]
        public void TestAccountsRedirectsIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.Accounts(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestAccountsReturnsView()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Accounts(3)
                .AssertViewRendered()
                .WithViewData<Workgroup>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Name);
            #endregion Assert		
        }

        #endregion Accounts Tests

        #region AddAccount Get Tests

        [TestMethod]
        public void TestAddAccountGetRegirectsIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.AddAccount(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestAddAccountGetReturnsView1()
        {
            #region Arrange
            SetupDataForAccounts1();
            #endregion Arrange

            #region Act
            var result = Controller.AddAccount(3)
                .AssertViewRendered()
                .WithViewData<WorkgroupAccountModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Accounts.Count());
            Assert.AreEqual(12, result.WorkGroupPermissions.Count());
            Assert.AreEqual(1, result.Approvers.Count());
            Assert.AreEqual(1, result.AccountManagers.Count());
            Assert.AreEqual(1, result.Purchasers.Count());
            Assert.IsNotNull(result.WorkgroupAccount);
            Assert.AreEqual(0, result.WorkgroupAccount.Id);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddAccountGetReturnsView2()
        {
            #region Arrange
            SetupDataForAccounts1();
            #endregion Arrange

            #region Act
            var result = Controller.AddAccount(2)
                .AssertViewRendered()
                .WithViewData<WorkgroupAccountModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Accounts.Count());
            Assert.AreEqual(3, result.WorkGroupPermissions.Count());
            Assert.AreEqual(1, result.Approvers.Count());
            Assert.AreEqual(1, result.AccountManagers.Count());
            Assert.AreEqual(1, result.Purchasers.Count());
            Assert.IsNotNull(result.WorkgroupAccount);
            Assert.AreEqual(0, result.WorkgroupAccount.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddAccountGetReturnsView3()
        {
            #region Arrange
            SetupDataForAccounts1();
            #endregion Arrange

            #region Act
            var result = Controller.AddAccount(1)
                .AssertViewRendered()
                .WithViewData<WorkgroupAccountModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Accounts.Count());
            Assert.AreEqual(3, result.WorkGroupPermissions.Count());
            Assert.AreEqual(0, result.Approvers.Count());
            Assert.AreEqual(0, result.AccountManagers.Count());
            Assert.AreEqual(0, result.Purchasers.Count());
            Assert.IsNotNull(result.WorkgroupAccount);
            Assert.AreEqual(0, result.WorkgroupAccount.Id);
            #endregion Assert
        }

        #endregion AddAccount Get Tests

        #region AddAccount Post Tests

        [TestMethod]
        public void TestAddAccountPostRedirectsIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.AddAccount(4, new WorkgroupAccount(), null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestAddAccountPostRedirectsToAccountsWhenValid()
        {
            #region Arrange
            SetupDataForAccounts1();
            var workgroupAccountToCreate = CreateValidEntities.WorkgroupAccount(9);
            workgroupAccountToCreate.Workgroup = WorkgroupRepository.GetNullableById(2); //This one will be replaced
            workgroupAccountToCreate.Account.SetIdTo("Blah");
            workgroupAccountToCreate.AccountManager.SetIdTo("AccMan");
            workgroupAccountToCreate.Approver.SetIdTo("App");
            workgroupAccountToCreate.Purchaser.SetIdTo("Purchase");
            #endregion Arrange

            #region Act
            var result = Controller.AddAccount(3, workgroupAccountToCreate, null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Accounts(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("Workgroup account saved.", Controller.Message);

            WorkgroupAccountRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            var args = (WorkgroupAccount) WorkgroupAccountRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Workgroup.Id);
            Assert.AreEqual("Blah", args.Account.Id);
            Assert.AreEqual("AccMan", args.AccountManager.Id);
            Assert.AreEqual("App", args.Approver.Id);
            Assert.AreEqual("Purchase", args.Purchaser.Id);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddAccountPostReturnsViewWhenInvalid()
        {
            #region Arrange
            SetupDataForAccounts1();
            var workgroupAccountToCreate = CreateValidEntities.WorkgroupAccount(9);
            workgroupAccountToCreate.Workgroup = WorkgroupRepository.GetNullableById(2); //This one will be replaced
            //workgroupAccountToCreate.Account.SetIdTo("Blah");
            workgroupAccountToCreate.Account = null;
            workgroupAccountToCreate.AccountManager.SetIdTo("AccMan");
            workgroupAccountToCreate.Approver.SetIdTo("App");
            workgroupAccountToCreate.Purchaser.SetIdTo("Purchase");
            #endregion Arrange

            #region Act
            var result = Controller.AddAccount(3, workgroupAccountToCreate, null)
                .AssertViewRendered()
                .WithViewData<WorkgroupAccountModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("The Account field is required.");
            WorkgroupAccountRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Accounts.Count());
            Assert.AreEqual(12, result.WorkGroupPermissions.Count());
            Assert.AreEqual(1, result.Approvers.Count());
            Assert.AreEqual(1, result.AccountManagers.Count());
            Assert.AreEqual(1, result.Purchasers.Count());
            Assert.IsNotNull(result.WorkgroupAccount);
            Assert.AreEqual(0, result.WorkgroupAccount.Id);

            Assert.AreEqual(3, result.WorkgroupAccount.Workgroup.Id);
            Assert.AreEqual(null, result.WorkgroupAccount.Account);
            Assert.AreEqual("AccMan", result.WorkgroupAccount.AccountManager.Id);
            Assert.AreEqual("App", result.WorkgroupAccount.Approver.Id);
            Assert.AreEqual("Purchase", result.WorkgroupAccount.Purchaser.Id);
            #endregion Assert
        }
        #endregion AddAccount Post Tests
    }
}
