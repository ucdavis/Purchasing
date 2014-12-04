using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Controllers;
using Rhino.Mocks;
using UCDArch.Testing.Fakes;

namespace Purchasing.Tests.ControllerTests.AutoApprovalControllerTests
{
    public partial class AutoApprovalControllerTests
    {
        #region Delete Get Tests
        [TestMethod]
        public void TestDeleteGetRedirectsToIndexWhenAutoApprovalNotFound1()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(4, true)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(true));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteGetRedirectsToIndexWhenAutoApprovalNotFound2()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(4, false)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteGetRedirectsToIndexWhenAutoApprovalNotFound3()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(4)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteGetRedirectsToErrorWhenCurrentIdDifferent()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Delete(3)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("No Access", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestDeleteGetRedirectsWhenAlreadyDeactivated()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(1, true)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(true));
            #endregion Act

            #region Assert
            Assert.AreEqual("Already deactivated", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            #endregion Assert	
        }

        [TestMethod]
        public void TestDeleteGetReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(3, true)
                .AssertViewRendered()
                .WithViewData<AutoApproval>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Id);
            Assert.IsTrue(Controller.ViewBag.ShowAll);
            #endregion Assert
        }
        #endregion Delete Get Tests

        #region Delete Post Tests
        [TestMethod]
        public void TestDeletePostRedirectsToIndexWhenAutoApprovalNotFound1()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(4, new AutoApproval(), true)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(true));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            AutoApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<AutoApproval>.Is.Anything));
            AutoApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<AutoApproval>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsToIndexWhenAutoApprovalNotFound2()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(4, new AutoApproval(), false)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            AutoApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<AutoApproval>.Is.Anything));
            AutoApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<AutoApproval>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsToIndexWhenAutoApprovalNotFound3()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(4, new AutoApproval())
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            AutoApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<AutoApproval>.Is.Anything));
            AutoApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<AutoApproval>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsToErrorWhenCurrentIdDifferent()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Delete(3, new AutoApproval())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("No Access", Controller.ErrorMessage);
            AutoApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<AutoApproval>.Is.Anything));
            AutoApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<AutoApproval>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsWhenAlreadyDeactivated()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(1, new AutoApproval(), true)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(true));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            AutoApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<AutoApproval>.Is.Anything));
            AutoApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<AutoApproval>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsToErrorWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(3, new AutoApproval(), true)
                .AssertActionRedirect()
                .ToAction<AutoApprovalController>(a => a.Index(true));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            Assert.AreEqual("AutoApproval Deactivated Successfully", Controller.Message);
            AutoApprovalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<AutoApproval>.Is.Anything));
            var args = (AutoApproval) AutoApprovalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<AutoApproval>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.IsFalse(args.IsActive);
            AutoApprovalRepository.AssertWasNotCalled(a => a.Remove(Arg<AutoApproval>.Is.Anything));
            #endregion Assert
        }
        #endregion Delete Post Tests
    }
}
