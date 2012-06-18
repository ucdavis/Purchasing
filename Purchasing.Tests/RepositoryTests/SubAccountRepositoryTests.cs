using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Entity Name:		SubAccount
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class SubAccountRepositoryTests : AbstractRepositoryTests<SubAccount, Guid, SubAccountMap>
    {
        /// <summary>
        /// Gets or sets the SubAccount repository.
        /// </summary>
        /// <value>The SubAccount repository.</value>
        public IRepository<SubAccount> SubAccountRepository { get; set; }

        public IRepositoryWithTypedId<Account, string> AccountRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="SubAccountRepositoryTests"/> class.
        /// </summary>
        public SubAccountRepositoryTests()
        {
            SubAccountRepository = new Repository<SubAccount>();
            AccountRepository = new RepositoryWithTypedId<Account, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override SubAccount GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.SubAccount(counter);
            rtValue.AccountNumber = AccountRepository.Queryable.First().Id;
            rtValue.SetIdTo(Guid.NewGuid());

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<SubAccount> GetQuery(int numberAtEnd)
        {
            return SubAccountRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(SubAccount entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(SubAccount entity, ARTAction action)
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
            AccountRepository.DbContext.BeginTransaction();
            for (int i = 0; i < 5; i++)
            {
                var account = CreateValidEntities.Account(i + 1);
                account.SetIdTo(string.Format("Acc{0}", (i + 1).ToString(CultureInfo.InvariantCulture)));
                AccountRepository.EnsurePersistent(account);
            }
            SubAccountRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            SubAccountRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region AccountNumber Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the AccountNumber with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAccountNumberWithNullValueDoesNotSave()
        {
            SubAccount subAccount = null;
            try
            {
                #region Arrange
                subAccount = GetValid(9);
                subAccount.AccountNumber = null;
                #endregion Arrange

                #region Act
                SubAccountRepository.DbContext.BeginTransaction();
                SubAccountRepository.EnsurePersistent(subAccount);
                SubAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(subAccount);
                var results = subAccount.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "AccountNumber"));
                //Assert.IsTrue(subAccount.IsTransient());
                Assert.IsFalse(subAccount.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the AccountNumber with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAccountNumberWithEmptyStringDoesNotSave()
        {
            SubAccount subAccount = null;
            try
            {
                #region Arrange
                subAccount = GetValid(9);
                subAccount.AccountNumber = string.Empty;
                #endregion Arrange

                #region Act
                SubAccountRepository.DbContext.BeginTransaction();
                SubAccountRepository.EnsurePersistent(subAccount);
                SubAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(subAccount);
                var results = subAccount.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "AccountNumber"));
                //Assert.IsTrue(subAccount.IsTransient());
                Assert.IsFalse(subAccount.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the AccountNumber with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAccountNumberWithSpacesOnlyDoesNotSave()
        {
            SubAccount subAccount = null;
            try
            {
                #region Arrange
                subAccount = GetValid(9);
                subAccount.AccountNumber = " ";
                #endregion Arrange

                #region Act
                SubAccountRepository.DbContext.BeginTransaction();
                SubAccountRepository.EnsurePersistent(subAccount);
                SubAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(subAccount);
                var results = subAccount.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "AccountNumber"));
                //Assert.IsTrue(subAccount.IsTransient());
                Assert.IsFalse(subAccount.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the AccountNumber with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAccountNumberWithTooLongValueDoesNotSave()
        {
            SubAccount subAccount = null;
            try
            {
                #region Arrange
                subAccount = GetValid(9);
                subAccount.AccountNumber = "x".RepeatTimes((10 + 1));
                #endregion Arrange

                #region Act
                SubAccountRepository.DbContext.BeginTransaction();
                SubAccountRepository.EnsurePersistent(subAccount);
                SubAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(subAccount);
                Assert.AreEqual(10 + 1, subAccount.AccountNumber.Length);
                var results = subAccount.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "AccountNumber", "10"));
                //Assert.IsTrue(subAccount.IsTransient());
                Assert.IsFalse(subAccount.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the AccountNumber with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAccountNumberWithOneCharacterSaves()
        {
            #region Arrange
            AccountRepository.DbContext.BeginTransaction();
            var account = CreateValidEntities.Account(1);
            account.SetIdTo("x");
            AccountRepository.EnsurePersistent(account);
            var subAccount = GetValid(9);
            subAccount.AccountNumber = "x";
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AccountNumber with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAccountNumberWithLongValueSaves()
        {
            AccountRepository.DbContext.BeginTransaction();
            var account = CreateValidEntities.Account(1);
            account.SetIdTo("x".RepeatTimes(10));
            AccountRepository.EnsurePersistent(account);
            #region Arrange
            var subAccount = GetValid(9);
            subAccount.AccountNumber = "x".RepeatTimes(10);
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, subAccount.AccountNumber.Length);
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion AccountNumber Tests

        #region SubAccountNumber Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the SubAccountNumber with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSubAccountNumberWithNullValueDoesNotSave()
        {
            SubAccount subAccount = null;
            try
            {
                #region Arrange
                subAccount = GetValid(9);
                subAccount.SubAccountNumber = null;
                #endregion Arrange

                #region Act
                SubAccountRepository.DbContext.BeginTransaction();
                SubAccountRepository.EnsurePersistent(subAccount);
                SubAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(subAccount);
                var results = subAccount.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "SubAccountNumber"));
                //Assert.IsTrue(subAccount.IsTransient());
                Assert.IsFalse(subAccount.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the SubAccountNumber with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSubAccountNumberWithEmptyStringDoesNotSave()
        {
            SubAccount subAccount = null;
            try
            {
                #region Arrange
                subAccount = GetValid(9);
                subAccount.SubAccountNumber = string.Empty;
                #endregion Arrange

                #region Act
                SubAccountRepository.DbContext.BeginTransaction();
                SubAccountRepository.EnsurePersistent(subAccount);
                SubAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(subAccount);
                var results = subAccount.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "SubAccountNumber"));
                //Assert.IsTrue(subAccount.IsTransient());
                Assert.IsFalse(subAccount.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the SubAccountNumber with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSubAccountNumberWithSpacesOnlyDoesNotSave()
        {
            SubAccount subAccount = null;
            try
            {
                #region Arrange
                subAccount = GetValid(9);
                subAccount.SubAccountNumber = " ";
                #endregion Arrange

                #region Act
                SubAccountRepository.DbContext.BeginTransaction();
                SubAccountRepository.EnsurePersistent(subAccount);
                SubAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(subAccount);
                var results = subAccount.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "SubAccountNumber"));
                //Assert.IsTrue(subAccount.IsTransient());
                Assert.IsFalse(subAccount.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the SubAccountNumber with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSubAccountNumberWithTooLongValueDoesNotSave()
        {
            SubAccount subAccount = null;
            try
            {
                #region Arrange
                subAccount = GetValid(9);
                subAccount.SubAccountNumber = "x".RepeatTimes((5 + 1));
                #endregion Arrange

                #region Act
                SubAccountRepository.DbContext.BeginTransaction();
                SubAccountRepository.EnsurePersistent(subAccount);
                SubAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(subAccount);
                Assert.AreEqual(5 + 1, subAccount.SubAccountNumber.Length);
                var results = subAccount.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "SubAccountNumber", "5"));
                //Assert.IsTrue(subAccount.IsTransient());
                Assert.IsFalse(subAccount.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the SubAccountNumber with one character saves.
        /// </summary>
        [TestMethod]
        public void TestSubAccountNumberWithOneCharacterSaves()
        {
            #region Arrange
            var subAccount = GetValid(9);
            subAccount.SubAccountNumber = "x";
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubAccountNumber with long value saves.
        /// </summary>
        [TestMethod]
        public void TestSubAccountNumberWithLongValueSaves()
        {
            #region Arrange
            var subAccount = GetValid(9);
            subAccount.SubAccountNumber = "x".RepeatTimes(5);
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(5, subAccount.SubAccountNumber.Length);
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion SubAccountNumber Tests

        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            SubAccount subAccount = null;
            try
            {
                #region Arrange
                subAccount = GetValid(9);
                subAccount.Name = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                SubAccountRepository.DbContext.BeginTransaction();
                SubAccountRepository.EnsurePersistent(subAccount);
                SubAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(subAccount);
                Assert.AreEqual(40 + 1, subAccount.Name.Length);
                var results = subAccount.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "40"));
                //Assert.IsTrue(subAccount.IsTransient());
                Assert.IsFalse(subAccount.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with null value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithNullValueSaves()
        {
            #region Arrange
            var subAccount = GetValid(9);
            subAccount.Name = null;
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithEmptyStringSaves()
        {
            #region Arrange
            var subAccount = GetValid(9);
            subAccount.Name = string.Empty;
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with one space saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneSpaceSaves()
        {
            #region Arrange
            var subAccount = GetValid(9);
            subAccount.Name = " ";
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var subAccount = GetValid(9);
            subAccount.Name = "x";
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var subAccount = GetValid(9);
            subAccount.Name = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, subAccount.Name.Length);
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            SubAccount subAccount = GetValid(9);
            subAccount.IsActive = false;
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(subAccount.IsActive);
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var subAccount = GetValid(9);
            subAccount.IsActive = true;
            #endregion Arrange

            #region Act
            SubAccountRepository.DbContext.BeginTransaction();
            SubAccountRepository.EnsurePersistent(subAccount);
            SubAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(subAccount.IsActive);
            Assert.IsFalse(subAccount.IsTransient());
            Assert.IsTrue(subAccount.IsValid());
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
            expectedFields.Add(new NameAndType("AccountNumber", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Guid", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("SubAccountNumber", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)5)]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(SubAccount));

        }

        #endregion Reflection of Database.	
		
		
    }
}