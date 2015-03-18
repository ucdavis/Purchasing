using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		User
    /// LookupFieldName:	FirstName
    /// </summary>
    [TestClass]
    public class UserRepositoryTests : AbstractRepositoryTests<User, string, UserMap>
    {
        /// <summary>
        /// Gets or sets the User repository.
        /// </summary>
        /// <value>The User repository.</value>
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; }
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository { get; set; }
        public IRepositoryWithTypedId<Role, string> RoleRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepositoryTests"/> class.
        /// </summary>
        public UserRepositoryTests()
        {
            UserRepository = new RepositoryWithTypedId<User, string>();
            OrganizationRepository = new RepositoryWithTypedId<Organization, string>();
            RoleRepository = new RepositoryWithTypedId<Role, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override User GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.User(counter);
            rtValue.SetIdTo(counter.HasValue ? counter.Value.ToString() : "99");

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<User> GetQuery(int numberAtEnd)
        {
            return UserRepository.Queryable.Where(a => a.FirstName.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(User entity, int counter)
        {
            Assert.AreEqual("FirstName" + counter, entity.FirstName);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(User entity, ARTAction action)
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
            OrganizationRepository.DbContext.BeginTransaction();
            LoadOrganizations(3);
            OrganizationRepository.DbContext.CommitTransaction();
            UserRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            UserRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region IsAway Tests

        /// <summary>
        /// Tests the IsAway is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsAwayIsFalseSaves()
        {
            #region Arrange
            User user = GetValid(9);
            user.IsAway = false;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsAway);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsAway is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsAwayIsTrueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.IsAway = true;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(user.IsAway);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        #endregion IsAway Tests

        #region Id Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Id with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestIdWithNullValueDoesNotSave()
        {
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.SetIdTo(null);
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "KerberosID"));
                //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Id with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestIdWithEmptyStringDoesNotSave()
        {
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.SetIdTo(string.Empty);
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "KerberosID"));
                ////Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Id with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestIdWithSpacesOnlyDoesNotSave()
        {
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.SetIdTo(" ");
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "KerberosID"));
                ////Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Id with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestIdWithTooLongValueDoesNotSave()
        {
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.SetIdTo( "x".RepeatTimes((10 + 1)));
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                Assert.AreEqual(10 + 1, user.Id.Length);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "KerberosID", "10"));
                ////Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Id with one character saves.
        /// </summary>
        [TestMethod]
        public void TestIdWithOneCharacterSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.SetIdTo( "x");
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Id with long value saves.
        /// </summary>
        [TestMethod]
        public void TestIdWithLongValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.SetIdTo("x".RepeatTimes(10));
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, user.Id.Length);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Id Tests

        #region FirstName Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the FirstName with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFirstNameWithNullValueDoesNotSave()
        {
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.FirstName = null;
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "First Name"));
                //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.FirstName = string.Empty;
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "First Name"));
                //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.FirstName = " ";
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "First Name"));
                //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.FirstName = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                Assert.AreEqual(50 + 1, user.FirstName.Length);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "First Name", "50"));
                //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            var user = GetValid(9);
            user.FirstName = "x";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FirstName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithLongValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.FirstName = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, user.FirstName.Length);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
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
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.LastName = null;
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Last Name"));
                //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.LastName = string.Empty;
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Last Name"));
                //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.LastName = " ";
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Last Name"));
                //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.LastName = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                Assert.AreEqual(50 + 1, user.LastName.Length);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Last Name", "50"));
                //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            var user = GetValid(9);
            user.LastName = "x";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithLongValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LastName = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, user.LastName.Length);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion LastName Tests

        #region Email Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Email with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithNullValueDoesNotSave()
        {
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.Email = null;
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Email"));
               //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.Email = string.Empty;
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Email"));
               //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.Email = " ";
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                var results = user.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Email"));
               //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
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
            User user = null;
            try
            {
                #region Arrange
                user = GetValid(9);
                user.Email = string.Format("x{0}@x.com", "x".RepeatTimes(44));
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(user);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(user);
                Assert.AreEqual(50 + 1, user.Email.Length);
                var results = user.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Email", "50"));
               //Assert.IsTrue(user.IsTransient());
                Assert.IsFalse(user.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Email with one character saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithFewCharacterSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.Email = "x@x.x";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithLongValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.Email = string.Format("x{0}@x.com", "x".RepeatTimes(43));
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, user.Email.Length);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Email Tests

        #region AwayUntil Tests

        [TestMethod]
        public void TestAwayUntilWithNullDateWillSave()
        {
            #region Arrange
            User record = GetValid(99);
            record.AwayUntil = null;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(null, record.AwayUntil);
            #endregion Assert
        }

        /// <summary>
        /// Tests the AwayUntil with past date will save.
        /// </summary>
        [TestMethod]
        public void TestAwayUntilWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            User record = GetValid(99);
            record.AwayUntil = compareDate;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.AwayUntil);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the AwayUntil with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestAwayUntilWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.AwayUntil = compareDate;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.AwayUntil);
            #endregion Assert
        }

        /// <summary>
        /// Tests the AwayUntil with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestAwayUntilWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.AwayUntil = compareDate;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.AwayUntil);
            #endregion Assert
        }
        #endregion AwayUntil Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            User user = GetValid(9);
            user.IsActive = false;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsActive);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.IsActive = true;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(user.IsActive);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region GettersOnly Tests

        [TestMethod]
        public void TestFullName()
        {
            #region Arrange
            var record = GetValid(9);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName9 LastName9", record.FullName);
            #endregion Assert		
        }

        [TestMethod]
        public void TestFullNameAndId()
        {
            #region Arrange
            var record = GetValid(9);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName9 LastName9 (9)", record.FullNameAndId);
            #endregion Assert
        }

        [TestMethod]
        public void TestFullNameAndIdLastFirst()
        {
            #region Arrange
            var record = GetValid(9);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("LastName9, FirstName9 (9)", record.FullNameAndIdLastFirst);
            #endregion Assert
        }

        #endregion GettersOnly Tests

        #region Organizations Tests
        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestOrganizationsWithANewValueDoesNotSave()
        {
            User record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Organizations.Add(CreateValidEntities.Organization(1));
                #endregion Arrange

                #region Act
                UserRepository.DbContext.BeginTransaction();
                UserRepository.EnsurePersistent(record);
                UserRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Organization, Entity: Purchasing.Core.Domain.Organization", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestOrganizationsWithPopulatedListWillSave()
        {
            #region Arrange
            User record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.Organizations.Add(OrganizationRepository.Queryable.Single(a=>a.Id == (i+1).ToString()));
            }
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Organizations);
            Assert.AreEqual(addedCount, record.Organizations.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }


        [TestMethod]
        public void TestOrganizationsWithEmptyListWillSave()
        {
            #region Arrange
            User record = GetValid(9);
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Organizations);
            Assert.AreEqual(0, record.Organizations.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestOrganizationsWithPopulatedListWillSave2()
        {
            #region Arrange
            User record = GetValid(9);
            const int addedCount = 3;
            for(int i = 0; i < addedCount; i++)
            {
                record.Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == (i + 1).ToString()));
            }
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            record = UserRepository.Queryable.Single(a => a.Id == "9");
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Organizations);
            Assert.AreEqual(addedCount, record.Organizations.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }



        #endregion Cascade Tests

        #endregion Organizations Tests

        #region Roles Tests
        #region Invalid Tests


        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestRolesWithPopulatedListWillSave()
        {
            #region Arrange
            RoleRepository.DbContext.BeginTransaction();
            for (int i = 0; i < 3; i++)
            {
                var role = CreateValidEntities.Role(i + 1);
                role.SetIdTo((i + 1).ToString(CultureInfo.InvariantCulture));
                RoleRepository.EnsurePersistent(role);
            }
            RoleRepository.DbContext.CommitTransaction();
            User record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.Roles.Add(RoleRepository.Queryable.Single(a => a.Id == (i+1).ToString(CultureInfo.InvariantCulture)));
            }
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Roles);
            Assert.AreEqual(addedCount, record.Roles.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }


        [TestMethod]
        public void TestRolesWithEmptyListWillSave()
        {
            #region Arrange
            User record = GetValid(9);
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Roles);
            Assert.AreEqual(0, record.Roles.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests
		


        #endregion Cascade Tests

        #endregion Roles Tests

        #region WorkgroupPermissions Tests
        #region Invalid Tests



        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestWorkgroupPermissionsWithPopulatedListWillSave()
        {
            #region Arrange
            User record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.WorkgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i+1));
            }
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.WorkgroupPermissions);
            Assert.AreEqual(addedCount, record.WorkgroupPermissions.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestWorkgroupPermissionsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            RoleRepository.DbContext.BeginTransaction();
            LoadRoles(3);
            LoadWorkgroups(3);
            RoleRepository.DbContext.CommitTransaction();
            User record = GetValid(9);
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupPermission>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupPermission(i + 1));
                relatedRecords[i].User = record;
                relatedRecords[i].Role = RoleRepository.Queryable.First();
                relatedRecords[i].Workgroup = Repository.OfType<Workgroup>().Queryable.First();
                Repository.OfType<WorkgroupPermission>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.WorkgroupPermissions.Add(relatedRecord);
            }
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.WorkgroupPermissions);
            Assert.AreEqual(addedCount, record.WorkgroupPermissions.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestWorkgroupPermissionsWithEmptyListWillSave()
        {
            #region Arrange
            User record = GetValid(9);
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.WorkgroupPermissions);
            Assert.AreEqual(0, record.WorkgroupPermissions.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestUserDoesNotCascadesUpdateRemoveWorkgroupPermission()
        {
            #region Arrange
            RoleRepository.DbContext.BeginTransaction();
            LoadRoles(3);
            LoadWorkgroups(3);
            RoleRepository.DbContext.CommitTransaction();
            var count = Repository.OfType<WorkgroupPermission>().Queryable.Count();
            User record = GetValid(9);
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupPermission>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupPermission(i + 1));
                relatedRecords[i].User = record;
                relatedRecords[i].Role = RoleRepository.Queryable.First();
                relatedRecords[i].Workgroup = Repository.OfType<Workgroup>().Queryable.First();
                Repository.OfType<WorkgroupPermission>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.WorkgroupPermissions.Add(relatedRecord);
            }
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.WorkgroupPermissions[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = UserRepository.GetNullableById(saveId);
            record.WorkgroupPermissions.RemoveAt(1);
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(record);
            UserRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<WorkgroupPermission>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<WorkgroupPermission>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }

		


        #endregion Cascade Tests

        #endregion WorkgroupPermissions Tests



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
            expectedFields.Add(new NameAndType("AwayUntil", "System.Nullable`1[System.DateTime]", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Away Until\")]"
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
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]",
                "[UCDArch.Core.DomainModel.DomainSignatureAttribute()]"
            }));

            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Is Active\")]"
            }));
            expectedFields.Add(new NameAndType("IsAway", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("LastName", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Last Name\")]",
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Organizations", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Organization]", new List<string>()));
            expectedFields.Add(new NameAndType("Roles", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Role]", new List<string>()));
            expectedFields.Add(new NameAndType("WorkgroupPermissions", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.WorkgroupPermission]", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(User));

            //Assert.Inconclusive("Override of Id looks like it removed two attributes");

        }

        #endregion Reflection of Database.	
		
		
    }
}