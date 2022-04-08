using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.WS;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using Purchasing.Tests;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

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
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything, Arg<bool>.Is.Anything), x => x.Repeat.Times(3));
            EventService.AssertWasCalled(a => a.OrderReRouted(order));
            Assert.AreEqual(3, order.Approvals.Count);
            Assert.IsNull(order.Approvals[0].User);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.AreEqual("LastName2", order.Approvals[1].User.LastName);
            Assert.IsNull(order.Approvals[2].User);
            #endregion Assert		
        }

        /// <summary>
        /// Current user is same as approver.
        /// </summary>
        [TestMethod]
        public void TestReRouteApprovalsForExistingOrderWhenCurrectLevelIsApprove02()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.CreatedBy = CreateValidEntities.User(3);
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
            UserIdentity.Expect(a => a.Current).Return("3");

            #endregion Arrange

            #region Act
            OrderService.ReRouteApprovalsForExistingOrder(order, "3", "2");
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything, Arg<bool>.Is.Anything), x => x.Repeat.Times(2));
            EventService.AssertWasCalled(a => a.OrderAutoApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything));
            EventService.AssertWasCalled(a => a.OrderReRouted(order));
            Assert.AreEqual(3, order.Approvals.Count);
            Assert.AreEqual("LastName3",order.Approvals[0].User.LastName);
            Assert.IsTrue(order.Approvals[0].Completed);
            Assert.AreEqual("LastName2", order.Approvals[1].User.LastName);
            Assert.IsNull(order.Approvals[2].User);
            #endregion Assert
        }

        /// <summary>
        /// Conditional Approval not done, other approval is
        /// </summary>
        [TestMethod]
        public void TestReRouteApprovalsForExistingOrderWhenCurrectLevelIsApprove03()
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
            order.Approvals.Add(CreateValidEntities.Approval(4));
            order.Approvals[0].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            order.Approvals[0].Completed = true;
            order.Approvals[0].User = CreateValidEntities.User(88);
            order.Approvals[1].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            order.Approvals[2].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            order.Approvals[3].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.ConditionalApprover);
            order.Approvals[3].User = CreateValidEntities.User(99);

            new FakeUsers(4, UserRepository);

            #endregion Arrange

            #region Act
            OrderService.ReRouteApprovalsForExistingOrder(order, string.Empty, "2");
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything, Arg<bool>.Is.Anything), x => x.Repeat.Times(3));
            EventService.AssertWasCalled(a => a.OrderReRouted(order));
            Assert.AreEqual(4, order.Approvals.Count);
            Assert.AreEqual("LastName99", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);
            Assert.IsNull(order.Approvals[1].User);
            Assert.IsFalse(order.Approvals[1].Completed);
            Assert.AreEqual("LastName2", order.Approvals[2].User.LastName);
            Assert.IsNull(order.Approvals[3].User);
            #endregion Assert
        }

        [TestMethod]
        public void TestReRouteApprovalsForExistingOrderWhenCurrectLevelIsApprove04()
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
            order.Approvals.Add(CreateValidEntities.Approval(4));
            order.Approvals[0].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            order.Approvals[0].Completed = false;
            order.Approvals[0].User = CreateValidEntities.User(88);
            order.Approvals[1].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            order.Approvals[2].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            order.Approvals[3].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.ConditionalApprover);
            order.Approvals[3].User = CreateValidEntities.User(99);
            order.Approvals[3].Completed = true;

            new FakeUsers(4, UserRepository);

            #endregion Arrange

            #region Act
            OrderService.ReRouteApprovalsForExistingOrder(order, string.Empty, "2");
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything, Arg<bool>.Is.Anything), x => x.Repeat.Times(3));
            EventService.AssertWasCalled(a => a.OrderReRouted(order));
            Assert.AreEqual(4, order.Approvals.Count);
            Assert.AreEqual("LastName99", order.Approvals[0].User.LastName);
            Assert.IsTrue(order.Approvals[0].Completed);
            Assert.IsNull(order.Approvals[1].User);
            Assert.IsFalse(order.Approvals[1].Completed);
            Assert.AreEqual("LastName2", order.Approvals[2].User.LastName);
            Assert.IsNull(order.Approvals[3].User);
            #endregion Assert
        }


        /// <summary>
        /// Multiple splits, external accounts for both
        /// </summary>
        [TestMethod]
        public void TestReRouteApprovalsForExistingOrderWhenCurrectLevelIsApprove05()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.CreatedBy = CreateValidEntities.User(109);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(CreateValidEntities.Split(1));
            order.Splits.Add(CreateValidEntities.Split(2));
            order.Splits[0].Order = order;
            order.Splits[1].Order = order;

            order.Splits[0].Account = "12345";
            order.Splits[1].Account = "23456";

            order.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);

            order.Approvals = new List<Approval>();
            order.Approvals.Add(CreateValidEntities.Approval(1));
            order.Approvals.Add(CreateValidEntities.Approval(2));
            order.Approvals.Add(CreateValidEntities.Approval(3));
            order.Approvals.Add(CreateValidEntities.Approval(4));
            order.Approvals[0].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            order.Approvals[0].Completed = true;
            order.Approvals[0].User = CreateValidEntities.User(88);
            order.Approvals[1].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            order.Approvals[2].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            order.Approvals[3].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.ConditionalApprover);
            order.Approvals[3].User = CreateValidEntities.User(99);

            new FakeUsers(4, UserRepository);
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);

            #endregion Arrange

            #region Act
            OrderService.ReRouteApprovalsForExistingOrder(order, string.Empty, string.Empty);
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything, Arg<bool>.Is.Anything), x => x.Repeat.Times(3));
            EventService.AssertWasCalled(a => a.OrderReRouted(order));
            SecurityService.AssertWasNotCalled(a => a.GetUser(Arg<string>.Is.Anything));
            Assert.AreEqual(4, order.Approvals.Count);
            Assert.AreEqual("LastName99", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);

            Assert.IsNull(order.Approvals[1].User);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[1].StatusCode.Id);

            Assert.IsNull(order.Approvals[2].User);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[2].StatusCode.Id);

            Assert.IsNull(order.Approvals[3].User);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[3].StatusCode.Id);
            #endregion Assert
        }


        /// <summary>
        /// External Accounts, but accounts found
        /// </summary>
        [TestMethod]
        public void TestReRouteApprovalsForExistingOrderWhenCurrectLevelIsApprove06()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.CreatedBy = CreateValidEntities.User(109);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(CreateValidEntities.Split(1));
            order.Splits.Add(CreateValidEntities.Split(2));
            order.Splits[0].Order = order;
            order.Splits[1].Order = order;

            order.Splits[0].Account = "12345";
            order.Splits[1].Account = "23456";

            order.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);

            order.Approvals = new List<Approval>();
            order.Approvals.Add(CreateValidEntities.Approval(1));
            order.Approvals.Add(CreateValidEntities.Approval(2));
            order.Approvals.Add(CreateValidEntities.Approval(3));
            order.Approvals.Add(CreateValidEntities.Approval(4));
            order.Approvals[0].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            order.Approvals[0].Completed = true;
            order.Approvals[0].User = CreateValidEntities.User(88);
            order.Approvals[1].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            order.Approvals[2].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            order.Approvals[3].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.ConditionalApprover);
            order.Approvals[3].User = CreateValidEntities.User(99);

            new FakeUsers(4, UserRepository);
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);

            var accounts = new List<Account>();
            accounts.Add(CreateValidEntities.Account(1));
            accounts.Add(CreateValidEntities.Account(2));
            accounts[0].AccountManagerId = "123";
            accounts[1].AccountManagerId = "124";
            accounts[0].SetIdTo("12345");
            accounts[1].SetIdTo("23456");
            new FakeAccounts(0, AccountRepository, accounts, true);
            SecurityService.Expect(a => a.GetUser("123")).Return(CreateValidEntities.User(66));
            SecurityService.Expect(a => a.GetUser("124")).Return(CreateValidEntities.User(77));
            #endregion Arrange

            #region Act
            OrderService.ReRouteApprovalsForExistingOrder(order, string.Empty, string.Empty);
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything, Arg<bool>.Is.Anything), x => x.Repeat.Times(3));
            EventService.AssertWasCalled(a => a.OrderReRouted(order));
            SecurityService.AssertWasCalled(a => a.GetUser(Arg<string>.Is.Anything), x => x.Repeat.Times(2));
            Assert.AreEqual(4, order.Approvals.Count);
            Assert.AreEqual("LastName99", order.Approvals[0].User.LastName);
            Assert.IsFalse(order.Approvals[0].Completed);

            Assert.AreEqual("LastName66", order.Approvals[1].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[1].StatusCode.Id);

            Assert.IsNull(order.Approvals[2].User);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[2].StatusCode.Id);

            Assert.AreEqual("LastName77", order.Approvals[3].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[3].StatusCode.Id);
            #endregion Assert
        }

        /// <summary>
        /// Workgroup Account and external account
        /// </summary>
        [TestMethod]
        public void TestReRouteApprovalsForExistingOrderWhenCurrectLevelIsApprove07()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.CreatedBy = CreateValidEntities.User(109);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(CreateValidEntities.Split(1));
            order.Splits.Add(CreateValidEntities.Split(2));
            order.Splits[0].Order = order;
            order.Splits[1].Order = order;

            order.Splits[0].Account = "12345";
            order.Splits[1].Account = "23456";

            order.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);

            order.Approvals = new List<Approval>();
            order.Approvals.Add(CreateValidEntities.Approval(1));
            order.Approvals.Add(CreateValidEntities.Approval(2));
            order.Approvals.Add(CreateValidEntities.Approval(3));
            order.Approvals.Add(CreateValidEntities.Approval(4));
            order.Approvals[0].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            order.Approvals[0].Completed = true;
            order.Approvals[0].User = CreateValidEntities.User(88);
            order.Approvals[1].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            order.Approvals[2].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            order.Approvals[3].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.ConditionalApprover);
            order.Approvals[3].User = CreateValidEntities.User(99);

            new FakeUsers(4, UserRepository);
            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Account = CreateValidEntities.Account(1);
            workgroupAccounts[0].Account.SetIdTo("23456");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(555);
            workgroupAccounts[0].Approver = CreateValidEntities.User(556);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(557);
            workgroupAccounts[0].Workgroup = CreateValidEntities.Workgroup(3);
            workgroupAccounts[0].Workgroup.SetIdTo(3);
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            order.Workgroup = workgroupAccounts[0].Workgroup;
            order.Workgroup.Accounts = workgroupAccounts;
            

            var accounts = new List<Account>();
            accounts.Add(CreateValidEntities.Account(1));
            //accounts.Add(CreateValidEntities.Account(2));
            accounts[0].AccountManagerId = "123";
            //accounts[1].AccountManagerId = "124";
            accounts[0].SetIdTo("12345");
            //accounts[1].SetIdTo("23456");
            new FakeAccounts(0, AccountRepository, accounts, true);
            SecurityService.Expect(a => a.GetUser("123")).Return(CreateValidEntities.User(66));
            //SecurityService.Expect(a => a.GetUser("124")).Return(CreateValidEntities.User(77));

            new FakeAutoApprovals(3, AutoAprovalRepository);
            #endregion Arrange

            #region Act
            OrderService.ReRouteApprovalsForExistingOrder(order, string.Empty, string.Empty);
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything, Arg<bool>.Is.Anything), x => x.Repeat.Times(4));
            EventService.AssertWasCalled(a => a.OrderReRouted(order));
            SecurityService.AssertWasCalled(a => a.GetUser(Arg<string>.Is.Anything), x => x.Repeat.Times(1));
            Assert.AreEqual(5, order.Approvals.Count);
            Assert.AreEqual("LastName99", order.Approvals[0].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.ConditionalApprover, order.Approvals[0].StatusCode.Id);

            Assert.AreEqual("LastName66", order.Approvals[1].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[1].StatusCode.Id);

            Assert.IsNull(order.Approvals[2].User);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[2].StatusCode.Id);

            Assert.AreEqual("LastName556", order.Approvals[3].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, order.Approvals[3].StatusCode.Id);

            Assert.AreEqual("LastName555", order.Approvals[4].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[4].StatusCode.Id);
            #endregion Assert
        }

        /// <summary>
        /// Workgroup Account and external account (same as #7 except external account is second which makes the purchaser not null
        /// </summary>
        [TestMethod]
        public void TestReRouteApprovalsForExistingOrderWhenCurrectLevelIsApprove08()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.CreatedBy = CreateValidEntities.User(109);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(CreateValidEntities.Split(1));
            order.Splits.Add(CreateValidEntities.Split(2));
            order.Splits[0].Order = order;
            order.Splits[1].Order = order;

            order.Splits[1].Account = "12345"; // switched order from one above
            order.Splits[0].Account = "23456";

            order.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);

            order.Approvals = new List<Approval>();
            order.Approvals.Add(CreateValidEntities.Approval(1));
            order.Approvals.Add(CreateValidEntities.Approval(2));
            order.Approvals.Add(CreateValidEntities.Approval(3));
            order.Approvals.Add(CreateValidEntities.Approval(4));
            order.Approvals[0].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            order.Approvals[0].Completed = true;
            order.Approvals[0].User = CreateValidEntities.User(88);
            order.Approvals[1].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            order.Approvals[2].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            order.Approvals[3].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.ConditionalApprover);
            order.Approvals[3].User = CreateValidEntities.User(99);

            new FakeUsers(4, UserRepository);
            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Account = CreateValidEntities.Account(1);
            workgroupAccounts[0].Account.SetIdTo("23456");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(555);
            workgroupAccounts[0].Approver = CreateValidEntities.User(556);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(557);
            workgroupAccounts[0].Workgroup = CreateValidEntities.Workgroup(3);
            workgroupAccounts[0].Workgroup.SetIdTo(3);
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            order.Workgroup = workgroupAccounts[0].Workgroup;
            order.Workgroup.Accounts = workgroupAccounts;


            var accounts = new List<Account>();
            accounts.Add(CreateValidEntities.Account(1));
            //accounts.Add(CreateValidEntities.Account(2));
            accounts[0].AccountManagerId = "123";
            //accounts[1].AccountManagerId = "124";
            accounts[0].SetIdTo("12345");
            //accounts[1].SetIdTo("23456");
            new FakeAccounts(0, AccountRepository, accounts, true);
            SecurityService.Expect(a => a.GetUser("123")).Return(CreateValidEntities.User(66));
            //SecurityService.Expect(a => a.GetUser("124")).Return(CreateValidEntities.User(77));

            new FakeAutoApprovals(3, AutoAprovalRepository);
            #endregion Arrange

            #region Act
            OrderService.ReRouteApprovalsForExistingOrder(order, string.Empty, string.Empty);
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything, Arg<bool>.Is.Anything), x => x.Repeat.Times(4));
            EventService.AssertWasCalled(a => a.OrderReRouted(order));
            SecurityService.AssertWasCalled(a => a.GetUser(Arg<string>.Is.Anything), x => x.Repeat.Times(1));
            Assert.AreEqual(5, order.Approvals.Count);
            Assert.AreEqual("LastName99", order.Approvals[0].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.ConditionalApprover, order.Approvals[0].StatusCode.Id);

            Assert.AreEqual("LastName556", order.Approvals[1].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, order.Approvals[1].StatusCode.Id);
            
            Assert.AreEqual("LastName555", order.Approvals[2].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[2].StatusCode.Id);

            Assert.AreEqual("LastName557", order.Approvals[3].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[3].StatusCode.Id);

            Assert.AreEqual("LastName66", order.Approvals[4].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[4].StatusCode.Id);

            #endregion Assert
        }

        /// <summary>
        /// Both accounts in workgroup
        /// </summary>
        [TestMethod]
        public void TestReRouteApprovalsForExistingOrderWhenCurrectLevelIsApprove09()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.CreatedBy = CreateValidEntities.User(109);
            order.SetIdTo(99);
            order.Splits = new List<Split>();
            order.Splits.Add(CreateValidEntities.Split(1));
            order.Splits.Add(CreateValidEntities.Split(2));
            order.Splits[0].Order = order;
            order.Splits[1].Order = order;

            order.Splits[1].Account = "12345"; // switched order from one above
            order.Splits[0].Account = "23456";

            order.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);

            order.Approvals = new List<Approval>();
            order.Approvals.Add(CreateValidEntities.Approval(1));
            order.Approvals.Add(CreateValidEntities.Approval(2));
            order.Approvals.Add(CreateValidEntities.Approval(3));
            order.Approvals.Add(CreateValidEntities.Approval(4));
            order.Approvals[0].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            order.Approvals[0].Completed = true;
            order.Approvals[0].User = CreateValidEntities.User(88);
            order.Approvals[1].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            order.Approvals[2].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            order.Approvals[3].StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.ConditionalApprover);
            order.Approvals[3].User = CreateValidEntities.User(99);

            new FakeUsers(4, UserRepository);
            var workgroupAccounts = new List<WorkgroupAccount>();
            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroupAccounts[0].Account = CreateValidEntities.Account(1);
            workgroupAccounts[0].Account.SetIdTo("23456");
            workgroupAccounts[0].AccountManager = CreateValidEntities.User(555);
            workgroupAccounts[0].Approver = CreateValidEntities.User(556);
            workgroupAccounts[0].Purchaser = CreateValidEntities.User(557);
            workgroupAccounts[0].Workgroup = CreateValidEntities.Workgroup(3);
            workgroupAccounts[0].Workgroup.SetIdTo(3);

            workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(2));
            workgroupAccounts[1].Account = CreateValidEntities.Account(2);
            workgroupAccounts[1].Account.SetIdTo("12345");
            workgroupAccounts[1].AccountManager = CreateValidEntities.User(444);
            workgroupAccounts[1].Approver = CreateValidEntities.User(445);
            workgroupAccounts[1].Purchaser = CreateValidEntities.User(446);
            workgroupAccounts[1].Workgroup = CreateValidEntities.Workgroup(3);
            workgroupAccounts[1].Workgroup.SetIdTo(3);
            new FakeWorkgroupAccounts(0, WorkgroupAccountRepository, workgroupAccounts);
            order.Workgroup = workgroupAccounts[0].Workgroup;
            order.Workgroup.Accounts = workgroupAccounts;

            new FakeAutoApprovals(3, AutoAprovalRepository);
            #endregion Arrange

            #region Act
            OrderService.ReRouteApprovalsForExistingOrder(order, string.Empty, string.Empty);
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything, Arg<bool>.Is.Anything), x => x.Repeat.Times(5));
            EventService.AssertWasCalled(a => a.OrderReRouted(order));
            SecurityService.AssertWasNotCalled(a => a.GetUser(Arg<string>.Is.Anything));
            Assert.AreEqual(6, order.Approvals.Count);
            Assert.AreEqual("LastName99", order.Approvals[0].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.ConditionalApprover, order.Approvals[0].StatusCode.Id);

            Assert.AreEqual("LastName556", order.Approvals[1].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, order.Approvals[1].StatusCode.Id);

            Assert.AreEqual("LastName555", order.Approvals[2].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[2].StatusCode.Id);

            Assert.AreEqual("LastName557", order.Approvals[3].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.Purchaser, order.Approvals[3].StatusCode.Id);

            Assert.AreEqual("LastName445", order.Approvals[4].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.Approver, order.Approvals[4].StatusCode.Id);

            Assert.AreEqual("LastName444", order.Approvals[5].User.LastName);
            Assert.AreEqual(OrderStatusCode.Codes.AccountManager, order.Approvals[5].StatusCode.Id);

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
