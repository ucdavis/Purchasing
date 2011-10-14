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
        #region AddressDetails Tests
        [TestMethod]
        public void TestAddressDetailsRedirectsIfWorkgroupIdNotFound()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            Controller.AddressDetails(4, 6)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddressDetailsRedirectsIfAddressIsNotInWorkgroup()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.AddressDetails(3, 6)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Addresses(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("Address not found.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddressDetailsReturnsView1()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.AddressDetails(2, 4)
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
        public void TestAddressDetailsReturnsView2()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.AddressDetails(2, 5)
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
        public void TestAddressDetailsReturnsView3()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.AddressDetails(2, 6)
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
        #endregion AddressDetails Tests

        #region EditAddress Get Tests

        [TestMethod]
        public void TestEditAddressGetRedirectsIfWorkgroupIdNotFound()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            Controller.EditAddress(4, 6)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditAddressGetRedirectsIfAddressIsNotInWorkgroup()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.EditAddress(3, 6)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Addresses(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("Address not found.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditAddressGetReturnsView1()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.EditAddress(2, 4)
                .AssertViewRendered()
                .WithViewData<WorkgroupAddressViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.States.Count());
            Assert.AreEqual("Name4", result.WorkgroupAddress.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditAddressGetReturnsView2()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.EditAddress(2, 5)
                .AssertViewRendered()
                .WithViewData<WorkgroupAddressViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.States.Count());
            Assert.AreEqual("Name5", result.WorkgroupAddress.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditAddressGetReturnsView3()
        {
            #region Arrange
            SetupDataForAddress();
            #endregion Arrange

            #region Act
            var result = Controller.EditAddress(2, 6)
                .AssertViewRendered()
                .WithViewData<WorkgroupAddressViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.States.Count());
            Assert.AreEqual("Name6", result.WorkgroupAddress.Name);
            #endregion Assert
        }
        #endregion EditAddress Get Tests
    }
}
