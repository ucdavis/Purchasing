using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Fakes;
using UCDArch.Testing.Extensions;
using Microsoft.AspNetCore.Mvc;

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
                .AssertActionRedirect();
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddSubOrganizationsGetReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.AddSubOrganizations(1)
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert

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
        public void TestAddSubOrganizationsPostRedirectsToSubOrganizationsWhenNoOrgsAdded1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            #endregion Arrange

            #region Act
            var result = Controller.AddSubOrganizations(3, null)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);

            Moq.Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()));
//TODO: Arrange
            Workgroup workgroupArgs = default;
            Moq.Mock.Get( WorkgroupRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()))
                .Callback<Workgroup>(x => workgroupArgs = x);
//ENDTODO 
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
            Assert.AreEqual(2, WorkgroupRepository.Queryable.Single(a => a.Id == 1).Organizations.Count()); //Organizations does not contain primary org
            #endregion Arrange

            #region Act
            var result = Controller.AddSubOrganizations(3, new string[0])
                .AssertActionRedirect();
            #endregion Act

            #region Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);

            Moq.Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()));
//TODO: Arrange
            Workgroup workgroupArgs = default;
            Moq.Mock.Get(WorkgroupRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()))
                .Callback<Workgroup>(x => workgroupArgs = x);
//ENDTODO
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
            #endregion Arrange

            #region Act
            var result = Controller.AddSubOrganizations(3, new[] { "1", "3" })
                .AssertActionRedirect();
            #endregion Act

            #region Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);

            Moq.Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()));
//TODO: Arrange
            Workgroup workgroupArgs = default;
            Moq.Mock.Get(WorkgroupRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()))
                .Callback<Workgroup>(x => workgroupArgs = x);
//ENDTODO
            Assert.IsNotNull(workgroupArgs);
            Assert.AreEqual(2, workgroupArgs.Organizations.Count());
            Assert.AreEqual("Name1", workgroupArgs.Organizations[0].Name);
            Assert.AreEqual("Name3", workgroupArgs.Organizations[1].Name);

            Assert.AreEqual(2, Controller.ViewBag.StepNumber);
            #endregion Assert
        }


        [TestMethod]
        public void TestAddSubOrganizationsPostRedirectsToSubOrganizationsWhenNewOrgsAdded()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDataForWorkgroupActions1();
         
            Assert.AreEqual(2, WorkgroupRepository.Queryable.Single(a => a.Id == 1).Organizations.Count()); //Organizations does not contain primary org
            #endregion Arrange

            #region Act
            var result = Controller.AddSubOrganizations(3, new[] { "1", "2" })
                .AssertActionRedirect();
            #endregion Act

            #region Assert


            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);

            Moq.Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()));
//TODO: Arrange
            Workgroup workgroupArgs = default;
            Moq.Mock.Get(WorkgroupRepository).Setup(a => a.EnsurePersistent(Moq.It.IsAny<Workgroup>()))
                .Callback<Workgroup>(x => workgroupArgs = x);
//ENDTODO
            Assert.IsNotNull(workgroupArgs);
            Assert.AreEqual(3, workgroupArgs.Organizations.Count());
            Assert.AreEqual("Name1", workgroupArgs.Organizations[0].Name);
            Assert.AreEqual("Name2", workgroupArgs.Organizations[1].Name);
            Assert.AreEqual("Name3", workgroupArgs.Organizations[2].Name);

            Assert.AreEqual(2, Controller.ViewBag.StepNumber);
            #endregion Assert
        }
        #endregion AddSubOrganizations Post Tests

        #region SubOrganizations Tests


        [TestMethod]
        public void TestSubOrganizationsReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            #endregion Arrange

            #region Act
            var result = Controller.SubOrganizations(1)
                .AssertViewRendered()
                .WithViewData<Workgroup>();
            #endregion Act

            #region Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("Name1", result.Name);
            Assert.AreEqual(2, result.Organizations.Count());

            Assert.AreEqual(2, Controller.ViewBag.StepNumber);
            #endregion Assert
        }
        #endregion SubOrganizations Tests
    }
}
