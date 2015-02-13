using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using Rhino.Mocks;
using UCDArch.Testing;
using System.Linq;
using System.Linq.Expressions;

namespace Purchasing.Tests.ServiceTests.NotificationServiceTests
{
    /// <summary>
    /// Approved at account manager stage.
    /// </summary>
    public partial class NotificationServiceTests
    {
        [TestMethod]
        public void TestOrderApprovedWhenApprovedFromAccManager01()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("flanders");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
            approval.Completed = true;


            order.Approvals = ApprovalRepository.Queryable.Where(a => a.Order.Id == order.Id).ToList();
            foreach (var approval1 in order.Approvals)
            {
                if (approval1.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
                {
                    approval1.Completed = true;
                }
            }
            order.GenerateRequestNumber();
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
            Assert.AreEqual("By Ned Flanders at Account Manager review.", order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);

            Assert.AreEqual("Approved", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("By Ned Flanders at Account Manager review.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("hsimpson", order.EmailQueuesV2[1].User.Id);

            Assert.AreEqual("Arrived", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("At your level (Purchaser) for review from Ned Flanders.", order.EmailQueuesV2[2].Details);
            Assert.AreEqual("awong", order.EmailQueuesV2[2].User.Id);

            #endregion Assert
        }

        /// <summary>
        /// Email Prefs
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedWhenApprovedFromAccManager02()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("flanders");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
            approval.Completed = true;


            order.Approvals = ApprovalRepository.Queryable.Where(a => a.Order.Id == order.Id).ToList();
            foreach(var approval1 in order.Approvals)
            {
                if(approval1.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
                {
                    approval1.Completed = true;
                }
            }
            order.GenerateRequestNumber();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("awong"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].PurchaserOrderArrive = false;


            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
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
            Assert.AreEqual("By Ned Flanders at Account Manager review.", order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);

            Assert.AreEqual("Approved", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("By Ned Flanders at Account Manager review.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("hsimpson", order.EmailQueuesV2[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Purchaser) for review from Ned Flanders.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[2].Text);
            //Assert.AreEqual("awong", order.EmailQueuesV2[2].User.Id);

            #endregion Assert
        }

        /// <summary>
        /// Email Prefs
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedWhenApprovedFromAccManager03()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("flanders");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
            approval.Completed = true;


            order.Approvals = ApprovalRepository.Queryable.Where(a => a.Order.Id == order.Id).ToList();
            foreach(var approval1 in order.Approvals)
            {
                if(approval1.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
                {
                    approval1.Completed = true;
                }
            }
            order.GenerateRequestNumber();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("awong"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].PurchaserOrderArrive = false;

            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[1].ApproverAccountManagerApproved = false;


            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
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
            Assert.AreEqual("By Ned Flanders at Account Manager review.", order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has been approved by Ned Flanders at Account Manager review.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[1].Text);
            //Assert.AreEqual("hsimpson", order.EmailQueuesV2[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Purchaser) for review from Ned Flanders.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[2].Text);
            //Assert.AreEqual("awong", order.EmailQueuesV2[2].User.Id);

            #endregion Assert
        }

        /// <summary>
        /// Email Prefs
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedWhenApprovedFromAccManager04()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("flanders");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
            approval.Completed = true;


            order.Approvals = ApprovalRepository.Queryable.Where(a => a.Order.Id == order.Id).ToList();
            foreach(var approval1 in order.Approvals)
            {
                if(approval1.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
                {
                    approval1.Completed = true;
                }
            }
            order.GenerateRequestNumber();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("awong"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].PurchaserOrderArrive = false;

            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[1].ApproverAccountManagerApproved = false;

            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[2].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[2].RequesterAccountManagerApproved = false;


            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, order.EmailQueuesV2.Count);
            //Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            //Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            //Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            //Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            //Assert.IsNull(order.EmailQueuesV2[0].Status);
            //Assert.AreEqual(string.Format("Order request {0} has been approved by Ned Flanders at Account Manager review.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has been approved by Ned Flanders at Account Manager review.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[1].Text);
            //Assert.AreEqual("hsimpson", order.EmailQueuesV2[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Purchaser) for review from Ned Flanders.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[2].Text);
            //Assert.AreEqual("awong", order.EmailQueuesV2[2].User.Id);

            #endregion Assert
        }
    }
}
