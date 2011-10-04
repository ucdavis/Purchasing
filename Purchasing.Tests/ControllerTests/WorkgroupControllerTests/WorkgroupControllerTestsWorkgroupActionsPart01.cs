using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.MappingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region Index Tests

        /// <summary>
        /// Actions #1
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
            SetupDataForWorkgroupActions1();
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<IEnumerable<Workgroup>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("Name4", result.ElementAt(0).Name);
            Assert.AreEqual("Name1", result.ElementAt(0).Organizations.ElementAt(0).Name);
            Assert.AreEqual("Name4", result.ElementAt(0).Organizations.ElementAt(1).Name);

            Assert.AreEqual("Name5", result.ElementAt(1).Name);
            Assert.AreEqual("Name1", result.ElementAt(1).Organizations.ElementAt(0).Name);
            Assert.AreEqual("Name5", result.ElementAt(1).Organizations.ElementAt(1).Name);

            Assert.AreEqual("Name6", result.ElementAt(2).Name);
            Assert.AreEqual("Name1", result.ElementAt(2).Organizations.ElementAt(0).Name);
            Assert.AreEqual("Name6", result.ElementAt(2).Organizations.ElementAt(1).Name);
            #endregion Assert		
        }


        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("continue tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion Index Tests
    }
}
