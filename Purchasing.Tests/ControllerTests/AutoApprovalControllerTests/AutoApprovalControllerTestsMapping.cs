using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;

namespace Purchasing.Tests.ControllerTests.AutoApprovalControllerTests
{
    public partial class AutoApprovalControllerTests 
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/AutoApproval/Index/?showAll=False".ShouldMapTo<AutoApprovalController>(a => a.Index(false), true);
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/AutoApproval/Details/5?showAll=False".ShouldMapTo<AutoApprovalController>(a => a.Details(5, false), true);
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/AutoApproval/Create/?showAll=False".ShouldMapTo<AutoApprovalController>(a => a.Create(false), true);
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/AutoApproval/Create/?showAll=False".ShouldMapTo<AutoApprovalController>(a => a.Create(new AutoApproval(), false), true);
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/AutoApproval/Edit/5".ShouldMapTo<AutoApprovalController>(a => a.Edit(5, false), true);
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/AutoApproval/Edit/5".ShouldMapTo<AutoApprovalController>(a => a.Edit(5, new AutoApproval(), false), true);
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestDeleteGetMapping()
        {
            "~/AutoApproval/Delete/5".ShouldMapTo<AutoApprovalController>(a => a.Delete(5, false), true);
        }

        /// <summary>
        /// #8
        /// </summary>
        [TestMethod]
        public void TestDeletePostMapping()
        {
            "~/AutoApproval/Delete/5".ShouldMapTo<AutoApprovalController>(a => a.Delete(5, new AutoApproval(), true), true);
        }
        #endregion Mapping Tests
    }
}
