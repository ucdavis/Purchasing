using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		OrderStatusCode
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class OrderStatusCodeRepositoryTests : AbstractRepositoryTests<OrderStatusCode, string, OrderStatusCodeMap>
    {
        /// <summary>
        /// Gets or sets the OrderStatusCode repository.
        /// </summary>
        /// <value>The OrderStatusCode repository.</value>
        public IRepository<OrderStatusCode> OrderStatusCodeRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderStatusCodeRepositoryTests"/> class.
        /// </summary>
        public OrderStatusCodeRepositoryTests()
        {
            OrderStatusCodeRepository = new Repository<OrderStatusCode>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override OrderStatusCode GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.OrderStatusCode(counter);
            rtValue.Id = counter.HasValue ? counter.Value.ToString() : "99";

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<OrderStatusCode> GetQuery(int numberAtEnd)
        {
            return OrderStatusCodeRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(OrderStatusCode entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(OrderStatusCode entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Name);
                    break;
                case ARTAction.Restore:
                    entity.Name = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Name;
                    entity.Name = updateValue;
                    break;
            }
        }

        [TestMethod]
        public override void CanUpdateEntity()
        {
            CanUpdateEntity(false);
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            OrderStatusCode orderStatusCode = null;
            try
            {
                #region Arrange
                orderStatusCode = GetValid(9);
                orderStatusCode.Name = null;
                #endregion Arrange

                #region Act
                OrderStatusCodeRepository.DbContext.BeginTransaction();
                OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
                OrderStatusCodeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderStatusCode);
                var results = orderStatusCode.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(orderStatusCode.IsTransient());
                Assert.IsFalse(orderStatusCode.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            OrderStatusCode orderStatusCode = null;
            try
            {
                #region Arrange
                orderStatusCode = GetValid(9);
                orderStatusCode.Name = string.Empty;
                #endregion Arrange

                #region Act
                OrderStatusCodeRepository.DbContext.BeginTransaction();
                OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
                OrderStatusCodeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderStatusCode);
                var results = orderStatusCode.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(orderStatusCode.IsTransient());
                Assert.IsFalse(orderStatusCode.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            OrderStatusCode orderStatusCode = null;
            try
            {
                #region Arrange
                orderStatusCode = GetValid(9);
                orderStatusCode.Name = " ";
                #endregion Arrange

                #region Act
                OrderStatusCodeRepository.DbContext.BeginTransaction();
                OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
                OrderStatusCodeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderStatusCode);
                var results = orderStatusCode.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(orderStatusCode.IsTransient());
                Assert.IsFalse(orderStatusCode.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            OrderStatusCode orderStatusCode = null;
            try
            {
                #region Arrange
                orderStatusCode = GetValid(9);
                orderStatusCode.Name = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                OrderStatusCodeRepository.DbContext.BeginTransaction();
                OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
                OrderStatusCodeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderStatusCode);
                Assert.AreEqual(50 + 1, orderStatusCode.Name.Length);
                var results = orderStatusCode.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "50"));
                //Assert.IsTrue(orderStatusCode.IsTransient());
                Assert.IsFalse(orderStatusCode.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var orderStatusCode = GetValid(9);
            orderStatusCode.Name = "x";
            #endregion Arrange

            #region Act
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderStatusCode.IsTransient());
            Assert.IsTrue(orderStatusCode.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var orderStatusCode = GetValid(9);
            orderStatusCode.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, orderStatusCode.Name.Length);
            Assert.IsFalse(orderStatusCode.IsTransient());
            Assert.IsTrue(orderStatusCode.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests
        
        #region Level Tests

        /// <summary>
        /// Tests the Level with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestLevelWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Level = int.MaxValue;
            #endregion Arrange

            #region Act
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            OrderStatusCodeRepository.EnsurePersistent(record);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.Level);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Level with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestLevelWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Level = int.MinValue;
            #endregion Arrange

            #region Act
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            OrderStatusCodeRepository.EnsurePersistent(record);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.Level);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion Level Tests

        #region IsComplete Tests

        /// <summary>
        /// Tests the IsComplete is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsCompleteIsFalseSaves()
        {
            #region Arrange
            OrderStatusCode orderStatusCode = GetValid(9);
            orderStatusCode.IsComplete = false;
            #endregion Arrange

            #region Act
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderStatusCode.IsComplete);
            Assert.IsFalse(orderStatusCode.IsTransient());
            Assert.IsTrue(orderStatusCode.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsComplete is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsCompleteIsTrueSaves()
        {
            #region Arrange
            var orderStatusCode = GetValid(9);
            orderStatusCode.IsComplete = true;
            #endregion Arrange

            #region Act
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(orderStatusCode.IsComplete);
            Assert.IsFalse(orderStatusCode.IsTransient());
            Assert.IsTrue(orderStatusCode.IsValid());
            #endregion Assert
        }

        #endregion IsComplete Tests

        #region KfsStatus Tests

        /// <summary>
        /// Tests the KfsStatus is false saves.
        /// </summary>
        [TestMethod]
        public void TestKfsStatusIsFalseSaves()
        {
            #region Arrange
            OrderStatusCode orderStatusCode = GetValid(9);
            orderStatusCode.KfsStatus = false;
            #endregion Arrange

            #region Act
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderStatusCode.KfsStatus);
            Assert.IsFalse(orderStatusCode.IsTransient());
            Assert.IsTrue(orderStatusCode.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the KfsStatus is true saves.
        /// </summary>
        [TestMethod]
        public void TestKfsStatusIsTrueSaves()
        {
            #region Arrange
            var orderStatusCode = GetValid(9);
            orderStatusCode.KfsStatus = true;
            #endregion Arrange

            #region Act
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(orderStatusCode.KfsStatus);
            Assert.IsFalse(orderStatusCode.IsTransient());
            Assert.IsTrue(orderStatusCode.IsValid());
            #endregion Assert
        }

        #endregion KfsStatus Tests

        #region ShowInFilterList Tests

        /// <summary>
        /// Tests the ShowInFilterList is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowInFilterListIsFalseSaves()
        {
            #region Arrange
            OrderStatusCode orderStatusCode = GetValid(9);
            orderStatusCode.ShowInFilterList = false;
            #endregion Arrange

            #region Act
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderStatusCode.ShowInFilterList);
            Assert.IsFalse(orderStatusCode.IsTransient());
            Assert.IsTrue(orderStatusCode.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowInFilterList is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowInFilterListIsTrueSaves()
        {
            #region Arrange
            var orderStatusCode = GetValid(9);
            orderStatusCode.ShowInFilterList = true;
            #endregion Arrange

            #region Act
            OrderStatusCodeRepository.DbContext.BeginTransaction();
            OrderStatusCodeRepository.EnsurePersistent(orderStatusCode);
            OrderStatusCodeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(orderStatusCode.ShowInFilterList);
            Assert.IsFalse(orderStatusCode.IsTransient());
            Assert.IsTrue(orderStatusCode.IsValid());
            #endregion Assert
        }

        #endregion ShowInFilterList Tests
        
        
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

            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsComplete", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("KfsStatus", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Level", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("ShowInFilterList", "System.Boolean", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(OrderStatusCode));

        }

        #endregion Reflection of Database.	
		
		
    }
}