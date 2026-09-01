using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Models.AggieEnterprise;
using Purchasing.Tests.Core;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Moq;

namespace Purchasing.Tests.ServiceTests.OrderServiceTests
{
    public partial class OrderServiceTests
    {
        #region EditExistingOrder Tests

        [TestMethod]
        public void TestEditExistingOrder()
        {
            #region Arrange
            var order = CreateValidEntities.Order(7);
            var order2 = CreateValidEntities.Order(8);
            #endregion Arrange

            #region Act
            OrderService.EditExistingOrder(order);
            #endregion Act

            #region Assert
            Mock.Get(EventService).Verify(a => a.OrderEdited(order));
            Mock.Get(EventService).Verify(a => a.OrderEdited(order2), Times.Never());
            #endregion Assert		
        }

        #endregion EditExistingOrder Tests

        #region ReRouteSingleApprovalForExistingOrder Tests

        [TestMethod]
        public void TestReRouteSingleApprovalForExistingOrder()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion ReRouteSingleApprovalForExistingOrder Tests

        #region TryPopulatePoNumberFromAggieEnterprise Tests

        [TestMethod]
        public async System.Threading.Tasks.Task TestTryPopulatePoNumberFromAggieEnterprisePopulatesPoAndAddsHistory()
        {
            var order = CreateValidEntities.Order(1);
            order.StatusCode.Id = OrderStatusCode.Codes.Complete;
            order.OrderType = new OrderType(OrderType.Types.AggieEnterprise);
            order.ReferenceNumber = " 184e9d20-f759-413e-8f42-06db62bf1e59 ";
            order.PoNumber = " ";

            Mock.Get(AggieEnterpriseService)
                .Setup(a => a.LookupOrderStatus("184e9d20-f759-413e-8f42-06db62bf1e59"))
                .ReturnsAsync(new AeResultStatus { PoNumber = " PO123456 " });

            var result = await OrderService.TryPopulatePoNumberFromAggieEnterprise(order);

            Assert.IsTrue(result);
            Assert.AreEqual("PO123456", order.PoNumber);
            Mock.Get(EventService).Verify(a => a.OrderUpdated(
                order,
                "PO # automatically populated from Aggie Enterprise: PO123456"));
            Mock.Get(OrderRepository).Verify(a => a.EnsurePersistent(order));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestTryPopulatePoNumberFromAggieEnterpriseDoesNothingWhenLookupHasNoPo()
        {
            var order = CreateValidEntities.Order(1);
            order.StatusCode.Id = OrderStatusCode.Codes.Complete;
            order.OrderType = new OrderType(OrderType.Types.AggieEnterprise);
            order.ReferenceNumber = "184e9d20-f759-413e-8f42-06db62bf1e59";
            order.PoNumber = null;

            Mock.Get(AggieEnterpriseService)
                .Setup(a => a.LookupOrderStatus(order.ReferenceNumber))
                .ReturnsAsync(new AeResultStatus { PoNumber = " " });

            var result = await OrderService.TryPopulatePoNumberFromAggieEnterprise(order);

            Assert.IsFalse(result);
            Assert.IsNull(order.PoNumber);
            Mock.Get(EventService).Verify(
                a => a.OrderUpdated(It.IsAny<Order>(), It.IsAny<string>()),
                Times.Never());
            Mock.Get(OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()), Times.Never());
        }

        [DataTestMethod]
        [DataRow(OrderStatusCode.Codes.Purchaser, OrderType.Types.AggieEnterprise, "184e9d20-f759-413e-8f42-06db62bf1e59", null)]
        [DataRow(OrderStatusCode.Codes.Complete, OrderType.Types.KfsDocument, "184e9d20-f759-413e-8f42-06db62bf1e59", null)]
        [DataRow(OrderStatusCode.Codes.Complete, OrderType.Types.AggieEnterprise, " ", null)]
        [DataRow(OrderStatusCode.Codes.Complete, OrderType.Types.AggieEnterprise, "184e9d20-f759-413e-8f42-06db62bf1e59", "PO123456")]
        public async System.Threading.Tasks.Task TestTryPopulatePoNumberFromAggieEnterpriseOnlyLooksUpEligibleOrders(
            string statusCode,
            string orderType,
            string referenceNumber,
            string poNumber)
        {
            var order = CreateValidEntities.Order(1);
            order.StatusCode.Id = statusCode;
            order.OrderType = new OrderType(orderType);
            order.ReferenceNumber = referenceNumber;
            order.PoNumber = poNumber;

            var result = await OrderService.TryPopulatePoNumberFromAggieEnterprise(order);

            Assert.IsFalse(result);
            Mock.Get(AggieEnterpriseService).Verify(
                a => a.LookupOrderStatus(It.IsAny<string>()),
                Times.Never());
            Mock.Get(EventService).Verify(
                a => a.OrderUpdated(It.IsAny<Order>(), It.IsAny<string>()),
                Times.Never());
            Mock.Get(OrderRepository).Verify(a => a.EnsurePersistent(It.IsAny<Order>()), Times.Never());
        }

        #endregion TryPopulatePoNumberFromAggieEnterprise Tests
    }
}
