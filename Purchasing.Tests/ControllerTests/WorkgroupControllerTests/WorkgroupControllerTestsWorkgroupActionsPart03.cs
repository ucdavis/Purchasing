using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Rhino.Mocks;
using UCDArch.Testing;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region Delete Get Tests

        [TestMethod]
        public void TestDeleteGetRedirectsToIndexWhenWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.Delete(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeleteGetReturnsView()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(3)
                .AssertViewRendered()
                .WithViewData<Workgroup>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Name3", result.Name);
            #endregion Assert
        } 
        #endregion Delete Get Tests

        #region Delete Post Tests

        [TestMethod]
        public void TestDeletePostRedirectsToIndexWhenWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            var worgroupToDelete = CreateValidEntities.Workgroup(4);
            worgroupToDelete.SetIdTo(4);
            #endregion Arrange

            #region Act
            Controller.Delete(4,worgroupToDelete)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            WorkgroupRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            WorkgroupRepository.AssertWasNotCalled(a => a.Remove(Arg<Workgroup>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePostSetsValueAndSaves()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (var i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i+1));
                workgroups[i].IsActive = true;
            }
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
            var worgroupToDelete = CreateValidEntities.Workgroup(3);
            worgroupToDelete.SetIdTo(3);
            #endregion Arrange

            #region Act
            Controller.Delete(3, worgroupToDelete)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Name3 was disabled successfully", Controller.Message);
            WorkgroupRepository.AssertWasNotCalled(a => a.Remove(Arg<Workgroup>.Is.Anything));
            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup) WorkgroupRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);
            Assert.IsFalse(args.IsActive);
            #endregion Assert		
        }
        #endregion Delete Post Tests
    }
}
