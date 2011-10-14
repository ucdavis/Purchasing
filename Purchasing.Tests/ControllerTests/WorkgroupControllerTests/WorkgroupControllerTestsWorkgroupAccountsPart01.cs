using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.MappingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Rhino.Mocks;
using UCDArch.Testing;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {

        [TestMethod]
        public void TestAccountsRedirectsIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.Accounts(4)
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestAccountsReturnsView()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Accounts(3)
                .AssertViewRendered()
                .WithViewData<Workgroup>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Name);
            #endregion Assert		
        }
    }
}
