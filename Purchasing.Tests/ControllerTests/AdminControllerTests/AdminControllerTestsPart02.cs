using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Controllers;
using Purchasing.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

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
            roles[0].SetIdTo(Role.Codes.DepartmentalAdmin);
            roles.Add(CreateValidEntities.Role(88));
            roles[1].SetIdTo(Role.Codes.Admin);
            roles.Add(CreateValidEntities.Role(77));
            roles[2].SetIdTo(Role.Codes.EmulationUser);
            new FakeRoles(0, RoleRepository, roles, true);

            new FakeUsers(3, UserRepository);
            var user = CreateValidEntities.User(4);
            #endregion Arrange

            #region Act
            Controller.ModifyAdmin(user)
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName4 LastName4 (4) was edited under the administrator role", Controller.Message);
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var userArgs = (User) UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0];
            Assert.IsNotNull(userArgs);
            Assert.AreEqual("FirstName4 LastName4 (4)", userArgs.FullNameAndId);
            Assert.AreEqual(1, userArgs.Roles.Count());
            Assert.AreEqual(Role.Codes.Admin, userArgs.Roles[0].Id);
            Assert.AreEqual("Email4@testy.com", userArgs.Email);
            Assert.IsTrue(userArgs.IsActive);

            EmailPreferencesRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailPreferences>.Is.Anything));
            var epArgs = (EmailPreferences) EmailPreferencesRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailPreferences>.Is.Anything))[0][0]; 
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
            roles[0].SetIdTo(Role.Codes.DepartmentalAdmin);
            roles.Add(CreateValidEntities.Role(88));
            roles[1].SetIdTo(Role.Codes.Admin);
            roles.Add(CreateValidEntities.Role(77));
            roles[2].SetIdTo(Role.Codes.EmulationUser);
            new FakeRoles(0, RoleRepository, roles, true);

            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].Organizations.Add(CreateValidEntities.Organization(1));
            users[0].Organizations.Add(CreateValidEntities.Organization(2));
            users[0].Roles.Add(RoleRepository.Queryable.Single(a => a.Id == Role.Codes.EmulationUser));
            users[0].SetIdTo("3");

            new FakeUsers(0, UserRepository, users, true);
            var user = CreateValidEntities.User(3);
            #endregion Arrange

            #region Act
            Controller.ModifyAdmin(user)
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) was edited under the administrator role", Controller.Message);
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var userArgs = (User)UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0];
            Assert.IsNotNull(userArgs);
            Assert.AreEqual("FirstName3 LastName3 (3)", userArgs.FullNameAndId);
            Assert.AreEqual(2, userArgs.Roles.Count());
            Assert.AreEqual(Role.Codes.EmulationUser, userArgs.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Admin, userArgs.Roles[1].Id);
            Assert.AreEqual(2, userArgs.Organizations.Count());
            Assert.AreEqual("Email3@testy.com", userArgs.Email);
            Assert.IsTrue(userArgs.IsActive);

            EmailPreferencesRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailPreferences>.Is.Anything));
            var epArgs = (EmailPreferences)EmailPreferencesRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailPreferences>.Is.Anything))[0][0];
            Assert.IsNotNull(epArgs);
            Assert.AreEqual("3", epArgs.Id);
            UserIdentity.AssertWasCalled(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "3"));
            #endregion Assert
        }

        [TestMethod]
        public void TestModifyAdminWhenUserFound2()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));

            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].SetIdTo(Role.Codes.DepartmentalAdmin);
            roles.Add(CreateValidEntities.Role(88));
            roles[1].SetIdTo(Role.Codes.Admin);
            roles.Add(CreateValidEntities.Role(77));
            roles[2].SetIdTo(Role.Codes.EmulationUser);
            new FakeRoles(0, RoleRepository, roles, true);

            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].Organizations.Add(CreateValidEntities.Organization(1));
            users[0].Organizations.Add(CreateValidEntities.Organization(2));
            users[0].Roles.Add(RoleRepository.Queryable.Single(a => a.Id == Role.Codes.EmulationUser));
            users[0].SetIdTo("3");

            new FakeUsers(0, UserRepository, users, true);
            var user = CreateValidEntities.User(3);
            new FakeEmailPreferences(3, EmailPreferencesRepository);
            #endregion Arrange

            #region Act
            Controller.ModifyAdmin(user)
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) was edited under the administrator role", Controller.Message);
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            

            EmailPreferencesRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailPreferences>.Is.Anything));

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
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
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
            UserIdentity.Expect(a => a.IsUserInRole("3", Role.Codes.Admin)).Return(false);
            #endregion Arrange

            #region Act
            Controller.RemoveAdmin("3")
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("3 is not an admin", Controller.Message);
            UserIdentity.AssertWasCalled(a => a.IsUserInRole("3", Role.Codes.Admin));
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveAdminRedirectsWhenUserInRole()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            UserIdentity.Expect(a => a.IsUserInRole("3", Role.Codes.Admin)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.RemoveAdmin("3")
                .AssertViewRendered()
                .WithViewData<User>();
            #endregion Act

            #region Assert
            UserIdentity.AssertWasCalled(a => a.IsUserInRole("3", Role.Codes.Admin));
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
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("User 4 not found.", Controller.ErrorMessage);
            UserRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
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
            users[0].SetIdTo("3");
            new FakeUsers(0, UserRepository, users, true);
            #endregion Arrange

            #region Act
            Controller.RemoveAdminRole("3")
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) was successfully removed from the admin role", Controller.Message);
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var args = (User) UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("FirstName3", args.FirstName);
            Assert.AreEqual(1, args.Roles.Count());
            Assert.AreEqual(Role.Codes.EmulationUser, args.Roles[0].Id);
            UserIdentity.AssertWasCalled(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "3"));
            #endregion Assert
        }

        #endregion RemoveAdminRole Post Tests

        [TestMethod]
        public void TestWriteMethodTests()
        {
            #region Arrange
            Assert.Inconclusive("Need to write these tests");
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert
        }
    }
}
