using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Controllers;

namespace Purchasing.Tests.ControllerTests.CustomFieldControllerTests
{
    public partial class CustomFieldControllerTests
    {
        #region Mapping Tests
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/CustomField/Index/Test".ShouldMapTo<CustomFieldController>(a => a.Index("Test"));
        }

        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/CustomField/Create/Test".ShouldMapTo<CustomFieldController>(a => a.Create("Test"));
        }

        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/CustomField/Create/Test".ShouldMapTo<CustomFieldController>(a => a.Create("Test", null));
        }

        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/CustomField/Edit/5".ShouldMapTo<CustomFieldController>(a => a.Edit(5));
        }

        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/CustomField/Edit/5".ShouldMapTo<CustomFieldController>(a => a.Edit(5, null));
        }

        [TestMethod]
        public void TestDeleteGetMapping()
        {
            "~/CustomField/Delete/5".ShouldMapTo<CustomFieldController>(a => a.Delete(5));
        }

        [TestMethod]
        public void TestUpdateOrderGetMapping()
        {
            "~/CustomField/UpdateOrder/5".ShouldMapTo<CustomFieldController>(a => a.UpdateOrder("5", null));
        }
        #endregion Mapping Tests
    }
}
