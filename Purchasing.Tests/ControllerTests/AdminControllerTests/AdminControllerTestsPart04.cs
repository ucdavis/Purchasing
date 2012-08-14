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
    }
}
