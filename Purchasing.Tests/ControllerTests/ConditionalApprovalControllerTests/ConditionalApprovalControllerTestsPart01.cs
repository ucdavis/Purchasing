using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Purchasing.Tests.ControllerTests.ConditionalApprovalControllerTests
{
    public partial class ConditionalApprovalControllerTests
    {
        #region ByOrg Tests

        [TestMethod]
        public void TestByOrgReturnsView1()
        {
            #region Arrange
            var org1 = CreateValidEntities.Organization(1);
            var org2 = CreateValidEntities.Organization(2);
            org1.Id = "test";
            org2.Id = "test2";
            var conditionalApprovals = new List<ConditionalApproval>();
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(1));
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(2));
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(3));
            conditionalApprovals[0].Organization = org1;
            conditionalApprovals[1].Organization = org2;
            conditionalApprovals[2].Organization = org1;

            new FakeConditionalApprovals(0, ConditionalApprovalRepository, conditionalApprovals);
            #endregion Arrange

            #region Act
            var result = Controller.ByOrg("test")
                .AssertViewRendered()
                .WithViewData<IQueryable<ConditionalApproval>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("test", Controller.ViewBag.OrganizationId);
            #endregion Assert		
        }

        [TestMethod]
        public void TestByOrgReturnsView2()
        {
            #region Arrange
            var org1 = CreateValidEntities.Organization(1);
            var org2 = CreateValidEntities.Organization(2);
            org1.Id = "test";
            org2.Id = "test2";
            var conditionalApprovals = new List<ConditionalApproval>();
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(1));
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(2));
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(3));
            conditionalApprovals[0].Organization = org1;
            conditionalApprovals[1].Organization = org2;
            conditionalApprovals[2].Organization = org1;

            new FakeConditionalApprovals(0, ConditionalApprovalRepository, conditionalApprovals);
            #endregion Arrange

            #region Act
            var result = Controller.ByOrg("test2")
                .AssertViewRendered()
                .WithViewData<IQueryable<ConditionalApproval>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("test2", Controller.ViewBag.OrganizationId);
            #endregion Assert
        }
        #endregion ByOrg Tests

        #region ByWorkgroup Tests

        [TestMethod]
        public void TestByWorkgroupReturnsView1()
        {
            #region Arrange
            var wg1 = CreateValidEntities.Workgroup(1);
            var wg2 = CreateValidEntities.Workgroup(2);
            wg1.Id = 2;
            wg2.Id = 3;
            var conditionalApprovals = new List<ConditionalApproval>();
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(1));
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(2));
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(3));
            conditionalApprovals[0].Workgroup = wg1;
            conditionalApprovals[1].Workgroup = wg2;
            conditionalApprovals[2].Workgroup = wg1;

            new FakeConditionalApprovals(0, ConditionalApprovalRepository, conditionalApprovals);
            #endregion Arrange

            #region Act
            var result = Controller.ByWorkgroup(2)
                .AssertViewRendered()
                .WithViewData<IQueryable<ConditionalApproval>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, Controller.ViewBag.WorkgroupId);
            #endregion Assert
        }

        [TestMethod]
        public void TestByWorkgroupReturnsView2()
        {
            #region Arrange
            var wg1 = CreateValidEntities.Workgroup(1);
            var wg2 = CreateValidEntities.Workgroup(2);
            wg1.Id = 2;
            wg2.Id = 3;
            var conditionalApprovals = new List<ConditionalApproval>();
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(1));
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(2));
            conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(3));
            conditionalApprovals[0].Workgroup = wg1;
            conditionalApprovals[1].Workgroup = wg2;
            conditionalApprovals[2].Workgroup = wg1;

            new FakeConditionalApprovals(0, ConditionalApprovalRepository, conditionalApprovals);
            #endregion Arrange

            #region Act
            var result = Controller.ByWorkgroup(3)
                .AssertViewRendered()
                .WithViewData<IQueryable<ConditionalApproval>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(3, Controller.ViewBag.WorkgroupId);
            #endregion Assert
        }
        #endregion ByWorkgroup Tests

        #region Delete Get Tests

        [TestMethod]
        public void TestDeleteGetRedirectsWhenConditionalApprovalNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "2");
            SetupDateForIndex1();
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny)).Returns(false);            
            #endregion Arrange

            #region Act
            Controller.Delete(19)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.Remove(It.IsAny<ConditionalApproval>()), Times.Never());
            Assert.AreEqual("Conditional Approval not found", Controller.ErrorMessage);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny), Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteGetRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var message = "Fake Message";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(false)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.Remove(It.IsAny<ConditionalApproval>()), Times.Never());
            Assert.AreEqual("Fake Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name1", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteGetRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var message = "Fake Message";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(false)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            #endregion Arrange

            #region Act
            Controller.Delete(7)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.Remove(It.IsAny<ConditionalApproval>()), Times.Never());
            Assert.AreEqual("Fake Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);            
            #endregion Assert
        }


        [TestMethod]
        public void TestDeleteGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDateForIndex1();
            var message = "Fake Message";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            #endregion Arrange

            #region Act
            var result = Controller.Delete(1)
                .AssertViewRendered()
                .WithViewData<ConditionalApprovalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Question1", result.Question);
            Assert.AreEqual("WName1", result.OrgOrWorkgroupName);
            Assert.AreEqual("FirstName99 LastName99 (99)", result.PrimaryUserName);
            Assert.AreEqual("", result.SecondaryUserName);
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.Remove(It.IsAny<ConditionalApproval>()), Times.Never());

            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name1", ((Organization)args[1]).Name);
            #endregion Assert		
        }


        [TestMethod]
        public void TestDeleteGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDateForIndex1();
            var message = "Fake Message";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            #endregion Arrange

            #region Act
            var result = Controller.Delete(7)
                .AssertViewRendered()
                .WithViewData<ConditionalApprovalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.Id);
            Assert.AreEqual("Question7", result.Question);
            Assert.AreEqual("OName1", result.OrgOrWorkgroupName);
            Assert.AreEqual("FirstName99 LastName99 (99)", result.PrimaryUserName);
            Assert.AreEqual("FirstName88 LastName88 (88)", result.SecondaryUserName);
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.Remove(It.IsAny<ConditionalApproval>()), Times.Never());

            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);            
            #endregion Assert
        }
        #endregion Delete Get Tests

        #region Delete Post Tests
        [TestMethod]
        public void TestDeletePostRedirectsToIndexInConditionalApprovalNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 19;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny)).Returns(false);
            #endregion Arrange

            #region Act
            Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.Remove(It.IsAny<ConditionalApproval>()), Times.Never());
            Assert.AreEqual("Conditional Approval not found", Controller.ErrorMessage);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny), Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 1;
            var message = "Fake Message";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(false)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            #endregion Arrange

            #region Act
            Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.Remove(It.IsAny<ConditionalApproval>()), Times.Never());
            Assert.AreEqual("Fake Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name1", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 7;
            var message = "Fake Message";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(false)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            #endregion Arrange

            #region Act
            Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.Remove(It.IsAny<ConditionalApproval>()), Times.Never());
            Assert.AreEqual("Fake Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostWhenValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDateForIndex1();
            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 1;
            var message = "Fake Message";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            ConditionalApproval args1 = default;
            Mock.Get( ConditionalApprovalRepository).Setup(a => a.Remove(It.IsAny<ConditionalApproval>()))
                .Callback<ConditionalApproval>(x => args1 = x);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);

            Assert.AreEqual("Conditional Approval removed successfully", Controller.Message);
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.Remove(It.IsAny<ConditionalApproval>()));
 
            Assert.IsNotNull(args1);
            Assert.AreEqual(1, args1.Id);

            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name1", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostWhenValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDateForIndex1();
            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 7;
            var message = "Fake Message";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            ConditionalApproval args1 = default;
            Mock.Get(ConditionalApprovalRepository).Setup(a => a.Remove(It.IsAny<ConditionalApproval>()))
                .Callback<ConditionalApproval>(x => args1 = x);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("1", result.RouteValues["id"]);

            Assert.AreEqual("Conditional Approval removed successfully", Controller.Message);
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.Remove(It.IsAny<ConditionalApproval>()));

            Assert.IsNotNull(args1);
            Assert.AreEqual(7, args1.Id);

            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);       
            #endregion Assert
        }
        #endregion Delete Post Tests
    }
}
