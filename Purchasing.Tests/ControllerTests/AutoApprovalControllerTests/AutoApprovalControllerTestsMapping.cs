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
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/AutoApproval/Index/?showAll=False".ShouldMapTo<AutoApprovalController>(a => a.Index(false), true);
        }
        #endregion Mapping Tests
    }
}
