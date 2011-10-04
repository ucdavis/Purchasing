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
using Purchasing.Tests.Core;
//using Purchasing.Controllers.Filters;
//using Purchasing.Services;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {

        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Workgroup/Index/".ShouldMapTo<WorkgroupController>(a => a.Index());
        }


        #region People Mapping Tests
        /// <summary>
        /// People #1
        /// </summary>
        [TestMethod]
        public void TestPeopleMapping()
        {
            "~/Workgroup/People/5".ShouldMapTo<WorkgroupController>(a => a.People(5, null));
        }

        /// <summary>
        /// People #2
        /// </summary>
        [TestMethod]
        public void TestAddPeopleGetMapping()
        {
            "~/Workgroup/AddPeople/5".ShouldMapTo<WorkgroupController>(a => a.AddPeople(5, null));
        }

        /// <summary>
        /// People #3
        /// </summary>
        [TestMethod]
        public void TestAddPeoplePostMapping()
        {
            "~/Workgroup/AddPeople/5".ShouldMapTo<WorkgroupController>(a => a.AddPeople(5, null, null));
        }

        /// <summary>
        /// People #4
        /// </summary>
        [TestMethod]
        public void TestDeletePeopleGetMapping()
        {
            "~/Workgroup/DeletePeople/5".ShouldMapTo<WorkgroupController>(a => a.DeletePeople(5, 3, null), true);
        }
        #endregion People Mapping Tests
    }
}
