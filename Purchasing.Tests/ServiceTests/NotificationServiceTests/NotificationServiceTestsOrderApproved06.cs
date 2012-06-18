using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Rhino.Mocks;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests.NotificationServiceTests
{
    public partial class NotificationServiceTests
    {

        /// <summary>
        /// Still has more Approvals not yet completed.
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedWhenApproved01()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;


            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.SetIdTo(order.Id);
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = new User("Blah");
            conditionalApproval.SecondaryUser = new User("Blah");
            approvals.Add(conditionalApproval);

            var AccManage = new Approval();
            AccManage.Order = new Order();
            AccManage.Order.SetIdTo(order.Id);
            AccManage.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.AccountManager);
            AccManage.User = new User("AccMan");
            AccManage.SecondaryUser = new User("AccMan");
            approvals.Add(AccManage);

            var pur = new Approval();
            pur.Order = new Order();
            pur.Order.SetIdTo(order.Id);
            pur.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.AccountManager);
            pur.User = new User("pur");
            pur.SecondaryUser = new User("pur");
            approvals.Add(pur);
            
            //var saveLevel = 0;
            foreach(var app in approvals)
            {
                //if(app.User != null && app.User.Id == "hsimpson")
                //{
                //    saveLevel = app.StatusCode.Level;
                //    app.User = null;
                //    app.SecondaryUser = null;
                //}
                order.AddApproval(app);
            }
            order.GenerateRequestNumber();
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.IsTrue(order.EmailQueues[0].Pending);
            Assert.IsNull(order.EmailQueues[0].Status);
            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Homer Simpson at Approver review.", order.EmailQueues[0].Text);
            #endregion Assert		
        }

        /// <summary>
        /// All approvals done 
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedWhenApproved02()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;


            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.SetIdTo(order.Id);
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = new User("Blah");
            conditionalApproval.SecondaryUser = new User("Blah");
            approvals.Add(conditionalApproval);

            var AccManage = new Approval();
            AccManage.Order = new Order();
            AccManage.Order.SetIdTo(order.Id);
            AccManage.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.AccountManager);
            AccManage.User = new User("AccMan");
            AccManage.SecondaryUser = null;
            approvals.Add(AccManage);

            var pur = new Approval();
            pur.Order = new Order();
            pur.Order.SetIdTo(order.Id);
            pur.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Purchaser);
            pur.User = new User("pur");
            pur.SecondaryUser = null;
            approvals.Add(pur);

            foreach(var app in approvals)
            {
                if(app.StatusCode.Id == OrderStatusCode.Codes.Approver)
                {
                    app.Completed = true;
                }
                order.AddApproval(app);
            }
            order.GenerateRequestNumber();
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
            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Homer Simpson at Approver review.", order.EmailQueues[0].Text);
            Assert.AreEqual("bender", order.EmailQueues[0].User.Id);

            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has arrived at your level (Account Manager) for review from Homer Simpson.", order.EmailQueues[1].Text);
            Assert.AreEqual("flanders", order.EmailQueues[1].User.Id);

            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has arrived at your level (Account Manager) for review from Homer Simpson.", order.EmailQueues[2].Text);
            Assert.AreEqual("AccMan", order.EmailQueues[2].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// All approvals done , but email prefs
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedWhenApproved03()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;


            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.SetIdTo(order.Id);
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = new User("Blah");
            conditionalApproval.SecondaryUser = new User("Blah");
            approvals.Add(conditionalApproval);

            var AccManage = new Approval();
            AccManage.Order = new Order();
            AccManage.Order.SetIdTo(order.Id);
            AccManage.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.AccountManager);
            AccManage.User = new User("AccMan");
            AccManage.SecondaryUser = null;            
            approvals.Add(AccManage);

            var pur = new Approval();
            pur.Order = new Order();
            pur.Order.SetIdTo(order.Id);
            pur.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Purchaser);
            pur.User = new User("pur");
            pur.SecondaryUser = null;
            approvals.Add(pur);

            foreach(var app in approvals)
            {
                if(app.StatusCode.Id == OrderStatusCode.Codes.Approver)
                {
                    app.Completed = true;
                }
                order.AddApproval(app);
            }
            order.GenerateRequestNumber();


            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("AccMan"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].AccountManagerOrderArrive = false;


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
            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Homer Simpson at Approver review.", order.EmailQueues[0].Text);
            Assert.AreEqual("bender", order.EmailQueues[0].User.Id);

            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has arrived at your level (Account Manager) for review from Homer Simpson.", order.EmailQueues[1].Text);
            Assert.AreEqual("flanders", order.EmailQueues[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Account Manager) for review from Homer Simpson.", "#testOrg-FT1P9YR"), order.EmailQueues[2].Text);
            //Assert.AreEqual("AccMan", order.EmailQueues[2].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// All approvals done , but email prefs
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedWhenApproved04()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;


            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.SetIdTo(order.Id);
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = new User("Blah");
            conditionalApproval.SecondaryUser = new User("Blah");
            approvals.Add(conditionalApproval);

            var AccManage = new Approval();
            AccManage.Order = new Order();
            AccManage.Order.SetIdTo(order.Id);
            AccManage.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.AccountManager);
            AccManage.User = new User("AccMan");
            AccManage.SecondaryUser = null;
            approvals.Add(AccManage);

            var pur = new Approval();
            pur.Order = new Order();
            pur.Order.SetIdTo(order.Id);
            pur.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Purchaser);
            pur.User = new User("pur");
            pur.SecondaryUser = null;
            approvals.Add(pur);

            foreach(var app in approvals)
            {
                if(app.StatusCode.Id == OrderStatusCode.Codes.Approver)
                {
                    app.Completed = true;
                }
                order.AddApproval(app);
            }
            order.GenerateRequestNumber();


            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("AccMan"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].AccountManagerOrderArrive = false;

            emailPrefs.Add(new EmailPreferences("flanders"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[1].AccountManagerOrderArrive = false;

            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, order.EmailQueues.Count);
            Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            Assert.IsTrue(order.EmailQueues[0].Pending);
            Assert.IsNull(order.EmailQueues[0].Status);

            Assert.AreEqual("Order request <a href=\"FakeHosttestOrg-FT1P9YR\">testOrg-FT1P9YR</a> for Unspecified Vendor has been approved by Homer Simpson at Approver review.", order.EmailQueues[0].Text);
            Assert.AreEqual("bender", order.EmailQueues[0].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Account Manager) for review from Homer Simpson.", "#testOrg-FT1P9YR"), order.EmailQueues[1].Text);
            //Assert.AreEqual("flanders", order.EmailQueues[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Account Manager) for review from Homer Simpson.", "#testOrg-FT1P9YR"), order.EmailQueues[2].Text);
            //Assert.AreEqual("AccMan", order.EmailQueues[2].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// All approvals done , but email prefs
        /// </summary>
        [TestMethod]
        public void TestOrderApprovedWhenApproved05()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);
            var approval = new Approval();
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            approval.Completed = true;


            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.SetIdTo(order.Id);
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = new User("Blah");
            conditionalApproval.SecondaryUser = new User("Blah");
            approvals.Add(conditionalApproval);

            var AccManage = new Approval();
            AccManage.Order = new Order();
            AccManage.Order.SetIdTo(order.Id);
            AccManage.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.AccountManager);
            AccManage.User = new User("AccMan");
            AccManage.SecondaryUser = null;
            approvals.Add(AccManage);

            var pur = new Approval();
            pur.Order = new Order();
            pur.Order.SetIdTo(order.Id);
            pur.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Purchaser);
            pur.User = new User("pur");
            pur.SecondaryUser = null;
            approvals.Add(pur);

            foreach(var app in approvals)
            {
                if(app.StatusCode.Id == OrderStatusCode.Codes.Approver)
                {
                    app.Completed = true;
                }
                order.AddApproval(app);
            }
            order.GenerateRequestNumber();


            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("AccMan"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].AccountManagerOrderArrive = false;

            emailPrefs.Add(new EmailPreferences("flanders"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[1].AccountManagerOrderArrive = false;

            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[2].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[2].RequesterApproverApproved = false;

            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            #endregion Arrange

            #region Act
            NotificationService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, order.EmailQueues.Count);
            //Assert.AreEqual(DateTime.Now.Date, order.EmailQueues[0].DateTimeCreated.Date);
            //Assert.IsNull(order.EmailQueues[0].DateTimeSent);
            //Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueues[0].NotificationType);
            //Assert.IsTrue(order.EmailQueues[0].Pending);
            //Assert.IsNull(order.EmailQueues[0].Status);
            //Assert.AreEqual(string.Format("Order request {0} has been approved by Homer Simpson at Approver review.", "#testOrg-FT1P9YR"), order.EmailQueues[0].Text);
            //Assert.AreEqual("bender", order.EmailQueues[0].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Account Manager) for review from Homer Simpson.", "#testOrg-FT1P9YR"), order.EmailQueues[1].Text);
            //Assert.AreEqual("flanders", order.EmailQueues[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Account Manager) for review from Homer Simpson.", "#testOrg-FT1P9YR"), order.EmailQueues[2].Text);
            //Assert.AreEqual("AccMan", order.EmailQueues[2].User.Id);
            #endregion Assert
        }
    }
}
