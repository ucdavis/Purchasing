using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;
using UCDArch.Web.ActionResults;
using Microsoft.AspNetCore.Mvc;
using UCDArch.Testing.Extensions;
using Moq;

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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
            WorkgroupPermission args = default;
            Mock.Get( WorkgroupPermissionRepository).Setup(a => a.Remove(It.IsAny<WorkgroupPermission>()))
                .Callback<WorkgroupPermission>(x => args = x);
            var permission = WorkgroupPermissionRepository.Queryable.Single(b => b.Id == 18);
            Mock.Get(WorkgroupService).Setup(a => a.RemoveFromCache(permission));
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 18, Role.Codes.Purchaser, new WorkgroupPermission(), null)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual(Role.Codes.Purchaser, result.RouteValues["roleFilter"]);

            Assert.AreEqual("Person successfully removed from role.", Controller.Message);

            Mock.Get(WorkgroupPermissionRepository).Verify(a => a.Remove(It.IsAny<WorkgroupPermission>()));
 
            Assert.IsNotNull(args);
            Assert.AreEqual(18, args.Id);

            Mock.Get(WorkgroupService).Verify(a => a.RemoveFromCache(permission));
            #endregion Assert	
        }

        [TestMethod]
        public void TestDeletePeoplePostWhenOnlyOneRoleAvailable2()
        {
            #region Arrange
            SetupDataForPeopleList();
            WorkgroupPermission args = default;
            Mock.Get(WorkgroupPermissionRepository).Setup(a => a.Remove(It.IsAny<WorkgroupPermission>()))
                .Callback<WorkgroupPermission>(x => args = x);
            var permission = WorkgroupPermissionRepository.Queryable.Single(b => b.Id == 18);
            Mock.Get(WorkgroupService).Setup(a => a.RemoveFromCache(permission));
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 18, Role.Codes.AccountManager, new WorkgroupPermission(), null)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual(Role.Codes.AccountManager, result.RouteValues["roleFilter"]);

            Assert.AreEqual("Person successfully removed from role.", Controller.Message);

            Mock.Get(WorkgroupPermissionRepository).Verify(a => a.Remove(It.IsAny<WorkgroupPermission>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(18, args.Id);

            Mock.Get(WorkgroupService).Verify(a => a.RemoveFromCache(permission));
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
            WorkgroupPermission args = default;
            Mock.Get( WorkgroupPermissionRepository).Setup(a => a.Remove(It.IsAny<WorkgroupPermission>()))
                .Callback<WorkgroupPermission>(x => args = x);
            var permission = WorkgroupPermissionRepository.Queryable.Single(b => b.Id == 21);
            Mock.Get(WorkgroupService).Setup(a => a.RemoveFromCache(permission));
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 21, Role.Codes.Purchaser, new WorkgroupPermission(), rolesToRemove)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual(Role.Codes.Purchaser, result.RouteValues["roleFilter"]);

            Mock.Get(WorkgroupPermissionRepository).Verify(a => a.Remove(It.IsAny<WorkgroupPermission>()));
 
            Assert.IsNotNull(args);
            Assert.AreEqual(21, args.Id);
            Assert.AreEqual("1 role removed from FirstName3 LastName3", Controller.Message);

            Mock.Get(WorkgroupService).Verify(a => a.RemoveFromCache(permission));
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
            var args = new List<WorkgroupPermission>();
            Mock.Get(WorkgroupPermissionRepository).Setup(a => a.Remove(It.IsAny<WorkgroupPermission>()))
                .Callback((WorkgroupPermission x) => args.Add(x));
            var permission = WorkgroupPermissionRepository.Queryable.Single(b => b.Id == 21);
            Mock.Get(WorkgroupService).Setup(a => a.RemoveFromCache(permission));
            #endregion Arrange

            #region Act
            var result = Controller.DeletePeople(3, 21, Role.Codes.Purchaser, new WorkgroupPermission(), rolesToRemove)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual(Role.Codes.Purchaser, result.RouteValues["roleFilter"]);

            Mock.Get(WorkgroupPermissionRepository).Verify(a => a.Remove(It.IsAny<WorkgroupPermission>()), Times.Exactly(4));

            Assert.IsNotNull(args);
            Assert.AreEqual(4, args.Count());
            Assert.AreEqual(21, ((WorkgroupPermission)args[0]).Id);
            Assert.AreEqual(15, ((WorkgroupPermission)args[1]).Id);
            Assert.AreEqual(20, ((WorkgroupPermission)args[2]).Id);
            Assert.AreEqual(19, ((WorkgroupPermission)args[3]).Id);

            Assert.AreEqual("4 roles removed from FirstName3 LastName3", Controller.Message);

            Mock.Get(WorkgroupService).Verify(a => a.RemoveFromCache(permission));
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
            Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(It.IsAny<User>()), Times.Never());
            Mock.Get(SearchService).Verify(a => a.FindUser(It.IsAny<string>()), Times.Never());
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
            Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(It.IsAny<User>()), Times.Never());
            Mock.Get(SearchService).Verify(a => a.FindUser(It.IsAny<string>()), Times.Never());
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

            Mock.Get(SearchService).Setup(a => a.SearchUsers("bob")).Returns(ldapUsers);
            #endregion Arrange

            #region Act
            var result = Controller.SearchUsers("Bob")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"Id\":\"belogin\",\"Label\":\"Bob Loblaw (belogin)\"}]", result.JsonResultString);
            Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(It.IsAny<User>()), Times.Never());
            Mock.Get(SearchService).Verify(a => a.SearchUsers("bob"));
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
            Mock.Get(SearchService).Setup(a => a.SearchUsers("bob")).Returns(new List<DirectoryUser>());
            #endregion Arrange

            #region Act
            var result = Controller.SearchUsers("Bob")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[]", result.JsonResultString);
            Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(It.IsAny<User>()), Times.Never());
            Mock.Get(SearchService).Verify(a => a.SearchUsers("bob"));
            #endregion Assert
        } 
        #endregion SearchUsers Tests
    }
}
