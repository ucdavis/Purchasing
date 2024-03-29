﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Purchasing.Tests.ControllerTests.HistoryControllerTests
{
    public partial class HistoryControllerTests
    {
        #region AdminOrders Tests

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsAll1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            new FakeColumnPreferences(3, ColumnPreferencesRepository);

            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();

            object[] args = default;
            Mock.Get(OrderService).Setup(
                a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("All", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(
                a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(null, args[4]); // because we chose all
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did not exist, so created with defaults
            Assert.AreEqual(50, Controller.ViewBag.DataTablesPageSize);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsAll2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            new FakeColumnPreferences(3, ColumnPreferencesRepository);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();

            object[] args = default;
            Mock.Get(OrderService).Setup(
                a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("All", DateTime.UtcNow.ToPacificTime().Date.AddDays(1), DateTime.UtcNow.ToPacificTime().Date.AddDays(2), DateTime.UtcNow.ToPacificTime().Date.AddDays(3), DateTime.UtcNow.ToPacificTime().Date.AddDays(4), true)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(true, args[3]);
            Assert.AreEqual(null, args[4]); // because we chose all
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(1), args[5]);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(2), args[6]);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(3), args[7]);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(4), args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.SelectedOrderStatus);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(1), result.StartDate);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(2), result.EndDate);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(3), result.StartLastActionDate);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(4), result.EndLastActionDate);
            Assert.AreEqual(true, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did not exist, so created with defaults
            Assert.AreEqual(50, Controller.ViewBag.DataTablesPageSize);
            #endregion Assert
        }


        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsAll3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();

            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("All", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(null, args[4]); // because we chose all
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsReceived1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("Received", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("yes", args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsPaid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("Paid", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual("yes", args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }
        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsReceived2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderHistories = new List<OrderHistory>();
            for (int i = 0; i < 3; i++)
            {
                orderHistories.Add(CreateValidEntities.OrderHistory(i + 1));
                orderHistories[i].Received = "Yes";
            }
            orderHistories[1].Received = "No";
            new FakeOrderHistory(0, OrderHistoryRepository, orderHistories);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("Received", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("yes", args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsPaid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderHistories = new List<OrderHistory>();
            for (int i = 0; i < 3; i++)
            {
                orderHistories.Add(CreateValidEntities.OrderHistory(i + 1));
                orderHistories[i].Received = "Yes";
            }
            orderHistories[1].Received = "No";
            new FakeOrderHistory(0, OrderHistoryRepository, orderHistories);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("Paid", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual("yes", args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsUnReceived()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderHistories = new List<OrderHistory>();
            for (int i = 0; i < 3; i++)
            {
                orderHistories.Add(CreateValidEntities.OrderHistory(i + 1));
                orderHistories[i].Received = "Yes";
            }
            orderHistories[1].Received = "No";
            new FakeOrderHistory(0, OrderHistoryRepository, orderHistories);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("UnReceived", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("no", args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsUnPaid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderHistories = new List<OrderHistory>();
            for (int i = 0; i < 3; i++)
            {
                orderHistories.Add(CreateValidEntities.OrderHistory(i + 1));
                orderHistories[i].Received = "No";
                orderHistories[i].Paid = "Yes";
            }
            orderHistories[1].Paid = "No";
            new FakeOrderHistory(0, OrderHistoryRepository, orderHistories);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("UnPaid", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual("no", args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }


        [TestMethod]
        public void TestAdminOrdersWhenNeedSpecialColumns1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderHistories = new List<OrderHistory>();
            for (int i = 0; i < 3; i++)
            {
                orderHistories.Add(CreateValidEntities.OrderHistory(i + 1));
                orderHistories[i].Received = "No";
                orderHistories[i].OrderId = i + 1;
            }
            //orderHistories[1].Received = "No";
            new FakeOrderHistory(0, OrderHistoryRepository, orderHistories);

            var approvals = new List<Approval>();
            for (int i = 0; i < 3; i++)
            {
                approvals.Add(CreateValidEntities.Approval(i + 1));
                approvals[i].Order = CreateValidEntities.Order(i + 1);
                approvals[i].Order.Id = i + 1;
            }

            approvals[1].Order.Id = 99;
            new FakeApprovals(0, ApprovalRepository, approvals);


            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            prefs[0].ShowApprover = true;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);

            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();

            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("UnReceived", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("no", args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenNeedSpecialColumns1A() //Paid filter
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderHistories = new List<OrderHistory>();
            for (int i = 0; i < 3; i++)
            {
                orderHistories.Add(CreateValidEntities.OrderHistory(i + 1));
                orderHistories[i].Received = "Yes";
                orderHistories[i].Paid = "No";
                orderHistories[i].OrderId = i + 1;
            }
            //orderHistories[1].Received = "No";
            new FakeOrderHistory(0, OrderHistoryRepository, orderHistories);

            var approvals = new List<Approval>();
            for (int i = 0; i < 3; i++)
            {
                approvals.Add(CreateValidEntities.Approval(i + 1));
                approvals[i].Order = CreateValidEntities.Order(i + 1);
                approvals[i].Order.Id = i + 1;
            }

            approvals[1].Order.Id = 99;
            new FakeApprovals(0, ApprovalRepository, approvals);


            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            prefs[0].ShowApprover = true;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);

            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();

            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("UnPaid", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual("no", args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenNeedSpecialColumns2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderHistories = new List<OrderHistory>();
            for (int i = 0; i < 3; i++)
            {
                orderHistories.Add(CreateValidEntities.OrderHistory(i + 1));
                orderHistories[i].Received = "No";
                orderHistories[i].OrderId = i + 1;
            }
            //orderHistories[1].Received = "No";
            new FakeOrderHistory(0, OrderHistoryRepository, orderHistories);

            var orderTracking = new List<OrderTracking>();
            for (int i = 0; i < 3; i++)
            {
                orderTracking.Add(CreateValidEntities.OrderTracking(i + 1));
                orderTracking[i].Order = new Order();
                orderTracking[i].Order.Id = 2;
            }
            orderTracking[1].Order.Id = 9;
            new FakeOrderTracking(0, OrderTrackingRepository, orderTracking);


            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            prefs[0].ShowDaysNotActedOn = true;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);

            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();

            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("UnReceived", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("no", args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenNeedSpecialColumns2A()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            var orderHistories = new List<OrderHistory>();
            for (int i = 0; i < 3; i++)
            {
                orderHistories.Add(CreateValidEntities.OrderHistory(i + 1));
                orderHistories[i].Received = "Yes";
                orderHistories[i].Paid = "No";
                orderHistories[i].OrderId = i + 1;
            }
            //orderHistories[1].Received = "No";
            new FakeOrderHistory(0, OrderHistoryRepository, orderHistories);

            var orderTracking = new List<OrderTracking>();
            for (int i = 0; i < 3; i++)
            {
                orderTracking.Add(CreateValidEntities.OrderTracking(i + 1));
                orderTracking[i].Order = new Order();
                orderTracking[i].Order.Id = 2;
            }
            orderTracking[1].Order.Id = 9;
            new FakeOrderTracking(0, OrderTrackingRepository, orderTracking);


            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            prefs[0].ShowDaysNotActedOn = true;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);

            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();

            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("UnPaid", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual("no", args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsApprover()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.Approver, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsAccountManager()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);

            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();

            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.AccountManager, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsPurchaser()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.Purchaser, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsComplete()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.Complete, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsCancelled()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.Cancelled, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Cancelled, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Cancelled, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsDenied()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Setup(new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].Id = "Me";
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.UtcNow.ToPacificTime().Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            object[] args = default;
            Mock.Get(OrderService).Setup(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, bool c, bool d, string e, DateTime? f, DateTime? g,
                          DateTime? h, DateTime? i) => args = new object[] { a, b, c, d, e, f, g, h, i });
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.Denied, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAdministrativeIndexedListofOrders(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                                  It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(false, args[3]);
            Assert.AreEqual(OrderStatusCode.Codes.Denied, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Denied, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }
        #endregion AdminOrders Tests


    }
}
