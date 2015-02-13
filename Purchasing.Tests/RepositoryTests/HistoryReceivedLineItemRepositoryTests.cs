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
    /// Entity Name:		HistoryReceivedLineItem
    /// LookupFieldName:	NewReceivedQuantity
    /// </summary>
    [TestClass]
    public class HistoryReceivedLineItemRepositoryTests : AbstractRepositoryTests<HistoryReceivedLineItem, int, HistoryReceivedLineItemMap>
    {
        /// <summary>
        /// Gets or sets the HistoryReceivedLineItem repository.
        /// </summary>
        /// <value>The HistoryReceivedLineItem repository.</value>
        public IRepository<HistoryReceivedLineItem> HistoryReceivedLineItemRepository { get; set; }
        public IRepository<LineItem> LineItemRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryReceivedLineItemRepositoryTests"/> class.
        /// </summary>
        public HistoryReceivedLineItemRepositoryTests()
        {
            HistoryReceivedLineItemRepository = new Repository<HistoryReceivedLineItem>();
            LineItemRepository = new Repository<LineItem>();
            OrderRepository = new Repository<Order>();
            UserRepository = new RepositoryWithTypedId<User, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override HistoryReceivedLineItem GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.HistoryReceivedLineItem(counter);
            rtValue.LineItem = LineItemRepository.Queryable.First();
            rtValue.User = UserRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<HistoryReceivedLineItem> GetQuery(int numberAtEnd)
        {
            return HistoryReceivedLineItemRepository.Queryable.Where(a => a.NewReceivedQuantity == numberAtEnd);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(HistoryReceivedLineItem entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(HistoryReceivedLineItem entity, ARTAction action)
        {
            const decimal updateValue = 99999.98m;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.NewReceivedQuantity);
                    break;
                case ARTAction.Restore:
                    entity.NewReceivedQuantity = DecimalRestoreValue;
                    break;
                case ARTAction.Update:
                    DecimalRestoreValue = entity.NewReceivedQuantity.HasValue ? entity.NewReceivedQuantity.Value : 0;
                    entity.NewReceivedQuantity = updateValue;
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
            LoadLineItems(3);
            OrderRepository.DbContext.CommitTransaction();
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region LineItem Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestHistoryReceivedLineItemsFieldLineItemWithAValueOfNullDoesNotSave()
        {
            HistoryReceivedLineItem record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.LineItem = null;
                #endregion Arrange

                #region Act
                HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
                HistoryReceivedLineItemRepository.EnsurePersistent(record);
                HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.LineItem, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The LineItem field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestHistoryReceivedLineItemNewLineItemDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.LineItem = new LineItem();
                thisFar = true;
                #endregion Arrange

                #region Act
                HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
                HistoryReceivedLineItemRepository.EnsurePersistent(record);
                HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.IsTrue(ex.Message.Contains("Entity: Purchasing.Core.Domain.LineItem"));
                Assert.IsTrue(ex.Message.Contains("Type: Purchasing.Core.Domain.LineItem"));
                throw;
            }
        }

        [TestMethod]
        public void TestHistoryReceivedLineItemWithExistingLineItemSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.LineItem = LineItemRepository.Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.LineItem.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion LineItem Tests
        
        #region UpdateDate Tests

        /// <summary>
        /// Tests the UpdateDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestUpdateDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            HistoryReceivedLineItem record = GetValid(99);
            record.UpdateDate = compareDate;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.UpdateDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the UpdateDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestUpdateDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.UpdateDate = compareDate;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.UpdateDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the UpdateDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestUpdateDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.UpdateDate = compareDate;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.UpdateDate);
            #endregion Assert
        }
        #endregion UpdateDate Tests
       
        #region OldReceivedQuantity Tests

        /// <summary>
        /// Tests the OldReceivedQuantity with null value saves.
        /// </summary>
        [TestMethod]
        public void TestOldReceivedQuantityWithNullValueSaves()
        {
            #region Arrange
            HistoryReceivedLineItem record = GetValid(9);
            record.OldReceivedQuantity = null;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(record.OldReceivedQuantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the OldReceivedQuantity with max decimal value saves.
        /// </summary>
        [TestMethod]
        public void TestOldReceivedQuantityWithMaxDecValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.OldReceivedQuantity = decimal.MaxValue;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(decimal.MaxValue, record.OldReceivedQuantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the OldReceivedQuantity with min dec value saves.
        /// </summary>
        [TestMethod]
        public void TestOldReceivedQuantityWithMinDecValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.OldReceivedQuantity = decimal.MinValue;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(decimal.MinValue, record.OldReceivedQuantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestOldReceivedQuantityWithZeroDecValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.OldReceivedQuantity = 0;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.OldReceivedQuantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion OldReceivedQuantity Tests

        #region OldReceivedQuantity Tests

        /// <summary>
        /// Tests the NewReceivedQuantity with null value saves.
        /// </summary>
        [TestMethod]
        public void TestNewReceivedQuantityWithNullValueSaves()
        {
            #region Arrange
            HistoryReceivedLineItem record = GetValid(9);
            record.NewReceivedQuantity = null;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(record.NewReceivedQuantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the NewReceivedQuantity with max decimal value saves.
        /// </summary>
        [TestMethod]
        public void TestNewReceivedQuantityWithMaxDecValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.NewReceivedQuantity = decimal.MaxValue;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(decimal.MaxValue, record.NewReceivedQuantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the NewReceivedQuantity with min dec value saves.
        /// </summary>
        [TestMethod]
        public void TestNewReceivedQuantityWithMinDecValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.NewReceivedQuantity = decimal.MinValue;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(decimal.MinValue, record.NewReceivedQuantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestNewReceivedQuantityWithZeroDecValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.NewReceivedQuantity = 0;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.NewReceivedQuantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion NewReceivedQuantity Tests

        #region CommentsUpdated Tests

        /// <summary>
        /// Tests the CommentsUpdated is false saves.
        /// </summary>
        [TestMethod]
        public void TestCommentsUpdatedIsFalseSaves()
        {
            #region Arrange
            HistoryReceivedLineItem historyReceivedLineItem = GetValid(9);
            historyReceivedLineItem.CommentsUpdated = false;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(historyReceivedLineItem);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(historyReceivedLineItem.CommentsUpdated);
            Assert.IsFalse(historyReceivedLineItem.IsTransient());
            Assert.IsTrue(historyReceivedLineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CommentsUpdated is true saves.
        /// </summary>
        [TestMethod]
        public void TestCommentsUpdatedIsTrueSaves()
        {
            #region Arrange
            var historyReceivedLineItem = GetValid(9);
            historyReceivedLineItem.CommentsUpdated = true;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(historyReceivedLineItem);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(historyReceivedLineItem.CommentsUpdated);
            Assert.IsFalse(historyReceivedLineItem.IsTransient());
            Assert.IsTrue(historyReceivedLineItem.IsValid());
            #endregion Assert
        }

        #endregion CommentsUpdated Tests

        #region User Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestHistoryReceivedLineItemsFieldUserWithAValueOfNullDoesNotSave()
        {
            HistoryReceivedLineItem record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.User = null;
                #endregion Arrange

                #region Act
                HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
                HistoryReceivedLineItemRepository.EnsurePersistent(record);
                HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
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
        public void TestHistoryReceivedLineItemNewUserDoesNotSave()
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
                HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
                HistoryReceivedLineItemRepository.EnsurePersistent(record);
                HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.IsTrue(ex.Message.Contains("Entity: Purchasing.Core.Domain.User"));
                Assert.IsTrue(ex.Message.Contains("Type: Purchasing.Core.Domain.User"));
                throw;
            }
        }

        [TestMethod]
        public void TestHistoryReceivedLineItemWithExistingUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(record);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.User.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion User Tests

        #region PayInvoice Tests

        /// <summary>
        /// Tests the PayInvoice is false saves.
        /// </summary>
        [TestMethod]
        public void TestPayInvoiceIsFalseSaves()
        {
            #region Arrange
            HistoryReceivedLineItem historyReceivedLineItem = GetValid(9);
            historyReceivedLineItem.PayInvoice = false;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(historyReceivedLineItem);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(historyReceivedLineItem.PayInvoice);
            Assert.IsFalse(historyReceivedLineItem.IsTransient());
            Assert.IsTrue(historyReceivedLineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PayInvoice is true saves.
        /// </summary>
        [TestMethod]
        public void TestPayInvoiceIsTrueSaves()
        {
            #region Arrange
            var historyReceivedLineItem = GetValid(9);
            historyReceivedLineItem.PayInvoice = true;
            #endregion Arrange

            #region Act
            HistoryReceivedLineItemRepository.DbContext.BeginTransaction();
            HistoryReceivedLineItemRepository.EnsurePersistent(historyReceivedLineItem);
            HistoryReceivedLineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(historyReceivedLineItem.PayInvoice);
            Assert.IsFalse(historyReceivedLineItem.IsTransient());
            Assert.IsTrue(historyReceivedLineItem.IsValid());
            #endregion Assert
        }

        #endregion PayInvoice Tests


        #region Constructor

        [TestMethod]
        public void TestConstructor()
        {
            #region Arrange
            var record = new HistoryReceivedLineItem();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.IsFalse(record.CommentsUpdated);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, record.UpdateDate.Date);
            #endregion Assert		
        }
        #endregion Constructor

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
            expectedFields.Add(new NameAndType("CommentsUpdated", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("LineItem", "Purchasing.Core.Domain.LineItem", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("NewReceivedQuantity", "System.Nullable`1[System.Decimal]", new List<string>()));
            expectedFields.Add(new NameAndType("OldReceivedQuantity", "System.Nullable`1[System.Decimal]", new List<string>()));
            expectedFields.Add(new NameAndType("PayInvoice", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("UpdateDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(HistoryReceivedLineItem));

        }

        #endregion Reflection of Database.	
		
		
    }
}