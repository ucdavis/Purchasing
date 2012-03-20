using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.WS;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using Purchasing.Tests;
using UCDArch.Core.Utils;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests.OrderServiceTests
{
    public partial class OrderServiceTests
    {

        [TestMethod]
        public void TestReRouteApprovalsForExistingOrderWhenCurrectLevelIsApprove01()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.CreatedBy = CreateValidEntities.User(109);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(CreateValidEntities.Split(1));
            order.Splits[0].Order = order;
            order.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);

            order.Approvals = new List<Approval>();
            order.Approvals.Add(CreateValidEntities.Approval(1));
            order.Approvals.Add(CreateValidEntities.Approval(2));
            order.Approvals.Add(CreateValidEntities.Approval(3));
            order.Approvals[0].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            order.Approvals[1].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            order.Approvals[2].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);

            new FakeUsers(4, UserRepository);

            #endregion Arrange

            #region Act
            OrderService.ReRouteApprovalsForExistingOrder(order, string.Empty, "2");
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(3));
            EventService.AssertWasCalled(a => a.OrderReRouted(order));
            Assert.AreEqual(3, order.Approvals.Count);
            Assert.IsNull(order.Approvals[0].User);
            Assert.AreEqual("LastName2", order.Approvals[1].User.LastName);
            Assert.IsNull(order.Approvals[2].User);
            #endregion Assert		
        }


        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Continue these tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
    }
}
