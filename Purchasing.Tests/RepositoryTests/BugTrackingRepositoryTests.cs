using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentNHibernate.Testing;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		BugTracking
    /// LookupFieldName:	UserId
    /// </summary>
    [TestClass]
    public class BugTrackingRepositoryTests : AbstractRepositoryTests<BugTracking, int, BugTrackingMap>
    {
        /// <summary>
        /// Gets or sets the BugTracking repository.
        /// </summary>
        /// <value>The BugTracking repository.</value>
        public IRepository<BugTracking> BugTrackingRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="BugTrackingRepositoryTests"/> class.
        /// </summary>
        public BugTrackingRepositoryTests()
        {
            BugTrackingRepository = new Repository<BugTracking>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override BugTracking GetValid(int? counter)
        {
            return CreateValidEntities.BugTracking(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<BugTracking> GetQuery(int numberAtEnd)
        {
            return BugTrackingRepository.Queryable.Where(a => a.UserId.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(BugTracking entity, int counter)
        {
            Assert.AreEqual("UserId" + counter, entity.UserId);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(BugTracking entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.UserId);
                    break;
                case ARTAction.Restore:
                    entity.UserId = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.UserId;
                    entity.UserId = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            BugTrackingRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            BugTrackingRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region UserId Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the UserId with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithNullValueDoesNotSave()
        {
            BugTracking bugTracking = null;
            try
            {
                #region Arrange
                bugTracking = GetValid(9);
                bugTracking.UserId = null;
                #endregion Arrange

                #region Act
                BugTrackingRepository.DbContext.BeginTransaction();
                BugTrackingRepository.EnsurePersistent(bugTracking);
                BugTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(bugTracking);
                var results = bugTracking.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "UserId"));
                Assert.IsTrue(bugTracking.IsTransient());
                Assert.IsFalse(bugTracking.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the UserId with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithEmptyStringDoesNotSave()
        {
            BugTracking bugTracking = null;
            try
            {
                #region Arrange
                bugTracking = GetValid(9);
                bugTracking.UserId = string.Empty;
                #endregion Arrange

                #region Act
                BugTrackingRepository.DbContext.BeginTransaction();
                BugTrackingRepository.EnsurePersistent(bugTracking);
                BugTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(bugTracking);
                var results = bugTracking.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "UserId"));
                Assert.IsTrue(bugTracking.IsTransient());
                Assert.IsFalse(bugTracking.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the UserId with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithSpacesOnlyDoesNotSave()
        {
            BugTracking bugTracking = null;
            try
            {
                #region Arrange
                bugTracking = GetValid(9);
                bugTracking.UserId = " ";
                #endregion Arrange

                #region Act
                BugTrackingRepository.DbContext.BeginTransaction();
                BugTrackingRepository.EnsurePersistent(bugTracking);
                BugTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(bugTracking);
                var results = bugTracking.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "UserId"));
                Assert.IsTrue(bugTracking.IsTransient());
                Assert.IsFalse(bugTracking.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the UserId with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithTooLongValueDoesNotSave()
        {
            BugTracking bugTracking = null;
            try
            {
                #region Arrange
                bugTracking = GetValid(9);
                bugTracking.UserId = "x".RepeatTimes((20 + 1));
                #endregion Arrange

                #region Act
                BugTrackingRepository.DbContext.BeginTransaction();
                BugTrackingRepository.EnsurePersistent(bugTracking);
                BugTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(bugTracking);
                Assert.AreEqual(20 + 1, bugTracking.UserId.Length);
                var results = bugTracking.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "UserId", "20"));
                Assert.IsTrue(bugTracking.IsTransient());
                Assert.IsFalse(bugTracking.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the UserId with one character saves.
        /// </summary>
        [TestMethod]
        public void TestUserIdWithOneCharacterSaves()
        {
            #region Arrange
            var bugTracking = GetValid(9);
            bugTracking.UserId = "x";
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(bugTracking);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(bugTracking.IsTransient());
            Assert.IsTrue(bugTracking.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the UserId with long value saves.
        /// </summary>
        [TestMethod]
        public void TestUserIdWithLongValueSaves()
        {
            #region Arrange
            var bugTracking = GetValid(9);
            bugTracking.UserId = "x".RepeatTimes(20);
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(bugTracking);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(20, bugTracking.UserId.Length);
            Assert.IsFalse(bugTracking.IsTransient());
            Assert.IsTrue(bugTracking.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion UserId Tests

        #region DateTimeStamp Tests

        /// <summary>
        /// Tests the DateTimeStamp with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTimeStampWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            BugTracking record = GetValid(99);
            record.DateTimeStamp = compareDate;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(record);
            BugTrackingRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTimeStamp);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the DateTimeStamp with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTimeStampWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.DateTimeStamp = compareDate;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(record);
            BugTrackingRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTimeStamp);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateTimeStamp with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateTimeStampWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.DateTimeStamp = compareDate;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(record);
            BugTrackingRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateTimeStamp);
            #endregion Assert
        }
        #endregion DateTimeStamp Tests
       
        #region TrackingMessage Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the TrackingMessage with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTrackingMessageWithTooLongValueDoesNotSave()
        {
            BugTracking bugTracking = null;
            try
            {
                #region Arrange
                bugTracking = GetValid(9);
                bugTracking.TrackingMessage = "x".RepeatTimes((500 + 1));
                #endregion Arrange

                #region Act
                BugTrackingRepository.DbContext.BeginTransaction();
                BugTrackingRepository.EnsurePersistent(bugTracking);
                BugTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(bugTracking);
                Assert.AreEqual(500 + 1, bugTracking.TrackingMessage.Length);
                var results = bugTracking.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "TrackingMessage", "500"));
                Assert.IsTrue(bugTracking.IsTransient());
                Assert.IsFalse(bugTracking.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the TrackingMessage with null value saves.
        /// </summary>
        [TestMethod]
        public void TestTrackingMessageWithNullValueSaves()
        {
            #region Arrange
            var bugTracking = GetValid(9);
            bugTracking.TrackingMessage = null;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(bugTracking);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(bugTracking.IsTransient());
            Assert.IsTrue(bugTracking.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the TrackingMessage with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestTrackingMessageWithEmptyStringSaves()
        {
            #region Arrange
            var bugTracking = GetValid(9);
            bugTracking.TrackingMessage = string.Empty;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(bugTracking);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(bugTracking.IsTransient());
            Assert.IsTrue(bugTracking.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the TrackingMessage with one space saves.
        /// </summary>
        [TestMethod]
        public void TestTrackingMessageWithOneSpaceSaves()
        {
            #region Arrange
            var bugTracking = GetValid(9);
            bugTracking.TrackingMessage = " ";
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(bugTracking);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(bugTracking.IsTransient());
            Assert.IsTrue(bugTracking.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the TrackingMessage with one character saves.
        /// </summary>
        [TestMethod]
        public void TestTrackingMessageWithOneCharacterSaves()
        {
            #region Arrange
            var bugTracking = GetValid(9);
            bugTracking.TrackingMessage = "x";
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(bugTracking);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(bugTracking.IsTransient());
            Assert.IsTrue(bugTracking.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the TrackingMessage with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTrackingMessageWithLongValueSaves()
        {
            #region Arrange
            var bugTracking = GetValid(9);
            bugTracking.TrackingMessage = "x".RepeatTimes(500);
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(bugTracking);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(500, bugTracking.TrackingMessage.Length);
            Assert.IsFalse(bugTracking.IsTransient());
            Assert.IsTrue(bugTracking.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion TrackingMessage Tests

        #region SplitId Tests

        /// <summary>
        /// Tests the SplitId with null value saves.
        /// </summary>
        [TestMethod]
        public void TestSplitIdWithNullValueSaves()
        {
            #region Arrange
            BugTracking record = GetValid(9);
            record.SplitId = null;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(record);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(record.SplitId);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the SplitId with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestSplitIdWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.SplitId = int.MaxValue;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(record);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.SplitId);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SplitId with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestSplitIdWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.SplitId = int.MinValue;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(record);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.SplitId);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion SplitId Tests
        
        #region LineItemId Tests

        /// <summary>
        /// Tests the LineItemId with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLineItemIdWithNullValueSaves()
        {
            #region Arrange
            BugTracking record = GetValid(9);
            record.LineItemId = null;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(record);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(record.LineItemId);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the LineItemId with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestLineItemIdWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.LineItemId = int.MaxValue;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(record);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.LineItemId);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LineItemId with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestLineItemIdWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.LineItemId = int.MinValue;
            #endregion Arrange

            #region Act
            BugTrackingRepository.DbContext.BeginTransaction();
            BugTrackingRepository.EnsurePersistent(record);
            BugTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.LineItemId);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion LineItemId Tests
       

        
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
            expectedFields.Add(new NameAndType("DateTimeStamp", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("LineItemId", "System.Nullable`1[System.Int32]", new List<string>()));
            expectedFields.Add(new NameAndType("OrderId", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("SplitId", "System.Nullable`1[System.Int32]", new List<string>()));
            expectedFields.Add(new NameAndType("TrackingMessage", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)500)]"
            }));
            expectedFields.Add(new NameAndType("UserId", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)20)]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(BugTracking));

        }

        #endregion Reflection of Database.	
		
		
    }
}