using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using FluentNHibernate.MappingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.ActionResults;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {

        #region BulkVendors

        [TestMethod]
        public void TestBulkVendorGetRedirectsToIndexIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            var result = Controller.BulkVendor(4)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["ShowAll"]);
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestBulkVendorGetRedirectsToDetailsIfAdministrative()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            workgroups.Add(CreateValidEntities.Workgroup(1));
            workgroups[0].Administrative = true;            
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            var result = Controller.BulkVendor(1)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Vendors may not be added to an administrative workgroup.", Controller.ErrorMessage);
            #endregion Assert
        }
        [TestMethod]
        public void TestBulkVendorGetReturnsView()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            workgroups.Add(CreateValidEntities.Workgroup(1));
            workgroups[0].Administrative = false;
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            var result = Controller.BulkVendor(1)
                .AssertViewRendered()
                .WithViewData<Workgroup>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            #endregion Assert
        }
        #endregion BulkVendors

        //Not testing post of BulkVendors

        #region CheckDuplicateVendor Tests

        [TestMethod]
        public void TestCheckDuplicateVendor1()
        {
            #region Arrange
            new FakeWorkgroupVendors(3, WorkgroupVendorRepository);
            #endregion Arrange

            #region Act
            var result = Controller.CheckDuplicateVendor(1, "test", "test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("{\"message\":\"\"}", result.JsonResultString);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCheckDuplicateVendor2()
        {
            #region Arrange
            var workgroupVendors = new List<WorkgroupVendor>();
            workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(1));
            workgroupVendors[0].Workgroup = new Workgroup();
            workgroupVendors[0].Workgroup.SetIdTo(1);
            new FakeWorkgroupVendors(0, WorkgroupVendorRepository, workgroupVendors);
            #endregion Arrange

            #region Act
            var result = Controller.CheckDuplicateVendor(1, "Name1", "Line11")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("{\"message\":\"It appears this vendor has already been added to this workgroup.\"}", result.JsonResultString);
            #endregion Assert
        }
        #endregion CheckDuplicateVendor Tests

        #region GetRequesters Tests

        [TestMethod]
        public void TestGetRequesters()
        {
            #region Arrange
            var workgroupPermissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 6; i++)
            {
                workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i+1));
                workgroupPermissions[i].User = CreateValidEntities.User(i + 1);
                workgroupPermissions[i].User.SetIdTo((i+1).ToString(CultureInfo.InvariantCulture));
                workgroupPermissions[i].User.IsActive = true;
                workgroupPermissions[i].Workgroup = CreateValidEntities.Workgroup(1);
                workgroupPermissions[i].Workgroup.SetIdTo(1);
                workgroupPermissions[i].Role = CreateValidEntities.Role(1);
                workgroupPermissions[i].Role.SetIdTo(Role.Codes.Requester);
            }

            workgroupPermissions[0].User.IsActive = false;
            workgroupPermissions[2].Workgroup.SetIdTo(99);
            workgroupPermissions[3].Role.SetIdTo(Role.Codes.Admin);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            var result = Controller.GetRequesters(1)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"Name\":\"FirstName2 LastName2 (2)\",\"Id\":\"2\"},{\"Name\":\"FirstName5 LastName5 (5)\",\"Id\":\"5\"},{\"Name\":\"FirstName6 LastName6 (6)\",\"Id\":\"6\"}]", result.JsonResultString);
            #endregion Assert		
        }
        #endregion GetRequesters Tests
    }
}
