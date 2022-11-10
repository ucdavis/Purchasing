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
    /// Entity Name:		WorkgroupAccount
    /// LookupFieldName:	Id
    /// </summary>
    [TestClass]
    public class WorkgroupAccountRepositoryTests : AbstractRepositoryTests<WorkgroupAccount, int, WorkgroupAccountMap>
    {
        /// <summary>
        /// Gets or sets the WorkgroupAccount repository.
        /// </summary>
        /// <value>The WorkgroupAccount repository.</value>
        public IRepository<WorkgroupAccount> WorkgroupAccountRepository { get; set; }
        public IRepositoryWithTypedId<Account, string> AccountRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkgroupAccountRepositoryTests"/> class.
        /// </summary>
        public WorkgroupAccountRepositoryTests()
        {
            WorkgroupAccountRepository = new Repository<WorkgroupAccount>();
            AccountRepository = new RepositoryWithTypedId<Account, string>();
            UserRepository = new RepositoryWithTypedId<User, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override WorkgroupAccount GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.WorkgroupAccount(counter, true);
            rtValue.Account = AccountRepository.Queryable.First();
            rtValue.Workgroup = Repository.OfType<Workgroup>().Queryable.First();
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<WorkgroupAccount> GetQuery(int numberAtEnd)
        {
            return WorkgroupAccountRepository.Queryable.Where(a => a.Id == numberAtEnd);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(WorkgroupAccount entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(WorkgroupAccount entity, ARTAction action)
        {
            const string updateValue = "Name2";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Account.Name);
                    break;
                case ARTAction.Restore:
                    entity.Account = AccountRepository.Queryable.Single(a => a.Id == "1");
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Account.Name;
                    entity.Account= AccountRepository.Queryable.Single(a => a.Id == "2");
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            AccountRepository.DbContext.BeginTransaction();
            LoadAccounts(3);
            LoadOrganizations(3);
            LoadWorkgroups(3);
            LoadUsers(3);
            AccountRepository.DbContext.CommitTransaction();

            WorkgroupAccountRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            WorkgroupAccountRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region Workgroup Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestWorkgroupAccountsFieldWorkgroupWithAValueOfNullDoesNotSave()
        {
            WorkgroupAccount record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Workgroup = null;
                #endregion Arrange

                #region Act
                WorkgroupAccountRepository.DbContext.BeginTransaction();
                WorkgroupAccountRepository.EnsurePersistent(record);
                WorkgroupAccountRepository.DbContext.CommitTransaction();
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
        public void TestWorkgroupAccountNewWorkgroupDoesNotSave()
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
                WorkgroupAccountRepository.DbContext.BeginTransaction();
                WorkgroupAccountRepository.EnsurePersistent(record);
                WorkgroupAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Workgroup, Entity: Purchasing.Core.Domain.Workgroup", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        public void TestWorkgroupAccountWithExistingWorkgroupSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Workgroup = Repository.OfType<Workgroup>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            WorkgroupAccountRepository.DbContext.BeginTransaction();
            WorkgroupAccountRepository.EnsurePersistent(record);
            WorkgroupAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Workgroup.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Workgroup Tests

        #region Account Tests


        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestWorkgroupAccountNewAccountDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Account = new Account();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupAccountRepository.DbContext.BeginTransaction();
                WorkgroupAccountRepository.EnsurePersistent(record);
                WorkgroupAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Account, Entity: Purchasing.Core.Domain.Account", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        public void TestWorkgroupAccountWithExistingAccountSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Account = AccountRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            WorkgroupAccountRepository.DbContext.BeginTransaction();
            WorkgroupAccountRepository.EnsurePersistent(record);
            WorkgroupAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.Account.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }
        #endregion Account Tests

        #region Approver Tests

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestWorkgroupAccountNewApproverDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Approver = new User();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupAccountRepository.DbContext.BeginTransaction();
                WorkgroupAccountRepository.EnsurePersistent(record);
                WorkgroupAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        public void TestWorkgroupAccountWithExistingApproverSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Approver = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            WorkgroupAccountRepository.DbContext.BeginTransaction();
            WorkgroupAccountRepository.EnsurePersistent(record);
            WorkgroupAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.Approver.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        //Delete this one if not nullable
        [TestMethod]
        public void TestWorkgroupAccountWithNullApproverSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Approver = null;
            #endregion Arrange

            #region Act
            WorkgroupAccountRepository.DbContext.BeginTransaction();
            WorkgroupAccountRepository.EnsurePersistent(record);
            WorkgroupAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Approver);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Approver Tests

        #region AccountManager Tests

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestWorkgroupAccountNewAccountManagerDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.AccountManager = new User();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupAccountRepository.DbContext.BeginTransaction();
                WorkgroupAccountRepository.EnsurePersistent(record);
                WorkgroupAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        public void TestWorkgroupAccountWithExistingAccountManagerSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.AccountManager = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            WorkgroupAccountRepository.DbContext.BeginTransaction();
            WorkgroupAccountRepository.EnsurePersistent(record);
            WorkgroupAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.AccountManager.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        //Delete this one if not nullable
        [TestMethod]
        public void TestWorkgroupAccountWithNullAccountManagerSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.AccountManager = null;
            #endregion Arrange

            #region Act
            WorkgroupAccountRepository.DbContext.BeginTransaction();
            WorkgroupAccountRepository.EnsurePersistent(record);
            WorkgroupAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.AccountManager);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion AccountManager Tests

        #region Purchaser Tests

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestWorkgroupAccountNewPurchaserDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Purchaser = new User();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupAccountRepository.DbContext.BeginTransaction();
                WorkgroupAccountRepository.EnsurePersistent(record);
                WorkgroupAccountRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        public void TestWorkgroupAccountWithExistingPurchaserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Purchaser = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            WorkgroupAccountRepository.DbContext.BeginTransaction();
            WorkgroupAccountRepository.EnsurePersistent(record);
            WorkgroupAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.Purchaser.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        //Delete this one if not nullable
        [TestMethod]
        public void TestWorkgroupAccountWithNullPurchaserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Purchaser = null;
            #endregion Arrange

            #region Act
            WorkgroupAccountRepository.DbContext.BeginTransaction();
            WorkgroupAccountRepository.EnsurePersistent(record);
            WorkgroupAccountRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Purchaser);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Purchaser Tests
        
        
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
            expectedFields.Add(new NameAndType("Account", "Purchasing.Core.Domain.Account", new List<string>()));
            expectedFields.Add(new NameAndType("AccountManager", "Purchasing.Core.Domain.User", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Account Manager\")]"
            }));
            expectedFields.Add(new NameAndType("Approver", "Purchasing.Core.Domain.User", new List<string>()));
            expectedFields.Add(new NameAndType("FinancialSegmentString", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)128)]",
                "[System.ComponentModel.DisplayNameAttribute(\"CoA\")]"
            }));
            expectedFields.Add(new NameAndType("GetName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));

            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)64)]"
            }));
            expectedFields.Add(new NameAndType("Purchaser", "Purchasing.Core.Domain.User", new List<string>()));
            expectedFields.Add(new NameAndType("Workgroup", "Purchasing.Core.Domain.Workgroup", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(WorkgroupAccount));

        }

        #endregion Reflection of Database.	
		
		
    }
}