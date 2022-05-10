using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using Moq;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class SecurityServiceTests
    {
        public IRepositoryFactory RepositoryFactory;
        public IQueryRepositoryFactory QueryRepositoryFactory;
        public IAccessQueryService AccessQueryService;
        public IUserIdentity UserIdentity;
        public IDirectorySearchService DirectorySearchService;
        public ISecurityService SecurityService;

        #region Init
        public SecurityServiceTests()
        {
            RepositoryFactory = new Mock<IRepositoryFactory>().Object;
            AccessQueryService = new Mock<IAccessQueryService>().Object;
            QueryRepositoryFactory = new Mock<IQueryRepositoryFactory>().Object;
            UserIdentity = new Mock<IUserIdentity>().Object;
            DirectorySearchService = new Mock<IDirectorySearchService>().Object;

            SecurityService = new SecurityService(RepositoryFactory, UserIdentity, DirectorySearchService, AccessQueryService, QueryRepositoryFactory);

        }
        #endregion Init

        #region HasWorkgroupOrOrganizationAccess Tests


        [TestMethod, Ignore] //This is causing problems when running tests in Release mode.
        public void TestHasWorkgroupOrOrganizationAccessWhenWorkgroupAndOrgAreNull()
        {
            #region Arrange
            var message = string.Empty;
            #endregion Arrange

            #region Act
            var result = SecurityService.HasWorkgroupOrOrganizationAccess(null, null, out message);
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            Assert.AreEqual("Workgroup and Organization not found.", message);
            #endregion Assert		
        }


        [TestMethod]
        public void Test1()
        {
            #region Arrange
            Assert.Inconclusive("Write HasWorkgroupOrOrganizationAccess Tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion HasWorkgroupOrOrganizationAccess Tests

        #region HasWorkgroupAccess Tests
        [TestMethod]
        public void Test2()
        {
            #region Arrange
            Assert.Inconclusive("Write HasWorkgroupAccess Tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion HasWorkgroupAccess Tests


        #region HasWorkgroupEditAccess Tests
        [TestMethod]
        public void Test3()
        {
            #region Arrange
            Assert.Inconclusive("Write HasWorkgroupEditAccess Tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion HasWorkgroupEditAccess Tests

        #region IsInRole Tests
        [TestMethod]
        public void Test4()
        {
            #region Arrange
            Assert.Inconclusive("Write IsInRole Tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion IsInRole Tests

        #region hasWorkgroupRole Tests
        [TestMethod]
        public void Test5()
        {
            #region Arrange
            Assert.Inconclusive("Write hasWorkgroupRole Tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion hasWorkgroupRole Tests

        #region GetAccessLevel Tests
        [TestMethod]
        public void Test6()
        {
            #region Arrange
            Assert.Inconclusive("Write GetAccessLevel Tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion GetAccessLevel Tests

        #region GetUser Tests
        [TestMethod]
        public void Test7()
        {
            #region Arrange
            Assert.Inconclusive("Write GetUser Tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion GetUser Tests




        [TestMethod]
        public void TestOthers()
        {
            #region Arrange
            Assert.Inconclusive("Write tests for UserSecurityService");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
    }
}
