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
        #region AccountsPayableOrders Tests

        [TestMethod]
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsAll1()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("All", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(
                a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.IsNull(args[2]);
            Assert.IsNull(args[3]);
            Assert.IsNull(args[4]);
            Assert.IsNull(args[5]);
            Assert.IsNull(args[6]);
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
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsAll2()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("All", DateTime.UtcNow.ToPacificTime().Date.AddDays(1), DateTime.UtcNow.ToPacificTime().Date.AddDays(2), DateTime.UtcNow.ToPacificTime().Date.AddDays(3), DateTime.UtcNow.ToPacificTime().Date.AddDays(4))
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.IsNull(args[2]);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(1), args[3]);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(2), args[4]);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(3), args[5]);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(4), args[6]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.SelectedOrderStatus);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(1), result.StartDate);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(2), result.EndDate);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(3), result.StartLastActionDate);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date.AddDays(4), result.EndLastActionDate);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did not exist, so created with defaults
            Assert.AreEqual(50, Controller.ViewBag.DataTablesPageSize);
            #endregion Assert
        }


        [TestMethod]
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsAll3()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("All", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.IsNull(args[2]);
            Assert.IsNull(args[3]);
            Assert.IsNull(args[4]);
            Assert.IsNull(args[5]);
            Assert.IsNull(args[6]);

            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsReceived1()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("Received", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("yes", args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]); 
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsPaid1()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("Paid", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual("yes", args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsReceived2()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("Received", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("yes", args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsPaid2()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("Paid", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual("yes", args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsUnReceived()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("UnReceived", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("no", args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsUnPaid()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("UnPaid", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual("no", args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }


        [TestMethod]
        public void TestAccountsPayableOrdersWhenNeedSpecialColumns1()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("UnReceived", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("no", args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenNeedSpecialColumns1A() //Paid filter
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("UnPaid", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual("no", args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenNeedSpecialColumns2()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("UnReceived", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual("no", args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenNeedSpecialColumns2A()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders("UnPaid", null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual("no", args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsApprover()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders(OrderStatusCode.Codes.Approver, null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, args[2]);
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsAccountManager()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders(OrderStatusCode.Codes.AccountManager, null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, args[2]);
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsPurchaser()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders(OrderStatusCode.Codes.Purchaser, null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, args[2]);
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsComplete()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders(OrderStatusCode.Codes.Complete, null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]);
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsCancelled()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders(OrderStatusCode.Codes.Cancelled, null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Cancelled, args[2]);
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
        public void TestAccountsPayableOrdersWhenOrderStatusFilterIsDenied()
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
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(
                                      rtValue)
                .Callback((string a, string b, string c, DateTime? d, DateTime? e, DateTime? f, DateTime? g) => args = new object[] { a, b, c, d, e, f, g });
            #endregion Arrange

            #region Act
            var result = Controller.AccountsPayableOrders(OrderStatusCode.Codes.Denied, null, null, null, null)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            Mock.Get(OrderService).Verify(a =>
                a.GetAccountsPayableIndexedListofOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));

            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Denied, args[2]);
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
