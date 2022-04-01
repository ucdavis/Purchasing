using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Tests.ControllerTests.WizardControllerTests
{
    public partial class WizardControllerTests
    {

        #region Index Tests
        [TestMethod]
        public void TestIndexReturnsView()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Controller.Index().AssertViewRendered();
            #endregion Assert		
        }
        #endregion Index Tests

        #region CreateWorkgroup Get Tests
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCreateWorkgroupGetThrowsExceptionIfNoCurrentUser()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NoMatch");
                SetupDataForWorkgroupActions1();
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.CreateWorkgroup();
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains no matching element", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestCreateWorkgroupGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.CreateWorkgroup()
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Organizations.Count());
            Assert.AreEqual("Name4 (4)", result.Organizations[0].ToString());
            Assert.AreEqual("Name5 (5)", result.Organizations[1].ToString());
            Assert.AreEqual("Name6 (6)", result.Organizations[2].ToString());
            Assert.IsNotNull(result.Workgroup);
            Assert.AreEqual(1, Controller.ViewBag.StepNumber);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateWorkgroupGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDataForWorkgroupActions1();
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.CreateWorkgroup()
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Organizations.Count());
            Assert.AreEqual("Name1 (1)", result.Organizations[0].ToString());
            Assert.AreEqual("Name2 (2)", result.Organizations[1].ToString());
            Assert.AreEqual("Name3 (3)", result.Organizations[2].ToString());
            Assert.IsNotNull(result.Workgroup);
            Assert.AreEqual(1, Controller.ViewBag.StepNumber);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateWorkgroupGetReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.CreateWorkgroup()
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Organizations.Count());
            Assert.AreEqual("Name7 (7)", result.Organizations[0].ToString());
            Assert.AreEqual("Name8 (8)", result.Organizations[1].ToString());
            Assert.AreEqual("Name9 (9)", result.Organizations[2].ToString());
            Assert.IsNotNull(result.Workgroup);
            Assert.AreEqual(1, Controller.ViewBag.StepNumber);
            #endregion Assert
        }

        #endregion CreateWorkgroup Get Tests

        #region CreateWorkgroup Post Tests

        [TestMethod]
        public void TestCreateWorkgroupPostReturnsViewWhenNotValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            var workgroup = CreateValidEntities.Workgroup(1);
            workgroup.PrimaryOrganization = OrganizationRepository.Queryable.Single(a => a.Id == "7");
            workgroup.Name = null;
            Controller.ModelState.AddModelError("Fake", "Error");
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(null).Dummy)).Return(true);
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.CreateWorkgroup(workgroup)
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Error");
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Organizations.Count());
            Assert.AreEqual("Name7 (7)", result.Organizations[0].ToString());
            Assert.AreEqual("Name8 (8)", result.Organizations[1].ToString());
            Assert.AreEqual("Name9 (9)", result.Organizations[2].ToString());
            Assert.IsNotNull(result.Workgroup);
            Assert.AreEqual(1, Controller.ViewBag.StepNumber);

            WorkgroupService.AssertWasNotCalled(a => a.CreateWorkgroup(Arg<Workgroup>.Is.Anything, Arg<string[]>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateWorkgroupPostReturnsViewWhenNotValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            var workgroup = CreateValidEntities.Workgroup(1);
            workgroup.PrimaryOrganization = OrganizationRepository.Queryable.Single(a => a.Id == "1");
            workgroup.Name = null;
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(null).Dummy)).Return(false);
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.CreateWorkgroup(workgroup)
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("You do not have access to the selected organization");
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Organizations.Count());
            Assert.AreEqual("Name7 (7)", result.Organizations[0].ToString());
            Assert.AreEqual("Name8 (8)", result.Organizations[1].ToString());
            Assert.AreEqual("Name9 (9)", result.Organizations[2].ToString());
            Assert.IsNotNull(result.Workgroup);
            Assert.AreEqual(1, Controller.ViewBag.StepNumber);

            WorkgroupService.AssertWasNotCalled(a => a.CreateWorkgroup(Arg<Workgroup>.Is.Anything, Arg<string[]>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateWorkgroupPostRedirectsWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            var workgroup = CreateValidEntities.Workgroup(1);
            workgroup.PrimaryOrganization = OrganizationRepository.Queryable.Single(a => a.Id == "7");
            workgroup.SetIdTo(99);
            WorkgroupService.Expect(a => a.CreateWorkgroup(workgroup, null)).Return(workgroup).Repeat.Any();
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(null).Dummy)).Return(true); 
            #endregion Arrange

            #region Act
            var result = Controller.CreateWorkgroup(workgroup)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(99, result.RouteValues["id"]);
            Assert.AreEqual(1, Controller.ViewBag.StepNumber);

            WorkgroupService.AssertWasCalled(a => a.CreateWorkgroup(Arg<Workgroup>.Is.Anything, Arg<string[]>.Is.Anything));
            var args = WorkgroupService.GetArgumentsForCallsMadeOn(a => a.CreateWorkgroup(Arg<Workgroup>.Is.Anything, Arg<string[]>.Is.Anything))[0];
            Assert.AreEqual("Name1", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            Assert.AreEqual("Name1 workgroup was created", Controller.Message);
            #endregion Assert		
        }
        #endregion CreateWorkgroup Post Tests


    }
}
