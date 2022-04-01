using System;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Models;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Purchasing.Tests.Core;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Tests.ControllerTests.CustomFieldControllerTests
{
    public partial class CustomFieldControllerTests
    {
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsToIndexIfCustomFieldNotFound()
        {
            #region Arrange
            new FakeCustomFields(3, CustomFieldRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(4)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Custom Field not found.", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditGetRedirectWhenNotAuthorized()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            customFields.Add(CreateValidEntities.CustomField(1));
            customFields[0].Organization = CreateValidEntities.Organization(9);

            new FakeCustomFields(0, CustomFieldRepository, customFields);
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Edit(1)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fail Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name9", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            customFields.Add(CreateValidEntities.CustomField(1));
            customFields[0].Organization = CreateValidEntities.Organization(9);

            new FakeCustomFields(0, CustomFieldRepository, customFields);
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out(null).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1)
                .AssertViewRendered()
                .WithViewData<CustomFieldViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.Organization.Name);
            Assert.AreEqual("Name9", result.CustomField.Organization.Name);
            Assert.AreEqual("Name1", result.CustomField.Name);
            Assert.IsTrue(result.CustomField.IsActive);
            Assert.AreEqual(0, result.CustomField.Rank);
            Assert.IsFalse(result.CustomField.IsRequired);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out(null).Dummy))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name9", ((Organization)args[1]).Name);
            #endregion Assert
        }
        #endregion Edit Get Tests

        #region Edit Post Tests
        [TestMethod]
        public void TestEditPostRedirectsToIndexIfCustomFieldNotFound()
        {
            #region Arrange
            new FakeCustomFields(3, CustomFieldRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(4, new CustomField())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Custom Field not found.", Controller.ErrorMessage);
            CustomFieldRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectWhenNotAuthorized()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            customFields.Add(CreateValidEntities.CustomField(1));
            customFields[0].Organization = CreateValidEntities.Organization(9);

            new FakeCustomFields(0, CustomFieldRepository, customFields);
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Edit(1, new CustomField())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fail Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name9", ((Organization)args[1]).Name);
            CustomFieldRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            customFields.Add(CreateValidEntities.CustomField(1));
            customFields[0].Organization = CreateValidEntities.Organization(9);

            new FakeCustomFields(0, CustomFieldRepository, customFields);
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out(null).Dummy)).Return(true);
            var customField = CreateValidEntities.CustomField(9);
            customField.Name = null;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, customField)
                .AssertViewRendered()
                .WithViewData<CustomFieldViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.Organization.Name);
            Assert.AreEqual("Name9", result.CustomField.Organization.Name);
            Assert.AreEqual(null, result.CustomField.Name);
            Assert.IsTrue(result.CustomField.IsActive);
            Assert.AreEqual(0, result.CustomField.Rank);
            Assert.IsFalse(result.CustomField.IsRequired);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out(null).Dummy))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name9", ((Organization)args[1]).Name);
            Controller.ModelState.AssertErrorsAre("The Name field is required.");
            CustomFieldRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostRedirectsWhenValid()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            customFields.Add(CreateValidEntities.CustomField(1));
            customFields[0].Organization = CreateValidEntities.Organization(9);
            customFields[0].Organization.SetIdTo("9");
            customFields[0].IsActive = true;
            new FakeCustomFields(0, CustomFieldRepository, customFields);
            var customField = CreateValidEntities.CustomField(8);
            customField.Organization = null;

            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out(null).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, customField)
                .AssertActionRedirect();           
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("9", result.RouteValues["id"]);
            Assert.AreEqual("CustomField Edited Successfully", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy));
            var ssargs = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out(null).Dummy))[0];
            Assert.IsNull(ssargs[0]);
            Assert.AreEqual("Name9", ((Organization)ssargs[1]).Name);

            CustomFieldRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything), x => x.Repeat.Times(2));
            var args = CustomFieldRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything)); 
            Assert.AreEqual(2, args.Count());
            Assert.AreEqual("Name1", ((CustomField)args[0][0]).Name);
            Assert.AreEqual(false, ((CustomField)args[0][0]).IsActive);
            Assert.AreEqual("Name8", ((CustomField)args[1][0]).Name);
            Assert.AreEqual(true, ((CustomField)args[1][0]).IsActive);
            Assert.AreEqual("Name9", ((CustomField)args[1][0]).Organization.Name);
            #endregion Assert		
        }

        #endregion Edit Post Tests

        #region Delete Get Tests

        [TestMethod]
        public void TestDeleteGetRedirectsToIndexIfCustomFieldNotFound()
        {
            #region Arrange
            new FakeCustomFields(3, CustomFieldRepository);
            #endregion Arrange

            #region Act
            Controller.Delete(4)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Custom Field not found.", Controller.ErrorMessage);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteGetRedirectWhenNotAuthorized()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            customFields.Add(CreateValidEntities.CustomField(1));
            customFields[0].Organization = CreateValidEntities.Organization(9);

            new FakeCustomFields(0, CustomFieldRepository, customFields);
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fail Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name9", ((Organization)args[1]).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteGetReturnsView()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            customFields.Add(CreateValidEntities.CustomField(1));
            customFields[0].Organization = CreateValidEntities.Organization(9);

            new FakeCustomFields(0, CustomFieldRepository, customFields);
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out(null).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(1)
                .AssertViewRendered()
                .WithViewData<CustomField>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.Organization.Name);
            Assert.AreEqual("Name1", result.Name);
            Assert.IsTrue(result.IsActive);
            Assert.AreEqual(0, result.Rank);
            Assert.IsFalse(result.IsRequired);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out(null).Dummy))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name9", ((Organization)args[1]).Name);
            #endregion Assert
        }
        #endregion Delete Get Tests

        #region Delete Post Tests

        [TestMethod]
        public void TestDeletePostRedirectsToIndexIfCustomFieldNotFound()
        {
            #region Arrange
            new FakeCustomFields(3, CustomFieldRepository);
            #endregion Arrange

            #region Act
            Controller.Delete(4, new CustomField())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Custom Field not found.", Controller.ErrorMessage);
            CustomFieldRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything));
            CustomFieldRepository.AssertWasNotCalled(a => a.Remove(Arg<CustomField>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectWhenNotAuthorized()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            customFields.Add(CreateValidEntities.CustomField(1));
            customFields[0].Organization = CreateValidEntities.Organization(9);

            new FakeCustomFields(0, CustomFieldRepository, customFields);
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Delete(1, new CustomField())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Fail Message", Controller.Message);
            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy));
            var args = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual("Name9", ((Organization)args[1]).Name);
            CustomFieldRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything));
            CustomFieldRepository.AssertWasNotCalled(a => a.Remove(Arg<CustomField>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostReturnsView()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            customFields.Add(CreateValidEntities.CustomField(1));
            customFields[0].Organization = CreateValidEntities.Organization(9);
            customFields[0].Organization.SetIdTo("9");
            customFields[0].IsActive = true;

            new FakeCustomFields(0, CustomFieldRepository, customFields);
            SecurityService.Expect(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out(null).Dummy)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(1, new CustomField())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("9", result.RouteValues["id"]);
            Assert.AreEqual("CustomField Removed Successfully", Controller.Message);

            SecurityService.AssertWasCalled(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out("Fail Message").Dummy));
            var ssargs = SecurityService.GetArgumentsForCallsMadeOn(a => a.HasWorkgroupOrOrganizationAccess(Arg<Workgroup>.Is.Anything,
                Arg<Organization>.Is.Anything,
                out Arg<string>.Out(null).Dummy))[0];
            Assert.IsNull(ssargs[0]);
            Assert.AreEqual("Name9", ((Organization)ssargs[1]).Name);

            CustomFieldRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything));
            var args = (CustomField) CustomFieldRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            Assert.IsFalse(args.IsActive);

            CustomFieldRepository.AssertWasNotCalled(a => a.Remove(Arg<CustomField>.Is.Anything));
            #endregion Assert
        }
        #endregion Delete Get Tests

        #region UpdateOrder Tests

        [TestMethod]
        public void TestUpdateOrderReturnsFalseWhenIdsAreNull()
        {
            #region Arrange
            new FakeCustomFields(10, CustomFieldRepository);
            #endregion Arrange

            #region Act
            var result = Controller.UpdateOrder("9", null)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("false", result.JsonResultString);
            CustomFieldRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestUpdateOrderReturnsTrueWhenNotExceptions1()
        {
            #region Arrange
            new FakeCustomFields(10, CustomFieldRepository);
            var idsToReorder = new List<int>() {99, 100};
            #endregion Arrange

            #region Act
            var result = Controller.UpdateOrder("9", idsToReorder)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("true", result.JsonResultString);
            CustomFieldRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestUpdateOrderReturnsTrueWhenNotExceptions2()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            var org = CreateValidEntities.Organization(9);
            org.SetIdTo("9");
            for (int i = 0; i < 5; i++)
            {
                customFields.Add(CreateValidEntities.CustomField(i+1));
                customFields[i].Organization = org;
                customFields[i].Rank = i + 1;
            }

            customFields[4].Rank = 1;
            customFields[0].Organization = CreateValidEntities.Organization(8);
            customFields[0].Organization.SetIdTo("8");
            new FakeCustomFields(0, CustomFieldRepository, customFields);
            var idsToReorder = new List<int>() { 1, 4,3,2,5,99 };
            #endregion Arrange

            #region Act
            var result = Controller.UpdateOrder("9", idsToReorder)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("true", result.JsonResultString);
            CustomFieldRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything), x => x.Repeat.Times(4));            
            var args = CustomFieldRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything)); 
            Assert.IsNotNull(args);
            Assert.AreEqual(4, ((CustomField)args[0][0]).Id);
            Assert.AreEqual(1, ((CustomField)args[0][0]).Rank);
            Assert.AreEqual(3, ((CustomField)args[1][0]).Id);
            Assert.AreEqual(2, ((CustomField)args[1][0]).Rank);
            Assert.AreEqual(2, ((CustomField)args[2][0]).Id);
            Assert.AreEqual(3, ((CustomField)args[2][0]).Rank);
            Assert.AreEqual(5, ((CustomField)args[3][0]).Id);
            Assert.AreEqual(4, ((CustomField)args[3][0]).Rank);
            #endregion Assert
        }


        [TestMethod]
        public void TestUpdateOrderReturnsFalseWhenException()
        {
            #region Arrange
            var customFields = new List<CustomField>();
            var org = CreateValidEntities.Organization(9);
            org.SetIdTo("9");
            for (int i = 0; i < 5; i++)
            {
                customFields.Add(CreateValidEntities.CustomField(i + 1));
                customFields[i].Organization = org;
                customFields[i].Rank = i + 1;
            }

            customFields[4].Rank = 1;
            customFields[0].Organization = CreateValidEntities.Organization(8);
            customFields[0].Organization.SetIdTo("8");
            new FakeCustomFields(0, CustomFieldRepository, customFields);
            var idsToReorder = new List<int>() { 1, 4, 3, 2, 5, 99 };

            CustomFieldRepository.Expect(a => a.EnsurePersistent(Arg<CustomField>.Is.Anything))
                .Throw(new ApplicationException("Fake Exception"));

            #endregion Arrange

            #region Act
            var result = Controller.UpdateOrder("9", idsToReorder)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("false", result.JsonResultString);
            CustomFieldRepository.AssertWasCalled(a => a.GetNullableById(Arg<int>.Is.Anything), x => x.Repeat.Times(2));
            #endregion Assert		
        }

        #endregion UpdateOrder Tests
    }
}
