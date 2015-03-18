using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Notification
    /// LookupFieldName:	Status
    /// </summary>
    [TestClass]
    public class NotificationRepositoryTests : AbstractRepositoryTests<Notification, Guid, NotificationMap>
    {
        /// <summary>
        /// Gets or sets the Notification repository.
        /// </summary>
        /// <value>The Notification repository.</value>
        public IRepository<Notification> NotificationRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRepositoryTests"/> class.
        /// </summary>
        public NotificationRepositoryTests()
        {
            NotificationRepository = new Repository<Notification>();
            UserRepository = new RepositoryWithTypedId<User, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Notification GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Notification(counter);
            var userToGet = counter.HasValue && counter.Value <= 5 ? counter.Value.ToString() : "1";
            rtValue.User = UserRepository.Queryable.Single(a => a.Id == userToGet);

            if (counter.HasValue && counter.Value == 3)
            {
                rtValue.Weekly = true;
            }
            else
            {
                rtValue.Weekly = false;
            }
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Notification> GetQuery(int numberAtEnd)
        {
            return NotificationRepository.Queryable.Where(a => a.User.Id == numberAtEnd.ToString());
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Notification entity, int counter)
        {
            Assert.AreEqual("Status" + counter, entity.Status);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Notification entity, ARTAction action)
        {
            const bool updateValue = false;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Weekly);
                    break;
                case ARTAction.Restore:
                    entity.Weekly = BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.Weekly;
                    entity.Weekly = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(5);
            UserRepository.DbContext.CommitTransaction();

            NotificationRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            NotificationRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region User Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestNotificationsFieldUserWithAValueOfNullDoesNotSave()
        {
            Notification record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.User = null;
                #endregion Arrange

                #region Act
                NotificationRepository.DbContext.BeginTransaction();
                NotificationRepository.EnsurePersistent(record);
                NotificationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.User, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The User field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestNotificationNewUserDoesNotSave()
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
                NotificationRepository.DbContext.BeginTransaction();
                NotificationRepository.EnsurePersistent(record);
                NotificationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestNotificationWithExistingUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(record);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.User.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion User Tests
        
        #region Created Tests

        /// <summary>
        /// Tests the Created with past date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            Notification record = GetValid(99);
            record.Created = compareDate;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(record);
            NotificationRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Created);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the Created with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.Created = compareDate;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(record);
            NotificationRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Created);
            #endregion Assert
        }

        /// <summary>
        /// Tests the Created with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.Created = compareDate;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(record);
            NotificationRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Created);
            #endregion Assert
        }
        #endregion Created Tests
       
        #region Sent Tests

        /// <summary>
        /// Tests the Sent with past date will save.
        /// </summary>
        [TestMethod]
        public void TestSentWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            Notification record = GetValid(99);
            record.Sent = compareDate;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(record);
            NotificationRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Sent);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the Sent with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestSentWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.Sent = compareDate;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(record);
            NotificationRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Sent);
            #endregion Assert
        }

        /// <summary>
        /// Tests the Sent with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestSentWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.Sent = compareDate;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(record);
            NotificationRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Sent);
            #endregion Assert
        }

        [TestMethod]
        public void TestSentWithNullDateDateWillSave()
        {
            #region Arrange
            var record = GetValid(99);
            record.Sent = null;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(record);
            NotificationRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(null, record.Sent);
            #endregion Assert
        }


        [TestMethod]
        public void TestNotificationSentIsReadOnly()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = NotificationRepository.Queryable.Single(a => a.User.Id == "3");
            record.Sent = compareDate;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(record);
            NotificationRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            record = NotificationRepository.Queryable.Single(a => a.User.Id == "3");
            #endregion Act

            #region Assert
            Assert.AreNotEqual(compareDate, record.Sent);
            #endregion Assert		
        }

        #endregion Sent Tests
        
        #region Status Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Status with null value saves.
        /// </summary>
        [TestMethod]
        public void TestStatusWithNullValueSaves()
        {
            #region Arrange
            var notification = GetValid(9);
            notification.Status = null;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Status with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestStatusWithEmptyStringSaves()
        {
            #region Arrange
            var notification = GetValid(9);
            notification.Status = string.Empty;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Status with one space saves.
        /// </summary>
        [TestMethod]
        public void TestStatusWithOneSpaceSaves()
        {
            #region Arrange
            var notification = GetValid(9);
            notification.Status = " ";
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Status with one character saves.
        /// </summary>
        [TestMethod]
        public void TestStatusWithOneCharacterSaves()
        {
            #region Arrange
            var notification = GetValid(9);
            notification.Status = "x";
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Status with long value saves.
        /// </summary>
        [TestMethod]
        public void TestStatusWithLongValueSaves()
        {
            #region Arrange
            var notification = GetValid(9);
            notification.Status = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, notification.Status.Length);
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestNotificationStatusIsReadOnly()
        {
            #region Arrange
            var record = NotificationRepository.Queryable.Single(a => a.User.Id == "3");
            record.Status = "Updated";
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(record);
            NotificationRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            record = NotificationRepository.Queryable.Single(a => a.User.Id == "3");
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Status);
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Status Tests

        #region Pending Tests

        /// <summary>
        /// Tests the Pending is false saves.
        /// </summary>
        [TestMethod]
        public void TestPendingIsFalseSaves()
        {
            #region Arrange
            Notification notification = GetValid(9);
            notification.Pending = false;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(notification.Pending);
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Pending is true saves.
        /// </summary>
        [TestMethod]
        public void TestPendingIsTrueSaves()
        {
            #region Arrange
            var notification = GetValid(9);
            notification.Pending = true;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(notification.Pending);
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        #endregion Pending Tests

        #region PerEvent Tests

        /// <summary>
        /// Tests the PerEvent is false saves.
        /// </summary>
        [TestMethod]
        public void TestPerEventIsFalseSaves()
        {
            #region Arrange
            Notification notification = GetValid(9);
            notification.PerEvent = false;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(notification.PerEvent);
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PerEvent is true saves.
        /// </summary>
        [TestMethod]
        public void TestPerEventIsTrueSaves()
        {
            #region Arrange
            var notification = GetValid(9);
            notification.PerEvent = true;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(notification.PerEvent);
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        #endregion PerEvent Tests

        #region Daily Tests

        /// <summary>
        /// Tests the Daily is false saves.
        /// </summary>
        [TestMethod]
        public void TestDailyIsFalseSaves()
        {
            #region Arrange
            Notification notification = GetValid(9);
            notification.Daily = false;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(notification.Daily);
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Daily is true saves.
        /// </summary>
        [TestMethod]
        public void TestDailyIsTrueSaves()
        {
            #region Arrange
            var notification = GetValid(9);
            notification.Daily = true;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(notification.Daily);
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        #endregion Daily Tests

        #region Weekly Tests

        /// <summary>
        /// Tests the Weekly is false saves.
        /// </summary>
        [TestMethod]
        public void TestWeeklyIsFalseSaves()
        {
            #region Arrange
            Notification notification = GetValid(9);
            notification.Weekly = false;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(notification.Weekly);
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Weekly is true saves.
        /// </summary>
        [TestMethod]
        public void TestWeeklyIsTrueSaves()
        {
            #region Arrange
            var notification = GetValid(9);
            notification.Weekly = true;
            #endregion Arrange

            #region Act
            NotificationRepository.DbContext.BeginTransaction();
            NotificationRepository.EnsurePersistent(notification);
            NotificationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(notification.Weekly);
            Assert.IsFalse(notification.IsTransient());
            Assert.IsTrue(notification.IsValid());
            #endregion Assert
        }

        #endregion Weekly Tests

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
            expectedFields.Add(new NameAndType("Created", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Daily", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Guid", new List<string>
                                                                         {
                                                                             "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                                                                             "[System.Xml.Serialization.XmlIgnoreAttribute()]"
                                                                         }));
            expectedFields.Add(new NameAndType("Pending", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("PerEvent", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Sent", "System.Nullable`1[System.DateTime]", new List<string>()));
            expectedFields.Add(new NameAndType("Status", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Weekly", "System.Boolean", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Notification));

        }

        #endregion Reflection of Database.	
		
		
    }
}