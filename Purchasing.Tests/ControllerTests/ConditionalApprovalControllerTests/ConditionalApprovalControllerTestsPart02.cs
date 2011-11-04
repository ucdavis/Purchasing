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

        [TestMethod]
        public void TestEditGetRedirectsWhenConditionalApprovalNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(null).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Edit(19)
                .AssertActionRedirect()
                .ToAction<ConditionalApprovalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Conditional Approval not found", Controller.ErrorMessage);
            SecurityService.AssertWasNotCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(null).Dummy));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            const string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Edit(1)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("WName1", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            const string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Edit(7)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
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
            const string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
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

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("WName1", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDateForIndex1();
            const string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
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

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual("OName1", ((Organization)args[1]).Name);   
            #endregion Assert
        }

    }
}
