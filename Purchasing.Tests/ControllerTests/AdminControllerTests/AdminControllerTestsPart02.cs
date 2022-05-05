using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Controllers;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Tests.ControllerTests.AdminControllerTests
{
    public partial class AdminControllerTests
    {

        #region ModifyAdmin Get Tests

        [TestMethod]
        public void TestModifyAdminReturnsView1()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ModifyAdmin("4")
                .AssertViewRendered()
                .WithViewData<User>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Id);
            Assert.AreEqual("  ()", result.FullNameAndId);
            Assert.IsTrue(result.IsActive);
            #endregion Assert		
        }
        [TestMethod]
        public void TestModifyAdminReturnsView2()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ModifyAdmin(string.Empty)
                .AssertViewRendered()
                .WithViewData<User>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Id);
            Assert.AreEqual("  ()", result.FullNameAndId);
            Assert.IsTrue(result.IsActive);
            #endregion Assert
        }

        [TestMethod]
        public void TestModifyAdminReturnsView3()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ModifyAdmin(id: null)
                .AssertViewRendered()
                .WithViewData<User>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Id);
            Assert.AreEqual("  ()", result.FullNameAndId);
            Assert.IsTrue(result.IsActive);
            #endregion Assert
        }
        #endregion ModifyAdmin Get Tests

        #region ModifyAdmin Post Tests

        [TestMethod]
        public void TestModifyAdminWithInvalidUserReturnsView()
        {
            #region Arrange
            var user = CreateValidEntities.User(9);
            Controller.ModelState.AddModelError("Fake", "Fake Error");
            #endregion Arrange

            #region Act
            var result = Controller.ModifyAdmin(user)
                .AssertViewRendered()
                .WithViewData<User>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("FirstName9", result.FirstName);         
            #endregion Assert		
        }


        [TestMethod]
        public void TestModifyAdminWhenUserNotFound()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
            
            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].Id = Role.Codes.DepartmentalAdmin;
            roles.Add(CreateValidEntities.Role(88));
            roles[1].Id = Role.Codes.Admin;
            roles.Add(CreateValidEntities.Role(77));
            roles[2].Id = Role.Codes.EmulationUser;
            new FakeRoles(0, RoleRepository, roles, true);

            new FakeUsers(3, UserRepository);
            var user = CreateValidEntities.User(4);
            #endregion Arrange

            #region Act
            Controller.ModifyAdmin(user)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName4 LastName4 (4) was edited under the administrator role", Controller.Message);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()));
//TODO: Arrange
            User userArgs = default;
            Moq.Mock.Get( UserRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<User>()))
                .Callback<User>(x => userArgs = x);
//ENDTODO
            Assert.IsNotNull(userArgs);
            Assert.AreEqual("FirstName4 LastName4 (4)", userArgs.FullNameAndId);
            Assert.AreEqual(1, userArgs.Roles.Count());
            Assert.AreEqual(Role.Codes.Admin, userArgs.Roles[0].Id);
            Assert.AreEqual("Email4@testy.com", userArgs.Email);
            Assert.IsTrue(userArgs.IsActive);

            Moq.Mock.Get(EmailPreferencesRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<EmailPreferences>()));
//TODO: Arrange
            EmailPreferences epArgs = default;
            Moq.Mock.Get( EmailPreferencesRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<EmailPreferences>()))
                .Callback<EmailPreferences>(x => epArgs = x);
//ENDTODO 
            Assert.IsNotNull(epArgs);
            Assert.AreEqual("4", epArgs.Id);
            #endregion Assert		
        }

        [TestMethod]
        public void TestModifyAdminWhenUserFound1()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));

            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].Id = Role.Codes.DepartmentalAdmin;
            roles.Add(CreateValidEntities.Role(88));
            roles[1].Id = Role.Codes.Admin;
            roles.Add(CreateValidEntities.Role(77));
            roles[2].Id = Role.Codes.EmulationUser;
            new FakeRoles(0, RoleRepository, roles, true);

            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].Organizations.Add(CreateValidEntities.Organization(1));
            users[0].Organizations.Add(CreateValidEntities.Organization(2));
            users[0].Roles.Add(RoleRepository.Queryable.Single(a => a.Id == Role.Codes.EmulationUser));
            users[0].Id = "3";

            new FakeUsers(0, UserRepository, users, true);
            var user = CreateValidEntities.User(3);
            #endregion Arrange

            #region Act
            Controller.ModifyAdmin(user)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) was edited under the administrator role", Controller.Message);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()));
//TODO: Arrange
            User userArgs = default;
            Moq.Mock.Get(UserRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<User>()))
                .Callback<User>(x => userArgs = x);
//ENDTODO
            Assert.IsNotNull(userArgs);
            Assert.AreEqual("FirstName3 LastName3 (3)", userArgs.FullNameAndId);
            Assert.AreEqual(2, userArgs.Roles.Count());
            Assert.AreEqual(Role.Codes.EmulationUser, userArgs.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Admin, userArgs.Roles[1].Id);
            Assert.AreEqual(2, userArgs.Organizations.Count());
            Assert.AreEqual("Email3@testy.com", userArgs.Email);
            Assert.IsTrue(userArgs.IsActive);

            Moq.Mock.Get(EmailPreferencesRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<EmailPreferences>()));
