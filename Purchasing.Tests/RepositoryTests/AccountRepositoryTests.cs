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
    /// Entity Name:		Account
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class AccountRepositoryTests : AbstractRepositoryTests<Account, string, AccountMap>
    {
        /// <summary>
        /// Gets or sets the Account repository.
        /// </summary>
        /// <value>The Account repository.</value>
        public IRepository<Account> AccountRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepositoryTests"/> class.
        /// </summary>
        public AccountRepositoryTests()
        {
            AccountRepository = new Repository<Account>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Account GetValid(int? counter)
        {
            return CreateValidEntities.Account(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Account> GetQuery(int numberAtEnd)
        {
            return AccountRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Account entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Account entity, ARTAction action)
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
            AccountRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            AccountRepository.DbContext.CommitTransaction();
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
            var account = GetValid(9);
            account.Name = null;
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithEmptyStringSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.Name = string.Empty;
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with one space saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneSpaceSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.Name = " ";
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.Name = "x";
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.Name = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, account.Name.Length);
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region AccountManager Tests


        #region Valid Tests

        /// <summary>
        /// Tests the AccountManager with null value saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerWithNullValueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.AccountManager = null;
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AccountManager with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerWithEmptyStringSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.AccountManager = string.Empty;
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AccountManager with one space saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerWithOneSpaceSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.AccountManager = " ";
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AccountManager with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerWithOneCharacterSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.AccountManager = "x";
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AccountManager with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerWithLongValueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.AccountManager = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, account.AccountManager.Length);
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion AccountManager Tests
        #region PrincipalInvestigator Tests
       

        #region Valid Tests

        /// <summary>
        /// Tests the PrincipalInvestigator with null value saves.
        /// </summary>
        [TestMethod]
        public void TestPrincipalInvestigatorWithNullValueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.PrincipalInvestigator = null;
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PrincipalInvestigator with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestPrincipalInvestigatorWithEmptyStringSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.PrincipalInvestigator = string.Empty;
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PrincipalInvestigator with one space saves.
        /// </summary>
        [TestMethod]
        public void TestPrincipalInvestigatorWithOneSpaceSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.PrincipalInvestigator = " ";
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PrincipalInvestigator with one character saves.
        /// </summary>
        [TestMethod]
        public void TestPrincipalInvestigatorWithOneCharacterSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.PrincipalInvestigator = "x";
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PrincipalInvestigator with long value saves.
        /// </summary>
        [TestMethod]
        public void TestPrincipalInvestigatorWithLongValueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.PrincipalInvestigator = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, account.PrincipalInvestigator.Length);
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion PrincipalInvestigator Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            Account account = GetValid(9);
            account.IsActive = false;
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(account.IsActive);
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.IsActive = true;
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(account.IsActive);
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
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
            expectedFields.Add(new NameAndType("AccountManager", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("PrincipalInvestigator", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Account));

        }

        #endregion Reflection of Database.	
		
		
    }
}