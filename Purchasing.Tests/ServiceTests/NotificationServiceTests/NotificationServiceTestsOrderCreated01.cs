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
        #region OrderCreated Tests

        [TestMethod]
        public void TestOrderCreatedNoPrefs1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = false;
            order.AddApproval(approval);
            order.GenerateRequestNumber();
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            #endregion Assert		
        }

        [TestMethod]
        public void TestOrderCreatedPrefs1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = false;
            order.AddApproval(approval);

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            order.GenerateRequestNumber();
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCreatedPrefs2()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = false;
            order.AddApproval(approval);

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.PerEvent;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            order.GenerateRequestNumber();
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCreatedPrefs3()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = false;
            order.AddApproval(approval);

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            order.GenerateRequestNumber();
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCreatedPrefs4()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = false;
            order.AddApproval(approval);

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Weekly;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            order.GenerateRequestNumber();

            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCreatedPrefs5()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = false;
            order.AddApproval(approval);

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].RequesterOrderSubmission = false;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, order.EmailQueuesV2.Count);
            #endregion Assert
        } 
        #endregion OrderCreated Tests
    }
}
