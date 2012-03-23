using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Rhino.Mocks;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests.NotificationServiceTests
{
    public partial class NotificationServiceTests
    {
        #region OrderApproved At Approver status Tests

        /// <summary>
        /// At Approval Stage, so only Bender gets notified
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedConditionalApproverNoEmailPrefs1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover);
            approval.Completed = true;
            order.GenerateRequestNumber();
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.IsTrue(order.EmailQueues[0].Pending);
            Assert.IsNull(order.EmailQueues[0].Status);
            Assert.AreEqual("Order request FakeHosttestOrg-FT1P9YRtestOrg-FT1P9YR for Unspecified Vendor has been approved by Homer Simpson at Conditional Approval review.", order.EmailQueues[0].Text);
            #endregion Assert
        }

        /// <summary>
        /// At Approval Stage, so only Bender gets notified, email prefs are just defaults
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedConditionalApproverEmailPrefs1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            #endregion Assert
        }

        /// <summary>
        /// At Approval Stage, so only Bender gets notified
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedConditionalApproverEmailPrefs2()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, order.EmailQueues[0].NotificationType);
            #endregion Assert
        }

        /// <summary>
        /// At Approval Stage, so only Bender gets notified
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedConditionalApproverEmailPrefs3()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Weekly;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, order.EmailQueues[0].NotificationType);
            #endregion Assert
        }

        /// <summary>
        /// At Approval Stage, so only Bender gets notified
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedConditionalApproverEmailPrefs4()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.PerEvent;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedConditionalApproverEmailPrefs5()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].RequesterApproverApproved = false;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, order.EmailQueues.Count);
            #endregion Assert
        }


        #endregion OrderApproved At Approver status Tests
    }
}
