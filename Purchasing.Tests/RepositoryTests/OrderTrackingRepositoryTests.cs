using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		OrderTracking
    /// LookupFieldName:	Description
    /// </summary>
    [TestClass]
    public class OrderTrackingRepositoryTests : AbstractRepositoryTests<OrderTracking, int, OrderTrackingMap>
    {
        /// <summary>
        /// Gets or sets the OrderTracking repository.
        /// </summary>
        /// <value>The OrderTracking repository.</value>
        public IRepository<OrderTracking> OrderTrackingRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; }
        public IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodeRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderTrackingRepositoryTests"/> class.
        /// </summary>
        public OrderTrackingRepositoryTests()
        {
            OrderTrackingRepository = new Repository<OrderTracking>();
            OrderRepository = new Repository<Order>();
            UserRepository = new RepositoryWithTypedId<User, string>();
            OrderStatusCodeRepository = new RepositoryWithTypedId<OrderStatusCode, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override OrderTracking GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.OrderTracking(counter);
            rtValue.StatusCode = OrderStatusCodeRepository.Queryable.First();
            rtValue.Order = OrderRepository.Queryable.First();
            rtValue.User = UserRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<OrderTracking> GetQuery(int numberAtEnd)
        {
            return OrderTrackingRepository.Queryable.Where(a => a.Description.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(OrderTracking entity, int counter)
        {
            Assert.AreEqual("Description" + counter, entity.Description);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(OrderTracking entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Description);
                    break;
                case ARTAction.Restore:
                    entity.Description = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Description;
                    entity.Description = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            LoadOrders(3);
            UserRepository.DbContext.CommitTransaction();

            OrderTrackingRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            OrderTrackingRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Description Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Description with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithNullValueDoesNotSave()
        {
            OrderTracking orderTracking = null;
            try
            {
                #region Arrange
                orderTracking = GetValid(9);
                orderTracking.Description = null;
                #endregion Arrange

                #region Act
                OrderTrackingRepository.DbContext.BeginTransaction();
                OrderTrackingRepository.EnsurePersistent(orderTracking);
                OrderTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderTracking);
                var results = orderTracking.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Description"));
                Assert.IsTrue(orderTracking.IsTransient());
                Assert.IsFalse(orderTracking.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Description with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithEmptyStringDoesNotSave()
        {
            OrderTracking orderTracking = null;
            try
            {
                #region Arrange
                orderTracking = GetValid(9);
                orderTracking.Description = string.Empty;
                #endregion Arrange

                #region Act
                OrderTrackingRepository.DbContext.BeginTransaction();
                OrderTrackingRepository.EnsurePersistent(orderTracking);
                OrderTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderTracking);
                var results = orderTracking.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Description"));
                Assert.IsTrue(orderTracking.IsTransient());
                Assert.IsFalse(orderTracking.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Description with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDescriptionWithSpacesOnlyDoesNotSave()
        {
            OrderTracking orderTracking = null;
            try
            {
                #region Arrange
                orderTracking = GetValid(9);
                orderTracking.Description = " ";
                #endregion Arrange

                #region Act
                OrderTrackingRepository.DbContext.BeginTransaction();
                OrderTrackingRepository.EnsurePersistent(orderTracking);
                OrderTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderTracking);
                var results = orderTracking.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Description"));
                Assert.IsTrue(orderTracking.IsTransient());
                Assert.IsFalse(orderTracking.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Description with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithOneCharacterSaves()
        {
            #region Arrange
            var orderTracking = GetValid(9);
            orderTracking.Description = "x";
            #endregion Arrange

            #region Act
            OrderTrackingRepository.DbContext.BeginTransaction();
            OrderTrackingRepository.EnsurePersistent(orderTracking);
            OrderTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderTracking.IsTransient());
            Assert.IsTrue(orderTracking.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithLongValueSaves()
        {
            #region Arrange
            var orderTracking = GetValid(9);
            orderTracking.Description = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            OrderTrackingRepository.DbContext.BeginTransaction();
            OrderTrackingRepository.EnsurePersistent(orderTracking);
            OrderTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, orderTracking.Description.Length);
            Assert.IsFalse(orderTracking.IsTransient());
            Assert.IsTrue(orderTracking.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Description Tests

        #region DateCreated Tests

        /// <summary>
        /// Tests the DateCreated with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            OrderTracking record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            OrderTrackingRepository.DbContext.BeginTransaction();
            OrderTrackingRepository.EnsurePersistent(record);
            OrderTrackingRepository.DbContext.CommitChanges();
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
            OrderTrackingRepository.DbContext.BeginTransaction();
            OrderTrackingRepository.EnsurePersistent(record);
            OrderTrackingRepository.DbContext.CommitChanges();
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
            OrderTrackingRepository.DbContext.BeginTransaction();
            OrderTrackingRepository.EnsurePersistent(record);
            OrderTrackingRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert
        }
        #endregion DateCreated Tests

        #region StatusCode Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrderTrackingsFieldStatusCodeWithAValueOfNullDoesNotSave()
        {
            OrderTracking record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.StatusCode = null;
                #endregion Arrange

                #region Act
                OrderTrackingRepository.DbContext.BeginTransaction();
                OrderTrackingRepository.EnsurePersistent(record);
                OrderTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.StatusCode, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The StatusCode field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderTrackingNewStatusCodeDoesNotSave()
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
                OrderTrackingRepository.DbContext.BeginTransaction();
                OrderTrackingRepository.EnsurePersistent(record);
                OrderTrackingRepository.DbContext.CommitTransaction();
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

        [TestMethod]
        public void TestOrderTrackingWithExistingStatusCodeSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            #endregion Arrange

            #region Act
            OrderTrackingRepository.DbContext.BeginTransaction();
            OrderTrackingRepository.EnsurePersistent(record);
            OrderTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(OrderStatusCode.Codes.Approver, record.StatusCode.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion StatusCode Tests

        #region Order Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrderTrackingsFieldOrderWithAValueOfNullDoesNotSave()
        {
            OrderTracking record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Order = null;
                #endregion Arrange

                #region Act
                OrderTrackingRepository.DbContext.BeginTransaction();
                OrderTrackingRepository.EnsurePersistent(record);
                OrderTrackingRepository.DbContext.CommitTransaction();
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
        public void TestOrderTrackingNewOrderDoesNotSave()
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
                OrderTrackingRepository.DbContext.BeginTransaction();
                OrderTrackingRepository.EnsurePersistent(record);
                OrderTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Order, Entity: Purchasing.Core.Domain.Order", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestOrderTrackingWithExistingOrderSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = OrderRepository.Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            OrderTrackingRepository.DbContext.BeginTransaction();
            OrderTrackingRepository.EnsurePersistent(record);
            OrderTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Order.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Order Tests

        #region User Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrderTrackingsFieldUserWithAValueOfNullDoesNotSave()
        {
            OrderTracking record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.User = null;
                #endregion Arrange

                #region Act
                OrderTrackingRepository.DbContext.BeginTransaction();
                OrderTrackingRepository.EnsurePersistent(record);
                OrderTrackingRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.User, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The User field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderTrackingNewUserDoesNotSave()
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
                OrderTrackingRepository.DbContext.BeginTransaction();
                OrderTrackingRepository.EnsurePersistent(record);
                OrderTrackingRepository.DbContext.CommitTransaction();
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

        [TestMethod]
        public void TestOrderTrackingWithExistingUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            OrderTrackingRepository.DbContext.BeginTransaction();
            OrderTrackingRepository.EnsurePersistent(record);
            OrderTrackingRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.User.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }


        #endregion User Tests


        #region Constructor Tests

        [TestMethod]
        public void TestConstructor()
        {
            #region Arrange
            var record = new OrderTracking();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, record.DateCreated.Date);
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
            expectedFields.Add(new NameAndType("DateCreated", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Description", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Order", "Purchasing.Core.Domain.Order", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("StatusCode", "Purchasing.Core.Domain.OrderStatusCode", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(OrderTracking));

        }

        #endregion Reflection of Database.	
		
		
    }
}