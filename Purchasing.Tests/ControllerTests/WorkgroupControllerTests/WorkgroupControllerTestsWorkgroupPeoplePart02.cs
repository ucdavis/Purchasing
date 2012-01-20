using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Testing;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region AddPeople Get Tests

        [TestMethod]
        public void TestAddPeopleGetRedirectsIfWorkgroupNotFound1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.AddPeople(4, null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddPeopleGetRedirectsIfWorkgroupNotFound2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.AddPeople(4, Role.Codes.DepartmentalAdmin)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeopleGetRedirectsIfNoAccess1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.AddPeople(3, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeopleGetRedirectsIfNoAccess2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.AddPeople(3, Role.Codes.DepartmentalAdmin)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }


        [TestMethod]
        public void TestAddPeopleGetReturnsView1()
        {
            #region Arrange
            SetupRoles(new List<Role>());
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, null)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Role);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            Assert.IsNull(result.Users);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeopleGetReturnsView2()
        {
            #region Arrange
            SetupRoles(new List<Role>());
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, Role.Codes.DepartmentalAdmin)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Role);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            Assert.IsNull(result.Users);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeopleGetReturnsView3()
        {
            #region Arrange
            SetupRoles(new List<Role>());
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, Role.Codes.Admin)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Role);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            Assert.IsNull(result.Users);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeopleGetReturnsView4()
        {
            #region Arrange
            SetupRoles(new List<Role>());
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, Role.Codes.Requester)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual(Role.Codes.Requester, result.Role.Id);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            Assert.IsNull(result.Users);
            #endregion Assert
        } 
        #endregion AddPeople Get Tests

        #region AddPeople Post Tests
        [TestMethod]
        public void TestAddPeoplePostRedirectsIfWorkgroupNotFound1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.AddPeople(4, new WorkgroupPeoplePostModel(), null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeoplePostRedirectsIfWorkgroupNotFound2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.AddPeople(4, new WorkgroupPeoplePostModel(), Role.Codes.DepartmentalAdmin)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup not found", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeoplePostRedirectsIfNoAccess1()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.AddPeople(3, new WorkgroupPeoplePostModel(), null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddPeoplePostRedirectsIfNoAccess2()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.AddPeople(3, new WorkgroupPeoplePostModel(), Role.Codes.DepartmentalAdmin)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Fake Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }


        [TestMethod]
        public void TestAddPeoplePostReturnsViewWhenInvalid1()
        {
            #region Arrange
            SetupDataForPeopleList();
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            var postModel = new WorkgroupPeoplePostModel();
            postModel.Role = new Role();
            postModel.Role.SetIdTo(Role.Codes.DepartmentalAdmin);
            postModel.Role.Level = 1;            
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, postModel, null)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();

            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Invalid Role Selected");
            Assert.IsNotNull(result);
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, result.Role.Id);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddPeoplePostReturnsViewWhenInvalid2()
        {
            #region Arrange
            SetupDataForPeopleList();
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            var postModel = new WorkgroupPeoplePostModel();
            postModel.Role = new Role();
            postModel.Role.SetIdTo(Role.Codes.DepartmentalAdmin);
            postModel.Role.Level = 1;
            postModel.Users = new List<string>();
            postModel.Users.Add("1");
            postModel.Users.Add("2");
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, postModel, null)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();

            #endregion Act

            #region Assert
            SearchService.AssertWasNotCalled(a => a.FindUser(Arg<string>.Is.Anything));
            
            Controller.ModelState.AssertErrorsAre("Invalid Role Selected");
            Assert.IsNotNull(result);
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, result.Role.Id);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);
            Assert.AreEqual(2, result.Users.Count());
            Assert.AreEqual("FirstName1 LastName1 (1)", result.Users[0].DisplayNameAndId);
            Assert.AreEqual("FirstName2 LastName2 (2)", result.Users[1].DisplayNameAndId);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }


        [TestMethod]
        public void TestAddPeoplePostReturnsViewWhenInvalid3()
        {
            #region Arrange
            SetupDataForPeopleList();
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            var ldapUser = new DirectoryUser();
            ldapUser.FirstName = "Me";
            ldapUser.LastName = "You";
            ldapUser.LoginId = "Logger";
            SearchService.Expect(a => a.FindUser("Me")).Return(ldapUser);
            var postModel = new WorkgroupPeoplePostModel();
            postModel.Role = new Role();
            postModel.Role.SetIdTo(Role.Codes.DepartmentalAdmin);
            postModel.Role.Level = 1;
            postModel.Users = new List<string>();
            postModel.Users.Add("1");
            postModel.Users.Add("Me");
            postModel.Users.Add("2");
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, postModel, null)
                .AssertViewRendered()
                .WithViewData<WorgroupPeopleCreateModel>();

            #endregion Act

            #region Assert
            SearchService.AssertWasCalled(a => a.FindUser("Me"));

            Controller.ModelState.AssertErrorsAre("Invalid Role Selected");
            Assert.IsNotNull(result);
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, result.Role.Id);
            Assert.AreEqual(4, result.Roles.Count());
            Assert.AreEqual(Role.Codes.Requester, result.Roles[0].Id);
            Assert.AreEqual(Role.Codes.Approver, result.Roles[1].Id);
            Assert.AreEqual(Role.Codes.AccountManager, result.Roles[2].Id);
            Assert.AreEqual(Role.Codes.Purchaser, result.Roles[3].Id);
            Assert.AreEqual(3, result.Users.Count());
            Assert.AreEqual("FirstName1 LastName1 (1)", result.Users[0].DisplayNameAndId);
            Assert.AreEqual("Me You (Logger)", result.Users[1].DisplayNameAndId);
            Assert.AreEqual("FirstName2 LastName2 (2)", result.Users[2].DisplayNameAndId);

            Assert.AreEqual("Name3", result.Workgroup.Name);

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name3", ((Workgroup)args[0]).Name);
            Assert.IsNull(args[1]);
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void TestWorkgroupPeoplePostModelValidation1()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var test = new WorkgroupPeoplePostModel();
                test.Users = null;
                test.Role = new Role();
                var context = new ValidationContext(test, null, null);
                thisFar = true;
                #endregion Arrange

                #region Act
                Validator.ValidateObject(test, context, true);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Must add at least one user", ex.Message);
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void TestWorkgroupPeoplePostModelValidation2()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var test = new WorkgroupPeoplePostModel();
                test.Users = new List<string>();
                test.Role = null;
                var context = new ValidationContext(test, null, null);
                thisFar = true;
                #endregion Arrange

                #region Act
                Validator.ValidateObject(test, context, true);
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("The Role field is required.", ex.Message);
                throw;
            }
        }
        
        [TestMethod]
        public void TestAddPeoplePostRedirectsToPeople1()
        {
            #region Arrange
            SetupDataForPeopleList();
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            var ldapUser = new DirectoryUser();
            ldapUser.FirstName = "Me";
            ldapUser.LastName = "You";
            ldapUser.LoginId = "Logger";
            ldapUser.EmailAddress = "tester@testy.com";
            SearchService.Expect(a => a.FindUser("Me")).Return(ldapUser);
            var postModel = new WorkgroupPeoplePostModel();
            postModel.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            postModel.Users = new List<string>();
            postModel.Users.Add("1");
            postModel.Users.Add("Me");
            postModel.Users.Add("2");
            postModel.Users.Add("3");
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, postModel, null)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual(Role.Codes.AccountManager, result.RouteValues["roleFilter"]);

            //SearchService.AssertWasCalled(a => a.FindUser("Me"));
            
            //UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            //var userArgs = (User) UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0]; 
            //Assert.IsNotNull(userArgs);
            //Assert.AreEqual("Me", userArgs.FirstName);
            //Assert.AreEqual("You", userArgs.LastName);
            //Assert.AreEqual("tester@testy.com", userArgs.Email);
            //Assert.AreEqual("Logger", userArgs.Id);

            //TODO: Assert WorkgroupService

            WorkgroupPermissionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything), x => x.Repeat.Times(3));
            var workgroupPermissionArgs = WorkgroupPermissionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything));
            Assert.IsNotNull(workgroupPermissionArgs);
            Assert.AreEqual(3, workgroupPermissionArgs.Count());
            Assert.AreEqual("1", ((WorkgroupPermission)workgroupPermissionArgs[0][0]).User.Id);
            Assert.AreEqual(Role.Codes.AccountManager, ((WorkgroupPermission)workgroupPermissionArgs[0][0]).Role.Id);
            Assert.AreEqual(3, ((WorkgroupPermission)workgroupPermissionArgs[0][0]).Workgroup.Id);

            Assert.AreEqual(null, ((WorkgroupPermission)workgroupPermissionArgs[1][0]).User); //This is null because our mock doesn't find the user that was added from the LDAP lookup
            Assert.AreEqual("2", ((WorkgroupPermission)workgroupPermissionArgs[2][0]).User.Id);

            Assert.AreEqual("Successfully added 3 people to workgroup as Account Manager. 1 not added because of duplicated role.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddPeoplePostRedirectsToPeople2()
        {
            #region Arrange
            SetupDataForPeopleList();
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
            var ldapUser = new DirectoryUser();
            ldapUser.FirstName = "Me";
            ldapUser.LastName = "You";
            ldapUser.LoginId = "Logger";
            ldapUser.EmailAddress = "tester@testy.com";
            SearchService.Expect(a => a.FindUser("Me")).Return(ldapUser);
            var postModel = new WorkgroupPeoplePostModel();
            postModel.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            postModel.Users = new List<string>();
            postModel.Users.Add("1");
            postModel.Users.Add("Me");
            postModel.Users.Add("2");
            postModel.Users.Add("3");
            #endregion Arrange

            #region Act
            var result = Controller.AddPeople(3, postModel, Role.Codes.Requester)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.People(3, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual(Role.Codes.AccountManager, result.RouteValues["roleFilter"]);

            SearchService.AssertWasCalled(a => a.FindUser("Me"));

            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var userArgs = (User)UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0];
            Assert.IsNotNull(userArgs);
            Assert.AreEqual("Me", userArgs.FirstName);
            Assert.AreEqual("You", userArgs.LastName);
            Assert.AreEqual("tester@testy.com", userArgs.Email);
            Assert.AreEqual("Logger", userArgs.Id);

            WorkgroupPermissionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything), x => x.Repeat.Times(3));
            var workgroupPermissionArgs = WorkgroupPermissionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything));
            Assert.IsNotNull(workgroupPermissionArgs);
            Assert.AreEqual(3, workgroupPermissionArgs.Count());
            Assert.AreEqual("1", ((WorkgroupPermission)workgroupPermissionArgs[0][0]).User.Id);
            Assert.AreEqual(Role.Codes.AccountManager, ((WorkgroupPermission)workgroupPermissionArgs[0][0]).Role.Id);
            Assert.AreEqual(3, ((WorkgroupPermission)workgroupPermissionArgs[0][0]).Workgroup.Id);

            Assert.AreEqual(null, ((WorkgroupPermission)workgroupPermissionArgs[1][0]).User); //This is null because our mock doesn't find the user that was added from the LDAP lookup
            Assert.AreEqual("2", ((WorkgroupPermission)workgroupPermissionArgs[2][0]).User.Id);

            Assert.AreEqual("Successfully added 3 people to workgroup as Account Manager. 1 not added because of duplicated role.", Controller.Message);
            #endregion Assert
        }
        #endregion AddPeople Post Tests
    }
}
