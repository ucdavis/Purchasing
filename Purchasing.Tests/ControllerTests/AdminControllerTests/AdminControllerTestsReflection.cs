using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Helpers;
using UCDArch.Web.Attributes;
using Purchasing.Tests.Extensions;

namespace Purchasing.Tests.ControllerTests.AdminControllerTests
{
    public partial class AdminControllerTests
    {
        #region Reflection Tests

        #region Controller Class Tests
        /// <summary>
        /// Tests the controller inherits from application controller.
        /// </summary>
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


        [TestMethod]
        public void TestControllerHasSevenAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(7, result.Count());
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
            var result = controllerClass.GetFilteredCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseTransactionsByDefaultAttribute not found.");
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
            var result = controllerClass.GetFilteredCustomAttributes(true).OfType<AutoValidateAntiforgeryTokenAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "AutoValidateAntiforgeryTokenAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasVersionAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetFilteredCustomAttributes(true).OfType<Mvc.Attributes.VersionAttribute>();
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
            var result = controllerClass.GetFilteredCustomAttributes(true).OfType<AuthorizeAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 1, "AuthorizeAttribute not found.");
            bool admin = result.ElementAt(0).Policy == "AD" || result.ElementAt(1).Policy == "AD";
            Assert.IsTrue(admin, "Admin Role not found");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasProfileAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetFilteredCustomAttributes(true).OfType<ProfileAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "ProfileAttribute not found.");
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
            //Noted bumped up to 25 for test method 
            Assert.AreEqual(25, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        /// <summary>
        /// 1
        /// </summary>
        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 2
        /// </summary>
        [TestMethod]
        public void TestControllerMethodModifyDepartmentalGetContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ModifyDepartmental");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 3
        /// </summary>
        [TestMethod]
        public void TestControllerMethodModifyDepartmentalPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ModifyDepartmental");
            var element = controllerMethod.ElementAt(1);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetFilteredCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 4
        /// </summary>
        [TestMethod]
        public void TestControllerMethodModifyAdminGetContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ModifyAdmin");
            #endregion Arrange

            #region Act           
            var allAttributes = controllerMethod.ElementAt(0).GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 5
        /// </summary>
        [TestMethod]
        public void TestControllerMethodModifyAdminPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ModifyAdmin");
            var element = controllerMethod.ElementAt(1);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetFilteredCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 6
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRemoveAdminGetContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "RemoveAdmin");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.ElementAt(0).GetFilteredCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 7
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRemoveAdminRoleContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "RemoveAdminRole");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetFilteredCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 8
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRemoveDepartmentalContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "RemoveDepartmental");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
           // var expectedAttribute = element.GetFilteredCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
           // Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 9
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRemoveDepartmentalRoleContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "RemoveDepartmentalRole");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetFilteredCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 10
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCloneContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Clone");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 11
        /// </summary>
        [TestMethod]
        public void TestControllerMethodFindUserContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "FindUser");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 12
        /// </summary>
        [TestMethod]
        public void TestControllerMethodSearchOrgsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "SearchOrgs");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 13
        /// </summary>
        [TestMethod]
        public void TestControllerMethodUpdateChildWorkgroupsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "UpdateChildWorkgroups");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 14
        /// </summary>
        [TestMethod]
        public void TestControllerMethodProcessWorkGroupContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ProcessWorkGroup");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetFilteredCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 15
        /// </summary>
        [TestMethod]
        public void TestControllerMethodGetChildWorkgroupIdsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "GetChildWorkgroupIds");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetFilteredCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 16
        /// </summary>
        [TestMethod]
        public void TestControllerMethodValidateChildWorkgroupsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ValidateChildWorkgroups");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 17
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRemoveExtraChildPermissionsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "RemoveExtraChildPermissions");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 18
        /// </summary>
        [TestMethod]
        public void TestControllerMethodTestExceptionContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("TestException");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 19
        /// </summary>
        [TestMethod]
        public void TestControllerMethodTestEmailContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("TestEmail");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 20
        /// </summary>
        [TestMethod]
        public void TestControllerMethodNeedToCheckWorkgroupPermissionsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("NeedToCheckWorkgroupPermissions");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetFilteredCustomAttributes(true).OfType<AllowAnonymousAttribute>();
            var allAttributes = controllerMethod.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, allAttributes.Count());
            Assert.AreEqual(1, expectedAttribute.Count());
            #endregion Assert
        }

        /// <summary>
        /// 21
        /// </summary>
        [TestMethod]
        public void TestControllerMethodModifySscAdminGetContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ModifySscAdmin");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 22
        /// </summary>
        [TestMethod]
        public void TestControllerMethodModifySscAdminPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ModifySscAdmin");
            var element = controllerMethod.ElementAt(1);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetFilteredCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 23
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRemoveSscAdminGetContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "RemoveSscAdmin");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.ElementAt(0).GetFilteredCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// 24
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRemoveSscAdminRoleContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "RemoveSscAdminRole");
            var element = controllerMethod.ElementAt(0);
            #endregion Arrange

            #region Act
            var expectedAttribute = element.GetFilteredCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetFilteredCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
