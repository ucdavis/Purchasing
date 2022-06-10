using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
            Controller.EditWorkgroupVendor(0, 4)
                .AssertActionRedirect();
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
            workgroupVendors[0].Workgroup.Id = 15;
            new FakeWorkgroupVendors(15, WorkgroupVendorRepository, workgroupVendors);
            #endregion Arrange

            #region Act
            var result = Controller.EditWorkgroupVendor(15, 1)
                .AssertActionRedirect();
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
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup Vendor not found.", Controller.ErrorMessage);
            Mock.Get(WorkgroupVendorRepository).Verify(a => a.EnsurePersistent(It.IsAny<WorkgroupVendor>()), Times.Never());
            #endregion Assert
        }


        [TestMethod]
        public void TestEditWorkgroupVendorPostWhereKfsRedirects()
        {
            #region Arrange
            var workgroupVendors = new List<WorkgroupVendor>();
            workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(1));
            workgroupVendors[0].Workgroup = CreateValidEntities.Workgroup(9);
            workgroupVendors[0].Workgroup.Id = 15;
            new FakeWorkgroupVendors(0, WorkgroupVendorRepository, workgroupVendors);
            #endregion Arrange

            #region Act
            var result = Controller.EditWorkgroupVendor(15, 1, new WorkgroupVendor())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.RouteValues["id"]);
            Assert.AreEqual("Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.", Controller.ErrorMessage);
            Mock.Get(WorkgroupVendorRepository).Verify(a => a.EnsurePersistent(It.IsAny<WorkgroupVendor>()), Times.Never());
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
            catch (Exception ex)
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
            Mock.Get(WorkgroupService).Setup(a => a.TransferValues(It.IsAny<WorkgroupVendor>(), ref It.Ref<WorkgroupVendor>.IsAny))
                .Callback((WorkgroupVendor source, ref WorkgroupVendor destination) => {
                    destination = workgroupVendor;
                });
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
            workgroup.Id = 15;

            var workgroupVendors = new List<WorkgroupVendor>();
            workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(5));
            workgroupVendors[0].VendorId = null;
            workgroupVendors[0].VendorAddressTypeCode = null;
            workgroupVendors[0].Workgroup = workgroup;
            new FakeWorkgroupVendors(0, WorkgroupVendorRepository, workgroupVendors);

            var workgroupVendor = CreateValidEntities.WorkgroupVendor(9);
            workgroupVendor.Workgroup.Id = 15;
            workgroupVendor.VendorId = null;
            workgroupVendor.VendorAddressTypeCode = null;
            workgroupVendor.Name = "Changed";
            Mock.Get(WorkgroupService).Setup(a => a.TransferValues(It.IsAny<WorkgroupVendor>(), ref It.Ref<WorkgroupVendor>.IsAny))
                .Callback((WorkgroupVendor source, ref WorkgroupVendor destination) => {
                    destination = workgroupVendor;
                });
            WorkgroupVendor oldArgs = default;
            WorkgroupVendor newArgs = default;
            var ensurePersistentCount = 0;
            Mock.Get(WorkgroupVendorRepository).Setup(a => a.EnsurePersistent(It.IsAny<WorkgroupVendor>()))
                .Callback<WorkgroupVendor>(x =>
                {
                    if (ensurePersistentCount++ == 0)
                    { oldArgs = x; }
                    else
                    { newArgs = x; }
                });
            //ENDTODO
            #endregion Arrange

            #region Act
            var result = Controller.EditWorkgroupVendor(15, 1, workgroupVendor)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.RouteValues["id"]);

            Assert.AreEqual("WorkgroupVendor Edited Successfully", Controller.Message);

            Mock.Get(WorkgroupVendorRepository).Verify(a => a.EnsurePersistent(It.IsAny<WorkgroupVendor>()), Times.Exactly(2));



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
