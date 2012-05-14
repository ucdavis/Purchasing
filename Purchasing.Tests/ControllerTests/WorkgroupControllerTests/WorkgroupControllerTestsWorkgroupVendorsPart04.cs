using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.MappingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Web.ActionResults;


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
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
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
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Details(1));
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
    }
}
