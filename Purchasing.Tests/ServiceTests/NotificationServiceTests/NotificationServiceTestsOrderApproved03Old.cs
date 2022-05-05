using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;

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
            Moq.Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("awong");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
            approval.Completed = true;
            order.GenerateRequestNumber();
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);
            Assert.AreEqual("Approved", order.EmailQueuesV2[0].Action);
            Assert.AreEqual("By Amy Wong at Purchaser review.", order.EmailQueuesV2[0].Details);

            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[1].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[1].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[1].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[1].Pending);
            Assert.IsNull(order.EmailQueuesV2[1].Status);
            Assert.AreEqual("Approved", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("By Amy Wong at Purchaser review.", order.EmailQueuesV2[1].Details);

            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[2].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[2].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[2].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[2].Pending);
            Assert.IsNull(order.EmailQueuesV2[2].Status);
            Assert.AreEqual("Approved", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("By Amy Wong at Purchaser review.", order.EmailQueuesV2[2].Details);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedPurchaserEmailPrefs1()
        {
            #region Arrange
            Moq.Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("awong");
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
            Assert.AreEqual(3, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[1].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[2].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedPurchaserEmailPrefs2()
        {
            #region Arrange
            Moq.Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("awong");
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
            Assert.AreEqual(3, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, order.EmailQueuesV2[1].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, order.EmailQueuesV2[2].NotificationType);
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderApprovedPurchaserEmailPrefs4()
        {
            #region Arrange
            Moq.Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("awong");
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
            Assert.AreEqual(2, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, order.EmailQueuesV2[1].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedPurchaserEmailPrefs5()
        {
            #region Arrange
            Moq.Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("awong");
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
            Assert.AreEqual(2, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, order.EmailQueuesV2[1].NotificationType);
            #endregion Assert
        }

        #endregion OrderApproved At Purchaser status Tests
    }
}
