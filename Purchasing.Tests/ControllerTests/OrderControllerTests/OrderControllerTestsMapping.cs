using System;
using System.Linq;
using Castle.Windsor;
using Purchasing.Web;
using Purchasing.Core.Domain;
using Purchasing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Controllers;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using Purchasing.WS;
using Purchasing.Tests.Core;

namespace Purchasing.Tests.ControllerTests.OrderControllerTests
{
    public partial class OrderControllerTests
    {

        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestExampleMapping()
        {
            "~/Order/Index/".ShouldMapTo<OrderController>(a => a.Index(null, null, null, false, false,null), true);
        }
        #endregion Mapping Tests
    }
}
