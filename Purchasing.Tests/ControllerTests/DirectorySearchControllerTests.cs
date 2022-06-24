using System;
using System.Linq;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Moq;


namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    public class DirectorySearchControllerTests : ControllerTestBase<DirectorySearchController>
    {
        private readonly Type _controllerClass = typeof(DirectorySearchController);
        public IRepositoryWithTypedId<User,string> UserRepository;
        public IDirectorySearchService DirectorySearchService;
        //public IRepository<Example> ExampleRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            UserRepository = Mock.Of<IRepositoryWithTypedId<User, string>>();
            DirectorySearchService = Mock.Of<IDirectorySearchService>();  
            Controller = new DirectorySearchController(DirectorySearchService, UserRepository);            
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());
            base.RegisterAdditionalServices(container);
        }

        #endregion Init

        #region Mapping Tests
        [TestMethod]
        public void TestFindPersonMapping()
        {
            "~/DirectorySearch/FindPerson/test".ShouldMapTo<DirectorySearchController>(a => a.FindPerson("test"), true);
        }

        [TestMethod]
        public void TestSearchUsersMapping()
        {
            "~/DirectorySearch/SearchUsers/test".ShouldMapTo<DirectorySearchController>(a => a.SearchUsers("test"), true);
        }
        #endregion Mapping Tests

        #region Method Tests

        #region FindPerson Tests

        [TestMethod]
        public void TestFindPersonReturnsExpectedResult()
        {
            #region Arrange
            var directoryUser = new DirectoryUser();
            directoryUser.LoginId = "someLogin";
            directoryUser.EmailAddress = "test@testy.com";
            directoryUser.LastName = "SomeLast";
            directoryUser.FirstName = "SomeFirst";
            Mock.Get(DirectorySearchService).Setup(a => a.FindUser("Test")).Returns(directoryUser);
            #endregion Arrange

            #region Act
            var result = Controller.FindPerson("Test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("{\"EmployeeId\":null,\"LoginId\":\"someLogin\",\"FirstName\":\"SomeFirst\",\"LastName\":\"SomeLast\",\"FullName\":null,\"EmailAddress\":\"test@testy.com\",\"PhoneNumber\":null}", result.JsonResultString);
            Mock.Get(DirectorySearchService).Verify(a => a.FindUser("Test"));
            #endregion Assert		
        }

        #endregion FindPerson Tests

        #region SearchUsers Tests
        [TestMethod]
        [ExpectedException(typeof (PreconditionException))]
        public void TestSearchUsersThrowsExceptionIfLdapDoesNotHaveRequiredFields1()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var directoryUser = new DirectoryUser();
                directoryUser.LoginId = string.Empty;
                directoryUser.EmailAddress = "test@testy.com";
                directoryUser.LastName = "SomeLast";
                directoryUser.FirstName = "SomeFirst";
                Mock.Get(DirectorySearchService).Setup(a => a.FindUser("test")).Returns(directoryUser);
                new FakeUsers(3, UserRepository);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.SearchUsers("Test");
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Precondition failed.", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestSearchUsersThrowsExceptionIfLdapDoesNotHaveRequiredFields2()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var directoryUser = new DirectoryUser();
                directoryUser.LoginId = "test";
                directoryUser.EmailAddress = null;
                directoryUser.LastName = "SomeLast";
                directoryUser.FirstName = "SomeFirst";
                Mock.Get(DirectorySearchService).Setup(a => a.FindUser("test")).Returns(directoryUser);
                new FakeUsers(3, UserRepository);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.SearchUsers("Test");
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Precondition failed.", ex.Message);
                throw ex;
            }
        }


        [TestMethod]
        public void TestSearchUsersWhenUserExists1()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var results = Controller.SearchUsers("email2@testy.com")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"Id\":\"2\",\"Label\":\"FirstName2 LastName2\"}]", results.JsonResultString);
            #endregion Assert		
        }

        [TestMethod]
        public void TestSearchUsersWhenUserExists2()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var results = Controller.SearchUsers("Email2@testy.com")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"Id\":\"2\",\"Label\":\"FirstName2 LastName2\"}]", results.JsonResultString);
            #endregion Assert
        }

        [TestMethod]
        public void TestSearchUsersWhenUserExists3()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var results = Controller.SearchUsers("2")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"Id\":\"2\",\"Label\":\"FirstName2 LastName2\"}]", results.JsonResultString);
            #endregion Assert
        }

        [TestMethod]
        public void TestSearchUsersWhenUserFoundWithLdap()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);

            var directoryUser = new DirectoryUser();
            directoryUser.LoginId = "test";
            directoryUser.EmailAddress = "test@testy.com";
            directoryUser.LastName = "SomeLast";
            directoryUser.FirstName = "SomeFirst";
            Mock.Get(DirectorySearchService).Setup(a => a.FindUser("test")).Returns(directoryUser);

            #endregion Arrange

            #region Act
            var results = Controller.SearchUsers("Test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"Id\":\"test\",\"Label\":\"SomeFirst SomeLast\"}]", results.JsonResultString);
            Mock.Get(DirectorySearchService).Verify(a => a.FindUser("test"));
            #endregion Assert
        }


        #endregion SearchUsers Tests
        #endregion Method Tests

        #region Reflection Tests

        #region Controller Class Tests
        [TestMethod]
        public void TestControllerInheritsFromApplicationController()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            Assert.IsNotNull(controllerClass.BaseType);
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("ApplicationController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has 6 attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasSevenAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(7, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "UseTransactionsByDefaultAttribute not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has anti forgery token attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAntiForgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AutoValidateAntiforgeryTokenAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "AutoValidateAntiforgeryTokenAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasVersionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<Mvc.Attributes.VersionAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "VersionAttribute not found.");
            #endregion Assert
        }


        [TestMethod]
        public void TestControllerHasAuthorizeAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "AuthorizeAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasProfileAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<ProfileAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "ProfileAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasAuthorizeApplicationAccess()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeApplicationAccessAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "AuthorizeApplicationAccessAttribute not found.");
            #endregion Assert
        }
        #endregion Controller Class Tests

        #region Controller Method Tests

        [TestMethod]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert           
            Assert.AreEqual(2, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodFindPersonContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("FindPerson");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<HttpGetAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpGetAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodSearchUsersContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("SearchUsers");
            #endregion Arrange

            #region Act            
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }


        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
