using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Controllers;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Testing;

namespace Purchasing.Tests.ControllerTests.DepartmentalAdminRequestControllerTests
{
    public partial class DepartmentalAdminRequestControllerTests
    {
        #region Approve Get Tests

        [TestMethod]
        public void TestApproveGetRedirectsWhenDarNotFound()
        {
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            #endregion Arrange

            #region Act
            Controller.Approve("4")
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Request not found", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestApproveGetRedirectsWhenDarCompleted()
        {
            #region Arrange
            var dars = new List<DepartmentalAdminRequest>();
            dars.Add(CreateValidEntities.DepartmentalAdminRequest(1));
            dars[0].Complete = true;
            new FakeDepartmentalAdminRequests(0, DepartmentalAdminRequestRepository, dars, false);
            #endregion Arrange

            #region Act
            Controller.Approve("1")
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Request was already completed", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestApproveReturnsViewWithExpectedValues1()
        {
            //User does not exist
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            new FakeUsers(2, UserRepository);
            new FakeOrganizations(3, OrganizationRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Approve("3")
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
        public void TestApproveReturnsViewWithExpectedValues2()
        {
            //User does exist
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            new FakeUsers(3, UserRepository);
            new FakeOrganizations(3, OrganizationRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Approve("3")
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
        public void TestApproveReturnsViewWithExpectedValues3()
        {
            //1 org not found
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            new FakeUsers(3, UserRepository);
            new FakeOrganizations(1, OrganizationRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Approve("3")
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
        public void TestApproveReturnsViewWithExpectedValues4()
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
            var result = Controller.Approve("3")
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


        #endregion Approve Get Tests

        #region Approve Post Tests
        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestApproveThrowsExceptionIfRequestNotFound()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
                var daRequest = new DepartmentalAdminRequestViewModel();
                daRequest.DepartmentalAdminRequest = CreateValidEntities.DepartmentalAdminRequest(9);
                daRequest.DepartmentalAdminRequest.SetIdTo("9");
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Approve(daRequest, null, null);
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
        public void TestApprovePostRedirectsIfNoOrgsSelected1()
        {
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            var daRequest = new DepartmentalAdminRequestViewModel();
            daRequest.DepartmentalAdminRequest = CreateValidEntities.DepartmentalAdminRequest(9);
            daRequest.DepartmentalAdminRequest.SetIdTo("3");
            #endregion Arrange

            #region Act
            var result = Controller.Approve(daRequest, null, new List<string>{"1","2"})
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Approve("3"));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("3", result.RouteValues["id"]);
            Assert.AreEqual("Must select at least one organization", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestApprovePostRedirectsIfNoOrgsSelected2()
        {
            #region Arrange
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            var daRequest = new DepartmentalAdminRequestViewModel();
            daRequest.DepartmentalAdminRequest = CreateValidEntities.DepartmentalAdminRequest(9);
            daRequest.DepartmentalAdminRequest.SetIdTo("3");
            #endregion Arrange

            #region Act
            var result = Controller.Approve(daRequest, new List<string>(), new List<string> { "1", "2" })
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Approve("3"));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("3", result.RouteValues["id"]);
            Assert.AreEqual("Must select at least one organization", Controller.ErrorMessage);
            #endregion Assert
        }



        [TestMethod]
        public void TestApproveWhenNewUser()
        {
            #region Arrange
            new FakeDepartmentalAdminRequests(9, DepartmentalAdminRequestRepository);
            var daRequest = new DepartmentalAdminRequestViewModel();
            daRequest.DepartmentalAdminRequest = CreateValidEntities.DepartmentalAdminRequest(9);
            daRequest.DepartmentalAdminRequest.SetIdTo("9");
            new FakeUsers(3, UserRepository);
            new FakeOrganizations(10, OrganizationRepository);
            #endregion Arrange

            #region Act
            Controller.Approve(daRequest, new List<string> {"1","5"}, null)
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName9 LastName9 (9) Granted Departmental Admin Access", Controller.Message);
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var userArgs = (User) UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0];
            Assert.IsNotNull(userArgs);
            Assert.AreEqual("9", userArgs.Id);
            Assert.AreEqual("FirstName9", userArgs.FirstName);
            Assert.AreEqual("LastName9", userArgs.LastName);
            Assert.AreEqual("test9@testy.com", userArgs.Email);
            Assert.AreEqual(1, userArgs.Roles.Count);
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, userArgs.Roles[0].Id);

            DepartmentalAdminRequestRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything));
            var darArgs = (DepartmentalAdminRequest) DepartmentalAdminRequestRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything))[0][0];
            Assert.IsNotNull(darArgs);
            Assert.IsTrue(darArgs.Complete);
            UserIdentity.AssertWasCalled(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "9"));
            #endregion Assert		
        }

        [TestMethod]
        public void TestApproveWhenExistingUser1()
        {
            #region Arrange
            new FakeDepartmentalAdminRequests(9, DepartmentalAdminRequestRepository);
            var daRequest = new DepartmentalAdminRequestViewModel();
            daRequest.DepartmentalAdminRequest = CreateValidEntities.DepartmentalAdminRequest(9);
            daRequest.DepartmentalAdminRequest.SetIdTo("3");
            new FakeUsers(3, UserRepository);
            new FakeOrganizations(10, OrganizationRepository);
            #endregion Arrange

            #region Act
            Controller.Approve(daRequest, new List<string> { "1", "5" }, null)
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) Granted Departmental Admin Access", Controller.Message);
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var userArgs = (User)UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0];
            Assert.IsNotNull(userArgs);
            Assert.AreEqual("3", userArgs.Id);
            Assert.AreEqual("FirstName3", userArgs.FirstName);
            Assert.AreEqual("LastName3", userArgs.LastName);
            Assert.AreEqual("Email3@testy.com", userArgs.Email);
            Assert.AreEqual(1, userArgs.Roles.Count);
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, userArgs.Roles[0].Id);

