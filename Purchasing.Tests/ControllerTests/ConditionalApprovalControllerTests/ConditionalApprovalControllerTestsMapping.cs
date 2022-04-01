using System;
using System.Linq;
using Castle.Windsor;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.Attributes;
using Purchasing.Tests.Core;


namespace Purchasing.Tests.ControllerTests.ConditionalApprovalControllerTests
{
    public partial class ConditionalApprovalControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1A
        /// </summary>
        [TestMethod]
        public void TestByOrgMapping()
        {
            "~/ConditionalApproval/ByOrg/test".ShouldMapTo<ConditionalApprovalController>(a => a.ByOrg("test"));
        }

                /// <summary>
        /// #1B
        /// </summary>
        [TestMethod]
        public void TestByWorkgroupMapping()
        {
            "~/ConditionalApproval/ByWorkgroup/3".ShouldMapTo<ConditionalApprovalController>(a => a.ByWorkgroup(3));
        }

        

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestDeleteGetMapping()
        {
            "~/ConditionalApproval/Delete/5".ShouldMapTo<ConditionalApprovalController>(a => a.Delete(5));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestDeletePostMapping()
        {
            "~/ConditionalApproval/Delete/".ShouldMapTo<ConditionalApprovalController>(a => a.Delete(null));
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/ConditionalApproval/Edit/5".ShouldMapTo<ConditionalApprovalController>(a => a.Edit(5));
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/ConditionalApproval/Edit/".ShouldMapTo<ConditionalApprovalController>(a => a.Edit(null));
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/ConditionalApproval/Create/5".ShouldMapTo<ConditionalApprovalController>(a => a.Create(null, null), true);
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/ConditionalApproval/Create/5".ShouldMapTo<ConditionalApprovalController>(a => a.Create(5, null, null), true);
        }
        #endregion Mapping Tests
    }
}
