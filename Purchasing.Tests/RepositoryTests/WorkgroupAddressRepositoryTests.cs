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
    /// Entity Name:		WorkgroupAddress
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class WorkgroupAddressRepositoryTests : AbstractRepositoryTests<WorkgroupAddress, int, WorkgroupAddressMap>
    {
        /// <summary>
        /// Gets or sets the WorkgroupAddress repository.
        /// </summary>
        /// <value>The WorkgroupAddress repository.</value>
        public IRepository<WorkgroupAddress> WorkgroupAddressRepository { get; set; }

        public IRepositoryWithTypedId<Building, string> BuildingRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkgroupAddressRepositoryTests"/> class.
        /// </summary>
        public WorkgroupAddressRepositoryTests()
        {
            WorkgroupAddressRepository = new Repository<WorkgroupAddress>();
            BuildingRepository = new RepositoryWithTypedId<Building, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override WorkgroupAddress GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.WorkgroupAddress(counter);
            rtValue.Workgroup = Repository.OfType<Workgroup>().Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<WorkgroupAddress> GetQuery(int numberAtEnd)
        {
            return WorkgroupAddressRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(WorkgroupAddress entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(WorkgroupAddress entity, ARTAction action)
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
            Repository.OfType<Workgroup>().DbContext.BeginTransaction();
            LoadOrganizations(3);
            LoadWorkgroups(3);
            LoadBuildings(3);
            Repository.OfType<Workgroup>().DbContext.CommitTransaction();

            WorkgroupAddressRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region Workgroup Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestWorkgroupAddresssFieldWorkgroupWithAValueOfNullDoesNotSave()
        {
            WorkgroupAddress record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Workgroup = null;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(record);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Workgroup, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Workgroup field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestWorkgroupAddressNewWorkgroupDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Workgroup = new Workgroup();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(record);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.Workgroup, Entity: Purchasing.Core.Domain.Workgroup", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestWorkgroupAddressWithExistingWorkgroupSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Workgroup = Repository.OfType<Workgroup>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(record);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Workgroup.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Workgroup Tests

        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Name = null;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
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
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Name = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
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
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Name = " ";
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
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
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Name = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                Assert.AreEqual(50 + 1, workgroupAddress.Name.Length);
                var results = workgroupAddress.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "50"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
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
            var workgroupAddress = GetValid(9);
            workgroupAddress.Name = "x";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, workgroupAddress.Name.Length);
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Building Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Building with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestBuildingWithTooLongValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Building = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                Assert.AreEqual(50 + 1, workgroupAddress.Building.Length);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Building", "50"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Building with null value saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingWithNullValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Building = null;
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Building with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingWithEmptyStringSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Building = string.Empty;
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Building with one space saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingWithOneSpaceSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Building = " ";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Building with one character saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Building = "x";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Building with long value saves.
        /// </summary>
        [TestMethod]
        public void TestBuildingWithLongValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Building = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, workgroupAddress.Building.Length);
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Building Tests

        #region Room Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Room with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestRoomWithTooLongValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Room = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                Assert.AreEqual(50 + 1, workgroupAddress.Room.Length);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Room", "50"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Room with null value saves.
        /// </summary>
        [TestMethod]
        public void TestRoomWithNullValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Room = null;
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Room with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestRoomWithEmptyStringSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Room = string.Empty;
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Room with one space saves.
        /// </summary>
        [TestMethod]
        public void TestRoomWithOneSpaceSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Room = " ";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Room with one character saves.
        /// </summary>
        [TestMethod]
        public void TestRoomWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Room = "x";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Room with long value saves.
        /// </summary>
        [TestMethod]
        public void TestRoomWithLongValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Room = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, workgroupAddress.Room.Length);
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Room Tests
     
        #region Address Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Address with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddressWithNullValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Address = null;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Address"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Address with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddressWithEmptyStringDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Address = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Address"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Address with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddressWithSpacesOnlyDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Address = " ";
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Address"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Address with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddressWithTooLongValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Address = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                Assert.AreEqual(100 + 1, workgroupAddress.Address.Length);
                var results = workgroupAddress.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Address", "100"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Address with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAddressWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Address = "x";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAddressWithLongValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Address = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, workgroupAddress.Address.Length);
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Address Tests

        #region City Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the City with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithNullValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.City = null;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "City"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the City with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithEmptyStringDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.City = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "City"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the City with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithSpacesOnlyDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.City = " ";
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "City"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the City with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithTooLongValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.City = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                Assert.AreEqual(100 + 1, workgroupAddress.City.Length);
                var results = workgroupAddress.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "City", "100"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the City with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.City = "x";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the City with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithLongValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.City = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, workgroupAddress.City.Length);
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion City Tests

        #region State Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the State with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithNullValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.State = null;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "State"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the State with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithEmptyStringDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.State = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "State"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the State with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithSpacesOnlyDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.State = " ";
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "State"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the State with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithTooLongValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.State = "x".RepeatTimes((2 + 1));
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                Assert.AreEqual(2 + 1, workgroupAddress.State.Length);
                var results = workgroupAddress.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "State", "2"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the State with one character saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.State = "x";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the State with long value saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithLongValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.State = "x".RepeatTimes(2);
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, workgroupAddress.State.Length);
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion State Tests

        #region Zip Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Zip with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithNullValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Zip = null;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Zip"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Zip with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithEmptyStringDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Zip = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Zip"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Zip with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithSpacesOnlyDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Zip = " ";
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Zip"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Zip with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithTooLongValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Zip = "12345-12345";
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                Assert.AreEqual(10 + 1, workgroupAddress.Zip.Length);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The field Zip must be a string with a maximum length of 10.", "Zip must be ##### or #####-####");
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestZipWithValidZipCharacterSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Zip = "11111";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Zip with long value saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithLongValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Zip = "12345-1234";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, workgroupAddress.Zip.Length);
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Zip Tests

        #region Phone Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Phone with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPhoneWithTooLongValueDoesNotSave()
        {
            WorkgroupAddress workgroupAddress = null;
            try
            {
                #region Arrange
                workgroupAddress = GetValid(9);
                workgroupAddress.Phone = "x".RepeatTimes((15 + 1));
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupAddress);
                Assert.AreEqual(15 + 1, workgroupAddress.Phone.Length);
                var results = workgroupAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Phone", "15"));
                Assert.IsTrue(workgroupAddress.IsTransient());
                Assert.IsFalse(workgroupAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Phone with null value saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneWithNullValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Phone = null;
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Phone with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneWithEmptyStringSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Phone = string.Empty;
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Phone with one space saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneWithOneSpaceSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Phone = " ";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Phone with one character saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Phone = "x";
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Phone with long value saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneWithLongValueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.Phone = "x".RepeatTimes(15);
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(15, workgroupAddress.Phone.Length);
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Phone Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            WorkgroupAddress workgroupAddress = GetValid(9);
            workgroupAddress.IsActive = false;
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupAddress.IsActive);
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var workgroupAddress = GetValid(9);
            workgroupAddress.IsActive = true;
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(workgroupAddress);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroupAddress.IsActive);
            Assert.IsFalse(workgroupAddress.IsTransient());
            Assert.IsTrue(workgroupAddress.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region BuildingCode Tests

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestWorkgroupAddressNewBuildingCodeDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.BuildingCode = new Building();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupAddressRepository.DbContext.BeginTransaction();
                WorkgroupAddressRepository.EnsurePersistent(record);
                WorkgroupAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.Building, Entity: Purchasing.Core.Domain.Building", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestWorkgroupAddressWithExistingBuildingCodeSaves()
        {
            #region Arrange

            var record = GetValid(9);
            record.BuildingCode = BuildingRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(record);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.BuildingCode.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        //Delete this one if not nullable
        [TestMethod]
        public void TestWorkgroupAddressWithNullBuildingCodeSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.BuildingCode = null;
            #endregion Arrange

            #region Act
            WorkgroupAddressRepository.DbContext.BeginTransaction();
            WorkgroupAddressRepository.EnsurePersistent(record);
            WorkgroupAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.BuildingCode);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion BuildingCode Tests

        #region DisplayName
                
        [TestMethod]
        public void TestDisplayName1()
        {
            #region Arrange
            var record = CreateValidEntities.WorkgroupAddress(3);
            #endregion Arrange

            #region Act
            var result = record.DisplayName;
            #endregion Act

            #region Assert
            Assert.AreEqual("Name3 (Address3, City3 CA)", result);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDisplayName2()
        {
            #region Arrange
            var record = CreateValidEntities.WorkgroupAddress(3);
            record.Address = "Address abcdefghijklmnopqrstuvwzyz 1234567890";
            record.City = "City abcdefghijklmnopqrstuvwzyz 1234567890";
            #endregion Arrange

            #region Act
            var result = record.DisplayName;
            #endregion Act

            #region Assert
            Assert.AreEqual("Name3 (Address abcdefghijklmn..., City abcdefghijklmnopqrstuvwzyz 1234567890 CA)", result);
            #endregion Assert
        }
        #endregion DisplayName
        #region Constructor Tests


        [TestMethod]
        public void TestConstructor()
        {
            #region Arrange
            var record = new WorkgroupAddress();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.IsTrue(record.IsActive);
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
            expectedFields.Add(new NameAndType("Address", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)100)]"
            }));
            expectedFields.Add(new NameAndType("Building", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("BuildingCode", "Purchasing.Core.Domain.Building", new List<string>()));
            expectedFields.Add(new NameAndType("City", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)100)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Phone", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)15)]"
            }));
            expectedFields.Add(new NameAndType("Room", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("State", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)2)]"
            }));
            expectedFields.Add(new NameAndType("Workgroup", "Purchasing.Core.Domain.Workgroup", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Zip", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RegularExpressionAttribute(\"^\\d{5}$|^\\d{5}-\\d{4}$\", ErrorMessage = \"Zip must be ##### or #####-####\")]",
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(WorkgroupAddress));

        }

        #endregion Reflection of Database.	
		
		
    }
}