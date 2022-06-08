using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Services;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Purchasing.Tests.ControllerTests.DepartmentalAdminRequestControllerTests
{
    public partial class DepartmentalAdminRequestControllerTests
    {

        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView()
        {
            #region Arrange
            var daRequest = new List<DepartmentalAdminRequest>();
            for (int i = 0; i < 4; i++)
            {
                daRequest.Add(CreateValidEntities.DepartmentalAdminRequest(i + 1));
                daRequest[i].Complete = false;
                daRequest[i].DateCreated = DateTime.UtcNow.ToPacificTime().Date.AddDays(-i);
            }
            daRequest[1].Complete = true;
            new FakeDepartmentalAdminRequests(0, DepartmentalAdminRequestRepository, daRequest, false);
            #endregion Arrange

            #region Act
            var results = Controller.Index()
                .AssertViewRendered()
                .WithViewData<IList<DepartmentalAdminRequest>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.Count());
            Assert.AreEqual("4", results[0].Id);
            Assert.AreEqual("3", results[1].Id);
            Assert.AreEqual("1", results[2].Id);
            #endregion Assert
        }

        #endregion Index Tests

        #region Create Get Tests
        [TestMethod]
        [ExpectedException(typeof (PreconditionException))]
        public void TestCreateGetThrowsExceptionIfLdapLookupFails()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext.Setup(new[] {""}, "Me");
                Mock.Get(DirectorySearchService).Setup(a => a.FindUser("Me")).Returns<DirectoryUser>(null);
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
                Assert.AreEqual("Person requesting Departmental Access ID not found. ID = Me", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestCreateGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] {""}, "Me");
            var ldap = new DirectoryUser
                           {
                               LoginId = "Me",
                               FirstName = "FirstName",
                               LastName = "LastNasme",
                               EmailAddress = "me@testy.com",
                               PhoneNumber = "999 999-9999"
                           };
            Mock.Get(DirectorySearchService).Setup(a => a.FindUser("Me")).Returns(ldap);
            #endregion Arrange

            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminRequestViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.DepartmentalAdminRequest);
            Assert.IsNull(result.ExistingOrganizations);
            Assert.AreEqual("me", result.DepartmentalAdminRequest.Id);
            Assert.AreEqual("FirstName", result.DepartmentalAdminRequest.FirstName);
            Assert.AreEqual("LastNasme", result.DepartmentalAdminRequest.LastName);
            Assert.AreEqual("me@testy.com", result.DepartmentalAdminRequest.Email);
            Assert.AreEqual("999 999-9999", result.DepartmentalAdminRequest.PhoneNumber);
            Assert.IsNull(result.DepartmentalAdminRequest.Organizations);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] {""}, "Me");
            var ldap = new DirectoryUser
                           {
                               LoginId = "Me",
                               FirstName = "FirstName",
                               LastName = "LastNasme",
                               EmailAddress = "me@testy.com",
                               PhoneNumber = "999 999-9999"
                           };
            Mock.Get(DirectorySearchService).Setup(a => a.FindUser("Me")).Returns(ldap);

            var daRequests = new List<DepartmentalAdminRequest>();
            daRequests.Add(CreateValidEntities.DepartmentalAdminRequest(1));
            daRequests[0].Id = "Me";
            daRequests[0].Organizations = "1,2";
            new FakeDepartmentalAdminRequests(0, DepartmentalAdminRequestRepository, daRequests, true);
            new FakeOrganizations(2, OrganizationRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminRequestViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.DepartmentalAdminRequest);
            Assert.IsNull(result.ExistingOrganizations);
            Assert.AreEqual("Me", result.DepartmentalAdminRequest.Id);
            Assert.AreEqual("FirstName", result.DepartmentalAdminRequest.FirstName);
            Assert.AreEqual("LastNasme", result.DepartmentalAdminRequest.LastName);
            Assert.AreEqual("me@testy.com", result.DepartmentalAdminRequest.Email);
            Assert.AreEqual("999 999-9999", result.DepartmentalAdminRequest.PhoneNumber);
            Assert.AreEqual("1,2", result.DepartmentalAdminRequest.Organizations);
            #endregion Assert
        }

        #endregion Create Get Tests

        #region Create Post Tests
        [TestMethod]
        [ExpectedException(typeof (PreconditionException))]
        public void TestRequestRequired()
        {
            var thisFar = false;
            try
            {
                #region Arrange

                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Create(null, new List<string> {"1"});
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Precondition failed.", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestCreatePostRedirectsIfNoOrgsSelected1()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            Controller.Create(new DepartmentalAdminRequestViewModel(), null)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Must select at least one organization", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsIfNoOrgsSelected2()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            Controller.Create(new DepartmentalAdminRequestViewModel(), new List<string>())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Must select at least one organization", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof (PreconditionException))]
        public void TestCreatePostRequiresLdapLookup()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext.Setup(new[] {""}, "Me");
                Mock.Get(DirectorySearchService).Setup(a => a.FindUser("Me")).Returns<DirectoryUser>(null);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Create(new DepartmentalAdminRequestViewModel(), new List<string> {"1", "2"});
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Person requesting Departmental Access ID not found. ID = Me", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestCreatePostRetursViewWhenInvalid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] {""}, "Me");
            var ldap = new DirectoryUser
                           {
                               LoginId = "Me",
                               FirstName = "FirstName",
                               LastName = "LastNasme",
                               EmailAddress = null,
                               //"me@testy.com",
                               PhoneNumber = "999 999-9999"
                           };
            Mock.Get(DirectorySearchService).Setup(a => a.FindUser("Me")).Returns(ldap);
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            var departmentalAdminRequestViewModel = new DepartmentalAdminRequestViewModel();
            departmentalAdminRequestViewModel.DepartmentalAdminRequest = CreateValidEntities.DepartmentalAdminRequest(1);
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.Id = "Me";
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.PhoneNumber = "333 321-1234";
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.DepartmentSize = 1;
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.SharedOrCluster = true;
            #endregion Arrange

            #region Act
            var results = Controller.Create(departmentalAdminRequestViewModel, new List<string> {"1", "3"})
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminRequestViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Controller.ModelState.AssertErrorsAre("The Email field is required.");
            Assert.AreEqual("There were Errors, please correct and try again.", Controller.ErrorMessage);
            Assert.AreEqual("me", results.DepartmentalAdminRequest.Id);
            Assert.AreEqual("FirstName", results.DepartmentalAdminRequest.FirstName);
            Assert.AreEqual("LastNasme", results.DepartmentalAdminRequest.LastName);
            Assert.AreEqual(null, results.DepartmentalAdminRequest.Email);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, results.DepartmentalAdminRequest.DateCreated.Date);
            Assert.AreEqual("1,3", results.DepartmentalAdminRequest.Organizations);
            Assert.AreEqual("333 321-1234", results.DepartmentalAdminRequest.PhoneNumber);
            Assert.AreEqual(true, results.DepartmentalAdminRequest.SharedOrCluster);
            Assert.AreEqual(1, results.DepartmentalAdminRequest.DepartmentSize);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRetursViewWhenInvalid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] {""}, "1");
            var ldap = new DirectoryUser
                           {
                               LoginId = "Me",
                               FirstName = "FirstName",
                               LastName = "LastNasme",
                               EmailAddress = null,
                               //"me@testy.com",
                               PhoneNumber = "999 999-9999"
                           };
            Mock.Get(DirectorySearchService).Setup(a => a.FindUser("1")).Returns(ldap);
            var dars = new List<DepartmentalAdminRequest>();
            dars.Add(CreateValidEntities.DepartmentalAdminRequest(1));
            dars[0].Organizations = "This Will Be Replaced";
            new FakeDepartmentalAdminRequests(0, DepartmentalAdminRequestRepository, dars, false);
            var departmentalAdminRequestViewModel = new DepartmentalAdminRequestViewModel();
            departmentalAdminRequestViewModel.DepartmentalAdminRequest = CreateValidEntities.DepartmentalAdminRequest(1);
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.Id = "Me";
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.PhoneNumber = "333 321-1234";
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.DepartmentSize = 1;
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.SharedOrCluster = true;
            #endregion Arrange

            #region Act
            var results = Controller.Create(departmentalAdminRequestViewModel, new List<string> {"1", "3"})
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminRequestViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Controller.ModelState.AssertErrorsAre("The Email field is required.");
            Assert.AreEqual("There were Errors, please correct and try again.", Controller.ErrorMessage);
            Assert.AreEqual("1", results.DepartmentalAdminRequest.Id);
            Assert.AreEqual("FirstName", results.DepartmentalAdminRequest.FirstName);
            Assert.AreEqual("LastNasme", results.DepartmentalAdminRequest.LastName);
            Assert.AreEqual(null, results.DepartmentalAdminRequest.Email);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, results.DepartmentalAdminRequest.DateCreated.Date);
            Assert.AreEqual("1,3", results.DepartmentalAdminRequest.Organizations);
            Assert.AreEqual("333 321-1234", results.DepartmentalAdminRequest.PhoneNumber);
            Assert.AreEqual(true, results.DepartmentalAdminRequest.SharedOrCluster);
            Assert.AreEqual(1, results.DepartmentalAdminRequest.DepartmentSize);
            Mock.Get(DepartmentalAdminRequestRepository).Verify(a => a.GetNullableById("1"));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostRedirectsWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] {""}, "Me");
            var ldap = new DirectoryUser
                           {
                               LoginId = "Me",
                               FirstName = "FirstName",
                               LastName = "LastNasme",
                               EmailAddress = "me@testy.com",
                               PhoneNumber = "999 999-9999"
                           };
            Mock.Get(DirectorySearchService).Setup(a => a.FindUser("Me")).Returns(ldap);
            new FakeDepartmentalAdminRequests(3, DepartmentalAdminRequestRepository);
            var departmentalAdminRequestViewModel = new DepartmentalAdminRequestViewModel();
            departmentalAdminRequestViewModel.DepartmentalAdminRequest = CreateValidEntities.DepartmentalAdminRequest(1);
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.Id = "Me";
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.PhoneNumber = "333 321-1234";
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.DepartmentSize = 1;
            departmentalAdminRequestViewModel.DepartmentalAdminRequest.SharedOrCluster = true;
            DepartmentalAdminRequest args = default;
            Mock.Get(DepartmentalAdminRequestRepository).Setup(a => a.EnsurePersistent(It.IsAny<DepartmentalAdminRequest>()))
                .Callback<DepartmentalAdminRequest>((x) => args = x);
            #endregion Arrange

            #region Act
            Controller.Create(departmentalAdminRequestViewModel, new List<string> {"1", "3"})
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Request created.", Controller.Message);
            Mock.Get(DepartmentalAdminRequestRepository).Verify(
                a => a.EnsurePersistent(It.IsAny<DepartmentalAdminRequest>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("me", args.Id);
            Assert.AreEqual("FirstName", args.FirstName);
            Assert.AreEqual("LastNasme", args.LastName);
            Assert.AreEqual("me@testy.com", args.Email);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, args.DateCreated.Date);
            Assert.AreEqual("1,3", args.Organizations);
            Assert.AreEqual("333 321-1234", args.PhoneNumber);
            Assert.AreEqual(true, args.SharedOrCluster);
            Assert.AreEqual(1, args.DepartmentSize);
            #endregion Assert
        }
        #endregion Create Post Tests
    }
}
