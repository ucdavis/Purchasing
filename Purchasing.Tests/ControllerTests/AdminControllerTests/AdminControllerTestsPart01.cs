using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Testing;

namespace Purchasing.Tests.ControllerTests.AdminControllerTests
{
    public partial class AdminControllerTests
    {

        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsViewWithExpectedResults()
        {
            #region Arrange
            var roles = new List<Role>();
            var counter = 1;
            for (int i = 0; i < 3; i++)
            {
                roles.Add(CreateValidEntities.Role(i+1));
                for (int j = 0; j < 3; j++)
                {
                    roles[i].Users.Add(CreateValidEntities.User(counter));
                    counter++;
                }
            }
            roles[0].Name = "Admin";
            roles[0].SetIdTo(Role.Codes.Admin);
            roles[2].Name = "DepartmentalAdmin";
            roles[2].SetIdTo(Role.Codes.DepartmentalAdmin);
            roles[1].SetIdTo(Role.Codes.Approver);

            new FakeRoles(0, RoleRepository, roles, true);
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<AdminListModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Admins.Count());
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(string.Format("FirstName{0}", i+1), result.Admins[i].FirstName, string.Format("Sequence {0}", i));
            }
            Assert.AreEqual(3, result.DepartmentalAdmins.Count());
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(string.Format("FirstName{0}", i + 7), result.DepartmentalAdmins[i].FirstName, string.Format("Sequence {0}", i));
            }
            #endregion Assert		
        }
        #endregion Index Tests

        #region ModifyDepartmental Get Tests

        [TestMethod]
        public void TestModifyDepartmentalWhenUserNotFound()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ModifyDepartmental("4")
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);
            Assert.IsNull(result.User.FirstName);
            Assert.IsTrue(result.User.IsActive);
            #endregion Assert		
        }

        [TestMethod]
        public void TestModifyDepartmentalWhenUserFound1()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ModifyDepartmental("3")
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);
            Assert.AreEqual("FirstName3",result.User.FirstName);
            Assert.IsTrue(result.User.IsActive);
            #endregion Assert
        }

        [TestMethod]
        public void TestModifyDepartmentalWhenUserFound2()
        {
            #region Arrange
            var users = new List<User>();
            users.Add(CreateValidEntities.User(1));
            users[0].IsActive = false;
            users[0].SetIdTo("TestNotActive");
            new FakeUsers(0, UserRepository, users, true);
            #endregion Arrange

            #region Act
            var result = Controller.ModifyDepartmental("TestNotActive")
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);
            Assert.AreEqual("FirstName1", result.User.FirstName);
            Assert.IsFalse(result.User.IsActive);
            #endregion Assert
        }
        #endregion ModifyDepartmental Get Tests

        #region ModifyDepartmental Post Texts

        [TestMethod]
        public void TestModifyDepartmentalReturnsViewIfNoOrgs1()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            var result = Controller.ModifyDepartmental(new DepartmentalAdminModel(), null)
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminModel>();
            #endregion Act

            #region Assert
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("You must select at least one department for a departmental Admin.");
            Assert.IsNotNull(result);
            #endregion Assert		
        }

        [TestMethod]
        public void TestModifyDepartmentalReturnsViewIfNoOrgs2()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            var result = Controller.ModifyDepartmental(new DepartmentalAdminModel(), new List<string>())
                .AssertViewRendered()
                .WithViewData<DepartmentalAdminModel>();
            #endregion Act

            #region Assert
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("You must select at least one department for a departmental Admin.");
            Assert.IsNotNull(result);
            #endregion Assert
        }


        [TestMethod]
        public void TestModifyDepartmentalSetsExpectedValues1()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
            new FakeUsers(3, UserRepository);
            new FakeOrganizations(6, OrganizationRepository);

            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].SetIdTo(Role.Codes.DepartmentalAdmin);
            new FakeRoles(0, RoleRepository, roles, true);

            var depUser = new DepartmentalAdminModel();
            depUser.User = CreateValidEntities.User(4);
            var orgs = new List<string> {"2", "4"};
            #endregion Arrange

            #region Act
            Controller.ModifyDepartmental(depUser, orgs)
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
            #endregion Act

            #region Assert
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var args = (User) UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("4", args.Id);
            Assert.AreEqual("FirstName4 LastName4", args.FullName);
            Assert.AreEqual("Email4@testy.com", args.Email);
            Assert.IsTrue(args.IsActive);
            Assert.AreEqual(1, args.Roles.Count());
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, args.Roles[0].Id);
            Assert.AreEqual(2, args.Organizations.Count());
            Assert.AreEqual("2", args.Organizations[0].Id);
            Assert.AreEqual("4", args.Organizations[1].Id);

            Assert.AreEqual("FirstName4 LastName4 (4) was added as a departmental admin to the specified organization(s)", Controller.Message);
            UserIdentity.AssertWasCalled(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "4"));
            #endregion Assert		
        }

        [TestMethod]
        public void TestModifyDepartmentalSetsExpectedValues2()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
            new FakeUsers(3, UserRepository);
            new FakeOrganizations(6, OrganizationRepository);

            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].SetIdTo(Role.Codes.DepartmentalAdmin);
            new FakeRoles(0, RoleRepository, roles, true);

            var depUser = new DepartmentalAdminModel();
            depUser.User = UserRepository.Queryable.Single(a => a.Id == "3");
            depUser.User.LastName = "Changed";
            depUser.User.Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "1"));

            var orgs = new List<string> { "2", "4" };
            #endregion Arrange

            #region Act
            Controller.ModifyDepartmental(depUser, orgs)
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
            #endregion Act

            #region Assert
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var args = (User)UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("3", args.Id);
            Assert.AreEqual("FirstName3 Changed", args.FullName);
            Assert.AreEqual("Email3@testy.com", args.Email);
            Assert.IsTrue(args.IsActive);
            Assert.AreEqual(1, args.Roles.Count());
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, args.Roles[0].Id);
            Assert.AreEqual(2, args.Organizations.Count());
            Assert.AreEqual("2", args.Organizations[0].Id);
            Assert.AreEqual("4", args.Organizations[1].Id);

            Assert.AreEqual("FirstName3 Changed (3) was added as a departmental admin to the specified organization(s)", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestModifyDepartmentalSetsExpectedValues3()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].SetIdTo(Role.Codes.DepartmentalAdmin);
            roles.Add(CreateValidEntities.Role(88));
            roles[1].SetIdTo(Role.Codes.Admin);
            roles.Add(CreateValidEntities.Role(77));
            roles[2].SetIdTo(Role.Codes.EmulationUser);
            new FakeRoles(0, RoleRepository, roles, true);

            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].Roles.Add(RoleRepository.Queryable.Single(a => a.Id == Role.Codes.DepartmentalAdmin));
            users[0].SetIdTo("3");
            new FakeUsers(0, UserRepository, users, true);
            new FakeOrganizations(6, OrganizationRepository);



            var depUser = new DepartmentalAdminModel();
            depUser.User = UserRepository.Queryable.Single(a => a.Id == "3");
            depUser.User.LastName = "Changed";
            depUser.User.Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "1"));

            var orgs = new List<string> { "2", "4" };
            #endregion Arrange

            #region Act
            Controller.ModifyDepartmental(depUser, orgs)
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
            #endregion Act

            #region Assert
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var args = (User)UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, args.Roles[0].Id);

            Assert.AreEqual("FirstName3 Changed (3) was added as a departmental admin to the specified organization(s)", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestModifyDepartmentalSetsExpectedValues4()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(99));
            roles[0].SetIdTo(Role.Codes.DepartmentalAdmin);
            roles.Add(CreateValidEntities.Role(88));
            roles[1].SetIdTo(Role.Codes.Admin);
            roles.Add(CreateValidEntities.Role(77));
            roles[2].SetIdTo(Role.Codes.EmulationUser);
            new FakeRoles(0, RoleRepository, roles, true);

            var users = new List<User>();
            users.Add(CreateValidEntities.User(3));
            users[0].Roles.Add(RoleRepository.Queryable.Single(a => a.Id == Role.Codes.EmulationUser));
            users[0].SetIdTo("3");
            new FakeUsers(0, UserRepository, users, true);
            new FakeOrganizations(6, OrganizationRepository);



            var depUser = new DepartmentalAdminModel();
            depUser.User = UserRepository.Queryable.Single(a => a.Id == "3");
            depUser.User.LastName = "Changed";
            depUser.User.Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "1"));

            var orgs = new List<string> { "2", "4" };
            #endregion Arrange

            #region Act
            Controller.ModifyDepartmental(depUser, orgs)
                .AssertActionRedirect()
                .ToAction<AdminController>(a => a.Index());
            #endregion Act

            #region Assert
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            var args = (User)UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(Role.Codes.EmulationUser, args.Roles[0].Id);
            Assert.AreEqual(Role.Codes.DepartmentalAdmin, args.Roles[1].Id);

            Assert.AreEqual("FirstName3 Changed (3) was added as a departmental admin to the specified organization(s)", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestModifyDepartmentalWhenOrgThanDoesNotExist()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeUsers(3, UserRepository);
                new FakeOrganizations(6, OrganizationRepository);

                var roles = new List<Role>();
                roles.Add(CreateValidEntities.Role(99));
                roles[0].SetIdTo(Role.Codes.DepartmentalAdmin);
                new FakeRoles(0, RoleRepository, roles, true);

                var depUser = new DepartmentalAdminModel();
                depUser.User = UserRepository.Queryable.Single(a => a.Id == "3");
                depUser.User.LastName = "Changed";
                depUser.User.Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "1"));

                var orgs = new List<string> { "2", "7" };
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.ModifyDepartmental(depUser, orgs);
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
        #endregion ModifyDepartmental Post Texts

    }
}
