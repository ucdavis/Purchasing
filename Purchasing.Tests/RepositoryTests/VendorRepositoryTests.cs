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
    /// Entity Name:		Vendor
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class VendorRepositoryTests : AbstractRepositoryTests<Vendor, string, VendorMap>
    {
        /// <summary>
        /// Gets or sets the Vendor repository.
        /// </summary>
        /// <value>The Vendor repository.</value>
        public IRepositoryWithTypedId<Vendor, string> VendorRepository { get; set; }
        public IRepositoryWithTypedId<VendorAddress, Guid> VendorAddressRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="VendorRepositoryTests"/> class.
        /// </summary>
        public VendorRepositoryTests()
        {
            VendorRepository = new RepositoryWithTypedId<Vendor, string>();
            VendorAddressRepository = new RepositoryWithTypedId<VendorAddress, Guid>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Vendor GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Vendor(counter);
            rtValue.SetIdTo(counter.HasValue ? counter.Value.ToString() : "99");

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Vendor> GetQuery(int numberAtEnd)
        {
            return VendorRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Vendor entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Vendor entity, ARTAction action)
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
            VendorRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            VendorRepository.DbContext.CommitTransaction();
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
            Vendor vendor = null;
            try
            {
                #region Arrange
                vendor = GetValid(9);
                vendor.Name = null;
                #endregion Arrange

                #region Act
                VendorRepository.DbContext.BeginTransaction();
                VendorRepository.EnsurePersistent(vendor);
                VendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendor);
                var results = vendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(vendor.IsTransient());
                Assert.IsFalse(vendor.IsValid());
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
            Vendor vendor = null;
            try
            {
                #region Arrange
                vendor = GetValid(9);
                vendor.Name = string.Empty;
                #endregion Arrange

                #region Act
                VendorRepository.DbContext.BeginTransaction();
                VendorRepository.EnsurePersistent(vendor);
                VendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendor);
                var results = vendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(vendor.IsTransient());
                Assert.IsFalse(vendor.IsValid());
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
            Vendor vendor = null;
            try
            {
                #region Arrange
                vendor = GetValid(9);
                vendor.Name = " ";
                #endregion Arrange

                #region Act
                VendorRepository.DbContext.BeginTransaction();
                VendorRepository.EnsurePersistent(vendor);
                VendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendor);
                var results = vendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(vendor.IsTransient());
                Assert.IsFalse(vendor.IsValid());
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
            Vendor vendor = null;
            try
            {
                #region Arrange
                vendor = GetValid(9);
                vendor.Name = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                VendorRepository.DbContext.BeginTransaction();
                VendorRepository.EnsurePersistent(vendor);
                VendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendor);
                Assert.AreEqual(40 + 1, vendor.Name.Length);
                var results = vendor.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "40"));
                //Assert.IsTrue(vendor.IsTransient());
                Assert.IsFalse(vendor.IsValid());
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
            var vendor = GetValid(9);
            vendor.Name = "x";
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.Name = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, vendor.Name.Length);
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests
        
        #region OwnershipCode Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the OwnershipCode with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOwnershipCodeWithTooLongValueDoesNotSave()
        {
            Vendor vendor = null;
            try
            {
                #region Arrange
                vendor = GetValid(9);
                vendor.OwnershipCode = "x".RepeatTimes((2 + 1));
                #endregion Arrange

                #region Act
                VendorRepository.DbContext.BeginTransaction();
                VendorRepository.EnsurePersistent(vendor);
                VendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendor);
                Assert.AreEqual(2 + 1, vendor.OwnershipCode.Length);
                var results = vendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "OwnershipCode", "2"));
                //Assert.IsTrue(vendor.IsTransient());
                Assert.IsFalse(vendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the OwnershipCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestOwnershipCodeWithNullValueSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.OwnershipCode = null;
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the OwnershipCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestOwnershipCodeWithEmptyStringSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.OwnershipCode = string.Empty;
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the OwnershipCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestOwnershipCodeWithOneSpaceSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.OwnershipCode = " ";
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the OwnershipCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestOwnershipCodeWithOneCharacterSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.OwnershipCode = "x";
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the OwnershipCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestOwnershipCodeWithLongValueSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.OwnershipCode = "x".RepeatTimes(2);
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, vendor.OwnershipCode.Length);
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion OwnershipCode Tests
  
        #region BusinessTypeCode Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the BusinessTypeCode with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestBusinessTypeCodeWithTooLongValueDoesNotSave()
        {
            Vendor vendor = null;
            try
            {
                #region Arrange
                vendor = GetValid(9);
                vendor.BusinessTypeCode = "x".RepeatTimes((2 + 1));
                #endregion Arrange

                #region Act
                VendorRepository.DbContext.BeginTransaction();
                VendorRepository.EnsurePersistent(vendor);
                VendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendor);
                Assert.AreEqual(2 + 1, vendor.BusinessTypeCode.Length);
                var results = vendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "BusinessTypeCode", "2"));
                //Assert.IsTrue(vendor.IsTransient());
                Assert.IsFalse(vendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the BusinessTypeCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestBusinessTypeCodeWithNullValueSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.BusinessTypeCode = null;
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BusinessTypeCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestBusinessTypeCodeWithEmptyStringSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.BusinessTypeCode = string.Empty;
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BusinessTypeCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestBusinessTypeCodeWithOneSpaceSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.BusinessTypeCode = " ";
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BusinessTypeCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestBusinessTypeCodeWithOneCharacterSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.BusinessTypeCode = "x";
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the BusinessTypeCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestBusinessTypeCodeWithLongValueSaves()
        {
            #region Arrange
            var vendor = GetValid(9);
            vendor.BusinessTypeCode = "x".RepeatTimes(2);
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(vendor);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, vendor.BusinessTypeCode.Length);
            Assert.IsFalse(vendor.IsTransient());
            Assert.IsTrue(vendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion BusinessTypeCode Tests

        #region VendorAddresses Tests
        #region Invalid Tests
        #endregion Invalid Tests
        #region Valid Tests


        [TestMethod]
        public void TestVendorAddressesWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Vendor record = GetValid(9);
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(record);
            VendorRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<VendorAddress>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.VendorAddress(i + 1));
                relatedRecords[i].SetIdTo(Guid.NewGuid());
                relatedRecords[i].Vendor = record;
                VendorAddressRepository.EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.VendorAddresses.Add(relatedRecord);
            }
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(record);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.VendorAddresses);
            Assert.AreEqual(addedCount, record.VendorAddresses.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestVendorAddressesWithEmptyListWillSave()
        {
            #region Arrange
            Vendor record = GetValid(9);
            #endregion Arrange

            #region Act
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(record);
            VendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.VendorAddresses);
            Assert.AreEqual(0, record.VendorAddresses.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestVendorCascadesUpdateToVendorAddress2()
        {
            #region Arrange
            var count = VendorAddressRepository.Queryable.Count();
            Vendor record = GetValid(9);
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(record);
            VendorRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<VendorAddress>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.VendorAddress(i + 1));
                relatedRecords[i].Vendor = record;
                relatedRecords[i].SetIdTo(Guid.NewGuid());
                VendorAddressRepository.EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.VendorAddresses.Add(relatedRecord);
            }
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(record);
            VendorRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.VendorAddresses[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = VendorRepository.GetNullableById(saveId);
            record.VendorAddresses[1].Name = "Updated";
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(record);
            VendorRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, VendorAddressRepository.Queryable.Count());
            var relatedRecord2 = VendorAddressRepository.GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Name);
            #endregion Assert
        }


        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestVendorDoesNotCascadesUpdateRemoveVendorAddress()
        {
            #region Arrange
            var count = VendorAddressRepository.Queryable.Count();
            Vendor record = GetValid(9);
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(record);
            VendorRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<VendorAddress>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.VendorAddress(i + 1));
                relatedRecords[i].Vendor = record;
                relatedRecords[i].SetIdTo(Guid.NewGuid());
                VendorAddressRepository.EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.VendorAddresses.Add(relatedRecord);
            }
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(record);
            VendorRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.VendorAddresses[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = VendorRepository.GetNullableById(saveId);
            record.VendorAddresses.RemoveAt(1);
            VendorRepository.DbContext.BeginTransaction();
            VendorRepository.EnsurePersistent(record);
            VendorRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), VendorAddressRepository.Queryable.Count());
            var relatedRecord2 = VendorAddressRepository.GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }

		


        #endregion Cascade Tests

        #endregion VendorAddresses Tests


        
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
            expectedFields.Add(new NameAndType("BusinessTypeCode", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)2)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("OwnershipCode", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)2)]"
            }));
            expectedFields.Add(new NameAndType("VendorAddresses", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.VendorAddress]", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Vendor));

        }

        #endregion Reflection of Database.	
		
		
    }
}