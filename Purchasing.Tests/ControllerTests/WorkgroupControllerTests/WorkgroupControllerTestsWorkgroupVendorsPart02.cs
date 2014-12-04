using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
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
            Controller.EditWorkgroupVendor(0,4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
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
            new FakeWorkgroupVendors(15, WorkgroupVendorRepository, workgroupVendors);
            #endregion Arrange

            #region Act
            var result = Controller.EditWorkgroupVendor(15, 1)
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
            new FakeVendors(3, VendorRepository);
            var workgroupVendors = new List<WorkgroupVendor>();
            workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(5));
            workgroupVendors[0].VendorId = null;
            workgroupVendors[0].VendorAddressTypeCode = null;
            new FakeWorkgroupVendors(0, WorkgroupVendorRepository, workgroupVendors);
            #endregion Arrange

            #region Act
            var result = Controller.EditWorkgroupVendor(0, 1)
                .AssertViewRendered()
                .WithViewData<WorkgroupVendorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name5", result.WorkgroupVendor.Name);
            #endregion Assert		
        }


        #endregion EditWorkgroupVendor Get Tests

        #region EditWorkgroupVendor Post Tests
        [TestMethod]
        public void TestEditWorkgroupVendorPostRedirectsWhenNotFound()
        {
            #region Arrange
            new FakeWorkgroupVendors(3, WorkgroupVendorRepository);
            #endregion Arrange

            #region Act
            Controller.EditWorkgroupVendor(0, 4, new WorkgroupVendor())
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup Vendor not found.", Controller.ErrorMessage);
            WorkgroupVendorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestEditWorkgroupVendorPostWhereKfsRedirects()
        {
            #region Arrange
            var workgroupVendors = new List<WorkgroupVendor>();
            workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(1));
            workgroupVendors[0].Workgroup = CreateValidEntities.Workgroup(9);
            workgroupVendors[0].Workgroup.SetIdTo(15);
            new FakeWorkgroupVendors(0, WorkgroupVendorRepository, workgroupVendors);
            #endregion Arrange

            #region Act
            var result = Controller.EditWorkgroupVendor(15, 1, new WorkgroupVendor())
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.VendorList(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.RouteValues["id"]);
            Assert.AreEqual("Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.", Controller.ErrorMessage);
            WorkgroupVendorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestEditWorkgroupVendorPostThrowsException1()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeVendors(3, VendorRepository);
                var workgroupVendors = new List<WorkgroupVendor>();
                workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(5));
                workgroupVendors[0].VendorId = null;
                workgroupVendors[0].VendorAddressTypeCode = null;
                new FakeWorkgroupVendors(0, WorkgroupVendorRepository, workgroupVendors);

                var workgroupVendor = CreateValidEntities.WorkgroupVendor(9);
                workgroupVendor.VendorId = "test";
                workgroupVendor.VendorAddressTypeCode = null;

                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.EditWorkgroupVendor(0, 1, workgroupVendor);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Can't have VendorId when editing", ex.Message);
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestEditWorkgroupVendorPostThrowsException2()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeVendors(3, VendorRepository);
                var workgroupVendors = new List<WorkgroupVendor>();
                workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(5));
                workgroupVendors[0].VendorId = null;
                workgroupVendors[0].VendorAddressTypeCode = null;
                new FakeWorkgroupVendors(0, WorkgroupVendorRepository, workgroupVendors);

                var workgroupVendor = CreateValidEntities.WorkgroupVendor(9);
                workgroupVendor.VendorId = " ";
                workgroupVendor.VendorAddressTypeCode = "tc";

                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.EditWorkgroupVendor(0, 1, workgroupVendor);
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Can't have vendorAddresstypeCode when editing", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestEditWorkgroupVendorPostReturnsViewWhenNotValid()
        {
            #region Arrange
            new FakeVendors(3, VendorRepository);
            var workgroupVendors = new List<WorkgroupVendor>();
            workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(5));
            workgroupVendors[0].VendorId = null;
            workgroupVendors[0].VendorAddressTypeCode = null;
            new FakeWorkgroupVendors(0, WorkgroupVendorRepository, workgroupVendors);

            var workgroupVendor = CreateValidEntities.WorkgroupVendor(9);
            workgroupVendor.VendorId = null;
            workgroupVendor.VendorAddressTypeCode = null;
            workgroupVendor.Name = null;
            WorkgroupService.Expect(a => a.TransferValues(Arg<WorkgroupVendor>.Is.Anything, ref Arg<WorkgroupVendor>.Ref(Is.Anything(), workgroupVendor).Dummy));
            #endregion Arrange

            #region Act
            var result = Controller.EditWorkgroupVendor(0, 1, workgroupVendor)
                .AssertViewRendered()
                .WithViewData<WorkgroupVendorViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("The Name field is required.");
            Assert.IsNotNull(result);
            Assert.AreEqual("Line19", result.WorkgroupVendor.Line1);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditWorkgroupVendorPostRedirectsWhenValid()
        {
            #region Arrange
            new FakeVendors(3, VendorRepository);
            var workgroup = CreateValidEntities.Workgroup(15);
            workgroup.SetIdTo(15);

            var workgroupVendors = new List<WorkgroupVendor>();
            workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(5));
            workgroupVendors[0].VendorId = null;
            workgroupVendors[0].VendorAddressTypeCode = null;
            workgroupVendors[0].Workgroup = workgroup;
            new FakeWorkgroupVendors(0, WorkgroupVendorRepository, workgroupVendors);

            var workgroupVendor = CreateValidEntities.WorkgroupVendor(9);
            workgroupVendor.Workgroup.SetIdTo(15);
            workgroupVendor.VendorId = null;
            workgroupVendor.VendorAddressTypeCode = null;
            workgroupVendor.Name = "Changed";
            WorkgroupService.Expect(a => a.TransferValues(Arg<WorkgroupVendor>.Is.Anything, ref Arg<WorkgroupVendor>.Ref(Is.Anything(), workgroupVendor).Dummy));
            #endregion Arrange

            #region Act
            var result = Controller.EditWorkgroupVendor(15, 1, workgroupVendor)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.VendorList(15));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.RouteValues["id"]);

            Assert.AreEqual("WorkgroupVendor Edited Successfully", Controller.Message);

            WorkgroupVendorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything), x => x.Repeat.Times(2));
            var oldArgs = (WorkgroupVendor) WorkgroupVendorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything))[0][0];
            var newArgs = (WorkgroupVendor)WorkgroupVendorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything))[1][0]; 

            Assert.IsNotNull(oldArgs);
            Assert.AreEqual("Name5", oldArgs.Name);
            Assert.IsFalse(oldArgs.IsActive);

            Assert.IsNotNull(newArgs);
            Assert.AreEqual("Changed", newArgs.Name);
            Assert.IsTrue(newArgs.IsActive);
            #endregion Assert
        }
 
        #endregion EditWorkgroupVendor Post Tests
    }
}
