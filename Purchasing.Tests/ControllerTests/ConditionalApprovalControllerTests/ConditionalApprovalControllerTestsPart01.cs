using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;

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
            org1.SetIdTo("test");
            org2.SetIdTo("test2");
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
            org1.SetIdTo("test");
            org2.SetIdTo("test2");
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
            wg1.SetIdTo(2);
            wg2.SetIdTo(3);
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
            wg1.SetIdTo(2);
            wg2.SetIdTo(3);
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
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(null).Dummy)).Return(false);            
            #endregion Arrange

            #region Act
            Controller.Delete(19)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            ConditionalApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            Assert.AreEqual("Conditional Approval not found", Controller.ErrorMessage);
            SecurityService.AssertWasNotCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(null).Dummy));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteGetRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            const string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            ConditionalApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
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
            const string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Delete(7)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            ConditionalApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
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
            const string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
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
            ConditionalApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
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
            const string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
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
            ConditionalApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
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
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(null).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            ConditionalApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            Assert.AreEqual("Conditional Approval not found", Controller.ErrorMessage);
            SecurityService.AssertWasNotCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(null).Dummy));
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
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            ConditionalApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
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
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            ConditionalApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
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
            const string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);

            Assert.AreEqual("Conditional Approval removed successfully", Controller.Message);
            ConditionalApprovalRepository.AssertWasCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            var args1 = (ConditionalApproval) ConditionalApprovalRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<ConditionalApproval>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args1);
            Assert.AreEqual(1, args1.Id);

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
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
            const string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("1", result.RouteValues["id"]);

            Assert.AreEqual("Conditional Approval removed successfully", Controller.Message);
            ConditionalApprovalRepository.AssertWasCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            var args1 = (ConditionalApproval)ConditionalApprovalRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<ConditionalApproval>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(7, args1.Id);

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);       
            #endregion Assert
        }
        #endregion Delete Post Tests
    }
}
