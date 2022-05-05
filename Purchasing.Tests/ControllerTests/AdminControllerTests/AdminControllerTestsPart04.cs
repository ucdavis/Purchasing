using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Tests.ControllerTests.AdminControllerTests
{
    public partial class AdminControllerTests
    {
        #region UpdateChildWorkgroups Tests

        [TestMethod]
        public void TestUpdateChildWorkgroupsReturnsView()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 5; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i+1));
                workgroups[i].IsActive = true;
                workgroups[i].Administrative = true;
            }

            workgroups[1].Administrative = false;
            workgroups[2].IsActive = false;
            workgroups[3].Administrative = false;
            workgroups[3].IsActive = false;

            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);

            #endregion Arrange

            #region Act
            var results = Controller.UpdateChildWorkgroups()
                .AssertViewRendered()
                .WithViewData<IList<Workgroup>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Name1", results[0].Name);
            Assert.AreEqual("Name5", results[1].Name);
            #endregion Assert		
        } 

        #endregion UpdateChildWorkgroups Tests

        #region ProcessWorkGroup Tests

        [TestMethod]
        public void TestProcessWorkGroupReturnsJsonNetResult1()
        {
            #region Arrange
            //Nothing because it catches exceptions
            #endregion Arrange

            #region Act
            var result = Controller.ProcessWorkGroup(99)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsFalse(data.success.Value);
            Assert.AreEqual("Value cannot be null.\r\nParameter name: source", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.AddRelatedAdminUsers(Moq.It.IsAny<Workgroup>()), Moq.Times.Never());
            #endregion Assert		
        }

        [TestMethod]
        public void TestProcessWorkGroupReturnsJsonNetResult2()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 5; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].IsActive = true;
                workgroups[i].Administrative = true;
            }

            workgroups[1].Administrative = false;
            workgroups[2].IsActive = false;
            workgroups[3].Administrative = false;
            workgroups[3].IsActive = false;

            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            var result = Controller.ProcessWorkGroup(6)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsFalse(data.success.Value);
            Assert.AreEqual("Sequence contains no matching element", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.AddRelatedAdminUsers(Moq.It.IsAny<Workgroup>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestProcessWorkGroupReturnsJsonNetResult3()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 5; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].IsActive = true;
                workgroups[i].Administrative = true;
            }

            workgroups[1].Administrative = false;
            workgroups[2].IsActive = false;
            workgroups[3].Administrative = false;
            workgroups[3].IsActive = false;

            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            var result = Controller.ProcessWorkGroup(2)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsFalse(data.success.Value);
            Assert.AreEqual("Precondition failed.", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.AddRelatedAdminUsers(Moq.It.IsAny<Workgroup>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestProcessWorkGroupReturnsJsonNetResult4()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 5; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].IsActive = true;
                workgroups[i].Administrative = true;
            }

            workgroups[1].Administrative = false;
            workgroups[2].IsActive = false;
            workgroups[3].Administrative = false;
            workgroups[3].IsActive = false;

            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            var result = Controller.ProcessWorkGroup(3)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsFalse(data.success.Value);
            Assert.AreEqual("Precondition failed.", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.AddRelatedAdminUsers(Moq.It.IsAny<Workgroup>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestProcessWorkGroupReturnsJsonNetResult5()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 5; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].IsActive = true;
                workgroups[i].Administrative = true;
            }

            workgroups[1].Administrative = false;
            workgroups[2].IsActive = false;
            workgroups[3].Administrative = false;
            workgroups[3].IsActive = false;

            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            var result = Controller.ProcessWorkGroup(5)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsTrue(data.success.Value);
            Assert.AreEqual("Updated", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.AddRelatedAdminUsers(RepositoryFactory.WorkgroupRepository.Queryable.Single(b => b.Id == 5)));
            #endregion Assert
        }
        #endregion ProcessWorkGroup Tests

        #region GetChildWorkgroupIds Tests
        [TestMethod]
        public void TestGetChildWorkgroupIdsReturnsJsonNetResult1()
        {
            #region Arrange
            //Nothing because it catches exceptions
            #endregion Arrange

            #region Act
            var result = Controller.GetChildWorkgroupIds(99)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsFalse(data.success.Value);
            Assert.AreEqual("Value cannot be null.\r\nParameter name: source", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.GetChildWorkgroups(Moq.It.IsAny<int>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestGetChildWorkgroupIdsReturnsJsonNetResult2()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 5; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].IsActive = true;
                workgroups[i].Administrative = true;
            }

            workgroups[1].Administrative = false;
            workgroups[2].IsActive = false;
            workgroups[3].Administrative = false;
            workgroups[3].IsActive = false;

            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            var result = Controller.GetChildWorkgroupIds(2)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsFalse(data.success.Value);
            Assert.AreEqual("Precondition failed.", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.GetChildWorkgroups(Moq.It.IsAny<int>()), Moq.Times.Never());
            #endregion Assert
        }
        [TestMethod]
        public void TestGetChildWorkgroupIdsReturnsJsonNetResult3()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 5; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].IsActive = true;
                workgroups[i].Administrative = true;
            }

            workgroups[1].Administrative = false;
            workgroups[2].IsActive = false;
            workgroups[3].Administrative = false;
            workgroups[3].IsActive = false;

            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            var result = Controller.GetChildWorkgroupIds(3)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsFalse(data.success.Value);
            Assert.AreEqual("Precondition failed.", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.GetChildWorkgroups(Moq.It.IsAny<int>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestGetChildWorkgroupIdsReturnsJsonNetResult4()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 5; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].IsActive = true;
                workgroups[i].Administrative = true;
            }

            workgroups[1].Administrative = false;
            workgroups[2].IsActive = false;
            workgroups[3].Administrative = false;
            workgroups[3].IsActive = false;

            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            Moq.Mock.Get(WorkgroupService).Setup(a => a.GetChildWorkgroups(5)).Returns(new List<int>() {5, 2, 6, 7});
            #endregion Arrange

            #region Act
            var result = Controller.GetChildWorkgroupIds(5)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsTrue(data.success.Value);
            Assert.AreEqual(" 2 5 6 7", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.GetChildWorkgroups(5));
            #endregion Assert
        }

        [TestMethod]
        public void TestGetChildWorkgroupIdsReturnsJsonNetResult5()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 5; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].IsActive = true;
                workgroups[i].Administrative = true;
            }

            workgroups[1].Administrative = false;
            workgroups[2].IsActive = false;
            workgroups[3].Administrative = false;
            workgroups[3].IsActive = false;

            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            Moq.Mock.Get(WorkgroupService).Setup(a => a.GetChildWorkgroups(5)).Returns<List<int>>(null);
            #endregion Arrange

            #region Act
            var result = Controller.GetChildWorkgroupIds(5)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsTrue(data.success.Value);
            Assert.AreEqual("None", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.GetChildWorkgroups(5));
            #endregion Assert
        }

        [TestMethod]
        public void TestGetChildWorkgroupIdsReturnsJsonNetResult6()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 5; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].IsActive = true;
                workgroups[i].Administrative = true;
            }

            workgroups[1].Administrative = false;
            workgroups[2].IsActive = false;
            workgroups[3].Administrative = false;
            workgroups[3].IsActive = false;

            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            Moq.Mock.Get(WorkgroupService).Setup(a => a.GetChildWorkgroups(5)).Returns(new List<int>());
            #endregion Arrange

            #region Act
            var result = Controller.GetChildWorkgroupIds(5)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = JObject.FromObject(result.Data);
            Assert.IsTrue(data.success.Value);
            Assert.AreEqual("None", data.message.Value);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.GetChildWorkgroups(5));
            #endregion Assert
        }
        #endregion GetChildWorkgroupIds Tests

        #region ValidateChildWorkgroups Tests


        [TestMethod]
        public void TestValidateChildWorkgroupsReturnsView1()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i+1));
                workgroups[i].Administrative = true; //no child workgroups
            }
            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            var result = Controller.ValidateChildWorkgroups()
                .AssertViewRendered()
                .WithViewData<List<ValidateChildWorkgroupsViewModel>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.GetParentWorkgroups(Moq.It.IsAny<int>()), Moq.Times.Never());
            #endregion Assert		
        }

        [TestMethod]
        public void TestValidateChildWorkgroupsReturnsView2()
        {
            #region Arrange
            new FakeUsers(4, UserRepository);
            SetupRoles();

            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].Administrative = true; //no child workgroups
            }
            workgroups[1].Administrative = false;
            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);

            Moq.Mock.Get(WorkgroupService).Setup(a => a.GetParentWorkgroups(2)).Returns(new List<int>() { 1 });

            var workgroupPermissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 7; i++)
            {
                workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i + 1));
            }

            //Parent Permissions
            workgroupPermissions[0].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[1].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[2].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);

            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "3");

            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);

            //Child Permissions 
            workgroupPermissions[3].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[4].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[5].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[6].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);

            //Non Admin permission
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);

            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[4].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[4].IsAdmin = true;

            workgroupPermissions[5].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[5].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[5].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[5].IsAdmin = true;

            workgroupPermissions[6].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[6].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[6].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[6].IsAdmin = true;

            new FakeWorkgroupPermissions(0, RepositoryFactory.WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            var result = Controller.ValidateChildWorkgroups()
                .AssertViewRendered()
                .WithViewData<List<ValidateChildWorkgroupsViewModel>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            Moq.Mock.Get(WorkgroupService).Verify(a => a.GetParentWorkgroups(2));
            #endregion Assert
        }

        [TestMethod]
        public void TestValidateChildWorkgroupsReturnsView3()
        {
            #region Arrange
            new FakeUsers(4, UserRepository);
            SetupRoles();

            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].Administrative = true; //no child workgroups
            }
            workgroups[1].Administrative = false;
            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);

            Moq.Mock.Get(WorkgroupService).Setup(a => a.GetParentWorkgroups(2)).Returns(new List<int>() {1});

            var workgroupPermissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 6; i++)
            {
                workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i+1));                
            }

            //Parent Permissions
            workgroupPermissions[0].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[1].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[2].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);

            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "3");

            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);

            //Child Permissions (With missing permission Account Manager)
            workgroupPermissions[3].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[4].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[5].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);

            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);

            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[4].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[4].IsAdmin = true;

            workgroupPermissions[5].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[5].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[5].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[5].IsAdmin = true;

            new FakeWorkgroupPermissions(0, RepositoryFactory.WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            var result = Controller.ValidateChildWorkgroups()
                .AssertViewRendered()
                .WithViewData<List<ValidateChildWorkgroupsViewModel>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result[0].ExtraChildPermissions.Count);
            Assert.AreEqual(1, result[0].MissingChildPermissions.Count);
            Assert.AreEqual(false, result[0].MissingChildPermissions[0].IsFullFeatured);
            Assert.AreEqual(Role.Codes.AccountManager, result[0].MissingChildPermissions[0].Role.Id);
            Assert.AreEqual("2", result[0].MissingChildPermissions[0].User.Id);
            Assert.AreEqual(1, result[0].MissingChildPermissions[0].Workgroup.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestValidateChildWorkgroupsReturnsView4()
        {
            #region Arrange
            new FakeUsers(4, UserRepository);
            SetupRoles();

            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].Administrative = true; //no child workgroups
            }
            workgroups[1].Administrative = false;
            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);

            Moq.Mock.Get(WorkgroupService).Setup(a => a.GetParentWorkgroups(2)).Returns(new List<int>() { 1 });

            var workgroupPermissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 7; i++)
            {
                workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i + 1));
            }

            //Parent Permissions
            workgroupPermissions[0].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[1].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[2].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);

            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "3");

            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);

            //Child Permissions 
            workgroupPermissions[3].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[4].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[5].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[6].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);

            //Non Admin permission
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);

            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[4].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[4].IsAdmin = true;

            workgroupPermissions[5].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[5].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[5].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[5].IsAdmin = true;

            workgroupPermissions[6].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[6].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[6].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[6].IsAdmin = true;

            workgroupPermissions[6].IsFullFeatured = true; //different from parent

            new FakeWorkgroupPermissions(0, RepositoryFactory.WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            var result = Controller.ValidateChildWorkgroups()
                .AssertViewRendered()
                .WithViewData<List<ValidateChildWorkgroupsViewModel>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].ExtraChildPermissions.Count);
            Assert.AreEqual(1, result[0].MissingChildPermissions.Count);
            Assert.AreEqual(false, result[0].MissingChildPermissions[0].IsFullFeatured);
            Assert.AreEqual(Role.Codes.AccountManager, result[0].MissingChildPermissions[0].Role.Id);
            Assert.AreEqual("2", result[0].MissingChildPermissions[0].User.Id);
            Assert.AreEqual(1, result[0].MissingChildPermissions[0].Workgroup.Id);

            Assert.AreEqual(true, result[0].ExtraChildPermissions[0].IsFullFeatured);
            Assert.AreEqual(Role.Codes.AccountManager, result[0].ExtraChildPermissions[0].Role.Id);
            Assert.AreEqual("2", result[0].ExtraChildPermissions[0].User.Id);
            Assert.AreEqual(2, result[0].ExtraChildPermissions[0].Workgroup.Id);
            Assert.AreEqual(1, result[0].ExtraChildPermissions[0].ParentWorkgroup.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestValidateChildWorkgroupsReturnsView5()
        {
            #region Arrange
            new FakeUsers(4, UserRepository);
            SetupRoles();

            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].Administrative = true; //no child workgroups
            }
            workgroups[1].Administrative = false;
            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);

            Moq.Mock.Get(WorkgroupService).Setup(a => a.GetParentWorkgroups(2)).Returns(new List<int>() { 1 });

            var workgroupPermissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 7; i++)
            {
                workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i + 1));
            }

            //Parent Permissions
            workgroupPermissions[0].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[1].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 3); //This one not there
            workgroupPermissions[2].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);  

            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "3");

            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);

            //Child Permissions 
            workgroupPermissions[3].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[4].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[5].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[6].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);

            //Non Admin permission
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);

            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[4].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[4].IsAdmin = true;

            workgroupPermissions[5].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[5].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[5].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[5].IsAdmin = true;

            workgroupPermissions[6].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[6].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[6].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[6].IsAdmin = true;

            new FakeWorkgroupPermissions(0, RepositoryFactory.WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            var result = Controller.ValidateChildWorkgroups()
                .AssertViewRendered()
                .WithViewData<List<ValidateChildWorkgroupsViewModel>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].ExtraChildPermissions.Count);
            Assert.AreEqual(0, result[0].MissingChildPermissions.Count);
            //Assert.AreEqual(false, result[0].MissingChildPermissions[0].IsFullFeatured);
            //Assert.AreEqual(Role.Codes.AccountManager, result[0].MissingChildPermissions[0].Role.Id);
            //Assert.AreEqual("2", result[0].MissingChildPermissions[0].User.Id);
            //Assert.AreEqual(1, result[0].MissingChildPermissions[0].Workgroup.Id);

            Assert.AreEqual(false, result[0].ExtraChildPermissions[0].IsFullFeatured);
            Assert.AreEqual(Role.Codes.AccountManager, result[0].ExtraChildPermissions[0].Role.Id);
            Assert.AreEqual("2", result[0].ExtraChildPermissions[0].User.Id);
            Assert.AreEqual(2, result[0].ExtraChildPermissions[0].Workgroup.Id);
            Assert.AreEqual(1, result[0].ExtraChildPermissions[0].ParentWorkgroup.Id);
            #endregion Assert
        }
        #endregion ValidateChildWorkgroups Tests

        #region RemoveExtraChildPermissions

        [TestMethod]
        public void TestRemoveExtraChildPermissions1()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].Administrative = true; //no child workgroups
            }
            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            Controller.RemoveExtraChildPermissions()
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Moq.Mock.Get(WorkgroupService).Verify(a => a.GetParentWorkgroups(Moq.It.IsAny<int>()), Moq.Times.Never());
            Moq.Mock.Get(RepositoryFactory.WorkgroupPermissionRepository).Verify(a => a.Remove(Moq.It.IsAny<WorkgroupPermission>()), Moq.Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveExtraChildPermissions2()
        {
            #region Arrange
            new FakeUsers(4, UserRepository);
            SetupRoles();

            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].Administrative = true; //no child workgroups
            }
            workgroups[1].Administrative = false;
            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);

            Moq.Mock.Get(WorkgroupService).Setup(a => a.GetParentWorkgroups(2)).Returns(new List<int>() { 1 });

            var workgroupPermissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 6; i++)
            {
                workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i + 1));
            }

            //Parent Permissions
            workgroupPermissions[0].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[1].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[2].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);

            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "3");

            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);

            //Child Permissions (With missing permission Account Manager)
            workgroupPermissions[3].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[4].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[5].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);

            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);

            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[4].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[4].IsAdmin = true;

            workgroupPermissions[5].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[5].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[5].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[5].IsAdmin = true;

            new FakeWorkgroupPermissions(0, RepositoryFactory.WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            Controller.RemoveExtraChildPermissions()
                            .AssertActionRedirect();
            #endregion Act

            #region Assert
            Moq.Mock.Get(RepositoryFactory.WorkgroupPermissionRepository).Verify(a => a.Remove(Moq.It.IsAny<WorkgroupPermission>()), Moq.Times.Never());
            Assert.AreEqual("0 permissions removed", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestRemoveExtraChildPermissions3()
        {
            #region Arrange
            new FakeUsers(4, UserRepository);
            SetupRoles();

            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].Administrative = true; //no child workgroups
            }
            workgroups[1].Administrative = false;
            new FakeWorkgroups(0, RepositoryFactory.WorkgroupRepository, workgroups);

            Moq.Mock.Get(WorkgroupService).Setup(a => a.GetParentWorkgroups(2)).Returns(new List<int>() { 1 });

            var workgroupPermissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 7; i++)
            {
                workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i + 1));
            }

            //Parent Permissions
            workgroupPermissions[0].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[1].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[2].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);

            workgroupPermissions[0].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[1].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[2].User = UserRepository.Queryable.Single(a => a.Id == "3");

            workgroupPermissions[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);

            //Child Permissions 
            workgroupPermissions[3].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[4].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[5].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            workgroupPermissions[6].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);

            //Non Admin permission
            workgroupPermissions[3].User = UserRepository.Queryable.Single(a => a.Id == "4");
            workgroupPermissions[3].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);

            workgroupPermissions[4].User = UserRepository.Queryable.Single(a => a.Id == "1");
            workgroupPermissions[4].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            workgroupPermissions[4].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[4].IsAdmin = true;

            workgroupPermissions[5].User = UserRepository.Queryable.Single(a => a.Id == "3");
            workgroupPermissions[5].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Purchaser);
            workgroupPermissions[5].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[5].IsAdmin = true;

            workgroupPermissions[6].User = UserRepository.Queryable.Single(a => a.Id == "2");
            workgroupPermissions[6].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            workgroupPermissions[6].ParentWorkgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            workgroupPermissions[6].IsAdmin = true;

            workgroupPermissions[6].IsFullFeatured = true; //different from parent

            new FakeWorkgroupPermissions(0, RepositoryFactory.WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            Controller.RemoveExtraChildPermissions()
                            .AssertActionRedirect();
            #endregion Act

            #region Assert
            Moq.Mock.Get(RepositoryFactory.WorkgroupPermissionRepository).Verify(a => a.Remove(Moq.It.IsAny<WorkgroupPermission>()));
//TODO: Arrange
            WorkgroupPermission args = default;
            Moq.Mock.Get(RepositoryFactory.WorkgroupPermissionRepository).Setup(a => a.Remove(Moq.It.IsAny<WorkgroupPermission>()))
                .Callback<WorkgroupPermission>(x => args = x);
//ENDTODO 
            Assert.AreEqual(7, args.Id);
            Assert.AreEqual("1 permissions removed", Controller.Message);
            #endregion Assert
        }
        #endregion RemoveExtraChildPermissions
    }
}
