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
        /// <summary>
        /// Account Manager passed, not account, no approver
        /// 
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder14()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.CreatedBy = CreateValidEntities.User(109);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            //order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Workgroup.SetIdTo(1);

            new FakeUsers(3, UserRepository);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, null, "2");
            #endregion Act

            #region Assert
            SecurityService.AssertWasNotCalled(a => a.GetUser(Arg<string>.Is.Anything)); // the account was not found in the workgroup or the account table
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(3));
            EventService.AssertWasNotCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            //var args = (Approval)EventService.GetArgumentsForCallsMadeOn(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything))[0][1];
            //Assert.AreEqual(OrderStatusCode.Codes.Approver, args.StatusCode.Id);

            EventService.AssertWasCalled(a => a.OrderCreated(order));

            Assert.AreEqual(3, order.Approvals.Count);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[1].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[2].StatusCode.Id);
            Assert.IsNull(order.Approvals[0].User);
            Assert.IsTrue(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.IsNull(order.Approvals[2].User);
            Assert.AreEqual(string.Empty, order.Splits[0].Account);
            #endregion Assert		
        }
    }
}
