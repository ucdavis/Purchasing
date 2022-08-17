using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.Windsor;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;
using Purchasing.WS;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Tests.ControllerTests.OrderControllerTests
{
    public partial class OrderControllerTests
    {

        #region Edit Get Tests
        [TestMethod]
        [ExpectedException(typeof (PreconditionException))]
        public void TestEditGetThrowsExceptionIfOrderNotFound()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeOrders(3, RepositoryFactory.OrderRepository);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Edit(4);
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Precondition failed.", ex.Message);
                throw ex;
            }
        }


        [TestMethod]
        public void TestEditGetReturnsView1()
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
            new FakeWorkAddresses(3, RepositoryFactory.WorkgroupAddressRepository);
            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);
            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);
            new FakeSplits(3, RepositoryFactory.SplitRepository);
            new FakeAccounts(3, RepositoryFactory.AccountRepository);

            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);

            new FakeOrders(0, RepositoryFactory.OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1)
                .AssertViewRendered()
                .WithViewData<OrderModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, Controller.ErrorMessage);
            Assert.AreEqual(1, result.Order.Id);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditGetReturnsView2()
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
                wgAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                wgAccounts[i].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 2);
            }
            wgAccounts[1].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            new FakeWorkgroupAccounts(0, RepositoryFactory.WorkgroupAccountRepository, wgAccounts);
            new FakeWorkgroupVendors(3, RepositoryFactory.WorkgroupVendorRepository);
            new FakeWorkAddresses(3, RepositoryFactory.WorkgroupAddressRepository);
            new FakeShippingTypes(3, RepositoryFactory.ShippingTypeRepository);
            new FakeCustomFields(3, RepositoryFactory.CustomFieldRepository);

            new FakeSplits(3, RepositoryFactory.SplitRepository);
            new FakeAccounts(3, RepositoryFactory.AccountRepository);

            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);

            new FakeOrders(0, RepositoryFactory.OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1)
                .AssertViewRendered()
                .WithViewData<OrderModifyModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Accounts.Count());
            Assert.IsNotNull(result.Order);
            Assert.AreEqual(3, result.Units.Count());
            Assert.AreEqual(3, result.ShippingTypes.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView3()
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

            new FakeSplits(3, RepositoryFactory.SplitRepository);
            new FakeAccounts(3, RepositoryFactory.AccountRepository);

            var orders = new List<Order>();
            orders.Add(CreateValidEntities.Order(1));
            orders[0].Workgroup = RepositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == 2);

            new FakeOrders(0, RepositoryFactory.OrderRepository, orders);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1)
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

        #endregion Edit Get Tests

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
    }
}
