using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using Rhino.Mocks;

namespace Purchasing.Tests.ServiceTests.NotificationServiceTests
{
    public partial class NotificationServiceTests
    {
        #region OrderApproved At Approver status Tests

        /// <summary>
        /// At Approval Stage, so only Bender gets notified
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedNoEmailPrefs1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;
            order.GenerateRequestNumber();
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert           
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);
            Assert.AreEqual("Approved", order.EmailQueuesV2[0].Action);
            Assert.AreEqual("By Homer Simpson at Approver review.", order.EmailQueuesV2[0].Details);
            #endregion Assert		
        }


        [TestMethod]
        public void TestOrderApprovedShouldCreateEmailQueueWithUserOrEmail()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.IsTrue(order.EmailQueuesV2[0].User != null || !string.IsNullOrWhiteSpace(order.EmailQueuesV2[0].Email));
            #endregion Assert		
        }

        /// <summary>
        /// At Approval Stage, so only Bender gets notified, email prefs are just defaults
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedEmailPrefs1()
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
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            #endregion Assert
        }

        /// <summary>
        /// At Approval Stage, so only Bender gets notified
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedEmailPrefs2()
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
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, order.EmailQueuesV2[0].NotificationType);
            #endregion Assert
        }

        /// <summary>
        /// At Approval Stage, so only Bender gets notified
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedEmailPrefs3()
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
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, order.EmailQueuesV2[0].NotificationType);
            #endregion Assert
        }

        /// <summary>
        /// At Approval Stage, so only Bender gets notified
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedEmailPrefs4()
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
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedEmailPrefs5()
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
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, order.EmailQueuesV2.Count);
            #endregion Assert
        }


        #endregion OrderApproved At Approver status Tests
    }
}
