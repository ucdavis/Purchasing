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
        #region OrderEdited At Approver status Tests
        [TestMethod]
        public void TestOrderEditedNoEmailPrefs1()
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
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, UserRepository.GetNullableById("hsimpson"));
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);
            Assert.AreEqual("Changed", order.EmailQueuesV2[0].Action);
            Assert.AreEqual("By Homer Simpson at Approver review.", order.EmailQueuesV2[0].Details);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderEditedShouldCreateEmailQueueWithUserOrEmail()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, UserRepository.GetNullableById("hsimpson"));
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.IsTrue(order.EmailQueuesV2[0].User != null || !string.IsNullOrWhiteSpace(order.EmailQueuesV2[0].Email));
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderEditedEmailPrefs1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var user = UserRepository.GetNullableById("hsimpson");
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, user);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderEditedEmailPrefs2()
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
            var user = UserRepository.GetNullableById("hsimpson");
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, user);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, order.EmailQueuesV2[0].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderEditedEmailPrefs3()
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
            var user = UserRepository.GetNullableById("hsimpson");
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, user);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, order.EmailQueuesV2[0].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderEditedEmailPrefs4()
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
            var user = UserRepository.GetNullableById("hsimpson");
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, user);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderEditedEmailPrefs5()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].RequesterApproverChanged = false;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var user = UserRepository.GetNullableById("hsimpson");
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, user);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, order.EmailQueuesV2.Count);
            #endregion Assert
        }
        #endregion OrderEdited At Approver status Tests
    }
}
