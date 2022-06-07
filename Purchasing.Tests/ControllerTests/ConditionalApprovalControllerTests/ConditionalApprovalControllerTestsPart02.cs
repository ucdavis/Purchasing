using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;
using UCDArch.Testing.Extensions;
using Moq;

namespace Purchasing.Tests.ControllerTests.ConditionalApprovalControllerTests
{
    public partial class ConditionalApprovalControllerTests
    {
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsWhenConditionalApprovalNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny)).Returns(false);
            #endregion Arrange

            #region Act
            Controller.Edit(19)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Conditional Approval not found", Controller.ErrorMessage);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny), Times.Never());
            #endregion Assert
        }

        public delegate void CallbackIn2Out1<T1, T2, T3>(T1 arg1, T2 arg2, out T3 arg3);

        [TestMethod]
        public void TestEditGetRedirectsWhenNoAccess1()
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
            Controller.Edit(1)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name1", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsWhenNoAccess2()
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
            Controller.Edit(7)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var message = "Fake Message";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1)
                .AssertViewRendered()
                .WithViewData<ConditionalApprovalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Question1", result.Question);
            Assert.AreEqual("WName1", result.OrgOrWorkgroupName);
            Assert.AreEqual("FirstName99 LastName99 (99)", result.PrimaryUserName);
            Assert.AreEqual(string.Empty, result.SecondaryUserName);

            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name1", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var message = "Fake Message";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            #endregion Arrange

            #region Act
            var result = Controller.Edit(7)
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

            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);
            #endregion Assert
        }
        #endregion Edit Get Tests

        #region Edit Post Tests
        [TestMethod]
        public void TestEditPostReturnsViewWhenNotValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny)).Returns(false);

            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 1;
            conditionalApprovalViewModel.OrgOrWorkgroupName = "Test";
            conditionalApprovalViewModel.PrimaryUserName = "Primy";
            conditionalApprovalViewModel.Question = "Que?";
            conditionalApprovalViewModel.SecondaryUserName = "Seconds";

            Controller.ModelState.AddModelError("Error", "Fake Error");
            #endregion Arrange

            #region Act
            var result = Controller.Edit(conditionalApprovalViewModel)
                .AssertViewRendered()
                .WithViewData<ConditionalApprovalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Fake Error");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Que?", result.Question);
            Assert.AreEqual("Test", result.OrgOrWorkgroupName);
            Assert.AreEqual("Primy", result.PrimaryUserName);
            Assert.AreEqual("Seconds", result.SecondaryUserName);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny), Times.Never());
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostRedirectsWhenConditionalApprovalNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny)).Returns(false);

            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 19;
            conditionalApprovalViewModel.OrgOrWorkgroupName = "Test";
            conditionalApprovalViewModel.PrimaryUserName = "Primy";
            conditionalApprovalViewModel.Question = "Que?";
            conditionalApprovalViewModel.SecondaryUserName = "Seconds";
            #endregion Arrange

            #region Act
            Controller.Edit(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Conditional Approval not found", Controller.ErrorMessage);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny), Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var message = "Fake Message";

            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 1;
            conditionalApprovalViewModel.OrgOrWorkgroupName = "Test";
            conditionalApprovalViewModel.PrimaryUserName = "Primy";
            conditionalApprovalViewModel.Question = "Que?";
            conditionalApprovalViewModel.SecondaryUserName = "Seconds";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(false)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            #endregion Arrange

            #region Act
            Controller.Edit(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name1", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var message = "Fake Message";

            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 7;
            conditionalApprovalViewModel.OrgOrWorkgroupName = "Test";
            conditionalApprovalViewModel.PrimaryUserName = "Primy";
            conditionalApprovalViewModel.Question = "Que?";
            conditionalApprovalViewModel.SecondaryUserName = "Seconds";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(false)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            #endregion Arrange

            #region Act
            Controller.Edit(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsAndSaves1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var message = "Fake Message";

            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 1;
            conditionalApprovalViewModel.OrgOrWorkgroupName = "Test";
            conditionalApprovalViewModel.PrimaryUserName = "Primy";
            conditionalApprovalViewModel.Question = "Que?";
            conditionalApprovalViewModel.SecondaryUserName = "Seconds";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            ConditionalApproval conditionalApprovalArgs = default;
            Mock.Get(ConditionalApprovalRepository).Setup(a => a.EnsurePersistent(It.IsAny<ConditionalApproval>()))
                .Callback<ConditionalApproval>(x => conditionalApprovalArgs = x);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);

            Assert.AreEqual("Conditional Approval edited successfully", Controller.Message);
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.EnsurePersistent(It.IsAny<ConditionalApproval>()));

            Assert.IsNotNull(conditionalApprovalArgs);
            Assert.AreEqual(1, conditionalApprovalArgs.Id);
            Assert.AreEqual("Que?", conditionalApprovalArgs.Question);
            Assert.AreEqual("WName1", conditionalApprovalArgs.Workgroup.Name);
            Assert.AreEqual("FirstName99 LastName99 (99)", conditionalApprovalArgs.PrimaryApprover.FullNameAndId);
            Assert.IsNull(conditionalApprovalArgs.SecondaryApprover);
            Assert.IsNull(conditionalApprovalArgs.Organization);

            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name1", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsAndSaves2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var message = "Fake Message";

            var conditionalApprovalViewModel = new ConditionalApprovalViewModel();
            conditionalApprovalViewModel.Id = 7;
            conditionalApprovalViewModel.OrgOrWorkgroupName = "Test";
            conditionalApprovalViewModel.PrimaryUserName = "Primy";
            conditionalApprovalViewModel.Question = "Que?";
            conditionalApprovalViewModel.SecondaryUserName = "Seconds";
            object[] args = default;
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((Workgroup a, Organization b, out string c) => args = new object[] { a, b, c = message });
            ConditionalApproval conditionalApprovalArgs = default;
            Mock.Get(ConditionalApprovalRepository).Setup(a => a.EnsurePersistent(It.IsAny<ConditionalApproval>()))
                .Callback<ConditionalApproval>(x => conditionalApprovalArgs = x);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("1", result.RouteValues["id"]);
            Assert.AreEqual("Conditional Approval edited successfully", Controller.Message);
            Mock.Get(ConditionalApprovalRepository).Verify(a => a.EnsurePersistent(It.IsAny<ConditionalApproval>()));

            Assert.IsNotNull(conditionalApprovalArgs);
            Assert.AreEqual(7, conditionalApprovalArgs.Id);
            Assert.AreEqual("Que?", conditionalApprovalArgs.Question);
            Assert.IsNull(conditionalApprovalArgs.Workgroup);
            Assert.AreEqual("FirstName99 LastName99 (99)", conditionalApprovalArgs.PrimaryApprover.FullNameAndId);
            Assert.AreEqual("FirstName88 LastName88 (88)", conditionalApprovalArgs.SecondaryApprover.FullNameAndId);
            Assert.AreEqual("OName1", conditionalApprovalArgs.Organization.Name);

            Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);
            Assert.IsNull(args[0]);
            #endregion Assert
        }
        #endregion Edit Post Tests
    }
}
