using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
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

        [TestMethod]
        public void TestUpdateChildWorkgroupsMapping()
        {
            "~/Admin/UpdateChildWorkgroups/".ShouldMapTo<AdminController>(a => a.UpdateChildWorkgroups());
        }
        
        [TestMethod]
        public void TestProcessWorkGroupMapping()
        {
            "~/Admin/ProcessWorkGroup/5".ShouldMapTo<AdminController>(a => a.ProcessWorkGroup(5));
        }

        [TestMethod]
        public void TestGetChildWorkgroupIdsMapping()
        {
            "~/Admin/GetChildWorkgroupIds/5".ShouldMapTo<AdminController>(a => a.GetChildWorkgroupIds(5));
        }

        [TestMethod]
        public void TestValidateChildWorkgroupsMapping()
        {
            "~/Admin/ValidateChildWorkgroups/5".ShouldMapTo<AdminController>(a => a.ValidateChildWorkgroups());
        }

        [TestMethod]
        public void TestRemoveExtraChildPermissionsMapping()
        {
            "~/Admin/RemoveExtraChildPermissions/".ShouldMapTo<AdminController>(a => a.RemoveExtraChildPermissions());
        }

        [TestMethod]
        public void TestTestExceptionMapping()
        {
            "~/Admin/TestException".ShouldMapTo<AdminController>(a => a.TestException());
        }

        [TestMethod]
        public void TestTestEmailMapping()
        {
            "~/Admin/TestEmail".ShouldMapTo<AdminController>(a => a.TestEmail());
        }

        [TestMethod]
        public void TestModifySscAdminGetMapping()
        {
            "~/Admin/ModifySscAdmin/test".ShouldMapTo<AdminController>(a => a.ModifySscAdmin("test"));
        }

        [TestMethod]
        public void TestModifySscAdminPostMapping()
        {
            "~/Admin/ModifySscAdmin/test".ShouldMapTo<AdminController>(a => a.ModifySscAdmin(new User()), true);
        }

        [TestMethod]
        public void TestRemoveSscAdminGetMapping()
        {
            "~/Admin/RemoveSscAdmin/test".ShouldMapTo<AdminController>(a => a.RemoveSscAdmin("test"));
        }
        [TestMethod]
        public void TestRemoveSscAdminRolePostMapping()
        {
            "~/Admin/RemoveSscAdmin/test".ShouldMapTo<AdminController>(a => a.RemoveSscAdmin("test"));
        }
        #endregion Mapping Tests
    }
}
