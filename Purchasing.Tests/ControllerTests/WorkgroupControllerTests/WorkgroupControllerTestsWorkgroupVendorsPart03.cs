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
using UCDArch.Testing;
using UCDArch.Web.ActionResults;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {

        #region DeleteWorkgroupVendor Get Tests
        
        [TestMethod]
        public void TestDeleteWorkgroupVendorGetRedirectsToIndesWhenNotFound()
        {
            #region Arrange
            new FakeWorkgroupVendors(3, WorkgroupVendorRepository);
            #endregion Arrange

            #region Act
            Controller.DeleteWorkgroupVendor(0,4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            WorkgroupVendorRepository.AssertWasNotCalled(a => a.Remove(Arg<WorkgroupVendor>.Is.Anything));
            WorkgroupVendorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestDeleteWorkgroupVendorGetReturnView()
        {
            #region Arrange
            new FakeWorkgroupVendors(3, WorkgroupVendorRepository);
            #endregion Arrange

            #region Act
            var result = Controller.DeleteWorkgroupVendor(0, 3)
                .AssertViewRendered()
                .WithViewData<WorkgroupVendor>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Id);
            WorkgroupVendorRepository.AssertWasNotCalled(a => a.Remove(Arg<WorkgroupVendor>.Is.Anything));
            WorkgroupVendorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything));
            #endregion Assert		
        }
        #endregion DeleteWorkgroupVendor Get Tests

        #region DeleteWorkgroupVendor Post Tests
        [TestMethod]
        public void TestDeleteWorkgroupVendorPostRedirectsToIndesWhenNotFound()
        {
            #region Arrange
            new FakeWorkgroupVendors(3, WorkgroupVendorRepository);
            #endregion Arrange

            #region Act
            Controller.DeleteWorkgroupVendor(0, 4, null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("WorkgroupVendor not found.", Controller.ErrorMessage);
            WorkgroupVendorRepository.AssertWasNotCalled(a => a.Remove(Arg<WorkgroupVendor>.Is.Anything));
            WorkgroupVendorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestDeleteWorkgroupVendorWhenFound1()
        {
            #region Arrange
            new FakeWorkgroups(4, WorkgroupRepository);
            var workgroupVendors = new List<WorkgroupVendor>();
            for (int i = 0; i < 3; i++)
            {
                workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(i+1));
                workgroupVendors[i].IsActive = true;
                workgroupVendors[i].Workgroup = WorkgroupRepository.GetNullableById(i + 2);

            }
            workgroupVendors[2].IsActive = false;
            new FakeWorkgroupVendors(3, WorkgroupVendorRepository, workgroupVendors);
            #endregion Arrange

            #region Act
            var result = Controller.DeleteWorkgroupVendor(3, 2, null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.VendorList(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("WorkgroupVendor Removed Successfully", Controller.Message);

            WorkgroupVendorRepository.AssertWasNotCalled(a => a.Remove(Arg<WorkgroupVendor>.Is.Anything));
            WorkgroupVendorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything));
            var args = (WorkgroupVendor) WorkgroupVendorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(2, args.Id);
            Assert.IsFalse(args.IsActive);
            #endregion Assert		
        }


        [TestMethod]
        public void TestDeleteWorkgroupVendorWhenFound2()
        {
            #region Arrange
            new FakeWorkgroups(4, WorkgroupRepository);
            var workgroupVendors = new List<WorkgroupVendor>();
            for(int i = 0; i < 3; i++)
            {
                workgroupVendors.Add(CreateValidEntities.WorkgroupVendor(i + 1));
                workgroupVendors[i].IsActive = true;
                workgroupVendors[i].Workgroup = WorkgroupRepository.GetNullableById(i + 2);

            }
            workgroupVendors[2].IsActive = false;
            new FakeWorkgroupVendors(3, WorkgroupVendorRepository, workgroupVendors);
            #endregion Arrange

            #region Act
            var result = Controller.DeleteWorkgroupVendor(4, 3, null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.VendorList(4));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.RouteValues["id"]);
            Assert.AreEqual("WorkgroupVendor Removed Successfully", Controller.Message);

            WorkgroupVendorRepository.AssertWasNotCalled(a => a.Remove(Arg<WorkgroupVendor>.Is.Anything));
            WorkgroupVendorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything));
            var args = (WorkgroupVendor)WorkgroupVendorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupVendor>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Id);
            Assert.IsFalse(args.IsActive);
            #endregion Assert
        }
        #endregion DeleteWorkgroupVendor Post Tests

        #region GetVendorAddresses Tests

        [TestMethod]
        public void TestGetVendorAddressesReturnsExpectedResults1()
        {
            #region Arrange
            new FakeVendors(5, VendorRepository);
            var vendorAddresses = new List<VendorAddress>();
            for (int i = 0; i < 5; i++)
            {
                vendorAddresses.Add(CreateValidEntities.VendorAddress(i+1));
                if(i < 3)
                {
                    vendorAddresses[i].Vendor = VendorRepository.GetNullableById("1");
                }
                else
                {
                    vendorAddresses[i].Vendor = VendorRepository.GetNullableById((i+1).ToString());
                }
            }
            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddresses, false);
            #endregion Arrange

            #region Act
            var result = Controller.GetVendorAddresses("1")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"TypeCode\":\"tc1\",\"Name\":\"(tc1) Line11, City1, XX 12345, AA\"},{\"TypeCode\":\"tc2\",\"Name\":\"(tc2) Line12, City2, XX 12345, AA\"},{\"TypeCode\":\"tc3\",\"Name\":\"(tc3) Line13, City3, XX 12345, AA\"}]", result.JsonResultString);
            #endregion Assert		
        }

        [TestMethod]
        public void TestGetVendorAddressesReturnsExpectedResults2()
        {
            #region Arrange
            new FakeVendors(5, VendorRepository);
            var vendorAddresses = new List<VendorAddress>();
            for(int i = 0; i < 5; i++)
            {
                vendorAddresses.Add(CreateValidEntities.VendorAddress(i + 1));
                if(i < 3)
                {
                    vendorAddresses[i].Vendor = VendorRepository.GetNullableById("1");
                }
                else
                {
                    vendorAddresses[i].Vendor = VendorRepository.GetNullableById((i + 1).ToString());
                }
            }
            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddresses, false);
            #endregion Arrange

            #region Act
            var result = Controller.GetVendorAddresses("4")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"TypeCode\":\"tc4\",\"Name\":\"Name4 (Line14, City4, XX 12345)\"}]", result.JsonResultString);
            #endregion Assert
        }

        [TestMethod]
        public void TestGetVendorAddressesReturnsExpectedResults3()
        {
            #region Arrange
            new FakeVendors(5, VendorRepository);
            var vendorAddresses = new List<VendorAddress>();
            for(int i = 0; i < 5; i++)
            {
                vendorAddresses.Add(CreateValidEntities.VendorAddress(i + 1));
                if(i < 3)
                {
                    vendorAddresses[i].Vendor = VendorRepository.GetNullableById("1");
                }
                else
                {
                    vendorAddresses[i].Vendor = VendorRepository.GetNullableById((i + 1).ToString());
                }
            }
            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddresses, false);
            #endregion Arrange

            #region Act
            var result = Controller.GetVendorAddresses("5")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[{\"TypeCode\":\"tc5\",\"Name\":\"Name5 (Line15, City5, XX 12345)\"}]", result.JsonResultString);
            #endregion Assert
        }
        #endregion GetVendorAddresses Tests
    }
}
