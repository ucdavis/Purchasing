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
    /// Entity Name:		Building
    /// LookupFieldName:	CampusCode
    /// </summary>
    [TestClass]
    public class BuildingRepositoryTests : AbstractRepositoryTests<Building, string, BuildingMap>
    {
        /// <summary>
        /// Gets or sets the Building repository.
        /// </summary>
        /// <value>The Building repository.</value>
        public IRepositoryWithTypedId<Building, string> BuildingRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingRepositoryTests"/> class.
        /// </summary>
        public BuildingRepositoryTests()
        {
            BuildingRepository = new RepositoryWithTypedId<Building, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Building GetValid(int? counter)
        {
            return CreateValidEntities.Building(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Building> GetQuery(int numberAtEnd)
        {
            return BuildingRepository.Queryable.Where(a => a.CampusCode.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Building entity, int counter)
        {
            Assert.AreEqual("CampusCode" + counter, entity.CampusCode);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Building entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.CampusCode);
                    break;
                case ARTAction.Restore:
                    entity.CampusCode = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.CampusCode;
                    entity.CampusCode = updateValue;
                    break;
            }
        }

        public override void CanUpdateEntity()
        {
            base.CanUpdateEntity(false);
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            BuildingRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            BuildingRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region CampusCode Tests

        #region Valid Tests

        /// <summary>
        /// Tests the CampusCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestCampusCodeWithNullValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusCode = null;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestCampusCodeWithEmptyStringSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusCode = string.Empty;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestCampusCodeWithOneSpaceSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusCode = " ";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCampusCodeWithOneCharacterSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusCode = "x";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCampusCodeWithLongValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusCode = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, building.CampusCode.Length);
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion CampusCode Tests

        #region BuildingCode Tests

        #region Valid Tests

        /// <summary>
        /// Tests the BuildingCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingCodeWithNullValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.BuildingCode = null;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BuildingCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingCodeWithEmptyStringSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.BuildingCode = string.Empty;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BuildingCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingCodeWithOneSpaceSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.BuildingCode = " ";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BuildingCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingCodeWithOneCharacterSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.BuildingCode = "x";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BuildingCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingCodeWithLongValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.BuildingCode = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, building.BuildingCode.Length);
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion BuildingCode Tests
        
        #region CampusName Tests

        #region Valid Tests

        /// <summary>
        /// Tests the CampusName with null value saves.
        /// </summary>
        [TestMethod]
        public void TestCampusNameWithNullValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusName = null;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusName with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestCampusNameWithEmptyStringSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusName = string.Empty;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusName with one space saves.
        /// </summary>
        [TestMethod]
        public void TestCampusNameWithOneSpaceSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusName = " ";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCampusNameWithOneCharacterSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusName = "x";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCampusNameWithLongValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusName = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, building.CampusName.Length);
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion CampusName Tests
        
        #region CampusShortName Tests

        #region Valid Tests

        /// <summary>
        /// Tests the CampusShortName with null value saves.
        /// </summary>
        [TestMethod]
        public void TestCampusShortNameWithNullValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusShortName = null;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusShortName with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestCampusShortNameWithEmptyStringSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusShortName = string.Empty;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusShortName with one space saves.
        /// </summary>
        [TestMethod]
        public void TestCampusShortNameWithOneSpaceSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusShortName = " ";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusShortName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCampusShortNameWithOneCharacterSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusShortName = "x";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusShortName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCampusShortNameWithLongValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusShortName = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, building.CampusShortName.Length);
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion CampusShortName Tests
        
        #region CampusTypeCode Tests

        #region Valid Tests

        /// <summary>
        /// Tests the CampusTypeCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestCampusTypeCodeWithNullValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusTypeCode = null;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusTypeCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestCampusTypeCodeWithEmptyStringSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusTypeCode = string.Empty;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusTypeCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestCampusTypeCodeWithOneSpaceSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusTypeCode = " ";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusTypeCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCampusTypeCodeWithOneCharacterSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusTypeCode = "x";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CampusTypeCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCampusTypeCodeWithLongValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.CampusTypeCode = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, building.CampusTypeCode.Length);
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion CampusTypeCode Tests
        
        #region BuildingName Tests

        #region Valid Tests

        /// <summary>
        /// Tests the BuildingName with null value saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingNameWithNullValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.BuildingName = null;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BuildingName with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingNameWithEmptyStringSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.BuildingName = string.Empty;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BuildingName with one space saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingNameWithOneSpaceSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.BuildingName = " ";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BuildingName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingNameWithOneCharacterSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.BuildingName = "x";
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BuildingName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingNameWithLongValueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.BuildingName = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, building.BuildingName.Length);
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }


        #endregion Valid Tests
        #endregion BuildingName Tests
        
        #region LastUpdateDate Tests

        /// <summary>
        /// Tests the LastUpdateDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestLastUpdateDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Building record = GetValid(99);
            record.LastUpdateDate = compareDate;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(record);
            BuildingRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastUpdateDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the LastUpdateDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestLastUpdateDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.LastUpdateDate = compareDate;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(record);
            BuildingRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastUpdateDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastUpdateDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestLastUpdateDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.LastUpdateDate = compareDate;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(record);
            BuildingRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastUpdateDate);
            #endregion Assert
        }
        #endregion LastUpdateDate Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            Building building = GetValid(9);
            building.IsActive = false;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(building.IsActive);
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var building = GetValid(9);
            building.IsActive = true;
            #endregion Arrange

            #region Act
            BuildingRepository.DbContext.BeginTransaction();
            BuildingRepository.EnsurePersistent(building);
            BuildingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(building.IsActive);
            Assert.IsFalse(building.IsTransient());
            Assert.IsTrue(building.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

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
            expectedFields.Add(new NameAndType("BuildingCode", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("BuildingName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("CampusCode", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("CampusName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("CampusShortName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("CampusTypeCode", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("LastUpdateDate", "System.DateTime", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Building));

        }

        #endregion Reflection of Database.	
		
		
    }
}