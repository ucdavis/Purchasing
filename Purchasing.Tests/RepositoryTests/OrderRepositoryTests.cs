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
    /// Entity Name:		Order
    /// LookupFieldName:	DeliverTo
    /// </summary>
    [TestClass]
    public class OrderRepositoryTests : AbstractRepositoryTests<Order, int, OrderMap>
    {
        /// <summary>
        /// Gets or sets the Order repository.
        /// </summary>
        /// <value>The Order repository.</value>
        public IRepository<Order> OrderRepository { get; set; }
        public IRepositoryWithTypedId<OrderType, string> OrderTypeRepository { get; set; }
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository { get; set; }
        public IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodeRepository { get; set; }
        private IRepositoryWithTypedId<User, string> UserRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRepositoryTests"/> class.
        /// </summary>
        public OrderRepositoryTests()
        {
            OrderRepository = new Repository<Order>();
            OrderTypeRepository = new RepositoryWithTypedId<OrderType, string>();
            OrganizationRepository = new RepositoryWithTypedId<Organization, string>();
            OrderStatusCodeRepository = new RepositoryWithTypedId<OrderStatusCode, string>();
            UserRepository = new RepositoryWithTypedId<User, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Order GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Order(counter);
            rtValue.OrderType = OrderTypeRepository.Queryable.Single(a => a.Id == OrderType.Types.OrderRequest);
            rtValue.Vendor = Repository.OfType<WorkgroupVendor>().Queryable.First();
            rtValue.Address = Repository.OfType<WorkgroupAddress>().Queryable.First();
            rtValue.Workgroup = Repository.OfType<Workgroup>().Queryable.First();
            rtValue.Organization = OrganizationRepository.Queryable.First();
            rtValue.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            rtValue.CreatedBy = UserRepository.Queryable.Single(a => a.Id == "2");
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Order> GetQuery(int numberAtEnd)
        {
            return OrderRepository.Queryable.Where(a => a.DeliverTo.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Order entity, int counter)
        {
            Assert.AreEqual("DeliverTo" + counter, entity.DeliverTo);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Order entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.DeliverTo);
                    break;
                case ARTAction.Restore:
                    entity.DeliverTo = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.DeliverTo;
                    entity.DeliverTo = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            OrderRepository.DbContext.BeginTransaction();
            LoadOrderTypes();
            LoadOrganizations(3);
            LoadWorkgroups(3);
            LoadWorkgroupVendors(3);
            LoadWorkgroupAddress(3);
            LoadOrderStatusCodes();
            LoadUsers(3);
            OrderRepository.DbContext.CommitTransaction();

            OrderRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            OrderRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region OrderType Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrdersFieldOrderTypeWithAValueOfNullDoesNotSave()
        {
            Order record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.OrderType = null;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.OrderType, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The OrderType field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderNewOrderTypeDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.OrderType = new OrderType();
                thisFar = true;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.OrderType, Entity: Purchasing.Core.Domain.OrderType", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestOrderWithExistingOrderTypeSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.OrderType = OrderTypeRepository.Queryable.Single(a => a.Id == OrderType.Types.PurchaseRequest);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(OrderType.Types.PurchaseRequest, record.OrderType.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion OrderType Tests

        #region Vendor Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrdersFieldVendorWithAValueOfNullDoesNotSave()
        {
            Order record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Vendor = null;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Vendor, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Vendor field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderNewVendorDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Vendor = new WorkgroupVendor();
                thisFar = true;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.WorkgroupVendor, Entity: Purchasing.Core.Domain.WorkgroupVendor", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestOrderWithExistingVendorSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Vendor = Repository.OfType<WorkgroupVendor>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Vendor.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Vendor Tests

        #region Address Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrdersFieldAddressWithAValueOfNullDoesNotSave()
        {
            Order record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Address = null;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Address, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Address field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderNewAddressDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Address = new WorkgroupAddress();
                thisFar = true;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.WorkgroupAddress, Entity: Purchasing.Core.Domain.WorkgroupAddress", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestOrderWithExistingAddressSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Address = Repository.OfType<WorkgroupAddress>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Address.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Address Tests

        #region ShippingType Tests


        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderNewShippingTypeDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.ShippingType = new ShippingType();
                thisFar = true;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.ShippingType, Entity: Purchasing.Core.Domain.ShippingType", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestOrderWithExistingShippingTypeSaves()
        {
            var shippingTypeRepository = new RepositoryWithTypedId<ShippingType, string>();
            LoadShippingTypes(3);
            #region Arrange
            var record = GetValid(9);
            record.ShippingType = shippingTypeRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.ShippingType.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestOrderWithNullShippingTypeSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.ShippingType = null;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.ShippingType);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion ShippingType Tests

        #region DeliverTo Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the DeliverTo with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDeliverToWithNullValueDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.DeliverTo = null;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(order);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(order);
                var results = order.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "DeliverTo"));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the DeliverTo with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDeliverToWithEmptyStringDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.DeliverTo = string.Empty;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(order);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(order);
                var results = order.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "DeliverTo"));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the DeliverTo with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDeliverToWithSpacesOnlyDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.DeliverTo = " ";
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(order);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(order);
                var results = order.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "DeliverTo"));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the DeliverTo with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDeliverToWithTooLongValueDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.DeliverTo = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(order);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(order);
                Assert.AreEqual(50 + 1, order.DeliverTo.Length);
                var results = order.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "DeliverTo", "50"));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the DeliverTo with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDeliverToWithOneCharacterSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.DeliverTo = "x";
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DeliverTo with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDeliverToWithLongValueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.DeliverTo = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, order.DeliverTo.Length);
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion DeliverTo Tests

        #region DeliverToEmail Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the DeliverToEmail with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDeliverToEmailWithTooLongValueDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.DeliverToEmail = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(order);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(order);
                Assert.AreEqual(50 + 1, order.DeliverToEmail.Length);
                var results = order.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "DeliverToEmail", "50"));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the DeliverToEmail with null value saves.
        /// </summary>
        [TestMethod]
        public void TestDeliverToEmailWithNullValueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.DeliverToEmail = null;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DeliverToEmail with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestDeliverToEmailWithEmptyStringSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.DeliverToEmail = string.Empty;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DeliverToEmail with one space saves.
        /// </summary>
        [TestMethod]
        public void TestDeliverToEmailWithOneSpaceSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.DeliverToEmail = " ";
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DeliverToEmail with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDeliverToEmailWithOneCharacterSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.DeliverToEmail = "x";
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DeliverToEmail with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDeliverToEmailWithLongValueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.DeliverToEmail = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, order.DeliverToEmail.Length);
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion DeliverToEmail Tests

        #region DateNeeded Tests

        /// <summary>
        /// Tests the DateNeeded with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateNeededWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Order record = GetValid(99);
            record.DateNeeded = compareDate;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateNeeded);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the DateNeeded with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateNeededWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.DateNeeded = compareDate;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateNeeded);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateNeeded with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateNeededWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.DateNeeded = compareDate;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateNeeded);
            #endregion Assert
        }

        [TestMethod]
        public void TestDateNeededWithNullDateDateWillSave()
        {
            #region Arrange
            var record = GetValid(99);
            record.DateNeeded = null;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(null, record.DateNeeded);
            #endregion Assert
        }
        #endregion DateNeeded Tests

        #region AllowBackorder Tests

        /// <summary>
        /// Tests the AllowBackorder is false saves.
        /// </summary>
        [TestMethod]
        public void TestAllowBackorderIsFalseSaves()
        {
            #region Arrange
            Order order = GetValid(9);
            order.AllowBackorder = false;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.AllowBackorder);
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AllowBackorder is true saves.
        /// </summary>
        [TestMethod]
        public void TestAllowBackorderIsTrueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.AllowBackorder = true;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(order.AllowBackorder);
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        #endregion AllowBackorder Tests

        #region EstimatedTax Tests

        [TestMethod]
        public void TestEstimatedTaxWithZeroUnitPriceSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.EstimatedTax = 0;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.EstimatedTax);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestEstimatedTaxWithUnitPriceSaves1()
        {
            #region Arrange
            var record = GetValid(9);
            record.EstimatedTax = 0.001m;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.001m, record.EstimatedTax);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestEstimatedTaxWithUnitPriceSaves2()
        {
            #region Arrange
            var record = GetValid(9);
            record.EstimatedTax = 999999999.999m;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999999999.999m, record.EstimatedTax);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion EstimatedTax Tests

        #region Workgroup Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrdersFieldWorkgroupWithAValueOfNullDoesNotSave()
        {
            Order record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Workgroup = null;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
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
        public void TestOrderNewWorkgroupDoesNotSave()
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
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.Workgroup, Entity: Purchasing.Core.Domain.Workgroup", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestOrderWithExistingWorkgroupSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Workgroup = Repository.OfType<Workgroup>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Workgroup.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Workgroup Tests

        #region Organization Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrdersFieldOrganizationWithAValueOfNullDoesNotSave()
        {
            Order record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Organization = null;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Organization, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Organization field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderNewOrganizationDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Organization = new Organization();
                thisFar = true;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.Organization, Entity: Purchasing.Core.Domain.Organization", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestOrderWithExistingOrganizationSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Organization = OrganizationRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.Organization.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }
        #endregion Organization Tests

        #region PoNumber Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the PoNumber with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPoNumberWithTooLongValueDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.PoNumber = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(order);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(order);
                Assert.AreEqual(50 + 1, order.PoNumber.Length);
                var results = order.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "PoNumber", "50"));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the PoNumber with null value saves.
        /// </summary>
        [TestMethod]
        public void TestPoNumberWithNullValueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.PoNumber = null;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PoNumber with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestPoNumberWithEmptyStringSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.PoNumber = string.Empty;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PoNumber with one space saves.
        /// </summary>
        [TestMethod]
        public void TestPoNumberWithOneSpaceSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.PoNumber = " ";
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PoNumber with one character saves.
        /// </summary>
        [TestMethod]
        public void TestPoNumberWithOneCharacterSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.PoNumber = "x";
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PoNumber with long value saves.
        /// </summary>
        [TestMethod]
        public void TestPoNumberWithLongValueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.PoNumber = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, order.PoNumber.Length);
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion PoNumber Tests

        #region LastCompletedApproval Tests

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderNewLastCompletedApprovalDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.LastCompletedApproval = new Approval();
                thisFar = true;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.Approval, Entity: Purchasing.Core.Domain.Approval", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestOrderWithExistingLastCompletedApprovalSaves()
        {
            #region Arrange
            Repository.OfType<Approval>().DbContext.BeginTransaction();
            LoadApprovals(3);
            Repository.OfType<Approval>().DbContext.CommitTransaction();
            var record = GetValid(9);
            record.LastCompletedApproval = Repository.OfType<Approval>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.LastCompletedApproval.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        //Delete this one if not nullable
        [TestMethod]
        public void TestOrderWithNullLastCompletedApprovalSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.LastCompletedApproval = null;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.LastCompletedApproval);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion LastCompletedApproval Tests

        #region ShippingAmount Tests

        [TestMethod]
        public void TestShippingAmountWithZeroUnitPriceSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.ShippingAmount = 0;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.ShippingAmount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestShippingAmountWithUnitPriceSaves1()
        {
            #region Arrange
            var record = GetValid(9);
            record.ShippingAmount = 0.001m;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.001m, record.ShippingAmount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestShippingAmountWithUnitPriceSaves2()
        {
            #region Arrange
            var record = GetValid(9);
            record.ShippingAmount = 999999999.999m;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999999999.999m, record.ShippingAmount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion ShippingAmount Tests

        #region Justification Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Justification with null value saves.
        /// </summary>
        [TestMethod]
        public void TestJustificationWithNullValueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.Justification = null;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Justification with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestJustificationWithEmptyStringSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.Justification = string.Empty;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Justification with one space saves.
        /// </summary>
        [TestMethod]
        public void TestJustificationWithOneSpaceSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.Justification = " ";
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Justification with one character saves.
        /// </summary>
        [TestMethod]
        public void TestJustificationWithOneCharacterSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.Justification = "x";
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Justification with long value saves.
        /// </summary>
        [TestMethod]
        public void TestJustificationWithLongValueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.Justification = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, order.Justification.Length);
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Justification Tests

        #region StatusCode Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrdersFieldStatusCodeWithAValueOfNullDoesNotSave()
        {
            Order record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.StatusCode = null;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
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
        public void TestOrderNewStatusCodeDoesNotSave()
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
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.OrderStatusCode, Entity: Purchasing.Core.Domain.OrderStatusCode", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestOrderWithExistingStatusCodeSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Complete);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(OrderStatusCode.Codes.Complete, record.StatusCode.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion StatusCode Tests

        #region CreatedBy Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestOrdersFieldCreatedByWithAValueOfNullDoesNotSave()
        {
            Order record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.CreatedBy = null;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.CreatedBy, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The CreatedBy field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestOrderNewCreatedByDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.CreatedBy = new User();
                thisFar = true;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(record);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestOrderWithExistingCreatedBySaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.CreatedBy = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.CreatedBy.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion CreatedBy Tests

        #region DateCreated Tests

        /// <summary>
        /// Tests the DateCreated with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateCreatedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Order record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
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
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
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
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.DateCreated = compareDate;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateCreated);
            #endregion Assert
        }
        #endregion DateCreated Tests

        #region HasControlledSubstance Tests

        /// <summary>
        /// Tests the HasControlledSubstance is false saves.
        /// </summary>
        [TestMethod]
        public void TestHasControlledSubstanceIsFalseSaves()
        {
            #region Arrange
            Order order = GetValid(9);
            order.HasControlledSubstance = false;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(order.HasControlledSubstance);
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the HasControlledSubstance is true saves.
        /// </summary>
        [TestMethod]
        public void TestHasControlledSubstanceIsTrueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.HasControlledSubstance = true;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(order.HasControlledSubstance);
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        #endregion HasControlledSubstance Tests


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
            expectedFields.Add(new NameAndType("Address", "Purchasing.Core.Domain.WorkgroupAddress", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("AllowBackorder", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("CreatedBy", "Purchasing.Core.Domain.User", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("DateCreated", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("DateNeeded", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("DeliverTo", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("DeliverToEmail", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("EstimatedTax", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("HasControlledSubstance", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
                                                                         {
                                                                             "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                                                                             "[System.Xml.Serialization.XmlIgnoreAttribute()]"
                                                                         }));
            expectedFields.Add(new NameAndType("Justification", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("LastCompletedApproval", "Purchasing.Core.Domain.Approval", new List<string>()));
            expectedFields.Add(new NameAndType("OrderType", "Purchasing.Core.Domain.OrderType", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Organization", "Purchasing.Core.Domain.Organization", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("PoNumber", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("ShippingAmount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("ShippingType", "Purchasing.Core.Domain.ShippingType", new List<string>()));
            expectedFields.Add(new NameAndType("StatusCode", "Purchasing.Core.Domain.OrderStatusCode", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Vendor", "Purchasing.Core.Domain.WorkgroupVendor", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Workgroup", "Purchasing.Core.Domain.Workgroup", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Order));

        }

        #endregion Reflection of Database.	
		
		
    }
}