using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;

namespace Purchasing.Tests.ControllerTests.WizardControllerTests
{
    public partial class WizardControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping1()
        {
            "~/Wizard/Index/".ShouldMapTo<WizardController>(a => a.Index());
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/Wizard/".ShouldMapTo<WizardController>(a => a.Index());
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestCreateWorkgroupGetMapping()
        {
            "~/Wizard/CreateWorkgroup/".ShouldMapTo<WizardController>(a => a.CreateWorkgroup());
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreateWorkgroupPostMapping()
        {
            "~/Wizard/CreateWorkgroup/".ShouldMapTo<WizardController>(a => a.CreateWorkgroup(null));
        }
        #endregion Mapping Tests
    }
}
