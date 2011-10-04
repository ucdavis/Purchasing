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
        #region DeletePeople Get Tests
        [TestMethod]
        public void TestDeletePeopleGetRedirectsIfWorkgroupNotFound1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePeople(3, 4, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePeopleGetRedirectsIfWorkgroupNotFound2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePeople(3, 4, Role.Codes.DepartmentalAdmin)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePeopleGetRedirectsIfNoAccess1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.DeletePeople(4, 3, null)
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
        public void TestDeletePeopleGetRedirectsIfNoAccess2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.DeletePeople(3, 3, Role.Codes.DepartmentalAdmin)
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
        public void TestDeletePeopleGetRedirectsIfWorkgroupPermissionNotFound1()
        {
            #region Arrange
            SetupDataForPeopleList();
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(22, 3, null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreSame(null, result.RouteValues["roleFilter"]);

            HasAccessService.AssertWasCalled(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)HasAccessService.GetArgumentsForCallsMadeOn(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);
            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePeopleGetRedirectsIfWorkgroupPermissionNotFound2()
        {
            #region Arrange
            SetupDataForPeopleList();
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(22, 3, Role.Codes.Requester)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreSame(Role.Codes.Requester, result.RouteValues["roleFilter"]);

            HasAccessService.AssertWasCalled(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)HasAccessService.GetArgumentsForCallsMadeOn(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePeopleGetRedirectsIfIdsDifferent()
        {
            #region Arrange
            SetupDataForPeopleList();
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(true);
            #endregion Arrange

            #region Act
            Controller.DeletePeople(20, 2, Role.Codes.Requester)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Person does not belong to workgroup.", Controller.Message);

            HasAccessService.AssertWasCalled(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)HasAccessService.GetArgumentsForCallsMadeOn(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name2", args.Name);
            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePeopleGetReturnsView1()
        {
            #region Arrange
            SetupDataForPeopleList();
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(21, 3, Role.Codes.Requester)
                .AssertViewRendered()
                .WithViewData<WorkgroupPeopleDeleteModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.WorkgroupPermissions.Count);
            foreach (var workgroupPermission in result.WorkgroupPermissions)
            {
                Assert.AreEqual("FirstName3", workgroupPermission.User.FirstName);
                Assert.AreEqual(3, workgroupPermission.Workgroup.Id);
                Assert.IsTrue(workgroupPermission.Role.Level >= 1 && workgroupPermission.Role.Level <= 4);
            }

            Assert.AreEqual(Role.Codes.Requester, Controller.ViewBag.roleFilter);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeletePeopleGetReturnsView2()
        {
            #region Arrange
            SetupDataForPeopleList();
            HasAccessService.Expect(a => a.DaAccessToWorkgroup(Arg<Workgroup>.Is.Anything)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(18, 3, Role.Codes.Purchaser)
                .AssertViewRendered()
                .WithViewData<WorkgroupPeopleDeleteModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.WorkgroupPermissions.Count);
            foreach(var workgroupPermission in result.WorkgroupPermissions)
            {
                Assert.AreEqual("FirstName6", workgroupPermission.User.FirstName);
                Assert.AreEqual(3, workgroupPermission.Workgroup.Id);
                Assert.IsTrue(workgroupPermission.Role.Level >= 1 && workgroupPermission.Role.Level <= 4);
            }

            Assert.AreEqual(Role.Codes.Purchaser, Controller.ViewBag.roleFilter);
            #endregion Assert
        }
        #endregion DeletePeople Get Tests
    }
}
