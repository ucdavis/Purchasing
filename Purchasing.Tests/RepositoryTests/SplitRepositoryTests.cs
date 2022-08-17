using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentNHibernate.Testing;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;
using UCDArch.Testing;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Split
    /// LookupFieldName:	LookupField
    /// </summary>
    [TestClass]
    public class SplitRepositoryTests : AbstractRepositoryTests<Split, int, SplitMap>
    {
        /// <summary>
        /// Gets or sets the Split repository.
        /// </summary>
        /// <value>The Split repository.</value>
        public IRepository<Split> SplitRepository { get; set; }
        public IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodeRepository { get; set; }
        public IRepositoryWithTypedId<Account, string> AccountRepository { get; set; }
        public IRepositoryWithTypedId<SubAccount, Guid> SubAccountRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitRepositoryTests"/> class.
        /// </summary>
        public SplitRepositoryTests()
        {
            SplitRepository = new Repository<Split>();
            OrderStatusCodeRepository = new RepositoryWithTypedId<OrderStatusCode, string>();
            AccountRepository = new RepositoryWithTypedId<Account, string>();
            SubAccountRepository = new RepositoryWithTypedId<SubAccount, Guid>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Split GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Split(counter);
            rtValue.Order = Repository.OfType<Order>().Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Split> GetQuery(int numberAtEnd)
        {
            return SplitRepository.Queryable.Where(a => a.Project.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Split entity, int counter)
        {
            Assert.AreEqual("Project" + counter, entity.Project);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Split entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Project);
                    break;
                case ARTAction.Restore:
                    entity.Project = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Project;
                    entity.Project = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Order>().DbContext.BeginTransaction();
            LoadOrders(3);
            LoadAccounts(3);     
            LoadSubAccounts(3);
            Repository.OfType<Order>().DbContext.CommitTransaction();

            SplitRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            SplitRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Amount Tests

        [TestMethod]
        public void TestAmountSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Amount = 0;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.Amount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAmountSaves1()
        {
            #region Arrange
            var record = GetValid(9);
            record.Amount = 0.001m;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.001m, record.Amount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAmountSaves2()
        {
            #region Arrange
            var record = GetValid(9);
            record.Amount = 999999999.999m;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999999999.999m, record.Amount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Amount Tests

        #region Order Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestSplitsFieldOrderWithAValueOfNullDoesNotSave()
        {
            Split record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Order = null;
                #endregion Arrange

                #region Act
                SplitRepository.DbContext.BeginTransaction();
                SplitRepository.EnsurePersistent(record);
                SplitRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Order, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Order field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestSplitNewOrderDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Order = new Order();
                thisFar = true;
                #endregion Arrange

                #region Act
                SplitRepository.DbContext.BeginTransaction();
                SplitRepository.EnsurePersistent(record);
                SplitRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Order, Entity: Purchasing.Core.Domain.Order", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        public void TestSplitWithExistingOrderSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = Repository.OfType<Order>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Order.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }
        #endregion Order Tests

        #region LineItem Tests

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestSplitNewLineItemDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.LineItem = new LineItem();
                thisFar = true;
                #endregion Arrange

                #region Act
                SplitRepository.DbContext.BeginTransaction();
                SplitRepository.EnsurePersistent(record);
                SplitRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.LineItem, Entity: Purchasing.Core.Domain.LineItem", ex.Message);
                throw ex;
            }
        }

        [TestMethod]
        public void TestSplitWithExistingLineItemSaves()
        {
            #region Arrange
            Repository.OfType<LineItem>().DbContext.BeginTransaction();
            LoadLineItems(3);
            Repository.OfType<LineItem>().DbContext.CommitTransaction();
            var record = GetValid(9);
            record.LineItem = Repository.OfType<LineItem>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.LineItem.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestSplitWithNullLineItemSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.LineItem = null;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.LineItem);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion LineItem Tests

        #region Account Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Account with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAccountWithTooLongValueDoesNotSave()
        {
            Split split = null;
            try
            {
                #region Arrange
                split = GetValid(9);
                split.Account = "x".RepeatTimes((10 + 1));
                #endregion Arrange

                #region Act
                SplitRepository.DbContext.BeginTransaction();
                SplitRepository.EnsurePersistent(split);
                SplitRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(split);
                Assert.AreEqual(10 + 1, split.Account.Length);
                var results = split.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Account", "10"));
                Assert.IsTrue(split.IsTransient());
                Assert.IsFalse(split.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Account with null value saves.
        /// </summary>
        [TestMethod]
        public void TestAccountWithNullValueSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.Account = null;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Account with empty string saves.
        /// </summary>
        //[TestMethod, Ignore]
        //public void TestAccountWithEmptyStringSaves()
        //{
        //    #region Arrange
        //    var split = GetValid(9);
        //    split.Account = string.Empty;
        //    #endregion Arrange

        //    #region Act
        //    SplitRepository.DbContext.BeginTransaction();
        //    SplitRepository.EnsurePersistent(split);
        //    SplitRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsFalse(split.IsTransient());
        //    Assert.IsTrue(split.IsValid());
        //    #endregion Assert
        //}

        /// <summary>
        /// Tests the Account with one space saves.
        /// </summary>
        [TestMethod]
        public void TestAccountWithOneSpaceSaves()
        {
            #region Arrange
            AccountRepository.DbContext.BeginTransaction();
            var account = CreateValidEntities.Account(1);
            account.Id = " ";
            AccountRepository.EnsurePersistent(account);
            var split = GetValid(9);
            split.Account = " ";
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Account with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAccountWithOneCharacterSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.Account = "1";
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Account with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAccountWithLongValueSaves()
        {
            #region Arrange
            AccountRepository.DbContext.BeginTransaction();
            var account = CreateValidEntities.Account(1);
            account.Id = "x".RepeatTimes(10);
            AccountRepository.EnsurePersistent(account);
            var split = GetValid(9);
            split.Account = "x".RepeatTimes(10);
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, split.Account.Length);
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Account Tests

        #region SubAccount Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the SubAccount with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSubAccountWithTooLongValueDoesNotSave()
        {
            Split split = null;
            try
            {
                #region Arrange
                split = GetValid(9);
                split.SubAccount = "x".RepeatTimes((5 + 1));
                #endregion Arrange

                #region Act
                SplitRepository.DbContext.BeginTransaction();
                SplitRepository.EnsurePersistent(split);
                SplitRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(split);
                Assert.AreEqual(5 + 1, split.SubAccount.Length);
                var results = split.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "SubAccount", "5"));
                Assert.IsTrue(split.IsTransient());
                Assert.IsFalse(split.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the SubAccount with null value saves.
        /// </summary>
        [TestMethod]
        public void TestSubAccountWithNullValueSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.SubAccount = null;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubAccount with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestSubAccountWithEmptyStringSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.SubAccount = string.Empty;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubAccount with one space saves.
        /// </summary>
        [TestMethod]
        public void TestSubAccountWithOneSpaceSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.SubAccount = " ";
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubAccount with one character saves.
        /// </summary>
        [TestMethod]
        public void TestSubAccountWithOneCharacterSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.SubAccount = "x";
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubAccount with long value saves.
        /// </summary>
        [TestMethod]
        public void TestSubAccountWithLongValueSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.SubAccount = "x".RepeatTimes(5);
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(5, split.SubAccount.Length);
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion SubAccount Tests

        #region Project Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Project with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestProjectWithTooLongValueDoesNotSave()
        {
            Split split = null;
            try
            {
                #region Arrange
                split = GetValid(9);
                split.Project = "x".RepeatTimes((10 + 1));
                #endregion Arrange

                #region Act
                SplitRepository.DbContext.BeginTransaction();
                SplitRepository.EnsurePersistent(split);
                SplitRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(split);
                Assert.AreEqual(10 + 1, split.Project.Length);
                var results = split.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Project", "10"));
                Assert.IsTrue(split.IsTransient());
                Assert.IsFalse(split.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Project with null value saves.
        /// </summary>
        [TestMethod]
        public void TestProjectWithNullValueSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.Project = null;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Project with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestProjectWithEmptyStringSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.Project = string.Empty;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Project with one space saves.
        /// </summary>
        [TestMethod]
        public void TestProjectWithOneSpaceSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.Project = " ";
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Project with one character saves.
        /// </summary>
        [TestMethod]
        public void TestProjectWithOneCharacterSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.Project = "x";
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Project with long value saves.
        /// </summary>
        [TestMethod]
        public void TestProjectWithLongValueSaves()
        {
            #region Arrange
            var split = GetValid(9);
            split.Project = "x".RepeatTimes(10);
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(split);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, split.Project.Length);
            Assert.IsFalse(split.IsTransient());
            Assert.IsTrue(split.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Project Tests

        #region Approvals Tests
        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(TransientObjectException))]
        public void TestApprovalsWithANewValueDoesNotSave()
        {
            Split record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Approvals.Add(CreateValidEntities.Approval(1));
                #endregion Arrange

                #region Act
                SplitRepository.DbContext.BeginTransaction();
                SplitRepository.EnsurePersistent(record);
                SplitRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception exOuter) when (exOuter.InnerException is Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Approval, Entity: Purchasing.Core.Domain.Approval", ex.Message);
                throw ex;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestApprovalsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Split record = GetValid(9);
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Approval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Approval(i + 1));
                relatedRecords[i].Split = record;
                relatedRecords[i].User = null;
                relatedRecords[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
                relatedRecords[i].Order = Repository.OfType<Order>().Queryable.First();
                Repository.OfType<Approval>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            record = SplitRepository.Queryable.Single(a => a.Id == saveId);
            Assert.AreEqual(3, record.Approvals.Count());
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Approvals);
            Assert.AreEqual(addedCount, record.Approvals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestApprovalsWithEmptyListWillSave()
        {
            #region Arrange
            Split record = GetValid(9);
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Approvals);
            Assert.AreEqual(0, record.Approvals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        #endregion Cascade Tests

        #endregion Approvals Tests

        #region DbAccount Tests


        [TestMethod]
        public void TestSplitWithExistingDbAccountSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.DbAccount = AccountRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.DbAccount.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        //Delete this one if not nullable
        [TestMethod]
        public void TestSplitWithNullDbAccountSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.DbAccount = null;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.DbAccount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion DbAccount Tests

        #region DbSubAccount Tests


        [TestMethod]
        public void TestSplitWithExistingDbSubAccountSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.DbSubAccount = SubAccountRepository.Queryable.Single(a => a.AccountNumber == "3");
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.DbSubAccount.AccountNumber);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        //Delete this one if not nullable
        [TestMethod]
        public void TestSplitWithNullDbSubAccountSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.DbSubAccount = null;
            #endregion Arrange

            #region Act
            SplitRepository.DbContext.BeginTransaction();
            SplitRepository.EnsurePersistent(record);
            SplitRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.DbSubAccount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion DbSubAccount Tests

        #region AccountDisplay Tests

        [TestMethod]
        public void TestAccountDisplay1()
        {
            #region Arrange
            var record = new Split();

            #endregion Arrange

            #region Act
           
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.AccountDisplay);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAccountDisplay2()
        {
            #region Arrange
            var record = new Split();
            record.Account = "Test";
            record.DbAccount = CreateValidEntities.Account(3);
            record.DbSubAccount = CreateValidEntities.SubAccount(4);
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert
            Assert.AreEqual("Name3 (3)", record.AccountDisplay);
            #endregion Assert
        }
        #endregion AccountDisplay Tests

        #region FullAccountDisplay Tests

        [TestMethod]
        public void TestFullAccountDisplay1()
        {
            #region Arrange
            var record = CreateValidEntities.Split(1);
            #endregion Arrange

            #region Act
            record.Account = null;
            record.SubAccount = null;
            #endregion Act

            #region Assert
            Assert.AreEqual(string.Empty, record.FullAccountDisplay);
            #endregion Assert		
        }

        public void TestFullAccountDisplay2()
        {
            #region Arrange
            var record = CreateValidEntities.Split(1);
            #endregion Arrange

            #region Act
            record.Account = "Test";
            record.SubAccount = null;
            #endregion Act

            #region Assert
            Assert.AreEqual("Test", record.FullAccountDisplay);
            #endregion Assert
        }

        public void TestFullAccountDisplay3()
        {
            #region Arrange
            var record = CreateValidEntities.Split(1);
            #endregion Arrange

            #region Act
            record.Account = "Test";
            record.SubAccount = string.Empty;
            #endregion Act

            #region Assert
            Assert.AreEqual("Test", record.FullAccountDisplay);
            #endregion Assert
        }

        public void TestFullAccountDisplay4()
        {
            #region Arrange
            var record = CreateValidEntities.Split(1);
            #endregion Arrange

            #region Act
            record.Account = "Test";
            record.SubAccount = "Acct";
            #endregion Act

            #region Assert
            Assert.AreEqual("Test [Acct]", record.FullAccountDisplay);
            #endregion Assert
        }
        #endregion FullAccountDisplay Tests

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
            expectedFields.Add(new NameAndType("Account", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]"
            }));
            expectedFields.Add(new NameAndType("AccountDisplay", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Amount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Approvals", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Approval]", new List<string>()));
            expectedFields.Add(new NameAndType("DbAccount", "Purchasing.Core.Domain.Account", new List<string>()));
            expectedFields.Add(new NameAndType("DbSubAccount", "Purchasing.Core.Domain.SubAccount", new List<string>()));
            expectedFields.Add(new NameAndType("FullAccountDisplay", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("LineItem", "Purchasing.Core.Domain.LineItem", new List<string>()));
            expectedFields.Add(new NameAndType("Order", "Purchasing.Core.Domain.Order", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Project", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]"
            }));
            expectedFields.Add(new NameAndType("SubAccount", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)5)]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Split));

        }

        #endregion Reflection of Database.	
		
		
    }
}