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
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("You must either supply the ID of a valid account or provide the userId for an account manager", ex.Message);
                throw;
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
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Workgroup.SetIdTo(1);
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            new FakeAccounts(3, AccountRepository);
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
            #endregion Act

            #region Assert
            SecurityService.AssertWasNotCalled(a => a.GetUser(Arg<string>.Is.Anything)); // the account was not found in the workgroup or the account table
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(2));
            EventService.AssertWasNotCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            EventService.AssertWasCalled(a => a.OrderCreated(order));

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
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            //order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Workgroup.SetIdTo(1);
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            var accounts = new List<Account>();
            accounts.Add(CreateValidEntities.Account(1));
            accounts[0].SetIdTo("12345");
            accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(0, AccountRepository, accounts, true);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
            #endregion Act

            #region Assert
            SecurityService.AssertWasCalled(a => a.GetUser("TestUser")); // the account was not found in the workgroup or the account table
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(2));
            EventService.AssertWasNotCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            EventService.AssertWasCalled(a => a.OrderCreated(order));

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
        [TestMethod]
        public void TestCreateApprovalsForNewOrder3()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            //order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Workgroup.SetIdTo(1);

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("11"); //same as approver
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsTrue(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        /// <summary>
        /// The account exists in the workgroup and there is NOT an auto approval because the current user is NOT the same as the approver AND there are no Auto Approvals
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder4()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            //order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Workgroup.SetIdTo(1);

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("22"); //NOT same as approver

            new FakeAutoApprovals(3, AutoAprovalRepository);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        /// <summary>
        /// The account exists in the workgroup and there is NOT an auto approval(service) because the current user is NOT the same as the approver But there is an Auto Approvals(db)
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder5()
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

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("22"); //NOT same as approver

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(11);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].Expiration = DateTime.Now.AddDays(1);
            autoApprovals[0].TargetUser = order.CreatedBy;
            autoApprovals[0].Account = null;
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsTrue(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        /// <summary>
        /// The account exists in the workgroup and there is NOT an auto approval(service) because the current user is NOT the same as the approver But there is an Auto Approvals(db)
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder6()
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

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("22"); //NOT same as approver

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(11);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].Expiration = DateTime.Now.AddDays(1);
            autoApprovals[0].TargetUser = null;
            autoApprovals[0].Account = workgroupAccounts[0].Account;
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsTrue(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateApprovalsForNewOrder7()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.CreatedBy = CreateValidEntities.User(109);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            //order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Splits[0].Amount = 10.77m;
            order.Workgroup.SetIdTo(1);

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("22"); //NOT same as approver

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(11);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].Expiration = DateTime.Now.AddDays(1);
            autoApprovals[0].TargetUser = null;
            autoApprovals[0].Account = workgroupAccounts[0].Account;
            autoApprovals[0].Equal = true;
            autoApprovals[0].LessThan = false;
            autoApprovals[0].MaxAmount = order.Splits[0].Amount;
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsTrue(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        /// <summary>
        /// Auto Approval expired
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder8()
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

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("22"); //NOT same as approver

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(11);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].TargetUser = null;
            autoApprovals[0].Account = workgroupAccounts[0].Account;
            autoApprovals[0].Expiration = DateTime.Now.AddDays(-1);
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        /// <summary>
        /// AutoApproval not active
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder9()
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

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("22"); //NOT same as approver

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(11);
            autoApprovals[0].IsActive = false;
            autoApprovals[0].TargetUser = null;
            autoApprovals[0].Account = workgroupAccounts[0].Account;
            autoApprovals[0].Expiration = DateTime.Now.AddDays(1);
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        /// <summary>
        /// Auto Approval not for approver
        /// </summary>
        [TestMethod]
        public void TestCreateApprovalsForNewOrder10()
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

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("22"); //NOT same as approver

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(22);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].TargetUser = null;
            autoApprovals[0].Account = workgroupAccounts[0].Account;
            autoApprovals[0].Expiration = DateTime.Now.AddDays(1);
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateApprovalsForNewOrder11()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.CreatedBy = CreateValidEntities.User(109);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            //order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Splits[0].Amount = 100.00m;
            order.Workgroup.SetIdTo(1);

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("22"); //NOT same as approver

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(11);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].TargetUser = null;
            autoApprovals[0].Account = workgroupAccounts[0].Account;
            autoApprovals[0].Expiration = DateTime.Now.AddDays(1);
            autoApprovals[0].Equal = true;
            autoApprovals[0].LessThan = false;
            autoApprovals[0].MaxAmount = 200.00m;
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateApprovalsForNewOrder12()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.CreatedBy = CreateValidEntities.User(109);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            //order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Splits[0].Amount = 201.00m;
            order.Workgroup.SetIdTo(1);

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("22"); //NOT same as approver

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(11);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].TargetUser = null;
            autoApprovals[0].Account = workgroupAccounts[0].Account;
            autoApprovals[0].Expiration = DateTime.Now.AddDays(1);
            autoApprovals[0].Equal = true;
            autoApprovals[0].LessThan = false;
            autoApprovals[0].MaxAmount = 200.00m;
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateApprovalsForNewOrder13()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.SetIdTo(99);
            order.CreatedBy = CreateValidEntities.User(109);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            //order.Splits[0].Account = "12345";
            order.Splits[0].Order = order;
            order.Splits[0].Amount = 200.00m;
            order.Workgroup.SetIdTo(1);

            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Workgroup = order.Workgroup;
            workgroupAccounts[0].Account = CreateValidEntities.Account(9);
            workgroupAccounts[0].Account.SetIdTo("12345");
            workgroupAccounts[0].Approver = CreateValidEntities.User(11);
            workgroupAccounts[0].Approver.SetIdTo("11");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(22);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(33);

            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            //var accounts = new List<Account>();
            //accounts.Add(CreateValidEntities.Account(1));
            //accounts[0].SetIdTo("12345");
            //accounts[0].AccountManagerId = "TestUser";
            new FakeAccounts(3, AccountRepository);
            SecurityService.Expect(a => a.GetUser("TestUser")).Return(CreateValidEntities.User(55));

            UserIdentity.Expect(a => a.Current).Return("22"); //NOT same as approver

            var autoApprovals = new List<AutoApproval>();
            autoApprovals.Add(CreateValidEntities.AutoApproval(1));
            autoApprovals[0].User = CreateValidEntities.User(11);
            autoApprovals[0].IsActive = true;
            autoApprovals[0].TargetUser = null;
            autoApprovals[0].Account = workgroupAccounts[0].Account;
            autoApprovals[0].Expiration = DateTime.Now.AddDays(1);
            autoApprovals[0].Equal = false;
            autoApprovals[0].LessThan = true;
            autoApprovals[0].MaxAmount = 200.00m;
            new FakeAutoApprovals(0, AutoAprovalRepository, autoApprovals);

            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
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
            Assert.AreEqual("LastName11", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName22", order.Approvals[1].User.LastName);
            Assert.AreEqual("LastName33", order.Approvals[2].User.LastName);
            Assert.AreEqual("12345", order.Splits[0].Account);
            #endregion Assert
        }
    }
}
