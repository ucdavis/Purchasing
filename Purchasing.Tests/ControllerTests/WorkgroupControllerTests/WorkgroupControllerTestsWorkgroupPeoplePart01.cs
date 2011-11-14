using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Rhino.Mocks;


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
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
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
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
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
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestPeopleRedirectsToErrorIfNoAccess1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.People(3, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestPeopleRedirectsToErrorIfNoAccess2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.People(3, Role.Codes.Admin)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }


        [TestMethod]
        public void TestPeopleReturnsView1()
        {
            #region Arrange
            SetupDataForPeopleList();
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
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

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.IsNull(Controller.ViewBag.roleFilter);
            #endregion Assert		
        }

        [TestMethod]
        public void TestPeopleReturnsView2()
        {
            #region Arrange
            SetupDataForPeopleList();
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
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

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.AreEqual(string.Empty, Controller.ViewBag.roleFilter);
            #endregion Assert
        }

        [TestMethod]
        public void TestPeopleReturnsView3()
        {
            #region Arrange
            SetupDataForPeopleList();
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
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

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.AreEqual(Role.Codes.Requester,Controller.ViewBag.roleFilter);
            #endregion Assert
        }

        [TestMethod]
        public void TestPeopleReturnsView4()
        {
            #region Arrange
            SetupDataForPeopleList();
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
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

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.AreEqual(Role.Codes.Purchaser, Controller.ViewBag.roleFilter);
            #endregion Assert
        }
        #endregion People Tests

        #endregion Workgroup People Tests


    }
}
