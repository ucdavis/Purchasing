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

        #region ModifySscAdmin Get Tests

        [TestMethod]
        public void TestModifySscAdminReturnsView1()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ModifySscAdmin("4")
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
        public void TestModifySscAdminReturnsView2()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ModifySscAdmin(string.Empty)
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
        public void TestModifySscAdminReturnsView3()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ModifySscAdmin(id: null)
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
        #endregion ModifySscAdmin Get Tests

        #region ModifySscAdmin Post Tests

        [TestMethod]
        public void TestModifySscAdminWithInvalidUserReturnsView()
        {
            #region Arrange
            var user = CreateValidEntities.User(9);
            Controller.ModelState.AddModelError("Fake", "Fake Error");
            #endregion Arrange

            #region Act
            var result = Controller.ModifySscAdmin(user)
                .AssertViewRendered()
                .WithViewData<User>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("FirstName9", result.FirstName);
            #endregion Assert
        }


        [TestMethod]
        public void TestModifySscAdminWhenUserNotFound()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));

            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].Id = Role.Codes.DepartmentalAdmin;
            roles.Add(CreateValidEntities.Role(88));
            roles[1].Id = Role.Codes.SscAdmin;
            roles.Add(CreateValidEntities.Role(77));
            roles[2].Id = Role.Codes.EmulationUser;
            new FakeRoles(0, RoleRepository, roles, true);

            new FakeUsers(3, UserRepository);
            var user = CreateValidEntities.User(4);
            EmailPreferences epArgs = default;
            Moq.Mock.Get(EmailPreferencesRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<EmailPreferences>()))
                .Callback<EmailPreferences>(x => epArgs = x);
            User userArgs = default;
            Moq.Mock.Get(UserRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<User>()))
                .Callback<User>(x => userArgs = x);
            #endregion Arrange

            #region Act
            Controller.ModifySscAdmin(user)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName4 LastName4 (4) was edited under the SSC administrator role", Controller.Message);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()));

            Assert.IsNotNull(userArgs);
            Assert.AreEqual("FirstName4 LastName4 (4)", userArgs.FullNameAndId);
            Assert.AreEqual(1, userArgs.Roles.Count());
            Assert.AreEqual(Role.Codes.SscAdmin, userArgs.Roles[0].Id);
            Assert.AreEqual("Email4@testy.com", userArgs.Email);
            Assert.IsTrue(userArgs.IsActive);

            Moq.Mock.Get(EmailPreferencesRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<EmailPreferences>()));

            Assert.IsNotNull(epArgs);
            Assert.AreEqual("4", epArgs.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestModifySscAdminWhenUserFound1()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));

            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].Id = Role.Codes.DepartmentalAdmin;
            roles.Add(CreateValidEntities.Role(88));
            roles[1].Id = Role.Codes.SscAdmin;
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
            EmailPreferences epArgs = default;
            Moq.Mock.Get(EmailPreferencesRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<EmailPreferences>()))
                .Callback<EmailPreferences>(x => epArgs = x);
            User userArgs = default;
            Moq.Mock.Get(UserRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<User>()))
                .Callback<User>(x => userArgs = x);
            #endregion Arrange

            #region Act
            Controller.ModifySscAdmin(user)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) was edited under the SSC administrator role", Controller.Message);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()));

            Assert.IsNotNull(userArgs);
            Assert.AreEqual("FirstName3 LastName3 (3)", userArgs.FullNameAndId);
            Assert.AreEqual(2, userArgs.Roles.Count());
            Assert.AreEqual(Role.Codes.EmulationUser, userArgs.Roles[0].Id);
            Assert.AreEqual(Role.Codes.SscAdmin, userArgs.Roles[1].Id);
            Assert.AreEqual(2, userArgs.Organizations.Count());
            Assert.AreEqual("Email3@testy.com", userArgs.Email);
            Assert.IsTrue(userArgs.IsActive);

            Moq.Mock.Get(EmailPreferencesRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<EmailPreferences>()));

            Assert.IsNotNull(epArgs);
            Assert.AreEqual("3", epArgs.Id);
            Moq.Mock.Get(UserIdentity).Verify(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "3"));
            #endregion Assert
        }

        [TestMethod]
        public void TestModifySscAdminWhenUserFound2()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));

            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].Id = Role.Codes.DepartmentalAdmin;
            roles.Add(CreateValidEntities.Role(88));
            roles[1].Id = Role.Codes.SscAdmin;
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
            Controller.ModifySscAdmin(user)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) was edited under the SSC administrator role", Controller.Message);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()));


            Moq.Mock.Get(EmailPreferencesRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<EmailPreferences>()), Moq.Times.Never());

            #endregion Assert
        }
        #endregion ModifySscAdmin Post Tests

        #region RemoveSscAdmin Tests

        [TestMethod]
        public void TestRemoveSscAdminRedirectsWhenUserNotFound()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            Controller.RemoveSscAdmin("4")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("User 4 not found.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveSscAdminRedirectsWhenUserNotInRole()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            Moq.Mock.Get(UserIdentity).Setup(a => a.IsUserInRole("3", Role.Codes.SscAdmin)).Returns(false);
            #endregion Arrange

            #region Act
            Controller.RemoveSscAdmin("3")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("3 is not an SSC admin", Controller.Message);
            Moq.Mock.Get(UserIdentity).Verify(a => a.IsUserInRole("3", Role.Codes.SscAdmin));
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveSscAdminRedirectsWhenUserInRole()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            Moq.Mock.Get(UserIdentity).Setup(a => a.IsUserInRole("3", Role.Codes.SscAdmin)).Returns(true);
            #endregion Arrange

            #region Act
            var result = Controller.RemoveSscAdmin("3")
                .AssertViewRendered()
                .WithViewData<User>();
            #endregion Act

            #region Assert
            Moq.Mock.Get(UserIdentity).Verify(a => a.IsUserInRole("3", Role.Codes.SscAdmin));
            Assert.IsNotNull(result);
            Assert.AreEqual("FirstName3 LastName3 (3)", result.FullNameAndId);
            #endregion Assert
        }
        #endregion RemoveSscAdmin Tests

        #region RemoveSscAdminRole Post Tests
        [TestMethod]
        public void TestRemoveSscAdminRoleRedirectsWhenUserNotFound()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            Controller.RemoveSscAdminRole("4")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("User 4 not found.", Controller.ErrorMessage);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRemoveSscAdminRoleWhenUserIsNotInRoleThrowsException()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeUsers(3, UserRepository);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.RemoveSscAdminRole("3");
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
        public void TestRemoveSscAdminRoleRemovesRole()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].Roles.Add(new Role(Role.Codes.EmulationUser));
            users[0].Roles.Add(new Role(Role.Codes.SscAdmin));
            users[0].Id = "3";
            new FakeUsers(0, UserRepository, users, true);
            User args = default;
            Moq.Mock.Get(UserRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<User>()))
                .Callback<User>(x => args = x);
            #endregion Arrange

            #region Act
            Controller.RemoveSscAdminRole("3")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) was successfully removed from the SSC admin role", Controller.Message);
            Moq.Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<User>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("FirstName3", args.FirstName);
            Assert.AreEqual(1, args.Roles.Count());
            Assert.AreEqual(Role.Codes.EmulationUser, args.Roles[0].Id);
            Moq.Mock.Get(UserIdentity).Verify(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "3"));
            #endregion Assert
        }

        #endregion RemoveSscAdminRole Post Tests

    }
}
