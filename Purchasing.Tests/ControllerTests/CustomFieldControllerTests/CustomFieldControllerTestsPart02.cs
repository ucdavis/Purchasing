using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Models;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using Purchasing.Tests.Core;

namespace Purchasing.Tests.ControllerTests.CustomFieldControllerTests
{
    public partial class CustomFieldControllerTests
    {
        #region Edit Get Tests

        [TestMethod]
        public void TestEditRedirectsToIndexIfCustomFieldNotFound()
        {
            #region Arrange
            new FakeCustomFields(3, CustomFieldRepository);
            #endregion Arrange

            #region Act
            //Controller.Edit(4)
            //    .AssertActionRedirect()
            //    .ToAction<CustomFieldController>(a => a.Index());
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        #endregion Edit Get Tests


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
