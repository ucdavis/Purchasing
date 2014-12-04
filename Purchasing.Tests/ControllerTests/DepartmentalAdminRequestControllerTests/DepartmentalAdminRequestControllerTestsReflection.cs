using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Helpers;
using UCDArch.Web.Attributes;

namespace Purchasing.Tests.ControllerTests.DepartmentalAdminRequestControllerTests
{
    public partial class DepartmentalAdminRequestControllerTests
    {
        #region Reflection Tests

        #region Controller Class Tests
        [TestMethod]
        public void TestControllerInheritsFromApplicationController()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            Assert.IsNotNull(controllerClass.BaseType);
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("ApplicationController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has 5 attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHas5Attributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "UseTransactionsByDefaultAttribute not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has anti forgery token attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAntiForgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "UseAntiForgeryTokenOnPostByDefault not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasVersionAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<Mvc.Attributes.VersionAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "VersionAttribute not found.");
            #endregion Assert
        }


        [TestMethod]
        public void TestControllerHasAuthorizeAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "AuthorizeAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasProfileAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<ProfileAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Any(), "ProfileAttribute not found.");
            #endregion Assert
        }

        #endregion Controller Class Tests

        #region Controller Method Tests

        [TestMethod]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert           
            Assert.AreEqual(11, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }


        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual(Role.Codes.Admin, expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act          
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            var element = controllerMethod.ElementAt(1);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestControllerMethodApproveContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Approve");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual(Role.Codes.Admin, expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodApproveContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Approve");
            var element = controllerMethod.ElementAt(1);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual(Role.Codes.Admin, expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodApproveContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Approve");
            var element = controllerMethod.ElementAt(1);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodDenyContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Deny");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual(Role.Codes.Admin, expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodDenyContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Deny");
            var element = controllerMethod.ElementAt(1);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual(Role.Codes.Admin, expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodDenyContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Deny");
            var element = controllerMethod.ElementAt(1);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodSearchOrgsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "SearchOrgs");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodDetailsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Details");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual(Role.Codes.Admin, expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodTookTrainingContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "TookTraining");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual(Role.Codes.Admin, expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodInstructionsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Instructions");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            //var expectedAttribute = element.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            //Assert.AreEqual(Role.Codes.Admin, expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