//TODO: Arrange
            EmailPreferences epArgs = default;
            Moq.Mock.Get(EmailPreferencesRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<EmailPreferences>()))
                .Callback<EmailPreferences>(x => epArgs = x);
//ENDTODO
            Assert.IsNotNull(epArgs);
            Assert.AreEqual("3", epArgs.Id);
            Moq.Mock.Get(UserIdentity).Verify(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "3"));
            #endregion Assert
        }

        [TestMethod]
        public void TestModifyAdminWhenUserFound2()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));

            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].Id = Role.Codes.DepartmentalAdmin;
            roles.Add(CreateValidEntities.Role(88));
            roles[1].Id = Role.Codes.Admin;
            roles.Add(CreateValidEntities.Role(77));
            roles[2].Id = Role.Codes.EmulationUser;
            new FakeRoles(0, RoleRepository, roles, true);

            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].Organizations.Add(CreateValidEntities.Organization(1));
            users[0].Organizations.Add(CreateValidEntities.Organization(2));
            users[0].Roles.Add(RoleRepository.Queryable.Single(a => a.Id == Role.Codes.EmulationUser));
            users[0].Id = "3";

            new FakeUsers(0, UserRepository, users, true);
            var user = CreateValidEntities.User(3);
            new FakeEmailPreferences(3, EmailPreferencesRepository);
            #endregion Arrange

            #region Act
            Controller.ModifyAdmin(user)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) was edited under the administrator role", Controller.Message);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()));
            

            Moq.Mock.Get(EmailPreferencesRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<EmailPreferences>()), Moq.Times.Never());

            #endregion Assert
        }
        #endregion ModifyAdmin Post Tests

        #region RemoveAdmin Tests

        [TestMethod]
        public void TestRemoveAdminRedirectsWhenUserNotFound()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            Controller.RemoveAdmin("4")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("User 4 not found.", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestRemoveAdminRedirectsWhenUserNotInRole()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            Moq.Mock.Get(UserIdentity).Setup(a => a.IsUserInRole("3", Role.Codes.Admin)).Returns(false);
            #endregion Arrange

            #region Act
            Controller.RemoveAdmin("3")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("3 is not an admin", Controller.Message);
            Moq.Mock.Get(UserIdentity).Verify(a => a.IsUserInRole("3", Role.Codes.Admin));
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveAdminRedirectsWhenUserInRole()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            Moq.Mock.Get(UserIdentity).Setup(a => a.IsUserInRole("3", Role.Codes.Admin)).Returns(true);
            #endregion Arrange

            #region Act
            var result = Controller.RemoveAdmin("3")
                .AssertViewRendered()
                .WithViewData<User>();
            #endregion Act

            #region Assert
            Moq.Mock.Get(UserIdentity).Verify(a => a.IsUserInRole("3", Role.Codes.Admin));
            Assert.IsNotNull(result);
            Assert.AreEqual("FirstName3 LastName3 (3)", result.FullNameAndId);
            #endregion Assert
        }
        #endregion RemoveAdmin Tests

        #region RemoveAdminRole Post Tests
        [TestMethod]
        public void TestRemoveAdminRoleRedirectsWhenUserNotFound()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            Controller.RemoveAdminRole("4")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("User 4 not found.", Controller.ErrorMessage);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestRemoveAdminRoleWhenUserIsNotInRoleThrowsException()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeUsers(3, UserRepository);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.RemoveAdminRole("3");
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains no elements", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestRemoveAdminRoleRemovesRole()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].Roles.Add(new Role(Role.Codes.EmulationUser));
            users[0].Roles.Add(new Role(Role.Codes.Admin));
            users[0].Id = "3";
            new FakeUsers(0, UserRepository, users, true);
            #endregion Arrange

            #region Act
            Controller.RemoveAdminRole("3")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) was successfully removed from the admin role", Controller.Message);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()));
//TODO: Arrange
            User args = default;
            Moq.Mock.Get( UserRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<User>()))
                .Callback<User>(x => args = x);
