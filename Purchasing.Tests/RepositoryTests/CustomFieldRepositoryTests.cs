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
    /// Entity Name:		CustomField
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class CustomFieldRepositoryTests : AbstractRepositoryTests<CustomField, int, CustomFieldMap>
    {
        /// <summary>
        /// Gets or sets the CustomField repository.
        /// </summary>
        /// <value>The CustomField repository.</value>
        public IRepository<CustomField> CustomFieldRepository { get; set; }
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFieldRepositoryTests"/> class.
        /// </summary>
        public CustomFieldRepositoryTests()
        {
            CustomFieldRepository = new Repository<CustomField>();
            OrganizationRepository = new RepositoryWithTypedId<Organization, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override CustomField GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.CustomField(counter);
            rtValue.Organization = OrganizationRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<CustomField> GetQuery(int numberAtEnd)
        {
            return CustomFieldRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(CustomField entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(CustomField entity, ARTAction action)
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

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            OrganizationRepository.DbContext.BeginTransaction();
            LoadOrganizations(3);
            OrganizationRepository.DbContext.CommitTransaction();
            CustomFieldRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            CustomFieldRepository.DbContext.CommitTransaction();
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
            CustomField customField = null;
            try
            {
                #region Arrange
                customField = GetValid(9);
                customField.Name = null;
                #endregion Arrange

                #region Act
                CustomFieldRepository.DbContext.BeginTransaction();
                CustomFieldRepository.EnsurePersistent(customField);
                CustomFieldRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(customField);
                var results = customField.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(customField.IsTransient());
                Assert.IsFalse(customField.IsValid());
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
            CustomField customField = null;
            try
            {
                #region Arrange
                customField = GetValid(9);
                customField.Name = string.Empty;
                #endregion Arrange

                #region Act
                CustomFieldRepository.DbContext.BeginTransaction();
                CustomFieldRepository.EnsurePersistent(customField);
                CustomFieldRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(customField);
                var results = customField.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(customField.IsTransient());
                Assert.IsFalse(customField.IsValid());
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
            CustomField customField = null;
            try
            {
                #region Arrange
                customField = GetValid(9);
                customField.Name = " ";
                #endregion Arrange

                #region Act
                CustomFieldRepository.DbContext.BeginTransaction();
                CustomFieldRepository.EnsurePersistent(customField);
                CustomFieldRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(customField);
                var results = customField.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(customField.IsTransient());
                Assert.IsFalse(customField.IsValid());
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
            var customField = GetValid(9);
            customField.Name = "x";
            #endregion Arrange

            #region Act
            CustomFieldRepository.DbContext.BeginTransaction();
            CustomFieldRepository.EnsurePersistent(customField);
            CustomFieldRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(customField.IsTransient());
            Assert.IsTrue(customField.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var customField = GetValid(9);
            customField.Name = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            CustomFieldRepository.DbContext.BeginTransaction();
            CustomFieldRepository.EnsurePersistent(customField);
            CustomFieldRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, customField.Name.Length);
            Assert.IsFalse(customField.IsTransient());
            Assert.IsTrue(customField.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Rank Tests

        /// <summary>
        /// Tests the Rank with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestRankWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Rank = int.MaxValue;
            #endregion Arrange

            #region Act
            CustomFieldRepository.DbContext.BeginTransaction();
            CustomFieldRepository.EnsurePersistent(record);
            CustomFieldRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.Rank);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Rank with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestRankWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Rank = int.MinValue;
            #endregion Arrange

            #region Act
            CustomFieldRepository.DbContext.BeginTransaction();
            CustomFieldRepository.EnsurePersistent(record);
            CustomFieldRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.Rank);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion Rank Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            CustomField customField = GetValid(9);
            customField.IsActive = false;
            #endregion Arrange

            #region Act
            CustomFieldRepository.DbContext.BeginTransaction();
            CustomFieldRepository.EnsurePersistent(customField);
            CustomFieldRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(customField.IsActive);
            Assert.IsFalse(customField.IsTransient());
            Assert.IsTrue(customField.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var customField = GetValid(9);
            customField.IsActive = true;
            #endregion Arrange

            #region Act
            CustomFieldRepository.DbContext.BeginTransaction();
            CustomFieldRepository.EnsurePersistent(customField);
            CustomFieldRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(customField.IsActive);
            Assert.IsFalse(customField.IsTransient());
            Assert.IsTrue(customField.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region IsRequired Tests

        /// <summary>
        /// Tests the IsRequired is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsRequiredIsFalseSaves()
        {
            #region Arrange
            CustomField customField = GetValid(9);
            customField.IsRequired = false;
            #endregion Arrange

            #region Act
            CustomFieldRepository.DbContext.BeginTransaction();
            CustomFieldRepository.EnsurePersistent(customField);
            CustomFieldRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(customField.IsRequired);
            Assert.IsFalse(customField.IsTransient());
            Assert.IsTrue(customField.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsRequired is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsRequiredIsTrueSaves()
        {
            #region Arrange
            var customField = GetValid(9);
            customField.IsRequired = true;
            #endregion Arrange

            #region Act
            CustomFieldRepository.DbContext.BeginTransaction();
            CustomFieldRepository.EnsurePersistent(customField);
            CustomFieldRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(customField.IsRequired);
            Assert.IsFalse(customField.IsTransient());
            Assert.IsTrue(customField.IsValid());
            #endregion Assert
        }

        #endregion IsRequired Tests

        #region Organization Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestCustomFieldsFieldOrganizationWithAValueOfNullDoesNotSave()
        {
            CustomField record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Organization = null;
                #endregion Arrange

                #region Act
                CustomFieldRepository.DbContext.BeginTransaction();
                CustomFieldRepository.EnsurePersistent(record);
                CustomFieldRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Organization, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Organization field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestCustomFieldNewOrganizationDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Organization = new Organization();
                thisFar = true;
                #endregion Arrange

                #region Act
                CustomFieldRepository.DbContext.BeginTransaction();
                CustomFieldRepository.EnsurePersistent(record);
                CustomFieldRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Organization, Entity: Purchasing.Core.Domain.Organization", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        public void TestCustomFieldWithExistingOrganizationSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Organization = OrganizationRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            CustomFieldRepository.DbContext.BeginTransaction();
            CustomFieldRepository.EnsurePersistent(record);
            CustomFieldRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.Organization.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Organization Tests

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

            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("IsRequired", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Organization", "Purchasing.Core.Domain.Organization", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Rank", "System.Int32", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(CustomField));

        }

        #endregion Reflection of Database.	
		
		
    }
}