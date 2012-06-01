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

namespace Purchasing.Tests.ControllerTests.AdminControllerTests
{
    public partial class AdminControllerTests
    {
        #region Mapping Tests
        [TestMethod]
        public void TestIndexMapping1()
        {
            "~/Admin/Index/".ShouldMapTo<AdminController>(a => a.Index());
        }

        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/Admin/".ShouldMapTo<AdminController>(a => a.Index());
        }

        [TestMethod]
        public void TestModifyDepartmentalGetMapping()
        {
            "~/Admin/ModifyDepartmental/Test".ShouldMapTo<AdminController>(a => a.ModifyDepartmental("Test"));
        }
        #endregion Mapping Tests
    }
}
