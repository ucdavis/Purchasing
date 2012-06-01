using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

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

        [TestMethod]
        public void TestWriteMethodTests()
        {
            #region Arrange
            Assert.Inconclusive("Need to write these tests");
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert
        }
    }
}
