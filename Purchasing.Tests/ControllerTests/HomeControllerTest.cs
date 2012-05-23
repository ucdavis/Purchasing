using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Web;
using Purchasing.Web.Controllers;

namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod, Ignore]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController(null, null,null, null);

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("Welcome to ASP.NET MVC!", result.ViewBag.Message);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController(null, null,null, null);

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
