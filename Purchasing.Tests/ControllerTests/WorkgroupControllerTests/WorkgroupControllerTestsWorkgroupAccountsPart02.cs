using System.Collections.Generic;
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
        #region AccountDetails Tests

        [TestMethod]
        public void TestAccountDetailsRedirectsIfNotFound()
        {
            #region Arrange
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            #endregion Arrange

            #region Act
            Controller.AccountDetails(0,4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Account could not be found", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestAccountDetailsReturnsView()
        {
            #region Arrange
            var accounts = new List<WorkgroupAccount>();
            for (var i = 0; i < 3; i++)
            {
                accounts.Add(CreateValidEntities.WorkgroupAccount(i+1));
            }
            accounts[2].Account.SetIdTo("blah");
            accounts[2].AccountManager.SetIdTo("myAccMan");
            accounts[2].Approver.SetIdTo("myApp");
            accounts[2].Purchaser.SetIdTo("myPur");
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, accounts);
            #endregion Arrange

            #region Act
            var result = Controller.AccountDetails(0,3)
                .AssertViewRendered()
                .WithViewData<WorkgroupAccount>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("blah", result.Account.Id);
            Assert.AreEqual("myAccMan", result.AccountManager.Id);
            Assert.AreEqual("myApp", result.Approver.Id);
            Assert.AreEqual("myPur", result.Purchaser.Id);
            #endregion Assert		
        }
        #endregion AccountDetails Tests

        #region EditAccount Get Tests

        [TestMethod]
        public void TestEditAccountGetRedirectsIfAccountNotFound()
        {
            #region Arrange
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            #endregion Arrange

            #region Act
            Controller.EditAccount(0,4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Account could not be found", Controller.ErrorMessage);
            #endregion Assert			
        }


        [TestMethod]
        public void TestEditAccountGetReturnsView()
        {
            #region Arrange
            SetupDataForAccounts1(true);
            var accounts = new List<WorkgroupAccount>();
            for(var i = 0; i < 3; i++)
            {
                accounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                accounts[i].Workgroup = WorkgroupRepository.GetNullableById(i+1);
            }
            accounts[2].Account.SetIdTo("blah");
            accounts[2].AccountManager.SetIdTo("myAccMan");
            accounts[2].Approver.SetIdTo("myApp");
            accounts[2].Purchaser.SetIdTo("myPur");
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository, accounts);
            #endregion Arrange

            #region Act
            var result = Controller.EditAccount(3, 3)
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
            Assert.AreEqual(3, result.WorkgroupAccount.Id);
            Assert.AreEqual("blah", result.WorkgroupAccount.Account.Id);
            Assert.AreEqual("myAccMan", result.WorkgroupAccount.AccountManager.Id);
            Assert.AreEqual("myApp", result.WorkgroupAccount.Approver.Id);
            Assert.AreEqual("myPur", result.WorkgroupAccount.Purchaser.Id);
            #endregion Assert		
        }
        #endregion EditAccount Get Tests

        #region EditAccount Post Tests
        [TestMethod]
        public void TestEditAccountPostRedirectsIfAccountNotFound()
        {
            #region Arrange
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            #endregion Arrange

            #region Act
            Controller.EditAccount(0, 4, new WorkgroupAccount())
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Account could not be found", Controller.ErrorMessage);
            WorkgroupAccountRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            #endregion Assert
        }



        [TestMethod]
        public void TestEditAccountPostRedirectsWhenValid()
        {
            #region Arrange
            var accounts = new List<WorkgroupAccount>();
            for(var i = 0; i < 3; i++)
            {
                accounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
            }
            accounts[2].Workgroup = CreateValidEntities.Workgroup(9);
            accounts[2].Workgroup.SetIdTo(9);
            accounts[2].Account.SetIdTo("blah");
            accounts[2].AccountManager.SetIdTo("myAccMan");
            accounts[2].Approver.SetIdTo("myApp");
            accounts[2].Purchaser.SetIdTo("myPur");
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, accounts);

            var accountToEdit = CreateValidEntities.WorkgroupAccount(8);
            accountToEdit.SetIdTo(8);
            accountToEdit.Workgroup = CreateValidEntities.Workgroup(7);
            accountToEdit.Workgroup.SetIdTo(7);
            accountToEdit.Account.SetIdTo("Eblah");
            accountToEdit.AccountManager.SetIdTo("EmyAccMan");
            accountToEdit.Approver.SetIdTo("EmyApp");
            accountToEdit.Purchaser.SetIdTo("EmyPur");
            #endregion Arrange

            #region Act
            var result = Controller.EditAccount(9, 3, accountToEdit)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Accounts(9));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.RouteValues["id"]);
            Assert.AreEqual("Workgroup account has been updated.", Controller.Message);

            WorkgroupAccountRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            var args = (WorkgroupAccount) WorkgroupAccountRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(9, args.Workgroup.Id); //Did not change
            Assert.AreEqual(3, args.Id); //Did not change
            Assert.AreEqual("Eblah", args.Account.Id);
            Assert.AreEqual("EmyAccMan", args.AccountManager.Id);
            Assert.AreEqual("EmyApp", args.Approver.Id);
            Assert.AreEqual("EmyPur", args.Purchaser.Id);
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditAccountPostWhenNotValidReturnsView()
        {
            #region Arrange
            SetupDataForAccounts1(true);
            var accounts = new List<WorkgroupAccount>();
            for(var i = 0; i < 3; i++)
            {
                accounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
            }
            accounts[2].Workgroup = WorkgroupRepository.GetNullableById(3);
            accounts[2].Account.SetIdTo("blah");
            accounts[2].AccountManager.SetIdTo("myAccMan");
            accounts[2].Approver.SetIdTo("myApp");
            accounts[2].Purchaser.SetIdTo("myPur");
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, accounts);

            var accountToEdit = CreateValidEntities.WorkgroupAccount(8);
            accountToEdit.SetIdTo(8);
            accountToEdit.Workgroup = CreateValidEntities.Workgroup(7);
            accountToEdit.Workgroup.SetIdTo(7);
            //accountToEdit.Account.SetIdTo("Eblah");
            accountToEdit.Account = null;
            //accountToEdit.AccountManager.SetIdTo("EmyAccMan");
            accountToEdit.AccountManager = null;
            accountToEdit.Approver.SetIdTo("EmyApp");
            accountToEdit.Purchaser.SetIdTo("EmyPur");
            #endregion Arrange

            #region Act
            var result = Controller.EditAccount(3, 3, accountToEdit)
                .AssertViewRendered()
                .WithViewData<WorkgroupAccountModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("The Account field is required.");
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Accounts.Count());
            Assert.AreEqual(12, result.WorkGroupPermissions.Count());
            Assert.AreEqual(1, result.Approvers.Count());
            Assert.AreEqual(1, result.AccountManagers.Count());
            Assert.AreEqual(1, result.Purchasers.Count());

            Assert.IsNull(result.WorkgroupAccount.Account);
            Assert.IsNull(result.WorkgroupAccount.AccountManager);
            #endregion Assert		
        }
        #endregion EditAccount Post Tests

        #region AccountDelete Get Tests
        [TestMethod]
        public void TestAccountDeleteGetRedirectsIfAccountNotFound()
        {
            #region Arrange
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            #endregion Arrange

            #region Act
            Controller.AccountDelete(0, 4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Account could not be found", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountDeleteGetRedirectsIfWorkgroupIdNoMatch()
        {
            #region Arrange
            SetupDataForAccounts1(true);
            var accounts = new List<WorkgroupAccount>();
            for(var i = 0; i < 3; i++)
            {
                accounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                accounts[i].Workgroup = WorkgroupRepository.GetNullableById(i + 1);
            }
            accounts[2].Account.SetIdTo("blah");
            accounts[2].AccountManager.SetIdTo("myAccMan");
            accounts[2].Approver.SetIdTo("myApp");
            accounts[2].Purchaser.SetIdTo("myPur");
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, accounts);
            #endregion Arrange

            #region Act
            Controller.AccountDelete(0, 3)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Account not part of workgroup", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestAccountDeleteGetReturnsView()
        {
            #region Arrange
            SetupDataForAccounts1(true);
            var accounts = new List<WorkgroupAccount>();
            for(var i = 0; i < 3; i++)
            {
                accounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                accounts[i].Workgroup = WorkgroupRepository.GetNullableById(i + 1);
            }
            accounts[2].Account.SetIdTo("blah");
            accounts[2].AccountManager.SetIdTo("myAccMan");
            accounts[2].Approver.SetIdTo("myApp");
            accounts[2].Purchaser.SetIdTo("myPur");
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, accounts);
            #endregion Arrange

            #region Act
            var result = Controller.AccountDelete(3, 3)
                .AssertViewRendered()
                .WithViewData<WorkgroupAccount>();
            #endregion Act

            #region Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Id);
            #endregion Assert
        }

        #endregion AccountDelete Get Tests

        #region AccountDelete Post Tests
        [TestMethod]
        public void TestAccountDeletePostRedirectsIfAccountNotFound()
        {
            #region Arrange
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            #endregion Arrange

            #region Act
            Controller.AccountDelete(0, 4, new WorkgroupAccount())
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Account could not be found", Controller.ErrorMessage);
            WorkgroupAccountRepository.AssertWasNotCalled(a => a.Remove(Arg<WorkgroupAccount>.Is.Anything));
            WorkgroupAccountRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountDeletePostRedirectsIfWorkgroupIdNoMatch()
        {
            #region Arrange
            SetupDataForAccounts1(true);
            var accounts = new List<WorkgroupAccount>();
            for(var i = 0; i < 3; i++)
            {
                accounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                accounts[i].Workgroup = WorkgroupRepository.GetNullableById(1);
            }
            accounts[2].Account.SetIdTo("blah");
            accounts[2].AccountManager.SetIdTo("myAccMan");
            accounts[2].Approver.SetIdTo("myApp");
            accounts[2].Purchaser.SetIdTo("myPur");
            accounts[1].Workgroup = WorkgroupRepository.GetNullableById(3);
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, accounts);
            #endregion Arrange

            #region Act
            Controller.AccountDelete(0, 2, new WorkgroupAccount())
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Account not part of workgroup", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestAccountDeletePostReturnsView()
        {
            #region Arrange
            SetupDataForAccounts1(true);
            var accounts = new List<WorkgroupAccount>();
            for(var i = 0; i < 3; i++)
            {
                accounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                accounts[i].Workgroup = WorkgroupRepository.GetNullableById(1);
            }
            accounts[2].Account.SetIdTo("blah");
            accounts[2].AccountManager.SetIdTo("myAccMan");
            accounts[2].Approver.SetIdTo("myApp");
            accounts[2].Purchaser.SetIdTo("myPur");
            accounts[1].Workgroup = WorkgroupRepository.GetNullableById(3); 
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, accounts);
            #endregion Arrange

            #region Act
            var result = Controller.AccountDelete(3, 2, new WorkgroupAccount())
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Accounts(3));
            #endregion Act

            #region Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);

            WorkgroupAccountRepository.AssertWasCalled(a => a.Remove(Arg<WorkgroupAccount>.Is.Anything));
            var workgroupAccountsargs = (WorkgroupAccount) WorkgroupAccountRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<WorkgroupAccount>.Is.Anything))[0][0]; 
            Assert.AreEqual(2, workgroupAccountsargs.Id);
            #endregion Assert
        }

        #endregion AccountDelete Post Tests
    }
}
