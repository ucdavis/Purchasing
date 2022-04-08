using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

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
            new FakeEmailPreferences(0, EmailPreferenceRepository);
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
            Assert.AreEqual("By Homer Simpson at Approver review.", order.EmailQueuesV2[0].Details);
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
            new FakeEmailPreferences(0, EmailPreferenceRepository);
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
            Assert.AreEqual("By Homer Simpson at Approver review.", order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);

            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Account Manager) for review from Homer Simpson.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("flanders", order.EmailQueuesV2[1].User.Id);

            Assert.AreEqual("Arrived", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("At your level (Account Manager) for review from Homer Simpson.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("AccMan".ToLower(), order.EmailQueuesV2[2].User.Id);
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
            emailPrefs.Add(new EmailPreferences("AccMan".ToLower()));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].AccountManagerOrderArrive = false;


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
            Assert.AreEqual("By Homer Simpson at Approver review.", order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);

            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Account Manager) for review from Homer Simpson.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("flanders", order.EmailQueuesV2[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Account Manager) for review from Homer Simpson.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[2].Text);
            //Assert.AreEqual("AccMan", order.EmailQueuesV2[2].User.Id);
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
            emailPrefs.Add(new EmailPreferences("AccMan".ToLower()));
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
            Assert.AreEqual(1, order.EmailQueuesV2.Count);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            Assert.IsNull(order.EmailQueuesV2[0].Status);

            Assert.AreEqual("Approved", order.EmailQueuesV2[0].Action);
            Assert.AreEqual("By Homer Simpson at Approver review.", order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Account Manager) for review from Homer Simpson.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[1].Text);
            //Assert.AreEqual("flanders", order.EmailQueuesV2[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Account Manager) for review from Homer Simpson.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[2].Text);
            //Assert.AreEqual("AccMan", order.EmailQueuesV2[2].User.Id);
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
            emailPrefs.Add(new EmailPreferences("AccMan".ToLower()));
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
            Assert.AreEqual(0, order.EmailQueuesV2.Count);
            //Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, order.EmailQueuesV2[0].DateTimeCreated.Date);
            //Assert.IsNull(order.EmailQueuesV2[0].DateTimeSent);
            //Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, order.EmailQueuesV2[0].NotificationType);
            //Assert.IsTrue(order.EmailQueuesV2[0].Pending);
            //Assert.IsNull(order.EmailQueuesV2[0].Status);
            //Assert.AreEqual(string.Format("Order request {0} has been approved by Homer Simpson at Approver review.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Account Manager) for review from Homer Simpson.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[1].Text);
            //Assert.AreEqual("flanders", order.EmailQueuesV2[1].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Account Manager) for review from Homer Simpson.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[2].Text);
            //Assert.AreEqual("AccMan", order.EmailQueuesV2[2].User.Id);
            #endregion Assert
        }
    }
}
