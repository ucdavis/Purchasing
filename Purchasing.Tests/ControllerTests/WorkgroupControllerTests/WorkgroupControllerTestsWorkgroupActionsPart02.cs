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
using UCDArch.Testing.Fakes;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region Details Tests

        [TestMethod]
        public void TestDetailsRedirectsToIndexIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.Details(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestDetailsReturnsView1()
        {
            #region Arrange
            SetupDataForDetails();
            #endregion Arrange

            #region Act
            var result = Controller.Details(3)
                .AssertViewRendered()
                .WithViewData<WorkgroupDetailsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.AccountCount);
            Assert.AreEqual(4, result.AddressCount);
            Assert.AreEqual(2, result.OrganizationCount);
            Assert.AreEqual(1, result.VendorCount);
            Assert.AreEqual(2, result.ApproverCount);
            Assert.AreEqual(3, result.AccountManagerCount);
            Assert.AreEqual(4, result.PurchaserCount);
            Assert.AreEqual(1, result.UserCount);
            #endregion Assert		
        }
        #endregion Details Tests
    }
}
