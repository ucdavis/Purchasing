using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;


namespace Purchasing.Tests.ControllerTests.HistoryControllerTests
{
    public partial class HistoryControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsAll1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            new FakeColumnPreferences(3, ColumnPreferencesRepository);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index("All", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(null, args[3]); // because we chose all
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
        public void TestIndexWhenOrderStatusFilterIsAll2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            new FakeColumnPreferences(3, ColumnPreferencesRepository);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index("All", DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(2), DateTime.Now.Date.AddDays(3), DateTime.Now.Date.AddDays(4), true, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(true, args[2]);
            Assert.AreEqual(null, args[3]); // because we chose all
            Assert.AreEqual(DateTime.Now.Date.AddDays(1), args[4]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(2), args[5]);
            Assert.AreEqual(false, args[6]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(3), args[7]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(4), args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.SelectedOrderStatus);
            Assert.AreEqual(DateTime.Now.Date.AddDays(1), result.StartDate);
            Assert.AreEqual(DateTime.Now.Date.AddDays(2), result.EndDate);
            Assert.AreEqual(DateTime.Now.Date.AddDays(3), result.StartLastActionDate);
            Assert.AreEqual(DateTime.Now.Date.AddDays(4), result.EndLastActionDate);
            Assert.AreEqual(true, result.ShowPending);
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did not exist, so created with defaults
            Assert.AreEqual(50, Controller.ViewBag.DataTablesPageSize);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsAll3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            new FakeColumnPreferences(3, ColumnPreferencesRepository);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index("All", DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(2), DateTime.Now.Date.AddDays(3), DateTime.Now.Date.AddDays(4), false, true)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(null, args[3]); // because we chose all
            Assert.AreEqual(DateTime.Now.Date.AddDays(1), args[4]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(2), args[5]);
            Assert.AreEqual(true, args[6]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(3), args[7]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(4), args[8]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.SelectedOrderStatus);
            Assert.AreEqual(DateTime.Now.Date.AddDays(1), result.StartDate);
            Assert.AreEqual(DateTime.Now.Date.AddDays(2), result.EndDate);
            Assert.AreEqual(DateTime.Now.Date.AddDays(3), result.StartLastActionDate);
            Assert.AreEqual(DateTime.Now.Date.AddDays(4), result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual(true, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did not exist, so created with defaults
            Assert.AreEqual(50, Controller.ViewBag.DataTablesPageSize);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsAll4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index("All", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(null, args[3]); // because we chose all
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsReceived1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index("Received", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("yes", args[0]);
            Assert.AreEqual(true, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[3]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsReceived2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            var orderHistories = new List<OrderHistory>();
            for (int i = 0; i < 3; i++)
            {
                orderHistories.Add(CreateValidEntities.OrderHistory(i+1));
                orderHistories[i].Received = "Yes";
            }
            orderHistories[1].Received = "No";
            new FakeOrderHistory(0, OrderHistoryRepository, orderHistories);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index("Received", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("yes", args[0]);
            Assert.AreEqual(true, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[3]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsUnReceived()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
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
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index("UnReceived", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("no", args[0]);
            Assert.AreEqual(true, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[3]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenNeedSpecialColumns1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
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
                approvals.Add(CreateValidEntities.Approval(i+1));
                approvals[i].Order = CreateValidEntities.Order(i + 1);
                approvals[i].Order.SetIdTo(i + 1);
            }

            approvals[1].Order.SetIdTo(99);
            new FakeApprovals(0, ApprovalRepository, approvals);


            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            prefs[0].ShowApprover = true;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index("UnReceived", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("no", args[0]);
            Assert.AreEqual(true, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[3]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
            Assert.AreEqual(null, args[7]);
            Assert.AreEqual(null, args[7]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, result.SelectedOrderStatus);
            Assert.AreEqual(null, result.StartDate);
            Assert.AreEqual(null, result.EndDate);
            Assert.AreEqual(null, result.StartLastActionDate);
            Assert.AreEqual(null, result.EndLastActionDate);
            Assert.AreEqual(false, result.ShowPending);
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenNeedSpecialColumns2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
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
                orderTracking.Add(CreateValidEntities.OrderTracking(i+1));
                orderTracking[i].Order = new Order();
                orderTracking[i].Order.SetIdTo(2);
            }
            orderTracking[1].Order.SetIdTo(9);            
            new FakeOrderTracking(0, OrderTrackingRepository, orderTracking);


            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            prefs[0].ShowDaysNotActedOn = true;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index("UnReceived", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("no", args[0]);
            Assert.AreEqual(true, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[3]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsApprover()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index(OrderStatusCode.Codes.Approver, null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, args[3]); 
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsAccountManager()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index(OrderStatusCode.Codes.AccountManager, null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsPurchaser()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index(OrderStatusCode.Codes.Purchaser, null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsComplete()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index(OrderStatusCode.Codes.Complete, null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual(true, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsCancelled()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index(OrderStatusCode.Codes.Cancelled, null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.Cancelled, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexWhenOrderStatusFilterIsDenied()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            var rtValue = new IndexedList<OrderHistory>();
            rtValue.LastModified = DateTime.Now.Date.AddHours(7);
            rtValue.Results = OrderHistoryRepository.Queryable.ToList();
            OrderService.Expect(
                a =>
                a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      rtValue);
            #endregion Arrange

            #region Act
            var result = Controller.Index(OrderStatusCode.Codes.Denied, null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetIndexedListofOrders(Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.IsNull(args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(false, args[2]);
            Assert.AreEqual(OrderStatusCode.Codes.Denied, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(false, args[6]);
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
            Assert.AreEqual(false, result.ShowCreated);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did exist
            Assert.AreEqual(25, Controller.ViewBag.DataTablesPageSize);
            Assert.AreEqual(3, result.OrderHistory.Count);
            #endregion Assert
        }
        #endregion Index Tests

    }
}
