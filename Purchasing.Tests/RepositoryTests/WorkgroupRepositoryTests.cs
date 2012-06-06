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
    /// Entity Name:		Workgroup
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class WorkgroupRepositoryTests : AbstractRepositoryTests<Workgroup, int, WorkgroupMap>
    {
        /// <summary>
        /// Gets or sets the Workgroup repository.
        /// </summary>
        /// <value>The Workgroup repository.</value>
        public IRepository<Workgroup> WorkgroupRepository { get; set; }
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository { get; set; }
        public IRepositoryWithTypedId<Account, string> AccountRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkgroupRepositoryTests"/> class.
        /// </summary>
        public WorkgroupRepositoryTests()
        {
            WorkgroupRepository = new Repository<Workgroup>();
            OrganizationRepository = new RepositoryWithTypedId<Organization, string>();
            AccountRepository = new RepositoryWithTypedId<Account, string>();
            UserRepository = new RepositoryWithTypedId<User, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Workgroup GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Workgroup(counter);
            rtValue.PrimaryOrganization = OrganizationRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Workgroup> GetQuery(int numberAtEnd)
        {
            return WorkgroupRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Workgroup entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Workgroup entity, ARTAction action)
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
            OrganizationRepository.DbContext.BeginTransaction();
            LoadOrganizations(3);
            LoadAccounts(3);
            OrganizationRepository.DbContext.CommitTransaction();

            WorkgroupRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            WorkgroupRepository.DbContext.CommitTransaction();
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
            Workgroup workgroup = null;
            try
            {
                #region Arrange
                workgroup = GetValid(9);
                workgroup.Name = null;
                #endregion Arrange

                #region Act
                WorkgroupRepository.DbContext.BeginTransaction();
                WorkgroupRepository.EnsurePersistent(workgroup);
                WorkgroupRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroup);
                var results = workgroup.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(workgroup.IsTransient());
                Assert.IsFalse(workgroup.IsValid());
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
            Workgroup workgroup = null;
            try
            {
                #region Arrange
                workgroup = GetValid(9);
                workgroup.Name = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupRepository.DbContext.BeginTransaction();
                WorkgroupRepository.EnsurePersistent(workgroup);
                WorkgroupRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroup);
                var results = workgroup.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(workgroup.IsTransient());
                Assert.IsFalse(workgroup.IsValid());
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
            Workgroup workgroup = null;
            try
            {
                #region Arrange
                workgroup = GetValid(9);
                workgroup.Name = " ";
                #endregion Arrange

                #region Act
                WorkgroupRepository.DbContext.BeginTransaction();
                WorkgroupRepository.EnsurePersistent(workgroup);
                WorkgroupRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroup);
                var results = workgroup.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(workgroup.IsTransient());
                Assert.IsFalse(workgroup.IsValid());
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
            Workgroup workgroup = null;
            try
            {
                #region Arrange
                workgroup = GetValid(9);
                workgroup.Name = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                WorkgroupRepository.DbContext.BeginTransaction();
                WorkgroupRepository.EnsurePersistent(workgroup);
                WorkgroupRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroup);
                Assert.AreEqual(50 + 1, workgroup.Name.Length);
                var results = workgroup.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "50"));
                Assert.IsTrue(workgroup.IsTransient());
                Assert.IsFalse(workgroup.IsValid());
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
            var workgroup = GetValid(9);
            workgroup.Name = "x";
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, workgroup.Name.Length);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Administrative Tests

        /// <summary>
        /// Tests the Administrative is false saves.
        /// </summary>
        [TestMethod]
        public void TestAdministrativeIsFalseSaves()
        {
            #region Arrange
            Workgroup workgroup = GetValid(9);
            workgroup.Administrative = false;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.Administrative);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Administrative is true saves.
        /// </summary>
        [TestMethod]
        public void TestAdministrativeIsTrueSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.Administrative = true;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroup.Administrative);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        #endregion Administrative Tests

        #region Disclaimer Tests


        #region Valid Tests

        /// <summary>
        /// Tests the Disclaimer with null value saves.
        /// </summary>
        [TestMethod]
        public void TestDisclaimerWithNullValueSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.Disclaimer = null;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Disclaimer with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestDisclaimerWithEmptyStringSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.Disclaimer = string.Empty;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Disclaimer with one space saves.
        /// </summary>
        [TestMethod]
        public void TestDisclaimerWithOneSpaceSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.Disclaimer = " ";
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Disclaimer with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDisclaimerWithOneCharacterSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.Disclaimer = "x";
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Disclaimer with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDisclaimerWithLongValueSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.Disclaimer = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, workgroup.Disclaimer.Length);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Disclaimer Tests

        #region Accounts Tests
        #region Invalid Tests

        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestAccountsWithPopulatedListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAccount(CreateValidEntities.WorkgroupAccount(i+1, true));
                record.Accounts[i].Account = AccountRepository.Queryable.First();
            }
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Accounts);
            Assert.AreEqual(addedCount, record.Accounts.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupAccount>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupAccount(i + 1, true));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].Account = AccountRepository.Queryable.First();
                Repository.OfType<WorkgroupAccount>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Accounts.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Accounts);
            Assert.AreEqual(addedCount, record.Accounts.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountsWithEmptyListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Accounts);
            Assert.AreEqual(0, record.Accounts.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestWorkgroupCascadesSaveToWorkgroupAccount()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupAccount>().Queryable.Count();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAccount(CreateValidEntities.WorkgroupAccount(i + 1, true));
                record.Accounts[i].Account = AccountRepository.Queryable.First();
            }

            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.Accounts.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupAccount>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestWorkgroupCascadesUpdateToWorkgroupAccount1()
        {
            #region Arrange
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();

            var count = Repository.OfType<WorkgroupAccount>().Queryable.Count();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAccount(CreateValidEntities.WorkgroupAccount(i + 1, true));
                record.Accounts[i].Account = AccountRepository.Queryable.First();
                record.Accounts[i].Approver = UserRepository.Queryable.First();
            }

            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Accounts[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Accounts[1].Approver = UserRepository.Queryable.Single(a => a.Id == "2");
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupAccount>().Queryable.Count());
            var relatedRecord = Repository.OfType<WorkgroupAccount>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("2", relatedRecord.Approver.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestWorkgroupCascadesUpdateToWorkgroupAccount2()
        {
            #region Arrange
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();

            var count = Repository.OfType<WorkgroupAccount>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupAccount>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupAccount(i + 1, true));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].Account = AccountRepository.Queryable.First();
                relatedRecords[i].Approver = UserRepository.Queryable.First();
                Repository.OfType<WorkgroupAccount>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Accounts.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Accounts[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Accounts[1].Approver = UserRepository.Queryable.Single(a => a.Id == "2");
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupAccount>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<WorkgroupAccount>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("2", relatedRecord2.Approver.Id);
            #endregion Assert
        }


        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestWorkgroupDoesNotCascadesUpdateRemoveWorkgroupAccount()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupAccount>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupAccount>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupAccount(i + 1, true));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].Account = AccountRepository.Queryable.First();
                Repository.OfType<WorkgroupAccount>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Accounts.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Accounts[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Accounts.RemoveAt(1);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<WorkgroupAccount>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<WorkgroupAccount>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests


        #endregion Accounts Tests

        #region Organizations Tests
        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestOrganizationsWithANewValueDoesNotSave()
        {
            Workgroup record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Organizations.Add(CreateValidEntities.Organization(1));
                #endregion Arrange

                #region Act
                WorkgroupRepository.DbContext.BeginTransaction();
                WorkgroupRepository.EnsurePersistent(record);
                WorkgroupRepository.DbContext.CommitTransaction();
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
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == (i+1).ToString()));
            }
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
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
            Workgroup record = GetValid(9);
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
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


        #endregion Cascade Tests

        #endregion Organizations Tests

        #region Vendors Tests
        #region Invalid Tests


        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestVendorsWithPopulatedListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddVendor(CreateValidEntities.WorkgroupVendor(i+1));
            }
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Vendors);
            Assert.AreEqual(addedCount, record.Vendors.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestVendorsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupVendor>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupVendor(i + 1));
                relatedRecords[i].Workgroup = record;
                Repository.OfType<WorkgroupVendor>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Vendors.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Vendors);
            Assert.AreEqual(addedCount, record.Vendors.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestVendorsWithEmptyListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Vendors);
            Assert.AreEqual(0, record.Vendors.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestWorkgroupCascadesSaveToWorkgroupVendor()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupVendor>().Queryable.Count();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddVendor(CreateValidEntities.WorkgroupVendor(i + 1));
            }

            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.Vendors.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupVendor>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestWorkgroupCascadesUpdateToWorkgroupVendor1()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupVendor>().Queryable.Count();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddVendor(CreateValidEntities.WorkgroupVendor(i + 1));
            }

            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Vendors[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Vendors[1].Name = "Updated";
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupVendor>().Queryable.Count());
            var relatedRecord = Repository.OfType<WorkgroupVendor>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestWorkgroupCascadesUpdateToWorkgroupVendor2()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupVendor>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupVendor>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupVendor(i + 1));
                relatedRecords[i].Workgroup = record;
                Repository.OfType<WorkgroupVendor>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Vendors.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Vendors[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Vendors[1].Name = "Updated";
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupVendor>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<WorkgroupVendor>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Name);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it (Delete this test, or the one below)
        /// </summary>
        [TestMethod]
        public void TestWorkgroupCascadesUpdateRemoveWorkgroupVendor()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupVendor>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupVendor>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupVendor(i + 1));
                relatedRecords[i].Workgroup = record;
                Repository.OfType<WorkgroupVendor>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Vendors.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Vendors[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Vendors.RemoveAt(1);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<WorkgroupVendor>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<WorkgroupVendor>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }

        [TestMethod]
        public void TestWorkgroupCascadesDeleteToWorkgroupVendor()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupVendor>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupVendor>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupVendor(i + 1));
                relatedRecords[i].Workgroup = record;
                Repository.OfType<WorkgroupVendor>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Vendors.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Vendors[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.Remove(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<WorkgroupVendor>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<WorkgroupVendor>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion Vendors Tests

        #region Addresses Tests
        #region Invalid Tests


        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestAddressesWithPopulatedListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAddress(CreateValidEntities.WorkgroupAddress(i + 1));
            }
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Addresses);
            Assert.AreEqual(addedCount, record.Addresses.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAddressesWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupAddress>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupAddress(i + 1));
                relatedRecords[i].Workgroup = record;
                Repository.OfType<WorkgroupAddress>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.AddAddress(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Addresses);
            Assert.AreEqual(addedCount, record.Addresses.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAddressesWithEmptyListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Addresses);
            Assert.AreEqual(0, record.Addresses.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestWorkgroupCascadesSaveToWorkgroupAddress()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupAddress>().Queryable.Count();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAddress(CreateValidEntities.WorkgroupAddress(i + 1));
            }

            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.Addresses.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupAddress>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestWorkgroupCascadesUpdateToWorkgroupAddress1()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupAddress>().Queryable.Count();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAddress(CreateValidEntities.WorkgroupAddress(i + 1));
            }

            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Addresses[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Addresses[1].Name = "Updated";
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupAddress>().Queryable.Count());
            var relatedRecord = Repository.OfType<WorkgroupAddress>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestWorkgroupCascadesUpdateToWorkgroupAddress2()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupAddress>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupAddress>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupAddress(i + 1));
                relatedRecords[i].Workgroup = record;
                Repository.OfType<WorkgroupAddress>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.AddAddress(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Addresses[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Addresses[1].Name = "Updated";
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupAddress>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<WorkgroupAddress>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Name);
            #endregion Assert
        }


        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestWorkgroupDoesNotCascadesUpdateRemoveWorkgroupAddress()
        {
            #region Arrange
            var count = Repository.OfType<WorkgroupAddress>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupAddress>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupAddress(i + 1));
                relatedRecords[i].Workgroup = record;
                Repository.OfType<WorkgroupAddress>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.AddAddress(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Addresses[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Addresses.RemoveAt(1);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<WorkgroupAddress>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<WorkgroupAddress>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }



        #endregion Cascade Tests

        #endregion Addresses Tests

        #region Permissions Tests
        #region Invalid Tests



        private IRepositoryWithTypedId<Role, string> LoadRecordsForPermissions()
        {
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            LoadRoles(3);
            UserRepository.DbContext.CommitTransaction();

            return new RepositoryWithTypedId<Role, string>();
        }
        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestPermissionsWithPopulatedListWillSave()
        {
            #region Arrange
            var roleRepository = LoadRecordsForPermissions();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddPermission(CreateValidEntities.WorkgroupPermission(i+1));
                record.Permissions[i].User = UserRepository.Queryable.Single(a => a.Id == "2");
                record.Permissions[i].Role = roleRepository.Queryable.Single(a => a.Id == "1");
            }
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Permissions);
            Assert.AreEqual(addedCount, record.Permissions.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestPermissionsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            var roleRepository = LoadRecordsForPermissions();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupPermission>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupPermission(i + 1));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].Role = roleRepository.Queryable.Single(a => a.Id == "2");
                relatedRecords[i].User = UserRepository.Queryable.Single(a => a.Id == "1");
                Repository.OfType<WorkgroupPermission>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Permissions.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Permissions);
            Assert.AreEqual(addedCount, record.Permissions.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestPermissionsWithEmptyListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Permissions);
            Assert.AreEqual(0, record.Permissions.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestWorkgroupCascadesSaveToWorkgroupPermission()
        {
            #region Arrange
            var roleRepository = LoadRecordsForPermissions();
            var count = Repository.OfType<WorkgroupPermission>().Queryable.Count();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddPermission(CreateValidEntities.WorkgroupPermission(i+1));
                record.Permissions[i].User = UserRepository.Queryable.Single(a => a.Id == "2");
                record.Permissions[i].Role = roleRepository.Queryable.Single(a => a.Id == "1");
            }

            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.Permissions.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupPermission>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestWorkgroupCascadesUpdateToWorkgroupPermission1()
        {
            #region Arrange
            var roleRepository = LoadRecordsForPermissions();
            var count = Repository.OfType<WorkgroupPermission>().Queryable.Count();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddPermission(CreateValidEntities.WorkgroupPermission(i+1));
                record.Permissions[i].User = UserRepository.Queryable.Single(a => a.Id == "2");
                record.Permissions[i].Role = roleRepository.Queryable.Single(a => a.Id == "1");
            }

            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Permissions[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Permissions[1].User.FirstName = "Updated";
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupPermission>().Queryable.Count());
            var relatedRecord = Repository.OfType<WorkgroupPermission>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.User.FirstName);
            #endregion Assert
        }

        [TestMethod]
        public void TestWorkgroupCascadesUpdateToWorkgroupPermission2()
        {
            #region Arrange
            var roleRepository = LoadRecordsForPermissions();
            var count = Repository.OfType<WorkgroupPermission>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupPermission>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupPermission(i + 1));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].Role = roleRepository.Queryable.Single(a => a.Id == "2");
                relatedRecords[i].User = UserRepository.Queryable.Single(a => a.Id == "1");
                Repository.OfType<WorkgroupPermission>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Permissions.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Permissions[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Permissions[1].User.FirstName = "Updated";
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<WorkgroupPermission>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<WorkgroupPermission>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.User.FirstName);
            #endregion Assert
        }

        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestWorkgroupDoesNotCascadesUpdateRemoveWorkgroupPermission()
        {
            #region Arrange
            var roleRepository = LoadRecordsForPermissions();
            var count = Repository.OfType<WorkgroupPermission>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<WorkgroupPermission>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.WorkgroupPermission(i + 1));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].Role = roleRepository.Queryable.Single(a => a.Id == "2");
                relatedRecords[i].User = UserRepository.Queryable.Single(a => a.Id == "1");
                Repository.OfType<WorkgroupPermission>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Permissions.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Permissions[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Permissions.RemoveAt(1);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<WorkgroupPermission>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<WorkgroupPermission>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }

		


        #endregion Cascade Tests


        #endregion Permissions Tests

        #region ConditionalApprovals Tests
        #region Invalid Tests



        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestConditionalApprovalsWithPopulatedListWillSave()
        {
            #region Arrange
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.ConditionalApprovals.Add(CreateValidEntities.ConditionalApproval(i+1));
                record.ConditionalApprovals[i].PrimaryApprover = UserRepository.Queryable.Single(a => a.Id == "2");
                record.ConditionalApprovals[i].Workgroup = record;
            }
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.ConditionalApprovals);
            Assert.AreEqual(addedCount, record.ConditionalApprovals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestConditionalApprovalsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<ConditionalApproval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.ConditionalApproval(i + 1));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].PrimaryApprover = UserRepository.Queryable.Single(a => a.Id == "2");
                Repository.OfType<ConditionalApproval>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.ConditionalApprovals.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.ConditionalApprovals);
            Assert.AreEqual(addedCount, record.ConditionalApprovals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestConditionalApprovalsWithEmptyListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.ConditionalApprovals);
            Assert.AreEqual(0, record.ConditionalApprovals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestWorkgroupCascadesSaveToConditionalApproval()
        {
            #region Arrange
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();
            var count = Repository.OfType<ConditionalApproval>().Queryable.Count();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.ConditionalApprovals.Add(CreateValidEntities.ConditionalApproval(i+1));
                record.ConditionalApprovals[i].PrimaryApprover = UserRepository.Queryable.Single(a => a.Id == "2");
                record.ConditionalApprovals[i].Workgroup = record;
            }

            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.ConditionalApprovals.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<ConditionalApproval>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestWorkgroupCascadesUpdateToConditionalApproval1()
        {
            #region Arrange
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();
            var count = Repository.OfType<ConditionalApproval>().Queryable.Count();
            Workgroup record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.ConditionalApprovals.Add(CreateValidEntities.ConditionalApproval(i+1));
                record.ConditionalApprovals[i].PrimaryApprover = UserRepository.Queryable.Single(a => a.Id == "2");
                record.ConditionalApprovals[i].Workgroup = record;
            }

            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.ConditionalApprovals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.ConditionalApprovals[1].Question = "Updated";
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<ConditionalApproval>().Queryable.Count());
            var relatedRecord = Repository.OfType<ConditionalApproval>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Question);
            #endregion Assert
        }

        [TestMethod]
        public void TestWorkgroupCascadesUpdateToConditionalApproval2()
        {
            #region Arrange
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();
            var count = Repository.OfType<ConditionalApproval>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<ConditionalApproval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.ConditionalApproval(i + 1));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].PrimaryApprover = UserRepository.Queryable.Single(a => a.Id == "2");
                Repository.OfType<ConditionalApproval>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.ConditionalApprovals.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.ConditionalApprovals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.ConditionalApprovals[1].Question = "Updated";
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<ConditionalApproval>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<ConditionalApproval>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Question);
            #endregion Assert
        }


        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestWorkgroupDoesNotCascadesUpdateRemoveConditionalApproval()
        {
            #region Arrange
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();
            var count = Repository.OfType<ConditionalApproval>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<ConditionalApproval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.ConditionalApproval(i + 1));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].PrimaryApprover = UserRepository.Queryable.Single(a => a.Id == "2");
                Repository.OfType<ConditionalApproval>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.ConditionalApprovals.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.ConditionalApprovals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.ConditionalApprovals.RemoveAt(1);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<ConditionalApproval>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<ConditionalApproval>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion ConditionalApprovals Tests

        #region Orders Tests
        #region Invalid Tests


        //[TestMethod, Ignore]
        //[ExpectedException(typeof(NHibernate.TransientObjectException))]
        //public void TestOrdersWithANewValueDoesNotSave()
        //{
        //    Workgroup record = null;
        //    try
        //    {
        //        #region Arrange
        //        LoadRecordsForOrder();
        //        record = GetValid(9);
        //        record.Orders.Add(CreateValidEntities.Order(1));
        //        record.Orders[0].OrderType = Repository.OfType<OrderType>().Queryable.First();
        //        record.Orders[0].Address = Repository.OfType<WorkgroupAddress>().Queryable.First();
        //        record.Orders[0].Workgroup = record;
        //        record.Orders[0].Organization = OrganizationRepository.Queryable.First();
        //        record.Orders[0].StatusCode = Repository.OfType<OrderStatusCode>().Queryable.First();
        //        #endregion Arrange

        //        #region Act
        //        WorkgroupRepository.DbContext.BeginTransaction();
        //        WorkgroupRepository.EnsurePersistent(record);
        //        WorkgroupRepository.DbContext.CommitTransaction();
        //        #endregion Act
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsNotNull(record);
        //        Assert.IsNotNull(ex);
        //        Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Order, Entity: Purchasing.Core.Domain.Order", ex.Message);
        //        throw;
        //    }
        //}

        private void LoadRecordsForOrder()
        {
            Repository.OfType<Order>().DbContext.BeginTransaction();
            LoadOrders(3);
            Repository.OfType<Order>().DbContext.CommitTransaction();
        }
        #endregion Invalid Tests
        #region Valid Tests


        [TestMethod]
        public void TestOrdersWithPopulatedExistingListWillSave()
        {
            #region Arrange
            LoadRecordsForOrder();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Order>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Order(i + 1));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].OrderType = Repository.OfType<OrderType>().Queryable.First();
                relatedRecords[i].Address = Repository.OfType<WorkgroupAddress>().Queryable.First();
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].Organization = OrganizationRepository.Queryable.First();
                relatedRecords[i].StatusCode = Repository.OfType<OrderStatusCode>().Queryable.First();
                relatedRecords[i].CreatedBy = UserRepository.Queryable.First();
                Repository.OfType<Order>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Orders.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Orders);
            Assert.AreEqual(addedCount, record.Orders.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestOrdersWithEmptyListWillSave()
        {
            #region Arrange
            Workgroup record = GetValid(9);
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Orders);
            Assert.AreEqual(0, record.Orders.Count);
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
        public void TestWorkgroupDoesNotCascadesUpdateRemoveOrder()
        {
            #region Arrange
            LoadRecordsForOrder();
            var count = Repository.OfType<Order>().Queryable.Count();
            Workgroup record = GetValid(9);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Order>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Order(i + 1));
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].OrderType = Repository.OfType<OrderType>().Queryable.First();
                relatedRecords[i].Address = Repository.OfType<WorkgroupAddress>().Queryable.First();
                relatedRecords[i].Workgroup = record;
                relatedRecords[i].Organization = OrganizationRepository.Queryable.First();
                relatedRecords[i].StatusCode = Repository.OfType<OrderStatusCode>().Queryable.First();
                relatedRecords[i].CreatedBy = UserRepository.Queryable.First();
                Repository.OfType<Order>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Orders.Add(relatedRecord);
            }
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Orders[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = WorkgroupRepository.GetNullableById(saveId);
            record.Orders.RemoveAt(1);
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<Order>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Order>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }

		


        #endregion Cascade Tests


        #endregion Orders Tests

        #region PrimaryOrganization Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestWorkgroupsFieldPrimaryOrganizationWithAValueOfNullDoesNotSave()
        {
            Workgroup record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.PrimaryOrganization = null;
                #endregion Arrange

                #region Act
                WorkgroupRepository.DbContext.BeginTransaction();
                WorkgroupRepository.EnsurePersistent(record);
                WorkgroupRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.PrimaryOrganization, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Primary Organization field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(PropertyValueException))]
        public void TestWorkgroupNewPrimaryOrganizationDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.PrimaryOrganization = new Organization();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupRepository.DbContext.BeginTransaction();
                WorkgroupRepository.EnsurePersistent(record);
                WorkgroupRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient value Purchasing.Core.Domain.Workgroup.PrimaryOrganization", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestWorkgroupWithExistingPrimaryOrganizationSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.PrimaryOrganization = OrganizationRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(record);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.PrimaryOrganization.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion PrimaryOrganization Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            Workgroup workgroup = GetValid(9);
            workgroup.IsActive = false;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.IsActive);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.IsActive = true;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroup.IsActive);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region AllConditionalApprovals Tests

        [TestMethod]
        public void TestAllConditionalApprovalsWithNoValues()
        {
            #region Arrange
            var record = GetValid(9);
            #endregion Arrange

            #region Act
            var result = record.AllConditionalApprovals;
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestAllConditionalApprovalsWithValues1()
        {
            #region Arrange
            LoadLocalConditionalApprovals();
            var record = GetValid(9);
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 2));
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 3));
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 4));
            #endregion Arrange

            #region Act
            var result = record.AllConditionalApprovals;
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestAllConditionalApprovalsWithValues2()
        {
            #region Arrange
            LoadLocalConditionalApprovals();
            var record = GetValid(9);
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 2));
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 3));
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 4));
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 4));
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 4));
            #endregion Arrange

            #region Act
            var result = record.AllConditionalApprovals;
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestAllConditionalApprovalsWithValues3()
        {
            #region Arrange
            LoadLocalConditionalApprovals();
            var record = GetValid(9);
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 2));
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 3));
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 4));

            var organization1 = CreateValidEntities.Organization(88);
            organization1.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 5));
            organization1.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 6));

            var organization2 = CreateValidEntities.Organization(88);
            organization2.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 7));
            record.Organizations.Add(organization1);
            record.Organizations.Add(organization2);
            #endregion Arrange

            #region Act
            var result = record.AllConditionalApprovals;
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestAllConditionalApprovalsWithValues4()
        {
            #region Arrange
            LoadLocalConditionalApprovals();
            var record = GetValid(9);
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 2));
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 3));
            record.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 4));

            var organization1 = CreateValidEntities.Organization(88);
            organization1.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 5));
            organization1.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 4));

            var organization2 = CreateValidEntities.Organization(88);
            organization2.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 7));
            record.Organizations.Add(organization1);
            record.Organizations.Add(organization2);
            #endregion Arrange

            #region Act
            var result = record.AllConditionalApprovals;
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestAllConditionalApprovalsWithValues5()
        {
            #region Arrange
            LoadLocalConditionalApprovals();
            var record = GetValid(9);


            var organization1 = CreateValidEntities.Organization(88);
            organization1.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 5));
            organization1.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 4));

            var organization2 = CreateValidEntities.Organization(88);
            organization2.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 7));
            organization2.ConditionalApprovals.Add(Repository.OfType<ConditionalApproval>().Queryable.Single(a => a.Id == 5));
            record.Organizations.Add(organization1);
            record.Organizations.Add(organization2);
            #endregion Arrange

            #region Act
            var result = record.AllConditionalApprovals;
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            #endregion Assert
        }

        private void LoadLocalConditionalApprovals()
        {
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();
            Workgroup record = GetValid(9);
            const int addedCount = 9;
            for(int i = 0; i < addedCount; i++)
            {
                record.ConditionalApprovals.Add(CreateValidEntities.ConditionalApproval(i + 1));
                record.ConditionalApprovals[i].PrimaryApprover = UserRepository.Queryable.Single(a => a.Id == "2");
                record.ConditionalApprovals[i].Workgroup = record;
            }
            WorkgroupRepository.EnsurePersistent(record);
        }
        #endregion AllConditionalApprovals Tests

        #region SyncAccounts Tests

        /// <summary>
        /// Tests the SyncAccounts is false saves.
        /// </summary>
        [TestMethod]
        public void TestSyncAccountsIsFalseSaves()
        {
            #region Arrange
            Workgroup workgroup = GetValid(9);
            workgroup.SyncAccounts = false;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.SyncAccounts);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SyncAccounts is true saves.
        /// </summary>
        [TestMethod]
        public void TestSyncAccountsIsTrueSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.SyncAccounts = true;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroup.SyncAccounts);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        #endregion SyncAccounts Tests

        #region AllowControlledSubstances Tests

        /// <summary>
        /// Tests the AllowControlledSubstances is false saves.
        /// </summary>
        [TestMethod]
        public void TestAllowControlledSubstancesIsFalseSaves()
        {
            #region Arrange
            Workgroup workgroup = GetValid(9);
            workgroup.AllowControlledSubstances = false;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.AllowControlledSubstances);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AllowControlledSubstances is true saves.
        /// </summary>
        [TestMethod]
        public void TestAllowControlledSubstancesIsTrueSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.AllowControlledSubstances = true;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroup.AllowControlledSubstances);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        #endregion AllowControlledSubstances Tests

        #region SharedOrCluster Tests

        /// <summary>
        /// Tests the SharedOrCluster is false saves.
        /// </summary>
        [TestMethod]
        public void TestSharedOrClusterIsFalseSaves()
        {
            #region Arrange
            Workgroup workgroup = GetValid(9);
            workgroup.SharedOrCluster = false;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.SharedOrCluster);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SharedOrCluster is true saves.
        /// </summary>
        [TestMethod]
        public void TestSharedOrClusterIsTrueSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.SharedOrCluster = true;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroup.SharedOrCluster);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        #endregion SharedOrCluster Tests

        #region ForceAccountApprover Tests

        /// <summary>
        /// Tests the ForceAccountApprover is false saves.
        /// </summary>
        [TestMethod]
        public void TestForceAccountApproverIsFalseSaves()
        {
            #region Arrange
            Workgroup workgroup = GetValid(9);
            workgroup.ForceAccountApprover = false;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroup.ForceAccountApprover);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ForceAccountApprover is true saves.
        /// </summary>
        [TestMethod]
        public void TestForceAccountApproverIsTrueSaves()
        {
            #region Arrange
            var workgroup = GetValid(9);
            workgroup.ForceAccountApprover = true;
            #endregion Arrange

            #region Act
            WorkgroupRepository.DbContext.BeginTransaction();
            WorkgroupRepository.EnsurePersistent(workgroup);
            WorkgroupRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroup.ForceAccountApprover);
            Assert.IsFalse(workgroup.IsTransient());
            Assert.IsTrue(workgroup.IsValid());
            #endregion Assert
        }

        #endregion ForceAccountApprover Tests


        #region Constructor Tests

        [TestMethod]
        public void TestConstructor()
        {
            #region Arrange
            var record = new Workgroup();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.IsNotNull(record.Accounts);
            Assert.AreEqual(0, record.Accounts.Count());
            Assert.IsNotNull(record.Organizations);
            Assert.AreEqual(0, record.Organizations.Count());
            Assert.IsNotNull(record.Vendors);
            Assert.AreEqual(0, record.Vendors.Count());
            Assert.IsNotNull(record.Permissions);
            Assert.AreEqual(0, record.Permissions.Count());
            Assert.IsNotNull(record.Addresses);
            Assert.AreEqual(0, record.Addresses.Count());
            Assert.IsNotNull(record.ConditionalApprovals);
            Assert.AreEqual(0, record.ConditionalApprovals.Count());
            Assert.IsNotNull(record.Orders);
            Assert.AreEqual(0, record.Orders.Count());
            Assert.IsTrue(record.IsActive);
            Assert.IsFalse(record.Administrative);
            Assert.IsFalse(record.SyncAccounts);
            Assert.IsFalse(record.SharedOrCluster);
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
            expectedFields.Add(new NameAndType("Accounts", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.WorkgroupAccount]", new List<string>()));
            expectedFields.Add(new NameAndType("Addresses", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.WorkgroupAddress]", new List<string>()));
            expectedFields.Add(new NameAndType("Administrative", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Is Administrative\")]"
            }));
            expectedFields.Add(new NameAndType("AllConditionalApprovals", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.ConditionalApproval]", new List<string>()));
            expectedFields.Add(new NameAndType("AllowControlledSubstances", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Controlled Substances Fields\")]"
            }));
            expectedFields.Add(new NameAndType("ConditionalApprovals", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.ConditionalApproval]", new List<string>()));
            expectedFields.Add(new NameAndType("Disclaimer", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DataTypeAttribute((System.ComponentModel.DataAnnotations.DataType)9)]"
            }));
            expectedFields.Add(new NameAndType("ForceAccountApprover", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Force Account Select at Approver\")]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Is Active\")]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Orders", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Order]", new List<string>()));
            expectedFields.Add(new NameAndType("Organizations", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Organization]", new List<string>()));
            expectedFields.Add(new NameAndType("Permissions", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.WorkgroupPermission]", new List<string>()));
            expectedFields.Add(new NameAndType("PrimaryOrganization", "Purchasing.Core.Domain.Organization", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Primary Organization\")]",
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("SharedOrCluster", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Shared or Cluster\")]"
            }));
            expectedFields.Add(new NameAndType("SyncAccounts", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Synchronize Accounts\")]"
            }));
            expectedFields.Add(new NameAndType("Vendors", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.WorkgroupVendor]", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Workgroup));

        }

        #endregion Reflection of Database.	
		
		
    }
}