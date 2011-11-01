using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using FluentNHibernate.MappingModel;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;


namespace Purchasing.Tests.ControllerTests.ConditionalApprovalControllerTests
{
    public partial class ConditionalApprovalControllerTests
    {

        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "2");            
            SetupDateForIndex1();
            #endregion Arrange

            #region Act
            var results = Controller.Index()
                .AssertViewRendered()
                .WithViewData<ConditionalApprovalIndexModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.ConditionalApprovalsForWorkgroups.Count());
            Assert.AreEqual(3, results.ConditionalApprovalsForWorkgroups[0].Workgroup.Id);
            Assert.AreEqual(4, results.ConditionalApprovalsForWorkgroups[1].Workgroup.Id);
            Assert.AreEqual(6, results.ConditionalApprovalsForWorkgroups[2].Workgroup.Id);
            
            Assert.AreEqual(4, results.ConditionalApprovalsForOrgs.Count());
            Assert.AreEqual("5", results.ConditionalApprovalsForOrgs[0].Organization.Id);
            Assert.AreEqual("6", results.ConditionalApprovalsForOrgs[1].Organization.Id);
            Assert.AreEqual("7", results.ConditionalApprovalsForOrgs[2].Organization.Id);
            Assert.AreEqual("8", results.ConditionalApprovalsForOrgs[3].Organization.Id);
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDateForIndex1();
            #endregion Arrange

            #region Act
            var results = Controller.Index()
                .AssertViewRendered()
                .WithViewData<ConditionalApprovalIndexModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.ConditionalApprovalsForWorkgroups.Count());
            Assert.AreEqual(1, results.ConditionalApprovalsForWorkgroups[0].Workgroup.Id);
            Assert.AreEqual(2, results.ConditionalApprovalsForWorkgroups[1].Workgroup.Id);

            Assert.AreEqual(4, results.ConditionalApprovalsForOrgs.Count());
            Assert.AreEqual("1", results.ConditionalApprovalsForOrgs[0].Organization.Id);
            Assert.AreEqual("2", results.ConditionalApprovalsForOrgs[1].Organization.Id);
            Assert.AreEqual("3", results.ConditionalApprovalsForOrgs[2].Organization.Id);
            Assert.AreEqual("4", results.ConditionalApprovalsForOrgs[3].Organization.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDateForIndex1();
            #endregion Arrange

            #region Act
            var results = Controller.Index()
                .AssertViewRendered()
                .WithViewData<ConditionalApprovalIndexModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.ConditionalApprovalsForWorkgroups.Count());
            Assert.AreEqual(5, results.ConditionalApprovalsForWorkgroups[0].Workgroup.Id);
            Assert.AreEqual(6, results.ConditionalApprovalsForWorkgroups[1].Workgroup.Id);

            Assert.AreEqual(4, results.ConditionalApprovalsForOrgs.Count());
            Assert.AreEqual("9", results.ConditionalApprovalsForOrgs[0].Organization.Id);
            Assert.AreEqual("10", results.ConditionalApprovalsForOrgs[1].Organization.Id);
            Assert.AreEqual("11", results.ConditionalApprovalsForOrgs[2].Organization.Id);
            Assert.AreEqual("12", results.ConditionalApprovalsForOrgs[3].Organization.Id);
            #endregion Assert
        }

        
        #endregion Index Tests
    }
}
