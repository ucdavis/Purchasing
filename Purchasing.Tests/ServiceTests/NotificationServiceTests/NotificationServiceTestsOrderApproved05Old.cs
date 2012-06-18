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
            Assert.AreEqual(4, order.EmailQueues.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.IsTrue(order.EmailQueues[0].Pending);
            Assert.IsNull(order.EmailQueues[0].Status);
            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Complete review.", order.EmailQueues[0].Text);

            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[1].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[1].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[1].NotificationType);
            Assert.IsTrue(order.EmailQueues[1].Pending);
            Assert.IsNull(order.EmailQueues[1].Status);
            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Complete review.", order.EmailQueues[1].Text);

            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[2].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[2].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[2].NotificationType);
            Assert.IsTrue(order.EmailQueues[2].Pending);
            Assert.IsNull(order.EmailQueues[2].Status);
            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Complete review.", order.EmailQueues[2].Text);

            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[3].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[3].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[3].NotificationType);
            Assert.IsTrue(order.EmailQueues[3].Pending);
            Assert.IsNull(order.EmailQueues[3].Status);
            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Complete review.", order.EmailQueues[3].Text);
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
            Assert.AreEqual(4, order.EmailQueues.Count);
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
            Assert.AreEqual(4, order.EmailQueues.Count);
            #endregion Assert
        }

        #endregion OrderApproved At Kuali status Tests
    }
}
