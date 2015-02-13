using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHibernate;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentNHibernate.Testing;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		OrderRequestSave
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class OrderRequestSaveRepositoryTests : AbstractRepositoryTests<OrderRequestSave, Guid, OrderRequestSaveMap>
    {
        /// <summary>
        /// Gets or sets the OrderRequestSave repository.
        /// </summary>
        /// <value>The OrderRequestSave repository.</value>
        public IRepository<OrderRequestSave> OrderRequestSaveRepository { get; set; }

        public IRepositoryWithTypedId<User, string> UserRepository { get; set; }
        public IRepository<Workgroup> WorkgroupRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRequestSaveRepositoryTests"/> class.
        /// </summary>
        public OrderRequestSaveRepositoryTests()
        {
            OrderRequestSaveRepository = new Repository<OrderRequestSave>();
            UserRepository = new RepositoryWithTypedId<User, string>();
            WorkgroupRepository = new Repository<Workgroup>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override OrderRequestSave GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.OrderRequestSave(counter);
            rtValue.User = UserRepository.Queryable.First();
            rtValue.Workgroup = WorkgroupRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<OrderRequestSave> GetQuery(int numberAtEnd)
        {
            return OrderRequestSaveRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(OrderRequestSave entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(OrderRequestSave entity, ARTAction action)
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
            WorkgroupRepository.DbContext.BeginTransaction();
            LoadOrganizations(3);
            LoadUsers(3);
            LoadWorkgroups(3);
            WorkgroupRepository.DbContext.CommitTransaction();

            OrderRequestSaveRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
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
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.Name = null;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsFalse(orderRequestSave.IsValid());
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
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.Name = string.Empty;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsFalse(orderRequestSave.IsValid());
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
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.Name = " ";
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsFalse(orderRequestSave.IsValid());
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
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.Name = "x".RepeatTimes((150 + 1));
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                Assert.AreEqual(150 + 1, orderRequestSave.Name.Length);
                var results = orderRequestSave.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "150"));
                Assert.IsFalse(orderRequestSave.IsValid());
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
            var orderRequestSave = GetValid(9);
            orderRequestSave.Name = "x";
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderRequestSave.IsTransient());
            Assert.IsTrue(orderRequestSave.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var orderRequestSave = GetValid(9);
            orderRequestSave.Name = "x".RepeatTimes(150);
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(150, orderRequestSave.Name.Length);
            Assert.IsFalse(orderRequestSave.IsTransient());
            Assert.IsTrue(orderRequestSave.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region User Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrderRequestSavesFieldUserWithAValueOfNullDoesNotSave()
        {
            OrderRequestSave record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.User = null;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(record);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.User, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The User field is required.");
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderRequestSaveNewUserDoesNotSave()
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
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(record);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.IsTrue(ex.Message.Contains("Entity: Purchasing.Core.Domain.User"));
                Assert.IsTrue(ex.Message.Contains("Type: Purchasing.Core.Domain.User"));
                throw;
            }
        }

        [TestMethod]
        public void TestOrderRequestSaveWithExistingUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(record);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.User.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }


        #endregion User Tests

        #region PreparedBy Tests

        //[TestMethod]
        //[ExpectedException(typeof (ApplicationException))]
        //public void TestOrderRequestSavesFieldPreparedByWithAValueOfNullDoesNotSave()
        //{
        //    OrderRequestSave record = null;
        //    try
        //    {
        //        #region Arrange
        //        record = GetValid(9);
        //        record.PreparedBy = null;
        //        #endregion Arrange

        //        #region Act
        //        OrderRequestSaveRepository.DbContext.BeginTransaction();
        //        OrderRequestSaveRepository.EnsurePersistent(record);
        //        OrderRequestSaveRepository.DbContext.CommitTransaction();
        //        #endregion Act
        //    }
        //    catch (Exception)
        //    {
        //        Assert.IsNotNull(record);
        //        Assert.AreEqual(record.PreparedBy, null);
        //        var results = record.ValidationResults().AsMessageList();
        //        results.AssertErrorsAre("The PreparedBy field is required.");
        //        Assert.IsTrue(record.IsTransient());
        //        Assert.IsFalse(record.IsValid());
        //        throw;
        //    }
        //}

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderRequestSaveNewPreparedByDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.PreparedBy = new User();
                thisFar = true;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(record);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.IsTrue(ex.Message.Contains("Entity: Purchasing.Core.Domain.User"));
                Assert.IsTrue(ex.Message.Contains("Type: Purchasing.Core.Domain.User"));
                throw;
            }
        }

        [TestMethod]
        public void TestOrderRequestSaveWithExistingPreparedBySaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.PreparedBy = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(record);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.PreparedBy.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        //Delete this one if not nullable
        [TestMethod]
        public void TestOrderRequestSaveWithNullPreparedBySaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.PreparedBy = null;
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(record);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.PreparedBy);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion PreparedBy Tests

        #region Workgroup Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrderRequestSavesFieldWorkgroupWithAValueOfNullDoesNotSave()
        {
            OrderRequestSave record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Workgroup = null;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(record);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Workgroup, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Workgroup field is required.");
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderRequestSaveNewWorkgroupDoesNotSave()
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
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(record);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.IsTrue(ex.Message.Contains("Entity: Purchasing.Core.Domain.Workgroup"));
                Assert.IsTrue(ex.Message.Contains("Type: Purchasing.Core.Domain.Workgroup"));
                throw;
            }
        }

        [TestMethod]
        public void TestOrderRequestSaveWithExistingWorkgroupSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(record);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Workgroup.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Workgroup Tests

        #region FormData Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the FormData with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFormDataWithNullValueDoesNotSave()
        {
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.FormData = null;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "FormData"));
                Assert.IsFalse(orderRequestSave.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FormData with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFormDataWithEmptyStringDoesNotSave()
        {
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.FormData = string.Empty;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "FormData"));
                Assert.IsFalse(orderRequestSave.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the FormData with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFormDataWithSpacesOnlyDoesNotSave()
        {
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.FormData = " ";
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "FormData"));
                Assert.IsFalse(orderRequestSave.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the FormData with one character saves.
        /// </summary>
        [TestMethod]
        public void TestFormDataWithOneCharacterSaves()
        {
            #region Arrange
            var orderRequestSave = GetValid(9);
            orderRequestSave.FormData = "x";
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderRequestSave.IsTransient());
            Assert.IsTrue(orderRequestSave.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FormData with long value saves.
        /// </summary>
        [TestMethod]
        public void TestFormDataWithLongValueSaves()
        {
            #region Arrange
            var orderRequestSave = GetValid(9);
            orderRequestSave.FormData = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, orderRequestSave.FormData.Length);
            Assert.IsFalse(orderRequestSave.IsTransient());
            Assert.IsTrue(orderRequestSave.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion FormData Tests

        #region AccountData Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the AccountData with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAccountDataWithNullValueDoesNotSave()
        {
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.AccountData = null;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "AccountData"));
                Assert.IsFalse(orderRequestSave.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the AccountData with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAccountDataWithEmptyStringDoesNotSave()
        {
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.AccountData = string.Empty;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "AccountData"));
                Assert.IsFalse(orderRequestSave.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the AccountData with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAccountDataWithSpacesOnlyDoesNotSave()
        {
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.AccountData = " ";
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "AccountData"));
                Assert.IsFalse(orderRequestSave.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the AccountData with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAccountDataWithOneCharacterSaves()
        {
            #region Arrange
            var orderRequestSave = GetValid(9);
            orderRequestSave.AccountData = "x";
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderRequestSave.IsTransient());
            Assert.IsTrue(orderRequestSave.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AccountData with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAccountDataWithLongValueSaves()
        {
            #region Arrange
            var orderRequestSave = GetValid(9);
            orderRequestSave.AccountData = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, orderRequestSave.AccountData.Length);
            Assert.IsFalse(orderRequestSave.IsTransient());
            Assert.IsTrue(orderRequestSave.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion AccountData Tests

        #region DateCreated Tests

        /// <summary>
        /// Tests the DateCreated with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            OrderRequestSave record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(record);
            OrderRequestSaveRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the DateCreated with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(record);
            OrderRequestSaveRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateCreated with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(record);
            OrderRequestSaveRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert
        }
        #endregion DateCreated Tests

        #region LastUpdate Tests

        /// <summary>
        /// Tests the LastUpdate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestLastUpdateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            OrderRequestSave record = GetValid(99);
            record.LastUpdate = compareDate;
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(record);
            OrderRequestSaveRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastUpdate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the LastUpdate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestLastUpdateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.LastUpdate = compareDate;
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(record);
            OrderRequestSaveRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastUpdate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastUpdate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestLastUpdateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.LastUpdate = compareDate;
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(record);
            OrderRequestSaveRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastUpdate);
            #endregion Assert
        }
        #endregion LastUpdate Tests
        
        #region Version Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Version with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestVersionWithNullValueDoesNotSave()
        {
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.Version = null;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Version"));
                Assert.IsFalse(orderRequestSave.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Version with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestVersionWithEmptyStringDoesNotSave()
        {
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.Version = string.Empty;
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Version"));
                Assert.IsFalse(orderRequestSave.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Version with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestVersionWithSpacesOnlyDoesNotSave()
        {
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.Version = " ";
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                var results = orderRequestSave.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Version"));
                Assert.IsFalse(orderRequestSave.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Version with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestVersionWithTooLongValueDoesNotSave()
        {
            OrderRequestSave orderRequestSave = null;
            try
            {
                #region Arrange
                orderRequestSave = GetValid(9);
                orderRequestSave.Version = "x".RepeatTimes((15 + 1));
                #endregion Arrange

                #region Act
                OrderRequestSaveRepository.DbContext.BeginTransaction();
                OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
                OrderRequestSaveRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderRequestSave);
                Assert.AreEqual(15 + 1, orderRequestSave.Version.Length);
                var results = orderRequestSave.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Version", "15"));
                Assert.IsFalse(orderRequestSave.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Version with one character saves.
        /// </summary>
        [TestMethod]
        public void TestVersionWithOneCharacterSaves()
        {
            #region Arrange
            var orderRequestSave = GetValid(9);
            orderRequestSave.Version = "x";
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderRequestSave.IsTransient());
            Assert.IsTrue(orderRequestSave.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Version with long value saves.
        /// </summary>
        [TestMethod]
        public void TestVersionWithLongValueSaves()
        {
            #region Arrange
            var orderRequestSave = GetValid(9);
            orderRequestSave.Version = "x".RepeatTimes(15);
            #endregion Arrange

            #region Act
            OrderRequestSaveRepository.DbContext.BeginTransaction();
            OrderRequestSaveRepository.EnsurePersistent(orderRequestSave);
            OrderRequestSaveRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(15, orderRequestSave.Version.Length);
            Assert.IsFalse(orderRequestSave.IsTransient());
            Assert.IsTrue(orderRequestSave.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Version Tests

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
            expectedFields.Add(new NameAndType("AccountData", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("DateCreated", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("FormData", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Guid", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("LastUpdate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)150)]"
            }));
            expectedFields.Add(new NameAndType("PreparedBy", "Purchasing.Core.Domain.User", new List<string>()));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Version", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)15)]"
            }));
            expectedFields.Add(new NameAndType("Workgroup", "Purchasing.Core.Domain.Workgroup", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(OrderRequestSave));

        }

        #endregion Reflection of Database.	
		
		
    }
}