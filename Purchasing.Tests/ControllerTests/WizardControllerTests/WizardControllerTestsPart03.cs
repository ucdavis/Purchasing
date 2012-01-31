using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using FluentNHibernate.Data;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Helpers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;

namespace Purchasing.Tests.ControllerTests.WizardControllerTests
{
    public partial class WizardControllerTests
    {
        #region AddPeople Get Tests

        [TestMethod]
        public void TestAddPeopleRedirectsToCreateWorkgroupWhenWorkgroupIdIsZero()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Controller.AddPeople(0, null)
                .AssertActionRedirect()
                .ToAction<WizardController>(a => a.CreateWorkgroup());
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddPeopleGetRedirectsIfNoAccess()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            string message = "Fake Message";
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.AddPeople(3, Role.Codes.Requester)
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
        [ExpectedException(typeof (PreconditionException))]
        public void TestAddPeopleChecksDatabaseRole()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var roles = new List<Role>();
                for (int i = 0; i < 4; i++)
                {
                    roles.Add(CreateValidEntities.Role(i+1));
                    roles[i].Level = i + 1;
                    roles[i].SetIdTo((i + 1).ToString());
                }
                new FakeWorkgroups(3, WorkgroupRepository);
                string message = string.Empty;
                SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything, Arg<Organization>.Is.Anything, out Arg<string>.Out(message).Dummy)).Return(true);
                new FakeRoles(0, RoleRepository, roles, true);

                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.AddPeople(3, "2");
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

        #endregion AddPeople Get Tests
    }
}
