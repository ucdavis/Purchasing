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
    /// Entity Name:		Attachment
    /// LookupFieldName:	FileName
    /// </summary>
    [TestClass]
    public class AttachmentRepositoryTests : AbstractRepositoryTests<Attachment, Guid, AttachmentMap>
    {
        /// <summary>
        /// Gets or sets the Attachment repository.
        /// </summary>
        /// <value>The Attachment repository.</value>
        public IRepository<Attachment> AttachmentRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentRepositoryTests"/> class.
        /// </summary>
        public AttachmentRepositoryTests()
        {
            AttachmentRepository = new Repository<Attachment>();
            UserRepository = new RepositoryWithTypedId<User, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Attachment GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Attachment(counter);
            rtValue.User = UserRepository.Queryable.Single(a => a.Id == "2");
            return rtValue;

        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Attachment> GetQuery(int numberAtEnd)
        {
            return AttachmentRepository.Queryable.Where(a => a.FileName.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Attachment entity, int counter)
        {
            Assert.AreEqual("FileName" + counter, entity.FileName);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Attachment entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.FileName);
                    break;
                case ARTAction.Restore:
                    entity.FileName = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.FileName;
                    entity.FileName = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();

            AttachmentRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            AttachmentRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region FileName Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the FileName with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFileNameWithNullValueDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.FileName = null;
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                var results = attachment.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "FileName"));
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FileName with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFileNameWithEmptyStringDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.FileName = string.Empty;
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                var results = attachment.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "FileName"));
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FileName with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFileNameWithSpacesOnlyDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.FileName = " ";
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                var results = attachment.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "FileName"));
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FileName with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFileNameWithTooLongValueDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.FileName = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                Assert.AreEqual(100 + 1, attachment.FileName.Length);
                var results = attachment.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "FileName", "100"));
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the FileName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestFileNameWithOneCharacterSaves()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.FileName = "x";
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FileName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestFileNameWithLongValueSaves()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.FileName = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, attachment.FileName.Length);
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion FileName Tests
        
        #region ContentType Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the ContentType with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestContentTypeWithNullValueDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.ContentType = null;
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                var results = attachment.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "ContentType"));
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the ContentType with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestContentTypeWithEmptyStringDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.ContentType = string.Empty;
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                var results = attachment.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "ContentType"));
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the ContentType with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestContentTypeWithSpacesOnlyDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.ContentType = " ";
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                var results = attachment.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "ContentType"));
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the ContentType with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestContentTypeWithTooLongValueDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.ContentType = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                Assert.AreEqual(200 + 1, attachment.ContentType.Length);
                var results = attachment.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "ContentType", "200"));
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the ContentType with one character saves.
        /// </summary>
        [TestMethod]
        public void TestContentTypeWithOneCharacterSaves()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.ContentType = "x";
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ContentType with long value saves.
        /// </summary>
        [TestMethod]
        public void TestContentTypeWithLongValueSaves()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.ContentType = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, attachment.ContentType.Length);
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion ContentType Tests

        #region Contents Tests
        /// <summary>
        /// Tests the Contents with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestContentsWithAValueOfNullDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.Contents = null;
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                Assert.AreEqual(attachment.Contents, null);
                var results = attachment.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Contents field is required.");
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }	
        }


        [TestMethod]
        public void TestAttachmentWithContentsSaves1()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.Contents = new byte[0];
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, attachment.Contents.Count());
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestAttachmentWithContentsSaves2()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.Contents = new byte[]{1,2,3,4};
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("1234", attachment.Contents.ByteArrayToString());
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert
        }
        #endregion Contents Tests

        #region DateCreated Tests

        /// <summary>
        /// Tests the DateCreated with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Attachment record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(record);
            AttachmentRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the DateCreated with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(record);
            AttachmentRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateCreated with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(record);
            AttachmentRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert
        }
        #endregion DateCreated Tests

        #region User Tests
        /// <summary>
        /// Tests the User with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserWithAValueOfNullDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.User = null;
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                Assert.AreEqual(attachment.User, null);
                var results = attachment.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The User field is required.");
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestAttachmentWithNewUserDoesNotSave()
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
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(record);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Tests the FieldToTest with A value of TestValue does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestAttachmentWithNewUserDoesNotCascadeSave()
        {
            var thisFar = false;
            try
            {
            #region Arrange
            var record = GetValid(9);
            record.User = new User("NoOne");
                
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
                thisFar = true;
            AttachmentRepository.EnsurePersistent(record);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsTrue(thisFar);
                throw;
            }	
        }

        #endregion User Tests

        #region Order Tests

        [TestMethod]
        public void TestAttachmentWithNullOrderSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = null;
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(record);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Order);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestAttachmentWithExistingOrderSaves()
        {
            #region Arrange
            Repository.OfType<Order>().DbContext.BeginTransaction();
            LoadOrders(3);
            Repository.OfType<Order>().DbContext.CommitTransaction();
            var record = GetValid(9);
            record.Order = Repository.OfType<Order>().Queryable.Single(a => a.Id == 2);
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(record);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, record.Order.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestAttachmentWithNewOrderDoesNotSave()
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
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(record);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Order, Entity: Purchasing.Core.Domain.Order", ex.Message);
                throw;
            }
        }
        
        #endregion Order Tests
        
        #region Category Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Category with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCategoryWithTooLongValueDoesNotSave()
        {
            Attachment attachment = null;
            try
            {
                #region Arrange
                attachment = GetValid(9);
                attachment.Category = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                AttachmentRepository.DbContext.BeginTransaction();
                AttachmentRepository.EnsurePersistent(attachment);
                AttachmentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(attachment);
                Assert.AreEqual(50 + 1, attachment.Category.Length);
                var results = attachment.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Category", "50"));
                Assert.IsTrue(attachment.IsTransient());
                Assert.IsFalse(attachment.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Category with null value saves.
        /// </summary>
        [TestMethod]
        public void TestCategoryWithNullValueSaves()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.Category = null;
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Category with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestCategoryWithEmptyStringSaves()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.Category = string.Empty;
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Category with one space saves.
        /// </summary>
        [TestMethod]
        public void TestCategoryWithOneSpaceSaves()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.Category = " ";
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Category with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCategoryWithOneCharacterSaves()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.Category = "x";
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Category with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCategoryWithLongValueSaves()
        {
            #region Arrange
            var attachment = GetValid(9);
            attachment.Category = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            AttachmentRepository.DbContext.BeginTransaction();
            AttachmentRepository.EnsurePersistent(attachment);
            AttachmentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, attachment.Category.Length);
            Assert.IsFalse(attachment.IsTransient());
            Assert.IsTrue(attachment.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Category Tests

        
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
            expectedFields.Add(new NameAndType("Category", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Contents", "System.Byte[]", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("ContentType", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)200)]"
            }));
            expectedFields.Add(new NameAndType("DateCreated", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("FileName", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)100)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Guid", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Order", "Purchasing.Core.Domain.Order", new List<string>()));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Attachment));

        }

        #endregion Reflection of Database.	
		
		
    }
}