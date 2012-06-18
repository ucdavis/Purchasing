using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
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
    /// Entity Name:		CustomFieldAnswer
    /// LookupFieldName:	Answer
    /// </summary>
    [TestClass]
    public class CustomFieldAnswerRepositoryTests : AbstractRepositoryTests<CustomFieldAnswer, int, CustomFieldAnswerMap>
    {
        /// <summary>
        /// Gets or sets the CustomFieldAnswer repository.
        /// </summary>
        /// <value>The CustomFieldAnswer repository.</value>
        public IRepository<CustomFieldAnswer> CustomFieldAnswerRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFieldAnswerRepositoryTests"/> class.
        /// </summary>
        public CustomFieldAnswerRepositoryTests()
        {
            CustomFieldAnswerRepository = new Repository<CustomFieldAnswer>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override CustomFieldAnswer GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.CustomFieldAnswer(counter);
            rtValue.Order = Repository.OfType<Order>().Queryable.First();
            rtValue.CustomField = Repository.OfType<CustomField>().Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<CustomFieldAnswer> GetQuery(int numberAtEnd)
        {
            return CustomFieldAnswerRepository.Queryable.Where(a => a.Answer.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(CustomFieldAnswer entity, int counter)
        {
            Assert.AreEqual("Answer" + counter, entity.Answer);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(CustomFieldAnswer entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Answer);
                    break;
                case ARTAction.Restore:
                    entity.Answer = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Answer;
                    entity.Answer = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Order>().DbContext.BeginTransaction();
            LoadOrganizations(3);
            LoadStatusCodes(3);
            LoadCustomField(3);
            LoadOrders(3);
            Repository.OfType<Order>().DbContext.CommitTransaction();
            CustomFieldAnswerRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            CustomFieldAnswerRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Answer Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Answer with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswerWithNullValueDoesNotSave()
        {
            CustomFieldAnswer customFieldAnswer = null;
            try
            {
                #region Arrange
                customFieldAnswer = GetValid(9);
                customFieldAnswer.Answer = null;
                #endregion Arrange

                #region Act
                CustomFieldAnswerRepository.DbContext.BeginTransaction();
                CustomFieldAnswerRepository.EnsurePersistent(customFieldAnswer);
                CustomFieldAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(customFieldAnswer);
                var results = customFieldAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Answer"));
                Assert.IsTrue(customFieldAnswer.IsTransient());
                Assert.IsFalse(customFieldAnswer.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Answer with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswerWithEmptyStringDoesNotSave()
        {
            CustomFieldAnswer customFieldAnswer = null;
            try
            {
                #region Arrange
                customFieldAnswer = GetValid(9);
                customFieldAnswer.Answer = string.Empty;
                #endregion Arrange

                #region Act
                CustomFieldAnswerRepository.DbContext.BeginTransaction();
                CustomFieldAnswerRepository.EnsurePersistent(customFieldAnswer);
                CustomFieldAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(customFieldAnswer);
                var results = customFieldAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Answer"));
                Assert.IsTrue(customFieldAnswer.IsTransient());
                Assert.IsFalse(customFieldAnswer.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Answer with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswerWithSpacesOnlyDoesNotSave()
        {
            CustomFieldAnswer customFieldAnswer = null;
            try
            {
                #region Arrange
                customFieldAnswer = GetValid(9);
                customFieldAnswer.Answer = " ";
                #endregion Arrange

                #region Act
                CustomFieldAnswerRepository.DbContext.BeginTransaction();
                CustomFieldAnswerRepository.EnsurePersistent(customFieldAnswer);
                CustomFieldAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(customFieldAnswer);
                var results = customFieldAnswer.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Answer"));
                Assert.IsTrue(customFieldAnswer.IsTransient());
                Assert.IsFalse(customFieldAnswer.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Answer with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithOneCharacterSaves()
        {
            #region Arrange
            var customFieldAnswer = GetValid(9);
            customFieldAnswer.Answer = "x";
            #endregion Arrange

            #region Act
            CustomFieldAnswerRepository.DbContext.BeginTransaction();
            CustomFieldAnswerRepository.EnsurePersistent(customFieldAnswer);
            CustomFieldAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(customFieldAnswer.IsTransient());
            Assert.IsTrue(customFieldAnswer.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithLongValueSaves()
        {
            #region Arrange
            var customFieldAnswer = GetValid(9);
            customFieldAnswer.Answer = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            CustomFieldAnswerRepository.DbContext.BeginTransaction();
            CustomFieldAnswerRepository.EnsurePersistent(customFieldAnswer);
            CustomFieldAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, customFieldAnswer.Answer.Length);
            Assert.IsFalse(customFieldAnswer.IsTransient());
            Assert.IsTrue(customFieldAnswer.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Answer Tests

        #region Order Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestCustomFieldAnswersFieldOrderWithAValueOfNullDoesNotSave()
        {
            CustomFieldAnswer record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Order = null;
                #endregion Arrange

                #region Act
                CustomFieldAnswerRepository.DbContext.BeginTransaction();
                CustomFieldAnswerRepository.EnsurePersistent(record);
                CustomFieldAnswerRepository.DbContext.CommitTransaction();
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
        public void TestCustomFieldAnswerNewOrderDoesNotSave()
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
                CustomFieldAnswerRepository.DbContext.BeginTransaction();
                CustomFieldAnswerRepository.EnsurePersistent(record);
                CustomFieldAnswerRepository.DbContext.CommitTransaction();
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
        public void TestCustomFieldAnswerWithExistingOrderSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = Repository.OfType<Order>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            CustomFieldAnswerRepository.DbContext.BeginTransaction();
            CustomFieldAnswerRepository.EnsurePersistent(record);
            CustomFieldAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Order.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Order Tests


        #region CustomField Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestCustomFieldAnswersFieldCustomFieldWithAValueOfNullDoesNotSave()
        {
            CustomFieldAnswer record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.CustomField = null;
                #endregion Arrange

                #region Act
                CustomFieldAnswerRepository.DbContext.BeginTransaction();
                CustomFieldAnswerRepository.EnsurePersistent(record);
                CustomFieldAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.CustomField, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The CustomField field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestCustomFieldAnswerNewCustomFieldDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.CustomField = new CustomField();
                thisFar = true;
                #endregion Arrange

                #region Act
                CustomFieldAnswerRepository.DbContext.BeginTransaction();
                CustomFieldAnswerRepository.EnsurePersistent(record);
                CustomFieldAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.CustomField, Entity: Purchasing.Core.Domain.CustomField", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestCustomFieldAnswerWithExistingCustomFieldSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.CustomField = Repository.OfType<CustomField>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            CustomFieldAnswerRepository.DbContext.BeginTransaction();
            CustomFieldAnswerRepository.EnsurePersistent(record);
            CustomFieldAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.CustomField.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion CustomField Tests


        
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
            expectedFields.Add(new NameAndType("Answer", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("CustomField", "Purchasing.Core.Domain.CustomField", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
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

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(CustomFieldAnswer));

        }

        #endregion Reflection of Database.	
		
		
    }
}