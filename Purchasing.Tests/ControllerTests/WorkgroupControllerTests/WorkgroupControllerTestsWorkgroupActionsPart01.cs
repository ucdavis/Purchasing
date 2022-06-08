using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            Mock.Get(WorkgroupService).Setup(a => a.LoadAdminWorkgroups(false)).Returns(new Workgroup[0]);
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<WorkgroupIndexModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.WorkGroups.Count());
            Assert.IsFalse(result.ShowAll);

            //Assert.AreEqual("Name4", result[0].Name);
            //Assert.AreEqual("Name1", result[0].Organizations.ElementAt(0).Name);
            //Assert.AreEqual("Name4", result[0].Organizations.ElementAt(1).Name);

            //Assert.AreEqual("Name5", result[1].Name);
            //Assert.AreEqual("Name1", result[1].Organizations.ElementAt(0).Name);
            //Assert.AreEqual("Name5", result[1].Organizations.ElementAt(1).Name);

            //Assert.AreEqual("Name6", result[2].Name);
            //Assert.AreEqual("Name1", result[2].Organizations.ElementAt(0).Name);
            //Assert.AreEqual("Name6", result[2].Organizations.ElementAt(1).Name);
            #endregion Assert		
        }
        
        [TestMethod]
        public void TestIndexReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "1");
            SetupDataForWorkgroupActions1();
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            Mock.Get(WorkgroupService).Setup(a => a.LoadAdminWorkgroups(false)).Returns(new Workgroup[0]);
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<WorkgroupIndexModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.WorkGroups.Count());
            #endregion Assert
        }

        #endregion Index Tests

        //#region Create Get Tests

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void TestCreateGetThrowsExceptionIfNoCurrentUser()
        //{
        //    var thisFar = false;
        //    try
        //    {
        //        #region Arrange
        //        Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "NoMatch");
        //        SetupDataForWorkgroupActions1();
        //        thisFar = true;
        //        #endregion Arrange

        //        #region Act
        //        Controller.Create();
        //        #endregion Act
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsTrue(thisFar);
        //        Assert.IsNotNull(ex);
        //        Assert.AreEqual("Sequence contains no elements", ex.Message);
        //        throw;
        //    }	
        //}


        //[TestMethod]
        //public void TestCreateGetReturnsView1()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "2");
        //    SetupDataForWorkgroupActions1();
        //    new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create()
        //        .AssertViewRendered()
        //        .WithViewData<WorkgroupModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(3, result.Organizations.Count());
        //    Assert.AreEqual("Name4", result.Organizations[0].ToString());
        //    Assert.AreEqual("Name5", result.Organizations[1].ToString());
        //    Assert.AreEqual("Name6", result.Organizations[2].ToString());
        //    Assert.IsNotNull(result.Workgroup);
        //    #endregion Assert		
        //}

        //[TestMethod]
        //public void TestCreateGetReturnsView2()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "1");
        //    SetupDataForWorkgroupActions1();
        //    new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create()
        //        .AssertViewRendered()
        //        .WithViewData<WorkgroupModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(3, result.Organizations.Count());
        //    Assert.AreEqual("Name1", result.Organizations[0].ToString());
        //    Assert.AreEqual("Name2", result.Organizations[1].ToString());
        //    Assert.AreEqual("Name3", result.Organizations[2].ToString());
        //    Assert.IsNotNull(result.Workgroup);
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestCreateGetReturnsView3()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
        //    SetupDataForWorkgroupActions1();
        //    new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create()
        //        .AssertViewRendered()
        //        .WithViewData<WorkgroupModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(3, result.Organizations.Count());
        //    Assert.AreEqual("Name7", result.Organizations[0].ToString());
        //    Assert.AreEqual("Name8", result.Organizations[1].ToString());
        //    Assert.AreEqual("Name9", result.Organizations[2].ToString());
        //    Assert.IsNotNull(result.Workgroup);
        //    #endregion Assert
        //}

        //#endregion Create Get Tests

        //#region Create Post Tests

        //[TestMethod]
        //public void TestCreatePostReturnsViewInNotValid()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
        //    SetupDataForWorkgroupActions1();
        //    var workgroup = CreateValidEntities.Workgroup(1);
        //    workgroup.Name = null;
        //    Controller.ModelState.AddModelError("Fake", "Error");
        //    Mock.Get(SecurityService).Setup(a => a.HasWorkgroupOrOrganizationAccess(It.IsAny<Workgroup>(), It.IsAny<Organization>(), out It.Ref<string>.IsAny)).Returns(true);
        //    new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create(workgroup, null)
        //        .AssertViewRendered()
        //        .WithViewData<WorkgroupModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Controller.ModelState.AssertErrorsAre("Error");
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(3, result.Organizations.Count());
        //    Assert.AreEqual("Name7", result.Organizations[0].ToString());
        //    Assert.AreEqual("Name8", result.Organizations[1].ToString());
        //    Assert.AreEqual("Name9", result.Organizations[2].ToString());
        //    Assert.IsNotNull(result.Workgroup);
        //    #endregion Assert		
        //}

        //[TestMethod]
        //public void TestCreatePostRedirectsToIndex1()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
        //    SetupDataForWorkgroupActions1();
        //    var workgroup = CreateValidEntities.Workgroup(1);
        //    Mock.Get(WorkgroupService).Setup(a => a.CreateWorkgroup(It.IsAny<Workgroup>(), It.IsAny<string[]>())).Returns(workgroup);
        //    #endregion Arrange

        //    #region Act
        //    Controller.Create(workgroup, null)
        //        .AssertActionRedirect()
        //        .ToAction<WorkgroupController>(a => a.Index(false));
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual("Name1 workgroup was created", Controller.Message);
        //    //Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(It.IsAny<Workgroup>()));
        //    //var args = (Workgroup) WorkgroupRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(It.IsAny<Workgroup>()))[0][0]; 
        //    Mock.Get(WorkgroupService).Verify(a => a.CreateWorkgroup(It.IsAny<Workgroup>(), It.IsAny<string[]>()));
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestCreatePostRedirectsToIndex2()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
        //    SetupDataForWorkgroupActions1(true);
        //    var workgroup = CreateValidEntities.Workgroup(1);
        //    var organizations = new List<Organization>();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        organizations.Add(CreateValidEntities.Organization(i+1));
        //        organizations[i].Id = (i + 1).ToString();
        //    }
        //    Mock.Get(WorkgroupService).Setup(a => a.CreateWorkgroup(It.IsAny<Workgroup>(), It.IsAny<string[]>())).Returns(workgroup);
        //    #endregion Arrange

        //    #region Act
        //    Controller.Create(workgroup, new[]{"2", "4"})
        //        .AssertActionRedirect()
        //        .ToAction<WorkgroupController>(a => a.Index(false));
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual("Name1 workgroup was created", Controller.Message);
        //    Mock.Get(WorkgroupService).Verify(a => a.CreateWorkgroup(It.IsAny<Workgroup>(), It.IsAny<string[]>()));
        //    var args = (string[])WorkgroupService.GetArgumentsForCallsMadeOn(a => a.CreateWorkgroup(It.IsAny<Workgroup>(), It.IsAny<string[]>()))[0][1];
        //    Assert.AreEqual(2, args.Count());
        //    Assert.AreEqual("2", args[0]);
        //    Assert.AreEqual("4", args[1]);

        //    #endregion Assert
        //}

        //#endregion Create Post Tests
    }
}
