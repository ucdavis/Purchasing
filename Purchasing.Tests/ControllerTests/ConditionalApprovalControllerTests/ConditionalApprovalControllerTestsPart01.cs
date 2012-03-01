using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Testing.Fakes;


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
                .AssertActionRedirect()
                .ToAction<ConditionalApprovalController>(a => a.Index());
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
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
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
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
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
                .AssertActionRedirect()
                .ToAction<ConditionalApprovalController>(a => a.Index());
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
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
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
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
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
                .AssertActionRedirect()
                .ToAction<ConditionalApprovalController>(a => a.ByWorkgroup(5));
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
            Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect()
                .ToAction<ConditionalApprovalController>(a => a.Index());
            #endregion Act

            #region Assert
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
