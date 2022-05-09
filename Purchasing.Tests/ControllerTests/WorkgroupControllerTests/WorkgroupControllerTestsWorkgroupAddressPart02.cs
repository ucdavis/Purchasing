using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Testing.Extensions;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region DeleteAddress Get

        [TestMethod]
        public void TestDeleteAddressGetRedirectsIfWorkgroupIdNotFound()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            Controller.DeleteAddress(4, 6)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeleteAddressGetRedirectsIfAddressIsNotInWorkgroup()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.DeleteAddress(3, 6)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("Address not found.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteAddressGetReturnsView1()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.DeleteAddress(2, 4)
                .AssertViewRendered()
                .WithViewData<WorkgroupAddressViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.State.Name);
            Assert.IsNull(result.States);
            Assert.AreEqual("Name4", result.WorkgroupAddress.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteAddressGetReturnsView2()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.DeleteAddress(2, 5)
                .AssertViewRendered()
                .WithViewData<WorkgroupAddressViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.State.Name);
            Assert.IsNull(result.States);
            Assert.AreEqual("Name5", result.WorkgroupAddress.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteAddressGetReturnsView3()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.DeleteAddress(2, 6)
                .AssertViewRendered()
                .WithViewData<WorkgroupAddressViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.State.Name);
            Assert.IsNull(result.States);
            Assert.AreEqual("Name6", result.WorkgroupAddress.Name);
            #endregion Assert
        }
        #endregion DeleteAddress Get

        #region Delete Address Post Tests

        [TestMethod]
        public void TestDeleteAddressPostRedirectsIfWorkgroupIdNotFound()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            Controller.DeleteAddress(4, 6, new WorkgroupAddress())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            Moq.Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteAddressPostRedirectsIfAddressIsNotInWorkgroup()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.DeleteAddress(3, 6, new WorkgroupAddress())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("Address not found.", Controller.ErrorMessage);
            Moq.Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()), Moq.Times.Never());
            #endregion Assert
        }
        
        [TestMethod]
        public void TestDeleteAddressPostRegdirectsAndSaves1()
        {
            #region Arrange
            SetupDataForAddress();
            Workgroup args = default;
            Moq.Mock.Get( WorkgroupRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()))
                .Callback<Workgroup>(x => args = x);
            #endregion Arrange

            #region Act
            var result = Controller.DeleteAddress(2, 4, new WorkgroupAddress())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Address deleted.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Moq.Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()));
 
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Addresses.Count());
            Assert.IsFalse(args.Addresses[0].IsActive);
            Assert.IsFalse(args.Addresses[1].IsActive);
            Assert.IsTrue(args.Addresses[2].IsActive);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeleteAddressPostRegdirectsAndSaves2()
        {
            #region Arrange
            SetupDataForAddress();
            Workgroup args = default;
            Moq.Mock.Get(WorkgroupRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()))
                .Callback<Workgroup>(x => args = x);
            #endregion Arrange

            #region Act
            var result = Controller.DeleteAddress(2, 5, new WorkgroupAddress())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Address deleted.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Moq.Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Addresses.Count());
            Assert.IsTrue(args.Addresses[0].IsActive);
            Assert.IsFalse(args.Addresses[1].IsActive);
            Assert.IsTrue(args.Addresses[2].IsActive);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteAddressPostRegdirectsAndSaves3()
        {
            #region Arrange
            SetupDataForAddress();
            Workgroup args = default;
            Moq.Mock.Get(WorkgroupRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()))
                .Callback<Workgroup>(x => args = x);
            #endregion Arrange

            #region Act
            var result = Controller.DeleteAddress(2, 6, new WorkgroupAddress())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Address deleted.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Moq.Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Addresses.Count());
            Assert.IsTrue(args.Addresses[0].IsActive);
            Assert.IsFalse(args.Addresses[1].IsActive);
            Assert.IsFalse(args.Addresses[2].IsActive);
            #endregion Assert
        }
        #endregion Delete Address Post Tests
    }
}
