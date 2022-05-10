using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using Moq;

namespace Purchasing.Tests.ServiceTests.NotificationServiceTests
{
    public partial class NotificationServiceTests
    {
        #region  OrderApproved At AccountManager status Tests
        /// <summary>
        /// At Account Manager Stage, so only Bender and homer get notified
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedAccountManagerNoEmailPrefs1()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("burns");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
            approval.Completed = true;
            order.GenerateRequestNumber();
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);
            Assert.AreEqual("Approved", order.EmailQueuesV2[0].Action);
            Assert.AreEqual("By Monty Burns at Account Manager review.", order.EmailQueuesV2[0].Details);

            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[1].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[1].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[1].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[1].Pending);
            Assert.IsNull(order.EmailQueuesV2[1].Status);
            Assert.AreEqual("Approved", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("By Monty Burns at Account Manager review.", order.EmailQueuesV2[1].Details);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedAccountManagerEmailPrefs1()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("burns");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs.Add(new EmailPreferences("hsimpson"));
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            order.GenerateRequestNumber();
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[1].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedAccountManagerEmailPrefs2()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("burns");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Weekly;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, order.EmailQueuesV2[0].NotificationType);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, order.EmailQueuesV2[1].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedAccountManagerEmailPrefs3()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("burns");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].RequesterAccountManagerApproved = false;
            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Weekly;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
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

        [TestMethod]
        public void TestOrderApprovedAccountManagerEmailPrefs4()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("burns");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));            
            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs[1].ApproverAccountManagerApproved = false;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
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
        public void TestOrderApprovedAccountManagerEmailPrefs5()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("burns");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].RequesterAccountManagerApproved = false;
            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs[1].ApproverAccountManagerApproved = false;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, order.EmailQueuesV2.Count);
            #endregion Assert
        }
        #endregion  OrderApproved At AccountManager status Tests
    }
}
