using System;
using System.Linq;
using Castle.Windsor;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Purchasing.Tests.ControllerTests.ConditionalApprovalControllerTests
{
    public partial class ConditionalApprovalControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping1()
        {
            "~/ConditionalApproval/Index/".ShouldMapTo<ConditionalApprovalController>(a => a.Index());
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/ConditionalApproval/".ShouldMapTo<ConditionalApprovalController>(a => a.Index());
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
        #endregion Mapping Tests
    }
}
