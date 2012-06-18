using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		ShippingType
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class ShippingTypeRepositoryTests : AbstractRepositoryTests<ShippingType, string, ShippingTypeMap>
    {
        /// <summary>
        /// Gets or sets the ShippingType repository.
        /// </summary>
        /// <value>The ShippingType repository.</value>
        public IRepositoryWithTypedId<ShippingType, string> ShippingTypeRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingTypeRepositoryTests"/> class.
        /// </summary>
        public ShippingTypeRepositoryTests()
        {
            ShippingTypeRepository = new RepositoryWithTypedId<ShippingType, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ShippingType GetValid(int? counter)
        {
            return CreateValidEntities.ShippingType(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ShippingType> GetQuery(int numberAtEnd)
        {
            return ShippingTypeRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ShippingType entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ShippingType entity, ARTAction action)
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
            ShippingTypeRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ShippingTypeRepository.DbContext.CommitTransaction();
        }

        public override void CanDeleteEntity()
        {
            return;
        }

        public override void CanUpdateEntity()
        {
            CanUpdateEntity(false);
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
            ShippingType shippingType = null;
            try
            {
                #region Arrange
                shippingType = GetValid(9);
                shippingType.Name = null;
                #endregion Arrange

                #region Act
                ShippingTypeRepository.DbContext.BeginTransaction();
                ShippingTypeRepository.EnsurePersistent(shippingType);
                ShippingTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(shippingType);
                var results = shippingType.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(shippingType.IsTransient());
                Assert.IsFalse(shippingType.IsValid());
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
            ShippingType shippingType = null;
            try
            {
                #region Arrange
                shippingType = GetValid(9);
                shippingType.Name = string.Empty;
                #endregion Arrange

                #region Act
                ShippingTypeRepository.DbContext.BeginTransaction();
                ShippingTypeRepository.EnsurePersistent(shippingType);
                ShippingTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(shippingType);
                var results = shippingType.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(shippingType.IsTransient());
                Assert.IsFalse(shippingType.IsValid());
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
            ShippingType shippingType = null;
            try
            {
                #region Arrange
                shippingType = GetValid(9);
                shippingType.Name = " ";
                #endregion Arrange

                #region Act
                ShippingTypeRepository.DbContext.BeginTransaction();
                ShippingTypeRepository.EnsurePersistent(shippingType);
                ShippingTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(shippingType);
                var results = shippingType.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(shippingType.IsTransient());
                Assert.IsFalse(shippingType.IsValid());
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
            ShippingType shippingType = null;
            try
            {
                #region Arrange
                shippingType = GetValid(9);
                shippingType.Name = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                ShippingTypeRepository.DbContext.BeginTransaction();
                ShippingTypeRepository.EnsurePersistent(shippingType);
                ShippingTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(shippingType);
                Assert.AreEqual(50 + 1, shippingType.Name.Length);
                var results = shippingType.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "50"));
                //Assert.IsTrue(shippingType.IsTransient());
                Assert.IsFalse(shippingType.IsValid());
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
            var shippingType = GetValid(9);
            shippingType.Name = "x";
            #endregion Arrange

            #region Act
            ShippingTypeRepository.DbContext.BeginTransaction();
            ShippingTypeRepository.EnsurePersistent(shippingType);
            ShippingTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(shippingType.IsTransient());
            Assert.IsTrue(shippingType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var shippingType = GetValid(9);
            shippingType.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            ShippingTypeRepository.DbContext.BeginTransaction();
            ShippingTypeRepository.EnsurePersistent(shippingType);
            ShippingTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, shippingType.Name.Length);
            Assert.IsFalse(shippingType.IsTransient());
            Assert.IsTrue(shippingType.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Warning Tests


        #region Valid Tests

        /// <summary>
        /// Tests the Warning with null value saves.
        /// </summary>
        [TestMethod]
        public void TestWarningWithNullValueSaves()
        {
            #region Arrange
            var shippingType = GetValid(9);
            shippingType.Warning = null;
            #endregion Arrange

            #region Act
            ShippingTypeRepository.DbContext.BeginTransaction();
            ShippingTypeRepository.EnsurePersistent(shippingType);
            ShippingTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(shippingType.IsTransient());
            Assert.IsTrue(shippingType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Warning with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestWarningWithEmptyStringSaves()
        {
            #region Arrange
            var shippingType = GetValid(9);
            shippingType.Warning = string.Empty;
            #endregion Arrange

            #region Act
            ShippingTypeRepository.DbContext.BeginTransaction();
            ShippingTypeRepository.EnsurePersistent(shippingType);
            ShippingTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(shippingType.IsTransient());
            Assert.IsTrue(shippingType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Warning with one space saves.
        /// </summary>
        [TestMethod]
        public void TestWarningWithOneSpaceSaves()
        {
            #region Arrange
            var shippingType = GetValid(9);
            shippingType.Warning = " ";
            #endregion Arrange

            #region Act
            ShippingTypeRepository.DbContext.BeginTransaction();
            ShippingTypeRepository.EnsurePersistent(shippingType);
            ShippingTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(shippingType.IsTransient());
            Assert.IsTrue(shippingType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Warning with one character saves.
        /// </summary>
        [TestMethod]
        public void TestWarningWithOneCharacterSaves()
        {
            #region Arrange
            var shippingType = GetValid(9);
            shippingType.Warning = "x";
            #endregion Arrange

            #region Act
            ShippingTypeRepository.DbContext.BeginTransaction();
            ShippingTypeRepository.EnsurePersistent(shippingType);
            ShippingTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(shippingType.IsTransient());
            Assert.IsTrue(shippingType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Warning with long value saves.
        /// </summary>
        [TestMethod]
        public void TestWarningWithLongValueSaves()
        {
            #region Arrange
            var shippingType = GetValid(9);
            shippingType.Warning = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            ShippingTypeRepository.DbContext.BeginTransaction();
            ShippingTypeRepository.EnsurePersistent(shippingType);
            ShippingTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, shippingType.Warning.Length);
            Assert.IsFalse(shippingType.IsTransient());
            Assert.IsTrue(shippingType.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Warning Tests
      
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
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Warning", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ShippingType));

        }

        #endregion Reflection of Database.	
		
		
    }
}