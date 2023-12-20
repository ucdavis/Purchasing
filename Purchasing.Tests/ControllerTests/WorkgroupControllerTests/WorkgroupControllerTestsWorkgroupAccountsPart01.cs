﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;

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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
        public async Task TestAddAccountPostRedirectsIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            (await Controller.AddAccount(4, new WorkgroupAccount(), null))
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public async Task TestAddAccountPostRedirectsToAccountsWhenValid()
        {
            #region Arrange
            SetupDataForAccounts1();
            var workgroupAccountToCreate = CreateValidEntities.WorkgroupAccount(9);
            workgroupAccountToCreate.Workgroup = WorkgroupRepository.GetNullableById(2); //This one will be replaced
            workgroupAccountToCreate.Account.Id = "Blah";
            workgroupAccountToCreate.AccountManager.Id = "AccMan";
            workgroupAccountToCreate.Approver.Id = "App";
            workgroupAccountToCreate.Purchaser.Id = "Purchase";
            WorkgroupAccount args = default;
            Mock.Get(WorkgroupAccountRepository).Setup(a => a.EnsurePersistent(It.IsAny<WorkgroupAccount>()))
                .Callback<WorkgroupAccount>(x => args = x);
            #endregion Arrange

            #region Act
            var result = ( await Controller.AddAccount(3, workgroupAccountToCreate, null))
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("Workgroup account saved.", Controller.Message);

            Mock.Get(WorkgroupAccountRepository).Verify(a => a.EnsurePersistent(It.IsAny<WorkgroupAccount>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Workgroup.Id);
            Assert.AreEqual("Blah", args.Account.Id);
            Assert.AreEqual("AccMan", args.AccountManager.Id);
            Assert.AreEqual("App", args.Approver.Id);
            Assert.AreEqual("Purchase", args.Purchaser.Id);
            #endregion Assert		
        }

        [TestMethod]
        public async Task TestAddAccountPostReturnsViewWhenInvalid()
        {
            #region Arrange
            SetupDataForAccounts1();
            var workgroupAccountToCreate = CreateValidEntities.WorkgroupAccount(9);
            workgroupAccountToCreate.Workgroup = WorkgroupRepository.GetNullableById(2); //This one will be replaced
            //workgroupAccountToCreate.Account.Id = "Blah";
            workgroupAccountToCreate.Account = null;
            workgroupAccountToCreate.AccountManager.Id = "AccMan";
            workgroupAccountToCreate.Approver.Id = "App";
            workgroupAccountToCreate.Purchaser.Id = "Purchase";
            workgroupAccountToCreate.Name = null; //This will cause error as account is now nullable and name is required

            new FakeAccounts(0, AccountRepository);
            #endregion Arrange

            #region Act
            var result = (await Controller.AddAccount(3, workgroupAccountToCreate, null))
                .AssertViewRendered()
                .WithViewData<WorkgroupAccountModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Name is required");
            Mock.Get(WorkgroupAccountRepository).Verify(a => a.EnsurePersistent(It.IsAny<WorkgroupAccount>()), Times.Never());

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
