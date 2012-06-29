using System;
using System.Linq;
using System.Web.Mvc;
using Castle.Windsor;
using Purchasing.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Helpers;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using Purchasing.Tests.Core;

namespace Purchasing.Tests.ControllerTests.DepartmentalAdminRequestControllerTests
{
    public partial class DepartmentalAdminRequestControllerTests
    {
        #region Mapping Tests
        [TestMethod]
        public void TestIndexMapping1()
        {
            "~/DepartmentalAdminRequest/Index/".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Index());
        }

        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/DepartmentalAdminRequest/".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Index());
        }

        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/DepartmentalAdminRequest/Create/".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Create());
        }
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/DepartmentalAdminRequest/Create/".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Create(null, null));
        }

        [TestMethod]
        public void TestApproveGetMapping()
        {
            "~/DepartmentalAdminRequest/Approve/test".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Approve("test"));
        }

        [TestMethod]
        public void TestApprovePostMapping()
        {
            "~/DepartmentalAdminRequest/Approve/".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Approve(null, null, null), true);
        }
        #endregion Mapping Tests
    }
}
