using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.MappingModel;
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
                .ToAction<WorkgroupController>(a => a.Index());
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
                .ToAction<WorkgroupController>(a => a.Index());
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
    }
}
