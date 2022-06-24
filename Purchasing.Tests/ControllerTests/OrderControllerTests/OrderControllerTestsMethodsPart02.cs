using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Purchasing.Tests.ControllerTests.OrderControllerTests
{
    public partial class OrderControllerTests
    {

        #region Request Get Tests

        [TestMethod]
        public void TestRequestRedirectsToSelectWorkgroupIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.Request(4)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("workgroup not found.", Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestRequestRedirectsToSelectWorkgroupIfWorkgroupNotActive()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            workgroups.Add(CreateValidEntities.Workgroup(1));
            workgroups[0].IsActive = false;
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            Controller.Request(1)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("workgroup not active.", Controller.ErrorMessage);
            #endregion Assert
        }


        [TestMethod]
        public void TestRequestGetRedirectsToNotAuthorizedIfUserIsNotRequesterInWorkgroup()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeWorkgroupPermissions(3, WorkgroupPermissionRepository);
            #endregion Arrange

            #region Act
            Controller.Request(2)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NoAccess_Workgroup, Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestRequestGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] {""}, "3");
            SetupRoles();
            new FakeUsers(3, UserRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            var wps = new List<WorkgroupPermission>();
            wps.Add(CreateValidEntities.WorkgroupPermission(1));
            wps[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            wps[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, wps);

            new FakeUnitOfMeasures(3, RepositoryFactory.UnitOfMeasureRepository);
            new FakeWorkgroupAccounts(3, RepositoryFactory.WorkgroupAccountRepository);
            new FakeWorkgroupVendors(3, RepositoryFactory.WorkgroupVendorRepository);
            new FakeWorkAddresses(3, RepositoryFactory.WorkgroupAddressRepository);
            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);
            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);

            //var units = RepositoryFactory.UnitOfMeasureRepository.Queryable.Cache().ToList();
            //var accounts =
            //    RepositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == 2).
            //        Select(x => x.Account).ToFuture();
            //var vendors =
            //    RepositoryFactory.WorkgroupVendorRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).OrderBy(a => a.Name).ToFuture();
            //var addresses =
            //    RepositoryFactory.WorkgroupAddressRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).ToFuture();
            //var shippingTypes = RepositoryFactory.ShippingTypeRepository.Queryable.Cache().ToList();
            //var approvers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.Approver).Select(x => x.User).
            //        ToFuture();
            //var accountManagers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).
            //        ToFuture();
            //var conditionalApprovals = WorkgroupRepository.Queryable.Single(a => a.Id = 2).AllConditionalApprovals;
            //var    customFields = RepositoryFactory.CustomFieldRepository.Queryable.Where(x => x.Organization.Id == WorkgroupRepository.Queryable.Single(a => a.Id == 2).PrimaryOrganization.Id && x.IsActive).ToFuture().ToList(); //call to list to exec the future batch

            #endregion Arrange

            #region Act
            var result = Controller.Request(2)
                .AssertViewRendered()
                .WithViewData<OrderModifyModel>();
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        [TestMethod]
        public void TestRequestGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
            SetupRoles();
            new FakeUsers(3, UserRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            var wps = new List<WorkgroupPermission>();
            wps.Add(CreateValidEntities.WorkgroupPermission(1));
            wps[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            wps[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, wps);

            new FakeUnitOfMeasures(3, RepositoryFactory.UnitOfMeasureRepository);
            var wgAccounts = new List<WorkgroupAccount>();
            for (int i = 0; i < 3; i++)
            {
                wgAccounts.Add(CreateValidEntities.WorkgroupAccount(i+1));
                wgAccounts[i].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            }
            wgAccounts[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            new FakeWorkgroupAccounts(0, RepositoryFactory.WorkgroupAccountRepository, wgAccounts);
            new FakeWorkgroupVendors(3, RepositoryFactory.WorkgroupVendorRepository);
            new FakeWorkAddresses(3, RepositoryFactory.WorkgroupAddressRepository);
            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);
            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);

            //var units = RepositoryFactory.UnitOfMeasureRepository.Queryable.Cache().ToList();
            //var accounts =
            //    RepositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == 2).
            //        Select(x => x.Account).ToFuture();
            //var vendors =
            //    RepositoryFactory.WorkgroupVendorRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).OrderBy(a => a.Name).ToFuture();
            //var addresses =
            //    RepositoryFactory.WorkgroupAddressRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).ToFuture();
            //var shippingTypes = RepositoryFactory.ShippingTypeRepository.Queryable.Cache().ToList();
            //var approvers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.Approver).Select(x => x.User).
            //        ToFuture();
            //var accountManagers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).
            //        ToFuture();
            //var conditionalApprovals = WorkgroupRepository.Queryable.Single(a => a.Id = 2).AllConditionalApprovals;
            //var    customFields = RepositoryFactory.CustomFieldRepository.Queryable.Where(x => x.Organization.Id == WorkgroupRepository.Queryable.Single(a => a.Id == 2).PrimaryOrganization.Id && x.IsActive).ToFuture().ToList(); //call to list to exec the future batch

            #endregion Arrange

            #region Act
            var result = Controller.Request(2)
                .AssertViewRendered()
                .WithViewData<OrderModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull( result);
            Assert.AreEqual(2, result.Accounts.Count());
            Assert.IsNotNull(result.Order);
            Assert.AreEqual(3, result.Units.Count());
            Assert.AreEqual(3, result.ShippingTypes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestGetReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
            SetupRoles();
            new FakeUsers(3, UserRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            var wps = new List<WorkgroupPermission>();
            wps.Add(CreateValidEntities.WorkgroupPermission(1));
            wps[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            wps[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, wps);

            new FakeUnitOfMeasures(3, RepositoryFactory.UnitOfMeasureRepository);
            new FakeWorkgroupAccounts(3, RepositoryFactory.WorkgroupAccountRepository);
            var wgVendors = new List<WorkgroupVendor>();
            for (int i = 0; i < 5; i++)
            {
                wgVendors.Add(CreateValidEntities.WorkgroupVendor(i + 1));
                wgVendors[i].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            }
            wgVendors[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            wgVendors[2].IsActive = false;
            wgVendors[3].Name = "AAA";
            new FakeWorkgroupVendors(0, RepositoryFactory.WorkgroupVendorRepository, wgVendors);
            new FakeWorkAddresses(3, RepositoryFactory.WorkgroupAddressRepository);
            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);
            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);

            //var units = RepositoryFactory.UnitOfMeasureRepository.Queryable.Cache().ToList();
            //var accounts =
            //    RepositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == 2).
            //        Select(x => x.Account).ToFuture();
            //var vendors =
            //    RepositoryFactory.WorkgroupVendorRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).OrderBy(a => a.Name).ToFuture();
            //var addresses =
            //    RepositoryFactory.WorkgroupAddressRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).ToFuture();
            //var shippingTypes = RepositoryFactory.ShippingTypeRepository.Queryable.Cache().ToList();
            //var approvers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.Approver).Select(x => x.User).
            //        ToFuture();
            //var accountManagers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).
            //        ToFuture();
            //var conditionalApprovals = WorkgroupRepository.Queryable.Single(a => a.Id = 2).AllConditionalApprovals;
            //var    customFields = RepositoryFactory.CustomFieldRepository.Queryable.Where(x => x.Organization.Id == WorkgroupRepository.Queryable.Single(a => a.Id == 2).PrimaryOrganization.Id && x.IsActive).ToFuture().ToList(); //call to list to exec the future batch

            #endregion Arrange

            #region Act
            var result = Controller.Request(2)
                .AssertViewRendered()
                .WithViewData<OrderModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Vendors.Count());
            Assert.AreEqual("AAA", result.Vendors.ElementAt(0).Name);
            Assert.AreEqual("Name1", result.Vendors.ElementAt(1).Name);
            Assert.AreEqual("Name5", result.Vendors.ElementAt(2).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestGetReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
            SetupRoles();
            new FakeUsers(3, UserRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            var wps = new List<WorkgroupPermission>();
            wps.Add(CreateValidEntities.WorkgroupPermission(1));
            wps[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            wps[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, wps);

            new FakeUnitOfMeasures(3, RepositoryFactory.UnitOfMeasureRepository);
            new FakeWorkgroupAccounts(3, RepositoryFactory.WorkgroupAccountRepository);
            new FakeWorkgroupVendors(3, RepositoryFactory.WorkgroupVendorRepository);
            var wgAddresses = new List<WorkgroupAddress>();
            for (int i = 0; i < 5; i++)
            {
                wgAddresses.Add(CreateValidEntities.WorkgroupAddress(i + 1));
                wgAddresses[i].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            }
            wgAddresses[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            wgAddresses[2].IsActive = false;
            wgAddresses[3].Name = "AAA";
            new FakeWorkAddresses(0, RepositoryFactory.WorkgroupAddressRepository, wgAddresses);
            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);
            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);

            //var units = RepositoryFactory.UnitOfMeasureRepository.Queryable.Cache().ToList();
            //var accounts =
            //    RepositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == 2).
            //        Select(x => x.Account).ToFuture();
            //var vendors =
            //    RepositoryFactory.WorkgroupVendorRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).OrderBy(a => a.Name).ToFuture();
            //var addresses =
            //    RepositoryFactory.WorkgroupAddressRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).ToFuture();
            //var shippingTypes = RepositoryFactory.ShippingTypeRepository.Queryable.Cache().ToList();
            //var approvers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.Approver).Select(x => x.User).
            //        ToFuture();
            //var accountManagers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).
            //        ToFuture();
            //var conditionalApprovals = WorkgroupRepository.Queryable.Single(a => a.Id = 2).AllConditionalApprovals;
            //var    customFields = RepositoryFactory.CustomFieldRepository.Queryable.Where(x => x.Organization.Id == WorkgroupRepository.Queryable.Single(a => a.Id == 2).PrimaryOrganization.Id && x.IsActive).ToFuture().ToList(); //call to list to exec the future batch

            #endregion Arrange

            #region Act
            var result = Controller.Request(2)
                .AssertViewRendered()
                .WithViewData<OrderModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Addresses.Count());
            Assert.AreEqual("Name1", result.Addresses.ElementAt(0).Name);
            Assert.AreEqual("AAA", result.Addresses.ElementAt(1).Name);
            Assert.AreEqual("Name5", result.Addresses.ElementAt(2).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestGetReturnsView5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
            SetupRoles();
            new FakeUsers(3, UserRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            var wps = new List<WorkgroupPermission>();
            wps.Add(CreateValidEntities.WorkgroupPermission(1));
            wps[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            wps[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);

            wps.Add(CreateValidEntities.WorkgroupPermission(2));
            wps[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            wps[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);

            wps.Add(CreateValidEntities.WorkgroupPermission(3));
            wps[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[2].User = UserRepository.Queryable.Single(a => a.Id == "2");
            wps[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Approver);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, wps);

            new FakeUnitOfMeasures(3, RepositoryFactory.UnitOfMeasureRepository);
            new FakeWorkgroupAccounts(3, RepositoryFactory.WorkgroupAccountRepository);
            new FakeWorkgroupVendors(3, RepositoryFactory.WorkgroupVendorRepository);
            new FakeWorkAddresses(0, RepositoryFactory.WorkgroupAddressRepository);
            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);
            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);

            //var units = RepositoryFactory.UnitOfMeasureRepository.Queryable.Cache().ToList();
            //var accounts =
            //    RepositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == 2).
            //        Select(x => x.Account).ToFuture();
            //var vendors =
            //    RepositoryFactory.WorkgroupVendorRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).OrderBy(a => a.Name).ToFuture();
            //var addresses =
            //    RepositoryFactory.WorkgroupAddressRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).ToFuture();
            //var shippingTypes = RepositoryFactory.ShippingTypeRepository.Queryable.Cache().ToList();
            //var approvers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.Approver).Select(x => x.User).
            //        ToFuture();
            //var accountManagers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).
            //        ToFuture();
            //var conditionalApprovals = WorkgroupRepository.Queryable.Single(a => a.Id = 2).AllConditionalApprovals;
            //var    customFields = RepositoryFactory.CustomFieldRepository.Queryable.Where(x => x.Organization.Id == WorkgroupRepository.Queryable.Single(a => a.Id == 2).PrimaryOrganization.Id && x.IsActive).ToFuture().ToList(); //call to list to exec the future batch

            #endregion Arrange

            #region Act
            var result = Controller.Request(2)
                .AssertViewRendered()
                .WithViewData<OrderModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Approvers.Count());
            Assert.AreEqual(0, result.AccountManagers.Count());
            Assert.AreEqual("FirstName1", result.Approvers.ElementAt(0).FirstName);
            Assert.AreEqual("FirstName2", result.Approvers.ElementAt(1).FirstName);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestGetReturnsView6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
            SetupRoles();
            new FakeUsers(3, UserRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            var wps = new List<WorkgroupPermission>();
            wps.Add(CreateValidEntities.WorkgroupPermission(1));
            wps[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            wps[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);

            wps.Add(CreateValidEntities.WorkgroupPermission(2));
            wps[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[1].User = UserRepository.Queryable.Single(a => a.Id == "1");
            wps[1].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);

            wps.Add(CreateValidEntities.WorkgroupPermission(3));
            wps[2].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[2].User = UserRepository.Queryable.Single(a => a.Id == "2");
            wps[2].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.AccountManager);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, wps);

            new FakeUnitOfMeasures(3, RepositoryFactory.UnitOfMeasureRepository);
            new FakeWorkgroupAccounts(3, RepositoryFactory.WorkgroupAccountRepository);
            new FakeWorkgroupVendors(3, RepositoryFactory.WorkgroupVendorRepository);
            new FakeWorkAddresses(0, RepositoryFactory.WorkgroupAddressRepository);
            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);
            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);

            //var units = RepositoryFactory.UnitOfMeasureRepository.Queryable.Cache().ToList();
            //var accounts =
            //    RepositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == 2).
            //        Select(x => x.Account).ToFuture();
            //var vendors =
            //    RepositoryFactory.WorkgroupVendorRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).OrderBy(a => a.Name).ToFuture();
            //var addresses =
            //    RepositoryFactory.WorkgroupAddressRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).ToFuture();
            //var shippingTypes = RepositoryFactory.ShippingTypeRepository.Queryable.Cache().ToList();
            //var approvers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.Approver).Select(x => x.User).
            //        ToFuture();
            //var accountManagers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).
            //        ToFuture();
            //var conditionalApprovals = WorkgroupRepository.Queryable.Single(a => a.Id = 2).AllConditionalApprovals;
            //var    customFields = RepositoryFactory.CustomFieldRepository.Queryable.Where(x => x.Organization.Id == WorkgroupRepository.Queryable.Single(a => a.Id == 2).PrimaryOrganization.Id && x.IsActive).ToFuture().ToList(); //call to list to exec the future batch

            #endregion Arrange

            #region Act
            var result = Controller.Request(2)
                .AssertViewRendered()
                .WithViewData<OrderModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Approvers.Count());
            Assert.AreEqual(2, result.AccountManagers.Count());
            Assert.AreEqual("FirstName1", result.AccountManagers.ElementAt(0).FirstName);
            Assert.AreEqual("FirstName2", result.AccountManagers.ElementAt(1).FirstName);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestGetReturnsView7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
            SetupRoles();
            new FakeUsers(3, UserRepository);
            var wgs = new List<Workgroup>();
            wgs.Add(CreateValidEntities.Workgroup(1));
            wgs.Add(CreateValidEntities.Workgroup(2));
            wgs[1].ConditionalApprovals.Add(CreateValidEntities.ConditionalApproval(1));
            wgs[1].ConditionalApprovals.Add(CreateValidEntities.ConditionalApproval(2));
            wgs[1].ConditionalApprovals.Add(CreateValidEntities.ConditionalApproval(3));
            new FakeWorkgroups(0, WorkgroupRepository, wgs);
            var wps = new List<WorkgroupPermission>();
            wps.Add(CreateValidEntities.WorkgroupPermission(1));
            wps[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            wps[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, wps);

            new FakeUnitOfMeasures(3, RepositoryFactory.UnitOfMeasureRepository);
            new FakeWorkgroupAccounts(3, RepositoryFactory.WorkgroupAccountRepository);
            new FakeWorkgroupVendors(3, RepositoryFactory.WorkgroupVendorRepository);
            new FakeWorkAddresses(0, RepositoryFactory.WorkgroupAddressRepository);
            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);
            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);

            //var units = RepositoryFactory.UnitOfMeasureRepository.Queryable.Cache().ToList();
            //var accounts =
            //    RepositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == 2).
            //        Select(x => x.Account).ToFuture();
            //var vendors =
            //    RepositoryFactory.WorkgroupVendorRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).OrderBy(a => a.Name).ToFuture();
            //var addresses =
            //    RepositoryFactory.WorkgroupAddressRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).ToFuture();
            //var shippingTypes = RepositoryFactory.ShippingTypeRepository.Queryable.Cache().ToList();
            //var approvers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.Approver).Select(x => x.User).
            //        ToFuture();
            //var accountManagers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).
            //        ToFuture();
            //var conditionalApprovals = WorkgroupRepository.Queryable.Single(a => a.Id = 2).AllConditionalApprovals;
            //var    customFields = RepositoryFactory.CustomFieldRepository.Queryable.Where(x => x.Organization.Id == WorkgroupRepository.Queryable.Single(a => a.Id == 2).PrimaryOrganization.Id && x.IsActive).ToFuture().ToList(); //call to list to exec the future batch

            #endregion Arrange

            #region Act
            var result = Controller.Request(2)
                .AssertViewRendered()
                .WithViewData<OrderModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.ConditionalApprovals.Count());

            #endregion Assert
        }

        [TestMethod]
        public void TestRequestGetReturnsView8()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "3");
            SetupRoles();
            new FakeUsers(3, UserRepository);
            new FakeOrganizations(3, RepositoryFactory.OrganizationRepository);
            var wgs = new List<Workgroup>();
            wgs.Add(CreateValidEntities.Workgroup(1));
            wgs.Add(CreateValidEntities.Workgroup(2));
            wgs[1].PrimaryOrganization = RepositoryFactory.OrganizationRepository.Queryable.Single(a => a.Id == "3");
            new FakeWorkgroups(0, WorkgroupRepository, wgs);

            var wps = new List<WorkgroupPermission>();
            wps.Add(CreateValidEntities.WorkgroupPermission(1));
            wps[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            wps[0].User = UserRepository.Queryable.Single(a => a.Id == "3");
            wps[0].Role = RoleRepository.Queryable.Single(a => a.Id == Role.Codes.Requester);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, wps);

            new FakeUnitOfMeasures(3, RepositoryFactory.UnitOfMeasureRepository);
            new FakeWorkgroupAccounts(3, RepositoryFactory.WorkgroupAccountRepository);
            new FakeWorkgroupVendors(3, RepositoryFactory.WorkgroupVendorRepository);
            new FakeWorkAddresses(0, RepositoryFactory.WorkgroupAddressRepository);
            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);


            var customFields = new List<CustomField>();
            for (int i = 0; i < 5; i++)
            {
                customFields.Add(CreateValidEntities.CustomField(i+1));
                customFields[i].Organization = RepositoryFactory.OrganizationRepository.Queryable.Single(a => a.Id == "3");
            }
            customFields[1].IsActive = false;
            customFields[2].Organization = RepositoryFactory.OrganizationRepository.Queryable.Single(a => a.Id == "2");
            new FakeCustomFields(0, RepositoryFactory.CustomFieldRepository, customFields);

            //var units = RepositoryFactory.UnitOfMeasureRepository.Queryable.Cache().ToList();
            //var accounts =
            //    RepositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == 2).
            //        Select(x => x.Account).ToFuture();
            //var vendors =
            //    RepositoryFactory.WorkgroupVendorRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).OrderBy(a => a.Name).ToFuture();
            //var addresses =
            //    RepositoryFactory.WorkgroupAddressRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.IsActive).ToFuture();
            //var shippingTypes = RepositoryFactory.ShippingTypeRepository.Queryable.Cache().ToList();
            //var approvers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.Approver).Select(x => x.User).
            //        ToFuture();
            //var accountManagers =
            //    RepositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
            //        x => x.Workgroup.Id == 2 && x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).
            //        ToFuture();
            //var conditionalApprovals = WorkgroupRepository.Queryable.Single(a => a.Id = 2).AllConditionalApprovals;
            var    customFields2 = RepositoryFactory.CustomFieldRepository.Queryable.Where(x => x.Organization.Id == WorkgroupRepository.Queryable.Single(a => a.Id == 2).PrimaryOrganization.Id && x.IsActive).ToFuture().ToList(); //call to list to exec the future batch

            #endregion Arrange

            #region Act
            var result = Controller.Request(2)
                .AssertViewRendered()
                .WithViewData<OrderModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CustomFields.Count());
            Assert.AreEqual("Name1", result.CustomFields.ElementAt(0).Name);
            Assert.AreEqual("Name4", result.CustomFields.ElementAt(1).Name);
            Assert.AreEqual("Name5", result.CustomFields.ElementAt(2).Name);
            #endregion Assert
        }
        #endregion Request Get Tests

        #region Request Post Tests

        [TestMethod]
        [ExpectedException(typeof (PreconditionException))]
        public void TestRequestPostWhenNotAuthorizedForWorkgroup()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeWorkgroups(3, WorkgroupRepository);
                Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(WorkgroupRepository.Queryable.Single(b => b.Id == 2))).Returns(false);
                Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(WorkgroupRepository.Queryable.Single(b => b.Id == 2));
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Request(new OrderViewModel{Workgroup = 2});
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("canCreateOrderInWorkgroup", ex.Message);
                Mock.Get(SecurityService).Verify();
                throw ex;
            }
        }


        [TestMethod]
        public void TestRequestPostRedirectsToReview()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] {""}, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";
            orderViewModel.Account = "acct123";
            orderViewModel.Approvers = "some app";
            orderViewModel.AccountManagers = "acct manage";
            orderViewModel.ConditionalApprovals = new []{3,4,7};
            orderViewModel.Justification = "Some Just";
            orderViewModel.FormSaveId = SpecificGuid.GetGuid(7);

            Order OrderArgs = default;
            Mock.Get(RepositoryFactory.OrderRepository)
                .Setup(a => a.EnsurePersistent(It.IsAny<Order>()))
                .Callback<Order>((x) => 
                {
                    SetOrderInstance(x);//Set the ID to 99 when it is saved
                    OrderArgs = x;
                }); 
            object[] orderServiceArgs2 = default;
            Mock.Get(OrderService).Setup(a => a.HandleSavedForm(It.IsAny<Order>(), It.IsAny<Guid>()))
                .Callback((Order a, Guid b) => orderServiceArgs2 = new object[] { a, b });
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange


            #region Act
            var result = Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("Some Just", ((Order)orderServiceArgs1[0]).Justification);
            Assert.AreEqual("acct123", orderServiceArgs1[2]);
            Assert.AreEqual("some app", orderServiceArgs1[3]);
            Assert.AreEqual("acct manage", orderServiceArgs1[4]);
            Assert.AreEqual(" 3 4 7", ((int[])orderServiceArgs1[1]).IntArrayToString());

            Mock.Get(OrderService).Verify(a => a.HandleSavedForm(It.IsAny<Order>(), It.IsAny<Guid>()));

            Assert.AreEqual("Some Just", ((Order)orderServiceArgs2[0]).Justification);
            Assert.AreEqual(SpecificGuid.GetGuid(7), ((Guid)orderServiceArgs2[1]));

            Mock.Get(OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()));
 

            Assert.AreEqual(99, result.RouteValues["id"]);
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            #endregion Assert		
        }

        #region Mostly BindOrderModel Tests


        [TestMethod]
        public void TestRequestPostWorkgroupBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;           
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("Name2", ((Order)orderServiceArgs1[0]).Workgroup.Name);
            #endregion Assert		
        }

        [TestMethod]
        public void TestRequestPostVendorBinding1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Vendor = 0;
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(null, ((Order)orderServiceArgs1[0]).Vendor);
            #endregion Assert
        }


        [TestMethod]
        public void TestRequestPostVendorBinding2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            new FakeWorkgroupVendors(3, RepositoryFactory.WorkgroupVendorRepository);
            Mock.Get(RepositoryFactory.WorkgroupVendorRepository).Setup(a => a.GetById(2)).Returns(RepositoryFactory.WorkgroupVendorRepository.Queryable.Single(b => b.Id == 2));

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Vendor = 2;
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("Name2", ((Order)orderServiceArgs1[0]).Vendor.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostAddressBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            new FakeWorkgroupAddress(3, RepositoryFactory.WorkgroupAddressRepository);
            Mock.Get(RepositoryFactory.WorkgroupAddressRepository).Setup(a => a.GetById(2)).Returns(RepositoryFactory.WorkgroupAddressRepository.Queryable.Single(b => b.Id == 2));

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.ShipAddress = 2;
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("Name2", ((Order)orderServiceArgs1[0]).Address.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostShippingTypeBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);
            Mock.Get(RepositoryFactory.ShippingTypeRepository).Setup(a => a.GetById("2")).Returns(RepositoryFactory.ShippingTypeRepository.Queryable.Single(b => b.Id == "2"));

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.ShippingType = "2";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("Name2", ((Order)orderServiceArgs1[0]).ShippingType.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostDateNeededBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.DateNeeded = DateTime.UtcNow.ToPacificTime().AddDays(3).Date;
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().AddDays(3).Date, ((Order)orderServiceArgs1[0]).DateNeeded);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostAllowBackorderBinding1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Backorder = "true";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(true, ((Order)orderServiceArgs1[0]).AllowBackorder);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostAllowBackorderBinding2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Backorder = string.Empty;
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(false, ((Order)orderServiceArgs1[0]).AllowBackorder);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostOrganizationBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var workgroups = new List<Workgroup>();
            workgroups.Add(CreateValidEntities.Workgroup(1));
            workgroups[0].PrimaryOrganization = CreateValidEntities.Organization(77);
            workgroups[0].Id = 1;
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 1);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);;
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(1)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 1;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.DateNeeded = DateTime.UtcNow.ToPacificTime().AddDays(3).Date;
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("Name77", ((Order)orderServiceArgs1[0]).Organization.Name);
            #endregion Assert
        }


        [TestMethod]
        public void TestRequestPostDeliverToBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.ShipTo = "Some Ship";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("Some Ship", ((Order)orderServiceArgs1[0]).DeliverTo);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostDeliverToEmailBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.ShipEmail = "ship@testy.com";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("ship@testy.com", ((Order)orderServiceArgs1[0]).DeliverToEmail);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostDeliverToPhoneBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.ShipPhone = "222 333 4444";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("222 333 4444", ((Order)orderServiceArgs1[0]).DeliverToPhone);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostOrderTypeBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            var orderTypes = new List<OrderType>();
            for (int i = 0; i < 3; i++)
            {
                orderTypes.Add(CreateValidEntities.OrderType(i+1));
                orderTypes[i].Id = (i + 1).ToString();
            }
            orderTypes[1].Id = OrderType.Types.OrderRequest;
            new FakeOrderTypes(0, RepositoryFactory.OrderTypeRepository, orderTypes, true);
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });
            
            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(OrderType.Types.OrderRequest, ((Order)orderServiceArgs1[0]).OrderType.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostCreatedByBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "2");
            new FakeUsers(3, RepositoryFactory.UserRepository);
            Mock.Get(RepositoryFactory.UserRepository).Setup(a => a.GetNullableById("2")).Returns(RepositoryFactory.UserRepository.Queryable.Single(b => b.Id == "2"));

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("FirstName2", ((Order)orderServiceArgs1[0]).CreatedBy.FirstName);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostJustificationBinding()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            
            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Justification = "Some Just";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual("Some Just", ((Order)orderServiceArgs1[0]).Justification);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostCommentsBinding1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "2");
            new FakeUsers(3, RepositoryFactory.UserRepository);
            Mock.Get(RepositoryFactory.UserRepository).Setup(a => a.GetNullableById("2")).Returns(RepositoryFactory.UserRepository.Queryable.Single(b => b.Id == "2"));

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Comments = "This is my Comment";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(1, ((Order)orderServiceArgs1[0]).OrderComments.Count);
            Assert.AreEqual("This is my Comment", ((Order)orderServiceArgs1[0]).OrderComments[0].Text);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, ((Order)orderServiceArgs1[0]).OrderComments[0].DateCreated.Date);
            Assert.AreEqual("FirstName2", ((Order)orderServiceArgs1[0]).OrderComments[0].User.FirstName);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostCommentsBinding2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "2");
            new FakeUsers(3, RepositoryFactory.UserRepository);
            Mock.Get(RepositoryFactory.UserRepository).Setup(a => a.GetNullableById("2")).Returns(RepositoryFactory.UserRepository.Queryable.Single(b => b.Id == "2"));

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Comments = "  ";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(0, ((Order)orderServiceArgs1[0]).OrderComments.Count);
            //Assert.AreEqual("This is my Comment", ((Order)orderServiceArgs1[0]).OrderComments[0].Text);
            //Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, ((Order)orderServiceArgs1[0]).OrderComments[0].DateCreated.Date);
            //Assert.AreEqual("FirstName2", ((Order)orderServiceArgs1[0]).OrderComments[0].User.FirstName);
            #endregion Assert
        }
        [TestMethod]
        public void TestRequestPostCommentsBinding3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "2");
            new FakeUsers(3, RepositoryFactory.UserRepository);
            Mock.Get(RepositoryFactory.UserRepository).Setup(a => a.GetNullableById("2")).Returns(RepositoryFactory.UserRepository.Queryable.Single(b => b.Id == "2"));

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Comments = string.Empty;
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(0, ((Order)orderServiceArgs1[0]).OrderComments.Count);
            //Assert.AreEqual("This is my Comment", ((Order)orderServiceArgs1[0]).OrderComments[0].Text);
            //Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, ((Order)orderServiceArgs1[0]).OrderComments[0].DateCreated.Date);
            //Assert.AreEqual("FirstName2", ((Order)orderServiceArgs1[0]).OrderComments[0].User.FirstName);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostCommentsBinding4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "2");
            new FakeUsers(3, RepositoryFactory.UserRepository);
            Mock.Get(RepositoryFactory.UserRepository).Setup(a => a.GetNullableById("2")).Returns(RepositoryFactory.UserRepository.Queryable.Single(b => b.Id == "2"));

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Comments = null;
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(0, ((Order)orderServiceArgs1[0]).OrderComments.Count);
            //Assert.AreEqual("This is my Comment", ((Order)orderServiceArgs1[0]).OrderComments[0].Text);
            //Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, ((Order)orderServiceArgs1[0]).OrderComments[0].DateCreated.Date);
            //Assert.AreEqual("FirstName2", ((Order)orderServiceArgs1[0]).OrderComments[0].User.FirstName);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostAttachmentsBinding1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeAttachments(5, RepositoryFactory.AttachmentRepository);

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.FileIds = null;
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(0, ((Order)orderServiceArgs1[0]).Attachments.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostAttachmentsBinding2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeAttachments(5, RepositoryFactory.AttachmentRepository, null, false, true);

            var xxx = RepositoryFactory.AttachmentRepository.Queryable.ToList();

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.FileIds = new[]{SpecificGuid.GetGuid(2)};
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(0, ((Order)orderServiceArgs1[0]).Attachments.Count); //This is now zero because the attachments get added after the order is saved
            
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostAttachmentsBinding3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeAttachments(5, RepositoryFactory.AttachmentRepository, null, false, true);

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.FileIds = new[] { SpecificGuid.GetGuid(2), SpecificGuid.GetGuid(3), SpecificGuid.GetGuid(4) };
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(0, ((Order)orderServiceArgs1[0]).Attachments.Count); //This is zero now because the attachments are added to the order after the order is saved
            //Assert.AreEqual(SpecificGuid.GetGuid(2), ((Order)orderServiceArgs1[0]).Attachments[0].Id);
            //Assert.AreEqual(SpecificGuid.GetGuid(3), ((Order)orderServiceArgs1[0]).Attachments[1].Id);
            //Assert.AreEqual(SpecificGuid.GetGuid(4), ((Order)orderServiceArgs1[0]).Attachments[2].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostCustomFieldAnswersBinding1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);
            for (int i = 0; i < 3; i++)
            {
                Mock.Get(RepositoryFactory.CustomFieldRepository).Setup(a => a.GetById(i + 1)).Returns(
                    RepositoryFactory.CustomFieldRepository.Queryable.Single(b => b.Id == (i + 1)));
            }
            orderViewModel.CustomFields = new OrderViewModel.CustomField[3];
            orderViewModel.CustomFields[0] = new OrderViewModel.CustomField{Answer = "Answer1", Id = 1};
            orderViewModel.CustomFields[1] = new OrderViewModel.CustomField { Answer = " ", Id = 2 };
            orderViewModel.CustomFields[2] = new OrderViewModel.CustomField { Answer = "Answer3", Id = 3 };
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(2, ((Order)orderServiceArgs1[0]).CustomFieldAnswers.Count);
            Assert.AreEqual("Name1", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[0].CustomField.Name);
            Assert.AreEqual("Name3", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[1].CustomField.Name);
            Assert.AreEqual("Answer1", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[0].Answer);
            Assert.AreEqual("Answer3", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[1].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostCustomFieldAnswersBinding2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);
            for (int i = 0; i < 3; i++)
            {
                Mock.Get(RepositoryFactory.CustomFieldRepository).Setup(a => a.GetById(i + 1)).Returns(
                    RepositoryFactory.CustomFieldRepository.Queryable.Single(b => b.Id == (i + 1)));
            }
            orderViewModel.CustomFields = new OrderViewModel.CustomField[3];
            orderViewModel.CustomFields[0] = new OrderViewModel.CustomField { Answer = "Answer1", Id = 1 };
            orderViewModel.CustomFields[1] = new OrderViewModel.CustomField { Answer = null, Id = 2 };
            orderViewModel.CustomFields[2] = new OrderViewModel.CustomField { Answer = "Answer3", Id = 3 };
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(2, ((Order)orderServiceArgs1[0]).CustomFieldAnswers.Count);
            Assert.AreEqual("Name1", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[0].CustomField.Name);
            Assert.AreEqual("Name3", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[1].CustomField.Name);
            Assert.AreEqual("Answer1", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[0].Answer);
            Assert.AreEqual("Answer3", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[1].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostCustomFieldAnswersBinding3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);
            for (int i = 0; i < 3; i++)
            {
                Mock.Get(RepositoryFactory.CustomFieldRepository).Setup(a => a.GetById(i + 1)).Returns(
                    RepositoryFactory.CustomFieldRepository.Queryable.Single(b => b.Id == (i + 1)));
            }
            orderViewModel.CustomFields = new OrderViewModel.CustomField[3];
            orderViewModel.CustomFields[0] = new OrderViewModel.CustomField { Answer = "Answer1", Id = 1 };
            orderViewModel.CustomFields[1] = new OrderViewModel.CustomField { Answer = string.Empty, Id = 2 };
            orderViewModel.CustomFields[2] = new OrderViewModel.CustomField { Answer = "Answer3", Id = 3 };
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(2, ((Order)orderServiceArgs1[0]).CustomFieldAnswers.Count);
            Assert.AreEqual("Name1", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[0].CustomField.Name);
            Assert.AreEqual("Name3", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[1].CustomField.Name);
            Assert.AreEqual("Answer1", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[0].Answer);
            Assert.AreEqual("Answer3", ((Order)orderServiceArgs1[0]).CustomFieldAnswers[1].Answer);
            #endregion Assert
        }


        [TestMethod]
        public void TestRequestPostRestrictedBinding1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Restricted = new OrderViewModel.ControlledSubstance();
            orderViewModel.Restricted.Status = "True";
            orderViewModel.Restricted.Class = "SomeClass";
            orderViewModel.Restricted.Use = "SomeUse";
            orderViewModel.Restricted.StorageSite = "SomeStorage";
            orderViewModel.Restricted.Custodian = "Somebody";
            orderViewModel.Restricted.Users = "Someone who is authorized";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(true, ((Order)orderServiceArgs1[0]).HasControlledSubstance);
            Assert.AreEqual("SomeClass", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().ClassSchedule);
            Assert.AreEqual("SomeUse", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().Use);
            Assert.AreEqual("SomeStorage", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().StorageSite);
            Assert.AreEqual("Somebody", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().Custodian);
            Assert.AreEqual("Someone who is authorized", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().EndUser);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostRestrictedBinding2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Restricted = new OrderViewModel.ControlledSubstance();
            orderViewModel.Restricted.Status = "true"; //Lower case is ignored
            orderViewModel.Restricted.Class = "SomeClass";
            orderViewModel.Restricted.Use = "SomeUse";
            orderViewModel.Restricted.StorageSite = "SomeStorage";
            orderViewModel.Restricted.Custodian = "Somebody";
            orderViewModel.Restricted.Users = "Someone who is authorized";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(false, ((Order)orderServiceArgs1[0]).HasControlledSubstance);
            Assert.IsNull(((Order)orderServiceArgs1[0]).GetAuthorizationInfo());
            //Assert.AreEqual("SomeClass", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().ClassSchedule);
            //Assert.AreEqual("SomeUse", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().Use);
            //Assert.AreEqual("SomeStorage", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().StorageSite);
            //Assert.AreEqual("Somebody", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().Custodian);
            //Assert.AreEqual("Someone who is authorized", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().EndUser);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostRestrictedBinding3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Restricted = new OrderViewModel.ControlledSubstance();
            orderViewModel.Restricted.Status = "false"; 
            orderViewModel.Restricted.Class = "SomeClass";
            orderViewModel.Restricted.Use = "SomeUse";
            orderViewModel.Restricted.StorageSite = "SomeStorage";
            orderViewModel.Restricted.Custodian = "Somebody";
            orderViewModel.Restricted.Users = "Someone who is authorized";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(false, ((Order)orderServiceArgs1[0]).HasControlledSubstance);
            Assert.IsNull(((Order)orderServiceArgs1[0]).GetAuthorizationInfo());
            //Assert.AreEqual("SomeClass", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().ClassSchedule);
            //Assert.AreEqual("SomeUse", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().Use);
            //Assert.AreEqual("SomeStorage", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().StorageSite);
            //Assert.AreEqual("Somebody", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().Custodian);
            //Assert.AreEqual("Someone who is authorized", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().EndUser);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostRestrictedBinding4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";

            orderViewModel.Restricted = null;
            //orderViewModel.Restricted.Status = "true"; //Lower case is ignored
            //orderViewModel.Restricted.Class = "SomeClass";
            //orderViewModel.Restricted.Use = "SomeUse";
            //orderViewModel.Restricted.StorageSite = "SomeStorage";
            //orderViewModel.Restricted.Custodian = "Somebody";
            //orderViewModel.Restricted.Users = "Someone who is authorized";
            object[] orderServiceArgs1 = default;
            Mock.Get(OrderService).Setup(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Order a, int[] b, string c, string d, string e) => orderServiceArgs1 = new object[] { a, b, c, d, e });

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Assert.AreEqual(false, ((Order)orderServiceArgs1[0]).HasControlledSubstance);
            Assert.IsNull(((Order)orderServiceArgs1[0]).GetAuthorizationInfo());
            //Assert.AreEqual("SomeClass", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().ClassSchedule);
            //Assert.AreEqual("SomeUse", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().Use);
            //Assert.AreEqual("SomeStorage", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().StorageSite);
            //Assert.AreEqual("Somebody", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().Custodian);
            //Assert.AreEqual("Someone who is authorized", ((Order)orderServiceArgs1[0]).GetAuthorizationInfo().EndUser);
            #endregion Assert
        }

        #region includeLineItemsAndSplits (BindOrderModel) Tests

        [TestMethod]
        public void TestRequestPostIncludeLineItemsAndSplits1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";
            object[] bugTrackingServiceArgs1 = default;
            Mock.Get(BugTrackingService).Setup(a => a.CheckForClearedOutSubAccounts(It.IsAny<Order>(), It.IsAny<OrderViewModel.Split[]>(), It.IsAny<OrderViewModel>()))
                .Callback((Order a, OrderViewModel.Split[] b, OrderViewModel c) => bugTrackingServiceArgs1 = new object[] { a, b, c });


            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Mock.Get(BugTrackingService).Verify(a => a.CheckForClearedOutSubAccounts(It.IsAny<Order>(), It.IsAny<OrderViewModel.Split[]>(), It.IsAny<OrderViewModel>()));

            Assert.AreEqual(1.23m, ((Order)bugTrackingServiceArgs1[0]).FreightAmount);
            Assert.AreEqual(null, bugTrackingServiceArgs1[1]);
            Assert.AreEqual("$1.23", ((OrderViewModel)bugTrackingServiceArgs1[2]).Freight);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostIncludeLineItemsAndSplits2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.25%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";
            orderViewModel.Splits = new OrderViewModel.Split[1];
            orderViewModel.Splits[0] = new OrderViewModel.Split();
            orderViewModel.Splits[0].Account = "Acct1";
            orderViewModel.Splits[0].Amount = "35";
            orderViewModel.Splits[0].SubAccount = "SubAcct1";
            object[] bugTrackingServiceArgs1 = default;
            Mock.Get(BugTrackingService).Setup(a => a.CheckForClearedOutSubAccounts(It.IsAny<Order>(), It.IsAny<OrderViewModel.Split[]>(), It.IsAny<OrderViewModel>()))
                .Callback((Order a, OrderViewModel.Split[] b, OrderViewModel c) => bugTrackingServiceArgs1 = new object[] { a, b, c });


            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(OrderService).Verify(a => a.CreateApprovalsForNewOrder(It.IsAny<Order>(), It.IsAny<int[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            Mock.Get(BugTrackingService).Verify(a => a.CheckForClearedOutSubAccounts(It.IsAny<Order>(), It.IsAny<OrderViewModel.Split[]>(), It.IsAny<OrderViewModel>()));

            Assert.AreEqual(1.23m, ((Order)bugTrackingServiceArgs1[0]).FreightAmount);
            Assert.AreEqual("Acct1", ((OrderViewModel.Split[])bugTrackingServiceArgs1[1])[0].Account);
            Assert.AreEqual("35", ((OrderViewModel.Split[])bugTrackingServiceArgs1[1])[0].Amount);
            Assert.AreEqual("SubAcct1", ((OrderViewModel.Split[])bugTrackingServiceArgs1[1])[0].SubAccount);
            Assert.AreEqual("$1.23", ((OrderViewModel)bugTrackingServiceArgs1[2]).Freight);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostIncludeLineItemsAndSplits3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.99%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";
            Order args = default;
            Mock.Get(RepositoryFactory.OrderRepository).Setup(a => a.EnsurePersistent(It.IsAny<Order>()))
                .Callback<Order>(x => args = x);


            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(RepositoryFactory.OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()));
 
            Assert.AreEqual(7.99m, args.EstimatedTax);
            Assert.AreEqual(2.35m, args.ShippingAmount);
            Assert.AreEqual(1.23m, args.FreightAmount);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostIncludeLineItemsAndSplits4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.99";
            orderViewModel.Shipping = "2.35";
            orderViewModel.Freight = "1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";
            Order args = default;
            Mock.Get(RepositoryFactory.OrderRepository).Setup(a => a.EnsurePersistent(It.IsAny<Order>()))
                .Callback<Order>(x => args = x);


            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(RepositoryFactory.OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()));

            Assert.AreEqual(7.99m, args.EstimatedTax);
            Assert.AreEqual(2.35m, args.ShippingAmount);
            Assert.AreEqual(1.23m, args.FreightAmount);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostIncludeLineItemsAndSplits5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "not valid";
            orderViewModel.Shipping = "not valid2";
            orderViewModel.Freight = "not valid 3";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Description = "Test";
            orderViewModel.Items[0].Quantity = "0";
            orderViewModel.Items[0].Price = "0";
            Order args = default;
            Mock.Get(RepositoryFactory.OrderRepository).Setup(a => a.EnsurePersistent(It.IsAny<Order>()))
                .Callback<Order>(x => args = x);


            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(RepositoryFactory.OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()));

            Assert.AreEqual(7.25m, args.EstimatedTax); //Default
            Assert.AreEqual(0, args.ShippingAmount); //Couldn't parse. Default used
            Assert.AreEqual(0, args.FreightAmount); //Couldn't parse. Default used
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostIncludeLineItemsAndSplits6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.99%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[3];
            for (int i = 0; i < 3; i++)
            {
                orderViewModel.Items[i] = new OrderViewModel.LineItem();
                orderViewModel.Items[i].Price = "1.02";
                orderViewModel.Items[i].Quantity = (i + 1).ToString();
            }

            orderViewModel.Items[1].Quantity = string.Empty;
            Order args = default;
            Mock.Get(RepositoryFactory.OrderRepository).Setup(a => a.EnsurePersistent(It.IsAny<Order>()))
                .Callback<Order>(x => args = x);

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(RepositoryFactory.OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()));

            Assert.AreEqual(2, args.LineItems.Count);
            Assert.AreEqual(1.02m, args.LineItems[0].UnitPrice);
            Assert.AreEqual(1, args.LineItems[0].Quantity);
            Assert.AreEqual(1.02m, args.LineItems[1].UnitPrice);
            Assert.AreEqual(3, args.LineItems[1].Quantity);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostIncludeLineItemsAndSplits7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.99%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Price = "1.99";
            orderViewModel.Items[0].Quantity = "14";
            orderViewModel.Items[0].CatalogNumber = "Cat1";
            orderViewModel.Items[0].CommodityCode = "123678";
            orderViewModel.Items[0].Description = "Desc1";
            orderViewModel.Items[0].Notes = "Notes1";
            orderViewModel.Items[0].Units = "Units1";
            orderViewModel.Items[0].Url = "Url99";

            new FakeCommodity(3, RepositoryFactory.CommodityRepository);
            Order args = default;
            Mock.Get(RepositoryFactory.OrderRepository).Setup(a => a.EnsurePersistent(It.IsAny<Order>()))
                .Callback<Order>(x => args = x);

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(RepositoryFactory.OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()));

            Assert.AreEqual(1, args.LineItems.Count);
            Assert.AreEqual(1.99m, args.LineItems[0].UnitPrice);
            Assert.AreEqual(14, args.LineItems[0].Quantity);
            Assert.AreEqual("Cat1", args.LineItems[0].CatalogNumber);

            Assert.AreEqual(null, args.LineItems[0].Commodity);
            Assert.AreEqual("Desc1", args.LineItems[0].Description);
            Assert.AreEqual("Notes1", args.LineItems[0].Notes);
            Assert.AreEqual("Units1", args.LineItems[0].Unit);
            Assert.AreEqual("Url99", args.LineItems[0].Url);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostIncludeLineItemsAndSplits8()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.99%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[1];
            orderViewModel.Items[0] = new OrderViewModel.LineItem();
            orderViewModel.Items[0].Price = "1.99";
            orderViewModel.Items[0].Quantity = "14";
            orderViewModel.Items[0].CatalogNumber = "Cat1";
            orderViewModel.Items[0].CommodityCode = "2";
            orderViewModel.Items[0].Description = "Desc1";
            orderViewModel.Items[0].Notes = "Notes1";
            orderViewModel.Items[0].Units = "Units1";
            orderViewModel.Items[0].Url = "Url99";

            new FakeCommodity(3, RepositoryFactory.CommodityRepository);
            Order args = default;
            Mock.Get(RepositoryFactory.OrderRepository).Setup(a => a.EnsurePersistent(It.IsAny<Order>()))
                .Callback<Order>(x => args = x);

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(RepositoryFactory.OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()));

            Assert.AreEqual(1, args.LineItems.Count);
            Assert.AreEqual(1.99m, args.LineItems[0].UnitPrice);
            Assert.AreEqual(14, args.LineItems[0].Quantity);
            Assert.AreEqual("Cat1", args.LineItems[0].CatalogNumber);

            Assert.AreEqual("2", args.LineItems[0].Commodity.Id);
            Assert.AreEqual("Desc1", args.LineItems[0].Description);
            Assert.AreEqual("Notes1", args.LineItems[0].Notes);
            Assert.AreEqual("Units1", args.LineItems[0].Unit);
            Assert.AreEqual("Url99", args.LineItems[0].Url);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostIncludeLineItemsAndSplits9()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.99%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[3];
            for (int i = 0; i < 3; i++)
            {
                orderViewModel.Items[i] = new OrderViewModel.LineItem();
                orderViewModel.Items[i].Price = "1.02";
                orderViewModel.Items[i].Quantity = (i + 1).ToString();
                orderViewModel.Items[i].Id = i;
            }

            orderViewModel.Items[1].Quantity = string.Empty;

            orderViewModel.SplitType = OrderViewModel.SplitTypes.Line;

            orderViewModel.Splits = new OrderViewModel.Split[4];
            for (int i = 0; i < 4; i++)
            {
                orderViewModel.Splits[i] = new OrderViewModel.Split();                
                orderViewModel.Splits[i].Account = string.Format("account{0}", i + 1);
                orderViewModel.Splits[i].Amount = "9.99";
            }

            orderViewModel.Splits[0].LineItemId = 0;
            orderViewModel.Splits[1].LineItemId = 0;
            orderViewModel.Splits[2].LineItemId = 2;
            orderViewModel.Splits[2].SubAccount = "SubAcct";
            orderViewModel.Splits[2].Project = "proj";
            orderViewModel.Splits[3].LineItemId = 2;
            orderViewModel.Splits[3].Account = string.Empty;
            Order args = default;
            Mock.Get(RepositoryFactory.OrderRepository).Setup(a => a.EnsurePersistent(It.IsAny<Order>()))
                .Callback<Order>(x => args = x);

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(RepositoryFactory.OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()));

            Assert.AreEqual(2, args.LineItems.Count);
            Assert.AreEqual(3, args.Splits.Count);
            Assert.AreEqual("account1", args.Splits[0].Account);
            Assert.AreEqual(1, args.Splits[0].LineItem.Quantity);
            Assert.AreEqual("account2", args.Splits[1].Account);
            Assert.AreEqual(1, args.Splits[1].LineItem.Quantity);
            Assert.AreEqual("account3", args.Splits[2].Account);
            Assert.AreEqual("SubAcct", args.Splits[2].SubAccount);
            Assert.AreEqual("proj", args.Splits[2].Project);
            Assert.AreEqual(3, args.Splits[2].LineItem.Quantity);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(9.99m, args.Splits[i].Amount);
            }
            #endregion Assert
        }
        #endregion includeLineItemsAndSplits (BindOrderModel) Tests

        [TestMethod]
        public void TestRequestPostOrderSplits1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.99%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[3];
            for (int i = 0; i < 3; i++)
            {
                orderViewModel.Items[i] = new OrderViewModel.LineItem();
                orderViewModel.Items[i].Price = "1.02";
                orderViewModel.Items[i].Quantity = (i + 1).ToString();
                orderViewModel.Items[i].Id = i;
            }

            orderViewModel.Items[1].Quantity = string.Empty;

            orderViewModel.SplitType = OrderViewModel.SplitTypes.Order;

            orderViewModel.Splits = new OrderViewModel.Split[4];
            for (int i = 0; i < 4; i++)
            {
                orderViewModel.Splits[i] = new OrderViewModel.Split();
                orderViewModel.Splits[i].Account = string.Format("account{0}", i + 1);
                orderViewModel.Splits[i].Amount = "9.99";
            }

            orderViewModel.Splits[0].LineItemId = 0;
            orderViewModel.Splits[1].LineItemId = 0;
            orderViewModel.Splits[2].LineItemId = 2;
            orderViewModel.Splits[2].SubAccount = "SubAcct";
            orderViewModel.Splits[2].Project = "proj";
            orderViewModel.Splits[3].LineItemId = 2;
            orderViewModel.Splits[3].Account = string.Empty;
            Order args = default;
            Mock.Get(RepositoryFactory.OrderRepository).Setup(a => a.EnsurePersistent(It.IsAny<Order>()))
                .Callback<Order>(x => args = x);

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(RepositoryFactory.OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()));

            Assert.AreEqual(2, args.LineItems.Count);
            Assert.AreEqual(3, args.Splits.Count);
            Assert.AreEqual("account1", args.Splits[0].Account);
            Assert.AreEqual(null, args.Splits[0].LineItem);
            Assert.AreEqual("account2", args.Splits[1].Account);
            Assert.AreEqual(null, args.Splits[1].LineItem);
            Assert.AreEqual("account3", args.Splits[2].Account);
            Assert.AreEqual("SubAcct", args.Splits[2].SubAccount);
            Assert.AreEqual("proj", args.Splits[2].Project);
            Assert.AreEqual(null, args.Splits[2].LineItem);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(9.99m, args.Splits[i].Amount);
            }
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestPostOrderSplits2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");

            new FakeWorkgroups(3, WorkgroupRepository);
            var workgroup = WorkgroupRepository.Queryable.Single(b => b.Id == 2);
            Mock.Get(SecurityService).Setup(a => a.HasWorkgroupAccess(workgroup)).Returns(true);
            Mock.Get(WorkgroupRepository).Setup(a => a.GetById(2)).Returns(workgroup);

            var orderViewModel = new OrderViewModel();
            orderViewModel.Workgroup = 2;
            orderViewModel.Tax = "7.99%";
            orderViewModel.Shipping = "$2.35";
            orderViewModel.Freight = "$1.23";
            orderViewModel.Items = new OrderViewModel.LineItem[3];
            for (int i = 0; i < 3; i++)
            {
                orderViewModel.Items[i] = new OrderViewModel.LineItem();
                orderViewModel.Items[i].Price = "1.02";
                orderViewModel.Items[i].Quantity = (i + 1).ToString();
                orderViewModel.Items[i].Id = i;
            }

            orderViewModel.Items[1].Quantity = string.Empty;

            orderViewModel.SplitType = OrderViewModel.SplitTypes.None;
            orderViewModel.Account = "DifferentAcct";
            orderViewModel.SubAccount = "DiffSubAcct";
            orderViewModel.Project = "DifferentProj";

            orderViewModel.Splits = new OrderViewModel.Split[4];
            for (int i = 0; i < 4; i++)
            {
                orderViewModel.Splits[i] = new OrderViewModel.Split();
                orderViewModel.Splits[i].Account = string.Format("account{0}", i + 1);
                orderViewModel.Splits[i].Amount = "9.99";
            }

            orderViewModel.Splits[0].LineItemId = 0;
            orderViewModel.Splits[1].LineItemId = 0;
            orderViewModel.Splits[2].LineItemId = 2;
            orderViewModel.Splits[2].SubAccount = "SubAcct";
            orderViewModel.Splits[2].Project = "proj";
            orderViewModel.Splits[3].LineItemId = 2;
            orderViewModel.Splits[3].Account = string.Empty;
            Order args = default;
            Mock.Get(RepositoryFactory.OrderRepository).Setup(a => a.EnsurePersistent(It.IsAny<Order>()))
                .Callback<Order>(x => args = x);

            #endregion Arrange

            #region Act
            Controller.Request(orderViewModel)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NewOrder_Success, Controller.Message);
            Mock.Get(RepositoryFactory.OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()));

            Assert.AreEqual(2, args.LineItems.Count);
            Assert.AreEqual(1, args.Splits.Count);
            Assert.AreEqual("DifferentAcct", args.Splits[0].Account);
            Assert.AreEqual("DiffSubAcct", args.Splits[0].SubAccount);
            Assert.AreEqual("DifferentProj", args.Splits[0].Project);
            Assert.AreEqual(null, args.Splits[0].LineItem);
            Assert.AreEqual(4.08m, args.Splits[0].Amount);
            Assert.AreEqual(4.08m, args.TotalFromDb);
            #endregion Assert
        }

        #endregion Mostly BindOrderModel Tests

        #endregion Request Post Tests
    }
}
