using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            OrderRepository.DbContext.CommitTransaction();

            OrderRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            OrderRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        
        
        
        
        
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

            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
                                                                         {
                                                                             "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                                                                             "[System.Xml.Serialization.XmlIgnoreAttribute()]"
                                                                         }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Order));

        }

        #endregion Reflection of Database.	
		
		
    }
}