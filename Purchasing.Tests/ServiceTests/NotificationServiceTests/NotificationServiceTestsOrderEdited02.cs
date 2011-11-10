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
        #region OrderEdited At AccountManager Status Tests
        [TestMethod]
        public void TestOrderEditedAccountManagerNoEmailPrefs1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("burns");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var user = UserRepository.GetNullableById("hsimpson");
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, user);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueues.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.IsTrue(order.EmailQueues[0].Pending);
            Assert.IsNull(order.EmailQueues[0].Status);
            Assert.AreEqual(string.Format("Order request {0}, has been approved by Monty Burns at Account Manager review.", "#111231-000001"), order.EmailQueues[0].Text);

            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[1].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[1].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[1].NotificationType);
            Assert.IsTrue(order.EmailQueues[1].Pending);
            Assert.IsNull(order.EmailQueues[1].Status);
            Assert.AreEqual(string.Format("Order request {0}, has been approved by Monty Burns at Account Manager review.", "#111231-000001"), order.EmailQueues[1].Text);
            #endregion Assert
        }
        #endregion OrderEdited At AccountManager Status Tests
    }
}
