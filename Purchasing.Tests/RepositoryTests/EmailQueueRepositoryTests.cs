﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		EmailQueue
    /// LookupFieldName:	Text
    /// </summary>
    [TestClass]
    public class EmailQueueRepositoryTests : AbstractRepositoryTests<EmailQueue, Guid, EmailQueueMap>
    {
        /// <summary>
        /// Gets or sets the EmailQueue repository.
        /// </summary>
        /// <value>The EmailQueue repository.</value>
        public IRepository<EmailQueue> EmailQueueRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailQueueRepositoryTests"/> class.
        /// </summary>
        public EmailQueueRepositoryTests()
        {
            EmailQueueRepository = new Repository<EmailQueue>();
            OrderRepository = new Repository<Order>();
            UserRepository = new RepositoryWithTypedId<User, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override EmailQueue GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.EmailQueue(counter);
            rtValue.Order = OrderRepository.Queryable.Single(a => a.Id == 2);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<EmailQueue> GetQuery(int numberAtEnd)
        {
            return EmailQueueRepository.Queryable.Where(a => a.Text.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(EmailQueue entity, int counter)
        {
            Assert.AreEqual("Text" + counter, entity.Text);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(EmailQueue entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Text);
                    break;
                case ARTAction.Restore:
                    entity.Text = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Text;
                    entity.Text = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            OrderRepository.DbContext.BeginTransaction();
            LoadOrders(3);
            LoadUsers(3);
            OrderRepository.DbContext.CommitTransaction();

            EmailQueueRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            EmailQueueRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region User Tests

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestEmailQueueNewUserDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.User = new User();
                thisFar = true;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(record);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        public void TestEmailQueueWithExistingUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.User.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestEmailQueueWithNullUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = null;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.User);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion User Tests

        #region Email Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Email with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithTooLongValueDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Email = string.Format("x{0}@x.x", "x".RepeatTimes((96)));
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                Assert.AreEqual(100 + 1, emailQueue.Email.Length);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Email", "100"));
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithInvalidEmailDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Email = "x@@x.com";
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is not a valid e-mail address.", "Email"));
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Email with null value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithNullValueSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Email = null;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }


        [TestMethod]
        public void TestEmailWithMinimalCharactersSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Email = "x@x.x";
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithLongValueSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Email = emailQueue.Email = string.Format("x{0}@x.x", "x".RepeatTimes((95)));
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, emailQueue.Email.Length);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Email Tests

        #region Text Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Text with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTextWithNullValueDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Text = null;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Text"));
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Text with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTextWithEmptyStringDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Text = string.Empty;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Text"));
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Text with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTextWithSpacesOnlyDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Text = " ";
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Text"));
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Text with one character saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithOneCharacterSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Text = "x";
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithLongValueSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Text = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, emailQueue.Text.Length);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Text Tests

        #region Order Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestEmailQueuesFieldOrderWithAValueOfNullDoesNotSave()
        {
            EmailQueue record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Order = null;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(record);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Order, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Order field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestEmailQueueNewOrderDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Order = new Order();
                thisFar = true;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(record);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Order, Entity: Purchasing.Core.Domain.Order", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        public void TestEmailQueueWithExistingOrderSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = OrderRepository.Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Order.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Order Tests

        #region Pending Tests

        /// <summary>
        /// Tests the Pending is false saves.
        /// </summary>
        [TestMethod]
        public void TestPendingIsFalseSaves()
        {
            #region Arrange
            EmailQueue emailQueue = GetValid(9);
            emailQueue.Pending = false;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.Pending);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Pending is true saves.
        /// </summary>
        [TestMethod]
        public void TestPendingIsTrueSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Pending = true;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailQueue.Pending);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        #endregion Pending Tests

        #region Status Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Status with null value saves.
        /// </summary>
        [TestMethod]
        public void TestStatusWithNullValueSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Status = null;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Status with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestStatusWithEmptyStringSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Status = string.Empty;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Status with one space saves.
        /// </summary>
        [TestMethod]
        public void TestStatusWithOneSpaceSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Status = " ";
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Status with one character saves.
        /// </summary>
        [TestMethod]
        public void TestStatusWithOneCharacterSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Status = "x";
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Status with long value saves.
        /// </summary>
        [TestMethod]
        public void TestStatusWithLongValueSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Status = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, emailQueue.Status.Length);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Status Tests

        #region NotificationType Tests

        [TestMethod]
        public void TestNotifiactionTypeValuesSave()
        {
            var counter = 0;
            foreach(var notificationType in Enum.GetValues(typeof(EmailPreferences.NotificationTypes)))
            {
                #region Arrange
                var emailQueue = GetValid(9);
                emailQueue.NotificationType = (EmailPreferences.NotificationTypes)notificationType;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act

                #region Assert
                Assert.AreEqual((EmailPreferences.NotificationTypes)notificationType, emailQueue.NotificationType);
                Assert.IsFalse(emailQueue.IsTransient());
                Assert.IsTrue(emailQueue.IsValid());
                #endregion Assert

                counter++;
            }
            Assert.AreEqual(3, counter);
        }
        #endregion NotificationType Tests

        #region DateTimeCreated Tests

        /// <summary>
        /// Tests the DateTimeCreated with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTimeCreatedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            EmailQueue record = GetValid(99);
            record.DateTimeCreated = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTimeCreated);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the DateTimeCreated with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTimeCreatedWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.DateTimeCreated = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTimeCreated);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateTimeCreated with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTimeCreatedWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.DateTimeCreated = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTimeCreated);
            #endregion Assert
        }
        #endregion DateTimeCreated Tests

        #region DateTimeSent Tests

        [TestMethod]
        public void TestDateTimeSentWithNullValueWillSave()
        {
            #region Arrange
            EmailQueue record = GetValid(99);
            record.DateTimeSent = null;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(null, record.DateTimeSent);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateTimeSent with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTimeSentWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            EmailQueue record = GetValid(99);
            record.DateTimeSent = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTimeSent);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the DateTimeSent with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTimeSentWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.DateTimeSent = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTimeSent);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateTimeSent with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTimeSentWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.DateTimeSent = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTimeSent);
            #endregion Assert
        }
        #endregion DateTimeSent Tests

        #region Constructor Tests

        [TestMethod]
        public void TestEmailQueueWithNoParamaters()
        {
            #region Arrange
            var record = new EmailQueue();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, record.DateTimeCreated.Date);
            Assert.IsTrue(record.Pending);
            Assert.IsNull(record.Order);
            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, record.NotificationType);
            Assert.IsNull(record.Text);
            Assert.IsNull(record.User);
            Assert.IsNull(record.Email);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEmailQueueWithParamaters1()
        {
            #region Arrange
            var record = new EmailQueue(OrderRepository.Queryable.Single(a => a.Id == 3), EmailPreferences.NotificationTypes.Daily, "SomeText");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, record.DateTimeCreated.Date);
            Assert.IsTrue(record.Pending);
            Assert.AreEqual(3, record.Order.Id);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Daily, record.NotificationType);
            Assert.AreEqual("SomeText", record.Text);
            Assert.IsNull(record.User);
            Assert.IsNull(record.Email);
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailQueueWithParamaters2()
        {
            #region Arrange
            var record = new EmailQueue(OrderRepository.Queryable.Single(a => a.Id == 3), EmailPreferences.NotificationTypes.Weekly, "SomeText", UserRepository.Queryable.Single(a => a.Id == "2"));
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, record.DateTimeCreated.Date);
            Assert.IsTrue(record.Pending);
            Assert.AreEqual(3, record.Order.Id);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, record.NotificationType);
            Assert.AreEqual("SomeText", record.Text);
            Assert.AreEqual("2", record.User.Id);
            Assert.IsNull(record.Email);
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailQueueWithParamaters3()
        {
            #region Arrange
            var record = new EmailQueue(OrderRepository.Queryable.Single(a => a.Id == 3), EmailPreferences.NotificationTypes.Weekly, "SomeText", email:"a@a.com");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, record.DateTimeCreated.Date);
            Assert.IsTrue(record.Pending);
            Assert.AreEqual(3, record.Order.Id);
            Assert.AreEqual(EmailPreferences.NotificationTypes.Weekly, record.NotificationType);
            Assert.AreEqual("SomeText", record.Text);
            Assert.IsNull(record.User);
            Assert.AreEqual("a@a.com", record.Email);
            #endregion Assert
        }
        
        #endregion Constructor Tests
        
        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("DateTimeCreated", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("DateTimeSent", "System.Nullable`1[System.DateTime]", new List<string>()));
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DataTypeAttribute((System.ComponentModel.DataAnnotations.DataType)10)]",
                "[System.ComponentModel.DataAnnotations.EmailAddressAttribute()]",                 
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)100)]"
                 
            }));
            expectedFields.Add(new NameAndType("Id", "System.Guid", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("NotificationType", "Purchasing.Core.Domain.EmailPreferences+NotificationTypes", new List<string>{
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Order", "Purchasing.Core.Domain.Order", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Pending", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Status", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Text", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(EmailQueue));

        }

        #endregion Reflection of Database.	
		
		
    }
}