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
        #region AccountDetails Tests

        [TestMethod]
        public void TestAccountDetailsRedirectsIfNotFound()
        {
            #region Arrange
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            #endregion Arrange

            #region Act
            Controller.AccountDetails(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
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
            var result = Controller.AccountDetails(3)
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
            Controller.EditAccount(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Account could not be found", Controller.ErrorMessage);
            #endregion Assert			
        }


        [TestMethod]
        public void TestEditAccountGetReturnsView()
        {
            #region Arrange
            SetupDataForAccounts1();
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
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, accounts);
            #endregion Arrange

            #region Act
            var result = Controller.EditAccount(3)
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
    }
}
