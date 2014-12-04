using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;

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

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestAddSubOrganizationsGetMapping()
        {
            "~/Wizard/AddSubOrganizations/5".ShouldMapTo<WizardController>(a => a.AddSubOrganizations(5));
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestAddSubOrganizationsPostMapping()
        {
            "~/Wizard/AddSubOrganizations/5".ShouldMapTo<WizardController>(a => a.AddSubOrganizations(5, new string[0]));
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestSubOrganizationsGetMapping()
        {
            "~/Wizard/SubOrganizations/5".ShouldMapTo<WizardController>(a => a.SubOrganizations(5));
        }
        #endregion Mapping Tests
    }
}
