using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.Windsor;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Core.Domain;
using Purchasing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;
using Purchasing.WS;

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
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.SelectWorkgroup());
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
                .AssertActionRedirect()
                .ToAction<OrderController>(a => a.SelectWorkgroup());
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
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual(Resources.NoAccess_Workgroup, Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestRequestGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "3");
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "3");
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


        #region Method Tests

        [TestMethod]
        public void TestWriteMethodTests()
        {
            #region Arrange
            Assert.Inconclusive("Need to write these tests");
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert
        }
        #endregion Method Tests
    }
}
