using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.ServiceTests.OrderServiceTests
{
    public partial class OrderServiceTests
    {
        /// <summary>
        /// Account manager is passed
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder23()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.Id = 99;
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = null;
            order.Splits[0].Order = order;
            order.Workgroup.Id = 1;
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            new FakeAccounts(3, AccountRepository);

            new FakeUsers(3, UserRepository);
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, null, "2");
            #endregion Act

            #region Assert
            Moq.Mock.Get(SecurityService).Verify(a => a.GetUser(Moq.It.IsAny<string>()), Moq.Times.Never()); // the account was not found in the workgroup or the account table
            Moq.Mock.Get(EventService).Verify(a => a.OrderApprovalAdded(Moq.It.IsAny<Order>(), Moq.It.IsAny<Approval>(), Moq.It.IsAny<bool>()), Moq.Times.Exactly(3));
            Moq.Mock.Get(EventService).Verify(a => a.OrderAutoApprovalAdded(Moq.It.IsAny<Order>(), Moq.It.IsAny<Approval>()), Moq.Times.Never());
            Moq.Mock.Get(EventService).Verify(a => a.OrderCreated(order));

            Assert.AreEqual(3, order.Approvals.Count);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, order.Approvals[0].StatusCode.Id);
            Assert.IsNull(order.Approvals[0].User);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[1].StatusCode.Id);
            Assert.AreEqual("LastName2", order.Approvals[1].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[2].StatusCode.Id);
            #endregion Assert
        }

        /// <summary>
        /// Account manager and approver are passed
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder24()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.Id = 99;
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = null;
            order.Splits[0].Order = order;
            order.Workgroup.Id = 1;
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            new FakeAccounts(3, AccountRepository);

            new FakeUsers(3, UserRepository);
            Moq.Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("22"); //NOT same as approver

            new FakeAutoApprovals(3, AutoAprovalRepository);
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, "1", "2");
            #endregion Act

            #region Assert
            Moq.Mock.Get(SecurityService).Verify(a => a.GetUser(Moq.It.IsAny<string>()), Moq.Times.Never()); // the account was not found in the workgroup or the account table
            Moq.Mock.Get(EventService).Verify(a => a.OrderApprovalAdded(Moq.It.IsAny<Order>(), Moq.It.IsAny<Approval>(), Moq.It.IsAny<bool>()), Moq.Times.Exactly(3));
            Moq.Mock.Get(EventService).Verify(a => a.OrderAutoApprovalAdded(Moq.It.IsAny<Order>(), Moq.It.IsAny<Approval>()), Moq.Times.Never());
            Moq.Mock.Get(EventService).Verify(a => a.OrderCreated(order));

            Assert.AreEqual(3, order.Approvals.Count);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual("LastName1", order.Approvals[0].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[1].StatusCode.Id);
            Assert.AreEqual("LastName2", order.Approvals[1].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[2].StatusCode.Id);
            #endregion Assert
        }

        /// <summary>
        /// Account manager and approver are passed, approver is current user
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder25()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.CreatedBy = CreateValidEntities.User(1);
            order.Id = 99;
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = null;
            order.Splits[0].Order = order;
            order.Workgroup.Id = 1;
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            new FakeAccounts(3, AccountRepository);

            new FakeUsers(3, UserRepository);
            Moq.Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("1");

            new FakeAutoApprovals(3, AutoAprovalRepository);
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, "1", "2");
            #endregion Act

            #region Assert
            Moq.Mock.Get(SecurityService).Verify(a => a.GetUser(Moq.It.IsAny<string>()), Moq.Times.Never()); // the account was not found in the workgroup or the account table
            Moq.Mock.Get(EventService).Verify(a => a.OrderApprovalAdded(Moq.It.IsAny<Order>(), Moq.It.IsAny<Approval>(), Moq.It.IsAny<bool>()), Moq.Times.Exactly(2));
            Moq.Mock.Get(EventService).Verify(a => a.OrderAutoApprovalAdded(Moq.It.IsAny<Order>(), Moq.It.IsAny<Approval>()));
            Moq.Mock.Get(EventService).Verify(a => a.OrderCreated(order));

            Assert.AreEqual(3, order.Approvals.Count);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual("LastName1", order.Approvals[0].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[1].StatusCode.Id);
            Assert.AreEqual("LastName2", order.Approvals[1].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[2].StatusCode.Id);
            #endregion Assert
        }

        /// <summary>
        /// Account manager and approver are passed, approver is not current user, but there is an auto approval for current user
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder26()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.CreatedBy = CreateValidEntities.User(109);
            order.Id = 99;
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = null;
            order.Splits[0].Order = order;
            order.Workgroup.Id = 1;
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            new FakeAccounts(3, AccountRepository);

            new FakeUsers(3, UserRepository);
            Moq.Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("3");

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(1);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].Expiration = DateTime.UtcNow.ToPacificTime().AddDays(1);
            autoApprovals[0].TargetUser = order.CreatedBy;
            autoApprovals[0].Account = null;
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, "1", "2");
            #endregion Act

            #region Assert
            Moq.Mock.Get(EventService).Verify(a => a.OrderApprovalAdded(Moq.It.IsAny<Order>(), Moq.It.IsAny<Approval>(), Moq.It.IsAny<bool>()), Moq.Times.Exactly(2));
            Moq.Mock.Get(EventService).Verify(a => a.OrderAutoApprovalAdded(Moq.It.IsAny<Order>(), Moq.It.IsAny<Approval>()));
//TODO: Arrange
            Approval args = default;
            Moq.Mock.Get(EventService).Setup(a => a.OrderAutoApprovalAdded(Moq.It.IsAny<Order>(), Moq.It.IsAny<Approval>()))
                .Callback<Order, Approval>((_, x) => args = x);
//ENDTODO
            Assert.AreEqual(OrderStatusCode.Codes.Approver, args.StatusCode.Id);

            Assert.AreEqual(3, order.Approvals.Count);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual("LastName1", order.Approvals[0].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[1].StatusCode.Id);
            Assert.AreEqual("LastName2", order.Approvals[1].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[2].StatusCode.Id);
            #endregion Assert
        }



    }
}
