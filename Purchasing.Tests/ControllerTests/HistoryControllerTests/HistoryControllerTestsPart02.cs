using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Web.Models;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;


namespace Purchasing.Tests.ControllerTests.HistoryControllerTests
{
    public partial class HistoryControllerTests
    {
        #region AdminOrders Tests

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsAll1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            new FakeColumnPreferences(3, ColumnPreferencesRepository);
            OrderService.Expect(
                a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("All", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(
                a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(null, args[2]); // because we chose all
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            new FakeColumnPreferences(3, ColumnPreferencesRepository);
            OrderService.Expect(
                a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("All", DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(2), DateTime.Now.Date.AddDays(3), DateTime.Now.Date.AddDays(4), true)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(true, args[1]);
            Assert.AreEqual(null, args[2]); // because we chose all
            Assert.AreEqual(DateTime.Now.Date.AddDays(1), args[3]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(2), args[4]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(3), args[5]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(4), args[6]);
            #endregion GetListOfOrder Args

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.SelectedOrderStatus);
            Assert.AreEqual(DateTime.Now.Date.AddDays(1), result.StartDate);
            Assert.AreEqual(DateTime.Now.Date.AddDays(2), result.EndDate);
            Assert.AreEqual(DateTime.Now.Date.AddDays(3), result.StartLastActionDate);
            Assert.AreEqual(DateTime.Now.Date.AddDays(4), result.EndLastActionDate);
            Assert.AreEqual(true, result.ShowPending);
            Assert.AreEqual("Me", result.ColumnPreferences.Id); // Did not exist, so created with defaults
            Assert.AreEqual(50, Controller.ViewBag.DataTablesPageSize);
            #endregion Assert
        }


        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsAll3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("All", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(null, args[2]); // because we chose all
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(null, args[5]);
            Assert.AreEqual(null, args[6]);
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("Received", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(true, args[0]);
            Assert.AreEqual(false, args[1]);
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
            Assert.AreEqual(0, result.OrderHistory.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsReceived2()
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
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("Received", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(true, args[0]);
            Assert.AreEqual(false, args[1]);
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
            Assert.IsNull(result.Approvals);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsUnReceived()
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
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("UnReceived", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(true, args[0]);
            Assert.AreEqual(false, args[1]);
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
            Assert.AreEqual(1, result.OrderHistory.Count);
            Assert.IsNull(result.Approvals);
            #endregion Assert
        }


        [TestMethod]
        public void TestAdminOrdersWhenNeedSpecialColumns1()
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
                approvals.Add(CreateValidEntities.Approval(i + 1));
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
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("UnReceived", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(true, args[0]);
            Assert.AreEqual(false, args[1]);
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
            Assert.AreEqual(2, result.Approvals.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenNeedSpecialColumns2()
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
                orderTracking.Add(CreateValidEntities.OrderTracking(i + 1));
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
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders("UnReceived", null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(true, args[0]);
            Assert.AreEqual(false, args[1]);
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
            Assert.AreEqual(2, result.OrderTracking.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminOrdersWhenOrderStatusFilterIsApprover()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.Approver, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(false, args[1]);
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
        public void TestAdminOrdersWhenOrderStatusFilterIsAccountManager()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.AccountManager, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(false, args[1]);
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
        public void TestAdminOrdersWhenOrderStatusFilterIsPurchaser()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.Purchaser, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(false, args[1]);
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
        public void TestAdminOrdersWhenOrderStatusFilterIsComplete()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.Complete, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(true, args[0]);
            Assert.AreEqual(false, args[1]);
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
        public void TestAdminOrdersWhenOrderStatusFilterIsCancelled()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.Cancelled, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(false, args[1]);
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
        public void TestAdminOrdersWhenOrderStatusFilterIsDenied()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            new FakeOrderHistory(3, OrderHistoryRepository);
            var prefs = new List<ColumnPreferences>();
            prefs.Add(CreateValidEntities.ColumnPreferences(1));
            prefs[0].SetIdTo("Me");
            prefs[0].DisplayRows = 25;
            new FakeColumnPreferences(0, ColumnPreferencesRepository, prefs, true);
            OrderService.Expect(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.AdminOrders(OrderStatusCode.Codes.Denied, null, null, null, null, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(a =>
                a.GetAdministrativeListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(false, args[1]);
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
