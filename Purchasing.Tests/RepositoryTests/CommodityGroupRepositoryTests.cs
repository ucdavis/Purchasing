using System;
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
    /// Entity Name:		CommodityGroup
    /// LookupFieldName:	GroupCode
    /// </summary>
    [TestClass]
    public class CommodityGroupRepositoryTests : AbstractRepositoryTests<CommodityGroup, Guid, CommodityGroupMap>
    {
        /// <summary>
        /// Gets or sets the CommodityGroup repository.
        /// </summary>
        /// <value>The CommodityGroup repository.</value>
        public IRepository<CommodityGroup> CommodityGroupRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="CommodityGroupRepositoryTests"/> class.
        /// </summary>
        public CommodityGroupRepositoryTests()
        {
            CommodityGroupRepository = new Repository<CommodityGroup>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override CommodityGroup GetValid(int? counter)
        {
            return CreateValidEntities.CommodityGroup(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<CommodityGroup> GetQuery(int numberAtEnd)
        {
            return CommodityGroupRepository.Queryable.Where(a => a.GroupCode.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(CommodityGroup entity, int counter)
        {
            Assert.AreEqual("GroupCode" + counter, entity.GroupCode);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(CommodityGroup entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.GroupCode);
                    break;
                case ARTAction.Restore:
                    entity.GroupCode = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.GroupCode;
                    entity.GroupCode = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            CommodityGroupRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            CommodityGroupRepository.DbContext.CommitTransaction();
        }

        [TestMethod]
        public override void CanUpdateEntity()
        {
            CanUpdateEntity(false);
        }

        #endregion Init and Overrides	
        
        #region GroupCode Tests

        #region Valid Tests

        /// <summary>
        /// Tests the GroupCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestGroupCodeWithNullValueSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.GroupCode = null;
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the GroupCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestGroupCodeWithEmptyStringSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.GroupCode = string.Empty;
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the GroupCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestGroupCodeWithOneSpaceSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.GroupCode = " ";
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the GroupCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestGroupCodeWithOneCharacterSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.GroupCode = "x";
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the GroupCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestGroupCodeWithLongValueSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.GroupCode = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, commodityGroup.GroupCode.Length);
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion GroupCode Tests
       
        #region Name Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with null value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithNullValueSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.Name = null;
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithEmptyStringSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.Name = string.Empty;
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with one space saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneSpaceSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.Name = " ";
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.Name = "x";
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.Name = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, commodityGroup.Name.Length);
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region SubGroupCode Tests

        #region Valid Tests

        /// <summary>
        /// Tests the SubGroupCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupCodeWithNullValueSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.SubGroupCode = null;
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupCodeWithEmptyStringSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.SubGroupCode = string.Empty;
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupCodeWithOneSpaceSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.SubGroupCode = " ";
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupCodeWithOneCharacterSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.SubGroupCode = "x";
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupCodeWithLongValueSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.SubGroupCode = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, commodityGroup.SubGroupCode.Length);
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion SubGroupCode Tests

        #region SubGroupName Tests

        #region Valid Tests

        /// <summary>
        /// Tests the SubGroupName with null value saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupNameWithNullValueSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.SubGroupName = null;
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupName with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupNameWithEmptyStringSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.SubGroupName = string.Empty;
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupName with one space saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupNameWithOneSpaceSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.SubGroupName = " ";
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupNameWithOneCharacterSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.SubGroupName = "x";
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubGroupName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestSubGroupNameWithLongValueSaves()
        {
            #region Arrange
            var commodityGroup = GetValid(9);
            commodityGroup.SubGroupName = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            CommodityGroupRepository.DbContext.BeginTransaction();
            CommodityGroupRepository.EnsurePersistent(commodityGroup);
            CommodityGroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, commodityGroup.SubGroupName.Length);
            Assert.IsFalse(commodityGroup.IsTransient());
            Assert.IsTrue(commodityGroup.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion SubGroupName Tests

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
            expectedFields.Add(new NameAndType("Id", "System.Guid", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SubGroupCode", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SubGroupName", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(CommodityGroup));

        }

        #endregion Reflection of Database.	
		
		
    }
}