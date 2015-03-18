using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		DepartmentalAdminRequest
    /// LookupFieldName:	FirstName
    /// </summary>
    [TestClass]
    public class DepartmentalAdminRequestRepositoryTests : AbstractRepositoryTests<DepartmentalAdminRequest, string, DepartmentalAdminRequestMap>
    {
        /// <summary>
        /// Gets or sets the DepartmentalAdminRequest repository.
        /// </summary>
        /// <value>The DepartmentalAdminRequest repository.</value>
        public IRepository<DepartmentalAdminRequest> DepartmentalAdminRequestRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentalAdminRequestRepositoryTests"/> class.
        /// </summary>
        public DepartmentalAdminRequestRepositoryTests()
        {
            DepartmentalAdminRequestRepository = new Repository<DepartmentalAdminRequest>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override DepartmentalAdminRequest GetValid(int? counter)
        {
            return CreateValidEntities.DepartmentalAdminRequest(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<DepartmentalAdminRequest> GetQuery(int numberAtEnd)
        {
            return DepartmentalAdminRequestRepository.Queryable.Where(a => a.FirstName.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(DepartmentalAdminRequest entity, int counter)
        {
            Assert.AreEqual("FirstName" + counter, entity.FirstName);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(DepartmentalAdminRequest entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.FirstName);
                    break;
                case ARTAction.Restore:
                    entity.FirstName = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.FirstName;
                    entity.FirstName = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region FirstName Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the FirstName with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFirstNameWithNullValueDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.FirstName = null;
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "First Name"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FirstName with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFirstNameWithEmptyStringDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.FirstName = string.Empty;
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "First Name"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FirstName with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFirstNameWithSpacesOnlyDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.FirstName = " ";
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "First Name"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FirstName with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFirstNameWithTooLongValueDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.FirstName = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                Assert.AreEqual(50 + 1, departmentalAdminRequest.FirstName.Length);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "First Name", "50"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the FirstName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithOneCharacterSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.FirstName = "x";
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FirstName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithLongValueSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.FirstName = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, departmentalAdminRequest.FirstName.Length);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion FirstName Tests

        #region LastName Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the LastName with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLastNameWithNullValueDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.LastName = null;
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Last Name"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the LastName with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLastNameWithEmptyStringDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.LastName = string.Empty;
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Last Name"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the LastName with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLastNameWithSpacesOnlyDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.LastName = " ";
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Last Name"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the LastName with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLastNameWithTooLongValueDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.LastName = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                Assert.AreEqual(50 + 1, departmentalAdminRequest.LastName.Length);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Last Name", "50"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the LastName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithOneCharacterSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.LastName = "x";
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithLongValueSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.LastName = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, departmentalAdminRequest.LastName.Length);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion LastName Tests

        #region PhoneNumber Tests
        #region Invalid Tests

  

        /// <summary>
        /// Tests the PhoneNumber with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPhoneNumberWithTooLongValueDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.PhoneNumber = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                Assert.AreEqual(50 + 1, departmentalAdminRequest.PhoneNumber.Length);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Phone Number", "50"));

                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the PhoneNumber with one character saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithOneCharacterSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.PhoneNumber = "x";
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PhoneNumber with long value saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneNumberWithLongValueSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.PhoneNumber = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, departmentalAdminRequest.PhoneNumber.Length);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion PhoneNumber Tests

        #region Email Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Email with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithNullValueDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.Email = null;
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Email"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Email with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithEmptyStringDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.Email = string.Empty;
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Email"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Email with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithSpacesOnlyDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.Email = " ";
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Email"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Email with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithTooLongValueDoesNotSave()
        {
            DepartmentalAdminRequest departmentalAdminRequest = null;
            try
            {
                #region Arrange
                departmentalAdminRequest = GetValid(9);
                departmentalAdminRequest.Email = string.Format("{0}@x.com", "x".RepeatTimes((44 + 1)));
                #endregion Arrange

                #region Act
                DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
                DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
                DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(departmentalAdminRequest);
                Assert.AreEqual(50 + 1, departmentalAdminRequest.Email.Length);
                var results = departmentalAdminRequest.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Email", "50"));
                Assert.IsFalse(departmentalAdminRequest.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestEmailWithMinCharactersSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.Email = "x@x.com";
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithLongValueSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.Email = string.Format("{0}@x.com", "x".RepeatTimes((44)));
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, departmentalAdminRequest.Email.Length);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Email Tests
  
        #region DepartmentSize Tests


        /// <summary>
        /// Tests the DepartmentSize with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestDepartmentSizeWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.DepartmentSize = int.MaxValue;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(record);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.DepartmentSize);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DepartmentSize with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestDepartmentSizeWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.DepartmentSize = int.MinValue;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(record);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.DepartmentSize);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion DepartmentSize Tests

        #region IsFullFeatured Tests

        /// <summary>
        /// Tests the IsFullFeatured is false saves.
        /// </summary>
        [TestMethod]
        public void TestSharedOrClusterIsFalseSaves()
        {
            #region Arrange
            DepartmentalAdminRequest departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.SharedOrCluster = false;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.SharedOrCluster);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsFullFeatured is true saves.
        /// </summary>
        [TestMethod]
        public void TestSharedOrClusterIsTrueSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.SharedOrCluster = true;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(departmentalAdminRequest.SharedOrCluster);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        #endregion IsFullFeatured Tests

        #region DateCreated Tests

        /// <summary>
        /// Tests the DateCreated with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            DepartmentalAdminRequest record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(record);
            DepartmentalAdminRequestRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the DateCreated with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(record);
            DepartmentalAdminRequestRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateCreated with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(record);
            DepartmentalAdminRequestRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert
        }
        #endregion DateCreated Tests

        #region Organizations Tests
        #region Invalid Tests

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Organizations with null value saves.
        /// </summary>
        [TestMethod]
        public void TestOrganizationsWithNullValueSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.Organizations = null;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Organizations with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestOrganizationsWithEmptyStringSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.Organizations = string.Empty;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Organizations with one space saves.
        /// </summary>
        [TestMethod]
        public void TestOrganizationsWithOneSpaceSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.Organizations = " ";
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Organizations with one character saves.
        /// </summary>
        [TestMethod]
        public void TestOrganizationsWithOneCharacterSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.Organizations = "x";
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Organizations with long value saves.
        /// </summary>
        [TestMethod]
        public void TestOrganizationsWithLongValueSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.Organizations = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, departmentalAdminRequest.Organizations.Length);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Organizations Tests

        #region Complete Tests

        /// <summary>
        /// Tests the Complete is false saves.
        /// </summary>
        [TestMethod]
        public void TestCompleteIsFalseSaves()
        {
            #region Arrange
            DepartmentalAdminRequest departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.Complete = false;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.Complete);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Complete is true saves.
        /// </summary>
        [TestMethod]
        public void TestCompleteIsTrueSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.Complete = true;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(departmentalAdminRequest.Complete);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        #endregion Complete Tests
        
        #region RequestCount Tests

        /// <summary>
        /// Tests the RequestCount with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestRequestCountWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.RequestCount = int.MaxValue;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(record);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.RequestCount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the RequestCount with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestRequestCountWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.RequestCount = int.MinValue;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(record);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.RequestCount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion RequestCount Tests

        #region AttendedTraining Tests

        /// <summary>
        /// Tests the AttendedTraining is false saves.
        /// </summary>
        [TestMethod]
        public void TestAttendedTrainingIsFalseSaves()
        {
            #region Arrange
            DepartmentalAdminRequest departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.AttendedTraining = false;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(departmentalAdminRequest.AttendedTraining);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AttendedTraining is true saves.
        /// </summary>
        [TestMethod]
        public void TestAttendedTrainingIsTrueSaves()
        {
            #region Arrange
            var departmentalAdminRequest = GetValid(9);
            departmentalAdminRequest.AttendedTraining = true;
            #endregion Arrange

            #region Act
            DepartmentalAdminRequestRepository.DbContext.BeginTransaction();
            DepartmentalAdminRequestRepository.EnsurePersistent(departmentalAdminRequest);
            DepartmentalAdminRequestRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(departmentalAdminRequest.AttendedTraining);
            Assert.IsFalse(departmentalAdminRequest.IsTransient());
            Assert.IsTrue(departmentalAdminRequest.IsValid());
            #endregion Assert
        }

        #endregion AttendedTraining Tests


        #region Constructor Tests

        [TestMethod]
        public void TestConstructor()
        {
            #region Arrange
            var record = new DepartmentalAdminRequest("test");
            #endregion Arrange

            #region Assert
            Assert.AreEqual("test", record.Id);
            Assert.IsFalse(record.SharedOrCluster);
            Assert.IsFalse(record.Complete);
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, record.DateCreated.Date);
            Assert.AreEqual(0, record.RequestCount);
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
            expectedFields.Add(new NameAndType("AttendedTraining", "System.Boolean", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Attended Training\")]"
            }));
            expectedFields.Add(new NameAndType("Complete", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("DateCreated", "System.DateTime", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Date Created\")]"
            }));
            expectedFields.Add(new NameAndType("DepartmentSize", "System.Int32", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Department Size\")]"
            }));
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>
            {
                 "[DataAnnotationsExtensions.EmailAttribute()]",
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"                 
            }));
            expectedFields.Add(new NameAndType("FirstName", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"First Name\")]",
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("FullName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("FullNameAndId", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("FullNameAndIdLastFirst", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"KerberosID\")]",
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]"
            }));
            expectedFields.Add(new NameAndType("LastName", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Last Name\")]",
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Organizations", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("PhoneNumber", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Phone Number\")]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("RequestCount", "System.Int32", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Request Count\")]"
            }));
            expectedFields.Add(new NameAndType("SharedOrCluster", "System.Boolean", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Shared Service Center Participant?\")]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(DepartmentalAdminRequest));

        }

        #endregion Reflection of Database.	
		
		
    }
}