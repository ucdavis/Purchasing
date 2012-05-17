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
using UCDArch.Testing;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Approval
    /// LookupFieldName:	Completed
    /// </summary>
    [TestClass]
    public class ApprovalRepositoryTests : AbstractRepositoryTests<Approval, int, ApprovalMap>
    {
        /// <summary>
        /// Gets or sets the Approval repository.
        /// </summary>
        /// <value>The Approval repository.</value>
        public IRepository<Approval> ApprovalRepository { get; set; }
        public IRepository<Split> SplitRepository { get; set; }

        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalRepositoryTests"/> class.
        /// </summary>
        public ApprovalRepositoryTests()
        {
            ApprovalRepository = new Repository<Approval>();
            SplitRepository = new Repository<Split>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Approval GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Approval(counter);
            rtValue.Order = Repository.OfType<Order>().Queryable.FirstOrDefault();
            rtValue.StatusCode = Repository.OfType<OrderStatusCode>().Queryable.FirstOrDefault();
            rtValue.User = null;

            if(counter != null && counter == 3)
            {
                rtValue.Completed = true;
            }
            else
            {
                rtValue.Completed = false;
            }

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Approval> GetQuery(int numberAtEnd)
        {
            return ApprovalRepository.Queryable.Where(a => a.Id == numberAtEnd);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Approval entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Approval entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Completed);
                    break;
                case ARTAction.Restore:
                    entity.Completed = BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.Completed;
                    entity.Completed = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            ApprovalRepository.DbContext.BeginTransaction();
            LoadStatusCodes(3);
            LoadOrders(3);
            ApprovalRepository.DbContext.CommitTransaction();

            ApprovalRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ApprovalRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region Completed Tests

        /// <summary>
        /// Tests the Completed is false saves.
        /// </summary>
        [TestMethod]
        public void TestCompletedIsFalseSaves()
        {
            #region Arrange
            Approval approval = GetValid(9);
            approval.Completed = false;
            #endregion Arrange

            #region Act
            ApprovalRepository.DbContext.BeginTransaction();
            ApprovalRepository.EnsurePersistent(approval);
            ApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(approval.Completed);
            Assert.IsFalse(approval.IsTransient());
            Assert.IsTrue(approval.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Completed is true saves.
        /// </summary>
        [TestMethod]
        public void TestCompletedIsTrueSaves()
        {
            #region Arrange
            var approval = GetValid(9);
            approval.Completed = true;
            #endregion Arrange

            #region Act
            ApprovalRepository.DbContext.BeginTransaction();
            ApprovalRepository.EnsurePersistent(approval);
            ApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(approval.Completed);
            Assert.IsFalse(approval.IsTransient());
            Assert.IsTrue(approval.IsValid());
            #endregion Assert
        }

        #endregion Completed Tests

        #region User Tests

        [TestMethod]
        public void TestApprovalWithNullUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = null;
            #endregion Arrange

            #region Act
            ApprovalRepository.DbContext.BeginTransaction();
            ApprovalRepository.EnsurePersistent(record);
            ApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.User);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());         
            #endregion Assert		
        }

        [TestMethod]
        public void TestApprovalWithExistingUserSaves()
        {
            #region Arrange
            var userRepository = new RepositoryWithTypedId<User, string>();
            userRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            userRepository.DbContext.CommitTransaction();
            var record = GetValid(9);
            record.User = userRepository.Queryable.Single(a => a.Id == "2");
            #endregion Arrange

            #region Act
            ApprovalRepository.DbContext.BeginTransaction();
            ApprovalRepository.EnsurePersistent(record);
            ApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("2", record.User.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof (NHibernate.TransientObjectException))]
        public void TestApprovalWithNewUserDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.User = new User();               
                thisFar = true;
                #endregion Arrange

                #region Act
                ApprovalRepository.DbContext.BeginTransaction();
                ApprovalRepository.EnsurePersistent(record);
                ApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw;
            }
        }

        #endregion User Tests

        #region SecondaryUser Tests

        [TestMethod]
        public void TestApprovalWithNullSecondaryUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.SecondaryUser = null;
            #endregion Arrange

            #region Act
            ApprovalRepository.DbContext.BeginTransaction();
            ApprovalRepository.EnsurePersistent(record);
            ApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.SecondaryUser);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestApprovalWithExistingSecondaryUserSaves()
        {
            #region Arrange
            var userRepository = new RepositoryWithTypedId<User, string>();
            userRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            userRepository.DbContext.CommitTransaction();
            var record = GetValid(9);
            record.SecondaryUser = userRepository.Queryable.Single(a => a.Id == "2");
            #endregion Arrange

            #region Act
            ApprovalRepository.DbContext.BeginTransaction();
            ApprovalRepository.EnsurePersistent(record);
            ApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("2", record.SecondaryUser.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestApprovalWithNewSecondaryUserDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.SecondaryUser = new User();
                thisFar = true;
                #endregion Arrange

                #region Act
                ApprovalRepository.DbContext.BeginTransaction();
                ApprovalRepository.EnsurePersistent(record);
                ApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw;
            }
        }

        #endregion SecondaryUser Tests

        #region StatusCode Tests

        [TestMethod]
        public void TestApprovalWithExistingStatusCodeSaves()
        {
            #region Arrange
            var orderStatusCodeRepository = new RepositoryWithTypedId<OrderStatusCode, string>();
            var record = GetValid(9);
            record.StatusCode = orderStatusCodeRepository.Queryable.Single(a => a.Id == "2");
            #endregion Arrange

            #region Act
            ApprovalRepository.DbContext.BeginTransaction();
            ApprovalRepository.EnsurePersistent(record);
            ApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("2", record.StatusCode.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestApprovalWithNullStatusCodeDoesNotSave()
        {
            var thisFar = false;
            Approval record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.StatusCode = null;

                thisFar = true;
                #endregion Arrange

                #region Act
                ApprovalRepository.DbContext.BeginTransaction();
                ApprovalRepository.EnsurePersistent(record);
                ApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsTrue(thisFar);
                Assert.AreEqual(record.StatusCode, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The StatusCode field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (NHibernate.TransientObjectException))]
        public void TestApprovalWithNewStatusCodeDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.StatusCode = new OrderStatusCode();

                thisFar = true;
                #endregion Arrange

                #region Act
                ApprovalRepository.DbContext.BeginTransaction();
                ApprovalRepository.EnsurePersistent(record);
                ApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.OrderStatusCode, Entity: Purchasing.Core.Domain.OrderStatusCode", ex.Message);
                throw;
            }
        }

        #endregion StatusCode Tests

        #region Order Tests

        [TestMethod]
        public void TestApprovalWithExistingOrderSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = Repository.OfType<Order>().Queryable.Single(a => a.Id == 2);
            #endregion Arrange

            #region Act
            ApprovalRepository.DbContext.BeginTransaction();
            ApprovalRepository.EnsurePersistent(record);
            ApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, record.Order.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestApprovalWithNullOrderDoesNotSave()
        {
            var thisFar = false;
            Approval record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Order = null;

                thisFar = true;
                #endregion Arrange

                #region Act
                ApprovalRepository.DbContext.BeginTransaction();
                ApprovalRepository.EnsurePersistent(record);
                ApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception)
            {
                Assert.IsTrue(thisFar);
                Assert.AreEqual(record.Order, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Order field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestApprovalWithNewOrderDoesNotSave()
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
                ApprovalRepository.DbContext.BeginTransaction();
                ApprovalRepository.EnsurePersistent(record);
                ApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Order, Entity: Purchasing.Core.Domain.Order", ex.Message);
                throw;
            }
        }

        #endregion Order Tests

        #region Split Tests

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestApprovalNewSplitDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Split = new Split();
                thisFar = true;
                #endregion Arrange

                #region Act
                ApprovalRepository.DbContext.BeginTransaction();
                ApprovalRepository.EnsurePersistent(record);
                ApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);                                
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Split, Entity: Purchasing.Core.Domain.Split", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestApprovalWithExistingSplitSaves()
        {
            #region Arrange
            SplitRepository.DbContext.BeginTransaction();
            LoadSplits(3);
            SplitRepository.DbContext.CommitTransaction();
            var record = GetValid(9);
            record.Split = SplitRepository.Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            ApprovalRepository.DbContext.BeginTransaction();
            ApprovalRepository.EnsurePersistent(record);
            ApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Split.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        //Delete this one if not nullable
        [TestMethod]
        public void TestApprovalWithNullSplitSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Split = null;
            #endregion Arrange

            #region Act
            ApprovalRepository.DbContext.BeginTransaction();
            ApprovalRepository.EnsurePersistent(record);
            ApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Split);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Split Tests


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
            expectedFields.Add(new NameAndType("Completed", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
                                                                         {
                                                                             "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                                                                             "[System.Xml.Serialization.XmlIgnoreAttribute()]"
                                                                         }));
            expectedFields.Add(new NameAndType("Order", "Purchasing.Core.Domain.Order", new List<string>
                                                                         { 
                                                                             "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
                                                                         }));
            expectedFields.Add(new NameAndType("SecondaryUser", "Purchasing.Core.Domain.User", new List<string>()));
            expectedFields.Add(new NameAndType("Split", "Purchasing.Core.Domain.Split", new List<string>()));
            expectedFields.Add(new NameAndType("StatusCode", "Purchasing.Core.Domain.OrderStatusCode", new List<string>
                                                                         { 
                                                                             "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
                                                                         }));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>()));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Approval));

        }

        #endregion Reflection of Database.	
		
		
    }
}