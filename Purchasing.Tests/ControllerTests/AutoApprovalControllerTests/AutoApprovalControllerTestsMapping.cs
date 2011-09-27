using System;
using System.Linq;
using Purchasing.Web;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Controllers.Dev;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using Purchasing.Tests.Core;

namespace Purchasing.Tests.ControllerTests.AutoApprovalControllerTests
{
    public partial class AutoApprovalControllerTests 
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/AutoApproval/Index/?showAll=False".ShouldMapTo<AutoApprovalController>(a => a.Index(false), true);
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/AutoApproval/Details/5?showAll=False".ShouldMapTo<AutoApprovalController>(a => a.Details(5, false), true);
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/AutoApproval/Create/?showAll=False".ShouldMapTo<AutoApprovalController>(a => a.Create(false), true);
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/AutoApproval/Create/?showAll=False".ShouldMapTo<AutoApprovalController>(a => a.Create(new AutoApproval(), false), true);
        }
        #endregion Mapping Tests
    }
}
