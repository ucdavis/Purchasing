using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;


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

        public IRepositoryWithTypedId<Organization, string> OrganizationRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepositoryTests"/> class.
        /// </summary>
        public AccountRepositoryTests()
        {
            AccountRepository = new Repository<Account>();
            OrganizationRepository = new RepositoryWithTypedId<Organization, string>();
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
            return AccountRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString(System.Globalization.CultureInfo.InvariantCulture)));
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

        #region AccountManagerId Tests

        #region Valid Tests

        /// <summary>
        /// Tests the AccountManagerId with null value saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerIdWithNullValueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.AccountManagerId = null;
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
        /// Tests the AccountManagerId with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerIdWithEmptyStringSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.AccountManagerId = string.Empty;
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
        /// Tests the AccountManagerId with one space saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerIdWithOneSpaceSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.AccountManagerId = " ";
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
        /// Tests the AccountManagerId with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerIdWithOneCharacterSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.AccountManagerId = "x";
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
        /// Tests the AccountManagerId with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerIdWithLongValueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.AccountManagerId = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, account.AccountManagerId.Length);
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion AccountManagerId Tests

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

        #region PrincipalInvestigatorId Tests
        
        #region Valid Tests

        /// <summary>
        /// Tests the PrincipalInvestigatorId with null value saves.
        /// </summary>
        [TestMethod]
        public void TestPrincipalInvestigatorIdWithNullValueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.PrincipalInvestigatorId = null;
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
        /// Tests the PrincipalInvestigatorId with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestPrincipalInvestigatorIdWithEmptyStringSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.PrincipalInvestigatorId = string.Empty;
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
        /// Tests the PrincipalInvestigatorId with one space saves.
        /// </summary>
        [TestMethod]
        public void TestPrincipalInvestigatorIdWithOneSpaceSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.PrincipalInvestigatorId = " ";
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
        /// Tests the PrincipalInvestigatorId with one character saves.
        /// </summary>
        [TestMethod]
        public void TestPrincipalInvestigatorIdWithOneCharacterSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.PrincipalInvestigatorId = "x";
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
        /// Tests the PrincipalInvestigatorId with long value saves.
        /// </summary>
        [TestMethod]
        public void TestPrincipalInvestigatorIdWithLongValueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.PrincipalInvestigatorId = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, account.PrincipalInvestigatorId.Length);
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion PrincipalInvestigatorId Tests

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

        #region NameAndId Tests

        [TestMethod]
        public void TestNameAndIdReturnsExpectedString()
        {
            #region Arrange
            var record = CreateValidEntities.Account(4);
            record.SetIdTo("IdTest");
            #endregion Arrange

            #region Act
            var result = record.NameAndId;
            #endregion Act

            #region Assert
            Assert.AreEqual("Name4 (IdTest)", result);
            #endregion Assert		
        }
        #endregion NameAndId Tests

        #region SubAccounts Tests
        #region Invalid Tests


        [TestMethod]
        [ExpectedException(typeof(NHibernate.StaleStateException))]
        public void TestSubAccountsWithANewValueDoesNotSave()
        {
            Account record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.SubAccounts.Add(CreateValidEntities.SubAccount(1));
                #endregion Arrange

                #region Act
                AccountRepository.DbContext.BeginTransaction();
                AccountRepository.EnsurePersistent(record);
                AccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Unexpected row count: 0; expected: 1", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests


        [TestMethod]
        public void TestSubAccountsWithEmptyListWillSave()
        {
            #region Arrange
            Account record = GetValid(9);
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(record);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.SubAccounts);
            Assert.AreEqual(0, record.SubAccounts.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #endregion SubAccounts Tests

        #region OrganizationId Tests

        #region Valid Tests

        /// <summary>
        /// Tests the OrganizationId with null value saves.
        /// </summary>
        [TestMethod]
        public void TestOrganizationIdWithNullValueSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.OrganizationId = null;
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
        /// Tests the OrganizationId with empty string saves.
        /// </summary>
        [TestMethod, Ignore]
        public void TestOrganizationIdWithEmptyStringSaves()
        {
            #region Arrange
            var account = GetValid(9);
            account.OrganizationId = string.Empty;
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
        /// Tests the OrganizationId with one space saves.
        /// </summary>
        [TestMethod]
        public void TestOrganizationIdWithOneSpaceSaves()
        {
            #region Arrange
            OrganizationRepository.DbContext.BeginTransaction();
            var organization = CreateValidEntities.Organization(1);
            organization.SetIdTo(" ");
            OrganizationRepository.EnsurePersistent(organization);
            OrganizationRepository.DbContext.CommitTransaction();
            var account = GetValid(9);
            account.OrganizationId = " ";
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
        /// Tests the OrganizationId with one character saves.
        /// Must match foreign key value
        /// </summary>
        [TestMethod]
        public void TestOrganizationIdWithOneCharacterSaves()
        {
            #region Arrange
            OrganizationRepository.DbContext.BeginTransaction();
            LoadOrganizations(3);
            OrganizationRepository.DbContext.CommitTransaction();
            var account = GetValid(9);
            account.OrganizationId = "2";
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
        /// Tests the OrganizationId with long value saves.
        /// </summary>
        [TestMethod]
        public void TestOrganizationIdWithLongValueSaves()
        {
            #region Arrange
            OrganizationRepository.DbContext.BeginTransaction();
            var organization = CreateValidEntities.Organization(1);
            organization.SetIdTo("x".RepeatTimes(999));
            OrganizationRepository.EnsurePersistent(organization);
            OrganizationRepository.DbContext.CommitTransaction();
            var account = GetValid(9);
            account.OrganizationId = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            AccountRepository.DbContext.BeginTransaction();
            AccountRepository.EnsurePersistent(account);
            AccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, account.OrganizationId.Length);
            Assert.IsFalse(account.IsTransient());
            Assert.IsTrue(account.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion OrganizationId Tests

        
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
            expectedFields.Add(new NameAndType("AccountManagerId", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("NameAndId", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("OrganizationId", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("PrincipalInvestigator", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("PrincipalInvestigatorId", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("SubAccounts", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.SubAccount]", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Account));

        }

        #endregion Reflection of Database.	
		
		
    }
}