using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Controllers;
using UCDArch.Testing.Fakes;
using UCDArch.Testing.Extensions;
using Moq;

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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
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
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
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
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(It.IsAny<AutoApproval>()), Times.Never());
            Mock.Get(AutoApprovalRepository).Verify(a => a.Remove(It.IsAny<AutoApproval>()), Times.Never());
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
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(It.IsAny<AutoApproval>()), Times.Never());
            Mock.Get(AutoApprovalRepository).Verify(a => a.Remove(It.IsAny<AutoApproval>()), Times.Never());
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
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.RouteValues["showAll"]);
            Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(It.IsAny<AutoApproval>()), Times.Never());
            Mock.Get(AutoApprovalRepository).Verify(a => a.Remove(It.IsAny<AutoApproval>()), Times.Never());
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
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("No Access", Controller.ErrorMessage);
            Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(It.IsAny<AutoApproval>()), Times.Never());
            Mock.Get(AutoApprovalRepository).Verify(a => a.Remove(It.IsAny<AutoApproval>()), Times.Never());
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
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(It.IsAny<AutoApproval>()), Times.Never());
            Mock.Get(AutoApprovalRepository).Verify(a => a.Remove(It.IsAny<AutoApproval>()), Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsToErrorWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupData3();
            AutoApproval args = default;
            Mock.Get( AutoApprovalRepository).Setup(a => a.EnsurePersistent(It.IsAny<AutoApproval>()))
                .Callback<AutoApproval>(x => args = x);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(3, new AutoApproval(), true)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.RouteValues["showAll"]);
            Assert.AreEqual("AutoApproval Deactivated Successfully", Controller.Message);
            Mock.Get(AutoApprovalRepository).Verify(a => a.EnsurePersistent(It.IsAny<AutoApproval>()));
 
            Assert.IsNotNull(args);
            Assert.IsFalse(args.IsActive);
            Mock.Get(AutoApprovalRepository).Verify(a => a.Remove(It.IsAny<AutoApproval>()), Times.Never());
            #endregion Assert
        }
        #endregion Delete Post Tests
    }
}
