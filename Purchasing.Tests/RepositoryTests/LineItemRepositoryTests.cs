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
    /// Entity Name:		LineItem
    /// LookupFieldName:	Unit
    /// </summary>
    [TestClass]
    public class LineItemRepositoryTests : AbstractRepositoryTests<LineItem, int, LineItemMap>
    {
        /// <summary>
        /// Gets or sets the LineItem repository.
        /// </summary>
        /// <value>The LineItem repository.</value>
        public IRepository<LineItem> LineItemRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; }
        public IRepositoryWithTypedId<Commodity, string> CommodityRepository { get; set; }
        public IRepository<Split> SplitRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemRepositoryTests"/> class.
        /// </summary>
        public LineItemRepositoryTests()
        {
            LineItemRepository = new Repository<LineItem>();
            OrderRepository = new Repository<Order>();
            CommodityRepository = new RepositoryWithTypedId<Commodity, string>();
            SplitRepository = new Repository<Split>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override LineItem GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.LineItem(counter);
            rtValue.Order = OrderRepository.Queryable.Single(a => a.Id == 2);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<LineItem> GetQuery(int numberAtEnd)
        {
            return LineItemRepository.Queryable.Where(a => a.Unit.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(LineItem entity, int counter)
        {
            Assert.AreEqual("Unit" + counter, entity.Unit);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(LineItem entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Unit);
                    break;
                case ARTAction.Restore:
                    entity.Unit = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Unit;
                    entity.Unit = updateValue;
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

            LineItemRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            LineItemRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region Quantity Tests

        [TestMethod]
        public void TestLineItemWithZeroQuantitySaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Quantity = 0;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.Quantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert	
        }

        [TestMethod]
        public void TestLineItemWithQuantitySaves1()
        {
            #region Arrange
            var record = GetValid(9);
            record.Quantity = 0.001m;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.001m, record.Quantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestLineItemWithQuantitySaves2()
        {
            #region Arrange
            var record = GetValid(9);
            record.Quantity = 999999999.999m;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999999999.999m, record.Quantity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Quantity Tests
        
        #region CatalogNumber Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the CatalogNumber with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCatalogNumberWithTooLongValueDoesNotSave()
        {
            LineItem lineItem = null;
            try
            {
                #region Arrange
                lineItem = GetValid(9);
                lineItem.CatalogNumber = "x".RepeatTimes((25 + 1));
                #endregion Arrange

                #region Act
                LineItemRepository.DbContext.BeginTransaction();
                LineItemRepository.EnsurePersistent(lineItem);
                LineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(lineItem);
                Assert.AreEqual(25 + 1, lineItem.CatalogNumber.Length);
                var results = lineItem.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "CatalogNumber", "25"));
                Assert.IsTrue(lineItem.IsTransient());
                Assert.IsFalse(lineItem.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the CatalogNumber with null value saves.
        /// </summary>
        [TestMethod]
        public void TestCatalogNumberWithNullValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.CatalogNumber = null;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CatalogNumber with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestCatalogNumberWithEmptyStringSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.CatalogNumber = string.Empty;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CatalogNumber with one space saves.
        /// </summary>
        [TestMethod]
        public void TestCatalogNumberWithOneSpaceSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.CatalogNumber = " ";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CatalogNumber with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCatalogNumberWithOneCharacterSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.CatalogNumber = "x";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CatalogNumber with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCatalogNumberWithLongValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.CatalogNumber = "x".RepeatTimes(25);
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(25, lineItem.CatalogNumber.Length);
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion CatalogNumber Tests

        #region Description Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Description with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithNullValueDoesNotSave()
        {
            LineItem lineItem = null;
            try
            {
                #region Arrange
                lineItem = GetValid(9);
                lineItem.Description = null;
                #endregion Arrange

                #region Act
                LineItemRepository.DbContext.BeginTransaction();
                LineItemRepository.EnsurePersistent(lineItem);
                LineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(lineItem);
                var results = lineItem.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Description"));
                Assert.IsTrue(lineItem.IsTransient());
                Assert.IsFalse(lineItem.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Description with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithEmptyStringDoesNotSave()
        {
            LineItem lineItem = null;
            try
            {
                #region Arrange
                lineItem = GetValid(9);
                lineItem.Description = string.Empty;
                #endregion Arrange

                #region Act
                LineItemRepository.DbContext.BeginTransaction();
                LineItemRepository.EnsurePersistent(lineItem);
                LineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(lineItem);
                var results = lineItem.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Description"));
                Assert.IsTrue(lineItem.IsTransient());
                Assert.IsFalse(lineItem.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Description with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithSpacesOnlyDoesNotSave()
        {
            LineItem lineItem = null;
            try
            {
                #region Arrange
                lineItem = GetValid(9);
                lineItem.Description = " ";
                #endregion Arrange

                #region Act
                LineItemRepository.DbContext.BeginTransaction();
                LineItemRepository.EnsurePersistent(lineItem);
                LineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(lineItem);
                var results = lineItem.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Description"));
                Assert.IsTrue(lineItem.IsTransient());
                Assert.IsFalse(lineItem.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Description with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithOneCharacterSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Description = "x";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithLongValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Description = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, lineItem.Description.Length);
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Description Tests

        #region Unit Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Unit with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUnitWithTooLongValueDoesNotSave()
        {
            LineItem lineItem = null;
            try
            {
                #region Arrange
                lineItem = GetValid(9);
                lineItem.Unit = "x".RepeatTimes((25 + 1));
                #endregion Arrange

                #region Act
                LineItemRepository.DbContext.BeginTransaction();
                LineItemRepository.EnsurePersistent(lineItem);
                LineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(lineItem);
                Assert.AreEqual(25 + 1, lineItem.Unit.Length);
                var results = lineItem.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Unit", "25"));
                Assert.IsTrue(lineItem.IsTransient());
                Assert.IsFalse(lineItem.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Unit with null value saves.
        /// </summary>
        [TestMethod]
        public void TestUnitWithNullValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Unit = null;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Unit with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestUnitWithEmptyStringSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Unit = string.Empty;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Unit with one space saves.
        /// </summary>
        [TestMethod]
        public void TestUnitWithOneSpaceSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Unit = " ";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Unit with one character saves.
        /// </summary>
        [TestMethod]
        public void TestUnitWithOneCharacterSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Unit = "x";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Unit with long value saves.
        /// </summary>
        [TestMethod]
        public void TestUnitWithLongValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Unit = "x".RepeatTimes(25);
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(25, lineItem.Unit.Length);
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Unit Tests

        #region UnitPrice Tests

        [TestMethod]
        public void TestLineItemWithZeroUnitPriceSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.UnitPrice = 0;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.UnitPrice);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestLineItemWithUnitPriceSaves1()
        {
            #region Arrange
            var record = GetValid(9);
            record.UnitPrice = 0.001m;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.001m, record.UnitPrice);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestLineItemWithUnitPriceSaves2()
        {
            #region Arrange
            var record = GetValid(9);
            record.UnitPrice = 999999999.999m;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999999999.999m, record.UnitPrice);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion UnitPrice Tests

        #region Url Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Url with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUrlWithTooLongValueDoesNotSave()
        {
            LineItem lineItem = null;
            try
            {
                #region Arrange
                lineItem = GetValid(9);
                lineItem.Url = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                LineItemRepository.DbContext.BeginTransaction();
                LineItemRepository.EnsurePersistent(lineItem);
                LineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(lineItem);
                Assert.AreEqual(200 + 1, lineItem.Url.Length);
                var results = lineItem.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Url", "200"));
                Assert.IsTrue(lineItem.IsTransient());
                Assert.IsFalse(lineItem.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Url with null value saves.
        /// </summary>
        [TestMethod]
        public void TestUrlWithNullValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Url = null;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Url with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestUrlWithEmptyStringSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Url = string.Empty;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Url with one space saves.
        /// </summary>
        [TestMethod]
        public void TestUrlWithOneSpaceSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Url = " ";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Url with one character saves.
        /// </summary>
        [TestMethod]
        public void TestUrlWithOneCharacterSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Url = "x";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Url with long value saves.
        /// </summary>
        [TestMethod]
        public void TestUrlWithLongValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Url = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, lineItem.Url.Length);
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Url Tests

        #region Notes Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Notes with null value saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithNullValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Notes = null;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Notes with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithEmptyStringSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Notes = string.Empty;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Notes with one space saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithOneSpaceSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Notes = " ";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Notes with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithOneCharacterSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Notes = "x";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Notes with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithLongValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Notes = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, lineItem.Notes.Length);
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Notes Tests

        #region Order Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestLineItemsFieldOrderWithAValueOfNullDoesNotSave()
        {
            LineItem record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Order = null;
                #endregion Arrange

                #region Act
                LineItemRepository.DbContext.BeginTransaction();
                LineItemRepository.EnsurePersistent(record);
                LineItemRepository.DbContext.CommitTransaction();
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
        public void TestLineItemNewOrderDoesNotSave()
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
                LineItemRepository.DbContext.BeginTransaction();
                LineItemRepository.EnsurePersistent(record);
                LineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.Order, Entity: Purchasing.Core.Domain.Order", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestLineItemWithExistingOrderSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = OrderRepository.Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Order.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Order Tests

        #region Commodity Tests

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestLineItemNewCommodityDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Commodity = new Commodity();
                thisFar = true;
                #endregion Arrange

                #region Act
                LineItemRepository.DbContext.BeginTransaction();
                LineItemRepository.EnsurePersistent(record);
                LineItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.Commodity, Entity: Purchasing.Core.Domain.Commodity", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestLineItemWithExistingCommoditySaves()
        {
            #region Arrange
            CommodityRepository.DbContext.BeginTransaction();
            LoadCommodity(3);
            CommodityRepository.DbContext.CommitTransaction();
            var record = GetValid(9);
            record.Commodity = CommodityRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.Commodity.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestLineItemWithNullCommoditySaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Commodity = null;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Commodity);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Commodity Tests

        #region QuantityReceived Tests

        [TestMethod]
        public void TestLineItemWithNullQuantityReceivedSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.QuantityReceived = null;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.QuantityReceived);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestLineItemWithZeroQuantityReceivedSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.QuantityReceived = 0;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.QuantityReceived);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestLineItemWithQuantityReceivedSaves1()
        {
            #region Arrange
            var record = GetValid(9);
            record.QuantityReceived = 0.001m;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.001m, record.QuantityReceived);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestLineItemWitQuantityReceivedSaves2()
        {
            #region Arrange
            var record = GetValid(9);
            record.UnitPrice = 999999999.999m;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999999999.999m, record.UnitPrice);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion UnitPrice Tests
   
        #region ReceivedNotes Tests


        #region Valid Tests

        /// <summary>
        /// Tests the ReceivedNotes with null value saves.
        /// </summary>
        [TestMethod]
        public void TestReceivedNotesWithNullValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.ReceivedNotes = null;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReceivedNotes with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestReceivedNotesWithEmptyStringSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.ReceivedNotes = string.Empty;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReceivedNotes with one space saves.
        /// </summary>
        [TestMethod]
        public void TestReceivedNotesWithOneSpaceSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.ReceivedNotes = " ";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReceivedNotes with one character saves.
        /// </summary>
        [TestMethod]
        public void TestReceivedNotesWithOneCharacterSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.ReceivedNotes = "x";
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReceivedNotes with long value saves.
        /// </summary>
        [TestMethod]
        public void TestReceivedNotesWithLongValueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.ReceivedNotes = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, lineItem.ReceivedNotes.Length);
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion ReceivedNotes Tests

        #region Received Tests

        /// <summary>
        /// Tests the Received is false saves.
        /// </summary>
        [TestMethod]
        public void TestReceivedIsFalseSaves()
        {
            #region Arrange
            LineItem lineItem = GetValid(9);
            lineItem.Received = false;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(lineItem.Received);
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Received is true saves.
        /// </summary>
        [TestMethod]
        public void TestReceivedIsTrueSaves()
        {
            #region Arrange
            var lineItem = GetValid(9);
            lineItem.Received = true;
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(lineItem);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(lineItem.Received);
            Assert.IsFalse(lineItem.IsTransient());
            Assert.IsTrue(lineItem.IsValid());
            #endregion Assert
        }

        #endregion Received Tests

        #region Splits Tests
        #region Valid Tests
        [TestMethod]
        public void TestSplitsWithPopulatedListWillSave()
        {
            #region Arrange
            LineItem record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddSplit(CreateValidEntities.Split(i+1));
            }
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Splits);
            Assert.AreEqual(addedCount, record.Splits.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestSplitsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            LineItem record = GetValid(9);
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Split>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Split(i + 1));
                relatedRecords[i].Order = record.Order;
                relatedRecords[i].LineItem = record;
                Repository.OfType<Split>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Splits.Add(relatedRecord);
            }
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Splits);
            Assert.AreEqual(addedCount, record.Splits.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestSplitsWithEmptyListWillSave()
        {
            #region Arrange
            LineItem record = GetValid(9);
            #endregion Arrange

            #region Act
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Splits);
            Assert.AreEqual(0, record.Splits.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestLineItemCascadesSaveToSplit()
        {
            #region Arrange
            var count = Repository.OfType<Split>().Queryable.Count();
            LineItem record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddSplit(CreateValidEntities.Split(i+1));
            }

            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = LineItemRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.Splits.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<Split>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestLineItemCascadesUpdateToSplit1()
        {
            #region Arrange
            var count = Repository.OfType<Split>().Queryable.Count();
            LineItem record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddSplit(CreateValidEntities.Split(i+1));
            }

            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Splits[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = LineItemRepository.GetNullableById(saveId);
            record.Splits[1].Project = "Updated";
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Split>().Queryable.Count());
            var relatedRecord = Repository.OfType<Split>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Project);
            #endregion Assert
        }

        [TestMethod]
        public void TestLineItemCascadesUpdateToSplit2()
        {
            #region Arrange
            var count = Repository.OfType<Split>().Queryable.Count();
            LineItem record = GetValid(9);
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Split>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Split(i + 1));
                relatedRecords[i].LineItem = record;
                relatedRecords[i].Order = OrderRepository.Queryable.Single(a => a.Id == 2);
                Repository.OfType<Split>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Splits.Add(relatedRecord);
            }
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Splits[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = LineItemRepository.GetNullableById(saveId);
            record.Splits[1].Project = "Updated";
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Split>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Split>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Project);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it (Delete this test, or the one below)
        /// </summary>
        [TestMethod]
        public void TestLineItemCascadesUpdateRemoveSplit()
        {
            #region Arrange
            var count = Repository.OfType<Split>().Queryable.Count();
            LineItem record = GetValid(9);
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Split>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Split(i + 1));
                relatedRecords[i].LineItem = record;
                relatedRecords[i].Order = OrderRepository.Queryable.Single(a => a.Id == 2);
                Repository.OfType<Split>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Splits.Add(relatedRecord);
            }
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Splits[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = LineItemRepository.GetNullableById(saveId);
            record.Splits.RemoveAt(1);
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<Split>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Split>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }

        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        //[TestMethod]
        //public void TestLineItemDoesNotCascadesUpdateRemoveSplit()
        //{
        //    #region Arrange
        //    var count = Repository.OfType<Split>().Queryable.Count();
        //    LineItem record = GetValid(9);
        //    LineItemRepository.DbContext.BeginTransaction();
        //    LineItemRepository.EnsurePersistent(record);
        //    LineItemRepository.DbContext.CommitTransaction();


        //    const int addedCount = 3;
        //    var relatedRecords = new List<Split>();
        //    for (int i = 0; i < addedCount; i++)
        //    {
        //        relatedRecords.Add(CreateValidEntities.Split(i + 1));
        //        relatedRecords[i].LineItem = record;
        //        relatedRecords[i].Order = OrderRepository.Queryable.Single(a => a.Id == 2);
        //        Repository.OfType<Split>().EnsurePersistent(relatedRecords[i]);
        //    }
        //    foreach (var relatedRecord in relatedRecords)
        //    {
        //        record.Splits.Add(relatedRecord);
        //    }
        //    LineItemRepository.DbContext.BeginTransaction();
        //    LineItemRepository.EnsurePersistent(record);
        //    LineItemRepository.DbContext.CommitTransaction();
        //    var saveId = record.Id;
        //    var saveRelatedId = record.Splits[1].Id;
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    #endregion Arrange

        //    #region Act
        //    record = LineItemRepository.GetNullableById(saveId);
        //    record.Splits.RemoveAt(1);
        //    LineItemRepository.DbContext.BeginTransaction();
        //    LineItemRepository.EnsurePersistent(record);
        //    LineItemRepository.DbContext.CommitTransaction();
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(count + (addedCount), Repository.OfType<Split>().Queryable.Count());
        //    var relatedRecord2 = Repository.OfType<Split>().GetNullableById(saveRelatedId);
        //    Assert.IsNotNull(relatedRecord2);
        //    #endregion Assert
        //}

        [TestMethod]
        public void TestLineItemCascadesDeleteToSplit()
        {
            #region Arrange
            var count = Repository.OfType<Split>().Queryable.Count();
            LineItem record = GetValid(9);
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Split>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Split(i + 1));
                relatedRecords[i].LineItem = record;
                relatedRecords[i].Order = OrderRepository.Queryable.Single(a => a.Id == 2);
                Repository.OfType<Split>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Splits.Add(relatedRecord);
            }
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.EnsurePersistent(record);
            LineItemRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Splits[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = LineItemRepository.GetNullableById(saveId);
            LineItemRepository.DbContext.BeginTransaction();
            LineItemRepository.Remove(record);
            LineItemRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<Split>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Split>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion Splits Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructor()
        {
            #region Arrange
            var record = new LineItem();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Splits);
            Assert.AreEqual(0, record.Splits.Count);
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
            expectedFields.Add(new NameAndType("CatalogNumber", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)25)]"
            }));
            expectedFields.Add(new NameAndType("Commodity", "Purchasing.Core.Domain.Commodity", new List<string>()));
            expectedFields.Add(new NameAndType("Description", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
                                                                         {
                                                                             "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                                                                             "[System.Xml.Serialization.XmlIgnoreAttribute()]"
                                                                         }));
            
            expectedFields.Add(new NameAndType("Notes", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Order", "Purchasing.Core.Domain.Order", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Quantity", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("QuantityReceived", "System.Nullable`1[System.Decimal]", new List<string>()));
            expectedFields.Add(new NameAndType("Received", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("ReceivedNotes", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Splits", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Split]", new List<string>()));
            expectedFields.Add(new NameAndType("Unit", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)25)]"
            }));
            expectedFields.Add(new NameAndType("UnitPrice", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Url", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)200)]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(LineItem));

        }

        #endregion Reflection of Database.	
		
		
    }
}