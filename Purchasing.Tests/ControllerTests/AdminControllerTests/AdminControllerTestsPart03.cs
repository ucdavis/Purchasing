using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Mvc.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Tests.ControllerTests.AdminControllerTests
{
    public partial class AdminControllerTests
    {
        #region AJAX Calls

        #region FindUser Tests

        [TestMethod]
        public void TestFindUserWhenUserExistsInDatabase1()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var results = Controller.FindUser("1")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"id\":\"1\",\"FirstName\":\"FirstName1\",\"LastName\":\"LastName1\",\"Email\":\"Email1@testy.com\",\"IsActive\":true}]", results.JsonResultString);
            #endregion Assert		
        }

        [TestMethod]
        public void TestFindUserWhenUserExistsInDatabase2()
        {
            #region Arrange
            var users = new List<User>();
            users.Add(CreateValidEntities.User(1));
            users[0].Email = users[0].Email.ToLower();
            new FakeUsers(0, UserRepository, users, false);
            #endregion Arrange

            #region Act
            var results = Controller.FindUser("Email1@testy.com")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"id\":\"1\",\"FirstName\":\"FirstName1\",\"LastName\":\"LastName1\",\"Email\":\"email1@testy.com\",\"IsActive\":true}]", results.JsonResultString);
            #endregion Assert
        }

        [TestMethod]
        public void TestFindUserWhenUserExistsInDatabase3()
        {
            #region Arrange
            var users = new List<User>();
            users.Add(CreateValidEntities.User(1));
            users[0].Email = users[0].Email.ToLower();
            new FakeUsers(0, UserRepository, users, false);
            #endregion Arrange

            #region Act
            var results = Controller.FindUser("email1@testy.com")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"id\":\"1\",\"FirstName\":\"FirstName1\",\"LastName\":\"LastName1\",\"Email\":\"email1@testy.com\",\"IsActive\":true}]", results.JsonResultString);
            #endregion Assert
        }


        [TestMethod]
        public void TestFindUsersWhenNotFound()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            SearchService.Expect(a => a.FindUser("test")).Return(null);
            #endregion Arrange

            #region Act
            var result = Controller.FindUser("Test");               
            #endregion Act

            #region Assert
            Assert.IsNull(result);
            SearchService.AssertWasCalled(a => a.FindUser("test"));
            #endregion Assert		
        }

        [TestMethod]
        public void TestFindUsersWhenFound()
        {
            #region Arrange
            var user = new DirectoryUser();
            user.LoginId = "test";
            user.EmailAddress = "test@testy.com";
            user.FirstName = "FN";
            user.LastName = "LN";
            new FakeUsers(3, UserRepository);
            SearchService.Expect(a => a.FindUser("test")).Return(user);
            #endregion Arrange

            #region Act
            var result = Controller.FindUser("Test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"id\":\"test\",\"FirstName\":\"FN\",\"LastName\":\"LN\",\"Email\":\"test@testy.com\",\"IsActive\":true}]", result.JsonResultString);
            SearchService.AssertWasCalled(a => a.FindUser("test"));
            #endregion Assert
        }
        #endregion FindUser Tests 

        #region SearchOrgs Tests

        [TestMethod]
        public void TestSearchOrgsWhenNoOrgsFound()
        {
            #region Arrange
            new FakeOrganizations(3, OrganizationRepository);
            #endregion Arrange

            #region Act
            var results = Controller.SearchOrgs("test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[]", results.JsonResultString);
            #endregion Assert		
        }

        [TestMethod]
        public void TestSearchOrgsWhenOrgsFound1()
        {
            #region Arrange
            new FakeOrganizations(3, OrganizationRepository);
            #endregion Arrange

            #region Act
            var results = Controller.SearchOrgs("Name1")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"id\":\"1\",\"label\":\"Name1 (1)\"}]", results.JsonResultString);
            #endregion Assert
        }

        [TestMethod]
        public void TestSearchOrgsWhenOrgsFound2()
        {
            #region Arrange
            new FakeOrganizations(3, OrganizationRepository);
            #endregion Arrange

            #region Act
            var results = Controller.SearchOrgs("2")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"id\":\"2\",\"label\":\"Name2 (2)\"}]", results.JsonResultString);
            #endregion Assert
        }

        #endregion SearchOrgs Tests

        #endregion AJAX Calls
    }
}
