using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using Purchasing.Tests.Core;

namespace Purchasing.Tests.ControllerTests.CustomFieldControllerTests
{
    public partial class CustomFieldControllerTests
    {
        #region Mapping Tests
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/CustomField/Index/Test".ShouldMapTo<CustomFieldController>(a => a.Index("Test"));
        }

        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/CustomField/Create/Test".ShouldMapTo<CustomFieldController>(a => a.Create("Test"));
        }

        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/CustomField/Create/Test".ShouldMapTo<CustomFieldController>(a => a.Create("Test", null));
        }

        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/CustomField/Edit/5".ShouldMapTo<CustomFieldController>(a => a.Edit(5));
        }
        #endregion Mapping Tests
    }
}
