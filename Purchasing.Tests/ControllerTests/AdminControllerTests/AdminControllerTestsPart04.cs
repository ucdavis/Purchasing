using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Controllers;
using Purchasing.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

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
            dynamic data = result.Data;
            Assert.IsFalse(data.success);
            Assert.AreEqual("Value cannot be null.\r\nParameter name: source", data.message);
            WorkgroupService.AssertWasNotCalled(a => a.AddRelatedAdminUsers(Arg<Workgroup>.Is.Anything));
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
            dynamic data = result.Data;
            Assert.IsFalse(data.success);
            Assert.AreEqual("Sequence contains no matching element", data.message);
            WorkgroupService.AssertWasNotCalled(a => a.AddRelatedAdminUsers(Arg<Workgroup>.Is.Anything));
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
            dynamic data = result.Data;
            Assert.IsFalse(data.success);
            Assert.AreEqual("Precondition failed.", data.message);
            WorkgroupService.AssertWasNotCalled(a => a.AddRelatedAdminUsers(Arg<Workgroup>.Is.Anything));
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
            dynamic data = result.Data;
            Assert.IsFalse(data.success);
            Assert.AreEqual("Precondition failed.", data.message);
            WorkgroupService.AssertWasNotCalled(a => a.AddRelatedAdminUsers(Arg<Workgroup>.Is.Anything));
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
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
            Assert.AreEqual("Updated", data.message);
            WorkgroupService.AssertWasCalled(a => a.AddRelatedAdminUsers(RepositoryFactory.WorkgroupRepository.Queryable.Single(b => b.Id == 5)));
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
            dynamic data = result.Data;
            Assert.IsFalse(data.success);
            Assert.AreEqual("Value cannot be null.\r\nParameter name: source", data.message);
            WorkgroupService.AssertWasNotCalled(a => a.GetChildWorkgroups(Arg<int>.Is.Anything));
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
            dynamic data = result.Data;
            Assert.IsFalse(data.success);
            Assert.AreEqual("Precondition failed.", data.message);
            WorkgroupService.AssertWasNotCalled(a => a.GetChildWorkgroups(Arg<int>.Is.Anything));
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
            dynamic data = result.Data;
            Assert.IsFalse(data.success);
            Assert.AreEqual("Precondition failed.", data.message);
            WorkgroupService.AssertWasNotCalled(a => a.GetChildWorkgroups(Arg<int>.Is.Anything));
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
            WorkgroupService.Expect(a => a.GetChildWorkgroups(5)).Return(new List<int>() {5, 2, 6, 7}).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.GetChildWorkgroupIds(5)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
            Assert.AreEqual(" 2 5 6 7", data.message);
            WorkgroupService.AssertWasCalled(a => a.GetChildWorkgroups(5));
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
            WorkgroupService.Expect(a => a.GetChildWorkgroups(5)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.GetChildWorkgroupIds(5)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
            Assert.AreEqual("None", data.message);
            WorkgroupService.AssertWasCalled(a => a.GetChildWorkgroups(5));
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
            WorkgroupService.Expect(a => a.GetChildWorkgroups(5)).Return(new List<int>()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.GetChildWorkgroupIds(5)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
            Assert.AreEqual("None", data.message);
            WorkgroupService.AssertWasCalled(a => a.GetChildWorkgroups(5));
            #endregion Assert
        }
        #endregion GetChildWorkgroupIds Tests
    }
}
