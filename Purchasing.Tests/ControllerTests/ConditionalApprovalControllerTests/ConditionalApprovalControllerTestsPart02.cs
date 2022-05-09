using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;
using UCDArch.Testing.Extensions;

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
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny)).Returns(false);
            #endregion Arrange

            #region Act
            Controller.Edit(19)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Conditional Approval not found", Controller.ErrorMessage);
            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            var message = "Fake Message";
            object[] args = default;
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out message)).Returns(false)
                .Callback((object[] x) => args = x);
            #endregion Arrange

            #region Act
            Controller.Edit(1)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny));

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
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out message)).Returns(false)
                .Callback((object[] x) => args = x);
            #endregion Arrange

            #region Act
            Controller.Edit(7)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny));

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
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((object[] x) => args = x);
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

            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny));

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
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((object[] x) => args = x);
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

            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny));

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
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny)).Returns(false);

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
            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny), Moq.Times.Never());
            #endregion Assert
        } 
        

        [TestMethod]
        public void TestEditPostRedirectsWhenConditionalApprovalNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny)).Returns(false);

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
            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny), Moq.Times.Never());
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
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out message)).Returns(false)
                .Callback((object[] x) => args = x);
            #endregion Arrange

            #region Act
            Controller.Edit(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny));

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
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out message)).Returns(false)
                .Callback((object[] x) => args = x);
            #endregion Arrange

            #region Act
            Controller.Edit(conditionalApprovalViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny));

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
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((object[] x) => args = x);
            ConditionalApproval conditionalApprovalArgs = default;
            Moq.Mock.Get(ConditionalApprovalRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<ConditionalApproval>()))
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
            Moq.Mock.Get(ConditionalApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<ConditionalApproval>()));

            Assert.IsNotNull(conditionalApprovalArgs);
            Assert.AreEqual(1, conditionalApprovalArgs.Id);
            Assert.AreEqual("Que?",conditionalApprovalArgs.Question);
            Assert.AreEqual("WName1", conditionalApprovalArgs.Workgroup.Name);
            Assert.AreEqual("FirstName99 LastName99 (99)", conditionalApprovalArgs.PrimaryApprover.FullNameAndId);
            Assert.IsNull(conditionalApprovalArgs.SecondaryApprover);
            Assert.IsNull(conditionalApprovalArgs.Organization);

            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny));

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
            Moq.Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out message)).Returns(true)
                .Callback((object[] x) => args = x);
            ConditionalApproval conditionalApprovalArgs = default;
            Moq.Mock.Get(ConditionalApprovalRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<ConditionalApproval>()))
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
            Moq.Mock.Get(ConditionalApprovalRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<ConditionalApproval>()));

            Assert.IsNotNull(conditionalApprovalArgs);
            Assert.AreEqual(7, conditionalApprovalArgs.Id);
            Assert.AreEqual("Que?", conditionalApprovalArgs.Question);
            Assert.IsNull(conditionalApprovalArgs.Workgroup);
            Assert.AreEqual("FirstName99 LastName99 (99)", conditionalApprovalArgs.PrimaryApprover.FullNameAndId);
            Assert.AreEqual("FirstName88 LastName88 (88)", conditionalApprovalArgs.SecondaryApprover.FullNameAndId);
            Assert.AreEqual("OName1", conditionalApprovalArgs.Organization.Name);

            Moq.Mock.Get(SecurityService).Verify(a => a.HasWorkgroupOrOrganizationAccess(Moq.It.IsAny<Workgroup>(), Moq.It.IsAny<Organization>(), out Moq.It.Ref<string>.IsAny));

            Assert.IsNotNull(args);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);
            Assert.IsNull(args[0]);
            #endregion Assert
        }
        #endregion Edit Post Tests
    }
}
