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

        #region CreateVendor Get Tests

        [TestMethod]
        public void TestCreateVendorGetRedirectsWhenWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.CreateVendor(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestCreateVendorGetReturnsView()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeVendors(5, VendorRepository);
            #endregion Arrange

            #region Act
            var result = Controller.CreateVendor(3)
                .AssertViewRendered()
                .WithViewData<WorkgroupVendorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Vendors.Count());
            #endregion Assert		
        }
        #endregion CreateVendor Get Tests

        #region CreateVendor Post Tests
        [TestMethod]
        public void TestCreateVendorPostRedirectsWhenWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.CreateVendor(4, new WorkgroupVendor(), false)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateVendorPostWhenValidRedirectsAndSaves1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(9));
            vendors[0].SetIdTo("VendorId9");
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddresses = new List<VendorAddress>();
            for (int i = 0; i < 3; i++)
            {
                var vendorAddress = CreateValidEntities.VendorAddress(i + 1);
                vendorAddress.Vendor = VendorRepository.GetNullableById("VendorId9");
                vendorAddresses.Add(vendorAddress);
            }

            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddresses, false);
            var vendorToCreate = CreateValidEntities.WorkgroupVendor(9);
            #endregion Arrange

            #region Act
            var result = Controller.CreateVendor(3, vendorToCreate, false)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.VendorList(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("WorkgroupVendor Created Successfully", Controller.Message);

            WorkgroupVendorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything));
            var args = (WorkgroupVendor) WorkgroupVendorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("VendorId9", args.VendorId);
            #endregion Assert		
        }
        #endregion CreateVendor Post Tests
    }
}
