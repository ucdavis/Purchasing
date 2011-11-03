using System.Linq;
using System.Web.Mvc;
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
            ActionResult redirectAction = new EmptyResult();
            //SecurityService.Expect(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything)).Return(null);
            #endregion Arrange

            #region Act
            Controller.Delete(19).AssertResultIs<EmptyResult>();
            #endregion Act

            #region Assert
            ConditionalApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            //SecurityService.AssertWasCalled(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything));
            //var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything))[0]; 
            //Assert.IsNotNull(args);
            //Assert.AreEqual(19, args[1]);
            //Assert.AreEqual(false, args[3]);
            #endregion Assert
        }


        [TestMethod]
        public void TestDeleteGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDateForIndex1();
            ActionResult redirectAction = null;
            //SecurityService.Expect(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything)).Return(ConditionalApprovalRepository.GetNullableById(1));
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

            //SecurityService.AssertWasCalled(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything));
            //var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything))[0];
            //Assert.IsNotNull(args);
            //Assert.AreEqual(1, args[1]);
            //Assert.AreEqual(false, args[3]);
            #endregion Assert		
        }


        [TestMethod]
        public void TestDeleteGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDateForIndex1();
            ActionResult redirectAction = null;
            //SecurityService.Expect(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything)).Return(ConditionalApprovalRepository.GetNullableById(7));
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

            //SecurityService.AssertWasCalled(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything));
            //var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything))[0];
            //Assert.IsNotNull(args);
            //Assert.AreEqual(7, args[1]);
            //Assert.AreEqual(false, args[3]);
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
            ActionResult redirectAction = new EmptyResult();
            //SecurityService.Expect(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything)).Return(null);
            #endregion Arrange

            #region Act
            Controller.Delete(conditionalApprovalViewModel).AssertResultIs<EmptyResult>();
            #endregion Act

            #region Assert
            ConditionalApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));

            //var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything))[0];
            //Assert.IsNotNull(args);
            //Assert.AreEqual(19, args[1]);
            //Assert.AreEqual(false, args[3]);
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
            ActionResult redirectAction = null;
            //SecurityService.Expect(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything)).Return(ConditionalApprovalRepository.GetNullableById(1));
            #endregion Arrange

            #region Act
            Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect()
                .ToAction<ConditionalApprovalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Conditional Approval removed successfully", Controller.Message);
            ConditionalApprovalRepository.AssertWasCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            var args = (ConditionalApproval) ConditionalApprovalRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<ConditionalApproval>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.Id);

            //var args2 = SecurityService.GetArgumentsForCallsMadeOn(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything))[0];
            //Assert.IsNotNull(args2);
            //Assert.AreEqual(1, args2[1]);
            //Assert.AreEqual(false, args2[3]);
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
            ActionResult redirectAction = null;
            //SecurityService.Expect(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything)).Return(ConditionalApprovalRepository.GetNullableById(7));            
            #endregion Arrange

            #region Act
            Controller.Delete(conditionalApprovalViewModel)
                .AssertActionRedirect()
                .ToAction<ConditionalApprovalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Conditional Approval removed successfully", Controller.Message);
            ConditionalApprovalRepository.AssertWasCalled(a => a.Remove(Arg<ConditionalApproval>.Is.Anything));
            var args = (ConditionalApproval)ConditionalApprovalRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<ConditionalApproval>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(7, args.Id);

            //var args2 = SecurityService.GetArgumentsForCallsMadeOn(a => a.ConditionalApprovalAccess(Arg<ConditionalApprovalController>.Is.Anything, Arg<int>.Is.Anything, out Arg<ActionResult>.Out(redirectAction).Dummy, Arg<bool>.Is.Anything))[0];
            //Assert.IsNotNull(args2);
            //Assert.AreEqual(7, args2[1]);
            //Assert.AreEqual(false, args2[3]);
            #endregion Assert
        }
        #endregion Delete Post Tests
    }
}
