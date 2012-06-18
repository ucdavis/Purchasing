using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Web.ActionResults;


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
            Controller.DeletePeople(4, 3, null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePeopleGetRedirectsIfWorkgroupNotFound2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePeople(4, 3, Role.Codes.DepartmentalAdmin)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePeopleGetRedirectsIfWorkgroupPermissionNotFound1()
        {
            #region Arrange
            SetupDataForPeopleList();

            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 22, null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreSame(null, result.RouteValues["roleFilter"]);

            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePeopleGetRedirectsIfWorkgroupPermissionNotFound2()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 22, Role.Codes.Requester)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreSame(Role.Codes.Requester, result.RouteValues["roleFilter"]);

            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePeopleGetRedirectsIfIdsDifferent()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            Controller.DeletePeople(2, 20, Role.Codes.Requester)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Person does not belong to workgroup.", Controller.Message);

            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePeopleGetReturnsView1()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 21, Role.Codes.Requester)
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
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 18, Role.Codes.Purchaser)
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

        #region DeletePeople Post Tests
        [TestMethod]
        public void TestDeletePeoplePostRedirectsIfWorkgroupNotFound1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePeople(4, 3, null, new WorkgroupPermission(), new string[0])
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePeoplePostRedirectsIfWorkgroupNotFound2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePeople(4, 3, Role.Codes.DepartmentalAdmin, new WorkgroupPermission(), new string[0])
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePeoplePostRedirectsIfWorkgroupPermissionNotFound1()
        {
            #region Arrange
            SetupDataForPeopleList();

            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 22, null, new WorkgroupPermission(), new string[0])
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreSame(null, result.RouteValues["roleFilter"]);
     
            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePeoplePostRedirectsIfWorkgroupPermissionNotFound2()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 22, Role.Codes.Requester, new WorkgroupPermission(), new string[0])
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreSame(Role.Codes.Requester, result.RouteValues["roleFilter"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePeoplePostRedirectsIfIdsDifferent()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            Controller.DeletePeople(2, 20, Role.Codes.Requester, new WorkgroupPermission(), new string[0])
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Person does not belong to workgroup.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePeoplePostWhenOnlyOneRoleAvailable1()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 18, Role.Codes.Purchaser, new WorkgroupPermission(), null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, Role.Codes.Purchaser));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual(Role.Codes.Purchaser, result.RouteValues["roleFilter"]);

            Assert.AreEqual("Person successfully removed from role.", Controller.Message);

            WorkgroupPermissionRepository.AssertWasCalled(a => a.Remove(Arg<WorkgroupPermission>.Is.Anything));
            var args = (WorkgroupPermission) WorkgroupPermissionRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<WorkgroupPermission>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(18, args.Id);

            WorkgroupService.AssertWasCalled(a => a.RemoveFromCache(WorkgroupPermissionRepository.Queryable.Single(b => b.Id == 18)));
            #endregion Assert	
        }

        [TestMethod]
        public void TestDeletePeoplePostWhenOnlyOneRoleAvailable2()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 18, Role.Codes.AccountManager, new WorkgroupPermission(), null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, Role.Codes.AccountManager));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual(Role.Codes.AccountManager, result.RouteValues["roleFilter"]);

            Assert.AreEqual("Person successfully removed from role.", Controller.Message);

            WorkgroupPermissionRepository.AssertWasCalled(a => a.Remove(Arg<WorkgroupPermission>.Is.Anything));
            var args = (WorkgroupPermission)WorkgroupPermissionRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<WorkgroupPermission>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(18, args.Id);

            WorkgroupService.AssertWasCalled(a => a.RemoveFromCache(WorkgroupPermissionRepository.Queryable.Single(b => b.Id == 18)));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePeoplePostWhenMultipleRolesAvailable1()
        {
            #region Arrange
            SetupDataForPeopleList();

            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 21, Role.Codes.Purchaser, new WorkgroupPermission(), null)
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

            Assert.AreEqual(Role.Codes.Purchaser, Controller.ViewBag.roleFilter);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeletePeoplePostWhenMultipleRolesAvailable2()
        {
            #region Arrange
            SetupDataForPeopleList();
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 21, Role.Codes.Purchaser, new WorkgroupPermission(), new string[0])
                .AssertViewRendered()
                .WithViewData<WorkgroupPeopleDeleteModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.WorkgroupPermissions.Count);
            foreach(var workgroupPermission in result.WorkgroupPermissions)
            {
                Assert.AreEqual("FirstName3", workgroupPermission.User.FirstName);
                Assert.AreEqual(3, workgroupPermission.Workgroup.Id);
                Assert.IsTrue(workgroupPermission.Role.Level >= 1 && workgroupPermission.Role.Level <= 4);
            }

            Assert.AreEqual(Role.Codes.Purchaser, Controller.ViewBag.roleFilter);
            #endregion Assert
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeletePeoplePostWhenMultipleRolesAvailableAndInvalidRolePassed()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                SetupDataForPeopleList();
                var rolesToRemove = new string[1];
                rolesToRemove[0] = Role.Codes.Admin;
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.DeletePeople(3, 21, Role.Codes.Purchaser, new WorkgroupPermission(), rolesToRemove);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains no matching element", ex.Message);
                throw;
            }	
        }
        [TestMethod]
        public void TestDeletePeoplePostWhenMultipleRolesAvailable3()
        {
            #region Arrange
            SetupDataForPeopleList();
            var rolesToRemove  = new string[1];
            rolesToRemove[0] = Role.Codes.Purchaser;
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 21, Role.Codes.Purchaser, new WorkgroupPermission(), rolesToRemove)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, Role.Codes.Purchaser));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual(Role.Codes.Purchaser, result.RouteValues["roleFilter"]);

            WorkgroupPermissionRepository.AssertWasCalled(a => a.Remove(Arg<WorkgroupPermission>.Is.Anything));
            var args = (WorkgroupPermission) WorkgroupPermissionRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<WorkgroupPermission>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(21, args.Id);
            Assert.AreEqual("1 role removed from FirstName3 LastName3", Controller.Message);

            WorkgroupService.AssertWasCalled(a => a.RemoveFromCache(WorkgroupPermissionRepository.Queryable.Single(b => b.Id == 21)));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePeoplePostWhenMultipleRolesAvailable4()
        {
            #region Arrange
            SetupDataForPeopleList();
            var rolesToRemove = new string[4];
            rolesToRemove[0] = Role.Codes.Purchaser;
            rolesToRemove[1] = Role.Codes.Requester;
            rolesToRemove[2] = Role.Codes.AccountManager;
            rolesToRemove[3] = Role.Codes.Approver;
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 21, Role.Codes.Purchaser, new WorkgroupPermission(), rolesToRemove)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, Role.Codes.Purchaser));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual(Role.Codes.Purchaser, result.RouteValues["roleFilter"]);

            WorkgroupPermissionRepository.AssertWasCalled(a => a.Remove(Arg<WorkgroupPermission>.Is.Anything), x => x.Repeat.Times(4));
            var args = WorkgroupPermissionRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<WorkgroupPermission>.Is.Anything));
            Assert.IsNotNull(args);
            Assert.AreEqual(4, args.Count());
            Assert.AreEqual(21, ((WorkgroupPermission)args[0][0]).Id);
            Assert.AreEqual(15, ((WorkgroupPermission)args[1][0]).Id);
            Assert.AreEqual(20, ((WorkgroupPermission)args[2][0]).Id);
            Assert.AreEqual(19, ((WorkgroupPermission)args[3][0]).Id);

            Assert.AreEqual("4 roles removed from FirstName3 LastName3", Controller.Message);

            WorkgroupService.AssertWasCalled(a => a.RemoveFromCache(WorkgroupPermissionRepository.Queryable.Single(b => b.Id == 21)));
            #endregion Assert
        }
        #endregion DeletePeople Post Tests

        #region SearchUsers Tests

        [TestMethod]
        public void TestSearchUsersReturnsExpectedResults1()
        {
            #region Arrange
            var users = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i+1));
                users[i].Email = "email" + (i + 1);                
            }
            new FakeUsers(0, UserRepository, users, false);
            #endregion Arrange

            #region Act
            var result = Controller.SearchUsers("2")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"Id\":\"2\",\"Label\":\"FirstName2 LastName2 (2)\"}]" , result.JsonResultString);
            UserRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            SearchService.AssertWasNotCalled(a => a.FindUser(Arg<string>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestSearchUsersReturnsExpectedResults2()
        {
            #region Arrange
            var users = new List<User>();
            for(int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Email = "email" + (i + 1);
            }
            new FakeUsers(0, UserRepository, users, false);
            #endregion Arrange

            #region Act
            var result = Controller.SearchUsers("email3")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"Id\":\"3\",\"Label\":\"FirstName3 LastName3 (3)\"}]", result.JsonResultString);
            UserRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            SearchService.AssertWasNotCalled(a => a.FindUser(Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestSearchUsersReturnsExpectedResults3()
        {
            #region Arrange
            var users = new List<User>();
            for(int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Email = "email" + (i + 1);
            }
            new FakeUsers(0, UserRepository, users, false);
            var ldapLookup = new DirectoryUser();
            ldapLookup.EmailAddress = "bobby@tester.com";
            ldapLookup.FirstName = "Bob";
            ldapLookup.LastName = "Loblaw";
            ldapLookup.LoginId = "belogin";

            var ldapUsers = new List<DirectoryUser>();
            ldapUsers.Add(ldapLookup);

            SearchService.Expect(a => a.SearchUsers("bob")).Return(ldapUsers);
            #endregion Arrange

            #region Act
            var result = Controller.SearchUsers("Bob")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"Id\":\"belogin\",\"Label\":\"Bob Loblaw (belogin)\"}]", result.JsonResultString);
            UserRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            SearchService.AssertWasCalled(a => a.SearchUsers("bob"));
            #endregion Assert
        }

        [TestMethod]
        public void TestSearchUsersReturnsExpectedResults4()
        {
            #region Arrange
            var users = new List<User>();
            for(int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Email = "email" + (i + 1);
            }
            new FakeUsers(0, UserRepository, users, false);
            SearchService.Expect(a => a.SearchUsers("bob")).Return(new List<DirectoryUser>());
            #endregion Arrange

            #region Act
            var result = Controller.SearchUsers("Bob")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[]", result.JsonResultString);
            UserRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            SearchService.AssertWasCalled(a => a.SearchUsers("bob"));
            #endregion Assert
        } 
        #endregion SearchUsers Tests
    }
}