//ENDTODO 
            Assert.IsNotNull(args);
            Assert.AreEqual("FirstName3", args.FirstName);
            Assert.AreEqual(1, args.Roles.Count());
            Assert.AreEqual(Role.Codes.EmulationUser, args.Roles[0].Id);
            Moq.Mock.Get(UserIdentity).Verify(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "3"));
            #endregion Assert
        }

        #endregion RemoveAdminRole Post Tests

        #region RemoveDepartmental Get Tests

        [TestMethod]
        public void TestRemoveDepartmentalRedirectsToActionWhenUserNotFound()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act

            Controller.RemoveDepartmental("4")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("User 4 not found.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveDepartmentalRedirectsToActionWhenUserNotDepartmentAdmin()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            Moq.Mock.Get(UserIdentity).Setup(a => a.IsUserInRole("3", Role.Codes.DepartmentalAdmin)).Returns(false);
            #endregion Arrange

            #region Act

            Controller.RemoveDepartmental("3")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Moq.Mock.Get(UserIdentity).Verify(a=> a.IsUserInRole("3", Role.Codes.DepartmentalAdmin));
            Assert.AreEqual("3 is not a departmental admin", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveDepartmentalReturnsView()
        {
            #region Arrange
            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].Organizations.Add(CreateValidEntities.Organization(2));
            users[0].Organizations.Add(CreateValidEntities.Organization(3));
            users[0].Id = "3";
            new FakeUsers(0, UserRepository, users, true);
            Moq.Mock.Get(UserIdentity).Setup(a => a.IsUserInRole("3", Role.Codes.DepartmentalAdmin)).Returns(true);
            #endregion Arrange

            #region Act

            var results =  Controller.RemoveDepartmental("3")
                .AssertViewRendered()
                .WithViewData<User>();
            #endregion Act

            #region Assert
            Moq.Mock.Get(UserIdentity).Verify(a => a.IsUserInRole("3", Role.Codes.DepartmentalAdmin));
            Assert.IsNotNull(results);
            Assert.AreEqual("FirstName3", results.FirstName);
            Assert.AreEqual(2, results.Organizations.Count());
            Assert.AreEqual("Name2", results.Organizations[0].Name);
            Assert.AreEqual("Name3", results.Organizations[1].Name);
            #endregion Assert
        }

        #endregion RemoveDepartmental Get Tests

        #region RemoveDepartmentalRole Post Tests

        [TestMethod]
        public void TestRemoveDepartmentalRoleWhenUserNotFound()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            Controller.RemoveDepartmentalRole("4")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("User 4 not found.", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestRemoveDepartmentalRoleWithoutRole()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var users = new List<User>();
                users.Add(CreateValidEntities.User(1));
                users[0].Roles.Add(CreateValidEntities.Role(1));
                users[0].Roles.Add(CreateValidEntities.Role(2));
                users[0].Roles[0].Id = Role.Codes.Admin;
                users[0].Roles[1].Id = Role.Codes.EmulationUser;
                new FakeUsers(0, UserRepository, users, false);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.RemoveDepartmentalRole("1");
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains no elements", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestRemoveDepartmentalRole()
        {
            #region Arrange
            var users = new List<User>();
            users.Add(CreateValidEntities.User(1));
            users[0].Roles.Add(CreateValidEntities.Role(1));
            users[0].Roles.Add(CreateValidEntities.Role(2));
            users[0].Roles[0].Id = Role.Codes.Admin;
            users[0].Roles[1].Id = Role.Codes.DepartmentalAdmin;
            users[0].Organizations.Add(CreateValidEntities.Organization(1));
            users[0].Organizations.Add(CreateValidEntities.Organization(2));
            new FakeUsers(0, UserRepository, users, false);
            #endregion Arrange

            #region Act
            Controller.RemoveDepartmentalRole("1")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Moq.Mock.Get(UserIdentity).Verify(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId,"1"));
            Assert.AreEqual("FirstName1 LastName1 (1) was successfully removed from the departmental admin role", Controller.Message);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()));
//TODO: Arrange
            User args = default;
            Moq.Mock.Get( UserRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<User>()))
                .Callback<User>(x => args = x);
//ENDTODO 
            Assert.IsNotNull(args);
            Assert.AreEqual(0, args.Organizations.Count());
            Assert.AreEqual("FirstName1", args.FirstName);
            Assert.AreEqual(1, args.Roles.Count());
            Assert.AreEqual(Role.Codes.Admin, args.Roles[0].Id);
            #endregion Assert		
        }
        #endregion RemoveDepartmentalRole Post Tests

        #region Clone Get Tests

        [TestMethod]
        public void TestCloneRedirectsWhenUserNotFound()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            Controller.Clone("4")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("User 4 not found.", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestCloneWhenFoundUserHasNoOrgs()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Clone("2")
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Please enter the new user's information. Department associations for FirstName2 LastName2 (2) have been selected by default.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);
            Assert.IsNull(result.User.Id);
            Assert.AreEqual(0, result.User.Organizations.Count());
            Assert.AreEqual(0, result.User.Roles.Count());
            Assert.IsTrue(result.User.IsActive);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCloneWhenFoundUserHasOrgs()
        {
            #region Arrange
            var users = new List<User>();
            users.Add(CreateValidEntities.User(1));
            users[0].Organizations.Add(CreateValidEntities.Organization(1));
            users[0].Organizations.Add(CreateValidEntities.Organization(3));
            users[0].Organizations.Add(CreateValidEntities.Organization(4));
            users[0].Roles.Add(CreateValidEntities.Role(1));
            new FakeUsers(0, UserRepository, users, false);
            #endregion Arrange

            #region Act
            var result = Controller.Clone("1")
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Please enter the new user's information. Department associations for FirstName1 LastName1 (1) have been selected by default.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);
            Assert.IsNull(result.User.Id);
            Assert.AreEqual(3, result.User.Organizations.Count());
            Assert.AreEqual(0, result.User.Roles.Count());
            Assert.IsTrue(result.User.IsActive);
            #endregion Assert
        }

        #endregion Clone Get Tests
    }
}
