using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.ActionResults;
using Microsoft.AspNetCore.Mvc;

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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
            Assert.AreEqual("blah", args.Account.Id);
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
            accounts[2].Account = null;
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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

        #region UpdateMultipleAccounts Get Tests

        [TestMethod]
        public void TestUpdateMultipleAccountsGetRedirectsToIndexWhenWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.UpdateMultipleAccounts(4)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestUpdateMultipleAccountsReturnsViewWithExpectedValues1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeWorkgroupPermissions(3, WorkgroupPermissionRepository);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3)
                .AssertViewRendered()
                .WithViewData<UpdateMultipleAccountsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.ApproverChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.ApproverChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.ApproverChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.ApproverChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.ApproverChoices[1].Item2);

            Assert.AreEqual(2, result.AccountManagerChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.AccountManagerChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.AccountManagerChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.AccountManagerChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.AccountManagerChoices[1].Item2);

            Assert.AreEqual(2, result.PurchaserChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.PurchaserChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.PurchaserChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.PurchaserChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.PurchaserChoices[1].Item2);

            Assert.IsNull(result.SelectedApprover);
            Assert.IsNull(result.SelectedAccountManager);
            Assert.IsNull(result.SelectedPurchaser);

            Assert.IsFalse(result.DefaultSelectedApprover);
            Assert.IsFalse(result.DefaultSelectedAccountManager);
            Assert.IsFalse(result.DefaultSelectedPurchaser);
            #endregion Assert		
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsReturnsViewWithExpectedValues2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3)
                .AssertViewRendered()
                .WithViewData<UpdateMultipleAccountsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.ApproverChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.ApproverChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.ApproverChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.ApproverChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.ApproverChoices[1].Item2);

            Assert.AreEqual("3", result.ApproverChoices[2].Item1);
            Assert.AreEqual("LastName3, FirstName3 (3)", result.ApproverChoices[2].Item2);

            Assert.AreEqual(2, result.AccountManagerChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.AccountManagerChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.AccountManagerChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.AccountManagerChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.AccountManagerChoices[1].Item2);

            Assert.AreEqual(2, result.PurchaserChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.PurchaserChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.PurchaserChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.PurchaserChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.PurchaserChoices[1].Item2);

            Assert.IsNull(result.SelectedApprover);
            Assert.IsNull(result.SelectedAccountManager);
            Assert.IsNull(result.SelectedPurchaser);

            Assert.IsFalse(result.DefaultSelectedApprover);
            Assert.IsFalse(result.DefaultSelectedAccountManager);
            Assert.IsFalse(result.DefaultSelectedPurchaser);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsReturnsViewWithExpectedValues3()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[0].IsDefaultForAccount = true;
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3)
                .AssertViewRendered()
                .WithViewData<UpdateMultipleAccountsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.ApproverChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.ApproverChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.ApproverChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.ApproverChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.ApproverChoices[1].Item2);

            Assert.AreEqual("3", result.ApproverChoices[2].Item1);
            Assert.AreEqual("LastName3, FirstName3 (3)", result.ApproverChoices[2].Item2);

            Assert.AreEqual(2, result.AccountManagerChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.AccountManagerChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.AccountManagerChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.AccountManagerChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.AccountManagerChoices[1].Item2);

            Assert.AreEqual(2, result.PurchaserChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.PurchaserChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.PurchaserChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.PurchaserChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.PurchaserChoices[1].Item2);

            Assert.AreEqual("3", result.SelectedApprover);
            Assert.IsNull(result.SelectedAccountManager);
            Assert.IsNull(result.SelectedPurchaser);

            Assert.IsTrue(result.DefaultSelectedApprover);
            Assert.IsFalse(result.DefaultSelectedAccountManager);
            Assert.IsFalse(result.DefaultSelectedPurchaser);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsReturnsViewWithExpectedValues4()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[0].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
            workgroupPermissions[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
            workgroupPermissions[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[2].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(4));
            workgroupPermissions[3].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[3].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(5));
            workgroupPermissions[4].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[4].IsDefaultForAccount = true;

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3)
                .AssertViewRendered()
                .WithViewData<UpdateMultipleAccountsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.ApproverChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.ApproverChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.ApproverChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.ApproverChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.ApproverChoices[1].Item2);

            Assert.AreEqual("3", result.ApproverChoices[2].Item1);
            Assert.AreEqual("LastName3, FirstName3 (3)", result.ApproverChoices[2].Item2);

            Assert.AreEqual(4, result.AccountManagerChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.AccountManagerChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.AccountManagerChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.AccountManagerChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.AccountManagerChoices[1].Item2);

            Assert.AreEqual("1", result.AccountManagerChoices[2].Item1);
            Assert.AreEqual("LastName1, FirstName1 (1)", result.AccountManagerChoices[2].Item2);
            Assert.AreEqual("4", result.AccountManagerChoices[3].Item1);
            Assert.AreEqual("LastName4, FirstName4 (4)", result.AccountManagerChoices[3].Item2);

            Assert.AreEqual(4, result.PurchaserChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.PurchaserChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.PurchaserChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.PurchaserChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.PurchaserChoices[1].Item2);

            Assert.AreEqual("2", result.PurchaserChoices[2].Item1);
            Assert.AreEqual("LastName2, FirstName2 (2)", result.PurchaserChoices[2].Item2);
            Assert.AreEqual("4", result.PurchaserChoices[3].Item1);
            Assert.AreEqual("LastName4, FirstName4 (4)", result.PurchaserChoices[3].Item2);

            Assert.AreEqual("3", result.SelectedApprover);
            Assert.AreEqual("1", result.SelectedAccountManager);
            Assert.AreEqual("4", result.SelectedPurchaser);

            Assert.IsTrue(result.DefaultSelectedApprover);
            Assert.IsTrue(result.DefaultSelectedAccountManager);
            Assert.IsTrue(result.DefaultSelectedPurchaser);
            #endregion Assert
        }
        #endregion UpdateMultipleAccounts Get Tests

        #region UpdateMultipleAccounts Post Tests
        [TestMethod]
        public void TestUpdateMultipleAccountsPostRedirectsToIndexWhenWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.UpdateMultipleAccounts(4, new UpdateMultipleAccountsViewModel())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsPostReturnsViewWithExpectedValuesWhenInvalid1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeWorkgroupPermissions(3, WorkgroupPermissionRepository);
            var updateMultipleAccountsViewModel = new UpdateMultipleAccountsViewModel();
            updateMultipleAccountsViewModel.DefaultSelectedApprover = true;
            updateMultipleAccountsViewModel.SelectedApprover = "DO_NOT_UPDATE";
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3, updateMultipleAccountsViewModel)
                .AssertViewRendered()
                .WithViewData<UpdateMultipleAccountsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.ApproverChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.ApproverChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.ApproverChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.ApproverChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.ApproverChoices[1].Item2);

            Assert.AreEqual(2, result.AccountManagerChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.AccountManagerChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.AccountManagerChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.AccountManagerChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.AccountManagerChoices[1].Item2);

            Assert.AreEqual(2, result.PurchaserChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.PurchaserChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.PurchaserChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.PurchaserChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.PurchaserChoices[1].Item2);

            Assert.AreEqual("DO_NOT_UPDATE", result.SelectedApprover);
            Assert.IsNull(result.SelectedAccountManager);
            Assert.IsNull(result.SelectedPurchaser);

            Assert.IsTrue(result.DefaultSelectedApprover);
            Assert.IsFalse(result.DefaultSelectedAccountManager);
            Assert.IsFalse(result.DefaultSelectedPurchaser);

            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All");
            WorkgroupService.AssertWasNotCalled(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsPostReturnsViewWithExpectedValuesWhenInvalid2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[0].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
            workgroupPermissions[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
            workgroupPermissions[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[2].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(4));
            workgroupPermissions[3].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[3].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(5));
            workgroupPermissions[4].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[4].IsDefaultForAccount = true;

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

            var updateMultipleAccountsViewModel = new UpdateMultipleAccountsViewModel();
            updateMultipleAccountsViewModel.DefaultSelectedApprover = true;
            updateMultipleAccountsViewModel.DefaultSelectedAccountManager = true;
            updateMultipleAccountsViewModel.DefaultSelectedPurchaser = true;
            updateMultipleAccountsViewModel.SelectedApprover = "CLEAR_ALL";
            updateMultipleAccountsViewModel.SelectedAccountManager = "1";
            updateMultipleAccountsViewModel.SelectedPurchaser = "4";
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3, updateMultipleAccountsViewModel)
                .AssertViewRendered()
                .WithViewData<UpdateMultipleAccountsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.ApproverChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.ApproverChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.ApproverChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.ApproverChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.ApproverChoices[1].Item2);

            Assert.AreEqual("3", result.ApproverChoices[2].Item1);
            Assert.AreEqual("LastName3, FirstName3 (3)", result.ApproverChoices[2].Item2);

            Assert.AreEqual(4, result.AccountManagerChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.AccountManagerChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.AccountManagerChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.AccountManagerChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.AccountManagerChoices[1].Item2);

            Assert.AreEqual("1", result.AccountManagerChoices[2].Item1);
            Assert.AreEqual("LastName1, FirstName1 (1)", result.AccountManagerChoices[2].Item2);
            Assert.AreEqual("4", result.AccountManagerChoices[3].Item1);
            Assert.AreEqual("LastName4, FirstName4 (4)", result.AccountManagerChoices[3].Item2);

            Assert.AreEqual(4, result.PurchaserChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.PurchaserChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.PurchaserChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.PurchaserChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.PurchaserChoices[1].Item2);

            Assert.AreEqual("2", result.PurchaserChoices[2].Item1);
            Assert.AreEqual("LastName2, FirstName2 (2)", result.PurchaserChoices[2].Item2);
            Assert.AreEqual("4", result.PurchaserChoices[3].Item1);
            Assert.AreEqual("LastName4, FirstName4 (4)", result.PurchaserChoices[3].Item2);

            Assert.AreEqual("CLEAR_ALL", result.SelectedApprover);
            Assert.AreEqual("1", result.SelectedAccountManager);
            Assert.AreEqual("4", result.SelectedPurchaser);

            Assert.IsTrue(result.DefaultSelectedApprover);
            Assert.IsTrue(result.DefaultSelectedAccountManager);
            Assert.IsTrue(result.DefaultSelectedPurchaser);

            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All");
            WorkgroupService.AssertWasNotCalled(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsPostReturnsViewWithExpectedValuesWhenInvalid3()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[0].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
            workgroupPermissions[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
            workgroupPermissions[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[2].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(4));
            workgroupPermissions[3].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[3].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(5));
            workgroupPermissions[4].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[4].IsDefaultForAccount = true;

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

            var updateMultipleAccountsViewModel = new UpdateMultipleAccountsViewModel();
            updateMultipleAccountsViewModel.DefaultSelectedApprover = true;
            updateMultipleAccountsViewModel.DefaultSelectedAccountManager = true;
            updateMultipleAccountsViewModel.DefaultSelectedPurchaser = true;
            updateMultipleAccountsViewModel.SelectedApprover = "CLEAR_ALL";
            updateMultipleAccountsViewModel.SelectedAccountManager = "CLEAR_ALL";
            updateMultipleAccountsViewModel.SelectedPurchaser = "CLEAR_ALL";
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3, updateMultipleAccountsViewModel)
                .AssertViewRendered()
                .WithViewData<UpdateMultipleAccountsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.ApproverChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.ApproverChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.ApproverChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.ApproverChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.ApproverChoices[1].Item2);

            Assert.AreEqual("3", result.ApproverChoices[2].Item1);
            Assert.AreEqual("LastName3, FirstName3 (3)", result.ApproverChoices[2].Item2);

            Assert.AreEqual(4, result.AccountManagerChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.AccountManagerChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.AccountManagerChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.AccountManagerChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.AccountManagerChoices[1].Item2);

            Assert.AreEqual("1", result.AccountManagerChoices[2].Item1);
            Assert.AreEqual("LastName1, FirstName1 (1)", result.AccountManagerChoices[2].Item2);
            Assert.AreEqual("4", result.AccountManagerChoices[3].Item1);
            Assert.AreEqual("LastName4, FirstName4 (4)", result.AccountManagerChoices[3].Item2);

            Assert.AreEqual(4, result.PurchaserChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.PurchaserChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.PurchaserChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.PurchaserChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.PurchaserChoices[1].Item2);

            Assert.AreEqual("2", result.PurchaserChoices[2].Item1);
            Assert.AreEqual("LastName2, FirstName2 (2)", result.PurchaserChoices[2].Item2);
            Assert.AreEqual("4", result.PurchaserChoices[3].Item1);
            Assert.AreEqual("LastName4, FirstName4 (4)", result.PurchaserChoices[3].Item2);

            Assert.AreEqual("CLEAR_ALL", result.SelectedApprover);
            Assert.AreEqual("CLEAR_ALL", result.SelectedAccountManager);
            Assert.AreEqual("CLEAR_ALL", result.SelectedPurchaser);

            Assert.IsTrue(result.DefaultSelectedApprover);
            Assert.IsTrue(result.DefaultSelectedAccountManager);
            Assert.IsTrue(result.DefaultSelectedPurchaser);

            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All", "If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All", "If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All");
            WorkgroupService.AssertWasNotCalled(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsPostReturnsViewWithExpectedValuesWhenInvalid4()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[0].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
            workgroupPermissions[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
            workgroupPermissions[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[2].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(4));
            workgroupPermissions[3].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[3].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(5));
            workgroupPermissions[4].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[4].IsDefaultForAccount = true;

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

            var updateMultipleAccountsViewModel = new UpdateMultipleAccountsViewModel();
            updateMultipleAccountsViewModel.DefaultSelectedApprover = true;
            updateMultipleAccountsViewModel.DefaultSelectedAccountManager = true;
            updateMultipleAccountsViewModel.DefaultSelectedPurchaser = true;
            updateMultipleAccountsViewModel.SelectedApprover = "DO_NOT_UPDATE";
            updateMultipleAccountsViewModel.SelectedAccountManager = "DO_NOT_UPDATE";
            updateMultipleAccountsViewModel.SelectedPurchaser = "DO_NOT_UPDATE";
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3, updateMultipleAccountsViewModel)
                .AssertViewRendered()
                .WithViewData<UpdateMultipleAccountsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.ApproverChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.ApproverChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.ApproverChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.ApproverChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.ApproverChoices[1].Item2);

            Assert.AreEqual("3", result.ApproverChoices[2].Item1);
            Assert.AreEqual("LastName3, FirstName3 (3)", result.ApproverChoices[2].Item2);

            Assert.AreEqual(4, result.AccountManagerChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.AccountManagerChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.AccountManagerChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.AccountManagerChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.AccountManagerChoices[1].Item2);

            Assert.AreEqual("1", result.AccountManagerChoices[2].Item1);
            Assert.AreEqual("LastName1, FirstName1 (1)", result.AccountManagerChoices[2].Item2);
            Assert.AreEqual("4", result.AccountManagerChoices[3].Item1);
            Assert.AreEqual("LastName4, FirstName4 (4)", result.AccountManagerChoices[3].Item2);

            Assert.AreEqual(4, result.PurchaserChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.PurchaserChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.PurchaserChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.PurchaserChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.PurchaserChoices[1].Item2);

            Assert.AreEqual("2", result.PurchaserChoices[2].Item1);
            Assert.AreEqual("LastName2, FirstName2 (2)", result.PurchaserChoices[2].Item2);
            Assert.AreEqual("4", result.PurchaserChoices[3].Item1);
            Assert.AreEqual("LastName4, FirstName4 (4)", result.PurchaserChoices[3].Item2);

            Assert.AreEqual("DO_NOT_UPDATE", result.SelectedApprover);
            Assert.AreEqual("DO_NOT_UPDATE", result.SelectedAccountManager);
            Assert.AreEqual("DO_NOT_UPDATE", result.SelectedPurchaser);

            Assert.IsTrue(result.DefaultSelectedApprover);
            Assert.IsTrue(result.DefaultSelectedAccountManager);
            Assert.IsTrue(result.DefaultSelectedPurchaser);

            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All", "If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All", "If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All");
            WorkgroupService.AssertWasNotCalled(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsPostReturnsViewWithExpectedValuesWhenInvalid5()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[0].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
            workgroupPermissions[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
            workgroupPermissions[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[2].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(4));
            workgroupPermissions[3].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[3].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(5));
            workgroupPermissions[4].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[4].IsDefaultForAccount = true;

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

            var updateMultipleAccountsViewModel = new UpdateMultipleAccountsViewModel();
            updateMultipleAccountsViewModel.DefaultSelectedApprover = false;
            updateMultipleAccountsViewModel.DefaultSelectedAccountManager = true;
            updateMultipleAccountsViewModel.DefaultSelectedPurchaser = false;
            updateMultipleAccountsViewModel.SelectedApprover = "DO_NOT_UPDATE";
            updateMultipleAccountsViewModel.SelectedAccountManager = "DO_NOT_UPDATE";
            updateMultipleAccountsViewModel.SelectedPurchaser = "DO_NOT_UPDATE";
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3, updateMultipleAccountsViewModel)
                .AssertViewRendered()
                .WithViewData<UpdateMultipleAccountsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.ApproverChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.ApproverChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.ApproverChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.ApproverChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.ApproverChoices[1].Item2);

            Assert.AreEqual("3", result.ApproverChoices[2].Item1);
            Assert.AreEqual("LastName3, FirstName3 (3)", result.ApproverChoices[2].Item2);

            Assert.AreEqual(4, result.AccountManagerChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.AccountManagerChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.AccountManagerChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.AccountManagerChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.AccountManagerChoices[1].Item2);

            Assert.AreEqual("1", result.AccountManagerChoices[2].Item1);
            Assert.AreEqual("LastName1, FirstName1 (1)", result.AccountManagerChoices[2].Item2);
            Assert.AreEqual("4", result.AccountManagerChoices[3].Item1);
            Assert.AreEqual("LastName4, FirstName4 (4)", result.AccountManagerChoices[3].Item2);

            Assert.AreEqual(4, result.PurchaserChoices.Count);
            Assert.AreEqual("DO_NOT_UPDATE", result.PurchaserChoices[0].Item1);
            Assert.AreEqual("-- Do Not Update --", result.PurchaserChoices[0].Item2);
            Assert.AreEqual("CLEAR_ALL", result.PurchaserChoices[1].Item1);
            Assert.AreEqual("-- Clear All --", result.PurchaserChoices[1].Item2);

            Assert.AreEqual("2", result.PurchaserChoices[2].Item1);
            Assert.AreEqual("LastName2, FirstName2 (2)", result.PurchaserChoices[2].Item2);
            Assert.AreEqual("4", result.PurchaserChoices[3].Item1);
            Assert.AreEqual("LastName4, FirstName4 (4)", result.PurchaserChoices[3].Item2);

            Assert.AreEqual("DO_NOT_UPDATE", result.SelectedApprover);
            Assert.AreEqual("DO_NOT_UPDATE", result.SelectedAccountManager);
            Assert.AreEqual("DO_NOT_UPDATE", result.SelectedPurchaser);

            Assert.IsFalse(result.DefaultSelectedApprover);
            Assert.IsTrue(result.DefaultSelectedAccountManager);
            Assert.IsFalse(result.DefaultSelectedPurchaser);

            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All");

            WorkgroupService.AssertWasNotCalled(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsPostRedirectsWhenValid1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[0].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
            workgroupPermissions[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
            workgroupPermissions[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[2].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(4));
            workgroupPermissions[3].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[3].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(5));
            workgroupPermissions[4].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[4].IsDefaultForAccount = true;

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

            var updateMultipleAccountsViewModel = new UpdateMultipleAccountsViewModel();
            updateMultipleAccountsViewModel.DefaultSelectedApprover = true;
            updateMultipleAccountsViewModel.DefaultSelectedAccountManager = true;
            updateMultipleAccountsViewModel.DefaultSelectedPurchaser = true;
            updateMultipleAccountsViewModel.SelectedApprover = "3";
            updateMultipleAccountsViewModel.SelectedAccountManager = "1";
            updateMultipleAccountsViewModel.SelectedPurchaser = "4";
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3, updateMultipleAccountsViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual("Values Updated", Controller.Message);
            

            WorkgroupService.AssertWasCalled(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything), x => x.Repeat.Times(3));
            var args1 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[0];
            var args2 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[1];
            var args3 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[2]; 

            Assert.AreEqual(3, ((Workgroup)args1[0]).Id);
            Assert.AreEqual(3, ((Workgroup)args2[0]).Id);
            Assert.AreEqual(3, ((Workgroup)args3[0]).Id);

            Assert.AreEqual(true, args1[1]);
            Assert.AreEqual(true, args2[1]);
            Assert.AreEqual(true, args3[1]);

            Assert.AreEqual("3", args1[2]);
            Assert.AreEqual("1", args2[2]);
            Assert.AreEqual("4", args3[2]);

            Assert.AreEqual("AP", args1[3]);
            Assert.AreEqual("AM", args2[3]);
            Assert.AreEqual("PR", args3[3]);

            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsPostRedirectsWhenValid2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[0].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
            workgroupPermissions[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
            workgroupPermissions[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[2].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(4));
            workgroupPermissions[3].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[3].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(5));
            workgroupPermissions[4].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[4].IsDefaultForAccount = true;

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

            var updateMultipleAccountsViewModel = new UpdateMultipleAccountsViewModel();
            updateMultipleAccountsViewModel.DefaultSelectedApprover = true;
            updateMultipleAccountsViewModel.DefaultSelectedAccountManager = false;
            updateMultipleAccountsViewModel.DefaultSelectedPurchaser = true;
            updateMultipleAccountsViewModel.SelectedApprover = "3";
            updateMultipleAccountsViewModel.SelectedAccountManager = "1";
            updateMultipleAccountsViewModel.SelectedPurchaser = "4";
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3, updateMultipleAccountsViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual("Values Updated", Controller.Message);


            WorkgroupService.AssertWasCalled(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything), x => x.Repeat.Times(3));
            var args1 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[0];
            var args2 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[1];
            var args3 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[2];

            Assert.AreEqual(3, ((Workgroup)args1[0]).Id);
            Assert.AreEqual(3, ((Workgroup)args2[0]).Id);
            Assert.AreEqual(3, ((Workgroup)args3[0]).Id);

            Assert.AreEqual(true, args1[1]);
            Assert.AreEqual(false, args2[1]);
            Assert.AreEqual(true, args3[1]);

            Assert.AreEqual("3", args1[2]);
            Assert.AreEqual("1", args2[2]);
            Assert.AreEqual("4", args3[2]);

            Assert.AreEqual("AP", args1[3]);
            Assert.AreEqual("AM", args2[3]);
            Assert.AreEqual("PR", args3[3]);

            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsPostRedirectsWhenValid3()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[0].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
            workgroupPermissions[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
            workgroupPermissions[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[2].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(4));
            workgroupPermissions[3].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[3].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(5));
            workgroupPermissions[4].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[4].IsDefaultForAccount = true;

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

            var updateMultipleAccountsViewModel = new UpdateMultipleAccountsViewModel();
            updateMultipleAccountsViewModel.DefaultSelectedApprover = true;
            updateMultipleAccountsViewModel.DefaultSelectedAccountManager = true;
            updateMultipleAccountsViewModel.DefaultSelectedPurchaser = false;
            updateMultipleAccountsViewModel.SelectedApprover = "3";
            updateMultipleAccountsViewModel.SelectedAccountManager = "1";
            updateMultipleAccountsViewModel.SelectedPurchaser = "4";
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3, updateMultipleAccountsViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual("Values Updated", Controller.Message);


            WorkgroupService.AssertWasCalled(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything), x => x.Repeat.Times(3));
            var args1 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[0];
            var args2 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[1];
            var args3 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[2];

            Assert.AreEqual(3, ((Workgroup)args1[0]).Id);
            Assert.AreEqual(3, ((Workgroup)args2[0]).Id);
            Assert.AreEqual(3, ((Workgroup)args3[0]).Id);

            Assert.AreEqual(true, args1[1]);
            Assert.AreEqual(true, args2[1]);
            Assert.AreEqual(false, args3[1]);

            Assert.AreEqual("3", args1[2]);
            Assert.AreEqual("1", args2[2]);
            Assert.AreEqual("4", args3[2]);

            Assert.AreEqual("AP", args1[3]);
            Assert.AreEqual("AM", args2[3]);
            Assert.AreEqual("PR", args3[3]);

            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateMultipleAccountsPostRedirectsWhenValid4()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeUsers(5, UserRepository);
            SetupRoles(new List<Role>());

            var workgroupPermissions = new List<WorkgroupPermission>();
            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
            workgroupPermissions[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[0].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
            workgroupPermissions[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].IsDefaultForAccount = true;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
            workgroupPermissions[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[2].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(4));
            workgroupPermissions[3].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[3].IsDefaultForAccount = false;

            workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(5));
            workgroupPermissions[4].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[4].IsDefaultForAccount = true;

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

            var updateMultipleAccountsViewModel = new UpdateMultipleAccountsViewModel();
            updateMultipleAccountsViewModel.DefaultSelectedApprover = false;
            updateMultipleAccountsViewModel.DefaultSelectedAccountManager = true;
            updateMultipleAccountsViewModel.DefaultSelectedPurchaser = true;
            updateMultipleAccountsViewModel.SelectedApprover = "3";
            updateMultipleAccountsViewModel.SelectedAccountManager = "1";
            updateMultipleAccountsViewModel.SelectedPurchaser = "4";
            #endregion Arrange

            #region Act
            var result = Controller.UpdateMultipleAccounts(3, updateMultipleAccountsViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual("Values Updated", Controller.Message);


            WorkgroupService.AssertWasCalled(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything), x => x.Repeat.Times(3));
            var args1 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[0];
            var args2 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[1];
            var args3 = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.UpdateDefaultAccountApprover(Arg<Workgroup>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[2];

            Assert.AreEqual(3, ((Workgroup)args1[0]).Id);
            Assert.AreEqual(3, ((Workgroup)args2[0]).Id);
            Assert.AreEqual(3, ((Workgroup)args3[0]).Id);

            Assert.AreEqual(false, args1[1]);
            Assert.AreEqual(true, args2[1]);
            Assert.AreEqual(true, args3[1]);

            Assert.AreEqual("3", args1[2]);
            Assert.AreEqual("1", args2[2]);
            Assert.AreEqual("4", args3[2]);

            Assert.AreEqual("AP", args1[3]);
            Assert.AreEqual("AM", args2[3]);
            Assert.AreEqual("PR", args3[3]);

            #endregion Assert
        }
        #endregion UpdateMultipleAccounts Post Tests

        #region UpdateAccount Tests

        [TestMethod]
        public void TestUpdateAccountWhenException1()
        {
            #region Arrange
            //Nothing
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(1, 1, "test", "test", "test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(false, data.success.Value);
            Assert.AreEqual("Error", data.message.Value);
            Assert.AreEqual(string.Empty, data.rtApprover.Value);
            Assert.AreEqual(string.Empty, data.rtAccountManager.Value);
            Assert.AreEqual(string.Empty, data.rtPurchaser.Value);
            #endregion Assert		
        }

        [TestMethod]
        public void TestUpdateAccountWhenException2()
        {
            #region Arrange
            var workgroupsAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                workgroupsAccounts.Add(CreateValidEntities.WorkgroupAccount(i+1));
                workgroupsAccounts[i].Workgroup.SetIdTo(7);
            }
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupsAccounts);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(1, 1, "DO_NOT_UPDATE", "DO_NOT_UPDATE", "DO_NOT_UPDATE")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(false, data.success.Value);
            Assert.AreEqual("Error", data.message.Value);
            Assert.AreEqual(string.Empty, data.rtApprover.Value);
            Assert.AreEqual(string.Empty, data.rtAccountManager.Value);
            Assert.AreEqual(string.Empty, data.rtPurchaser.Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateAccountReturnsExpectedValue1()
        {
            #region Arrange
            var workgroupsAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                workgroupsAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupsAccounts[i].Workgroup.SetIdTo(7);
                workgroupsAccounts[i].Approver = null;
                workgroupsAccounts[i].AccountManager = null;
                workgroupsAccounts[i].Purchaser = null;
            }
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupsAccounts);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(7, 1, "DO_NOT_UPDATE", "DO_NOT_UPDATE", "DO_NOT_UPDATE")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(true, data.success.Value);
            Assert.AreEqual("Done", data.message.Value);
            Assert.AreEqual(string.Empty, data.rtApprover.Value);
            Assert.AreEqual(string.Empty, data.rtAccountManager.Value);
            Assert.AreEqual(string.Empty, data.rtPurchaser.Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateAccountReturnsExpectedValue2()
        {
            #region Arrange
            new FakeUsers(10, UserRepository);
            var workgroupsAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                workgroupsAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupsAccounts[i].Workgroup.SetIdTo(7);
                workgroupsAccounts[i].Approver = UserRepository.Queryable.Single(a => a.Id == "2");
                workgroupsAccounts[i].AccountManager = UserRepository.Queryable.Single(a => a.Id == "4");
                workgroupsAccounts[i].Purchaser = UserRepository.Queryable.Single(a => a.Id == "6");
            }
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupsAccounts);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(7, 1, "DO_NOT_UPDATE", "DO_NOT_UPDATE", "DO_NOT_UPDATE")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(true, data.success.Value);
            Assert.AreEqual("Done", data.message.Value);
            Assert.AreEqual("FirstName2 LastName2 (2)", data.rtApprover.Value);
            Assert.AreEqual("FirstName4 LastName4 (4)", data.rtAccountManager.Value);
            Assert.AreEqual("FirstName6 LastName6 (6)", data.rtPurchaser.Value);
            WorkgroupAccountRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateAccountReturnsExpectedValue3()
        {
            #region Arrange
            new FakeUsers(10, UserRepository);
            var workgroupsAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                workgroupsAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupsAccounts[i].Workgroup.SetIdTo(7);
                workgroupsAccounts[i].Approver = UserRepository.Queryable.Single(a => a.Id == "2");
                workgroupsAccounts[i].AccountManager = UserRepository.Queryable.Single(a => a.Id == "4");
                workgroupsAccounts[i].Purchaser = UserRepository.Queryable.Single(a => a.Id == "6");
            }
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupsAccounts);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(7, 1, "CLEAR_ALL", "DO_NOT_UPDATE", "DO_NOT_UPDATE")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(true, data.success.Value);
            Assert.AreEqual("Done", data.message.Value);
            Assert.AreEqual("", data.rtApprover.Value);
            Assert.AreEqual("FirstName4 LastName4 (4)", data.rtAccountManager.Value);
            Assert.AreEqual("FirstName6 LastName6 (6)", data.rtPurchaser.Value);
            WorkgroupAccountRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            var args = (WorkgroupAccount) WorkgroupAccountRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything))[0][0]; 
            Assert.IsNotNull(result);
            Assert.AreEqual(1, args.Id);
            Assert.AreEqual(7, args.Workgroup.Id);
            Assert.IsNull(args.Approver);
            Assert.AreEqual("FirstName4 LastName4 (4)", args.AccountManager.FullNameAndId);
            Assert.AreEqual("FirstName6 LastName6 (6)", args.Purchaser.FullNameAndId);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateAccountReturnsExpectedValue4()
        {
            #region Arrange
            new FakeUsers(10, UserRepository);
            var workgroupsAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                workgroupsAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupsAccounts[i].Workgroup.SetIdTo(7);
                workgroupsAccounts[i].Approver = UserRepository.Queryable.Single(a => a.Id == "2");
                workgroupsAccounts[i].AccountManager = UserRepository.Queryable.Single(a => a.Id == "4");
                workgroupsAccounts[i].Purchaser = UserRepository.Queryable.Single(a => a.Id == "6");
            }
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupsAccounts);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(7, 1, "DO_NOT_UPDATE", "CLEAR_ALL", "DO_NOT_UPDATE")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(true, data.success.Value);
            Assert.AreEqual("Done", data.message.Value);
            Assert.AreEqual("FirstName2 LastName2 (2)", data.rtApprover.Value);
            Assert.AreEqual("", data.rtAccountManager.Value);
            Assert.AreEqual("FirstName6 LastName6 (6)", data.rtPurchaser.Value);
            WorkgroupAccountRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            var args = (WorkgroupAccount)WorkgroupAccountRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything))[0][0];
            Assert.IsNotNull(result);
            Assert.AreEqual(1, args.Id);
            Assert.AreEqual(7, args.Workgroup.Id);
            Assert.AreEqual("FirstName2 LastName2 (2)", args.Approver.FullNameAndId);
            Assert.IsNull(args.AccountManager);
            Assert.AreEqual("FirstName6 LastName6 (6)", args.Purchaser.FullNameAndId);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateAccountReturnsExpectedValue5()
        {
            #region Arrange
            new FakeUsers(10, UserRepository);
            var workgroupsAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                workgroupsAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupsAccounts[i].Workgroup.SetIdTo(7);
                workgroupsAccounts[i].Approver = UserRepository.Queryable.Single(a => a.Id == "2");
                workgroupsAccounts[i].AccountManager = UserRepository.Queryable.Single(a => a.Id == "4");
                workgroupsAccounts[i].Purchaser = UserRepository.Queryable.Single(a => a.Id == "6");
            }
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupsAccounts);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(7, 1, "DO_NOT_UPDATE", "DO_NOT_UPDATE", "CLEAR_ALL")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(true, data.success.Value);
            Assert.AreEqual("Done", data.message.Value);
            Assert.AreEqual("FirstName2 LastName2 (2)", data.rtApprover.Value);
            Assert.AreEqual("FirstName4 LastName4 (4)", data.rtAccountManager.Value);
            Assert.AreEqual("", data.rtPurchaser.Value);
            WorkgroupAccountRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            var args = (WorkgroupAccount)WorkgroupAccountRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything))[0][0];
            Assert.IsNotNull(result);
            Assert.AreEqual(1, args.Id);
            Assert.AreEqual(7, args.Workgroup.Id);
            Assert.AreEqual("FirstName2 LastName2 (2)", args.Approver.FullNameAndId);
            Assert.AreEqual("FirstName4 LastName4 (4)", args.AccountManager.FullNameAndId);
            Assert.IsNull(args.Purchaser);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateAccountReturnsExpectedValue6()
        {
            #region Arrange
            new FakeUsers(10, UserRepository);
            var workgroupsAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                workgroupsAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupsAccounts[i].Workgroup.SetIdTo(7);
                workgroupsAccounts[i].Approver = UserRepository.Queryable.Single(a => a.Id == "2");
                workgroupsAccounts[i].AccountManager = UserRepository.Queryable.Single(a => a.Id == "4");
                workgroupsAccounts[i].Purchaser = UserRepository.Queryable.Single(a => a.Id == "6");
            }
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupsAccounts);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(7, 1, "CLEAR_ALL", "CLEAR_ALL", "CLEAR_ALL")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(true, data.success.Value);
            Assert.AreEqual("Done", data.message.Value);
            Assert.AreEqual("", data.rtApprover.Value);
            Assert.AreEqual("", data.rtAccountManager.Value);
            Assert.AreEqual("", data.rtPurchaser.Value);
            WorkgroupAccountRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            var args = (WorkgroupAccount)WorkgroupAccountRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything))[0][0];
            Assert.IsNotNull(result);
            Assert.AreEqual(1, args.Id);
            Assert.AreEqual(7, args.Workgroup.Id);
            Assert.IsNull(args.Approver);
            Assert.IsNull(args.AccountManager);
            Assert.IsNull(args.Purchaser);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateAccountReturnsExpectedValue7()
        {
            #region Arrange
            new FakeUsers(10, UserRepository);
            var workgroupsAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                workgroupsAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupsAccounts[i].Workgroup.SetIdTo(7);
                workgroupsAccounts[i].Approver = UserRepository.Queryable.Single(a => a.Id == "2");
                workgroupsAccounts[i].AccountManager = UserRepository.Queryable.Single(a => a.Id == "4");
                workgroupsAccounts[i].Purchaser = UserRepository.Queryable.Single(a => a.Id == "6");
            }
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupsAccounts);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(7, 1, "3", "DO_NOT_UPDATE", "DO_NOT_UPDATE")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(true, data.success.Value);
            Assert.AreEqual("Done", data.message.Value);
            Assert.AreEqual("FirstName3 LastName3 (3)", data.rtApprover.Value);
            Assert.AreEqual("FirstName4 LastName4 (4)", data.rtAccountManager.Value);
            Assert.AreEqual("FirstName6 LastName6 (6)", data.rtPurchaser.Value);
            WorkgroupAccountRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            var args = (WorkgroupAccount)WorkgroupAccountRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything))[0][0];
            Assert.IsNotNull(result);
            Assert.AreEqual(1, args.Id);
            Assert.AreEqual(7, args.Workgroup.Id);
            Assert.AreEqual("FirstName3 LastName3 (3)", args.Approver.FullNameAndId);
            Assert.AreEqual("FirstName4 LastName4 (4)", args.AccountManager.FullNameAndId);
            Assert.AreEqual("FirstName6 LastName6 (6)", args.Purchaser.FullNameAndId);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateAccountReturnsExpectedValue8()
        {
            #region Arrange
            new FakeUsers(10, UserRepository);
            var workgroupsAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                workgroupsAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupsAccounts[i].Workgroup.SetIdTo(7);
                workgroupsAccounts[i].Approver = UserRepository.Queryable.Single(a => a.Id == "2");
                workgroupsAccounts[i].AccountManager = UserRepository.Queryable.Single(a => a.Id == "4");
                workgroupsAccounts[i].Purchaser = UserRepository.Queryable.Single(a => a.Id == "6");
            }
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupsAccounts);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(7, 1, "3", "5", "7")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(true, data.success.Value);
            Assert.AreEqual("Done", data.message.Value);
            Assert.AreEqual("FirstName3 LastName3 (3)", data.rtApprover.Value);
            Assert.AreEqual("FirstName5 LastName5 (5)", data.rtAccountManager.Value);
            Assert.AreEqual("FirstName7 LastName7 (7)", data.rtPurchaser.Value);
            WorkgroupAccountRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything));
            var args = (WorkgroupAccount)WorkgroupAccountRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupAccount>.Is.Anything))[0][0];
            Assert.IsNotNull(result);
            Assert.AreEqual(1, args.Id);
            Assert.AreEqual(7, args.Workgroup.Id);
            Assert.AreEqual("FirstName3 LastName3 (3)", args.Approver.FullNameAndId);
            Assert.AreEqual("FirstName5 LastName5 (5)", args.AccountManager.FullNameAndId);
            Assert.AreEqual("FirstName7 LastName7 (7)", args.Purchaser.FullNameAndId);
            #endregion Assert
        }

        [TestMethod]
        public void TestUpdateAccountReturnsExpectedValue9()
        {
            #region Arrange
            new FakeUsers(10, UserRepository);
            var workgroupsAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                workgroupsAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupsAccounts[i].Workgroup.SetIdTo(7);
                workgroupsAccounts[i].Approver = UserRepository.Queryable.Single(a => a.Id == "2");
                workgroupsAccounts[i].AccountManager = UserRepository.Queryable.Single(a => a.Id == "4");
                workgroupsAccounts[i].Purchaser = UserRepository.Queryable.Single(a => a.Id == "6");
            }
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupsAccounts);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateAccount(7, 1, "3", "5", "999")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.AreEqual(false, data.success.Value);
            Assert.AreEqual("Error", data.message.Value);
            Assert.AreEqual(string.Empty, data.rtApprover.Value);
            Assert.AreEqual(string.Empty, data.rtAccountManager.Value);
            Assert.AreEqual(string.Empty, data.rtPurchaser.Value);
            #endregion Assert
        }
        #endregion UpdateAccount Tests

    }
}
