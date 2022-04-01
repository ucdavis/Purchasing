using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;


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
            "~/Workgroup/Index/".ShouldMapTo<WorkgroupController>(a => a.Index(false), ignoreParameters:true);
        }

        /// <summary>
        /// Actions #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/Workgroup/".ShouldMapTo<WorkgroupController>(a => a.Index(false), ignoreParameters:true);
        }

        ///// <summary>
        ///// Actions #2
        ///// </summary>
        //[TestMethod]
        //public void TestCreateGetMapping()
        //{
        //    "~/Workgroup/Create".ShouldMapTo<WorkgroupController>(a => a.Create());
        //}

        ///// <summary>
        ///// Actions #3
        ///// </summary>
        //[TestMethod]
        //public void TestCreatePostMapping()
        //{
        //    "~/Workgroup/Create".ShouldMapTo<WorkgroupController>(a => a.Create(null, null));
        //}

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
            "~/Workgroup/Edit/".ShouldMapTo<WorkgroupController>(a => a.Edit(0,null, null), true);
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
            "~/Workgroup/Delete/".ShouldMapTo<WorkgroupController>(a => a.Delete(0, null), true);
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

        #region Workgroup Accounts Mapping Tests
        /// <summary>
        /// Accounts #1
        /// </summary>
        [TestMethod]
        public void TestAccountsMapping()
        {
            "~/Workgroup/Accounts/5".ShouldMapTo<WorkgroupController>(a => a.Accounts(5));
        }

        /// <summary>
        /// Accounts #2
        /// </summary>
        [TestMethod]
        public void TestAddAccountGetMapping()
        {
            "~/Workgroup/AddAccount/5".ShouldMapTo<WorkgroupController>(a => a.AddAccount(5));
        }

        /// <summary>
        /// Accounts #3
        /// </summary>
        [TestMethod]
        public void TestAddAccountPostMapping()
        {
            "~/Workgroup/AddAccount/5".ShouldMapTo<WorkgroupController>(a => a.AddAccount(5, null, null));
        }

        /// <summary>
        /// Accounts #4
        /// </summary>
        [TestMethod]
        public void TestAccountDetailsMapping()
        {
            "~/Workgroup/AccountDetails/5".ShouldMapTo<WorkgroupController>(a => a.AccountDetails(0, 5), true);
        }

        /// <summary>
        /// Accounts #5
        /// </summary>
        [TestMethod]
        public void TestEditAccountGetMapping()
        {
            "~/Workgroup/EditAccount/5".ShouldMapTo<WorkgroupController>(a => a.EditAccount(0, 5), true);
        }

        /// <summary>
        /// Accounts #6
        /// </summary>
        [TestMethod]
        public void TestEditAccountPostMapping()
        {
            "~/Workgroup/EditAccount/5".ShouldMapTo<WorkgroupController>(a => a.EditAccount(0, 5, null), true);
        }

        /// <summary>
        /// Accounts #7
        /// </summary>
        [TestMethod]
        public void TestAccountDeleteGetMapping()
        {
            "~/Workgroup/AccountDelete/0?AccountId=5".ShouldMapTo<WorkgroupController>(a => a.AccountDelete(0, 5), true);
        }

        /// <summary>
        /// Accounts #8
        /// </summary>
        [TestMethod]
        public void TestAccountDeletePostMapping()
        {
            "~/Workgroup/AccountDelete/5".ShouldMapTo<WorkgroupController>(a => a.AccountDelete(0, 5, null), true);
        }

        /// <summary>
        /// Accounts #9
        /// </summary>
        [TestMethod]
        public void TestUpdateMultipleAccountsGetMapping()
        {
            "~/Workgroup/UpdateMultipleAccounts/5".ShouldMapTo<WorkgroupController>(a => a.UpdateMultipleAccounts(5));
        }

        /// <summary>
        /// Accounts #10
        /// </summary>
        [TestMethod]
        public void TestUpdateMultipleAccountsPostMapping()
        {
            "~/Workgroup/UpdateMultipleAccounts/5".ShouldMapTo<WorkgroupController>(a => a.UpdateMultipleAccounts(5, new UpdateMultipleAccountsViewModel()), true);
        }

        /// <summary>
        /// (45)
        /// </summary>
        [TestMethod]
        public void TestUpdateAccountMapping()
        {
            "~/Workgroup/UpdateAccount/5".ShouldMapTo<WorkgroupController>(a => a.UpdateAccount(5, 5, "test", "test", "test"), true);
        }
        #endregion Workgroup Accounts Mapping Tests

        #region Workgroup Vendors Mapping Tests
        /// <summary>
        /// Vendors #1
        /// </summary>
        [TestMethod]
        public void TestVendorListMapping()
        {
            "~/Workgroup/VendorList/5".ShouldMapTo<WorkgroupController>(a => a.VendorList(5));
        }

        /// <summary>
        /// Vendors #2
        /// </summary>
        [TestMethod]
        public void TestCreateVendorGetMapping()
        {
            "~/Workgroup/CreateVendor/5".ShouldMapTo<WorkgroupController>(a => a.CreateVendor(5));
        }

        /// <summary>
        /// Vendors #3
        /// </summary>
        [TestMethod]
        public void TestCreateVendorPostMapping()
        {
            "~/Workgroup/CreateVendor/5".ShouldMapTo<WorkgroupController>(a => a.CreateVendor(5, null, false, false), true);
        }

        /// <summary>
        /// Vendors #4
        /// </summary>
        [TestMethod]
        public void TestEditWorkgroupVendorGetMapping()
        {
            "~/Workgroup/EditWorkgroupVendor/5".ShouldMapTo<WorkgroupController>(a => a.EditWorkgroupVendor(0, 5), true);
        }

        /// <summary>
        /// Vendors #5
        /// </summary>
        [TestMethod]
        public void TestEditWorkgroupVendorPostMapping()
        {
            "~/Workgroup/EditWorkgroupVendor/5".ShouldMapTo<WorkgroupController>(a => a.EditWorkgroupVendor(0, 5, null, false), true);
        }

        /// <summary>
        /// Vendors #6
        /// </summary>
        [TestMethod]
        public void TestDeleteWorkgroupVendorGetMapping()
        {
            "~/Workgroup/DeleteWorkgroupVendor/5".ShouldMapTo<WorkgroupController>(a => a.DeleteWorkgroupVendor(0, 5), true);
        }

        /// <summary>
        /// Vendors #7
        /// </summary>
        [TestMethod]
        public void TestDeleteWorkgroupVendorPostMapping()
        {
            "~/Workgroup/DeleteWorkgroupVendor/5".ShouldMapTo<WorkgroupController>(a => a.DeleteWorkgroupVendor(0, 5, null), true);
        }

        /// <summary>
        /// Vendors #8
        /// </summary>
        [TestMethod]
        public void TestGetVendorAddressesMapping()
        {
            "~/Workgroup/GetVendorAddresses/5".ShouldMapTo<WorkgroupController>(a => a.GetVendorAddresses("blah"), true);
        }

        /// <summary>
        /// Vendors #9 (39)
        /// </summary>
        [TestMethod]
        public void TestGetBulkVendorMapping()
        {
            "~/Workgroup/BulkVendor/5".ShouldMapTo<WorkgroupController>(a => a.BulkVendor(5));
        }

        /// <summary>
        /// Vendors #10 (40)
        /// </summary>
        [TestMethod]
        public void TestPostBulkVendorMapping()
        {
            "~/Workgroup/BulkVendor/5".ShouldMapTo<WorkgroupController>(a => a.BulkVendor(5, null), true);
        }

        [TestMethod]
        public void TestCheckDuplicateVendorMapping()
        {
            "~/Workgroup/CheckDuplicateVendor/5".ShouldMapTo<WorkgroupController>(a => a.CheckDuplicateVendor(5, null, null), true);
        }

        [TestMethod]
        public void TestGetRequestersMapping()
        {
            "~/Workgroup/GetRequesters/5".ShouldMapTo<WorkgroupController>(a => a.GetRequesters(5));
        }

        /// <summary>
        /// Vendors #1
        /// </summary>
        [TestMethod]
        public void TestExportableVendorListMapping()
        {
            "~/Workgroup/ExportableVendorList/5".ShouldMapTo<WorkgroupController>(a => a.ExportableVendorList(5));
        }
        #endregion Workgroup Vendors Mapping Tests

        [TestMethod]
        public void TestWhoHasAccessToWorkgroupMapping()
        {
            "~/Workgroup/WhoHasAccessToWorkgroup/5".ShouldMapTo<WorkgroupController>(a => a.WhoHasAccessToWorkgroup(5));
        }
    }
}
