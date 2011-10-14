using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region Workgroup Actions Mapping Tests
        /// <summary>
        /// Actions #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping1()
        {
            "~/Workgroup/Index/".ShouldMapTo<WorkgroupController>(a => a.Index());
        }

        /// <summary>
        /// Actions #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/Workgroup/".ShouldMapTo<WorkgroupController>(a => a.Index());
        }

        /// <summary>
        /// Actions #2
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Workgroup/Create".ShouldMapTo<WorkgroupController>(a => a.Create());
        }

        /// <summary>
        /// Actions #3
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/Workgroup/Create".ShouldMapTo<WorkgroupController>(a => a.Create(null, null));
        }

        /// <summary>
        /// Actions #4
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/Workgroup/Details/5".ShouldMapTo<WorkgroupController>(a => a.Details(5));
        }

        /// <summary>
        /// Actions #5
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Workgroup/Edit/5".ShouldMapTo<WorkgroupController>(a => a.Edit(5));
        }

        /// <summary>
        /// Actions #6
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Workgroup/Edit/".ShouldMapTo<WorkgroupController>(a => a.Edit(null, null));
        }

        /// <summary>
        /// Actions #7
        /// </summary>
        [TestMethod]
        public void TestDeleteGetMapping()
        {
            "~/Workgroup/Delete/5".ShouldMapTo<WorkgroupController>(a => a.Delete(5));
        }

        /// <summary>
        /// Actions #8
        /// </summary>
        [TestMethod]
        public void TestDeletePostMapping()
        {
            "~/Workgroup/Delete/".ShouldMapTo<WorkgroupController>(a => a.Delete(null));
        }
        #endregion Workgroup Actions Mapping Tests

        #region Addresses Mapping Tests
        /// <summary>
        /// Address #1
        /// </summary>
        [TestMethod]
        public void TestAddressesMapping()
        {
            "~/Workgroup/Addresses/5".ShouldMapTo<WorkgroupController>(a => a.Addresses(5));
        }

        /// <summary>
        /// Address #2
        /// </summary>
        [TestMethod]
        public void TestAddAddressGetMapping()
        {
            "~/Workgroup/AddAddress/5".ShouldMapTo<WorkgroupController>(a => a.AddAddress(5));
        }

        /// <summary>
        /// Address #3
        /// </summary>
        [TestMethod]
        public void TestAddAddressPostMapping()
        {
            "~/Workgroup/AddAddress/5".ShouldMapTo<WorkgroupController>(a => a.AddAddress(5, null));
        }

        /// <summary>
        /// Address #4
        /// </summary>
        [TestMethod]
        public void TestDeleteAddressGetMapping()
        {
            "~/Workgroup/DeleteAddress/5".ShouldMapTo<WorkgroupController>(a => a.DeleteAddress(5, 6), true);
        }

        /// <summary>
        /// Address #5
        /// </summary>
        [TestMethod]
        public void TestDeleteAddressPostMapping()
        {
            "~/Workgroup/DeleteAddress/5".ShouldMapTo<WorkgroupController>(a => a.DeleteAddress(5, 6, new WorkgroupAddress()), true);
        }

        /// <summary>
        /// Address #6
        /// </summary>
        [TestMethod]
        public void TestAddressDetailsMapping()
        {
            "~/Workgroup/AddressDetails/5".ShouldMapTo<WorkgroupController>(a => a.AddressDetails(5, 6), true);
        }

        /// <summary>
        /// Address #7
        /// </summary>
        [TestMethod]
        public void TestEditAddressGetMapping()
        {
            "~/Workgroup/EditAddress/5".ShouldMapTo<WorkgroupController>(a => a.EditAddress(5, 6), true);
        }

        /// <summary>
        /// Address #8
        /// </summary>
        [TestMethod]
        public void TestEditAddressPostMapping()
        {
            "~/Workgroup/EditAddress/5".ShouldMapTo<WorkgroupController>(a => a.EditAddress(5, 6, new WorkgroupAddress()), true);
        }
        #endregion Addresses Mapping Tests

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
