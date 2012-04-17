using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Commodity
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class CommodityRepositoryTests : AbstractRepositoryTests<Commodity, string, CommodityMap>
    {
        /// <summary>
        /// Gets or sets the Commodity repository.
        /// </summary>
        /// <value>The Commodity repository.</value>
        public IRepository<Commodity> CommodityRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="CommodityRepositoryTests"/> class.
        /// </summary>
        public CommodityRepositoryTests()
        {
            CommodityRepository = new Repository<Commodity>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Commodity GetValid(int? counter)
        {
            return CreateValidEntities.Commodity(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Commodity> GetQuery(int numberAtEnd)
        {
            return CommodityRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Commodity entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Commodity entity, ARTAction action)
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
            CommodityRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            CommodityRepository.DbContext.CommitTransaction();
        }

        [TestMethod]
        public override void CanUpdateEntity()
        {
            CanUpdateEntity(false);
        }

        #endregion Init and Overrides	
        
        #region Name Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with null value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithNullValueSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.Name = null;
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithEmptyStringSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.Name = string.Empty;
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with one space saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneSpaceSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.Name = " ";
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.Name = "x";
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.Name = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, commodity.Name.Length);
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region GroupCode Tests


        #region Valid Tests

        /// <summary>
        /// Tests the GroupCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestGroupCodeWithNullValueSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.GroupCode = null;
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the GroupCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestGroupCodeWithEmptyStringSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.GroupCode = string.Empty;
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the GroupCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestGroupCodeWithOneSpaceSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.GroupCode = " ";
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the GroupCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestGroupCodeWithOneCharacterSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.GroupCode = "x";
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the GroupCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestGroupCodeWithLongValueSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.GroupCode = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, commodity.GroupCode.Length);
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion GroupCode Tests

        #region SubGroupCode Tests

        #region Valid Tests

        /// <summary>
        /// Tests the SubGroupCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupCodeWithNullValueSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.SubGroupCode = null;
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupCodeWithEmptyStringSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.SubGroupCode = string.Empty;
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupCodeWithOneSpaceSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.SubGroupCode = " ";
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupCodeWithOneCharacterSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.SubGroupCode = "x";
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupCodeWithLongValueSaves()
        {
            #region Arrange
            var commodity = GetValid(9);
            commodity.SubGroupCode = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            CommodityRepository.DbContext.BeginTransaction();
            CommodityRepository.EnsurePersistent(commodity);
            CommodityRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, commodity.SubGroupCode.Length);
            Assert.IsFalse(commodity.IsTransient());
            Assert.IsTrue(commodity.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion SubGroupCode Tests

        #region NameAndId Tests

        [TestMethod]
        public void TestNameAndId()
        {
            #region Arrange
            var record = CreateValidEntities.Commodity(3);
            #endregion Arrange

            #region Act
            var result = record.NameAndId;
            #endregion Act

            #region Assert
            Assert.AreEqual("Name3 (3)", result);
            #endregion Assert		
        }

        #endregion NameAndId Tests

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
            expectedFields.Add(new NameAndType("GroupCode", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("NameAndId", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SubGroupCode", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Commodity));

        }

        #endregion Reflection of Database.	
		
		
    }
}