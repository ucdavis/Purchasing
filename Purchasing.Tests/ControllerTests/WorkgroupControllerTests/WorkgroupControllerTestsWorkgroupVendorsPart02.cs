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
using UCDArch.Testing;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region EditWorkgroupVendor Get Tests

        [TestMethod]
        public void TestEditWorkgroupVendorGetRedirectsWhenNotFound()
        {
            #region Arrange
            new FakeWorkgroupVendors(3, WorkgroupVendorRepository);
            #endregion Arrange

            #region Act
            Controller.EditWorkgroupVendor(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup Vendor not found.", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditWorkgroupVendorGetWhereKfsRedirects()
        {
            #region Arrange
            var workgroupVendors = new List<WorkgroupVendor>();
            workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(1));
            workgroupVendors[0].Workgroup = CreateValidEntities.Workgroup(9);
            workgroupVendors[0].Workgroup.SetIdTo(15);
            new FakeWorkgroupVendors(0, WorkgroupVendorRepository, workgroupVendors);
            #endregion Arrange

            #region Act
            var result = Controller.EditWorkgroupVendor(1)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.VendorList(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.RouteValues["id"]);
            Assert.AreEqual("Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditWorkgroupVendorGetReturnsView()
        {
            #region Arrange
            new FakeWorkgroupVendors(3, WorkgroupVendorRepository);
            #endregion Arrange

            #region Act
            var result = Controller.EditWorkgroupVendor(2)
                .AssertViewRendered()
                .WithViewData<WorkgroupVendorViewModel>();
            #endregion Act

            #region Assert
            #endregion Assert		
        }


        #endregion EditWorkgroupVendor Get Tests
    }
}
