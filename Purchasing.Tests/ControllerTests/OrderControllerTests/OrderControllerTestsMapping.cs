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
        public void TestIndexMapping1()
        {
            "~/Order/".ShouldMapTo<OrderController>(a => a.Index());
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/Order/Index/".ShouldMapTo<OrderController>(a => a.Index());
        }

        [TestMethod]
        public void TestSelectWorkgroupMapping()
        {
            "~/Order/SelectWorkgroup/".ShouldMapTo<OrderController>(a => a.SelectWorkgroup());
        }

        [TestMethod]
        public void TestReroutePurchaserGetMapping()
        {
            "~/Order/ReroutePurchaser/5".ShouldMapTo<OrderController>(a => a.ReroutePurchaser(5));
        }

        [TestMethod]
        public void TestReroutePurchaserPostMapping()
        {
            "~/Order/ReroutePurchaser/5".ShouldMapTo<OrderController>(a => a.ReroutePurchaser(5, null));
        }

        [TestMethod]
        public void TestRequestGetMapping()
        {
            "~/Order/Request/5".ShouldMapTo<OrderController>(a => a.Request(5));
        }

        [TestMethod]
        public void TestRequestPostMapping()
        {
            "~/Order/Request/5".ShouldMapTo<OrderController>(a => a.Request(null));
        }

        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Order/Edit/5".ShouldMapTo<OrderController>(a => a.Edit(5));
        }
        #endregion Mapping Tests
    }
}
