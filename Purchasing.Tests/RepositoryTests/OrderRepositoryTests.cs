using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
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
            LoadCustomField(3);
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
        public void TestOrderWithNullVendorSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Vendor = null;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Vendor);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
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
                order.DeliverToEmail = string.Format("x{0}@x.com", "x".RepeatTimes(44));
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

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDeliverToEmailWithOnlySpaceDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.DeliverToEmail = " ";
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(order);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception)
            {
                Assert.IsNotNull(order);
                var results = order.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The DeliverToEmail field is not a valid e-mail address."));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDeliverToEmailWithEmptyStringDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.DeliverToEmail = string.Empty;
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(order);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception)
            {
                Assert.IsNotNull(order);
                var results = order.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The DeliverToEmail field is not a valid e-mail address."));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDeliverToEmailWithInvalidEmailDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.DeliverToEmail = "x@@x.com";
                #endregion Arrange

                #region Act
                OrderRepository.DbContext.BeginTransaction();
                OrderRepository.EnsurePersistent(order);
                OrderRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception)
            {
                Assert.IsNotNull(order);
                var results = order.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The DeliverToEmail field is not a valid e-mail address."));
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


        [TestMethod]
        public void TestDeliverToEmailWithFewCharactersSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.DeliverToEmail = "x@x.x";
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
            order.DeliverToEmail = string.Format("x{0}@x.com", "x".RepeatTimes(43));
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


        #region FreightAmount Tests

        [TestMethod]
        public void TestFreightAmountWithZeroUnitPriceSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.FreightAmount = 0;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.FreightAmount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestFreightAmountWithUnitPriceSaves1()
        {
            #region Arrange
            var record = GetValid(9);
            record.FreightAmount = 0.001m;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.001m, record.FreightAmount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestFreightAmountWithUnitPriceSaves2()
        {
            #region Arrange
            var record = GetValid(9);
            record.FreightAmount = 999999999.999m;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999999999.999m, record.FreightAmount);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion FreightAmount Tests

        #region Justification Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Justification with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestJustificationWithNullValueDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.Justification = null;
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
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Justification"));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Justification with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestJustificationWithEmptyStringDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.Justification = string.Empty;
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
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Justification"));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Justification with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestJustificationWithSpacesOnlyDoesNotSave()
        {
            Order order = null;
            try
            {
                #region Arrange
                order = GetValid(9);
                order.Justification = " ";
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
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Justification"));
                Assert.IsTrue(order.IsTransient());
                Assert.IsFalse(order.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

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

        #region DateOrdered Tests

        /// <summary>
        /// Tests the DateOrdered with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateOrderedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Order record = GetValid(99);
            var orderTracking = new OrderTracking();
            orderTracking.StatusCode = new OrderStatusCode();
            orderTracking.StatusCode.SetIdTo(OrderStatusCode.Codes.Purchaser);
            orderTracking.DateCreated = compareDate;
            record.AddTracking(orderTracking);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateOrdered);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the DateOrdered with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateOrderedWithNullDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Order record = GetValid(99);
            var orderTracking = new OrderTracking();
            orderTracking.StatusCode = new OrderStatusCode();
            orderTracking.StatusCode.SetIdTo(OrderStatusCode.Codes.AccountManager); //Not Purchaser
            orderTracking.DateCreated = compareDate;
            record.AddTracking(orderTracking);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(null, record.DateOrdered);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateOrdered with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateOrderedWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            var orderTracking = new OrderTracking();
            orderTracking.StatusCode = new OrderStatusCode();
            orderTracking.StatusCode.SetIdTo(OrderStatusCode.Codes.Purchaser);
            orderTracking.DateCreated = compareDate;
            record.AddTracking(orderTracking);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateOrdered);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateOrdered with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateOrderedWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            var orderTracking = new OrderTracking();
            orderTracking.StatusCode = new OrderStatusCode();
            orderTracking.StatusCode.SetIdTo(OrderStatusCode.Codes.Purchaser);
            orderTracking.DateCreated = compareDate;
            record.AddTracking(orderTracking);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateOrdered);
            #endregion Assert
        }
        #endregion DateOrdered Tests

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

        #region IList Tests
        #region Attachments Tests

        #region Valid Tests
        [TestMethod]
        public void TestAttachmentsWithPopulatedListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAttachment(CreateValidEntities.Attachment(i+1));
                record.Attachments[i].User = UserRepository.Queryable.First();
            }
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Attachments);
            Assert.AreEqual(addedCount, record.Attachments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAttachmentsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Attachment>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Attachment(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                Repository.OfType<Attachment>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Attachments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Attachments);
            Assert.AreEqual(addedCount, record.Attachments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestAttachmentsWithEmptyListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Attachments);
            Assert.AreEqual(0, record.Attachments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestOrderCascadesSaveToAttachment()
        {
            #region Arrange
            var count = Repository.OfType<Attachment>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddAttachment(CreateValidEntities.Attachment(i+1));
                record.Attachments[i].User = UserRepository.Queryable.First();
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.Attachments.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<Attachment>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesUpdateToAttachment1()
        {
            #region Arrange
            var attachmentRepository = new RepositoryWithTypedId<Attachment, Guid>();
            var count = Repository.OfType<Attachment>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for(int i = 0; i < addedCount; i++)
            {
                record.AddAttachment(CreateValidEntities.Attachment(i + 1));
                record.Attachments[i].User = UserRepository.Queryable.First();
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Attachments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.Attachments[1].FileName = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Attachment>().Queryable.Count());
            var relatedRecord = attachmentRepository.GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.FileName);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesUpdateToAttachment2()
        {
            #region Arrange
            var attachmentRepository = new RepositoryWithTypedId<Attachment, Guid>();
            var count = attachmentRepository.Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Attachment>();
            for(int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Attachment(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                attachmentRepository.EnsurePersistent(relatedRecords[i]);
            }
            foreach(var relatedRecord in relatedRecords)
            {
                record.Attachments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Attachments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach(var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.Attachments[1].FileName = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach(var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, attachmentRepository.Queryable.Count());
            var relatedRecord2 = attachmentRepository.GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.FileName);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it (Delete this test, or the one below)
        /// </summary>
        [TestMethod]
        public void TestOrderCascadesUpdateRemoveAttachment()
        {
            #region Arrange
            var attachmentRepository = new RepositoryWithTypedId<Attachment, Guid>();
            var count = attachmentRepository.Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Attachment>();
            for(int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Attachment(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                attachmentRepository.EnsurePersistent(relatedRecords[i]);
            }
            foreach(var relatedRecord in relatedRecords)
            {
                record.Attachments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Attachments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.Attachments.RemoveAt(1);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount - 1), attachmentRepository.Queryable.Count());
            var relatedRecord2 = attachmentRepository.GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesDeleteToAttachment()
        {
            #region Arrange
            var attachmentRepository = new RepositoryWithTypedId<Attachment, Guid>();
            var count = attachmentRepository.Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Attachment>();
            for(int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Attachment(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                attachmentRepository.EnsurePersistent(relatedRecords[i]);
            }
            foreach(var relatedRecord in relatedRecords)
            {
                record.Attachments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Attachments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.Remove(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, attachmentRepository.Queryable.Count());
            var relatedRecord2 = attachmentRepository.GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		
        #endregion Cascade Tests

        #endregion Attachments Tests

        #region LineItems Tests
        #region Valid Tests
        [TestMethod]
        public void TestLineItemsWithPopulatedListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddLineItem(CreateValidEntities.LineItem(i+1));
            }
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.LineItems);
            Assert.AreEqual(addedCount, record.LineItems.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestLineItemsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<LineItem>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.LineItem(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<LineItem>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.LineItems.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.LineItems);
            Assert.AreEqual(addedCount, record.LineItems.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestLineItemsWithEmptyListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.LineItems);
            Assert.AreEqual(0, record.LineItems.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestOrderCascadesSaveToLineItem()
        {
            #region Arrange
            var count = Repository.OfType<LineItem>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddLineItem(CreateValidEntities.LineItem(i + 1));
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.LineItems.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<LineItem>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesUpdateToLineItem1()
        {
            #region Arrange
            var count = Repository.OfType<LineItem>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddLineItem(CreateValidEntities.LineItem(i + 1));
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.LineItems[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.LineItems[1].Unit = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<LineItem>().Queryable.Count());
            var relatedRecord = Repository.OfType<LineItem>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Unit);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesUpdateToLineItem2()
        {
            #region Arrange
            var count = Repository.OfType<LineItem>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<LineItem>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.LineItem(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<LineItem>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.LineItems.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.LineItems[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.LineItems[1].Unit = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<LineItem>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<LineItem>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Unit);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it (Delete this test, or the one below)
        /// </summary>
        [TestMethod]
        public void TestOrderCascadesUpdateRemoveLineItem()
        {
            #region Arrange
            var count = Repository.OfType<LineItem>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<LineItem>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.LineItem(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<LineItem>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.LineItems.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.LineItems[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.LineItems.RemoveAt(1);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<LineItem>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<LineItem>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesDeleteToLineItem()
        {
            #region Arrange
            var count = Repository.OfType<LineItem>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<LineItem>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.LineItem(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<LineItem>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.LineItems.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.LineItems[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.Remove(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<LineItem>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<LineItem>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests
        #endregion LineItems Tests

        #region Approvals Tests

        #region Valid Tests
        [TestMethod]
        public void TestApprovalsWithPopulatedListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddApproval(CreateValidEntities.Approval(i+1));
                record.Approvals[i].User = null;
                record.Approvals[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
            }
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Approvals);
            Assert.AreEqual(addedCount, record.Approvals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestApprovalsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Approval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Approval(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = null;
                relatedRecords[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
                Repository.OfType<Approval>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Approvals.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
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
            Order record = GetValid(9);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
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


        [TestMethod]
        public void TestOrderCascadesSaveToApproval()
        {
            #region Arrange
            var count = Repository.OfType<Approval>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddApproval(CreateValidEntities.Approval(i+1));
                record.Approvals[i].User = null;
                record.Approvals[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.Approvals.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<Approval>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesUpdateToApproval1()
        {
            #region Arrange
            var count = Repository.OfType<Approval>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddApproval(CreateValidEntities.Approval(i + 1));
                record.Approvals[i].User = null;
                record.Approvals[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Approvals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.Approvals[1].SecondaryUser = record.Approvals[1].SecondaryUser = UserRepository.Queryable.Single(a => a.Id == "3");
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Approval>().Queryable.Count());
            var relatedRecord = Repository.OfType<Approval>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("3", relatedRecord.SecondaryUser.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesUpdateToApproval2()
        {
            #region Arrange
            var count = Repository.OfType<Approval>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Approval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Approval(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = null;
                relatedRecords[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
                Repository.OfType<Approval>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Approvals.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Approvals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.Approvals[1].SecondaryUser = UserRepository.Queryable.Single(a => a.Id == "3");
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Approval>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Approval>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("3", relatedRecord2.SecondaryUser.Id);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it
        /// </summary>
        [TestMethod]
        public void TestOrderCascadesUpdateRemoveApproval()
        {
            #region Arrange
            var count = Repository.OfType<Approval>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Approval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Approval(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = null;
                relatedRecords[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
                Repository.OfType<Approval>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Approvals.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Approvals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.Approvals.RemoveAt(1);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<Approval>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Approval>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesDeleteToApproval()
        {
            #region Arrange
            var count = Repository.OfType<Approval>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Approval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Approval(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = null;
                relatedRecords[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
                Repository.OfType<Approval>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Approvals.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Approvals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.Remove(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<Approval>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Approval>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion Approvals Tests

        #region Splits Tests

        #region Valid Tests
        [TestMethod]
        public void TestSplitsWithPopulatedListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddSplit(CreateValidEntities.Split(i+1));
            }
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Splits);
            Assert.AreEqual(addedCount, record.Splits.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestSplitsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Split>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Split(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<Split>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Splits.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Splits);
            Assert.AreEqual(addedCount, record.Splits.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestSplitsWithEmptyListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Splits);
            Assert.AreEqual(0, record.Splits.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestOrderCascadesSaveToSplit()
        {
            #region Arrange
            var count = Repository.OfType<Split>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddSplit(CreateValidEntities.Split(i+1));
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.Splits.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<Split>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesUpdateToSplit1()
        {
            #region Arrange
            var count = Repository.OfType<Split>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddSplit(CreateValidEntities.Split(i+1));
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Splits[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.Splits[1].Project = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Split>().Queryable.Count());
            var relatedRecord = Repository.OfType<Split>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Project);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesUpdateToSplit2()
        {
            #region Arrange
            var count = Repository.OfType<Split>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Split>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Split(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<Split>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Splits.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Splits[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.Splits[1].Project = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Split>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Split>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Project);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it 
        /// </summary>
        [TestMethod]
        public void TestOrderCascadesUpdateRemoveSplit()
        {
            #region Arrange
            var count = Repository.OfType<Split>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Split>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Split(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<Split>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Splits.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Splits[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.Splits.RemoveAt(1);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<Split>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Split>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesDeleteToSplit()
        {
            #region Arrange
            var count = Repository.OfType<Split>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Split>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Split(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<Split>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Splits.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Splits[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.Remove(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<Split>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Split>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion Splits Tests

        #region OrderTrackings Tests
        #region Valid Tests
        [TestMethod]
        public void TestOrderTrackingsWithPopulatedListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddTracking(CreateValidEntities.OrderTracking(i+1));
                record.OrderTrackings[i].User = UserRepository.Queryable.First();
                record.OrderTrackings[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
            }
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.OrderTrackings);
            Assert.AreEqual(addedCount, record.OrderTrackings.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderTrackingsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<OrderTracking>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.OrderTracking(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                relatedRecords[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
                Repository.OfType<OrderTracking>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.OrderTrackings.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.OrderTrackings);
            Assert.AreEqual(addedCount, record.OrderTrackings.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderTrackingsWithEmptyListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.OrderTrackings);
            Assert.AreEqual(0, record.OrderTrackings.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestOrderCascadesSaveToOrderTracking()
        {
            #region Arrange
            var count = Repository.OfType<OrderTracking>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddTracking(CreateValidEntities.OrderTracking(i + 1));
                record.OrderTrackings[i].User = UserRepository.Queryable.First();
                record.OrderTrackings[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.OrderTrackings.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<OrderTracking>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesUpdateToOrderTracking1()
        {
            #region Arrange
            var count = Repository.OfType<OrderTracking>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddTracking(CreateValidEntities.OrderTracking(i + 1));
                record.OrderTrackings[i].User = UserRepository.Queryable.First();
                record.OrderTrackings[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.OrderTrackings[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.OrderTrackings[1].Description = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<OrderTracking>().Queryable.Count());
            var relatedRecord = Repository.OfType<OrderTracking>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Description);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesUpdateToOrderTracking2()
        {
            #region Arrange
            var count = Repository.OfType<OrderTracking>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<OrderTracking>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.OrderTracking(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                relatedRecords[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
                Repository.OfType<OrderTracking>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.OrderTrackings.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.OrderTrackings[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.OrderTrackings[1].Description = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<OrderTracking>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<OrderTracking>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Description);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it 
        /// </summary>
        [TestMethod]
        public void TestOrderCascadesUpdateRemoveOrderTracking()
        {
            #region Arrange
            var count = Repository.OfType<OrderTracking>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<OrderTracking>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.OrderTracking(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                relatedRecords[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
                Repository.OfType<OrderTracking>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.OrderTrackings.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.OrderTrackings[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.OrderTrackings.RemoveAt(1);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<OrderTracking>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<OrderTracking>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesDeleteToOrderTracking()
        {
            #region Arrange
            var count = Repository.OfType<OrderTracking>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<OrderTracking>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.OrderTracking(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                relatedRecords[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
                Repository.OfType<OrderTracking>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.OrderTrackings.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.OrderTrackings[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.Remove(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<OrderTracking>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<OrderTracking>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion OrderTrackings Tests

        #region KfsDocuments Tests

        #region Valid Tests
        [TestMethod]
        public void TestKfsDocumentsWithPopulatedListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddKfsDocument(CreateValidEntities.KfsDocument(i+1));
            }
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.KfsDocuments);
            Assert.AreEqual(addedCount, record.KfsDocuments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestKfsDocumentsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<KfsDocument>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.KfsDocument(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<KfsDocument>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.KfsDocuments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.KfsDocuments);
            Assert.AreEqual(addedCount, record.KfsDocuments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestKfsDocumentsWithEmptyListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.KfsDocuments);
            Assert.AreEqual(0, record.KfsDocuments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestOrderCascadesSaveToKfsDocument()
        {
            #region Arrange
            var count = Repository.OfType<KfsDocument>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddKfsDocument(CreateValidEntities.KfsDocument(i + 1));
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.KfsDocuments.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<KfsDocument>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesUpdateToKfsDocument1()
        {
            #region Arrange
            var count = Repository.OfType<KfsDocument>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddKfsDocument(CreateValidEntities.KfsDocument(i + 1));
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.KfsDocuments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.KfsDocuments[1].DocNumber = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<KfsDocument>().Queryable.Count());
            var relatedRecord = Repository.OfType<KfsDocument>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.DocNumber);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesUpdateToKfsDocument2()
        {
            #region Arrange
            var count = Repository.OfType<KfsDocument>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<KfsDocument>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.KfsDocument(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<KfsDocument>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.KfsDocuments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.KfsDocuments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.KfsDocuments[1].DocNumber = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<KfsDocument>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<KfsDocument>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.DocNumber);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it (Delete this test, or the one below)
        /// </summary>
        [TestMethod]
        public void TestOrderCascadesUpdateRemoveKfsDocument()
        {
            #region Arrange
            var count = Repository.OfType<KfsDocument>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<KfsDocument>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.KfsDocument(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<KfsDocument>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.KfsDocuments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.KfsDocuments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.KfsDocuments.RemoveAt(1);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<KfsDocument>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<KfsDocument>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesDeleteToKfsDocument()
        {
            #region Arrange
            var count = Repository.OfType<KfsDocument>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<KfsDocument>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.KfsDocument(i + 1));
                relatedRecords[i].Order = record;
                Repository.OfType<KfsDocument>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.KfsDocuments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.KfsDocuments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.Remove(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<KfsDocument>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<KfsDocument>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion KfsDocuments Tests

        #region OrderComments Tests
        #region Valid Tests
        [TestMethod]
        public void TestOrderCommentsWithPopulatedListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddOrderComment(CreateValidEntities.OrderComment(i+1));
                record.OrderComments[i].User = UserRepository.Queryable.First();
            }
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.OrderComments);
            Assert.AreEqual(addedCount, record.OrderComments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCommentsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<OrderComment>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.OrderComment(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                Repository.OfType<OrderComment>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.OrderComments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.OrderComments);
            Assert.AreEqual(addedCount, record.OrderComments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCommentsWithEmptyListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.OrderComments);
            Assert.AreEqual(0, record.OrderComments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestOrderCascadesSaveToOrderComment()
        {
            #region Arrange
            var count = Repository.OfType<OrderComment>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddOrderComment(CreateValidEntities.OrderComment(i+1));
                record.OrderComments[i].User = UserRepository.Queryable.First();
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.OrderComments.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<OrderComment>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesUpdateToOrderComment1()
        {
            #region Arrange
            var count = Repository.OfType<OrderComment>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddOrderComment(CreateValidEntities.OrderComment(i + 1));
                record.OrderComments[i].User = UserRepository.Queryable.First();
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.OrderComments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.OrderComments[1].Text = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<OrderComment>().Queryable.Count());
            var relatedRecord = Repository.OfType<OrderComment>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Text);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesUpdateToOrderComment2()
        {
            #region Arrange
            var count = Repository.OfType<OrderComment>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<OrderComment>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.OrderComment(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                Repository.OfType<OrderComment>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.OrderComments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.OrderComments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.OrderComments[1].Text = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<OrderComment>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<OrderComment>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Text);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it (Delete this test, or the one below)
        /// </summary>
        [TestMethod]
        public void TestOrderCascadesUpdateRemoveOrderComment()
        {
            #region Arrange
            var count = Repository.OfType<OrderComment>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<OrderComment>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.OrderComment(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                Repository.OfType<OrderComment>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.OrderComments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.OrderComments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.OrderComments.RemoveAt(1);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<OrderComment>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<OrderComment>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesDeleteToOrderComment()
        {
            #region Arrange
            var count = Repository.OfType<OrderComment>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<OrderComment>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.OrderComment(i + 1));
                relatedRecords[i].Order = record;
                relatedRecords[i].User = UserRepository.Queryable.First();
                Repository.OfType<OrderComment>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.OrderComments.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.OrderComments[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.Remove(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<OrderComment>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<OrderComment>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion OrderComments Tests

        #region EmailQueues Tests
        #region Valid Tests
        [TestMethod]
        public void TestEmailQueuesWithPopulatedListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddEmailQueue(CreateValidEntities.EmailQueue(i+1));
            }
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.EmailQueues);
            Assert.AreEqual(addedCount, record.EmailQueues.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailQueuesWithPopulatedExistingListWillSave()
        {
            #region Arrange
            var emailQueueRepository = new RepositoryWithTypedId<EmailQueue, Guid>();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<EmailQueue>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailQueue(i + 1));
                relatedRecords[i].Order = record;
                emailQueueRepository.EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.EmailQueues.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.EmailQueues);
            Assert.AreEqual(addedCount, record.EmailQueues.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailQueuesWithEmptyListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.EmailQueues);
            Assert.AreEqual(0, record.EmailQueues.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestOrderCascadesSaveToEmailQueue()
        {
            #region Arrange
            var emailQueueRepository = new RepositoryWithTypedId<EmailQueue, Guid>();
            var count = emailQueueRepository.Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddEmailQueue(CreateValidEntities.EmailQueue(i + 1));
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.EmailQueues.Count);
            Assert.AreEqual(count + addedCount, emailQueueRepository.Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesUpdateToEmailQueue1()
        {
            #region Arrange
            var emailQueueRepository = new RepositoryWithTypedId<EmailQueue, Guid>();
            var count = emailQueueRepository.Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddEmailQueue(CreateValidEntities.EmailQueue(i + 1));
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.EmailQueues[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.EmailQueues[1].Text = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, emailQueueRepository.Queryable.Count());
            var relatedRecord = emailQueueRepository.GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Text);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesUpdateToEmailQueue2()
        {
            #region Arrange
            var emailQueueRepository = new RepositoryWithTypedId<EmailQueue, Guid>();
            var count = emailQueueRepository.Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<EmailQueue>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailQueue(i + 1));
                relatedRecords[i].Order = record;
                emailQueueRepository.EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.EmailQueues.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.EmailQueues[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.EmailQueues[1].Text = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, emailQueueRepository.Queryable.Count());
            var relatedRecord2 = emailQueueRepository.GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Text);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it (Delete this test, or the one below)
        /// </summary>
        [TestMethod]
        public void TestOrderCascadesUpdateRemoveEmailQueue()
        {
            #region Arrange
            var emailQueueRepository = new RepositoryWithTypedId<EmailQueue, Guid>();
            var count = emailQueueRepository.Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<EmailQueue>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailQueue(i + 1));
                relatedRecords[i].Order = record;
                emailQueueRepository.EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.EmailQueues.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.EmailQueues[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.EmailQueues.RemoveAt(1);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), emailQueueRepository.Queryable.Count());
            var relatedRecord2 = emailQueueRepository.GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesDeleteToEmailQueue()
        {
            #region Arrange
            var emailQueueRepository = new RepositoryWithTypedId<EmailQueue, Guid>();
            var count = emailQueueRepository.Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<EmailQueue>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailQueue(i + 1));
                relatedRecords[i].Order = record;
                emailQueueRepository.EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.EmailQueues.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.EmailQueues[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.Remove(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, emailQueueRepository.Queryable.Count());
            var relatedRecord2 = emailQueueRepository.GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion EmailQueues Tests

        #region ControlledSubstances Tests


        [TestMethod]
        public void TestControlledSubstances1()
        {
            #region Arrange
            var record = GetValid(9);
            record.SetAuthorizationInfo(CreateValidEntities.ControlledSubstanceInformation(1));
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.IsTrue(record.HasControlledSubstance);
            Assert.AreEqual("Custodian1", record.GetAuthorizationInfo().Custodian);
            #endregion Assert		
        }

        [TestMethod]
        public void TestControlledSubstances2()
        {
            #region Arrange
            var record = GetValid(9);
            record.SetAuthorizationInfo(CreateValidEntities.ControlledSubstanceInformation(1));
            record.SetAuthorizationInfo(CreateValidEntities.ControlledSubstanceInformation(2));
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.IsTrue(record.HasControlledSubstance);
            Assert.AreEqual("Custodian2", record.GetAuthorizationInfo().Custodian);
            #endregion Assert
        }

        [TestMethod]
        public void TestControlledSubstances3()
        {
            #region Arrange
            var record = GetValid(9);
            record.SetAuthorizationInfo(CreateValidEntities.ControlledSubstanceInformation(1));
            record.SetAuthorizationInfo(CreateValidEntities.ControlledSubstanceInformation(2));
            record.ClearAuthorizationInfo();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.IsFalse(record.HasControlledSubstance);
            Assert.AreEqual(null, record.GetAuthorizationInfo());
            #endregion Assert
        }
        #endregion ControlledSubstances Tests


        #endregion IList Tests

        #region HasLineSplits Tests

        /// <summary>
        /// Tests the HasLineSplits is false saves.
        /// </summary>
        [TestMethod]
        public void TestHasLineSplitsIsFalseSaves()
        {
            #region Arrange
            Order order = GetValid(9);
            order.AddSplit(new Split());
            order.Splits[0].LineItem = null;
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.IsFalse(order.HasLineSplits);
            #endregion Assert
        }

        /// <summary>
        /// Tests the HasLineSplits is true saves.
        /// </summary>
        [TestMethod]
        public void TestHasLineSplitsIsTrueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.AddSplit(new Split());
            order.Splits[0].LineItem = new LineItem();
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert
            Assert.IsTrue(order.HasLineSplits);

            #endregion Assert
        }

        #endregion HasLineSplits Tests

        #region DaysNotActedOn Tests


        [TestMethod]
        public void TestDaysNotActedOn1()
        {
            #region Arrange
            var record = GetValid(9);
            record.AddTracking(CreateValidEntities.OrderTracking(1));
            record.OrderTrackings[0].StatusCode.IsComplete = true;
            record.OrderTrackings[0].DateCreated = DateTime.Now.AddDays(-10);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.DaysNotActedOn);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDaysNotActedOn2()
        {
            #region Arrange
            var record = GetValid(9);
            record.AddTracking(CreateValidEntities.OrderTracking(1));
            record.AddTracking(CreateValidEntities.OrderTracking(2));
            record.AddTracking(CreateValidEntities.OrderTracking(3));
            record.OrderTrackings[2].StatusCode.IsComplete = true;
            record.OrderTrackings[2].DateCreated = DateTime.Now.AddDays(-10);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.DaysNotActedOn);
            #endregion Assert
        }

        [TestMethod]
        public void TestDaysNotActedOn3()
        {
            #region Arrange
            var record = GetValid(9);
            record.AddTracking(CreateValidEntities.OrderTracking(1));
            record.AddTracking(CreateValidEntities.OrderTracking(2));
            record.AddTracking(CreateValidEntities.OrderTracking(3));
            record.OrderTrackings[2].DateCreated = DateTime.Now.AddDays(-10);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.DaysNotActedOn);
            #endregion Assert
        }

        [TestMethod]
        public void TestDaysNotActedOn4()
        {
            #region Arrange
            var record = GetValid(9);
            record.AddTracking(CreateValidEntities.OrderTracking(1));
            record.AddTracking(CreateValidEntities.OrderTracking(2));
            record.AddTracking(CreateValidEntities.OrderTracking(3));
            record.OrderTrackings[0].DateCreated = DateTime.Now.AddDays(-5);
            record.OrderTrackings[1].DateCreated = DateTime.Now.AddDays(-6);
            record.OrderTrackings[2].DateCreated = DateTime.Now.AddDays(-7);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual(5, record.DaysNotActedOn);
            #endregion Assert
        }

        #endregion DaysNotActedOn Tests

        #region PeoplePendingAction Tests


        [TestMethod]
        public void TestPeoplePendingAction1()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.User = null;
            approval.Completed = true;
            record.AddApproval(approval);
            approval = CreateValidEntities.Approval(2);
            approval.User = CreateValidEntities.User(100);
            approval.Completed = true;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.PeoplePendingAction);
            #endregion Assert		
        }

        [TestMethod]
        public void TestPeoplePendingAction2()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.User = null;
            approval.Completed = true;
            record.AddApproval(approval);
            approval = CreateValidEntities.Approval(2);
            approval.User = CreateValidEntities.User(100);
            approval.Completed = true;
            record.AddApproval(approval);

            approval = CreateValidEntities.Approval(3);
            approval.User = CreateValidEntities.User(101);
            approval.Completed = false;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName101 LastName101 ()", record.PeoplePendingAction);
            #endregion Assert
        }

        [TestMethod]
        public void TestPeoplePendingAction3()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.User = null;
            approval.Completed = true;
            record.AddApproval(approval);
            approval = CreateValidEntities.Approval(2);
            approval.User = CreateValidEntities.User(100);
            approval.Completed = true;
            record.AddApproval(approval);

            approval = CreateValidEntities.Approval(3);
            approval.User = CreateValidEntities.User(101);
            approval.Completed = false;
            approval.StatusCode.Level = 1;
            record.AddApproval(approval);

            approval = CreateValidEntities.Approval(4);
            approval.User = CreateValidEntities.User(102);
            approval.Completed = false;
            approval.StatusCode.Level = 2;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName101 LastName101 (), FirstName102 LastName102 ()", record.PeoplePendingAction);
            #endregion Assert
        }

        [TestMethod]
        public void TestPeoplePendingAction4()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.User = null;
            approval.Completed = true;
            record.AddApproval(approval);
            approval = CreateValidEntities.Approval(2);
            approval.User = CreateValidEntities.User(100);
            approval.Completed = true;
            record.AddApproval(approval);

            approval = CreateValidEntities.Approval(3);
            approval.User = CreateValidEntities.User(101);
            approval.Completed = false;
            approval.StatusCode.Level = 2;
            record.AddApproval(approval);

            approval = CreateValidEntities.Approval(4);
            approval.User = CreateValidEntities.User(102);
            approval.Completed = false;
            approval.StatusCode.Level = 1;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName102 LastName102 (), FirstName101 LastName101 ()", record.PeoplePendingAction);
            #endregion Assert
        }

        [TestMethod]
        public void TestPeoplePendingAction5()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.User = null;
            approval.Completed = true;
            record.AddApproval(approval);
            approval = CreateValidEntities.Approval(2);
            approval.User = CreateValidEntities.User(100);
            approval.Completed = true;
            record.AddApproval(approval);

            approval = CreateValidEntities.Approval(3);
            approval.User = CreateValidEntities.User(101);
            approval.Completed = false;
            approval.StatusCode.Level = 2;
            approval.StatusCode.SetIdTo("X1");
            record.AddApproval(approval);

            approval = CreateValidEntities.Approval(4);
            approval.User = CreateValidEntities.User(102);
            approval.Completed = false;
            approval.StatusCode.Level = 1;
            approval.StatusCode.SetIdTo("X2");
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName102 LastName102 (X2), FirstName101 LastName101 (X1)", record.PeoplePendingAction);
            #endregion Assert
        }
        #endregion PeoplePendingAction Tests

        #region AccountNumbers Tests

        [TestMethod]
        public void TestAccountNumbers1()
        {
            #region Arrange
            var record = GetValid(9);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.AccountNumbers);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAccountNumbers2()
        {
            #region Arrange
            var record = GetValid(9);
            record.AddSplit(CreateValidEntities.Split(1));
            record.Splits[0].Account = null;
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.AccountNumbers);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountNumbers3()
        {
            #region Arrange
            var record = GetValid(9);
            record.AddSplit(CreateValidEntities.Split(1));
            record.Splits[0].Account = "SomeAccount";
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("SomeAccount", record.AccountNumbers);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountNumbers4()
        {
            #region Arrange
            var record = GetValid(9);
            record.AddSplit(CreateValidEntities.Split(1));
            record.AddSplit(CreateValidEntities.Split(2));
            record.AddSplit(CreateValidEntities.Split(3));
            record.AddSplit(CreateValidEntities.Split(4));
            record.Splits[0].Account = "SomeAccount";
            record.Splits[1].Account = "SomeAccount4";
            record.Splits[2].Account = "SomeAccount2";
            record.Splits[3].Account = "SomeAccount";
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("SomeAccount, SomeAccount4, SomeAccount2", record.AccountNumbers);
            #endregion Assert
        }
        #endregion AccountNumbers Tests

        #region ApproverName Tests

        [TestMethod]
        public void TestApproverName1()
        {
            #region Arrange
            var record = GetValid(9);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("[Workgroup]", record.ApproverName);
            #endregion Assert		
        }

        [TestMethod]
        public void TestApproverName2()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            approval.User = null;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("[Workgroup]", record.ApproverName);
            #endregion Assert
        }

        [TestMethod]
        public void TestApproverName3()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = false;
            approval.SecondaryUser = null;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.ApproverName);
            #endregion Assert
        }

        [TestMethod]
        public void TestApproverName4()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = false;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = false;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.ApproverName);
            #endregion Assert
        }

        [TestMethod]
        public void TestApproverName5()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = false;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = true;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.ApproverName);
            #endregion Assert
        }


        [TestMethod]
        public void TestApproverName6()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = true;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = true;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.ApproverName);
            #endregion Assert
        }
        [TestMethod]
        public void TestApproverName7()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = true;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = true;
            record.AddApproval(approval);

            approval = CreateValidEntities.Approval(2);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            approval.User = CreateValidEntities.User(9);
            approval.User.IsActive = true;
            approval.User.IsAway = false;
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.ApproverName);
            #endregion Assert
        }

        [TestMethod]
        public void TestApproverName8()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = true;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = false;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName7 LastName7", record.ApproverName);
            #endregion Assert
        }

        [TestMethod]
        public void TestApproverName9()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = false;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = false;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName6 LastName6", record.ApproverName);
            #endregion Assert
        }
        
        #endregion ApproverName Tests

        #region AccountManagerName Tests

        [TestMethod]
        public void TestAccountManagerName1()
        {
            #region Arrange
            var record = GetValid(9);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("[Workgroup]", record.AccountManagerName);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountManagerName2()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            approval.User = null;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("[Workgroup]", record.AccountManagerName);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountManagerName3()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = false;
            approval.SecondaryUser = null;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.AccountManagerName);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountManagerName4()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = false;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = false;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.AccountManagerName);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountManagerName5()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = false;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = true;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.AccountManagerName);
            #endregion Assert
        }


        [TestMethod]
        public void TestAccountManagerName6()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = true;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = true;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.AccountManagerName);
            #endregion Assert
        }
        [TestMethod]
        public void TestAccountManagerName7()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = true;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = true;
            record.AddApproval(approval);

            approval = CreateValidEntities.Approval(2);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            approval.User = CreateValidEntities.User(9);
            approval.User.IsActive = true;
            approval.User.IsAway = false;
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.AccountManagerName);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountManagerName8()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = true;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = false;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName7 LastName7", record.AccountManagerName);
            #endregion Assert
        }

        [TestMethod]
        public void TestAccountManagerName9()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.AccountManager);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = false;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = false;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName6 LastName6", record.AccountManagerName);
            #endregion Assert
        }

        #endregion AccountManagerName Tests

        #region PurchaserName Tests

        [TestMethod]
        public void TestPurchaserName1()
        {
            #region Arrange
            var record = GetValid(9);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("[Workgroup]", record.PurchaserName);
            #endregion Assert
        }

        [TestMethod]
        public void TestPurchaserName2()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            approval.User = null;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("[Workgroup]", record.PurchaserName);
            #endregion Assert
        }

        [TestMethod]
        public void TestPurchaserName3()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = false;
            approval.SecondaryUser = null;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.PurchaserName);
            #endregion Assert
        }

        [TestMethod]
        public void TestPurchaserName4()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = false;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = false;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.PurchaserName);
            #endregion Assert
        }

        [TestMethod]
        public void TesPurchaserName5()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = false;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = true;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.PurchaserName);
            #endregion Assert
        }


        [TestMethod]
        public void TestPurchaserName6()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = true;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = true;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.PurchaserName);
            #endregion Assert
        }
        [TestMethod]
        public void TestPurchaserName7()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = true;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = true;
            record.AddApproval(approval);

            approval = CreateValidEntities.Approval(2);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
            approval.User = CreateValidEntities.User(9);
            approval.User.IsActive = true;
            approval.User.IsAway = false;
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("", record.PurchaserName);
            #endregion Assert
        }

        [TestMethod]
        public void TestPurchaserName8()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = true;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = false;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName7 LastName7", record.PurchaserName);
            #endregion Assert
        }

        [TestMethod]
        public void TestPurchaserName9()
        {
            #region Arrange
            var record = GetValid(9);
            var approval = CreateValidEntities.Approval(1);
            approval.StatusCode = OrderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Purchaser);
            approval.User = CreateValidEntities.User(6);
            approval.User.IsActive = true;
            approval.User.IsAway = false;
            approval.SecondaryUser = CreateValidEntities.User(7);
            approval.SecondaryUser.IsActive = true;
            approval.SecondaryUser.IsAway = false;
            record.AddApproval(approval);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("FirstName6 LastName6", record.PurchaserName);
            #endregion Assert
        }

        #endregion PurchaserName Tests

        #region VendorName Tests


        [TestMethod]
        public void TestVendorName1()
        {
            #region Arrange
            var record = new Order();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("-- Unspecified --", record.VendorName);
            #endregion Assert		
        }

        [TestMethod]
        public void TestVendorName2()
        {
            #region Arrange
            var record = new Order();
            record.Vendor = CreateValidEntities.WorkgroupVendor(99);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("Name99 (Line199, City99, CA 95616, US)", record.VendorName);
            #endregion Assert
        }

        #endregion VendorName Tests

        #region TotalFromDb Tests

        [TestMethod]
        public void TestTotalFromDbWithZeroUnitPriceSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.TotalFromDb = 0;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.TotalFromDb);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestTotalFromDbWithUnitPriceSaves1()
        {
            #region Arrange
            var record = GetValid(9);
            record.TotalFromDb = 0.001m;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.001m, record.TotalFromDb);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TesTotalFromDbWithUnitPriceSaves2()
        {
            #region Arrange
            var record = GetValid(9);
            record.TotalFromDb = 999999999.999m;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999999999.999m, record.TotalFromDb);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion TotalFromDb Tests

        #region CompletionReason Tests

        #region Valid Tests

        /// <summary>
        /// Tests the CompletionReason with null value saves.
        /// </summary>
        [TestMethod]
        public void TestCompletionReasonWithNullValueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.CompletionReason = null;
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
        /// Tests the CompletionReason with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestCompletionReasonWithEmptyStringSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.CompletionReason = string.Empty;
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
        /// Tests the CompletionReason with one space saves.
        /// </summary>
        [TestMethod]
        public void TestCompletionReasonWithOneSpaceSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.CompletionReason = " ";
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
        /// Tests the CompletionReason with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCompletionReasonWithOneCharacterSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.CompletionReason = "x";
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
        /// Tests the CompletionReason with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCompletionReasonWithLongValueSaves()
        {
            #region Arrange
            var order = GetValid(9);
            order.CompletionReason = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, order.CompletionReason.Length);
            Assert.IsFalse(order.IsTransient());
            Assert.IsTrue(order.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion CompletionReason Tests

        #region GrandTotalFromDb Tests


        [TestMethod]
        public void TestGrandTotalFromDbReturnsExpectedvalue()
        {
            #region Arrange
            var order = GetValid(9);
            order.LineItems = new List<LineItem>();
            order.LineItems.Add(CreateValidEntities.LineItem(1));
            order.LineItems.Add(CreateValidEntities.LineItem(2));
            order.LineItems.Add(CreateValidEntities.LineItem(3));
            order.LineItems[0].Quantity = 1;
            order.LineItems[0].UnitPrice = 10;            
            order.LineItems[1].Quantity = 2;
            order.LineItems[1].UnitPrice = 0.1m;
            order.LineItems[2].Quantity = 3;
            order.LineItems[2].UnitPrice = 0.01m;
            order.LineItems[0].Order = order;
            order.LineItems[1].Order = order;
            order.LineItems[2].Order = order;
            order.FreightAmount = 100;
            order.EstimatedTax = 15;
            order.ShippingAmount = 1000;
            var total = order.Total();
            order.TotalFromDb = total;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(order);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10.23m, total);
            Assert.AreEqual(1126.7645m, order.GrandTotal());
            #endregion Assert	
        }
        
        
        #endregion GrandTotalFromDb Tests

        #region CustomFieldAnswers Tests
        #region Invalid Tests
        [TestMethod]
        public void TestCustomFieldAnswersWithNullListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            record.CustomFieldAnswers = null;
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(record.CustomFieldAnswers);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }


        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestCustomFieldAnswersWithPopulatedListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddCustomAnswer(CreateValidEntities.CustomFieldAnswer(i + 1));
                record.CustomFieldAnswers[i].CustomField = Repository.OfType<CustomField>().Queryable.First();
            }
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CustomFieldAnswers);
            Assert.AreEqual(addedCount, record.CustomFieldAnswers.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCustomFieldAnswersWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<CustomFieldAnswer>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.CustomFieldAnswer(i + 1));
                relatedRecords[i].CustomField = Repository.OfType<CustomField>().Queryable.First();
                relatedRecords[i].Order = record;
                Repository.OfType<CustomFieldAnswer>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.CustomFieldAnswers.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CustomFieldAnswers);
            Assert.AreEqual(addedCount, record.CustomFieldAnswers.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCustomFieldAnswersWithEmptyListWillSave()
        {
            #region Arrange
            Order record = GetValid(9);
            #endregion Arrange

            #region Act
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CustomFieldAnswers);
            Assert.AreEqual(0, record.CustomFieldAnswers.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestOrderCascadesSaveToCustomFieldAnswer()
        {
            #region Arrange
            var count = Repository.OfType<CustomFieldAnswer>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddCustomAnswer(CreateValidEntities.CustomFieldAnswer(i + 1));
                record.CustomFieldAnswers[i].CustomField = Repository.OfType<CustomField>().Queryable.First();
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(addedCount, record.CustomFieldAnswers.Count);
            Assert.AreEqual(count + addedCount, Repository.OfType<CustomFieldAnswer>().Queryable.Count());
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesUpdateToCustomFieldAnswer1()
        {
            #region Arrange
            var count = Repository.OfType<CustomFieldAnswer>().Queryable.Count();
            Order record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.AddCustomAnswer(CreateValidEntities.CustomFieldAnswer(i + 1));
                record.CustomFieldAnswers[i].CustomField = Repository.OfType<CustomField>().Queryable.First();
            }

            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CustomFieldAnswers[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.CustomFieldAnswers[1].Answer = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<CustomFieldAnswer>().Queryable.Count());
            var relatedRecord = Repository.OfType<CustomFieldAnswer>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord);
            Assert.AreEqual("Updated", relatedRecord.Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrderCascadesUpdateToCustomFieldAnswer2()
        {
            #region Arrange
            var count = Repository.OfType<CustomFieldAnswer>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<CustomFieldAnswer>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.CustomFieldAnswer(i + 1));
                relatedRecords[i].CustomField = Repository.OfType<CustomField>().Queryable.First();
                relatedRecords[i].Order = record;
                Repository.OfType<CustomFieldAnswer>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.CustomFieldAnswers.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CustomFieldAnswers[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.CustomFieldAnswers[1].Answer = "Updated";
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<CustomFieldAnswer>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<CustomFieldAnswer>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Answer);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it 
        /// </summary>
        [TestMethod]
        public void TestOrderCascadesUpdateRemoveCustomFieldAnswer()
        {
            #region Arrange
            var count = Repository.OfType<CustomFieldAnswer>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<CustomFieldAnswer>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.CustomFieldAnswer(i + 1));
                relatedRecords[i].CustomField = Repository.OfType<CustomField>().Queryable.First();
                relatedRecords[i].Order = record;
                Repository.OfType<CustomFieldAnswer>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.CustomFieldAnswers.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CustomFieldAnswers[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            record.CustomFieldAnswers.RemoveAt(1);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<CustomFieldAnswer>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<CustomFieldAnswer>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }


        [TestMethod]
        public void TestOrderCascadesDeleteToCustomFieldAnswer()
        {
            #region Arrange
            var count = Repository.OfType<CustomFieldAnswer>().Queryable.Count();
            Order record = GetValid(9);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<CustomFieldAnswer>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.CustomFieldAnswer(i + 1));
                relatedRecords[i].CustomField = Repository.OfType<CustomField>().Queryable.First();
                relatedRecords[i].Order = record;
                Repository.OfType<CustomFieldAnswer>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.CustomFieldAnswers.Add(relatedRecord);
            }
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.EnsurePersistent(record);
            OrderRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CustomFieldAnswers[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrderRepository.GetNullableById(saveId);
            OrderRepository.DbContext.BeginTransaction();
            OrderRepository.Remove(record);
            OrderRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<CustomFieldAnswer>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<CustomFieldAnswer>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests

        #endregion CustomFieldAnswers Tests

        #region RequestNumber

        [TestMethod]
        public void TestRequestNumberGenerates1()
        {
            #region Arrange
            var record = CreateValidEntities.Order(9);
            record.DateCreated = new DateTime(2012, 01, 10);
            record.CreatedBy = new User("Blah");
            record.Organization.SetIdTo("TestOrg");
            #endregion Arrange

            #region Act
            record.GenerateRequestNumber();
            #endregion Act

            #region Assert
            Assert.AreEqual("TestOrg-DO74QFW", record.RequestNumber);
            #endregion Assert		
        }

        [TestMethod]
        public void TestRequestNumberGenerates2()
        {
            #region Arrange
            var record = CreateValidEntities.Order(9);
            record.DateCreated = new DateTime(2012, 01, 10);
            record.CreatedBy = new User("Blah");
            record.Organization = new Organization();
            record.Organization.SetIdTo("MyOrg");
            #endregion Arrange

            #region Act
            record.GenerateRequestNumber();
            #endregion Act

            #region Assert
            Assert.AreEqual("MyOrg-DO74QFW", record.RequestNumber);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestNumberGenerates3()
        {
            #region Arrange
            var record = CreateValidEntities.Order(9);
            record.DateCreated = new DateTime(2012, 01, 10);
            record.CreatedBy = new User("jcs");
            record.Organization = new Organization();
            record.Organization.SetIdTo("MyOrg");
            #endregion Arrange

            #region Act
            record.GenerateRequestNumber();
            #endregion Act

            #region Assert
            Assert.AreEqual("MyOrg-D2GVCE", record.RequestNumber);
            #endregion Assert
        }

        [TestMethod]
        public void TestRequestNumberGenerates4()
        {
            #region Arrange
            var record = CreateValidEntities.Order(9);
            record.DateCreated = new DateTime(2012, 01, 10).AddTicks(1);
            record.CreatedBy = new User("jcs");
            record.Organization = new Organization();
            record.Organization.SetIdTo("MyOrg");
            #endregion Arrange

            #region Act
            record.GenerateRequestNumber();
            #endregion Act

            #region Assert
            Assert.AreEqual("MyOrg-D2GVDB", record.RequestNumber);
            #endregion Assert
        }
        #endregion RequestNumber
        

        #region Constructor Tests

        [TestMethod]
        public void TestConstructor()
        {
            #region Arrange
            var record = new Order();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Attachments);
            Assert.AreEqual(0, record.Attachments.Count());
            Assert.IsNotNull(record.LineItems);
            Assert.AreEqual(0, record.LineItems.Count());
            Assert.IsNotNull(record.Approvals);
            Assert.AreEqual(0, record.Approvals.Count());
            Assert.IsNotNull(record.Splits);
            Assert.AreEqual(0, record.Splits.Count());
            Assert.IsNotNull(record.OrderTrackings);
            Assert.AreEqual(0, record.OrderTrackings.Count());
            Assert.IsNotNull(record.KfsDocuments);
            Assert.AreEqual(0, record.KfsDocuments.Count());
            Assert.IsNotNull(record.OrderComments);
            Assert.AreEqual(0, record.OrderComments.Count());
            //Assert.IsNotNull(record.ControlledSubstances);
            //Assert.AreEqual(0, record.ControlledSubstances.Count());
            Assert.IsNotNull(record.EmailQueues);
            Assert.AreEqual(0, record.EmailQueues.Count());

            Assert.AreEqual(DateTime.Now.Date, record.DateCreated.Date);
            Assert.IsFalse(record.HasControlledSubstance);
            Assert.AreEqual(7.25m, record.EstimatedTax);

            Assert.IsNull(record.RequestNumber);
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
            expectedFields.Add(new NameAndType("AccountManagerName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("AccountNumbers", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Address", "Purchasing.Core.Domain.WorkgroupAddress", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("AllowBackorder", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Approvals", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Approval]", new List<string>()));
            expectedFields.Add(new NameAndType("ApproverName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Attachments", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Attachment]", new List<string>()));
            expectedFields.Add(new NameAndType("CompletionReason", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("ControlledSubstances", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.ControlledSubstanceInformation]", new List<string>()));
            expectedFields.Add(new NameAndType("CreatedBy", "Purchasing.Core.Domain.User", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("CustomFieldAnswers", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.CustomFieldAnswer]", new List<string>()));
            expectedFields.Add(new NameAndType("DateCreated", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("DateNeeded", "System.DateTime", new List<string>
            {               
                 "[DataAnnotationsExtensions.DateAttribute()]",
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("DateOrdered", "System.Nullable`1[System.DateTime]", new List<string>()));
            expectedFields.Add(new NameAndType("DaysNotActedOn", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("DeliverTo", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("DeliverToEmail", "System.String", new List<string>
            {
                 "[DataAnnotationsExtensions.EmailAttribute()]",
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("EmailQueues", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.EmailQueue]", new List<string>()));
            expectedFields.Add(new NameAndType("EstimatedTax", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("FreightAmount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("GrandTotalFromDb", "System.Decimal", new List<string>()));       
            expectedFields.Add(new NameAndType("HasControlledSubstance", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("HasLineSplits", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Justification", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("KfsDocuments", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.KfsDocument]", new List<string>()));
            expectedFields.Add(new NameAndType("LastCompletedApproval", "Purchasing.Core.Domain.Approval", new List<string>()));
            expectedFields.Add(new NameAndType("LineItems", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.LineItem]", new List<string>()));
            expectedFields.Add(new NameAndType("OrderComments", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.OrderComment]", new List<string>()));
            expectedFields.Add(new NameAndType("OrderTrackings", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.OrderTracking]", new List<string>()));
            expectedFields.Add(new NameAndType("OrderType", "Purchasing.Core.Domain.OrderType", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Organization", "Purchasing.Core.Domain.Organization", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("PeoplePendingAction", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("PoNumber", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("PurchaserName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("RequestNumber", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("ShippingAmount", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("ShippingType", "Purchasing.Core.Domain.ShippingType", new List<string>()));
            expectedFields.Add(new NameAndType("Splits", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Split]", new List<string>()));
            expectedFields.Add(new NameAndType("StatusCode", "Purchasing.Core.Domain.OrderStatusCode", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("TotalFromDb", "System.Decimal", new List<string>()));
            expectedFields.Add(new NameAndType("Vendor", "Purchasing.Core.Domain.WorkgroupVendor", new List<string>()));
            expectedFields.Add(new NameAndType("VendorName", "System.String", new List<string>()));
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