using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using Purchasing.Tests.Core;


namespace Purchasing.Tests.ControllerTests.HistoryControllerTests
{
    public partial class HistoryControllerTests
    {

        #region Mapping Tests
        [TestMethod]
        public void TestIndexMapping1()
        {
            "~/History/Index/".ShouldMapTo<HistoryController>(a => a.Index(null, null, null, null, null, false, false), true);
        }

        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/History/".ShouldMapTo<HistoryController>(a => a.Index(null, null, null, null, null, false, false), true);
        }
        #endregion Mapping Tests
    }
}
