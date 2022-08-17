using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region Workgroup People Tests

        #region People Tests

        [TestMethod]
        public void TestPeopleRedirectsToIndexInWorkgroupNotFound1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.People(4, null)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestPeopleRedirectsToIndexInWorkgroupNotFound2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.People(4, Role.Codes.Requester)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestPeopleRedirectsToIndexInWorkgroupNotFound3()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.People(4, "DA")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestPeopleReturnsView1()
        {
            #region Arrange
            SetupDataForPeopleList();

            #endregion Arrange

            #region Act
            var result = Controller.People(3, null)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleListModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual(4, result.UserRoles.Count());
            Assert.AreEqual("FirstName3", result.UserRoles[0].User.FirstName);
            Assert.AreEqual("Requester, Approver, Account Manager, Purchaser", result.UserRoles[0].RolesList);
            Assert.AreEqual("FirstName4", result.UserRoles[1].User.FirstName);
            Assert.AreEqual("Approver", result.UserRoles[1].RolesList);
            Assert.AreEqual("FirstName5", result.UserRoles[2].User.FirstName);
            Assert.AreEqual("Account Manager", result.UserRoles[2].RolesList);
            Assert.AreEqual("FirstName6", result.UserRoles[3].User.FirstName);
            Assert.AreEqual("Purchaser", result.UserRoles[3].RolesList);


            Assert.IsNull(Controller.ViewBag.roleFilter);
            #endregion Assert		
        }

        [TestMethod]
        public void TestPeopleReturnsView2()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            var result = Controller.People(3, string.Empty)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleListModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual(4, result.UserRoles.Count());
            Assert.AreEqual("FirstName3", result.UserRoles[0].User.FirstName);
            Assert.AreEqual("Requester, Approver, Account Manager, Purchaser", result.UserRoles[0].RolesList);
            Assert.AreEqual("FirstName4", result.UserRoles[1].User.FirstName);
            Assert.AreEqual("Approver", result.UserRoles[1].RolesList);
            Assert.AreEqual("FirstName5", result.UserRoles[2].User.FirstName);
            Assert.AreEqual("Account Manager", result.UserRoles[2].RolesList);
            Assert.AreEqual("FirstName6", result.UserRoles[3].User.FirstName);
            Assert.AreEqual("Purchaser", result.UserRoles[3].RolesList);

            Assert.AreEqual(string.Empty, Controller.ViewBag.roleFilter);
            #endregion Assert
        }

        [TestMethod]
        public void TestPeopleReturnsView3()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            var result = Controller.People(3, Role.Codes.Requester)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleListModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual(1, result.UserRoles.Count());
            Assert.AreEqual("FirstName3", result.UserRoles[0].User.FirstName);
            Assert.AreEqual("Requester, Approver, Account Manager, Purchaser", result.UserRoles[0].RolesList);

            Assert.AreEqual(Role.Codes.Requester,Controller.ViewBag.roleFilter);
            #endregion Assert
        }

        [TestMethod]
        public void TestPeopleReturnsView4()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            var result = Controller.People(3, Role.Codes.Purchaser)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleListModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual(2, result.UserRoles.Count());
            Assert.AreEqual("FirstName3", result.UserRoles[0].User.FirstName);
            Assert.AreEqual("Requester, Approver, Account Manager, Purchaser", result.UserRoles[0].RolesList);
            Assert.AreEqual("FirstName6", result.UserRoles[1].User.FirstName);
            Assert.AreEqual("Purchaser", result.UserRoles[1].RolesList);

            Assert.AreEqual(Role.Codes.Purchaser, Controller.ViewBag.roleFilter);
            #endregion Assert
        }
        #endregion People Tests

        #endregion Workgroup People Tests


    }
}
