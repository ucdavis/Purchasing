using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Web.ActionResults;

namespace Purchasing.Tests.ControllerTests.DepartmentalAdminRequestControllerTests
{
    public partial class DepartmentalAdminRequestControllerTests
    {
        #region Deny Get Tests

        [TestMethod]
        public void TestDenyGetRedirectsWhenDarNotFound()
        {
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            #endregion Arrange

            #region Act
            Controller.Deny("4")
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Request not found", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestDenyGetRedirectsWhenDarCompleted()
        {
            #region Arrange
            var dars = new List<DepartmentalAdminRequest>();
            dars.Add(CreateValidEntities.DepartmentalAdminRequest(1));
            dars[0].Complete = true;
            new FakeDepartmentalAdminRequests(0, DepartmentalAdminRequestRepository, dars, false);
            #endregion Arrange

            #region Act
            Controller.Deny("1")
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Request was already completed", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestDenyReturnsViewWithExpectedValues1()
        {
            //User does not exist
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            new FakeUsers(2, UserRepository);
            new FakeOrganizations(3, OrganizationRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Deny("3")
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminRequestViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("3", result.DepartmentalAdminRequest.Id);
            Assert.IsFalse(result.UserExists);
            Assert.IsFalse(result.UserIsAlreadyDA);
            Assert.AreEqual(0, result.ExistingOrganizations.Count());
            Assert.AreEqual(2, result.Organizations.Count());
            Assert.AreEqual("1", result.Organizations[0].Id);
            Assert.AreEqual("2", result.Organizations[1].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestDenyReturnsViewWithExpectedValues2()
        {
            //User does exist
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            new FakeUsers(3, UserRepository);
            new FakeOrganizations(3, OrganizationRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Deny("3")
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminRequestViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("3", result.DepartmentalAdminRequest.Id);
            Assert.IsTrue(result.UserExists);
            Assert.IsFalse(result.UserIsAlreadyDA);
            Assert.AreEqual(0, result.ExistingOrganizations.Count());
            Assert.AreEqual(2, result.Organizations.Count());
            Assert.AreEqual("1", result.Organizations[0].Id);
            Assert.AreEqual("2", result.Organizations[1].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestDenyReturnsViewWithExpectedValues3()
        {
            //1 org not found
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            new FakeUsers(3, UserRepository);
            new FakeOrganizations(1, OrganizationRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Deny("3")
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminRequestViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("3", result.DepartmentalAdminRequest.Id);
            Assert.IsTrue(result.UserExists);
            Assert.IsFalse(result.UserIsAlreadyDA);
            Assert.AreEqual(0, result.ExistingOrganizations.Count());
            Assert.AreEqual(1, result.Organizations.Count());
            Assert.AreEqual("1", result.Organizations[0].Id);
            //Assert.AreEqual("2", result.Organizations[1].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestDenyReturnsViewWithExpectedValues4()
        {
            //User does exist and is a da already
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            new FakeOrganizations(10, OrganizationRepository);

            var users = new List<User>();
            users.Add(CreateValidEntities.User(1));
            users[0].SetIdTo("3");
            users[0].Roles = new List<Role>();
            users[0].Roles.Add(new Role(Role.Codes.DepartmentalAdmin));
            users[0].Organizations = new List<Organization>();
            users[0].Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "6"));
            users[0].Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "8"));
            new FakeUsers(0, UserRepository, users, true);
            #endregion Arrange

            #region Act
            var result = Controller.Deny("3")
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminRequestViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("3", result.DepartmentalAdminRequest.Id);
            Assert.IsTrue(result.UserExists);
            Assert.IsTrue(result.UserIsAlreadyDA);
            Assert.AreEqual(2, result.ExistingOrganizations.Count());
            Assert.AreEqual("6", result.ExistingOrganizations[0].Id);
            Assert.AreEqual("8", result.ExistingOrganizations[1].Id);
            Assert.AreEqual(2, result.Organizations.Count());
            Assert.AreEqual("1", result.Organizations[0].Id);
            Assert.AreEqual("2", result.Organizations[1].Id);
            #endregion Assert
        }


        #endregion Deny Get Tests

        #region Deny Post Tests

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestDenyPostThrowsExceptionIdDarNotFound()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
                var dar = new DepartmentalAdminRequestViewModel();
                dar.DepartmentalAdminRequest = new DepartmentalAdminRequest("5");
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Deny(dar);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains no matching element", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestDenyPostRedirectsWhenAlreadyCompleted()
        {
            #region Arrange
            var dars = new List<DepartmentalAdminRequest>();
            dars.Add(CreateValidEntities.DepartmentalAdminRequest(3));
            dars[0].SetIdTo("3");
            dars[0].Complete = true;
            new FakeDepartmentalAdminRequests(0, DepartmentalAdminRequestRepository, dars, true);
            var dar = new DepartmentalAdminRequestViewModel
                          {DepartmentalAdminRequest = new DepartmentalAdminRequest("3")};
            #endregion Arrange

            #region Act
            Controller.Deny(dar)
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Request was already completed", Controller.Message);
            DepartmentalAdminRequestRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything));
            DepartmentalAdminRequestRepository.AssertWasNotCalled(a => a.Remove(Arg<DepartmentalAdminRequest>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestDenyPostRedirectsWhenValid()
        {
            #region Arrange
            var dars = new List<DepartmentalAdminRequest>();
            dars.Add(CreateValidEntities.DepartmentalAdminRequest(3));
            dars[0].SetIdTo("3");
            dars[0].Complete = false;
            new FakeDepartmentalAdminRequests(0, DepartmentalAdminRequestRepository, dars, true);
            var dar = new DepartmentalAdminRequestViewModel { DepartmentalAdminRequest = new DepartmentalAdminRequest("3") };
            #endregion Arrange

            #region Act
            Controller.Deny(dar)
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Request Denied for FirstName3 LastName3 (3)", Controller.Message);
            DepartmentalAdminRequestRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything));
            var args = (DepartmentalAdminRequest) DepartmentalAdminRequestRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything))[0][0]; 
            Assert.IsTrue(args.Complete);

            DepartmentalAdminRequestRepository.AssertWasNotCalled(a => a.Remove(Arg<DepartmentalAdminRequest>.Is.Anything));
            #endregion Assert
        }
        
        #endregion Deny Post Tests

        #region SearchOrgs Tests

        [TestMethod]
        public void TestSearchOrgsWhenNoOrgsFound()
        {
            #region Arrange
            new FakeOrganizations(3, OrganizationRepository);
            #endregion Arrange

            #region Act
            var results = Controller.SearchOrgs("test")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[]", results.JsonResultString);
            #endregion Assert
        }

        [TestMethod]
        public void TestSearchOrgsWhenOrgsFound1()
        {
            #region Arrange
            new FakeOrganizations(3, OrganizationRepository);
            #endregion Arrange

            #region Act
            var results = Controller.SearchOrgs("Name1")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"id\":\"1\",\"label\":\"Name1 (1)\"}]", results.JsonResultString);
            #endregion Assert
        }

        [TestMethod]
        public void TestSearchOrgsWhenOrgsFound2()
        {
            #region Arrange
            new FakeOrganizations(3, OrganizationRepository);
            #endregion Arrange

            #region Act
            var results = Controller.SearchOrgs("2")
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual("[{\"id\":\"2\",\"label\":\"Name2 (2)\"}]", results.JsonResultString);
            #endregion Assert
        }

        #endregion SearchOrgs Tests
    }
}
