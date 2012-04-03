using System;
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
        #region Details Tests

        [TestMethod]
        public void TestDetailsRedirectsToIndexIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.Details(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestDetailsReturnsView1()
        {
            #region Arrange
            SetupDataForDetails();
            new FakeConditionalApprovals(3, ConditionalApprovalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Details(3)
                .AssertViewRendered()
                .WithViewData<WorkgroupDetailsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.AccountCount);
            Assert.AreEqual(4, result.AddressCount);
            Assert.AreEqual(2, result.OrganizationCount);
            Assert.AreEqual(1, result.VendorCount);
            Assert.AreEqual(2, result.ApproverCount);
            Assert.AreEqual(3, result.AccountManagerCount);
            Assert.AreEqual(4, result.PurchaserCount);
            Assert.AreEqual(1, result.UserCount);
            #endregion Assert		
        }
        #endregion Details Tests

        #region Edit Get Tests
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestEditGetThrowsExceptionIfNoCurrentUser()
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
                Controller.Edit(3);
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
        public void TestEditGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3)
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Organizations.Count());
            Assert.AreEqual("Name1", result.Organizations[0].ToString());
            Assert.AreEqual("Name3", result.Organizations[1].ToString());
            Assert.AreEqual("Name4", result.Organizations[2].ToString());
            Assert.AreEqual("Name5", result.Organizations[3].ToString());
            Assert.AreEqual("Name6", result.Organizations[4].ToString());
            Assert.IsNotNull(result.Workgroup);
            Assert.AreEqual("Name3", result.Workgroup.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "1");
            SetupDataForWorkgroupActions1();
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3)
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Organizations.Count());
            Assert.AreEqual("Name1", result.Organizations[0].ToString());
            Assert.AreEqual("Name3", result.Organizations[1].ToString());
            Assert.AreEqual("Name2", result.Organizations[2].ToString());
            Assert.IsNotNull(result.Workgroup);
            Assert.AreEqual("Name3", result.Workgroup.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
            SetupDataForWorkgroupActions1();
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3)
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Organizations.Count());
            Assert.AreEqual("Name1", result.Organizations[0].ToString());
            Assert.AreEqual("Name3", result.Organizations[1].ToString());
            Assert.AreEqual("Name7", result.Organizations[2].ToString());
            Assert.AreEqual("Name8", result.Organizations[3].ToString());
            Assert.AreEqual("Name9", result.Organizations[4].ToString());
            Assert.IsNotNull(result.Workgroup);
            Assert.AreEqual("Name3", result.Workgroup.Name);
            #endregion Assert
        }
        #endregion Edit Get Tests

        #region Edit Post Tests

        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            Controller.ModelState.AddModelError("Fake", "Error");
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, WorkgroupRepository.GetNullableById(3), null)
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Organizations.Count());
            Assert.AreEqual("Name1", result.Organizations[0].ToString());
            Assert.AreEqual("Name3", result.Organizations[1].ToString());
            Assert.AreEqual("Name3", result.Organizations[2].ToString());
            Assert.AreEqual("Name4", result.Organizations[3].ToString());
            Assert.AreEqual("Name5", result.Organizations[4].ToString());
            Assert.AreEqual("Name6", result.Organizations[5].ToString());
            Assert.IsNotNull(result.Workgroup);
            Assert.AreEqual("Name3", result.Workgroup.Name);
            WorkgroupRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            #endregion Assert	
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            Controller.ModelState.AddModelError("Fake", "Error");
            var orgs = new[] {"2", "4"};
            new FakeOrganizationDescendants(3, OrganizationDescendantRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, WorkgroupRepository.GetNullableById(3), orgs)
                .AssertViewRendered()
                .WithViewData<WorkgroupModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Organizations.Count());
            Assert.AreEqual("Name2", result.Organizations[0].ToString());
            Assert.AreEqual("Name4", result.Organizations[1].ToString());
            Assert.AreEqual("Name3", result.Organizations[2].ToString());
            Assert.AreEqual("Name5", result.Organizations[3].ToString());
            Assert.AreEqual("Name6", result.Organizations[4].ToString());
            Assert.IsNotNull(result.Workgroup);
            Assert.AreEqual("Name3", result.Workgroup.Name);
            Assert.AreEqual(3, result.Workgroup.Organizations.Count);
            Assert.AreEqual("Name2", result.Workgroup.Organizations[0].Name);
            Assert.AreEqual("Name4", result.Workgroup.Organizations[1].Name);
            Assert.AreEqual("Name3", result.Workgroup.Organizations[2].Name);
            WorkgroupRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostRedirectsWhenWorkgroupNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            var workgroup = CreateValidEntities.Workgroup(9);
            workgroup.SetIdTo(WorkgroupRepository.Queryable.Max(a => a.Id) + 1);
            #endregion Arrange

            #region Act
            Controller.Edit(0, workgroup, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditPostReplacesOrganizationsWithSuppliedOnes1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            var orgs = new[] { "1", "7" };

            #endregion Arrange

            #region Act
            Controller.Edit(3, WorkgroupRepository.GetNullableById(3), orgs)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Name3 was modified successfully", Controller.Message);
            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup) WorkgroupRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);
            Assert.AreEqual(3, args.Organizations.Count());
            Assert.AreEqual("Name1", args.Organizations[0].Name);
            Assert.AreEqual("Name7", args.Organizations[1].Name);
            Assert.AreEqual("Name3", args.Organizations[2].Name);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostReplacesOrganizationsWithSuppliedOnes2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            var orgs = new[] { "1", "3" };
            #endregion Arrange

            #region Act
            Controller.Edit(3, WorkgroupRepository.GetNullableById(3), orgs)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index(false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Name3 was modified successfully", Controller.Message);
            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything));
            var args = (Workgroup)WorkgroupRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Workgroup>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", args.Name);
            Assert.AreEqual(3, args.Organizations.Count());
            Assert.AreEqual("Name1", args.Organizations[0].Name);
            Assert.AreEqual("Name3", args.Organizations[1].Name);
            Assert.AreEqual("Name3", args.Organizations[2].Name); //Limitation of the test, this doesn't happen when running normally which is correct
            #endregion Assert
        }
        #endregion Edit Post Tests
    }
}
