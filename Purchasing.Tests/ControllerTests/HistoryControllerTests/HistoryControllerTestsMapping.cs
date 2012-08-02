﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;


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

        [TestMethod]
        public void TestAdminOrdersMapping()
        {
            "~/History/AdminOrders/".ShouldMapTo<HistoryController>(a => a.AdminOrders(null, null, null, null, null, false), true);
        }

        [TestMethod]
        public void TestRecentActivityMapping()
        {
            "~/History/RecentActivity/".ShouldMapTo<HistoryController>(a => a.RecentActivity());
        }

        [TestMethod]
        public void TestRecentCommentsMapping()
        {
            "~/History/RecentComments/".ShouldMapTo<HistoryController>(a => a.RecentComments());
        }

        [TestMethod]
        public void TestRecentlyCompletedMapping()
        {
            "~/History/RecentlyCompleted/".ShouldMapTo<HistoryController>(a => a.RecentlyCompleted());
        }
        #endregion Mapping Tests
    }
}
