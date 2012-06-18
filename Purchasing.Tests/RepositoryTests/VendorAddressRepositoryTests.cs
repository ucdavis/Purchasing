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
    /// Entity Name:		VendorAddress
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class VendorAddressRepositoryTests : AbstractRepositoryTests<VendorAddress, Guid, VendorAddressMap>
    {
        /// <summary>
        /// Gets or sets the VendorAddress repository.
        /// </summary>
        /// <value>The VendorAddress repository.</value>
        public IRepositoryWithTypedId<VendorAddress, Guid> VendorAddressRepository { get; set; }
        public IRepositoryWithTypedId<Vendor, string> VendorRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="VendorAddressRepositoryTests"/> class.
        /// </summary>
        public VendorAddressRepositoryTests()
        {
            VendorAddressRepository = new RepositoryWithTypedId<VendorAddress, Guid>();
            VendorRepository = new RepositoryWithTypedId<Vendor, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override VendorAddress GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.VendorAddress(counter);
            rtValue.SetIdTo(Guid.NewGuid());
            rtValue.Vendor = VendorRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<VendorAddress> GetQuery(int numberAtEnd)
        {
            return VendorAddressRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(VendorAddress entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(VendorAddress entity, ARTAction action)
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
            LoadVendors(3);
            VendorRepository.DbContext.CommitTransaction();

            VendorAddressRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            VendorAddressRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region TypeCode Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the TypeCode with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeCodeWithNullValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.TypeCode = null;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "TypeCode"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TypeCode with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeCodeWithEmptyStringDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.TypeCode = string.Empty;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "TypeCode"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TypeCode with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeCodeWithSpacesOnlyDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.TypeCode = " ";
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "TypeCode"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TypeCode with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeCodeWithTooLongValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.TypeCode = "x".RepeatTimes((4 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(4 + 1, vendorAddress.TypeCode.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "TypeCode", "4"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the TypeCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestTypeCodeWithOneCharacterSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.TypeCode = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the TypeCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTypeCodeWithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.TypeCode = "x".RepeatTimes(4);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, vendorAddress.TypeCode.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion TypeCode Tests

        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Name = null;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Name = string.Empty;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Name = " ";
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Name = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(40 + 1, vendorAddress.Name.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "40"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            var vendorAddress = GetValid(9);
            vendorAddress.Name = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Name = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, vendorAddress.Name.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Line1 Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Line1 with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine1WithNullValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Line1 = null;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Line1"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Line1 with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine1WithEmptyStringDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Line1 = string.Empty;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Line1"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Line1 with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine1WithSpacesOnlyDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Line1 = " ";
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Line1"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Line1 with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine1WithTooLongValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Line1 = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(40 + 1, vendorAddress.Line1.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Line1", "40"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Line1 with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLine1WithOneCharacterSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line1 = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line1 with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLine1WithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line1 = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, vendorAddress.Line1.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Line1 Tests

        #region Line2 Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Line2 with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine2WithTooLongValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Line2 = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(40 + 1, vendorAddress.Line2.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Line2", "40"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Line2 with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLine2WithNullValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line2 = null;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line2 with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestLine2WithEmptyStringSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line2 = string.Empty;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line2 with one space saves.
        /// </summary>
        [TestMethod]
        public void TestLine2WithOneSpaceSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line2 = " ";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line2 with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLine2WithOneCharacterSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line2 = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line2 with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLine2WithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line2 = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, vendorAddress.Line2.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Line2 Tests

        #region Line3 Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Line3 with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine3WithTooLongValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Line3 = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(40 + 1, vendorAddress.Line3.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Line3", "40"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Line3 with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLine3WithNullValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line3 = null;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line3 with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestLine3WithEmptyStringSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line3 = string.Empty;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line3 with one space saves.
        /// </summary>
        [TestMethod]
        public void TestLine3WithOneSpaceSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line3 = " ";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line3 with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLine3WithOneCharacterSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line3 = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line3 with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLine3WithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Line3 = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, vendorAddress.Line3.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Line3 Tests
   
        #region City Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the City with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithNullValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.City = null;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "City"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.City = string.Empty;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "City"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.City = " ";
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "City"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.City = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(40 + 1, vendorAddress.City.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "City", "40"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            var vendorAddress = GetValid(9);
            vendorAddress.City = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the City with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.City = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, vendorAddress.City.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.State = null;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "State"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.State = string.Empty;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "State"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.State = " ";
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "State"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.State = "x".RepeatTimes((2 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(2 + 1, vendorAddress.State.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "State", "2"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            var vendorAddress = GetValid(9);
            vendorAddress.State = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the State with long value saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.State = "x".RepeatTimes(2);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, vendorAddress.State.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Zip = null;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Zip"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Zip = string.Empty;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Zip"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Zip = " ";
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Zip"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
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
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Zip = "x".RepeatTimes((11 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(11 + 1, vendorAddress.Zip.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Zip", "11"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Zip with one character saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithOneCharacterSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Zip = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Zip with long value saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Zip = "x".RepeatTimes(11);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(11, vendorAddress.Zip.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Zip Tests

        #region CountryCode Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the CountryCode with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCountryCodeWithTooLongValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.CountryCode = "x".RepeatTimes((2 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(2 + 1, vendorAddress.CountryCode.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "CountryCode", "2"));
                //Assert.IsTrue(vendorAddress.IsTransient());
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the CountryCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestCountryCodeWithNullValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.CountryCode = null;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CountryCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestCountryCodeWithEmptyStringSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.CountryCode = string.Empty;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CountryCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestCountryCodeWithOneSpaceSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.CountryCode = " ";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CountryCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCountryCodeWithOneCharacterSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.CountryCode = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CountryCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCountryCodeWithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.CountryCode = "x".RepeatTimes(2);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, vendorAddress.CountryCode.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion CountryCode Tests

        #region Vendor Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestVendorAddresssFieldVendorWithAValueOfNullDoesNotSave()
        {
            VendorAddress record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Vendor = null;
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(record);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Vendor, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Vendor field is required.");
                //Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }


        [TestMethod]
        public void TestVendorAddressWithExistingVendorSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Vendor = VendorRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(record);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.Vendor.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Vendor Tests

        #region DisplayName Tests

        [TestMethod]
        public void TestDisplayName1()
        {
            #region Arrange
            var record = CreateValidEntities.VendorAddress(99);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("(tc99) Line199, City99, XX 12345, AA", record.DisplayName);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDisplayName2()
        {
            #region Arrange
            var record = CreateValidEntities.VendorAddress(99);
            record.CountryCode = null;
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("(tc99) Line199, City99, XX 12345, ", record.DisplayName);
            #endregion Assert
        }
        #endregion DisplayName Tests

        #region PhoneNumber Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the PhoneNumber with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPhoneNumberWithTooLongValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.PhoneNumber = "x".RepeatTimes((15 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(15 + 1, vendorAddress.PhoneNumber.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "PhoneNumber", "15"));
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the PhoneNumber with null value saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithNullValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.PhoneNumber = null;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PhoneNumber with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithEmptyStringSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.PhoneNumber = string.Empty;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PhoneNumber with one space saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithOneSpaceSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.PhoneNumber = " ";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PhoneNumber with one character saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithOneCharacterSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.PhoneNumber = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PhoneNumber with long value saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.PhoneNumber = "x".RepeatTimes(15);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(15, vendorAddress.PhoneNumber.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion PhoneNumber Tests

        #region FaxNumber Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the FaxNumber with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFaxNumberWithTooLongValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.FaxNumber = "x".RepeatTimes((15 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(15 + 1, vendorAddress.FaxNumber.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "FaxNumber", "15"));
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the FaxNumber with null value saves.
        /// </summary>
        [TestMethod]
        public void TestFaxNumberWithNullValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.FaxNumber = null;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FaxNumber with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestFaxNumberWithEmptyStringSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.FaxNumber = string.Empty;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FaxNumber with one space saves.
        /// </summary>
        [TestMethod]
        public void TestFaxNumberWithOneSpaceSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.FaxNumber = " ";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FaxNumber with one character saves.
        /// </summary>
        [TestMethod]
        public void TestFaxNumberWithOneCharacterSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.FaxNumber = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FaxNumber with long value saves.
        /// </summary>
        [TestMethod]
        public void TestFaxNumberWithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.FaxNumber = "x".RepeatTimes(15);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(15, vendorAddress.FaxNumber.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion FaxNumber Tests

        #region Email Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Email with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithTooLongValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Email = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(50 + 1, vendorAddress.Email.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Email", "50"));
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Email with null value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithNullValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Email = null;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithEmptyStringSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Email = string.Empty;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with one space saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithOneSpaceSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Email = " ";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with one character saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithOneCharacterSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Email = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Email = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, vendorAddress.Email.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Email Tests

        #region Url Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Url with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUrlWithTooLongValueDoesNotSave()
        {
            VendorAddress vendorAddress = null;
            try
            {
                #region Arrange
                vendorAddress = GetValid(9);
                vendorAddress.Url = "x".RepeatTimes((128 + 1));
                #endregion Arrange

                #region Act
                VendorAddressRepository.DbContext.BeginTransaction();
                VendorAddressRepository.EnsurePersistent(vendorAddress);
                VendorAddressRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(vendorAddress);
                Assert.AreEqual(128 + 1, vendorAddress.Url.Length);
                var results = vendorAddress.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Url", "128"));
                Assert.IsFalse(vendorAddress.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Url with null value saves.
        /// </summary>
        [TestMethod]
        public void TestUrlWithNullValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Url = null;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Url with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestUrlWithEmptyStringSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Url = string.Empty;
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Url with one space saves.
        /// </summary>
        [TestMethod]
        public void TestUrlWithOneSpaceSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Url = " ";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Url with one character saves.
        /// </summary>
        [TestMethod]
        public void TestUrlWithOneCharacterSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Url = "x";
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Url with long value saves.
        /// </summary>
        [TestMethod]
        public void TestUrlWithLongValueSaves()
        {
            #region Arrange
            var vendorAddress = GetValid(9);
            vendorAddress.Url = "x".RepeatTimes(128);
            #endregion Arrange

            #region Act
            VendorAddressRepository.DbContext.BeginTransaction();
            VendorAddressRepository.EnsurePersistent(vendorAddress);
            VendorAddressRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(128, vendorAddress.Url.Length);
            Assert.IsFalse(vendorAddress.IsTransient());
            Assert.IsTrue(vendorAddress.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Url Tests


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
            expectedFields.Add(new NameAndType("City", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("CountryCode", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)2)]"
            }));
            expectedFields.Add(new NameAndType("DisplayName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("FaxNumber", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)15)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Guid", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Line1", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("Line2", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("Line3", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("PhoneNumber", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)15)]"
            }));
            expectedFields.Add(new NameAndType("State", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)2)]"
            }));
            expectedFields.Add(new NameAndType("TypeCode", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)4)]"
            }));
            expectedFields.Add(new NameAndType("Url", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)128)]"
            }));
            expectedFields.Add(new NameAndType("Vendor", "Purchasing.Core.Domain.Vendor", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Zip", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)11)]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(VendorAddress));

        }

        #endregion Reflection of Database.	
		
		
    }
}