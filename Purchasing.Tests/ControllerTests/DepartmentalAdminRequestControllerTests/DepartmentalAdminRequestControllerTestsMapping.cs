using System;
using System.Linq;
using System.Web.Mvc;
using Castle.Windsor;
using Purchasing.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Services;
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
            "~/DepartmentalAdminRequest/Index/".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Index(null));
        }

        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/DepartmentalAdminRequest/".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Index(null));
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

        [TestMethod]
        public void TestDenyGetMapping()
        {
            "~/DepartmentalAdminRequest/Deny/test".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Deny("test"));
        }

        [TestMethod]
        public void TestDenyPostMapping()
        {
            "~/DepartmentalAdminRequest/Deny/".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Deny(new DepartmentalAdminRequestViewModel()), true);
        }

        [TestMethod]
        public void TestSearchOrgsGetMapping()
        {
            "~/DepartmentalAdminRequest/SearchOrgs/test".ShouldMapTo<DepartmentalAdminRequestController>(a => a.SearchOrgs("test"), true);
        }

        [TestMethod]
        public void TestDetailsGetMapping()
        {
            "~/DepartmentalAdminRequest/Details/test".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Details("test"), true);
        }

        [TestMethod]
        public void TestTookTrainingGetMapping()
        {
            "~/DepartmentalAdminRequest/TookTraining/test".ShouldMapTo<DepartmentalAdminRequestController>(a => a.TookTraining("test", "Default"), true);
        }

        [TestMethod]
        public void TestInstructionsMapping()
        {
            "~/DepartmentalAdminRequest/Instructions".ShouldMapTo<DepartmentalAdminRequestController>(a => a.Instructions());
        }
        #endregion Mapping Tests
    }
}
