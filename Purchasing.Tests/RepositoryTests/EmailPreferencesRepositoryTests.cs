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
    /// Entity Name:		EmailPreferences
    /// LookupFieldName:	RequesterOrderSubmission
    /// </summary>
    [TestClass]
    public class EmailPreferencesRepositoryTests : AbstractRepositoryTests<EmailPreferences, string, EmailPreferencesMap>
    {
        /// <summary>
        /// Gets or sets the EmailPreferences repository.
        /// </summary>
        /// <value>The EmailPreferences repository.</value>
        public IRepository<EmailPreferences> EmailPreferencesRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailPreferencesRepositoryTests"/> class.
        /// </summary>
        public EmailPreferencesRepositoryTests()
        {
            EmailPreferencesRepository = new Repository<EmailPreferences>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override EmailPreferences GetValid(int? counter)
        {
            return CreateValidEntities.EmailPreferences(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<EmailPreferences> GetQuery(int numberAtEnd)
        {
            return EmailPreferencesRepository.Queryable.Where(a => a.Id.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(EmailPreferences entity, int counter)
        {
            Assert.AreEqual(counter.ToString(), entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(EmailPreferences entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.RequesterOrderSubmission);
                    break;
                case ARTAction.Restore:
                    entity.RequesterOrderSubmission = BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.RequesterOrderSubmission;
                    entity.RequesterOrderSubmission = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            EmailPreferencesRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            EmailPreferencesRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region RequesterOrderSubmission Tests

        /// <summary>
        /// Tests the RequesterOrderSubmission is false saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterOrderSubmissionIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.RequesterOrderSubmission = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.RequesterOrderSubmission);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the RequesterOrderSubmission is true saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterOrderSubmissionIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.RequesterOrderSubmission = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.RequesterOrderSubmission);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion RequesterOrderSubmission Tests

        #region RequesterApproverApproved Tests

        /// <summary>
        /// Tests the RequesterApproverApproved is false saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterApproverApprovedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.RequesterApproverApproved = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.RequesterApproverApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the RequesterApproverApproved is true saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterApproverApprovedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.RequesterApproverApproved = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.RequesterApproverApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion RequesterApproverApproved Tests

        #region RequesterApproverChanged Tests

        /// <summary>
        /// Tests the RequesterApproverChanged is false saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterApproverChangedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.RequesterApproverChanged = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.RequesterApproverChanged);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the RequesterApproverChanged is true saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterApproverChangedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.RequesterApproverChanged = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.RequesterApproverChanged);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion RequesterApproverChanged Tests

        #region RequesterAccountManagerApproved Tests

        /// <summary>
        /// Tests the RequesterAccountManagerApproved is false saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterAccountManagerApprovedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.RequesterAccountManagerApproved = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.RequesterAccountManagerApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the RequesterAccountManagerApproved is true saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterAccountManagerApprovedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.RequesterAccountManagerApproved = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.RequesterAccountManagerApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion RequesterAccountManagerApproved Tests

        #region RequesterAccountManagerChanged Tests

        /// <summary>
        /// Tests the RequesterAccountManagerChanged is false saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterAccountManagerChangedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.RequesterAccountManagerChanged = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.RequesterAccountManagerChanged);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the RequesterAccountManagerChanged is true saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterAccountManagerChangedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.RequesterAccountManagerChanged = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.RequesterAccountManagerChanged);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion RequesterAccountManagerChanged Tests

        #region RequesterPurchaserAction Tests

        /// <summary>
        /// Tests the RequesterPurchaserAction is false saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterPurchaserActionIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.RequesterPurchaserAction = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.RequesterPurchaserAction);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the RequesterPurchaserAction is true saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterPurchaserActionIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.RequesterPurchaserAction = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.RequesterPurchaserAction);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion RequesterPurchaserAction Tests

        #region RequesterPurchaserChanged Tests

        /// <summary>
        /// Tests the RequesterPurchaserChanged is false saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterPurchaserChangedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.RequesterPurchaserChanged = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.RequesterPurchaserChanged);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the RequesterPurchaserChanged is true saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterPurchaserChangedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.RequesterPurchaserChanged = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.RequesterPurchaserChanged);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion RequesterPurchaserChanged Tests

        #region RequesterKualiProcessed Tests

        /// <summary>
        /// Tests the RequesterKualiProcessed is false saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterKualiProcessedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.RequesterKualiProcessed = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.RequesterKualiProcessed);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the RequesterKualiProcessed is true saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterKualiProcessedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.RequesterKualiProcessed = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.RequesterKualiProcessed);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion RequesterKualiProcessed Tests

        #region RequesterKualiApproved Tests

        /// <summary>
        /// Tests the RequesterKualiApproved is false saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterKualiApprovedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.RequesterKualiApproved = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.RequesterKualiApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the RequesterKualiApproved is true saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterKualiApprovedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.RequesterKualiApproved = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.RequesterKualiApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion RequesterKualiApproved Tests

        #region RequesterReceived Tests

        /// <summary>
        /// Tests the RequesterReceived is false saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterReceivedIsFalseSaves()
        {
            #region Arrange
            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.RequesterReceived = false;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailPreferences.RequesterReceived);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the RequesterReceived is true saves.
        /// </summary>
        [TestMethod]
        public void TestRequesterReceivedIsTrueSaves()
        {
            #region Arrange
            var emailPreferences = GetValid(9);
            emailPreferences.RequesterReceived = true;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailPreferences.RequesterReceived);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        #endregion RequesterReceived Tests



        #region ApproverAccountManagerApproved Tests

        /// <summary>
        /// Tests the ApproverAccountManagerApproved is false saves.
        /// </summary>
        [TestMethod]
        public void TestApproverAccountManagerApprovedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.ApproverAccountManagerApproved = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.ApproverAccountManagerApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the ApproverAccountManagerApproved is true saves.
        /// </summary>
        [TestMethod]
        public void TestApproverAccountManagerApprovedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.ApproverAccountManagerApproved = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.ApproverAccountManagerApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion ApproverAccountManagerApproved Tests

        #region ApproverAccountManagerDenied Tests

        /// <summary>
        /// Tests the ApproverAccountManagerDenied is false saves.
        /// </summary>
        [TestMethod]
        public void TestApproverAccountManagerDeniedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.ApproverAccountManagerDenied = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.ApproverAccountManagerDenied);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the ApproverAccountManagerDenied is true saves.
        /// </summary>
        [TestMethod]
        public void TestApproverAccountManagerDeniedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.ApproverAccountManagerDenied = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.ApproverAccountManagerDenied);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion ApproverAccountManagerDenied Tests

        #region ApproverPurchaserProcessed Tests

        /// <summary>
        /// Tests the ApproverPurchaserProcessed is false saves.
        /// </summary>
        [TestMethod]
        public void TestApproverPurchaserProcessedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.ApproverPurchaserProcessed = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.ApproverPurchaserProcessed);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the ApproverPurchaserProcessed is true saves.
        /// </summary>
        [TestMethod]
        public void TestApproverPurchaserProcessedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.ApproverPurchaserProcessed = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.ApproverPurchaserProcessed);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion ApproverPurchaserProcessed Tests

        #region ApproverKualiApproved Tests

        /// <summary>
        /// Tests the ApproverKualiApproved is false saves.
        /// </summary>
        [TestMethod]
        public void TestApproverKualiApprovedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.ApproverKualiApproved = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.ApproverKualiApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the ApproverKualiApproved is true saves.
        /// </summary>
        [TestMethod]
        public void TestApproverKualiApprovedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.ApproverKualiApproved = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.ApproverKualiApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion ApproverKualiApproved Tests

        #region ApproverOrderCompleted Tests

        /// <summary>
        /// Tests the ApproverOrderCompleted is false saves.
        /// </summary>
        [TestMethod]
        public void TestApproverOrderCompletedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.ApproverOrderCompleted = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.ApproverOrderCompleted);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the ApproverOrderCompleted is true saves.
        /// </summary>
        [TestMethod]
        public void TestApproverOrderCompletedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.ApproverOrderCompleted = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.ApproverOrderCompleted);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion ApproverOrderCompleted Tests

        #region ApproverOrderArrive Tests

        /// <summary>
        /// Tests the ApproverOrderArrive is false saves.
        /// </summary>
        [TestMethod]
        public void TestApproverOrderArriveIsFalseSaves()
        {
            #region Arrange
            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.ApproverOrderArrive = false;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailPreferences.ApproverOrderArrive);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ApproverOrderArrive is true saves.
        /// </summary>
        [TestMethod]
        public void TestApproverOrderArriveIsTrueSaves()
        {
            #region Arrange
            var emailPreferences = GetValid(9);
            emailPreferences.ApproverOrderArrive = true;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailPreferences.ApproverOrderArrive);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        #endregion ApproverOrderArrive Tests



        #region AccountManagerPurchaserProcessed Tests

        /// <summary>
        /// Tests the AccountManagerPurchaserProcessed is false saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerPurchaserProcessedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.AccountManagerPurchaserProcessed = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.AccountManagerPurchaserProcessed);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the AccountManagerPurchaserProcessed is true saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerPurchaserProcessedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.AccountManagerPurchaserProcessed = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.AccountManagerPurchaserProcessed);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion AccountManagerPurchaserProcessed Tests

        #region AccountManagerKualiApproved Tests

        /// <summary>
        /// Tests the AccountManagerKualiApproved is false saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerKualiApprovedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.AccountManagerKualiApproved = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.AccountManagerKualiApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the AccountManagerKualiApproved is true saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerKualiApprovedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.AccountManagerKualiApproved = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.AccountManagerKualiApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion AccountManagerKualiApproved Tests

        #region AccountManagerOrderArrive Tests

        /// <summary>
        /// Tests the AccountManagerOrderArrive is false saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerOrderArriveIsFalseSaves()
        {
            #region Arrange
            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.AccountManagerOrderArrive = false;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailPreferences.AccountManagerOrderArrive);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AccountManagerOrderArrive is true saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerOrderArriveIsTrueSaves()
        {
            #region Arrange
            var emailPreferences = GetValid(9);
            emailPreferences.AccountManagerOrderArrive = true;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailPreferences.AccountManagerOrderArrive);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        #endregion AccountManagerOrderArrive Tests

        #region ShowAccountInEmail Tests

        /// <summary>
        /// Tests the ShowAccountInEmail is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowAccountInEmailIsFalseSaves()
        {
            #region Arrange
            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.ShowAccountInEmail = false;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailPreferences.ShowAccountInEmail);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowAccountInEmail is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowAccountInEmailIsTrueSaves()
        {
            #region Arrange
            var emailPreferences = GetValid(9);
            emailPreferences.ShowAccountInEmail = true;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailPreferences.ShowAccountInEmail);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowAccountInEmail Tests

    


        #region AccountManagerOrderCompleted Tests

        /// <summary>
        /// Tests the AccountManagerOrderCompleted is false saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerOrderCompletedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.AccountManagerOrderCompleted = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.AccountManagerOrderCompleted);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the AccountManagerOrderCompleted is true saves.
        /// </summary>
        [TestMethod]
        public void TestAccountManagerOrderCompletedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.AccountManagerOrderCompleted = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.AccountManagerOrderCompleted);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion AccountManagerOrderCompleted Tests

        #region PurchaserKualiApproved Tests

        /// <summary>
        /// Tests the PurchaserKualiApproved is false saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserKualiApprovedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.PurchaserKualiApproved = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.PurchaserKualiApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the PurchaserKualiApproved is true saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserKualiApprovedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.PurchaserKualiApproved = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.PurchaserKualiApproved);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion PurchaserKualiApproved Tests

        #region PurchaserOrderArrive Tests

        /// <summary>
        /// Tests the PurchaserOrderArrive is false saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserOrderArriveIsFalseSaves()
        {
            #region Arrange
            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.PurchaserOrderArrive = false;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailPreferences.PurchaserOrderArrive);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PurchaserOrderArrive is true saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserOrderArriveIsTrueSaves()
        {
            #region Arrange
            var emailPreferences = GetValid(9);
            emailPreferences.PurchaserOrderArrive = true;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailPreferences.PurchaserOrderArrive);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        #endregion PurchaserOrderArrive Tests


        #region PurchaserOrderCompleted Tests

        /// <summary>
        /// Tests the PurchaserOrderCompleted is false saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserOrderCompletedIsFalseSaves()
        {
            #region Arrange

            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.PurchaserOrderCompleted = false;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailPreferences.PurchaserOrderCompleted);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the PurchaserOrderCompleted is true saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserOrderCompletedIsTrueSaves()
        {
            #region Arrange

            var emailPreferences = GetValid(9);
            emailPreferences.PurchaserOrderCompleted = true;

            #endregion Arrange

            #region Act

            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailPreferences.PurchaserOrderCompleted);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());

            #endregion Assert
        }

        #endregion PurchaserOrderCompleted Tests

        #region NotificationType Tests


        [TestMethod]
        public void TestNotificationType1()
        {
            #region Arrange
            var record = CreateValidEntities.EmailPreferences(9);
            #endregion Arrange

            #region Act
            record.NotificationType = EmailPreferences.NotificationTypes.Daily;
            #endregion Act

            #region Assert
            Assert.AreEqual("Daily", record.NotificationType.ToString());
            #endregion Assert		
        }

        [TestMethod]
        public void TestNotificationType2()
        {
            #region Arrange
            var record = CreateValidEntities.EmailPreferences(9);
            #endregion Arrange

            #region Act
            record.NotificationType = EmailPreferences.NotificationTypes.PerEvent;
            #endregion Act

            #region Assert
            Assert.AreEqual("PerEvent", record.NotificationType.ToString());
            #endregion Assert
        }

        [TestMethod]
        public void TestNotificationType3()
        {
            #region Arrange
            var record = CreateValidEntities.EmailPreferences(9);
            #endregion Arrange

            #region Act
            record.NotificationType = EmailPreferences.NotificationTypes.Weekly;
            #endregion Act

            #region Assert
            Assert.AreEqual("Weekly", record.NotificationType.ToString());
            #endregion Assert
        }

        #endregion NotificationType Tests

        #region PurchaserKfsItemReceived Tests

        /// <summary>
        /// Tests the PurchaserKfsItemReceived is false saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserKfsItemReceivedIsFalseSaves()
        {
            #region Arrange
            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.PurchaserKfsItemReceived = false;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailPreferences.PurchaserKfsItemReceived);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PurchaserKfsItemReceived is true saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserKfsItemReceivedIsTrueSaves()
        {
            #region Arrange
            var emailPreferences = GetValid(9);
            emailPreferences.PurchaserKfsItemReceived = true;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailPreferences.PurchaserKfsItemReceived);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        #endregion PurchaserKfsItemReceived Tests

        #region PurchaserPCardItemReceived Tests

        /// <summary>
        /// Tests the PurchaserPCardItemReceived is false saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserPCardItemReceivedIsFalseSaves()
        {
            #region Arrange
            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.PurchaserPCardItemReceived = false;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailPreferences.PurchaserPCardItemReceived);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PurchaserPCardItemReceived is true saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserPCardItemReceivedIsTrueSaves()
        {
            #region Arrange
            var emailPreferences = GetValid(9);
            emailPreferences.PurchaserPCardItemReceived = true;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailPreferences.PurchaserPCardItemReceived);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        #endregion PurchaserPCardItemReceived Tests

        #region PurchaserCampusServicesItemReceived Tests

        /// <summary>
        /// Tests the PurchaserCampusServicesItemReceived is false saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserCampusServicesItemReceivedIsFalseSaves()
        {
            #region Arrange
            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.PurchaserCampusServicesItemReceived = false;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailPreferences.PurchaserCampusServicesItemReceived);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PurchaserCampusServicesItemReceived is true saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserCampusServicesItemReceivedIsTrueSaves()
        {
            #region Arrange
            var emailPreferences = GetValid(9);
            emailPreferences.PurchaserCampusServicesItemReceived = true;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailPreferences.PurchaserCampusServicesItemReceived);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        #endregion PurchaserCampusServicesItemReceived Tests

        #region AddAttachment Tests

        /// <summary>
        /// Tests the AddAttachment is false saves.
        /// </summary>
        [TestMethod]
        public void TestAddAttachmentIsFalseSaves()
        {
            #region Arrange
            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.AddAttachment = false;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailPreferences.AddAttachment);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AddAttachment is true saves.
        /// </summary>
        [TestMethod]
        public void TestAddAttachmentIsTrueSaves()
        {
            #region Arrange
            var emailPreferences = GetValid(9);
            emailPreferences.AddAttachment = true;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailPreferences.AddAttachment);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        #endregion AddAttachment Tests

        #region AddNote Tests

        /// <summary>
        /// Tests the AddNote is false saves.
        /// </summary>
        [TestMethod]
        public void TestAddNoteIsFalseSaves()
        {
            #region Arrange
            EmailPreferences emailPreferences = GetValid(9);
            emailPreferences.AddNote = false;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailPreferences.AddNote);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AddNote is true saves.
        /// </summary>
        [TestMethod]
        public void TestAddNoteIsTrueSaves()
        {
            #region Arrange
            var emailPreferences = GetValid(9);
            emailPreferences.AddNote = true;
            #endregion Arrange

            #region Act
            EmailPreferencesRepository.DbContext.BeginTransaction();
            EmailPreferencesRepository.EnsurePersistent(emailPreferences);
            EmailPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(emailPreferences.AddNote);
            Assert.IsFalse(emailPreferences.IsTransient());
            Assert.IsTrue(emailPreferences.IsValid());
            #endregion Assert
        }

        #endregion AddNote Tests
       


        #region Constructor Tests

        [TestMethod]
        public void TestExpectedDefaultValues()
        {
            #region Arrange
            var record = new EmailPreferences("Test");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("Test", record.Id);
            Assert.IsTrue(record.RequesterOrderSubmission);
            Assert.IsTrue(record.RequesterApproverApproved);
            Assert.IsTrue(record.RequesterApproverChanged);
            Assert.IsTrue(record.RequesterAccountManagerApproved);
            Assert.IsTrue(record.RequesterAccountManagerChanged);
            Assert.IsTrue(record.RequesterPurchaserAction);
            Assert.IsTrue(record.RequesterPurchaserChanged);
            Assert.IsTrue(record.RequesterKualiProcessed);
            Assert.IsTrue(record.RequesterKualiApproved);
            Assert.IsTrue(record.RequesterReceived);

            Assert.IsTrue(record.ApproverAccountManagerApproved);
            Assert.IsTrue(record.ApproverAccountManagerDenied);
            Assert.IsTrue(record.ApproverPurchaserProcessed);
            Assert.IsTrue(record.ApproverKualiApproved);
            Assert.IsTrue(record.ApproverOrderCompleted);
            Assert.IsTrue(record.ApproverOrderArrive);

            Assert.IsTrue(record.AccountManagerPurchaserProcessed);
            Assert.IsTrue(record.AccountManagerKualiApproved);
            Assert.IsTrue(record.AccountManagerOrderCompleted);
            Assert.IsTrue(record.AccountManagerOrderArrive);
            Assert.IsFalse(record.ShowAccountInEmail);

            Assert.IsTrue(record.PurchaserKualiApproved);
            Assert.IsTrue(record.PurchaserOrderCompleted);
            Assert.IsTrue(record.PurchaserOrderArrive);
            Assert.IsTrue(record.PurchaserKfsItemReceived);
            Assert.IsTrue(record.PurchaserPCardItemReceived);
            Assert.IsTrue(record.PurchaserCampusServicesItemReceived);

            Assert.IsTrue(record.AddAttachment);
            Assert.IsTrue(record.AddNote);

            Assert.AreEqual(EmailPreferences.NotificationTypes.PerEvent, record.NotificationType);
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
            expectedFields.Add(new NameAndType("AccountManagerKualiApproved", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Kuali Approved\")]"
            }));
            expectedFields.Add(new NameAndType("AccountManagerOrderArrive", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Order Arrived\")]"
            }));
            expectedFields.Add(new NameAndType("AccountManagerOrderCompleted", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Request Completed\")]"
            }));
            expectedFields.Add(new NameAndType("AccountManagerPurchaserProcessed", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Purchaser Processed\")]"
            }));
            expectedFields.Add(new NameAndType("AddAttachment", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Attachment Added\")]"
            }));
            expectedFields.Add(new NameAndType("AddNote", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Note Added\")]"
            }));
            expectedFields.Add(new NameAndType("ApproverAccountManagerApproved", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Account Manager Reviewed\")]"
            }));
            expectedFields.Add(new NameAndType("ApproverAccountManagerDenied", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Account Manager Denied\")]"
            }));
            expectedFields.Add(new NameAndType("ApproverKualiApproved", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Kuali Approved\")]"
            }));
            expectedFields.Add(new NameAndType("ApproverOrderArrive", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Order Arrived\")]"
            }));
            expectedFields.Add(new NameAndType("ApproverOrderCompleted", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Request Completed\")]"
            }));
            expectedFields.Add(new NameAndType("ApproverPurchaserProcessed", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Purchaser Processed\")]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("NotificationType", "Purchasing.Core.Domain.EmailPreferences+NotificationTypes", new List<string>()));
            expectedFields.Add(new NameAndType("PurchaserCampusServicesItemReceived", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Campus Services Item Received\")]"
            }));
            expectedFields.Add(new NameAndType("PurchaserKfsItemReceived", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"KFS Item Received\")]"
            }));
            expectedFields.Add(new NameAndType("PurchaserKualiApproved", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Kuali Approved\")]"
            }));
            expectedFields.Add(new NameAndType("PurchaserOrderArrive", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Order Arrived\")]"
            }));
            expectedFields.Add(new NameAndType("PurchaserOrderCompleted", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Request Completed\")]"
            }));
            expectedFields.Add(new NameAndType("PurchaserPCardItemReceived", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"P Card Item Received\")]"
            }));
            expectedFields.Add(new NameAndType("RequesterAccountManagerApproved", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Account Manager Reviewed\")]"
            }));
            expectedFields.Add(new NameAndType("RequesterAccountManagerChanged", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Account Manager Updates Request\")]"
            }));
            expectedFields.Add(new NameAndType("RequesterApproverApproved", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Approver Reviewed\")]"
            }));
            expectedFields.Add(new NameAndType("RequesterApproverChanged", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Approver Updates Request\")]"
            }));
            expectedFields.Add(new NameAndType("RequesterKualiApproved", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Kuali Approved\")]"
            }));
            expectedFields.Add(new NameAndType("RequesterKualiProcessed", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Kuali Updates Request\")]"
            }));
            expectedFields.Add(new NameAndType("RequesterOrderSubmission", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Order Submitted\")]"
            }));
            expectedFields.Add(new NameAndType("RequesterPurchaserAction", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Purchaser Processed\")]"
            }));
            expectedFields.Add(new NameAndType("RequesterPurchaserChanged", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Purchaser Updates Request\")]"
            }));
            expectedFields.Add(new NameAndType("RequesterReceived", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Item Received\")]"
            }));
            expectedFields.Add(new NameAndType("ShowAccountInEmail", "System.Boolean", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Accounts in Email\")]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(EmailPreferences));

        }

        #endregion Reflection of Database.	
		
		
    }
}