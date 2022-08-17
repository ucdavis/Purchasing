using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Helpers;
using UCDArch.Web.Attributes;
using VersionAttribute = Purchasing.Mvc.Attributes.VersionAttribute;

namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
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

        /// <summary>
        /// Tests the controller has only 7 attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasEightAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(8, result.Count());
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
            var result = controllerClass.GetCustomAttributes(true).OfType<AutoValidateAntiforgeryTokenAttribute>();
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
            var result = controllerClass.GetCustomAttributes(true).OfType<VersionAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "VersionAttribute not found.");
            #endregion Assert
        }

        //[TestMethod]
        //public void TestControllerHasAuthorizeAttribute()
        //{
        //    #region Arrange
        //    var controllerClass = ControllerClass;
        //    #endregion Arrange

        //    #region Act
        //    var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(result.Count() > 0, "AuthorizeAttribute not found.");
        //    #endregion Assert
        //}

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
            Assert.IsTrue(result.Count() > 0, "ProfileAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasAuthorizeWorkgroupAccessAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeWorkgroupAccessAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "AuthorizeWorkgroupAccessAttribute not found.");
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
            var found = false;
            for(int i = 0; i < result.Count(); i++)
            {
                if(result.ElementAt(i).Policy == "DA")
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "DA policy not Found");
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
            Assert.AreEqual(45, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        #region People Methods
        /// <summary>
        /// People #1 (1)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPeopleContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("People");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// People #2 (2)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAddPeopleGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "AddPeople");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(0);
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// People #3 (3)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAddPeoplePostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "AddPeople");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }


        /// <summary>
        /// People #4 (4)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDeletePeopleGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "DeletePeople");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(0);
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// People #5 (5)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDeletePeoplePostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "DeletePeople");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }


        /// <summary>
        /// People #6 (6)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodSearchUsersContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("SearchUsers");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }
        #endregion People Methods

        #region Actions Methods
        /// <summary>
        /// Actions #1 (7)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }


        /// <summary>
        /// Actions #4 (10)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDetailsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("Details");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Actions #5 (11)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Actions #6 (12)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Actions #7 (13)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDeleteGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Delete");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Actions #8 (14)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDeletePostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Delete");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }
        #endregion Actions Methods

        #region Address Methods
        /// <summary>
        /// Address #1 (15)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAddressesContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("Addresses");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Address #2 (16)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAddAddressGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "AddAddress");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Actions #3 (17)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAddAddressPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "AddAddress");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Address #4 (18)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDeleteAddressGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "DeleteAddress");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Actions #5 (19)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDeleteAddressPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "DeleteAddress");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Address #6 (20)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAddressDetailsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "AddressDetails");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Address #7 (21)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditAddressGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "EditAddress");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Actions #8 (22)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditAddressPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "EditAddress");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }
        #endregion Address Methods

        #region Workgroup Account Methods
        /// <summary>
        /// Accounts #1 (23)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAccountsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("Accounts");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Accounts #2 (24)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAddAccountGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "AddAccount");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Accounts #3 (25)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAddAccountPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "AddAccount");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Accounts #4 (26)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodAccountDetailsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "AccountDetails");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }


        /// <summary>
        /// Accounts #5 (27)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditAccountGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "EditAccount");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Accounts #6 (28)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditAccountPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "EditAccount");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Accounts #7 (37)
        /// </summary>
        [TestMethod]
        public void TestControllerMethoAccountDeleteGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "AccountDelete");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(0);
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }


        /// <summary>
        /// Accounts #8 (38)
        /// </summary>
        [TestMethod]
        public void TestControllerMethoAccountDeletePostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "AccountDelete");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count());
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Accounts #9 (43)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodUpdateMultipleAccountsGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "UpdateMultipleAccounts");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Accounts #10 (44)
        /// </summary>
        [TestMethod]
        public void TestControllerMethoUpdateMultipleAccountsPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "UpdateMultipleAccounts");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count());
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// (45)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodUpdateAccountContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("UpdateAccount");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        #endregion Workgroup Account Methods

        #region Workgroup Vendor Methods
        /// <summary>
        /// Vendors #1 (29)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodVendorListContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "VendorList");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Vendors #2 (30)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateVendorGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "CreateVendor");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Accounts #3 (31)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateVendorPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "CreateVendor");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Vendors #4 (32)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditWorkgroupVendorGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "EditWorkgroupVendor");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Vendors #5 (33)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditWorkgroupVendorPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "EditWorkgroupVendor");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Vendors #6 (34)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDeleteWorkgroupVendorGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "DeleteWorkgroupVendor");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Vendors #7 (35)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDeleteWorkgroupVendorPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "DeleteWorkgroupVendor");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Vendors #8 (36) -- (37 is in accounts)
        /// </summary>
        [TestMethod]
        public void TestControllerGetVendorAddressesContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "GetVendorAddresses");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Vendors #9 (39) 
        /// </summary>
        [TestMethod]
        public void TestControllerGetBulkVendorContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "BulkVendor");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Vendors #10 (40) 
        /// </summary>
        [TestMethod]
        public void TestControllerPostBulkVendorContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "BulkVendor");
            #endregion Arrange

            #region Act
            var element = controllerMethod.ElementAt(1);
            var expectedAttribute = element.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = element.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Vendors #11 (41) 
        /// </summary>
        [TestMethod]
        public void TestControllerCheckDuplicateVendorContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "CheckDuplicateVendor");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Vendors #12 Moved to Ajax Controller
        /// </summary>
        //[TestMethod]
        //public void TestControllerSearchBuildingContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = ControllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "SearchBuilding");
        //    #endregion Arrange

        //    #region Act
        //    var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(0, allAttributes.Count());
        //    #endregion Assert
        //}

        [TestMethod]
        public void TestControllerGetRequestersContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "GetRequesters");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Vendors ?? (42)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodExportableVendorListContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ExportableVendorList");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }
        #endregion Workgroup Vendor Methods

        #region Misc Methods
        /// <summary>
        /// Vendors #1 (29)
        /// </summary>
        [TestMethod]
        public void TestControllerMethodWhoHasAccessToWorkgroupContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "WhoHasAccessToWorkgroup");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        } 
        #endregion Misc Methods

        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
