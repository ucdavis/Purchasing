using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Rhino.Mocks;
using System.Linq;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests.NotificationServiceTests
{
    public partial class NotificationServiceTests
    {
        /*
         * Test approvals when approver is null
         * Test when approver is away and secondary user is null
         * Test when approver is away and secondary user is away
         * test when approver is away and secondary user is not away
         * test when approver is not away
         * test when conditional approver is away - > secondary user
         * test when conditional approver is not away and secondary user is not away
         * 
         */ 

        /// <summary>
        /// Approver is not null and not away
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);

            foreach (var approval in approvals)
            {
                order.AddApproval(approval);
            }

            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueues.Count());
            Assert.AreEqual(string.Format("Order request {0} has been submitted.", "#111231-000001"), order.EmailQueues[0].Text);
            Assert.AreEqual("bender", order.EmailQueues[0].User.Id);
            Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#111231-000001"), order.EmailQueues[1].Text);
            Assert.AreEqual("hsimpson", order.EmailQueues[1].User.Id);
            #endregion Assert		
        }

        /// <summary>
        /// Approver is not null and is away, secondary is not away or null
        /// Both get notified
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival2()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);

            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    approval.User.IsAway = true;
                    approval.SecondaryUser = UserRepository.Queryable.Single(a => a.Id == "awong");
                }
                order.AddApproval(approval);
            }

            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, order.EmailQueues.Count());
            Assert.AreEqual(string.Format("Order request {0} has been submitted.", "#111231-000001"), order.EmailQueues[0].Text);
            Assert.AreEqual("bender", order.EmailQueues[0].User.Id);
            Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#111231-000001"), order.EmailQueues[1].Text);
            Assert.AreEqual("hsimpson", order.EmailQueues[1].User.Id);
            Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#111231-000001"), order.EmailQueues[2].Text);
            Assert.AreEqual("awong", order.EmailQueues[2].User.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCreatedProcessArrival3()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);

            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.SetIdTo(order.Id);
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("zoidberg");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("flanders");
            approvals.Add(conditionalApproval);

            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    approval.User.IsAway = true;
                    approval.SecondaryUser = UserRepository.Queryable.Single(a => a.Id == "awong");
                }
                order.AddApproval(approval);
            }

            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, order.EmailQueues.Count());
            Assert.AreEqual(string.Format("Order request {0} has been submitted.", "#111231-000001"), order.EmailQueues[0].Text);
            Assert.AreEqual("bender", order.EmailQueues[0].User.Id);
            Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#111231-000001"), order.EmailQueues[1].Text);
            Assert.AreEqual("hsimpson", order.EmailQueues[1].User.Id);
            Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#111231-000001"), order.EmailQueues[2].Text);
            Assert.AreEqual("awong", order.EmailQueues[2].User.Id);
            Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#111231-000001"), order.EmailQueues[3].Text);
            Assert.AreEqual("zoidberg", order.EmailQueues[3].User.Id);
            Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#111231-000001"), order.EmailQueues[4].Text);
            Assert.AreEqual("flanders", order.EmailQueues[4].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// Multiple approvers that are the same.
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival4()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);

            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.SetIdTo(order.Id);
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("hsimpson");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("hsimpson");
            approvals.Add(conditionalApproval);

            foreach(var approval in approvals)
            {
                order.AddApproval(approval);
            }

            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueues.Count());
            Assert.AreEqual(string.Format("Order request {0} has been submitted.", "#111231-000001"), order.EmailQueues[0].Text);
            Assert.AreEqual("bender", order.EmailQueues[0].User.Id);
            Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#111231-000001"), order.EmailQueues[1].Text);
            Assert.AreEqual("hsimpson", order.EmailQueues[1].User.Id);
            #endregion Assert
        }
    }
}
