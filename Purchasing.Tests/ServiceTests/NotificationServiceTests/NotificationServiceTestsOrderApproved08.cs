using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
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
        public void TestOrderApprovedWhenApprovedFromPurchaser01()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
            approval.Completed = true;


            order.Approvals = ApprovalRepository.Queryable.Where(a => a.Order.Id == order.Id).ToList();
            foreach(var approval1 in order.Approvals)
            {
                if(approval1.StatusCode.Id == OrderStatusCode.Codes.Purchaser)
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
            Assert.AreEqual(4, order.EmailQueues.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.IsTrue(order.EmailQueues[0].Pending);
            Assert.IsNull(order.EmailQueues[0].Status);
            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Purchaser review.", order.EmailQueues[0].Text);
            Assert.AreEqual("bender", order.EmailQueues[0].User.Id);

            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Purchaser review.", order.EmailQueues[1].Text);
            Assert.AreEqual("hsimpson", order.EmailQueues[1].User.Id);

            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Purchaser review.", order.EmailQueues[2].Text);
            Assert.AreEqual("flanders", order.EmailQueues[2].User.Id);

            //Don't care about this, just an artifact of the test data
            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Complete-Not Uploaded KFS) for review from Amy Wong.", "#testOrg-FT1P9YR"), order.EmailQueues[3].Text);
            //Assert.AreEqual("flanders", order.EmailQueues[3].User.Id);

            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedWhenApprovedFromPurchaser02()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
            approval.Completed = true;


            order.Approvals = ApprovalRepository.Queryable.Where(a => a.Order.Id == order.Id).ToList();
            foreach(var approval1 in order.Approvals)
            {
                if(approval1.StatusCode.Id == OrderStatusCode.Codes.Purchaser)
                {
                    approval1.Completed = true;
                }
            }
            order.GenerateRequestNumber();
            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("flanders"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].AccountManagerPurchaserProcessed = false;

            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
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
            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Purchaser review.", order.EmailQueues[0].Text);
            Assert.AreEqual("bender", order.EmailQueues[0].User.Id);

            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Purchaser review.", order.EmailQueues[1].Text);
            Assert.AreEqual("hsimpson", order.EmailQueues[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has been approved by Amy Wong at Purchaser review.", "#testOrg-FT1P9YR"), order.EmailQueues[2].Text);
            //Assert.AreEqual("flanders", order.EmailQueues[2].User.Id);

            #endregion Assert
        }

        [TestMethod]
        public void TestOrderApprovedWhenApprovedFromPurchaser03()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("awong");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser);
            approval.Completed = true;


            order.Approvals = ApprovalRepository.Queryable.Where(a => a.Order.Id == order.Id).ToList();
            foreach(var approval1 in order.Approvals)
            {
                if(approval1.StatusCode.Id == OrderStatusCode.Codes.Purchaser)
                {
                    approval1.Completed = true;
                }
            }
            order.GenerateRequestNumber();
            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("flanders"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].AccountManagerPurchaserProcessed = false;

            emailPrefs.Add(new EmailPreferences("hsimpson"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[1].ApproverPurchaserProcessed = false;

            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueues.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.IsTrue(order.EmailQueues[0].Pending);
            Assert.IsNull(order.EmailQueues[0].Status);
            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Amy Wong at Purchaser review.", order.EmailQueues[0].Text);
            Assert.AreEqual("bender", order.EmailQueues[0].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has been approved by Amy Wong at Purchaser review.", "#testOrg-FT1P9YR"), order.EmailQueues[1].Text);
            //Assert.AreEqual("hsimpson", order.EmailQueues[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has been approved by Amy Wong at Purchaser review.", "#testOrg-FT1P9YR"), order.EmailQueues[2].Text);
            //Assert.AreEqual("flanders", order.EmailQueues[2].User.Id);

            #endregion Assert
        }

    }
}
