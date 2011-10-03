using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region AddPeople Get Tests

        [TestMethod]
        public void TestAddPeopleGetRedirectsIfWorkgroupNotFound1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.AddPeople(4, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddPeopleGetRedirectsIfWorkgroupNotFound2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.AddPeople(4, Role.Codes.DepartmentalAdmin)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeopleGetRedirectsIfNoAccess1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.AddPeople(3, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You must be a department admin for this workgroup to access a workgroup's people", Controller.Message);
            HasAccessService.AssertWasCalled(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)HasAccessService.GetArgumentsForCallsMadeOn(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeopleGetRedirectsIfNoAccess2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.AddPeople(3, Role.Codes.DepartmentalAdmin)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You must be a department admin for this workgroup to access a workgroup's people", Controller.Message);
            HasAccessService.AssertWasCalled(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)HasAccessService.GetArgumentsForCallsMadeOn(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);
            #endregion Assert
        }


        [TestMethod]
        public void TestAddPeopleGetReturnsView1()
        {
            #region Arrange
            SetupRoles(new List<Role>());
            new FakeWorkgroups(3, WorkgroupRepository);
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, null)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();
            #endregion Act

            #region Assert
            HasAccessService.AssertWasCalled(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)HasAccessService.GetArgumentsForCallsMadeOn(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Role);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            Assert.IsNull(result.Users);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeopleGetReturnsView2()
        {
            #region Arrange
            SetupRoles(new List<Role>());
            new FakeWorkgroups(3, WorkgroupRepository);
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, Role.Codes.DepartmentalAdmin)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();
            #endregion Act

            #region Assert
            HasAccessService.AssertWasCalled(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)HasAccessService.GetArgumentsForCallsMadeOn(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Role);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            Assert.IsNull(result.Users);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeopleGetReturnsView3()
        {
            #region Arrange
            SetupRoles(new List<Role>());
            new FakeWorkgroups(3, WorkgroupRepository);
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, Role.Codes.Admin)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();
            #endregion Act

            #region Assert
            HasAccessService.AssertWasCalled(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)HasAccessService.GetArgumentsForCallsMadeOn(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Role);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            Assert.IsNull(result.Users);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeopleGetReturnsView4()
        {
            #region Arrange
            SetupRoles(new List<Role>());
            new FakeWorkgroups(3, WorkgroupRepository);
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, Role.Codes.Requester)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();
            #endregion Act

            #region Assert
            HasAccessService.AssertWasCalled(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)HasAccessService.GetArgumentsForCallsMadeOn(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);

            Assert.IsNotNull(result);
            Assert.AreEqual(Role.Codes.Requester, result.Role.Id);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            Assert.IsNull(result.Users);
            #endregion Assert
        } 
        #endregion AddPeople Get Tests
    }
}
