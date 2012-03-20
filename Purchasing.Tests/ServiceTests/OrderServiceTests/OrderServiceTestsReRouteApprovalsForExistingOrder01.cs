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
            UserIdentity.Expect(a => a.Current).Return("3");

            #endregion Arrange

            #region Act
            OrderService.ReRouteApprovalsForExistingOrder(order, "3", "2");
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(2));
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
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(3));
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
            EventService.AssertWasCalled(a => a.OrderApprovalAdded(Arg<Order>.Is.Anything, Arg<Approval>.Is.Anything), x => x.Repeat.Times(3));
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
