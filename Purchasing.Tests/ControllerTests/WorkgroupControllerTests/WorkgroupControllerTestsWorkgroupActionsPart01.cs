using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<IEnumerable<Workgroup>>().ToList();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());

            Assert.AreEqual("Name4", result[0].Name);
            Assert.AreEqual("Name1", result[0].Organizations.ElementAt(0).Name);
            Assert.AreEqual("Name4", result[0].Organizations.ElementAt(1).Name);

            Assert.AreEqual("Name5", result[1].Name);
            Assert.AreEqual("Name1", result[1].Organizations.ElementAt(0).Name);
            Assert.AreEqual("Name5", result[1].Organizations.ElementAt(1).Name);

            Assert.AreEqual("Name6", result[2].Name);
            Assert.AreEqual("Name1", result[2].Organizations.ElementAt(0).Name);
            Assert.AreEqual("Name6", result[2].Organizations.ElementAt(1).Name);
            #endregion Assert		
        }
        
        [TestMethod]
        public void TestIndexReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDataForWorkgroupActions1();
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<IEnumerable<Workgroup>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.Count());
            #endregion Assert
        }

        #endregion Index Tests

        #region Create Get Tests

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCreateGetThrowsExceptionIfNoCurrentUser()
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
                Controller.Create();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains no elements", ex.Message);
                throw;
            }	
        }


        [TestMethod]
        public void TestCreateGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            #endregion Arrange

            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Organizations.Count());
            Assert.AreEqual("Name4", result.Organizations[0].ToString());
            Assert.AreEqual("Name5", result.Organizations[1].ToString());
            Assert.AreEqual("Name6", result.Organizations[2].ToString());
            Assert.IsNotNull(result.Workgroup);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDataForWorkgroupActions1();
            #endregion Arrange

            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Organizations.Count());
            Assert.AreEqual("Name1", result.Organizations[0].ToString());
            Assert.AreEqual("Name2", result.Organizations[1].ToString());
            Assert.AreEqual("Name3", result.Organizations[2].ToString());
            Assert.IsNotNull(result.Workgroup);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            #endregion Arrange

            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Organizations.Count());
            Assert.AreEqual("Name7", result.Organizations[0].ToString());
            Assert.AreEqual("Name8", result.Organizations[1].ToString());
            Assert.AreEqual("Name9", result.Organizations[2].ToString());
            Assert.IsNotNull(result.Workgroup);
            #endregion Assert
        }

        #endregion Create Get Tests

        #region Create Post Tests

        [TestMethod]
        public void TestCreatePostReturnsViewInNotValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            var workgroup = CreateValidEntities.Workgroup(1);
            workgroup.Name = null;
            Controller.ModelState.AddModelError("Fake", "Error");
            #endregion Arrange

            #region Act
            var result = Controller.Create(workgroup, null)
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Error");
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Organizations.Count());
            Assert.AreEqual("Name7", result.Organizations[0].ToString());
            Assert.AreEqual("Name8", result.Organizations[1].ToString());
            Assert.AreEqual("Name9", result.Organizations[2].ToString());
            Assert.IsNotNull(result.Workgroup);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostRedirectsToIndex1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            var workgroup = CreateValidEntities.Workgroup(1);
            #endregion Arrange

            #region Act
            Controller.Create(workgroup, null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Name1 workgroup was created", Controller.Message);
            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup) WorkgroupRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            Assert.AreEqual(1, args.Organizations.Count);
            Assert.AreEqual("Name1", args.Organizations[0].Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsToIndex2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1(true);
            var workgroup = CreateValidEntities.Workgroup(1);
            var organizations = new List<Organization>();
            for (int i = 0; i < 5; i++)
            {
                organizations.Add(CreateValidEntities.Organization(i+1));
                organizations[i].SetIdTo((i + 1).ToString());
            }
            OrganizationRepository.Expect(a => a.Queryable).Return(organizations.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(workgroup, new[]{"2", "4"})
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Name1 workgroup was created", Controller.Message);
            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)WorkgroupRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            Assert.AreEqual(3, args.Organizations.Count);
            Assert.AreEqual("Name2", args.Organizations[0].Name);
            Assert.AreEqual("Name4", args.Organizations[1].Name);
            Assert.AreEqual("Name1", args.Organizations[2].Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsToIndex3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1(true);
            var workgroup = CreateValidEntities.Workgroup(1);
            var organizations = new List<Organization>();
            for(int i = 0; i < 5; i++)
            {
                organizations.Add(CreateValidEntities.Organization(i + 1));
                organizations[i].SetIdTo((i + 1).ToString());
            }
            OrganizationRepository.Expect(a => a.Queryable).Return(organizations.AsQueryable()).Repeat.Any();
            workgroup.PrimaryOrganization = organizations[0];
            #endregion Arrange

            #region Act
            Controller.Create(workgroup, new[] { "1", "4" })
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Name1 workgroup was created", Controller.Message);
            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)WorkgroupRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            Assert.AreEqual(2, args.Organizations.Count);
            Assert.AreEqual("Name1", args.Organizations[0].Name);
            Assert.AreEqual("Name4", args.Organizations[1].Name);
            #endregion Assert
        }
        #endregion Create Post Tests
    }
}
