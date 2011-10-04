using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;


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

        /// <summary>
        /// People #5
        /// </summary>
        [TestMethod]
        public void TestDeletePeoplePostMapping()
        {
            "~/Workgroup/DeletePeople/5".ShouldMapTo<WorkgroupController>(a => a.DeletePeople(5, 3, null, null, null), true);
        }

        /// <summary>
        /// People #6
        /// </summary>
        [TestMethod]
        public void TestSearchUsersMapping()
        {
            "~/Workgroup/SearchUsers/".ShouldMapTo<WorkgroupController>(a => a.SearchUsers(null));
        }
        #endregion People Mapping Tests
    }
}
