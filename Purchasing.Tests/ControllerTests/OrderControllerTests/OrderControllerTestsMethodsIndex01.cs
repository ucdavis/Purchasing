using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Core.Domain;
using Purchasing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;
using Purchasing.WS;

namespace Purchasing.Tests.ControllerTests.OrderControllerTests
{
    public partial class OrderControllerTests
    {

        //[TestMethod]
        //public void TestIndex1()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
        //    SetupDataForTests1();
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Index(null, null, null, false, false, null)
        //        .AssertViewRendered()
        //        .WithViewData<FilteredOrderListModelDto>();
        //    #endregion Act

        //    #region Assert
        //    OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything));
        //    var args = OrderService.GetArgumentsForCallsMadeOn(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything))[0]; 
        //    Assert.AreEqual(false,args[0]);
        //    Assert.AreEqual(false, args[1]);
        //    Assert.AreEqual(null, args[2]);
        //    Assert.AreEqual(null, args[3]);
        //    Assert.AreEqual(null, args[4]);
        //    Assert.AreEqual(false, args[5]);
        //    Assert.IsNotNull(result);
        //    #endregion Assert		
        //}

        //[TestMethod]
        //public void TestIndex2()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
        //    SetupDataForTests1();
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Index("XX", null, null, false, false, null)
        //        .AssertViewRendered()
        //        .WithViewData<FilteredOrderListModelDto>();
        //    #endregion Act

        //    #region Assert
        //    OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything));
        //    var args = OrderService.GetArgumentsForCallsMadeOn(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything))[0];
        //    Assert.AreEqual(false, args[0]);
        //    Assert.AreEqual(false, args[1]);
        //    Assert.AreEqual("XX", args[2]);
        //    Assert.AreEqual(null, args[3]);
        //    Assert.AreEqual(null, args[4]);
        //    Assert.IsNotNull(result);
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestIndex3()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
        //    SetupDataForTests1();
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Index("XX", new DateTime(2012, 01, 02), new DateTime(2012, 02, 03), true, false, null)
        //        .AssertViewRendered()
        //        .WithViewData<FilteredOrderListModelDto>();
        //    #endregion Act

        //    #region Assert
        //    OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything));
        //    var args = OrderService.GetArgumentsForCallsMadeOn(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything))[0];
        //    Assert.AreEqual(false, args[0]);
        //    Assert.AreEqual(true, args[1]);
        //    Assert.AreEqual("XX", args[2]);
        //    Assert.AreEqual(new DateTime(2012, 01, 02), args[3]);
        //    Assert.AreEqual(new DateTime(2012, 02, 03), args[4]);
        //    Assert.IsNotNull(result);
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestIndex4()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
        //    SetupDataForTests1();
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Index(OrderStatusCode.Codes.Complete, new DateTime(2012, 01, 02), new DateTime(2012, 02, 03), true, false, null)
        //        .AssertViewRendered()
        //        .WithViewData<FilteredOrderListModelDto>();
        //    #endregion Act

        //    #region Assert
        //    OrderService.AssertWasCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything));
        //    var args = OrderService.GetArgumentsForCallsMadeOn(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything))[0];
        //    Assert.AreEqual(true, args[0]);
        //    Assert.AreEqual(true, args[1]);
        //    Assert.AreEqual("CP", args[2]);
        //    Assert.AreEqual(new DateTime(2012, 01, 02), args[3]);
        //    Assert.AreEqual(new DateTime(2012, 02, 03), args[4]);
        //    Assert.IsNotNull(result);
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestIndex5()
        //{
        //    #region Arrange
        //    IRepository<CompletedOrdersThisWeek> completedOrdersThisWeekRepository = new Repository<CompletedOrdersThisWeek>();
        //    Controller.Repository.Expect(a => a.OfType<CompletedOrdersThisWeek>()).Return(completedOrdersThisWeekRepository);

        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
        //    SetupDataForTests1();
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Index(OrderStatusCode.Codes.Complete, new DateTime(2012, 01, 02), new DateTime(2012, 02, 03), true, false, "notMonth")
        //        .AssertViewRendered()
        //        .WithViewData<FilteredOrderListModelDto>();
        //    #endregion Act

        //    #region Assert
        //    OrderService.AssertWasNotCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything));
        //    Assert.IsNotNull(result);
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestIndex6()
        //{
        //    #region Arrange
        //    IRepository<CompletedOrdersThisMonth> completedOrdersThisMonthRepository = new Repository<CompletedOrdersThisMonth>();
        //    Controller.Repository.Expect(a => a.OfType<CompletedOrdersThisMonth>()).Return(completedOrdersThisMonthRepository);

        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
        //    SetupDataForTests1();
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Index(OrderStatusCode.Codes.Complete, new DateTime(2012, 01, 02), new DateTime(2012, 02, 03), true, false, "month")
        //        .AssertViewRendered()
        //        .WithViewData<FilteredOrderListModelDto>();
        //    #endregion Act

        //    #region Assert
        //    OrderService.AssertWasNotCalled(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything));
        //    Assert.IsNotNull(result);
        //    #endregion Assert
        //}

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
