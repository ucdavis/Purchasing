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
    /// Entity Name:		UnitOfMeasure
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class UnitOfMeasureRepositoryTests : AbstractRepositoryTests<UnitOfMeasure, string, UnitOfMeasureMap>
    {
        /// <summary>
        /// Gets or sets the UnitOfMeasure repository.
        /// </summary>
        /// <value>The UnitOfMeasure repository.</value>
        public IRepositoryWithTypedId<UnitOfMeasure, string> UnitOfMeasureRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfMeasureRepositoryTests"/> class.
        /// </summary>
        public UnitOfMeasureRepositoryTests()
        {
            UnitOfMeasureRepository = new RepositoryWithTypedId<UnitOfMeasure, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override UnitOfMeasure GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.UnitOfMeasure(counter);
            rtValue.Id = counter.HasValue ? counter.Value.ToString() : "99";

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<UnitOfMeasure> GetQuery(int numberAtEnd)
        {
            return UnitOfMeasureRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(UnitOfMeasure entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(UnitOfMeasure entity, ARTAction action)
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
            UnitOfMeasureRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            UnitOfMeasureRepository.DbContext.CommitTransaction();
        }
        public override void CanDeleteEntity()
        {
            return;
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
            UnitOfMeasure unitOfMeasure = null;
            try
            {
                #region Arrange
                unitOfMeasure = GetValid(9);
                unitOfMeasure.Name = null;
                #endregion Arrange

                #region Act
                UnitOfMeasureRepository.DbContext.BeginTransaction();
                UnitOfMeasureRepository.EnsurePersistent(unitOfMeasure);
                UnitOfMeasureRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(unitOfMeasure);
                var results = unitOfMeasure.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(UnitOfMeasuret.IsTransient());
                Assert.IsFalse(unitOfMeasure.IsValid());
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
            UnitOfMeasure unitOfMeasure = null;
            try
            {
                #region Arrange
                unitOfMeasure = GetValid(9);
                unitOfMeasure.Name = string.Empty;
                #endregion Arrange

                #region Act
                UnitOfMeasureRepository.DbContext.BeginTransaction();
                UnitOfMeasureRepository.EnsurePersistent(unitOfMeasure);
                UnitOfMeasureRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(unitOfMeasure);
                var results = unitOfMeasure.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(UnitOfMeasuret.IsTransient());
                Assert.IsFalse(unitOfMeasure.IsValid());
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
            UnitOfMeasure unitOfMeasure = null;
            try
            {
                #region Arrange
                unitOfMeasure = GetValid(9);
                unitOfMeasure.Name = " ";
                #endregion Arrange

                #region Act
                UnitOfMeasureRepository.DbContext.BeginTransaction();
                UnitOfMeasureRepository.EnsurePersistent(unitOfMeasure);
                UnitOfMeasureRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(unitOfMeasure);
                var results = unitOfMeasure.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(UnitOfMeasuret.IsTransient());
                Assert.IsFalse(unitOfMeasure.IsValid());
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
            UnitOfMeasure unitOfMeasure = null;
            try
            {
                #region Arrange
                unitOfMeasure = GetValid(9);
                unitOfMeasure.Name = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                UnitOfMeasureRepository.DbContext.BeginTransaction();
                UnitOfMeasureRepository.EnsurePersistent(unitOfMeasure);
                UnitOfMeasureRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(unitOfMeasure);
                Assert.AreEqual(50 + 1, unitOfMeasure.Name.Length);
                var results = unitOfMeasure.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "50"));
                //Assert.IsTrue(UnitOfMeasuret.IsTransient());
                Assert.IsFalse(unitOfMeasure.IsValid());
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
            var unitOfMeasure = GetValid(9);
            unitOfMeasure.Name = "x";
            #endregion Arrange

            #region Act
            UnitOfMeasureRepository.DbContext.BeginTransaction();
            UnitOfMeasureRepository.EnsurePersistent(unitOfMeasure);
            UnitOfMeasureRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(unitOfMeasure.IsTransient());
            Assert.IsTrue(unitOfMeasure.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var unitOfMeasure = GetValid(9);
            unitOfMeasure.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            UnitOfMeasureRepository.DbContext.BeginTransaction();
            UnitOfMeasureRepository.EnsurePersistent(unitOfMeasure);
            UnitOfMeasureRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, unitOfMeasure.Name.Length);
            Assert.IsFalse(unitOfMeasure.IsTransient());
            Assert.IsTrue(unitOfMeasure.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests    
        
        
        
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
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(UnitOfMeasure));

        }

        #endregion Reflection of Database.	
		
		
    }
}