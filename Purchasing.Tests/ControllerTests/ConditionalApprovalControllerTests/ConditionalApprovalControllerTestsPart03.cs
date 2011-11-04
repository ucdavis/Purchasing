using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Testing.Fakes;


namespace Purchasing.Tests.ControllerTests.ConditionalApprovalControllerTests
{
    public partial class ConditionalApprovalControllerTests
    {
        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetRedirectsIfInvalidParameterUsed()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            const string approvalType = "Duck";
            #endregion Arrange

            #region Act
            Controller.Create(approvalType)
                .AssertActionRedirect()
                .ToAction<ConditionalApprovalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual(string.Format("You cannot create a conditional approval for type {0} because you are not associated with any {0}s.", approvalType), Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestCreateGetRedirectsWhenWorkgroupIfNoWorkgroups()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "4");
            SetupDateForIndex1();
            const string approvalType = "Workgroup";
            #endregion Arrange

            #region Act
            Controller.Create(approvalType)
                .AssertActionRedirect()
                .ToAction<ConditionalApprovalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual(string.Format("You cannot create a conditional approval for type {0} because you are not associated with any {0}s.", approvalType), Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetRedirectsWhenOrganizationIfNoOrganizations()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "4");
            SetupDateForIndex1();
            const string approvalType = "Organization";
            #endregion Arrange

            #region Act
            Controller.Create(approvalType)
                .AssertActionRedirect()
                .ToAction<ConditionalApprovalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual(string.Format("You cannot create a conditional approval for type {0} because you are not associated with any {0}s.", approvalType), Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            const string approvalType = "Workgroup";
            #endregion Arrange

            #region Act
            var result = Controller.Create(approvalType)
                .AssertViewRendered()
                .WithViewData<ConditionalApprovalModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(approvalType, result.ApprovalType);
            Assert.AreEqual(3, result.Workgroups.Count);
            Assert.IsNull(result.Organization);
            Assert.IsNull(result.Organizations);
            Assert.IsNull(result.PrimaryApprover);
            Assert.IsNull(result.Question);
            Assert.IsNull(result.SecondaryApprover);
            Assert.IsNull(result.Workgroup);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            const string approvalType = "Organization";
            #endregion Arrange

            #region Act
            var result = Controller.Create(approvalType)
                .AssertViewRendered()
                .WithViewData<ConditionalApprovalModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(approvalType, result.ApprovalType);
            Assert.AreEqual(4, result.Organizations.Count);
            Assert.IsNull(result.Organization);
            Assert.IsNull(result.Workgroups);
            Assert.IsNull(result.PrimaryApprover);
            Assert.IsNull(result.Question);
            Assert.IsNull(result.SecondaryApprover);
            Assert.IsNull(result.Workgroup);
            #endregion Assert
        }
        #endregion Create Get Tests

        #region Create Post Tests

        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Do these tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        } 
        #endregion Create Post Tests
    }
}
