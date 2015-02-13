using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Audit
    /// LookupFieldName:	ObjectName
    /// </summary>
    [TestClass]
    public class AuditRepositoryTests : AbstractRepositoryTests<Audit, Guid, AuditMap>
    {
        /// <summary>
        /// Gets or sets the Audit repository.
        /// </summary>
        /// <value>The Audit repository.</value>
        public IRepository<Audit> AuditRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditRepositoryTests"/> class.
        /// </summary>
        public AuditRepositoryTests()
        {
            AuditRepository = new Repository<Audit>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Audit GetValid(int? counter)
        {
            return CreateValidEntities.Audit(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Audit> GetQuery(int numberAtEnd)
        {
            return AuditRepository.Queryable.Where(a => a.ObjectName.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Audit entity, int counter)
        {
            Assert.AreEqual("ObjectName" + counter, entity.ObjectName);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Audit entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.ObjectName);
                    break;
                case ARTAction.Restore:
                    entity.ObjectName = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.ObjectName;
                    entity.ObjectName = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            AuditRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            AuditRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region ObjectName Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the ObjectName with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestObjectNameWithNullValueDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.ObjectName = null;
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                var results = audit.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "ObjectName"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the ObjectName with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestObjectNameWithEmptyStringDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.ObjectName = string.Empty;
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                var results = audit.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "ObjectName"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the ObjectName with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestObjectNameWithSpacesOnlyDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.ObjectName = " ";
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                var results = audit.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "ObjectName"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the ObjectName with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestObjectNameWithTooLongValueDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.ObjectName = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                Assert.AreEqual(50 + 1, audit.ObjectName.Length);
                var results = audit.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "ObjectName", "50"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the ObjectName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestObjectNameWithOneCharacterSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.ObjectName = "x";
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ObjectName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestObjectNameWithLongValueSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.ObjectName = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, audit.ObjectName.Length);
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion ObjectName Tests

        #region ObjectId Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the ObjectId with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestObjectIdWithTooLongValueDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.ObjectId = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                Assert.AreEqual(50 + 1, audit.ObjectId.Length);
                var results = audit.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "ObjectId", "50"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the ObjectId with null value saves.
        /// </summary>
        [TestMethod]
        public void TestObjectIdWithNullValueSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.ObjectId = null;
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ObjectId with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestObjectIdWithEmptyStringSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.ObjectId = string.Empty;
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ObjectId with one space saves.
        /// </summary>
        [TestMethod]
        public void TestObjectIdWithOneSpaceSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.ObjectId = " ";
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ObjectId with one character saves.
        /// </summary>
        [TestMethod]
        public void TestObjectIdWithOneCharacterSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.ObjectId = "x";
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ObjectId with long value saves.
        /// </summary>
        [TestMethod]
        public void TestObjectIdWithLongValueSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.ObjectId = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, audit.ObjectId.Length);
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion ObjectId Tests

        #region AuditAction Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the AuditAction with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAuditActionWithNullValueDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.AuditAction = null;
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                var results = audit.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "AuditAction"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the AuditAction with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAuditActionWithEmptyStringDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.AuditAction = string.Empty;
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                var results = audit.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "AuditAction"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the AuditAction with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAuditActionWithSpacesOnlyDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.AuditAction = " ";
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                var results = audit.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "AuditAction"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the AuditAction with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAuditActionWithTooLongValueDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.AuditAction = "x".RepeatTimes((1 + 1));
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                Assert.AreEqual(1 + 1, audit.AuditAction.Length);
                var results = audit.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "AuditAction", "1"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the AuditAction with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAuditActionWithOneCharacterSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.AuditAction = "x";
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AuditAction with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAuditActionWithLongValueSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.AuditAction = "x".RepeatTimes(1);
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, audit.AuditAction.Length);
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion AuditAction Tests

        #region Username Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Username with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUsernameWithNullValueDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.Username = null;
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                var results = audit.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Username"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Username with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUsernameWithEmptyStringDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.Username = string.Empty;
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                var results = audit.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Username"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Username with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUsernameWithSpacesOnlyDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.Username = " ";
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                var results = audit.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Username"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Username with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUsernameWithTooLongValueDoesNotSave()
        {
            Audit audit = null;
            try
            {
                #region Arrange
                audit = GetValid(9);
                audit.Username = "x".RepeatTimes((256 + 1));
                #endregion Arrange

                #region Act
                AuditRepository.DbContext.BeginTransaction();
                AuditRepository.EnsurePersistent(audit);
                AuditRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(audit);
                Assert.AreEqual(256 + 1, audit.Username.Length);
                var results = audit.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Username", "256"));
                Assert.IsTrue(audit.IsTransient());
                Assert.IsFalse(audit.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Username with one character saves.
        /// </summary>
        [TestMethod]
        public void TestUsernameWithOneCharacterSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.Username = "x";
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Username with long value saves.
        /// </summary>
        [TestMethod]
        public void TestUsernameWithLongValueSaves()
        {
            #region Arrange
            var audit = GetValid(9);
            audit.Username = "x".RepeatTimes(256);
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(audit);
            AuditRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(256, audit.Username.Length);
            Assert.IsFalse(audit.IsTransient());
            Assert.IsTrue(audit.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Username Tests

        #region AuditDate Tests

        /// <summary>
        /// Tests the AuditDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestAuditDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            Audit record = GetValid(99);
            record.AuditDate = compareDate;
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(record);
            AuditRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.AuditDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the AuditDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestAuditDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.AuditDate = compareDate;
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(record);
            AuditRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.AuditDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the AuditDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestAuditDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.AuditDate = compareDate;
            #endregion Arrange

            #region Act
            AuditRepository.DbContext.BeginTransaction();
            AuditRepository.EnsurePersistent(record);
            AuditRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.AuditDate);
            #endregion Assert
        }
        #endregion AuditDate Tests

        #region Method Tests

        [TestMethod]
        public void TestSetActionCodeReturnsExpectedValuesForCreate()
        {
            #region Arrange
            const string expectedValue = "C";
            var record = new Audit();
            #endregion Arrange

            #region Act
            record.SetActionCode(AuditActionType.Create);
            var result = record.AuditAction;
            #endregion Act

            #region Assert
            Assert.AreEqual(expectedValue, result);
            #endregion Assert		
        }

        [TestMethod]
        public void TestSetActionCodeReturnsExpectedValuesForUpdate()
        {
            #region Arrange
            const string expectedValue = "U";
            var record = new Audit();
            #endregion Arrange

            #region Act
            record.SetActionCode(AuditActionType.Update);
            var result = record.AuditAction;
            #endregion Act

            #region Assert
            Assert.AreEqual(expectedValue, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestSetActionCodeReturnsExpectedValuesForDelete()
        {
            #region Arrange
            const string expectedValue = "D";
            var record = new Audit();
            #endregion Arrange

            #region Act
            record.SetActionCode(AuditActionType.Delete);
            var result = record.AuditAction;
            #endregion Act

            #region Assert
            Assert.AreEqual(expectedValue, result);
            #endregion Assert
        }
        #endregion Method Tests
        
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
            expectedFields.Add(new NameAndType("AuditAction", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)1)]"
            }));
            expectedFields.Add(new NameAndType("AuditDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Guid", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("ObjectId", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("ObjectName", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Username", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)256)]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Audit));

        }

        #endregion Reflection of Database.		
    }
}