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
            RepositoryFactory = Mock.Of<IRepositoryFactory>();
            AccessQueryService = Mock.Of<IAccessQueryService>();
            QueryRepositoryFactory = Mock.Of<IQueryRepositoryFactory>();
            UserIdentity = Mock.Of<IUserIdentity>();
            DirectorySearchService = Mock.Of<IDirectorySearchService>();

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


        [Ignore("Write HasWorkgroupOrOrganizationAccess Tests")]
        public void Test1()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion HasWorkgroupOrOrganizationAccess Tests

        #region HasWorkgroupAccess Tests
        [Ignore("Write HasWorkgroupAccess Tests")]
        public void Test2()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion HasWorkgroupAccess Tests


        #region HasWorkgroupEditAccess Tests
        [Ignore("Write HasWorkgroupEditAccess Tests")]
        public void Test3()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion HasWorkgroupEditAccess Tests

        #region IsInRole Tests
        [Ignore("Write IsInRole Tests")]
        public void Test4()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion IsInRole Tests

        #region hasWorkgroupRole Tests
        [Ignore("Write hasWorkgroupRole Tests")]
        public void Test5()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion hasWorkgroupRole Tests

        #region GetAccessLevel Tests
        [Ignore("Write GetAccessLevel Tests")]
        public void Test6()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion GetAccessLevel Tests

        #region GetUser Tests
        [Ignore("Write GetUser Tests")]
        public void Test7()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert
        }
        #endregion GetUser Tests




        [Ignore("Write tests for UserSecurityService")]
        public void TestOthers()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
    }
}