            Assert.AreEqual(2, userArgs.Organizations.Count());
            Assert.AreEqual("1", userArgs.Organizations[0].Id);
            Assert.AreEqual("5", userArgs.Organizations[1].Id);

            DepartmentalAdminRequestRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything));
            var darArgs = (DepartmentalAdminRequest)DepartmentalAdminRequestRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything))[0][0];
            Assert.IsNotNull(darArgs);
            Assert.IsTrue(darArgs.Complete);
            UserIdentity.AssertWasCalled(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "3"));
            #endregion Assert
        }

        [TestMethod]
        public void TestApproveWhenExistingUser2()
        {
            #region Arrange
            new FakeDepartmentalAdminRequests(9, DepartmentalAdminRequestRepository);
            var daRequest = new DepartmentalAdminRequestViewModel();
            daRequest.DepartmentalAdminRequest = CreateValidEntities.DepartmentalAdminRequest(9);
            daRequest.DepartmentalAdminRequest.SetIdTo("3");
            new FakeOrganizations(10, OrganizationRepository);
            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].SetIdTo("3");
            users[0].Roles.Add(RoleRepository.GetById(Role.Codes.DepartmentalAdmin));
            users[0].Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "1"));
            users[0].Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "2"));
            new FakeUsers(0, UserRepository, users, true);
            #endregion Arrange

            #region Act
            Controller.Approve(daRequest, new List<string> { "1", "5" }, new List<string>{"1", "2"})
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) Replaced Departmental Admin Access", Controller.Message);
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var userArgs = (User)UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0];
            Assert.IsNotNull(userArgs);
            Assert.AreEqual("3", userArgs.Id);
            Assert.AreEqual("FirstName3", userArgs.FirstName);
            Assert.AreEqual("LastName3", userArgs.LastName);
            Assert.AreEqual("Email3@testy.com", userArgs.Email);
            Assert.AreEqual(1, userArgs.Roles.Count);
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, userArgs.Roles[0].Id);

            Assert.AreEqual(2, userArgs.Organizations.Count());
            Assert.AreEqual("1", userArgs.Organizations[0].Id);
            Assert.AreEqual("5", userArgs.Organizations[1].Id);

            DepartmentalAdminRequestRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything));
            var darArgs = (DepartmentalAdminRequest)DepartmentalAdminRequestRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything))[0][0];
            Assert.IsNotNull(darArgs);
            Assert.IsTrue(darArgs.Complete);
            UserIdentity.AssertWasCalled(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "3"));
            #endregion Assert
        }

        [TestMethod]
        public void TestApproveWhenExistingUser3()
        {
            #region Arrange
            new FakeDepartmentalAdminRequests(9, DepartmentalAdminRequestRepository);
            var daRequest = new DepartmentalAdminRequestViewModel();
            daRequest.DepartmentalAdminRequest = CreateValidEntities.DepartmentalAdminRequest(9);
            daRequest.DepartmentalAdminRequest.SetIdTo("3");
            daRequest.MergeExistingOrgs = true; //Merge Them
            new FakeOrganizations(10, OrganizationRepository);
            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].SetIdTo("3");
            users[0].Roles.Add(RoleRepository.GetById(Role.Codes.DepartmentalAdmin));
            users[0].Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "1"));
            users[0].Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "2"));
            new FakeUsers(0, UserRepository, users, true);
            #endregion Arrange

            #region Act
            Controller.Approve(daRequest, new List<string> { "1", "5" }, new List<string>{"1", "2"})
                .AssertActionRedirect()
                .ToAction<DepartmentalAdminRequestController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName3 LastName3 (3) Updated Departmental Admin Access", Controller.Message);
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var userArgs = (User)UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0];
            Assert.IsNotNull(userArgs);
            Assert.AreEqual("3", userArgs.Id);
            Assert.AreEqual("FirstName3", userArgs.FirstName);
            Assert.AreEqual("LastName3", userArgs.LastName);
            Assert.AreEqual("Email3@testy.com", userArgs.Email);
            Assert.AreEqual(1, userArgs.Roles.Count);
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, userArgs.Roles[0].Id);
            
            Assert.AreEqual(3, userArgs.Organizations.Count());
            Assert.AreEqual("1", userArgs.Organizations[0].Id);
            Assert.AreEqual("5", userArgs.Organizations[1].Id);
            Assert.AreEqual("2", userArgs.Organizations[2].Id);

            DepartmentalAdminRequestRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything));
            var darArgs = (DepartmentalAdminRequest)DepartmentalAdminRequestRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<DepartmentalAdminRequest>.Is.Anything))[0][0];
            Assert.IsNotNull(darArgs);
            Assert.IsTrue(darArgs.Complete);
            UserIdentity.AssertWasCalled(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "3"));
            #endregion Assert
        }

        #endregion Approve Post Tests

    }
}
