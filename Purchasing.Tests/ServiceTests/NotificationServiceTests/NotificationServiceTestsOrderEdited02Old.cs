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
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager);
            approval.Completed = true;
            order.GenerateRequestNumber();
            #endregion Arrange

            #region Act
            NotificationService.OrderEdited(order, user);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.IsTrue(order.EmailQueues[0].Pending);
            Assert.IsNull(order.EmailQueues[0].Status);
            Assert.AreEqual("Order request FakeHosttestOrg-FT1P9YRtestOrg-FT1P9YR for Unspecified Vendor has been changed by Homer Simpson.", order.EmailQueues[0].Text);
            #endregion Assert
        }


        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Continue these tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion OrderEdited At AccountManager Status Tests
    }
}
