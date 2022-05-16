using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Services;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Purchasing.Mvc.Utility;
using IdAndName = Purchasing.Core.Services.IdAndName;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Moq;

namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    public class AccountsControllerTests : ControllerTestBase<AccountsController>
    {
        private readonly Type _controllerClass = typeof(AccountsController);
        public IRepositoryWithTypedId<SubAccount, Guid> SubAccountRepository;
        public ISearchService SearchService;


        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            SubAccountRepository = new Mock<IRepositoryWithTypedId<SubAccount, Guid>>().Object;
            SearchService = new Mock<ISearchService>().Object;

            Controller = new AccountsController(SubAccountRepository, SearchService);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());
            base.RegisterAdditionalServices(container);
        }

 
        #endregion Init

        #region Mapping Tests
        [TestMethod]
        public void TestSearchKfsAccountsMapping()
        {
            "~/Accounts/SearchKfsAccounts/Test".ShouldMapTo<AccountsController>(a => a.SearchKfsAccounts("Test"), true);
        }
        [TestMethod]
        public void TestSearchSubAccountsMapping()
        {
            "~/Accounts/SearchSubAccounts/Test".ShouldMapTo<AccountsController>(a => a.SearchSubAccounts("Test"), true);
        }
        #endregion Mapping Tests

        #region Method Tests

        #region SearchKfsAccounts Tests

        [TestMethod]
        public void TestSearchKfsAccountsReturnsExpectedResults1()
        {
            #region Arrange
            Mock.Get(SearchService).Setup(a => a.SearchAccounts("Test")).Returns(new List<IdAndName>());
            #endregion Arrange

            #region Act
            var result = Controller.SearchKfsAccounts("Test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[]", result.JsonResultString);
            #endregion Assert		
        }

        [TestMethod]
        public void TestSearchKfsAccountsReturnsExpectedResults2()
        {
            #region Arrange
            var accounts = new List<IdAndName>();
            for (int i = 0; i < 3; i++)
            {
                accounts.Add(new IdAndName((i+1).ToString(CultureInfo.InvariantCulture), "name"));
            }
            Mock.Get(SearchService).Setup(a => a.SearchAccounts("Test")).Returns(accounts);
            #endregion Arrange           
            
            #region Act
            var result = Controller.SearchKfsAccounts("Test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"Id\":\"1\",\"Name\":\"name (1)\"},{\"Id\":\"2\",\"Name\":\"name (2)\"},{\"Id\":\"3\",\"Name\":\"name (3)\"}]", result.JsonResultString);
            #endregion Assert
        }
        #endregion SearchKfsAccounts Tests

        #region SearchSubAccounts Tests

        [TestMethod]
        public void TestSearchSubAccountsReturnsExpectedResults1()
        {
            #region Arrange
            new FakeSubAccounts(3, SubAccountRepository);
            #endregion Arrange

            #region Act
            var result = Controller.SearchSubAccounts("Test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[]", result.JsonResultString);
            #endregion Assert		
        }

        [TestMethod]
        public void TestSearchSubAccountsReturnsExpectedResults2()
        {
            #region Arrange
            var subAccounts = new List<SubAccount>();
            for (int i = 0; i < 5; i++)
            {
                subAccounts.Add(CreateValidEntities.SubAccount(i+1));
                subAccounts[i].IsActive = true;
                subAccounts[i].AccountNumber = "Test";
            }
            subAccounts[1].IsActive = false;
            subAccounts[2].AccountNumber = "not me";
            subAccounts[3].IsActive = false;
            subAccounts[3].AccountNumber = "xxx";

            new FakeSubAccounts(0, SubAccountRepository, subAccounts, false);
            #endregion Arrange

            #region Act
            var result = Controller.SearchSubAccounts("Test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"Id\":\"Sub1\",\"Name\":\"Sub1\",\"Title\":\"Name1\"},{\"Id\":\"Sub5\",\"Name\":\"Sub5\",\"Title\":\"Name5\"}]", result.JsonResultString);
            #endregion Assert
        }

        #endregion SearchSubAccounts Tests

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
        public void TestControllerMethodSearchKfsAccountsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("SearchKfsAccounts");
            #endregion Arrange

            #region Act           
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodSearchSubAccountsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("SearchSubAccounts");
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
