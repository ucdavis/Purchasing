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

        [TestMethod]
        public void TestModifyDepartmentalPostMapping()
        {
            "~/Admin/ModifyDepartmental/Test".ShouldMapTo<AdminController>(a => a.ModifyDepartmental(null, null), true);
        }

        [TestMethod]
        public void TestModifyAdminGetMapping()
        {
            "~/Admin/ModifyAdmin/test".ShouldMapTo<AdminController>(a => a.ModifyAdmin("test"));
        }
        
        [TestMethod]
        public void TestModifyAdminPostMapping()
        {
            "~/Admin/ModifyAdmin/test".ShouldMapTo<AdminController>(a => a.ModifyAdmin(new User()), true);
        }

        [TestMethod]
        public void TestRemoveAdminGetMapping()
        {
            "~/Admin/RemoveAdmin/test".ShouldMapTo<AdminController>(a => a.RemoveAdmin("test"));
        }
        [TestMethod]
        public void TestRemoveAdminRolePostMapping()
        {
            "~/Admin/RemoveAdminRole/test".ShouldMapTo<AdminController>(a => a.RemoveAdminRole("test"));
        }
        [TestMethod]
        public void TestRemoveDepartmentalGetMapping()
        {
            "~/Admin/RemoveDepartmental/test".ShouldMapTo<AdminController>(a => a.RemoveDepartmental("test"));
        }
        

        [TestMethod]
        public void TestRemoveDepartmentalRolePostMapping()
        {
            "~/Admin/RemoveDepartmentalRole/test".ShouldMapTo<AdminController>(a => a.RemoveDepartmentalRole("test"));
        }

        [TestMethod]
        public void TestCloneGetMapping()
        {
            "~/Admin/Clone/test".ShouldMapTo<AdminController>(a => a.Clone("test"));
        }

        [TestMethod]
        public void TestFindUserGetMapping()
        {
            "~/Admin/FindUser/test".ShouldMapTo<AdminController>(a => a.FindUser("test"), true);
        }

        [TestMethod]
        public void TestSearchOrgsMapping()
        {
            "~/Admin/SearchOrgs/test".ShouldMapTo<AdminController>(a => a.SearchOrgs("test"), true);
        }
        #endregion Mapping Tests
    }
}
