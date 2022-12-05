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
using UCDArch.Testing;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		AutoApproval
    /// LookupFieldName:	MaxAmount
    /// </summary>
    [TestClass]
    public class AutoApprovalRepositoryTests : AbstractRepositoryTests<AutoApproval, int, AutoApprovalMap>
    {
        /// <summary>
        /// Gets or sets the AutoApproval repository.
        /// </summary>
        /// <value>The AutoApproval repository.</value>
        public IRepository<AutoApproval> AutoApprovalRepository { get; set; }

        public IRepositoryWithTypedId<User, string> UserRepository { get; set; }
        public IRepositoryWithTypedId<Account, string> AccountRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoApprovalRepositoryTests"/> class.
        /// </summary>
        public AutoApprovalRepositoryTests()
        {
            AutoApprovalRepository = new Repository<AutoApproval>();
            AccountRepository = new RepositoryWithTypedId<Account, string>();
            UserRepository = new RepositoryWithTypedId<User, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override AutoApproval GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.AutoApproval(counter);
            rtValue.MaxAmount = counter.HasValue ? counter.Value : 10.77m;

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<AutoApproval> GetQuery(int numberAtEnd)
        {
            return AutoApprovalRepository.Queryable.Where(a => a.MaxAmount == numberAtEnd);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(AutoApproval entity, int counter)
        {
            Assert.AreEqual(counter, entity.MaxAmount);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(AutoApproval entity, ARTAction action)
        {
            const decimal updateValue = 99.99m;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.MaxAmount);
                    break;
                case ARTAction.Restore:
                    entity.MaxAmount = DecimalRestoreValue;
                    break;
                case ARTAction.Update:
                    DecimalRestoreValue = entity.MaxAmount;
                    entity.MaxAmount = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            AutoApprovalRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            LoadSpecificUsers();
            LoadAccounts(3);
            AutoApprovalRepository.DbContext.CommitTransaction();

            AutoApprovalRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            AutoApprovalRepository.DbContext.CommitTransaction();
        }

        private void LoadSpecificUsers()
        {
            var user = CreateValidEntities.User(55);
            user.Id = "55";
            UserRepository.EnsurePersistent(user);
            var user2 = CreateValidEntities.User(98);
            user2.Id = "98";
            UserRepository.EnsurePersistent(user2);
        }
        #endregion Init and Overrides	

        #region TargetUser Tests

        [TestMethod]
        public void TestAutoApprovalTargetUserIfNullSaves()
        {
            #region Arrange
            var record = CreateValidEntities.AutoApproval(99);
            record.TargetUser = null;
            record.Account = AccountRepository.Queryable.Single(a => a.Id == "2");
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.TargetUser);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestAutoApprovalTargetUserIfPopulatedSaves()
        {
            #region Arrange
            var record = CreateValidEntities.AutoApproval(99);
            record.TargetUser = UserRepository.Queryable.Single(a => a.Id == "2");
            record.Account = null;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("2", record.TargetUser.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTargetUserWithAValueOfNullDoesNotSaveIfAccountIsNull()
        {
            AutoApproval autoApproval = null;
            try
            {
                #region Arrange
                autoApproval = GetValid(9);
                autoApproval.TargetUser = null;
                autoApproval.Account = null;
                #endregion Arrange

                #region Act
                AutoApprovalRepository.DbContext.BeginTransaction();
                AutoApprovalRepository.EnsurePersistent(autoApproval);
                AutoApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(autoApproval);
                Assert.AreEqual(autoApproval.TargetUser, null);
                Assert.AreEqual(autoApproval.Account, null);
                var results = autoApproval.ValidationResults().AsMessageList();
                results.AssertErrorsAre("A user must be selected.");
                Assert.IsTrue(autoApproval.IsTransient());
                Assert.IsFalse(autoApproval.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTargetUserWithAExistingValueDoesNotSaveIfAccountIsPopulated()
        {
            AutoApproval autoApproval = null;
            try
            {
                #region Arrange
                autoApproval = GetValid(9);
                autoApproval.TargetUser = UserRepository.Queryable.Single(a => a.Id == "2");
                autoApproval.Account = AccountRepository.Queryable.Single(a => a.Id == "3");
                #endregion Arrange

                #region Act
                AutoApprovalRepository.DbContext.BeginTransaction();
                AutoApprovalRepository.EnsurePersistent(autoApproval);
                AutoApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception)
            {
                Assert.IsNotNull(autoApproval);
                Assert.IsNotNull(autoApproval.User);
                Assert.IsNotNull(autoApproval.Account);
                var results = autoApproval.ValidationResults().AsMessageList();
                results.AssertErrorsAre("A user must be selected.");
                Assert.IsTrue(autoApproval.IsTransient());
                Assert.IsFalse(autoApproval.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestTargetUserWithANewValueDoesNotSave()
        {
            AutoApproval autoApproval = null;
            try
            {
                #region Arrange
                autoApproval = GetValid(9);
                autoApproval.TargetUser = new User();
                autoApproval.Account = null;
                #endregion Arrange

                #region Act
                AutoApprovalRepository.DbContext.BeginTransaction();
                AutoApprovalRepository.EnsurePersistent(autoApproval);
                AutoApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsNotNull(autoApproval);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw ex;
            }
        }

        #endregion TargetUser Tests

        #region Account Tests
        [TestMethod]
        public void TestAutoApprovalAccountIfNullSaves()
        {
            #region Arrange
            var record = CreateValidEntities.AutoApproval(99);
            record.Account = null;
            record.TargetUser = UserRepository.Queryable.Single(a => a.Id == "2");
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Account);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAutoApprovalAccountIfPopulatedSaves()
        {
            #region Arrange
            var record = CreateValidEntities.AutoApproval(99);
            record.TargetUser = null;
            record.Account = AccountRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.Account.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestAccountWithANewValueDoesNotSave()
        {
            AutoApproval autoApproval = null;
            try
            {
                #region Arrange
                autoApproval = GetValid(9);
                autoApproval.TargetUser = null;
                autoApproval.Account = new Account();
                #endregion Arrange

                #region Act
                AutoApprovalRepository.DbContext.BeginTransaction();
                AutoApprovalRepository.EnsurePersistent(autoApproval);
                AutoApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsNotNull(autoApproval);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Account, Entity: Purchasing.Core.Domain.Account", ex.Message);
                throw ex;
            }
        }
        //Other tests for validation of account are done for the TargetUser
        #endregion Account Tests

        #region MaxAmount Tests

        [TestMethod]
        public void TestMaxAmount1()
        {
            #region Arrange
            var record = CreateValidEntities.AutoApproval(99);
            record.MaxAmount = 0;            
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.MaxAmount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestMaxAmount2()
        {
            #region Arrange
            var record = CreateValidEntities.AutoApproval(99);
            record.MaxAmount = 0.01m;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.01m, record.MaxAmount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestMaxAmount3()
        {
            #region Arrange
            var record = CreateValidEntities.AutoApproval(99);
            record.MaxAmount = 99999999.99m;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(99999999.99m, record.MaxAmount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion MaxAmount Tests

        #region LessThan Tests

        /// <summary>
        /// Tests the LessThan is false saves.
        /// </summary>
        [TestMethod]
        public void TestLessThanIsFalseSaves()
        {
            #region Arrange
            AutoApproval autoApproval = GetValid(9);
            autoApproval.LessThan = false;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(autoApproval);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(autoApproval.LessThan);
            Assert.IsFalse(autoApproval.IsTransient());
            Assert.IsTrue(autoApproval.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LessThan is true saves.
        /// </summary>
        [TestMethod]
        public void TestLessThanIsTrueSaves()
        {
            #region Arrange
            var autoApproval = GetValid(9);
            autoApproval.LessThan = true;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(autoApproval);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(autoApproval.LessThan);
            Assert.IsFalse(autoApproval.IsTransient());
            Assert.IsTrue(autoApproval.IsValid());
            #endregion Assert
        }

        #endregion LessThan Tests

        #region Equal Tests

        /// <summary>
        /// Tests the Equal is false saves.
        /// </summary>
        [TestMethod]
        public void TestEqualIsFalseSaves()
        {
            #region Arrange
            AutoApproval autoApproval = GetValid(9);
            autoApproval.Equal = false;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(autoApproval);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(autoApproval.Equal);
            Assert.IsFalse(autoApproval.IsTransient());
            Assert.IsTrue(autoApproval.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Equal is true saves.
        /// </summary>
        [TestMethod]
        public void TestEqualIsTrueSaves()
        {
            #region Arrange
            var autoApproval = GetValid(9);
            autoApproval.Equal = true;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(autoApproval);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(autoApproval.Equal);
            Assert.IsFalse(autoApproval.IsTransient());
            Assert.IsTrue(autoApproval.IsValid());
            #endregion Assert
        }

        #endregion Equal Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            AutoApproval autoApproval = GetValid(9);
            autoApproval.IsActive = false;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(autoApproval);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(autoApproval.IsActive);
            Assert.IsFalse(autoApproval.IsTransient());
            Assert.IsTrue(autoApproval.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var autoApproval = GetValid(9);
            autoApproval.IsActive = true;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(autoApproval);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(autoApproval.IsActive);
            Assert.IsFalse(autoApproval.IsTransient());
            Assert.IsTrue(autoApproval.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region User Tests

        [TestMethod]
        public void TestUserWithAPopulatedValueSaves()
        {
            #region Arrange
            AutoApproval autoApproval = GetValid(9);
            autoApproval.User = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(autoApproval);
            AutoApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", autoApproval.User.Id);
            Assert.IsFalse(autoApproval.IsTransient());
            Assert.IsTrue(autoApproval.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserWithAValueOfNullDoesNotSave()
        {
            AutoApproval autoApproval = null;
            try
            {
                #region Arrange
                autoApproval = GetValid(9);
                autoApproval.User = null;
                #endregion Arrange

                #region Act
                AutoApprovalRepository.DbContext.BeginTransaction();
                AutoApprovalRepository.EnsurePersistent(autoApproval);
                AutoApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception)
            {
                Assert.IsNotNull(autoApproval);
                Assert.AreEqual(null, autoApproval.User);
                var results = autoApproval.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The User field is required.");
                Assert.IsTrue(autoApproval.IsTransient());
                Assert.IsFalse(autoApproval.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestUserWithNewValueDoesNotSave()
        {
            AutoApproval autoApproval = null;
            try
            {
                #region Arrange
                autoApproval = GetValid(9);
                autoApproval.User = new User();
                #endregion Arrange

                #region Act
                AutoApprovalRepository.DbContext.BeginTransaction();
                AutoApprovalRepository.EnsurePersistent(autoApproval);
                AutoApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsNotNull(autoApproval);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw ex;
            }
        }
        #endregion User Tests

        #region Expiration Tests

        /// <summary>
        /// Tests the Expiration with past date will save.
        /// </summary>
        [TestMethod]
        public void TestExpirationWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            AutoApproval record = GetValid(99);
            record.Expiration = compareDate;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Expiration);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the Expiration with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestExpirationWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.Expiration = compareDate;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Expiration);
            #endregion Assert
        }

        /// <summary>
        /// Tests the Expiration with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestExpirationWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.Expiration = compareDate;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Expiration);
            #endregion Assert
        }

        [TestMethod]
        public void TestExpirationWithNullValueWillSave()
        {
            #region Arrange
            var record = GetValid(99);
            record.Expiration = null;
            #endregion Arrange

            #region Act
            AutoApprovalRepository.DbContext.BeginTransaction();
            AutoApprovalRepository.EnsurePersistent(record);
            AutoApprovalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(null, record.Expiration);
            #endregion Assert
        }
        #endregion Expiration Tests
        

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
            expectedFields.Add(new NameAndType("Equal", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Expiration", "System.Nullable`1[System.DateTime]", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DataTypeAttribute((System.ComponentModel.DataAnnotations.DataType)1)]"
                                                                         }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
                                                                         {
                                                                             "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                                                                             "[System.Xml.Serialization.XmlIgnoreAttribute()]"
                                                                         }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("LessThan", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("MaxAmount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("TargetUser", "Purchasing.Core.Domain.User", new List<string>()));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
                                                                         }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(AutoApproval));

        }

        #endregion Reflection of Database.	
		
		
    }
}