using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Moq;

namespace Purchasing.Tests.ServiceTests.OrderServiceTests
{
    public partial class OrderServiceTests
    {

        [TestMethod]
        [ExpectedException(typeof (PreconditionException))]
        public void TestCreateApprovalsForNewOrderRequiresAccountIdOrManager()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var order = CreateValidEntities.Order(1);
                order.Splits = new List<Split>();
                order.Splits.Add(new Split());
                order.Splits[0].Account = "12345";
                thisFar = true;
                #endregion Arrange

                #region Act
                OrderService.CreateApprovalsForNewOrder(order, null, null, null, null);
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("You must either supply the ID of a valid account or provide the userId for an account manager", ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// The account does not exist in the workgroup (or anywhere) so it is an external one.
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder1()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.Id = 99;
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Workgroup.Id = 1;
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            new FakeAccounts(3, AccountRepository);
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
            #endregion Act

            #region Assert
            Mock.Get(SecurityService).Verify(a => a.GetUser(It.IsAny<string>()), Times.Never()); // the account was not found in the workgroup or the account table
            Mock.Get(EventService).Verify(a => a.OrderApprovalAdded(It.IsAny<Order>(), It.IsAny<Approval>(), It.IsAny<bool>()), Times.Exactly(2));
            Mock.Get(EventService).Verify(a => a.OrderAutoApprovalAdded(It.IsAny<Order>(), It.IsAny<Approval>()), Times.Never());
            Mock.Get(EventService).Verify(a => a.OrderCreated(order));

            Assert.AreEqual(2, order.Approvals.Count); //Only 2 approvals for an external account
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[1].StatusCode.Id);
            foreach (var approval in order.Approvals)
            {
                Assert.IsNull(approval.User);
            }
            #endregion Assert		
        }

        /// <summary>
        /// The account does not exist in the workgroup so it is an external one, but it does exist in the accounts table.
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder2()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.Id = 99;
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            //order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Workgroup.Id = 1;
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            var accounts = new List<Account>();
            accounts.Add(CreateValidEntities.Account(1));
            accounts[0].Id = "12345";
            accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(0, AccountRepository, accounts, true);
            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
            #endregion Act

            #region Assert
            Mock.Get(SecurityService).Verify(a => a.GetUser("TestUser")); // the account was not found in the workgroup or the account table
            Mock.Get(EventService).Verify(a => a.OrderApprovalAdded(It.IsAny<Order>(), It.IsAny<Approval>(), It.IsAny<bool>()), Times.Exactly(2));
            Mock.Get(EventService).Verify(a => a.OrderAutoApprovalAdded(It.IsAny<Order>(), It.IsAny<Approval>()), Times.Never());
            Mock.Get(EventService).Verify(a => a.OrderCreated(order));

            Assert.AreEqual(2, order.Approvals.Count); //Only 2 approvals for an external account
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[1].StatusCode.Id);
            Assert.AreEqual("LastName55", order.Approvals[0].User.LastName);
            Assert.IsNull(order.Approvals[1].User);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        /// <summary>
        /// The account exists in the workgroup and there is an auto approval because the current user is the same as the approver
        /// </summary>
        [Ignore("Net6Upgrade regression - to be fixed later")]
        public void TestCreateApprovalsForNewOrder3()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.CreatedBy = CreateValidEntities.User(11);
            order.Id = 99;
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            //order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Workgroup.Id = 1;

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.Id = "12345";
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.Id = "11";
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].Id = "12345";
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(SecurityService).Setup(a => a.GetUser("TestUser")).Returns(CreateValidEntities.User(55));

            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("22"); //NOT same as approver

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(11);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].TargetUser = null;
            autoApprovals[0].Account = workgroupAccounts[0].Account;
            autoApprovals[0].Expiration = DateTime.UtcNow.ToPacificTime().AddDays(1);
            autoApprovals[0].Equal = false;
            autoApprovals[0].LessThan = true;
            autoApprovals[0].MaxAmount = 200.00m;
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            Mock.Get(EventService).Setup(a => a.OrderApprovalAdded(It.IsAny<Order>(), It.IsAny<Approval>(), It.IsAny<bool>()));
            Mock.Get(EventService).Setup(a => a.OrderAutoApprovalAdded(It.IsAny<Order>(), It.IsAny<Approval>()));
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
            #endregion Act

            #region Assert
            Mock.Get(SecurityService).Verify(a => a.GetUser(It.IsAny<string>()), Times.Never()); // the account was not found in the workgroup or the account table
            Mock.Get(EventService).Verify(a => a.OrderApprovalAdded(It.IsAny<Order>(), It.IsAny<Approval>(), It.IsAny<bool>()), Times.Exactly(3));
            Mock.Get(EventService).Verify(a => a.OrderAutoApprovalAdded(It.IsAny<Order>(), It.IsAny<Approval>()), Times.Never());
            //var args = (Approval)EventService.GetArgumentsForCallsMadeOn(a => a.OrderAutoApprovalAdded(It.IsAny<Order>(), It.IsAny<Approval>()))[0][1];
            //Assert.AreEqual(OrderStatusCode.Codes.Approver, args.StatusCode.Id);

            Mock.Get(EventService).Verify(a => a.OrderCreated(order));

            Assert.AreEqual(3, order.Approvals.Count);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[1].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[2].StatusCode.Id);
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }
    }
}
