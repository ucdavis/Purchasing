using System;
using System.Linq;
using Castle.Windsor;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Web.Models;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;
using MvcContrib.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Purchasing.Core;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


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
            OrderService.Expect(
                a =>
                a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.Index("All", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a =>a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(null, args[2]); // because we chose all
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(false, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
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
            OrderService.Expect(
                a =>
                a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.Index("All", DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(2), DateTime.Now.Date.AddDays(3), DateTime.Now.Date.AddDays(4), true, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(true, args[1]);
            Assert.AreEqual(null, args[2]); // because we chose all
            Assert.AreEqual(DateTime.Now.Date.AddDays(1), args[3]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(2), args[4]);
            Assert.AreEqual(false, args[5]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(3), args[6]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(4), args[7]);
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
            OrderService.Expect(
                a =>
                a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.Index("All", DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(2), DateTime.Now.Date.AddDays(3), DateTime.Now.Date.AddDays(4), false, true)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(null, args[2]); // because we chose all
            Assert.AreEqual(DateTime.Now.Date.AddDays(1), args[3]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(2), args[4]);
            Assert.AreEqual(true, args[5]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(3), args[6]);
            Assert.AreEqual(DateTime.Now.Date.AddDays(4), args[7]);
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
            OrderService.Expect(
                a =>
                a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.Index("All", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(false, args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(null, args[2]); // because we chose all
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(false, args[5]);
            Assert.AreEqual(null, args[6]);
            Assert.AreEqual(null, args[7]);
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
            OrderService.Expect(
                a =>
                a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.Index("Received", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(true, args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(false, args[5]);
            Assert.AreEqual(null, args[6]);
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
            Assert.AreEqual(0, result.OrderHistory.Count);
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
            OrderService.Expect(
                a =>
                a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.Index("Received", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(true, args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(false, args[5]);
            Assert.AreEqual(null, args[6]);
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
            Assert.IsNull(result.Approvals);
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
            OrderService.Expect(
                a =>
                a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.Index("UnReceived", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(true, args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(false, args[5]);
            Assert.AreEqual(null, args[6]);
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
            Assert.AreEqual(1, result.OrderHistory.Count);
            Assert.IsNull(result.Approvals);
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
            OrderService.Expect(
                a =>
                a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<bool>.Is.Anything,
                                  Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(
                                      OrderHistoryRepository.Queryable);
            #endregion Arrange

            #region Act
            var result = Controller.Index("UnReceived", null, null, null, null, false, false)
                .AssertViewRendered()
                .WithViewData<FilteredOrderListModelDto>();
            #endregion Act

            #region Assert

            #region GetListOfOrder Args
            OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                                           Arg<string>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                                           Arg<bool>.Is.Anything,
                                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = OrderService.GetArgumentsForCallsMadeOn(
                    a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything,
                                           Arg<string>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything,
                                           Arg<bool>.Is.Anything,
                                           Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(true, args[0]);
            Assert.AreEqual(false, args[1]);
            Assert.AreEqual(OrderStatusCode.Codes.Complete, args[2]); // because we chose Receive/unReceive
            Assert.AreEqual(null, args[3]);
            Assert.AreEqual(null, args[4]);
            Assert.AreEqual(false, args[5]);
            Assert.AreEqual(null, args[6]);
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
            Assert.AreEqual(2, result.Approvals.Count());
            #endregion Assert
        }
        #endregion Index Tests


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
