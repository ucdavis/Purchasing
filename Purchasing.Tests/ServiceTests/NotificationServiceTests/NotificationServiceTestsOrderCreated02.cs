﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using System.Linq;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Moq;

namespace Purchasing.Tests.ServiceTests.NotificationServiceTests
{
    public partial class NotificationServiceTests
    {
        /*
         * Test approvals when approver is null
         * Test when approver is away and secondary user is null
         * Test when approver is away and secondary user is away
         * test when approver is away and secondary user is not away
         * test when approver is not away
         * test when conditional approver is away - > secondary user
         * test when conditional approver is not away and secondary user is not away
         * 
         */ 

        /// <summary>
        /// Approver is not null and not away
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival1()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);

            foreach (var approval in approvals)
            {
                order.AddApproval(approval);
            }
            order.GenerateRequestNumber();
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueuesV2.Count());
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("hsimpson", order.EmailQueuesV2[1].User.Id);
            #endregion Assert		
        }

        /// <summary>
        /// Approver is not null and is away, secondary is not away or null
        /// Both get notified
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival2()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);

            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    approval.User.IsAway = true;
                    approval.SecondaryUser = UserRepository.Queryable.Single(a => a.Id == "awong");
                }
                order.AddApproval(approval);
            }
            order.GenerateRequestNumber();
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, order.EmailQueuesV2.Count());
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("hsimpson", order.EmailQueuesV2[1].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[2].Details);
            Assert.AreEqual("awong", order.EmailQueuesV2[2].User.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCreatedProcessArrival3()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);

            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.Id = order.Id;
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("zoidberg");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("flanders");
            approvals.Add(conditionalApproval);

            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    approval.User.IsAway = true;
                    approval.SecondaryUser = UserRepository.Queryable.Single(a => a.Id == "awong");
                }
                order.AddApproval(approval);
            }
            order.GenerateRequestNumber();
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, order.EmailQueuesV2.Count());
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("hsimpson", order.EmailQueuesV2[1].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[2].Details);
            Assert.AreEqual("awong", order.EmailQueuesV2[2].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[3].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[3].Details);
            Assert.AreEqual("zoidberg", order.EmailQueuesV2[3].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[4].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[4].Details);
            Assert.AreEqual("flanders", order.EmailQueuesV2[4].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// Multiple approvers that are the same.
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival4()
        {
            #region Arrange
            Mock.Get(UserIdentity).Setup(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);

            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.Id = order.Id;
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("hsimpson");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("hsimpson");
            approvals.Add(conditionalApproval);

            foreach(var approval in approvals)
            {
                order.AddApproval(approval);
            }
            order.GenerateRequestNumber();
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueuesV2.Count());
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("hsimpson", order.EmailQueuesV2[1].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// Approver is null, needs to look in workgroup permissions
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival5()
        {
            #region Arrange
            Mock.Get(UserIdentity).Setup(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);

            var saveLevel = 0;
            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    saveLevel = approval.StatusCode.Level;
                    approval.User = null;                    
                    approval.SecondaryUser = null;
                }
                order.AddApproval(approval);
            }
            order.Workgroup.Permissions = new List<WorkgroupPermission>();
            var permission = CreateValidEntities.WorkgroupPermission(1);
            permission.User = new User("Flarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            permission = CreateValidEntities.WorkgroupPermission(2);
            permission.User = new User("Blarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            order.GenerateRequestNumber();

            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, order.EmailQueuesV2.Count());
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("Flarg".ToLower(), order.EmailQueuesV2[1].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[2].Details);
            Assert.AreEqual("Blarg".ToLower(), order.EmailQueuesV2[2].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// Approver is null, needs to look in workgroup permissions. But Also a Conditional Approver
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival6()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.Id = order.Id;
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("zoidberg");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("flanders");
            approvals.Add(conditionalApproval);

            var saveLevel = 0;
            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    saveLevel = approval.StatusCode.Level;
                    approval.User = null;
                    approval.SecondaryUser = null;
                }
                order.AddApproval(approval);
            }
            order.Workgroup.Permissions = new List<WorkgroupPermission>();
            var permission = CreateValidEntities.WorkgroupPermission(1);
            permission.User = new User("Flarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            permission = CreateValidEntities.WorkgroupPermission(2);
            permission.User = new User("Blarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            order.GenerateRequestNumber();
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, order.EmailQueuesV2.Count());
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("Flarg".ToLower(), order.EmailQueuesV2[1].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[2].Details);
            Assert.AreEqual("Blarg".ToLower(), order.EmailQueuesV2[2].User.Id);

            Assert.AreEqual("Arrived", order.EmailQueuesV2[3].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[3].Details);
            Assert.AreEqual("zoidberg", order.EmailQueuesV2[3].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[4].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[4].Details);
            Assert.AreEqual("flanders", order.EmailQueuesV2[4].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// Approver is null, needs to look in workgroup permissions. But Also a Conditional Approver with same user
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival7()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.Id = order.Id;
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("zoidberg");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("zoidberg");
            approvals.Add(conditionalApproval);

            var saveLevel = 0;
            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    saveLevel = approval.StatusCode.Level;
                    approval.User = null;
                    approval.SecondaryUser = null;
                }
                order.AddApproval(approval);
            }
            order.Workgroup.Permissions = new List<WorkgroupPermission>();
            var permission = CreateValidEntities.WorkgroupPermission(1);
            permission.User = new User("Flarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            permission = CreateValidEntities.WorkgroupPermission(2);
            permission.User = new User("Blarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            order.GenerateRequestNumber();
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            new FakeEmailPreferences(0, EmailPreferenceRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(4, order.EmailQueuesV2.Count());
            Assert.AreEqual("Submitted", order.EmailQueuesV2[0].Action);
            Assert.AreEqual(null, order.EmailQueuesV2[0].Details);
            Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("Flarg".ToLower(), order.EmailQueuesV2[1].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[2].Details);
            Assert.AreEqual("Blarg".ToLower(), order.EmailQueuesV2[2].User.Id);

            Assert.AreEqual("Arrived", order.EmailQueuesV2[3].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[3].Details);
            Assert.AreEqual("zoidberg", order.EmailQueuesV2[3].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// Same as above, but some users email prefs say not when order arrives for approval.
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival8()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.Id = order.Id;
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("zoidberg");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("zoidberg");
            approvals.Add(conditionalApproval);

            var saveLevel = 0;
            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    saveLevel = approval.StatusCode.Level;
                    approval.User = null;
                    approval.SecondaryUser = null;
                }
                order.AddApproval(approval);
            }
            order.Workgroup.Permissions = new List<WorkgroupPermission>();
            var permission = CreateValidEntities.WorkgroupPermission(1);
            permission.User = new User("Flarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            permission = CreateValidEntities.WorkgroupPermission(2);
            permission.User = new User("Blarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            order.GenerateRequestNumber();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].RequesterOrderSubmission = false;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, order.EmailQueuesV2.Count());
            //Assert.AreEqual(string.Format("Order request {0} has been submitted.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[0].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[0].Details);
            Assert.AreEqual("Flarg".ToLower(), order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("Blarg".ToLower(), order.EmailQueuesV2[1].User.Id);

            Assert.AreEqual("Arrived", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[2].Details);
            Assert.AreEqual("zoidberg", order.EmailQueuesV2[2].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// Same as above, but some users email prefs say not when order arrives for approval.
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival9()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.Id = order.Id;
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("zoidberg");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("zoidberg");
            approvals.Add(conditionalApproval);

            var saveLevel = 0;
            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    saveLevel = approval.StatusCode.Level;
                    approval.User = null;
                    approval.SecondaryUser = null;
                }
                order.AddApproval(approval);
            }
            order.Workgroup.Permissions = new List<WorkgroupPermission>();
            var permission = CreateValidEntities.WorkgroupPermission(1);
            permission.User = new User("Flarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            permission = CreateValidEntities.WorkgroupPermission(2);
            permission.User = new User("Blarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            order.GenerateRequestNumber();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].RequesterOrderSubmission = false;

            emailPrefs.Add(new EmailPreferences("Flarg"));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[1].RequesterOrderSubmission = false;  //Doesn't Matter, did not create it

            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, order.EmailQueuesV2.Count());
            //Assert.AreEqual(string.Format("Order request {0} has been submitted.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[0].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[0].Details);
            Assert.AreEqual("Flarg".ToLower(), order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("Blarg".ToLower(), order.EmailQueuesV2[1].User.Id);

            Assert.AreEqual("Arrived", order.EmailQueuesV2[2].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[2].Details);
            Assert.AreEqual("zoidberg", order.EmailQueuesV2[2].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// Same as above, but some users email prefs say not when order arrives for approval.
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival10()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.Id = order.Id;
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("zoidberg");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("zoidberg");
            approvals.Add(conditionalApproval);

            var saveLevel = 0;
            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    saveLevel = approval.StatusCode.Level;
                    approval.User = null;
                    approval.SecondaryUser = null;
                }
                order.AddApproval(approval);
            }
            order.Workgroup.Permissions = new List<WorkgroupPermission>();
            var permission = CreateValidEntities.WorkgroupPermission(1);
            permission.User = new User("Flarg".ToLower());
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            permission = CreateValidEntities.WorkgroupPermission(2);
            permission.User = new User("Blarg".ToLower());
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            order.GenerateRequestNumber();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].RequesterOrderSubmission = false;

            emailPrefs.Add(new EmailPreferences("Flarg".ToLower()));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[1].RequesterOrderSubmission = false;  //Doesn't Matter, did not create it
            emailPrefs[1].ApproverOrderArrive = false;

            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);

            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, order.EmailQueuesV2.Count());
            //Assert.AreEqual(string.Format("Order request {0} has been submitted.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("Flarg", order.EmailQueuesV2[0].User.Id);
            Assert.AreEqual("Arrived", order.EmailQueuesV2[0].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[0].Details);
            Assert.AreEqual("Blarg".ToLower(), order.EmailQueuesV2[0].User.Id);

            Assert.AreEqual("Arrived", order.EmailQueuesV2[1].Action);
            Assert.AreEqual("At your level (Approver) for review from Bender Rodriguez.", order.EmailQueuesV2[1].Details);
            Assert.AreEqual("zoidberg", order.EmailQueuesV2[1].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// Same as above, but some users email prefs say not when order arrives for approval.
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival11()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.Id = order.Id;
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("zoidberg");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("zoidberg");
            approvals.Add(conditionalApproval);

            var saveLevel = 0;
            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    saveLevel = approval.StatusCode.Level;
                    approval.User = null;
                    approval.SecondaryUser = null;
                }
                order.AddApproval(approval);
            }
            order.Workgroup.Permissions = new List<WorkgroupPermission>();
            var permission = CreateValidEntities.WorkgroupPermission(1);
            permission.User = new User("Flarg".ToLower());
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            permission = CreateValidEntities.WorkgroupPermission(2);
            permission.User = new User("Blarg".ToLower());
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            order.GenerateRequestNumber();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].RequesterOrderSubmission = false;

            emailPrefs.Add(new EmailPreferences("Flarg".ToLower()));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[1].RequesterOrderSubmission = true;  //Doesn't Matter, did not create it
            emailPrefs[1].ApproverOrderArrive = false;

            emailPrefs.Add(new EmailPreferences("Blarg".ToLower()));
            emailPrefs[2].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[2].RequesterOrderSubmission = false;  //Doesn't Matter, did not create it
            emailPrefs[2].ApproverOrderArrive = false;

            emailPrefs.Add(new EmailPreferences("zoidberg"));
            emailPrefs[3].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[3].ApproverOrderArrive = false;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, order.EmailQueuesV2.Count());
            //Assert.AreEqual(string.Format("Order request {0} has been submitted.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("Flarg", order.EmailQueuesV2[0].User.Id);
            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("Blarg", order.EmailQueuesV2[0].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[1].Text);
            //Assert.AreEqual("zoidberg", order.EmailQueuesV2[1].User.Id);
            #endregion Assert
        }

        /// <summary>
        /// Check that those further up the approval chain are not notified
        /// </summary>
        [TestMethod]
        public void TestOrderCreatedProcessArrival12()
        {
            #region Arrange
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("bender");
            SetupUsers();
            var order = SetupData1("bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver));
            order.DateCreated = new DateTime(2011, 12, 31, 09, 49, 33);

            var approvals = new List<Approval>();
            CreateApprovals(approvals, OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), order);
            var conditionalApproval = new Approval();
            conditionalApproval.Order = new Order();
            conditionalApproval.Order.Id = order.Id;
            conditionalApproval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            conditionalApproval.User = UserRepository.GetNullableById("zoidberg");
            conditionalApproval.SecondaryUser = UserRepository.GetNullableById("zoidberg");
            approvals.Add(conditionalApproval);

            var accountManager = new Approval();
            accountManager.Order = new Order();
            accountManager.Order.Id = order.Id;
            accountManager.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.AccountManager);
            accountManager.User = UserRepository.GetNullableById("AccManag".ToLower());
            accountManager.SecondaryUser = UserRepository.GetNullableById("AccManag".ToLower());
            approvals.Add(accountManager);

            var purchaser = new Approval();
            purchaser.Order = new Order();
            purchaser.Order.Id = order.Id;
            purchaser.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.AccountManager);
            purchaser.User = UserRepository.GetNullableById("pur");
            purchaser.SecondaryUser = UserRepository.GetNullableById("pur");
            approvals.Add(purchaser);

            var saveLevel = 0;
            foreach(var approval in approvals)
            {
                if(approval.User != null && approval.User.Id == "hsimpson")
                {
                    saveLevel = approval.StatusCode.Level;
                    approval.User = null;
                    approval.SecondaryUser = null;
                }
                order.AddApproval(approval);
            }
            order.Workgroup.Permissions = new List<WorkgroupPermission>();
            var permission = CreateValidEntities.WorkgroupPermission(1);
            permission.User = new User("Flarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            permission = CreateValidEntities.WorkgroupPermission(2);
            permission.User = new User("Blarg");
            permission.Role = new Role(OrderStatusCode.Codes.Approver);
            permission.Role.Level = saveLevel;
            order.Workgroup.Permissions.Add(permission);

            order.GenerateRequestNumber();

            var emailPrefs = new List<EmailPreferences>();
            emailPrefs.Add(new EmailPreferences("bender"));
            emailPrefs[0].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[0].RequesterOrderSubmission = false;

            emailPrefs.Add(new EmailPreferences("Flarg".ToLower()));
            emailPrefs[1].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[1].RequesterOrderSubmission = true;  //Doesn't Matter, did not create it
            emailPrefs[1].ApproverOrderArrive = false;

            emailPrefs.Add(new EmailPreferences("Blarg".ToLower()));
            emailPrefs[2].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[2].RequesterOrderSubmission = false;  //Doesn't Matter, did not create it
            emailPrefs[2].ApproverOrderArrive = false;

            emailPrefs.Add(new EmailPreferences("zoidberg"));
            emailPrefs[3].NotificationType = EmailPreferences.NotificationTypes.Daily;
            emailPrefs[3].ApproverOrderArrive = false;
            new FakeEmailPreferences(0, EmailPreferenceRepository, emailPrefs, true);
            new FakeAdminWorkgroups(3, AdminWorkgroupRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            NotificationService.OrderCreated(order);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, order.EmailQueuesV2.Count());
            Assert.AreEqual(7, order.Approvals.Count());
            //Assert.AreEqual(string.Format("Order request {0} has been submitted.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("bender", order.EmailQueuesV2[0].User.Id);
            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("Flarg", order.EmailQueuesV2[0].User.Id);
            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[0].Text);
            //Assert.AreEqual("Blarg", order.EmailQueuesV2[0].User.Id);

            //Assert.AreEqual(string.Format("Order request {0} has arrived at your level (Approver) for review from Bender Rodriguez.", "#testOrg-FT1P9YR"), order.EmailQueuesV2[1].Text);
            //Assert.AreEqual("zoidberg", order.EmailQueuesV2[1].User.Id);
            #endregion Assert
        }
    }
}
