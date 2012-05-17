using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		KfsDocument
    /// LookupFieldName:	DocNumber
    /// </summary>
    [TestClass]
    public class KfsDocumentRepositoryTests : AbstractRepositoryTests<KfsDocument, int, KfsDocumentMap>
    {
        /// <summary>
        /// Gets or sets the KfsDocument repository.
        /// </summary>
        /// <value>The KfsDocument repository.</value>
        public IRepository<KfsDocument> KfsDocumentRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; }

        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="KfsDocumentRepositoryTests"/> class.
        /// </summary>
        public KfsDocumentRepositoryTests()
        {
            KfsDocumentRepository = new Repository<KfsDocument>();
            OrderRepository = new Repository<Order>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override KfsDocument GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.KfsDocument(counter);
            rtValue.Order = OrderRepository.Queryable.Single(a => a.Id == 2);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<KfsDocument> GetQuery(int numberAtEnd)
        {
            return KfsDocumentRepository.Queryable.Where(a => a.DocNumber.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(KfsDocument entity, int counter)
        {
            Assert.AreEqual("DocNumber" + counter, entity.DocNumber);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(KfsDocument entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.DocNumber);
                    break;
                case ARTAction.Restore:
                    entity.DocNumber = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.DocNumber;
                    entity.DocNumber = updateValue;
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
            OrderRepository.DbContext.CommitTransaction();
            KfsDocumentRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            KfsDocumentRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region DocNumber Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the DocNumber with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDocNumberWithNullValueDoesNotSave()
        {
            KfsDocument kfsDocument = null;
            try
            {
                #region Arrange
                kfsDocument = GetValid(9);
                kfsDocument.DocNumber = null;
                #endregion Arrange

                #region Act
                KfsDocumentRepository.DbContext.BeginTransaction();
                KfsDocumentRepository.EnsurePersistent(kfsDocument);
                KfsDocumentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(kfsDocument);
                var results = kfsDocument.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "DocNumber"));
                Assert.IsTrue(kfsDocument.IsTransient());
                Assert.IsFalse(kfsDocument.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the DocNumber with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDocNumberWithEmptyStringDoesNotSave()
        {
            KfsDocument kfsDocument = null;
            try
            {
                #region Arrange
                kfsDocument = GetValid(9);
                kfsDocument.DocNumber = string.Empty;
                #endregion Arrange

                #region Act
                KfsDocumentRepository.DbContext.BeginTransaction();
                KfsDocumentRepository.EnsurePersistent(kfsDocument);
                KfsDocumentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(kfsDocument);
                var results = kfsDocument.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "DocNumber"));
                Assert.IsTrue(kfsDocument.IsTransient());
                Assert.IsFalse(kfsDocument.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the DocNumber with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDocNumberWithSpacesOnlyDoesNotSave()
        {
            KfsDocument kfsDocument = null;
            try
            {
                #region Arrange
                kfsDocument = GetValid(9);
                kfsDocument.DocNumber = " ";
                #endregion Arrange

                #region Act
                KfsDocumentRepository.DbContext.BeginTransaction();
                KfsDocumentRepository.EnsurePersistent(kfsDocument);
                KfsDocumentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(kfsDocument);
                var results = kfsDocument.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "DocNumber"));
                Assert.IsTrue(kfsDocument.IsTransient());
                Assert.IsFalse(kfsDocument.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the DocNumber with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDocNumberWithTooLongValueDoesNotSave()
        {
            KfsDocument kfsDocument = null;
            try
            {
                #region Arrange
                kfsDocument = GetValid(9);
                kfsDocument.DocNumber = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                KfsDocumentRepository.DbContext.BeginTransaction();
                KfsDocumentRepository.EnsurePersistent(kfsDocument);
                KfsDocumentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(kfsDocument);
                Assert.AreEqual(50 + 1, kfsDocument.DocNumber.Length);
                var results = kfsDocument.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "DocNumber", "50"));
                Assert.IsTrue(kfsDocument.IsTransient());
                Assert.IsFalse(kfsDocument.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the DocNumber with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDocNumberWithOneCharacterSaves()
        {
            #region Arrange
            var kfsDocument = GetValid(9);
            kfsDocument.DocNumber = "x";
            #endregion Arrange

            #region Act
            KfsDocumentRepository.DbContext.BeginTransaction();
            KfsDocumentRepository.EnsurePersistent(kfsDocument);
            KfsDocumentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(kfsDocument.IsTransient());
            Assert.IsTrue(kfsDocument.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DocNumber with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDocNumberWithLongValueSaves()
        {
            #region Arrange
            var kfsDocument = GetValid(9);
            kfsDocument.DocNumber = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            KfsDocumentRepository.DbContext.BeginTransaction();
            KfsDocumentRepository.EnsurePersistent(kfsDocument);
            KfsDocumentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, kfsDocument.DocNumber.Length);
            Assert.IsFalse(kfsDocument.IsTransient());
            Assert.IsTrue(kfsDocument.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion DocNumber Tests

        #region Order Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestKfsDocumentsFieldOrderWithAValueOfNullDoesNotSave()
        {
            KfsDocument record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Order = null;
                #endregion Arrange

                #region Act
                KfsDocumentRepository.DbContext.BeginTransaction();
                KfsDocumentRepository.EnsurePersistent(record);
                KfsDocumentRepository.DbContext.CommitTransaction();
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
        public void TestKfsDocumentNewOrderDoesNotSave()
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
                KfsDocumentRepository.DbContext.BeginTransaction();
                KfsDocumentRepository.EnsurePersistent(record);
                KfsDocumentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Order, Entity: Purchasing.Core.Domain.Order", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestKfsDocumentWithExistingOrderSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = OrderRepository.Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            KfsDocumentRepository.DbContext.BeginTransaction();
            KfsDocumentRepository.EnsurePersistent(record);
            KfsDocumentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Order.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Order Tests
        
        
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
            expectedFields.Add(new NameAndType("DocNumber", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
                                                                         {
                                                                             "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                                                                             "[System.Xml.Serialization.XmlIgnoreAttribute()]"
                                                                         }));
            expectedFields.Add(new NameAndType("Order", "Purchasing.Core.Domain.Order", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(KfsDocument));

        }

        #endregion Reflection of Database.	
		
		
    }
}