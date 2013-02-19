﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Rhino.Mocks;

namespace Purchasing.Tests.ServiceTests.NotificationServiceTests
{
    public partial class NotificationServiceTests
    {
        #region OrderApproved At Kuali status Tests
        [TestMethod]
        public void TestOrderApprovedKualiNoEmailPrefs1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete);
            approval.Completed = true;
            order.GenerateRequestNumber();
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(4, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);
            Assert.AreEqual("Approved", order.EmailQueuesV2[0].Action);
            Assert.AreEqual("By Amy Wong at Complete review.", order.EmailQueuesV2[0].Details);

            Assert.AreEqual(DateTime.Now.Date, order.EmailQueuesV2[1].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[1].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[1].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[1].Pending);
            Assert.IsNull(order.EmailQueuesV2[1].Status);
            Assert.AreEqual("Approved", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("By Amy Wong at Complete review.", order.EmailQueuesV2[1].Details);

            Assert.AreEqual(DateTime.Now.Date, order.EmailQueuesV2[2].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[2].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[2].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[2].Pending);
            Assert.IsNull(order.EmailQueuesV2[2].Status);
            Assert.AreEqual("Approved", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("By Amy Wong at Complete review.", order.EmailQueuesV2[2].Details);

            Assert.AreEqual(DateTime.Now.Date, order.EmailQueuesV2[3].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[3].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[3].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[3].Pending);
            Assert.IsNull(order.EmailQueuesV2[3].Status);
            Assert.AreEqual("Approved", order.EmailQueuesV2[3].Action);
            Assert.AreEqual("By Amy Wong at Complete review.", order.EmailQueuesV2[3].Details);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedKualiEmailPrefs1A()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs.Add(new EmailPreferences("flanders"));
            emailPrefs.Add(new EmailPreferences("awong"));
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(4, order.EmailQueuesV2.Count);
            #endregion Assert
        }

        /// <summary>
        /// Same as A above, but no prefs
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedKualiEmailPrefs1B()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();

            //var emailPrefs = new List<EmailPreferences>();
            //emailPrefs.Add(new EmailPreferences("bender"));
            //emailPrefs.Add(new EmailPreferences("hsimpson"));
            //emailPrefs.Add(new EmailPreferences("flanders"));
            //emailPrefs.Add(new EmailPreferences("awong"));
            //new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Complete);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(4, order.EmailQueuesV2.Count);
            #endregion Assert
        }

        #endregion OrderApproved At Kuali status Tests
    }
}
