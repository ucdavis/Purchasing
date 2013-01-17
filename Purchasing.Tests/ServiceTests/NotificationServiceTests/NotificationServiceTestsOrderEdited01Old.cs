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
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, UserRepository.GetNullableById("hsimpson"));
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.IsTrue(order.EmailQueues[0].Pending);
            Assert.IsNull(order.EmailQueues[0].Status);
            Assert.AreEqual("Order request <a href=\"http://prepurchasing.ucdavis.edu/Order/Lookup/testOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been changed by Homer Simpson.", order.EmailQueues[0].Text);
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
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, UserRepository.GetNullableById("hsimpson"));
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.IsTrue(order.EmailQueues[0].User != null || !string.IsNullOrWhiteSpace(order.EmailQueues[0].Email));
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
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
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
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, order.EmailQueues[0].NotificationType);
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
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, order.EmailQueues[0].NotificationType);
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
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
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
            Assert.AreEqual(0, order.EmailQueues.Count);
            #endregion Assert
        }
        #endregion OrderEdited At Approver status Tests
    }
}
