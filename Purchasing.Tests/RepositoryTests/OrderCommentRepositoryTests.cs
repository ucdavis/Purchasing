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
    /// Entity Name:		OrderComment
    /// LookupFieldName:	Text
    /// </summary>
    [TestClass]
    public class OrderCommentRepositoryTests : AbstractRepositoryTests<OrderComment, int, OrderCommentMap>
    {
        /// <summary>
        /// Gets or sets the OrderComment repository.
        /// </summary>
        /// <value>The OrderComment repository.</value>
        public IRepository<OrderComment> OrderCommentRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; } 

        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCommentRepositoryTests"/> class.
        /// </summary>
        public OrderCommentRepositoryTests()
        {
            OrderCommentRepository = new Repository<OrderComment>();
            UserRepository = new RepositoryWithTypedId<User, string>();
            OrderRepository = new Repository<Order>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override OrderComment GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.OrderComment(counter);
            rtValue.Order = OrderRepository.Queryable.First();
            rtValue.User = UserRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<OrderComment> GetQuery(int numberAtEnd)
        {
            return OrderCommentRepository.Queryable.Where(a => a.Text.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(OrderComment entity, int counter)
        {
            Assert.AreEqual("Text" + counter, entity.Text);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(OrderComment entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Text);
                    break;
                case ARTAction.Restore:
                    entity.Text = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Text;
                    entity.Text = updateValue;
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
            OrderCommentRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            OrderCommentRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Text Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Text with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTextWithNullValueDoesNotSave()
        {
            OrderComment orderComment = null;
            try
            {
                #region Arrange
                orderComment = GetValid(9);
                orderComment.Text = null;
                #endregion Arrange

                #region Act
                OrderCommentRepository.DbContext.BeginTransaction();
                OrderCommentRepository.EnsurePersistent(orderComment);
                OrderCommentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderComment);
                var results = orderComment.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Text"));
                Assert.IsTrue(orderComment.IsTransient());
                Assert.IsFalse(orderComment.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Text with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTextWithEmptyStringDoesNotSave()
        {
            OrderComment orderComment = null;
            try
            {
                #region Arrange
                orderComment = GetValid(9);
                orderComment.Text = string.Empty;
                #endregion Arrange

                #region Act
                OrderCommentRepository.DbContext.BeginTransaction();
                OrderCommentRepository.EnsurePersistent(orderComment);
                OrderCommentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderComment);
                var results = orderComment.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Text"));
                Assert.IsTrue(orderComment.IsTransient());
                Assert.IsFalse(orderComment.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Text with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTextWithSpacesOnlyDoesNotSave()
        {
            OrderComment orderComment = null;
            try
            {
                #region Arrange
                orderComment = GetValid(9);
                orderComment.Text = " ";
                #endregion Arrange

                #region Act
                OrderCommentRepository.DbContext.BeginTransaction();
                OrderCommentRepository.EnsurePersistent(orderComment);
                OrderCommentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderComment);
                var results = orderComment.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Text"));
                Assert.IsTrue(orderComment.IsTransient());
                Assert.IsFalse(orderComment.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Text with one character saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithOneCharacterSaves()
        {
            #region Arrange
            var orderComment = GetValid(9);
            orderComment.Text = "x";
            #endregion Arrange

            #region Act
            OrderCommentRepository.DbContext.BeginTransaction();
            OrderCommentRepository.EnsurePersistent(orderComment);
            OrderCommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderComment.IsTransient());
            Assert.IsTrue(orderComment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithLongValueSaves()
        {
            #region Arrange
            var orderComment = GetValid(9);
            orderComment.Text = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            OrderCommentRepository.DbContext.BeginTransaction();
            OrderCommentRepository.EnsurePersistent(orderComment);
            OrderCommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, orderComment.Text.Length);
            Assert.IsFalse(orderComment.IsTransient());
            Assert.IsTrue(orderComment.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Text Tests

        #region DateCreated Tests

        /// <summary>
        /// Tests the DateCreated with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            OrderComment record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            OrderCommentRepository.DbContext.BeginTransaction();
            OrderCommentRepository.EnsurePersistent(record);
            OrderCommentRepository.DbContext.CommitChanges();
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
            OrderCommentRepository.DbContext.BeginTransaction();
            OrderCommentRepository.EnsurePersistent(record);
            OrderCommentRepository.DbContext.CommitChanges();
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
            OrderCommentRepository.DbContext.BeginTransaction();
            OrderCommentRepository.EnsurePersistent(record);
            OrderCommentRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert
        }
        #endregion DateCreated Tests

        #region User Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrderCommentsFieldUserWithAValueOfNullDoesNotSave()
        {
            OrderComment record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.User = null;
                #endregion Arrange

                #region Act
                OrderCommentRepository.DbContext.BeginTransaction();
                OrderCommentRepository.EnsurePersistent(record);
                OrderCommentRepository.DbContext.CommitTransaction();
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
        public void TestOrderCommentNewUserDoesNotSave()
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
                OrderCommentRepository.DbContext.BeginTransaction();
                OrderCommentRepository.EnsurePersistent(record);
                OrderCommentRepository.DbContext.CommitTransaction();
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
        public void TestOrderCommentWithExistingUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            OrderCommentRepository.DbContext.BeginTransaction();
            OrderCommentRepository.EnsurePersistent(record);
            OrderCommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.User.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion User Tests

        #region Order Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrderCommentsFieldOrderWithAValueOfNullDoesNotSave()
        {
            OrderComment record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Order = null;
                #endregion Arrange

                #region Act
                OrderCommentRepository.DbContext.BeginTransaction();
                OrderCommentRepository.EnsurePersistent(record);
                OrderCommentRepository.DbContext.CommitTransaction();
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
        public void TestOrderCommentNewOrderDoesNotSave()
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
                OrderCommentRepository.DbContext.BeginTransaction();
                OrderCommentRepository.EnsurePersistent(record);
                OrderCommentRepository.DbContext.CommitTransaction();
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
        public void TestOrderCommentWithExistingOrderSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = OrderRepository.Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            OrderCommentRepository.DbContext.BeginTransaction();
            OrderCommentRepository.EnsurePersistent(record);
            OrderCommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Order.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Order Tests


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
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Order", "Purchasing.Core.Domain.Order", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Text", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(OrderComment));

        }

        #endregion Reflection of Database.	
		
		
    }
}