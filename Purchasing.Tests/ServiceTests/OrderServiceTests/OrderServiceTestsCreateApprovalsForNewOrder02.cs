using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Rhino.Mocks;
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
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName2", order.Approvals[1].User.LastName);
            Assert.IsNull(order.Approvals[2].User);
            Assert.AreEqual(null, order.Splits[0].Account);
            #endregion Assert		
        }

        /// <summary>
        /// Account Manager passed, not account,  approver passed and is current user
        /// 
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder15()
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
            UserIdentity.Expect(a => a.Current).Return("3");
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, "3", "2");
            #endregion Act

            #region Assert
            SecurityService.AssertWasNotCalled(a => a.GetUser(Arg<string>.Is.Anything)); // the account was not found in the workgroup or the account table
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(2));
            EventService.AssertWasCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            var args = (Approval)EventService.GetArgumentsForCallsMadeOn(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything))[0][1];
            Assert.AreEqual(OrderStatusCode.Codes.Approver, args.StatusCode.Id);

            EventService.AssertWasCalled(a => a.OrderCreated(order));

            Assert.AreEqual(3, order.Approvals.Count);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[1].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[2].StatusCode.Id);
            Assert.AreEqual("LastName3", order.Approvals[0].User.LastName);
            Assert.IsTrue(order.Approvals[0].Completed);
            Assert.AreEqual("LastName2", order.Approvals[1].User.LastName);
            Assert.IsNull(order.Approvals[2].User);
            Assert.AreEqual(null, order.Splits[0].Account);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.StatusCode.Id);
            #endregion Assert
        }

        /// <summary>
        /// Account Manager passed, not account,  approver passed and is *NOT* current user
        /// 
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder16()
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
            UserIdentity.Expect(a => a.Current).Return("1");

            new FakeAutoApprovals(3, AutoAprovalRepository);
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, "3", "2");
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
            Assert.AreEqual("LastName3", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName2", order.Approvals[1].User.LastName);
            Assert.IsNull(order.Approvals[2].User);
            Assert.AreEqual(null, order.Splits[0].Account);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, order.StatusCode.Id);
            #endregion Assert
        }


        /// <summary>
        /// Multiple Splits
        /// there is only ever one purchaser approval per order
        /// 2 account managers, both null
        /// 0 approvers because both external
        /// The account does not exist in the workgroup (or anywhere) so it is an external one.
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder17()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;

            order.Splits.Add(new Split());
            order.Splits[1].Account = "23456";
            order.Splits[1].Order = order;

            order.Workgroup.SetIdTo(1);
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            new FakeAccounts(3, AccountRepository);
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, null, null);
            #endregion Act

            #region Assert
            SecurityService.AssertWasNotCalled(a => a.GetUser(Arg<string>.Is.Anything)); // the account was not found in the workgroup or the account table
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(3));
            EventService.AssertWasNotCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            EventService.AssertWasCalled(a => a.OrderCreated(order));

            Assert.AreEqual(3, order.Approvals.Count); //no approvers, 2 account managers, 1 purchaser
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[1].StatusCode.Id);
            var purchaserCount = 0;
            var approverCount = 0;
            var acctManagerCount = 0;
            foreach(var approval in order.Approvals)
            {
                switch (approval.StatusCode.Id)
                {
                    case OrderStatusCode.Codes.Purchaser:
                        purchaserCount++;
                        break;
                    case OrderStatusCode.Codes.AccountManager:
                        acctManagerCount++;
                        break;
                    case OrderStatusCode.Codes.Approver:
                        approverCount++;
                        break;
                    default:
                        throw new Exception("Should not be here!");                        
                }

                Assert.IsNull(approval.User);
            }
            Assert.AreEqual(1, purchaserCount);
            Assert.AreEqual(2, acctManagerCount);
            Assert.AreEqual(0, approverCount);
            #endregion Assert
        }

        /// <summary>
        /// Multiple Splits
        /// there is only ever one purchaser approval per order
        /// 2 account managers, both null
        /// 0 approvers because both external
        /// The 1 account does not exist in the workgroup (or anywhere) so it is an external one, 1 is external but exists.
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder18()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;

            order.Splits.Add(new Split());
            order.Splits[1].Account = "23456";
            order.Splits[1].Order = order;

            order.Workgroup.SetIdTo(1);
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);

            var accounts = new List<Account>();
            accounts.Add(CreateValidEntities.Account(1));
            accounts[0].SetIdTo("23456");
            accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(0, AccountRepository, accounts, true);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, null, null);
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.GetUser("TestUser")); 
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(3));
            EventService.AssertWasNotCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            EventService.AssertWasCalled(a => a.OrderCreated(order));

            Assert.AreEqual(3, order.Approvals.Count); //no approvers, 2 account managers, 1 purchaser
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[1].StatusCode.Id);
            var purchaserCount = 0;
            var approverCount = 0;
            var acctManagerCount = 0;
            foreach(var approval in order.Approvals)
            {
                switch(approval.StatusCode.Id)
                {
                    case OrderStatusCode.Codes.Purchaser:
                        purchaserCount++;
                        break;
                    case OrderStatusCode.Codes.AccountManager:
                        acctManagerCount++;
                        break;
                    case OrderStatusCode.Codes.Approver:
                        approverCount++;
                        break;
                    default:
                        throw new Exception("Should not be here!");
                }

                //Assert.IsNull(approval.User);
            }
            Assert.AreEqual(1, purchaserCount);
            Assert.AreEqual(2, acctManagerCount);
            Assert.AreEqual(0, approverCount);
            Assert.AreEqual("LastName55", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            Assert.AreEqual("23456", order.Splits[1].Account);
            #endregion Assert
        }

        /// <summary>
        /// Multiple Splits
        /// there is only ever one purchaser approval per order
        /// 2 account managers, both null
        /// 0 approvers because both external
        /// Both accounts exist in external table
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder19()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;

            order.Splits.Add(new Split());
            order.Splits[1].Account = "23456";
            order.Splits[1].Order = order;

            order.Workgroup.SetIdTo(1);
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);

            var accounts = new List<Account>();
            accounts.Add(CreateValidEntities.Account(1));
            accounts[0].SetIdTo("23456");
            accounts[0].AccountManagerId = "TestUser";
            accounts.Add(CreateValidEntities.Account(2));
            accounts[1].SetIdTo("12345");
            accounts[1].AccountManagerId = "TestUser2";
            new FakeAccounts(0, AccountRepository, accounts, true);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));
            SecurityService.Expect(a => a.GetUser("TestUser2")).Return(CreateValidEntities.User(66));
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, null, null);
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.GetUser("TestUser"));
            SecurityService.AssertWasCalled(a => a.GetUser("TestUser2"));
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(3));
            EventService.AssertWasNotCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            EventService.AssertWasCalled(a => a.OrderCreated(order));

            Assert.AreEqual(3, order.Approvals.Count); //no approvers, 2 account managers, 1 purchaser
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[1].StatusCode.Id);
            var purchaserCount = 0;
            var approverCount = 0;
            var acctManagerCount = 0;
            foreach(var approval in order.Approvals)
            {
                switch(approval.StatusCode.Id)
                {
                    case OrderStatusCode.Codes.Purchaser:
                        purchaserCount++;
                        break;
                    case OrderStatusCode.Codes.AccountManager:
                        acctManagerCount++;
                        break;
                    case OrderStatusCode.Codes.Approver:
                        approverCount++;
                        break;
                    default:
                        throw new Exception("Should not be here!");
                }

                //Assert.IsNull(approval.User);
            }
            Assert.AreEqual(1, purchaserCount);
            Assert.AreEqual(2, acctManagerCount);
            Assert.AreEqual(0, approverCount);
            Assert.AreEqual("LastName66", order.Approvals[0].User.LastName);
            Assert.AreEqual("LastName55", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            Assert.AreEqual("23456", order.Splits[1].Account);
            #endregion Assert
        }

        /// <summary>
        /// 2 external account, 1 with workgroup
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder20()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;

            order.Splits.Add(new Split());
            order.Splits[1].Account = "23456";
            order.Splits[1].Order = order;

            order.Splits.Add(new Split());
            order.Splits[2].Account = "777";
            order.Splits[2].Order = order;

            order.Workgroup.SetIdTo(1);
            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("777");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);

            var accounts = new List<Account>();
            accounts.Add(CreateValidEntities.Account(1));
            accounts[0].SetIdTo("23456");
            accounts[0].AccountManagerId = "TestUser";
            accounts.Add(CreateValidEntities.Account(2));
            accounts[1].SetIdTo("12345");
            accounts[1].AccountManagerId = null;
            new FakeAccounts(0, AccountRepository, accounts, true);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));
            SecurityService.Expect(a => a.GetUser(null)).Return(null);

            new FakeAutoApprovals(3, AutoAprovalRepository);
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, null, null);
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.GetUser("TestUser"));
            SecurityService.AssertWasCalled(a => a.GetUser(null));
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(5));
            EventService.AssertWasNotCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            EventService.AssertWasCalled(a => a.OrderCreated(order));

            Assert.AreEqual(5, order.Approvals.Count);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[1].StatusCode.Id);
            var purchaserCount = 0;
            var approverCount = 0;
            var acctManagerCount = 0;
            foreach(var approval in order.Approvals)
            {
                switch(approval.StatusCode.Id)
                {
                    case OrderStatusCode.Codes.Purchaser:
                        purchaserCount++;
                        break;
                    case OrderStatusCode.Codes.AccountManager:
                        acctManagerCount++;
                        break;
                    case OrderStatusCode.Codes.Approver:
                        approverCount++;
                        break;
                    default:
                        throw new Exception("Should not be here!");
                }

                //Assert.IsNull(approval.User);
            }
            Assert.AreEqual(1, purchaserCount);
            Assert.AreEqual(3, acctManagerCount);
            Assert.AreEqual(1, approverCount);
            //Assert.AreEqual("LastName66", order.Approvals[0].User.LastName);
            Assert.AreEqual("LastName55", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            Assert.AreEqual("23456", order.Splits[1].Account);
            #endregion Assert
        }

        /// <summary>
        /// 2 external account, 1 with workgroup
        /// 1 auto Approval
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder21()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;

            order.Splits.Add(new Split());
            order.Splits[1].Account = "23456";
            order.Splits[1].Order = order;

            order.Splits.Add(new Split());
            order.Splits[2].Account = "777";
            order.Splits[2].Order = order;

            order.Workgroup.SetIdTo(1);
            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("777");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);

            var accounts = new List<Account>();
            accounts.Add(CreateValidEntities.Account(1));
            accounts[0].SetIdTo("23456");
            accounts[0].AccountManagerId = "TestUser";
            accounts.Add(CreateValidEntities.Account(2));
            accounts[1].SetIdTo("12345");
            accounts[1].AccountManagerId = null;
            new FakeAccounts(0, AccountRepository, accounts, true);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));
            SecurityService.Expect(a => a.GetUser(null)).Return(null);

            new FakeAutoApprovals(3, AutoAprovalRepository);

            UserIdentity.Expect(a => a.Current).Return("11");
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, null, null);
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.GetUser("TestUser"));
            SecurityService.AssertWasCalled(a => a.GetUser(null));
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(4));
            EventService.AssertWasCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            EventService.AssertWasCalled(a => a.OrderCreated(order));

            Assert.AreEqual(5, order.Approvals.Count);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[1].StatusCode.Id);
            var purchaserCount = 0;
            var approverCount = 0;
            var acctManagerCount = 0;
            foreach(var approval in order.Approvals)
            {
                switch(approval.StatusCode.Id)
                {
                    case OrderStatusCode.Codes.Purchaser:
                        purchaserCount++;
                        break;
                    case OrderStatusCode.Codes.AccountManager:
                        acctManagerCount++;
                        break;
                    case OrderStatusCode.Codes.Approver:
                        approverCount++;
                        break;
                    default:
                        throw new Exception("Should not be here!");
                }

                //Assert.IsNull(approval.User);
            }
            Assert.AreEqual(1, purchaserCount);
            Assert.AreEqual(3, acctManagerCount);
            Assert.AreEqual(1, approverCount);
            //Assert.AreEqual("LastName66", order.Approvals[0].User.LastName);
            Assert.AreEqual("LastName55", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            Assert.AreEqual("23456", order.Splits[1].Account);
            Assert.IsTrue(order.Approvals[3].Completed);
            #endregion Assert
        }

        /// <summary>
        /// 2 external account, 1 with workgroup
        /// 1 auto Approval because of amount
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder22()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;

            order.Splits.Add(new Split());
            order.Splits[1].Account = "23456";
            order.Splits[1].Order = order;

            order.Splits.Add(new Split());
            order.Splits[2].Account = "777";
            order.Splits[2].Order = order;

            order.Workgroup.SetIdTo(1);
            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("777");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);

            var accounts = new List<Account>();
            accounts.Add(CreateValidEntities.Account(1));
            accounts[0].SetIdTo("23456");
            accounts[0].AccountManagerId = "TestUser";
            accounts.Add(CreateValidEntities.Account(2));
            accounts[1].SetIdTo("12345");
            accounts[1].AccountManagerId = null;
            new FakeAccounts(0, AccountRepository, accounts, true);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));
            SecurityService.Expect(a => a.GetUser(null)).Return(null);

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(11);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].Expiration = DateTime.Now.AddDays(1);
            autoApprovals[0].TargetUser = null;
            autoApprovals[0].Account = workgroupAccounts[0].Account;
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            //UserIdentity.Expect(a => a.Current).Return("11");
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, null, null, null);
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.GetUser("TestUser"));
            SecurityService.AssertWasCalled(a => a.GetUser(null));
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(4));
            EventService.AssertWasCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            EventService.AssertWasCalled(a => a.OrderCreated(order));

            Assert.AreEqual(5, order.Approvals.Count);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[0].StatusCode.Id);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[1].StatusCode.Id);
            var purchaserCount = 0;
            var approverCount = 0;
            var acctManagerCount = 0;
            foreach(var approval in order.Approvals)
            {
                switch(approval.StatusCode.Id)
                {
                    case OrderStatusCode.Codes.Purchaser:
                        purchaserCount++;
                        break;
                    case OrderStatusCode.Codes.AccountManager:
                        acctManagerCount++;
                        break;
                    case OrderStatusCode.Codes.Approver:
                        approverCount++;
                        break;
                    default:
                        throw new Exception("Should not be here!");
                }

                //Assert.IsNull(approval.User);
            }
            Assert.AreEqual(1, purchaserCount);
            Assert.AreEqual(3, acctManagerCount);
            Assert.AreEqual(1, approverCount);
            //Assert.AreEqual("LastName66", order.Approvals[0].User.LastName);
            Assert.AreEqual("LastName55", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            Assert.AreEqual("23456", order.Splits[1].Account);
            Assert.IsTrue(order.Approvals[3].Completed);
            #endregion Assert
        }
    }
}
