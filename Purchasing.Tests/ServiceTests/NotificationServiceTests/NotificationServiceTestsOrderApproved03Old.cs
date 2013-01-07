using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Rhino.Mocks;

namespace Purchasing.Tests.ServiceTests.NotificationServiceTests
{
    public partial class NotificationServiceTests
    {
        #region OrderApproved At Purchaser status Tests
        /// <summary>
        /// At Purchaser Stage, so only Bender, homer, and flanders get notified
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedPurchaserNoEmailPrefs1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
            approval.Completed = true;
            order.GenerateRequestNumber();
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, order.EmailQueues.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.IsTrue(order.EmailQueues[0].Pending);
            Assert.IsNull(order.EmailQueues[0].Status);
            Assert.AreEqual("Order request <a href=\"http://prepurchasing.ucdavis.edu/Order/Lookup/testOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Purchaser review.", order.EmailQueues[0].Text);

            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[1].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[1].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[1].NotificationType);
            Assert.IsTrue(order.EmailQueues[1].Pending);
            Assert.IsNull(order.EmailQueues[1].Status);
            Assert.AreEqual("Order request <a href=\"http://prepurchasing.ucdavis.edu/Order/Lookup/testOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Purchaser review.", order.EmailQueues[1].Text);

            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[2].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[2].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[2].NotificationType);
            Assert.IsTrue(order.EmailQueues[2].Pending);
            Assert.IsNull(order.EmailQueues[2].Status);
            Assert.AreEqual("Order request <a href=\"http://prepurchasing.ucdavis.edu/Order/Lookup/testOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Purchaser review.", order.EmailQueues[2].Text);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedPurchaserEmailPrefs1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs.Add(new EmailPreferences("flanders"));
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[1].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[2].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedPurchaserEmailPrefs2()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs.Add(new EmailPreferences("flanders"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[2].NotificationType = EmailPreferences.NotificationTypes.Weekly;
            emailPrefs[0].RequesterApproverApproved = false; //no effect here
            emailPrefs[0].RequesterAccountManagerApproved = false; //no effect here
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, order.EmailQueues[1].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, order.EmailQueues[2].NotificationType);
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderApprovedPurchaserEmailPrefs4()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs.Add(new EmailPreferences("flanders"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[2].NotificationType = EmailPreferences.NotificationTypes.Weekly;
            emailPrefs[0].ApproverPurchaserProcessed = false; //no effect, this is the requester 
            emailPrefs[1].ApproverPurchaserProcessed = false;
            emailPrefs[2].ApproverPurchaserProcessed = false; //no effect, this is the account manager
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, order.EmailQueues[1].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedPurchaserEmailPrefs5()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs.Add(new EmailPreferences("flanders"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[2].NotificationType = EmailPreferences.NotificationTypes.Weekly;
            emailPrefs[0].AccountManagerPurchaserProcessed = false; //no effect, this is the requester 
            emailPrefs[1].AccountManagerPurchaserProcessed = false; //no effect, this is the approver
            emailPrefs[2].AccountManagerPurchaserProcessed = false; 
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, order.EmailQueues[1].NotificationType);
            #endregion Assert
        }

        #endregion OrderApproved At Purchaser status Tests
    }
}
