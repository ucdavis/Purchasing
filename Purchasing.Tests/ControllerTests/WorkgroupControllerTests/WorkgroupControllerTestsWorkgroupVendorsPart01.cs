using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using UCDArch.Testing.Extensions;

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
                .AssertActionRedirect();
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

        #region ExportableVendorList Tests

        [TestMethod]
        public void TestExportableVendorListRedirectsWhenWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.ExportableVendorList(4)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestExportableVendorListReturnsListOfVendowsForWorkgroup1()
        {
            #region Arrange
            SetupDataForVendors1();
            #endregion Arrange

            #region Act
            var result = Controller.ExportableVendorList(2)
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
        public void TestExportableVendorListReturnsListOfVendowsForWorkgroup2()
        {
            #region Arrange
            SetupDataForVendors1();
            #endregion Arrange

            #region Act
            var result = Controller.ExportableVendorList(1)
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
        public void TestExportableVendorListReturnsListOfVendowsForWorkgroup3()
        {
            #region Arrange
            SetupDataForVendors1();
            #endregion Arrange

            #region Act
            var result = Controller.ExportableVendorList(3)
                .AssertViewRendered()
                .WithViewData<List<WorkgroupVendor>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            #endregion Assert
        }

        #endregion ExportableVendorList Tests

        #region CreateVendor Get Tests

        [TestMethod]
        public void TestCreateVendorGetRedirectsWhenWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.CreateVendor(4)
                .AssertActionRedirect();
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
            //new FakeVendors(5, VendorRepository);
            #endregion Arrange

            #region Act
            var result = Controller.CreateVendor(3)
                .AssertViewRendered()
                .WithViewData<WorkgroupVendorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            //Assert.AreEqual(5, result.Vendors.Count());
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
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateVendorPostWhenValidRedirectsAndSaves1()
        {
            #region Arrange
            SetupDataForVendors2();
            var vendorToCreate = CreateValidEntities.WorkgroupVendor(9);

            Moq.Mock.Get(WorkgroupService).Setup(a => a.TransferValues(Moq.It.IsAny<WorkgroupVendor>(), ref Moq.It.Ref<WorkgroupVendor>.IsAny));
            new FakeWorkgroupVendors(0, WorkgroupVendorRepository);
            WorkgroupVendor args = default;
            Moq.Mock.Get( WorkgroupVendorRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()))
                .Callback<WorkgroupVendor>(x => args = x);
            
            #endregion Arrange

            #region Act
            var result = Controller.CreateVendor(3, vendorToCreate, false)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("WorkgroupVendor Created Successfully", Controller.Message);

            Moq.Mock.Get(WorkgroupVendorRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()));
 
            Assert.IsNotNull(args);
            Assert.AreEqual("VendorId9", args.VendorId);
            Assert.AreEqual("tc9", args.VendorAddressTypeCode);
            Assert.AreEqual("Name9", args.Name);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateVendorPostWhenValidRedirectsAndSaves2()
        {
            #region Arrange
            SetupDataForVendors2();
            var vendorToCreate = CreateValidEntities.WorkgroupVendor(9);
            Moq.Mock.Get(WorkgroupService).Setup(a => a.TransferValues(Moq.It.IsAny<WorkgroupVendor>(), ref Moq.It.Ref<WorkgroupVendor>.IsAny));

            new FakeWorkgroupVendors(0, WorkgroupVendorRepository);
            WorkgroupVendor args = default;
            Moq.Mock.Get(WorkgroupVendorRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()))
                .Callback<WorkgroupVendor>(x => args = x);
            #endregion Arrange

            #region Act
            var result = Controller.CreateVendor(3, vendorToCreate, true)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("WorkgroupVendor Created Successfully", Controller.Message);

            Moq.Mock.Get(WorkgroupVendorRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("VendorId9", args.VendorId);
            Assert.AreEqual("tc9", args.VendorAddressTypeCode);
            Assert.AreEqual("Name9", args.Name);

            #endregion Assert
        }

        [TestMethod]
        public void TestCreateVendorPostWhenValidRedirectsAndSaves3()
        {
            #region Arrange
            SetupDataForVendors2();
            var vendorToCreate = CreateValidEntities.WorkgroupVendor(9);
            vendorToCreate.VendorId = null;
            Moq.Mock.Get(WorkgroupService).Setup(a => a.TransferValues(Moq.It.IsAny<WorkgroupVendor>(), ref Moq.It.Ref<WorkgroupVendor>.IsAny));
            WorkgroupVendor args = default;
            Moq.Mock.Get(WorkgroupVendorRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()))
                .Callback<WorkgroupVendor>(x => args = x);
            #endregion Arrange

            #region Act
            var result = Controller.CreateVendor(3, vendorToCreate, false)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("WorkgroupVendor Created Successfully", Controller.Message);

            Moq.Mock.Get(WorkgroupVendorRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args.VendorId);
            Assert.AreEqual("tc9", args.VendorAddressTypeCode);
            Assert.AreEqual("Name9", args.Name);
            Assert.AreEqual("Line19", args.Line1);
            Assert.AreEqual(null, args.Line2);
            Assert.AreEqual(null, args.Line3);
            Assert.AreEqual("City9", args.City);
            Assert.AreEqual("CA", args.State);
            Assert.AreEqual("95616", args.Zip);
            Assert.AreEqual("US", args.CountryCode);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateVendorPostWhenValidRedirectsAndSaves4()
        {
            #region Arrange
            SetupDataForVendors2();
            var vendorToCreate = CreateValidEntities.WorkgroupVendor(9);
            vendorToCreate.VendorId = null;
            Moq.Mock.Get(WorkgroupService).Setup(a => a.TransferValues(Moq.It.IsAny<WorkgroupVendor>(), ref Moq.It.Ref<WorkgroupVendor>.IsAny));
            WorkgroupVendor args = default;
            Moq.Mock.Get(WorkgroupVendorRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()))
                .Callback<WorkgroupVendor>(x => args = x);
            #endregion Arrange

            #region Act
            var result = Controller.CreateVendor(3, vendorToCreate, true)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("WorkgroupVendor Created Successfully", Controller.Message);

            Moq.Mock.Get(WorkgroupVendorRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args.VendorId);
            Assert.AreEqual("tc9", args.VendorAddressTypeCode);
            Assert.AreEqual("Name9", args.Name);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateVendorPostWithInvalidValueReturnsView1()
        {
            #region Arrange
            SetupDataForVendors3();
            var vendorToCreate = CreateValidEntities.WorkgroupVendor(9);
            vendorToCreate.Line1 = null;
            Moq.Mock.Get(WorkgroupService).Setup(a => a.TransferValues(Moq.It.IsAny<WorkgroupVendor>(), ref Moq.It.Ref<WorkgroupVendor>.IsAny));
            #endregion Arrange

            #region Act
            Controller.CreateVendor(3, vendorToCreate, false)
                .AssertViewRendered()
                .WithViewData<WorkgroupVendorViewModel>();
            #endregion Act

            #region Assert
            Moq.Mock.Get(WorkgroupVendorRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()), Moq.Times.Never());
            Controller.ModelState.AssertErrorsAre("The Line1 field is required.");
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateVendorPostWithInvalidValueReturnsView3()
        {
            #region Arrange
            SetupDataForVendors3();
            var vendorToCreate = CreateValidEntities.WorkgroupVendor(9);
            vendorToCreate.VendorId = null;
            vendorToCreate.City = null;
            Moq.Mock.Get(WorkgroupService).Setup(a => a.TransferValues(Moq.It.IsAny<WorkgroupVendor>(), ref Moq.It.Ref<WorkgroupVendor>.IsAny));
            #endregion Arrange

            #region Act
            var result = Controller.CreateVendor(3, vendorToCreate, false)
                .AssertViewRendered()
                .WithViewData<WorkgroupVendorViewModel>();
            #endregion Act

            #region Assert
            Moq.Mock.Get(WorkgroupVendorRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()), Moq.Times.Never());
            Controller.ModelState.AssertErrorsAre("The City field is required.");

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.NewVendor);
            Assert.AreEqual(null, result.VendorAddress);
            Assert.AreEqual(null, result.WorkgroupVendor.City);
            Assert.AreEqual("Line19", result.WorkgroupVendor.Line1);
            Assert.AreEqual(null, result.Vendor);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateVendorPostWithInvalidValueReturnsView4()
        {
            #region Arrange
            SetupDataForVendors3();
            var vendorToCreate = CreateValidEntities.WorkgroupVendor(9);
            vendorToCreate.VendorId = null;
            vendorToCreate.City = null;
            Moq.Mock.Get(WorkgroupService).Setup(a => a.TransferValues(Moq.It.IsAny<WorkgroupVendor>(), ref Moq.It.Ref<WorkgroupVendor>.IsAny));
            #endregion Arrange

            #region Act
            var result = Controller.CreateVendor(3, vendorToCreate, true)
                .AssertViewRendered()
                .WithViewData<WorkgroupVendorViewModel>();
            #endregion Act

            #region Assert
            Moq.Mock.Get(WorkgroupVendorRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<WorkgroupVendor>()), Moq.Times.Never());
            Controller.ModelState.AssertErrorsAre("The City field is required.");

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.NewVendor);
            Assert.AreEqual(null, result.VendorAddress);
            Assert.AreEqual(null, result.WorkgroupVendor.City);
            Assert.AreEqual("Line19", result.WorkgroupVendor.Line1);
            Assert.AreEqual(null, result.Vendor);
            #endregion Assert
        }
        #endregion CreateVendor Post Tests
    }
}
