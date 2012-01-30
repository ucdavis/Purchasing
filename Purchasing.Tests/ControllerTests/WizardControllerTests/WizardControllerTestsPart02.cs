using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using FluentNHibernate.Data;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Helpers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;

namespace Purchasing.Tests.ControllerTests.WizardControllerTests
{
    public partial class WizardControllerTests
    {
        #region AddSubOrganizations Get Tests

        [TestMethod]
        public void TestAddSubOrganizationsGetRedirectsToCreateWorkgroupIfIdIsZero()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Controller.AddSubOrganizations(0)
                .AssertActionRedirect()
                .ToAction<WizardController>(a => a.CreateWorkgroup());
            #endregion Assert		
        }


        [TestMethod]
        public void TestAddSubOrganizationsGetRedirectsIfNoAccess()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.AddSubOrganizations(3)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddSubOrganizationsGetReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            string message = string.Empty;
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.AddSubOrganizations(1)
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual("Name1", result.Workgroup.Name);
            Assert.AreEqual(3, result.Organizations.Count());

            Assert.AreEqual(2, Controller.ViewBag.StepNumber);
            Assert.AreEqual(1, Controller.ViewBag.WorkgroupId);
            #endregion Assert
        }

        #endregion AddSubOrganizations Get Tests

        #region AddSubOrganizations Post Tests

        [TestMethod]
        public void TestAddSubOrganizationsPostRedirectsIfNoAccess()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.AddSubOrganizations(3, new string[0])
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddSubOrganizationsPostRedirectsToSubOrganizationsWhenNoOrgsAdded1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            string message = string.Empty;
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            Assert.AreEqual(2, WorkgroupRepository.Queryable.Single(a => a.Id == 1).Organizations.Count()); //Organizations does not contain primary org
            #endregion Arrange

            #region Act
            var result = Controller.AddSubOrganizations(3, null)
                .AssertActionRedirect()
                .ToAction<WizardController>(a => a.SubOrganizations(3));
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);

            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            var workgroupArgs = (Workgroup) WorkgroupRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything))[0][0]; 
            Assert.IsNotNull(workgroupArgs);
            Assert.AreEqual(2, workgroupArgs.Organizations.Count());
            Assert.AreEqual("Name1", workgroupArgs.Organizations[0].Name);
            Assert.AreEqual("Name3", workgroupArgs.Organizations[1].Name);

            Assert.AreEqual(2, Controller.ViewBag.StepNumber);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddSubOrganizationsPostRedirectsToSubOrganizationsWhenNoOrgsAdded2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            string message = string.Empty;
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            Assert.AreEqual(2, WorkgroupRepository.Queryable.Single(a => a.Id == 1).Organizations.Count()); //Organizations does not contain primary org
            #endregion Arrange

            #region Act
            var result = Controller.AddSubOrganizations(3, new string[0])
                .AssertActionRedirect()
                .ToAction<WizardController>(a => a.SubOrganizations(3));
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);

            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            var workgroupArgs = (Workgroup)WorkgroupRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(workgroupArgs);
            Assert.AreEqual(2, workgroupArgs.Organizations.Count());
            Assert.AreEqual("Name1", workgroupArgs.Organizations[0].Name);
            Assert.AreEqual("Name3", workgroupArgs.Organizations[1].Name);

            Assert.AreEqual(2, Controller.ViewBag.StepNumber);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddSubOrganizationsPostRedirectsToSubOrganizationsWhenNoNewOrgsAdded3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDataForWorkgroupActions1();
            string message = string.Empty;
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            Assert.AreEqual(2, WorkgroupRepository.Queryable.Single(a => a.Id == 1).Organizations.Count()); //Organizations does not contain primary org
            #endregion Arrange

            #region Act
            var result = Controller.AddSubOrganizations(3, new[] { "1", "3" })
                .AssertActionRedirect()
                .ToAction<WizardController>(a => a.SubOrganizations(3));
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);

            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            var workgroupArgs = (Workgroup)WorkgroupRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(workgroupArgs);
            Assert.AreEqual(2, workgroupArgs.Organizations.Count());
            Assert.AreEqual("Name1", workgroupArgs.Organizations[0].Name);
            Assert.AreEqual("Name3", workgroupArgs.Organizations[1].Name);

            Assert.AreEqual(2, Controller.ViewBag.StepNumber);
            #endregion Assert
        }
        #endregion AddSubOrganizations Post Tests
    }
}
