using System;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.Attributes;
using Purchasing.Tests.Core;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Purchasing.Tests.ControllerTests.CustomFieldControllerTests
{
    public partial class CustomFieldControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexRedirectsWhenOrgNotFound()
        {
            #region Arrange
            new FakeOrganizations(3, OrganazationRepository);
            #endregion Arrange

            #region Act
            Controller.Index("4")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Organization not found.", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestIndexRedirectWhenNotAuthorized()
        {
            #region Arrange
            new FakeOrganizations(3, OrganazationRepository);
            object[] args = default;
            Mock.Get(SecurityService).Setup(a =>a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), 
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny)).Returns(false)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = "" });
            #endregion Arrange

            #region Act
            Controller.Index("2")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fail Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), 
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny));
 
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name2", ((Organization)args[1]).Name);
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexReturnsView()
        {
            #region Arrange
            new FakeOrganizations(3, OrganazationRepository);
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny)).Returns(true)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = "" });
            #endregion Arrange

            #region Act
            var result = Controller.Index("2")
                .AssertViewRendered()
                .WithViewData<Organization>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.Name);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny));

            Assert.IsNull(args[0]);
            Assert.AreEqual("Name2", ((Organization)args[1]).Name);
            #endregion Assert
        }
        #endregion Index Tests

        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetRedirectsWhenOrgNotFound()
        {
            #region Arrange
            new FakeOrganizations(3, OrganazationRepository);
            #endregion Arrange

            #region Act
            Controller.Create("4")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Organization not found for custom field.", Controller.Message);
            #endregion Assert			
        }

        [TestMethod]
        public void TestCreateRedirectWhenNotAuthorized()
        {
            #region Arrange
            new FakeOrganizations(3, OrganazationRepository);
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny)).Returns(false)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = "" });
            #endregion Arrange

            #region Act
            Controller.Create("2")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fail Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny));

            Assert.IsNull(args[0]);
            Assert.AreEqual("Name2", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateReturnsView()
        {
            #region Arrange
            new FakeOrganizations(3, OrganazationRepository);
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny)).Returns(true)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = "" });
            #endregion Arrange

            #region Act
            var result = Controller.Create("2")
                .AssertViewRendered()
                .WithViewData<CustomFieldViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.Organization.Name);
            Assert.AreEqual("Name2", result.CustomField.Organization.Name);
            Assert.IsNull(result.CustomField.Name);
            Assert.IsTrue(result.CustomField.IsActive);
            Assert.AreEqual(0, result.CustomField.Rank);
            Assert.IsFalse(result.CustomField.IsRequired);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny));

            Assert.IsNull(args[0]);
            Assert.AreEqual("Name2", ((Organization)args[1]).Name);
            #endregion Assert
        }
        #endregion Create Get Tests

        #region Create Post Tests

        [TestMethod]
        public void TestCreatePosttRedirectsWhenOrgNotFound()
        {
            #region Arrange
            new FakeOrganizations(3, OrganazationRepository);
            #endregion Arrange

            #region Act
            Controller.Create("4", new CustomField())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Organization not found for custom field.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectWhenNotAuthorized()
        {
            #region Arrange
            new FakeOrganizations(3, OrganazationRepository);
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny)).Returns(false)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = "" });
            #endregion Arrange

            #region Act
            Controller.Create("2", new CustomField())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fail Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny));

            Assert.IsNull(args[0]);
            Assert.AreEqual("Name2", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostReturnsViewWhenInvalid()
        {
            #region Arrange
            new FakeOrganizations(3, OrganazationRepository);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny)).Returns(true);
            var customField = new CustomField();
            customField.IsRequired = true;
            customField.Organization = null;
            customField.Rank = 9;
            #endregion Arrange

            #region Act
            var result = Controller.Create("2", customField)
                .AssertViewRendered()
                .WithViewData<CustomFieldViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.CustomField.Organization);
            Assert.AreEqual(9, result.CustomField.Rank);
            Assert.IsTrue(result.CustomField.IsRequired);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny));
            Mock.Get(CustomFieldRepository).Verify(a => a.EnsurePersistent(It.IsAny<CustomField>()), Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenValid()
        {
            #region Arrange
            new FakeOrganizations(3, OrganazationRepository);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(),
                It.IsAny<Organization>(),
                out It.Ref<string>.IsAny)).Returns(true);
            var customField = CreateValidEntities.CustomField(1);
            CustomField args = default;
            Mock.Get( CustomFieldRepository).Setup(a => a.EnsurePersistent(It.IsAny<CustomField>()))
                .Callback<CustomField>(x => args = x);
            #endregion Arrange

            #region Act
            var result = Controller.Create("2", customField)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("2", result.RouteValues["id"]);
            Assert.AreEqual("CustomField Created Successfully", Controller.Message);


            Mock.Get(CustomFieldRepository).Verify(a => a.EnsurePersistent(It.IsAny<CustomField>()));

            Mock.Get(CustomFieldRepository).Verify(a => a.EnsurePersistent(It.IsAny<CustomField>()));
 
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            #endregion Assert
        }
        #endregion Create Post Tests

    }
}
