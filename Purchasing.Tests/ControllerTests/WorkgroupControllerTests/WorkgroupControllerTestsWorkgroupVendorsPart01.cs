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
        #region VendorList Tests

        [TestMethod]
        public void TestVendorListRedirectsWhenWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.VendorList(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestVendorListReturnsListOfVendowsForWorkgroup1()
        {
            #region Arrange
            SetupDataForVendors1();
            #endregion Arrange

            #region Act
            var result = Controller.VendorList(2)
                .AssertViewRendered()
                .WithViewData<List<WorkgroupVendor>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("Name4", result[0].Name);
            Assert.AreEqual("Name5", result[1].Name);
            Assert.AreEqual("Name6", result[2].Name);
            #endregion Assert		
        }

        [TestMethod]
        public void TestVendorListReturnsListOfVendowsForWorkgroup2()
        {
            #region Arrange
            SetupDataForVendors1();
            #endregion Arrange

            #region Act
            var result = Controller.VendorList(1)
                .AssertViewRendered()
                .WithViewData<List<WorkgroupVendor>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Name1", result[0].Name);
            Assert.AreEqual("Name3", result[1].Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestVendorListReturnsListOfVendowsForWorkgroup3()
        {
            #region Arrange
            SetupDataForVendors1();
            #endregion Arrange

            #region Act
            var result = Controller.VendorList(3)
                .AssertViewRendered()
                .WithViewData<List<WorkgroupVendor>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            #endregion Assert
        }

        #endregion VendorList Tests
    }
}
