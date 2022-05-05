using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Moq;

using Castle.Windsor;
using CommonServiceLocator;
using UCDArch.Core.CommonValidator;
//using UCDArch.Core.NHibernateValidator.CommonValidatorAdapter;
using UCDArch.Web.IoC;
using Castle.MicroKernel.Registration;

namespace Purchasing.Tests.Core
{

    // ReSharper disable InconsistentNaming
    [TestClass]
    public abstract class AbstractRepositoryTests<T, IdT, TMap> : FluentRepositoryTestBase<TMap> where T : DomainObjectWithTypedId<IdT>
    // ReSharper restore InconsistentNaming
    {
        protected int EntriesAdded;
        protected string RestoreValue;
        protected bool BoolRestoreValue;
        protected int? IntRestoreValue;
        protected DateTime DateTimeRestoreValue;
        protected decimal DecimalRestoreValue;
        private readonly IRepository<T> _intRepository;
        private readonly IRepositoryWithTypedId<T, string> _stringRepository;
        private readonly IRepositoryWithTypedId<T, Guid> _guidRepository;

        #region Init

        protected AbstractRepositoryTests()
        {
            //HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();
            if (typeof(IdT) == typeof(int))
            {
                _intRepository = new Repository<T>();
            }
            if (typeof(IdT) == typeof(Guid))
            {
                _guidRepository = new RepositoryWithTypedId<T, Guid>();
            }
            if (typeof(IdT) == typeof(string))
            {
                _stringRepository = new RepositoryWithTypedId<T, string>();
            }
        }



        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected abstract T GetValid(int? counter);

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected abstract void FoundEntityComparison(T entity, int counter);

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected abstract void UpdateUtility(T entity, ARTAction action);

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected abstract IQueryable<T> GetQuery(int numberAtEnd);

        /// <summary>
        /// Loads the records for CRUD Tests.
        /// </summary>
        /// <returns></returns>
        protected virtual void LoadRecords(int entriesToAdd)
        {
            EntriesAdded += entriesToAdd;
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = GetValid(i + 1);
                if (typeof(IdT) == typeof(int))
                {
                    _intRepository.EnsurePersistent(validEntity);
                }
                else if (typeof(IdT) == typeof(Guid))
                {
                    _guidRepository.EnsurePersistent(validEntity, true);
                }
                else
                {
                    _stringRepository.EnsurePersistent(validEntity);
                }
            }
        }

        protected override void InitServiceLocator()
        {
            //base.InitServiceLocator();
            var container = ServiceLocatorInitializer.Init();

            base.RegisterAdditionalServices(container);
        }

        #endregion Init

        #region CRUD Tests

        /// <summary>
        /// Determines whether this instance [can save valid entity].
        /// </summary>
        [TestMethod]
        public void CanSaveValidEntity()
        {
            var validEntity = GetValid(null);
            if (typeof(IdT) == typeof(int))
            {
                _intRepository.EnsurePersistent(validEntity);
            }
            else if (typeof(IdT) == typeof(Guid))
            {
                _guidRepository.EnsurePersistent(validEntity, true);
            }
            else
            {
                _stringRepository.EnsurePersistent(validEntity);
            }

            Assert.AreEqual(false, validEntity.IsTransient());
        }


        /// <summary>
        /// Determines whether this instance [can commit valid entity].
        /// </summary>
        [TestMethod]
        public void CanCommitValidEntity()
        {
            var validEntity = GetValid(null);
            if (typeof(IdT) == typeof(int))
            {
                _intRepository.DbContext.BeginTransaction();
                _intRepository.EnsurePersistent(validEntity);
                Assert.IsFalse(validEntity.IsTransient());
                _intRepository.DbContext.CommitTransaction();
            }
            else if (typeof(IdT) == typeof(Guid))
            {
                _guidRepository.DbContext.BeginTransaction();
                _guidRepository.EnsurePersistent(validEntity, true);
                Assert.IsFalse(validEntity.IsTransient());
                _guidRepository.DbContext.CommitTransaction();
            }
            else
            {
                _stringRepository.DbContext.BeginTransaction();
                _stringRepository.EnsurePersistent(validEntity);
                Assert.IsFalse(validEntity.IsTransient());
                _stringRepository.DbContext.CommitTransaction();
            }

        }


        /// <summary>
        /// Determines whether this instance [can get all entities].
        /// </summary>
        [TestMethod]
        public void CanGetAllEntities()
        {
            List<T> foundEntities;
            if (typeof(IdT) == typeof(int))
            {
                foundEntities = _intRepository.GetAll().ToList();
            }
            else if (typeof(IdT) == typeof(Guid))
            {
                foundEntities = _guidRepository.GetAll().ToList();
            }
            else
            {
                foundEntities = _stringRepository.GetAll().ToList();
            }
            Assert.AreEqual(EntriesAdded, foundEntities.Count, "GetAll() returned a different number of records");
            for (int i = 0; i < EntriesAdded; i++)
            {
                FoundEntityComparison(foundEntities[i], i + 1);
            }
        }

        /// <summary>
        /// Determines whether this instance [can query entities].
        /// </summary>
        [TestMethod]
        public void CanQueryEntities()
        {
            List<T> foundEntry = GetQuery(3).ToList();
            Assert.AreEqual(1, foundEntry.Count);
            FoundEntityComparison(foundEntry[0], 3);
        }


        /// <summary>
        /// Determines whether this instance [can get entity using get by id where id is int].
        /// </summary>
        [TestMethod]
        public virtual void CanGetEntityUsingGetById()
        {
            if (typeof(IdT) == typeof(int))
            {
                Assert.IsTrue(EntriesAdded >= 2, "There are not enough entries to complete this test.");
                var foundEntity = Repository.OfType<T>().GetById(2);
                FoundEntityComparison(foundEntity, 2);
            }
            else if (typeof(IdT) == typeof(Guid))
            {
                Assert.IsTrue(EntriesAdded >= 2, "There are not enough entries to complete this test.");
                var guidFor2 = _guidRepository.GetAll()[1].Id.ToString();
                var foundEntity = _guidRepository.GetById(new Guid(guidFor2));
                FoundEntityComparison(foundEntity, 2);
            }
            else
            {
                Assert.IsTrue(EntriesAdded >= 2, "There are not enough entries to complete this test.");
                var foundEntity = _stringRepository.GetById("2");
                FoundEntityComparison(foundEntity, 2);
            }
        }


        /// <summary>
        /// Determines whether this instance [can get entity using get by nullable with valid id where id is int].
        /// </summary>
        [TestMethod]
        public virtual void CanGetEntityUsingGetByNullableWithValidId()
        {
            if (typeof(IdT) == typeof(int))
            {
                Assert.IsTrue(EntriesAdded >= 2, "There are not enough entries to complete this test.");
                var foundEntity = _intRepository.GetNullableById(2);
                FoundEntityComparison(foundEntity, 2);
            }
            else if (typeof(IdT) == typeof(Guid))
            {
                Assert.IsTrue(EntriesAdded >= 2, "There are not enough entries to complete this test.");
                var guidFor2 = _guidRepository.GetAll()[1].Id.ToString();
                var foundEntity = _guidRepository.GetNullableById(new Guid(guidFor2));
                FoundEntityComparison(foundEntity, 2);
            }
            else
            {
                Assert.IsTrue(EntriesAdded >= 2, "There are not enough entries to complete this test.");
                var foundEntity = _stringRepository.GetNullableById("2");
                FoundEntityComparison(foundEntity, 2);
            }
        }

        /// <summary>
        /// Determines whether this instance [can get null value using get by nullable with invalid id where id is int].
        /// </summary>
        [TestMethod]
        public virtual void CanGetNullValueUsingGetByNullableWithInvalidId()
        {
            if (typeof(IdT) == typeof(int))
            {
                var foundEntity = _intRepository.GetNullableById(EntriesAdded + 1);
                Assert.IsNull(foundEntity);
            }
            else if (typeof(IdT) == typeof(Guid))
            {
                var foundEntity = _guidRepository.GetNullableById(Guid.NewGuid());
                Assert.IsNull(foundEntity);
            }
            else
            {
                var foundEntity = _stringRepository.GetNullableById((EntriesAdded + 1).ToString());
                Assert.IsNull(foundEntity);
            }
        }

        public void CanUpdateEntity(bool doesItAllowUpdate)
        {
            //Get an entity to update
            T foundEntity;
            if (typeof(IdT) == typeof(int))
            {
                foundEntity = _intRepository.GetAll()[2];
            }
            else if (typeof(IdT) == typeof(Guid))
            {
                foundEntity = _guidRepository.GetAll()[2];
            }
            else
            {
                foundEntity = _stringRepository.GetAll()[2];
            }


            //Update and commit entity
            if (typeof(IdT) == typeof(int))
            {
                _intRepository.DbContext.BeginTransaction();
                UpdateUtility(foundEntity, ARTAction.Update);
                _intRepository.EnsurePersistent(foundEntity);
                _intRepository.DbContext.CommitTransaction();
            }
            else if (typeof(IdT) == typeof(Guid))
            {
                _guidRepository.DbContext.BeginTransaction();
                UpdateUtility(foundEntity, ARTAction.Update);
                _guidRepository.EnsurePersistent(foundEntity, true);
                _guidRepository.DbContext.CommitTransaction();
            }
            else
            {
                _stringRepository.DbContext.BeginTransaction();
                UpdateUtility(foundEntity, ARTAction.Update);
                _stringRepository.EnsurePersistent(foundEntity);
                _stringRepository.DbContext.CommitTransaction();
            }

            NHibernateSessionManager.Instance.GetSession().Evict(foundEntity);

            if (doesItAllowUpdate)
            {
                //Compare entity
                T compareEntity;
                if (typeof(IdT) == typeof(int))
                {
                    compareEntity = _intRepository.GetAll()[2];
                }
                else if (typeof(IdT) == typeof(Guid))
                {
                    compareEntity = _guidRepository.GetAll()[2];
                }
                else
                {
                    compareEntity = _stringRepository.GetAll()[2];
                }
                UpdateUtility(compareEntity, ARTAction.Compare);

                //Restore entity, do not commit, then get entity to make sure it isn't restored.            
                UpdateUtility(compareEntity, ARTAction.Restore);
                NHibernateSessionManager.Instance.GetSession().Evict(compareEntity);
                //For testing at least, this is required to clear the changes from memory.
                T checkNotUpdatedEntity;
                if (typeof(IdT) == typeof(int))
                {
                    checkNotUpdatedEntity = _intRepository.GetAll()[2];
                }
                else if (typeof(IdT) == typeof(Guid))
                {
                    checkNotUpdatedEntity = _guidRepository.GetAll()[2];
                }
                else
                {
                    checkNotUpdatedEntity = _stringRepository.GetAll()[2];
                }
                UpdateUtility(checkNotUpdatedEntity, ARTAction.Compare);
            }
            else
            {
                //Compare entity
                T compareEntity;
                if (typeof(IdT) == typeof(int))
                {
                    compareEntity = _intRepository.GetAll()[2];
                }
                else if (typeof(IdT) == typeof(Guid))
                {
                    compareEntity = _guidRepository.GetAll()[2];
                }
                else
                {
                    compareEntity = _stringRepository.GetAll()[2];
                }
                UpdateUtility(compareEntity, ARTAction.CompareNotUpdated);
            }
        }

        /// <summary>
        /// Determines whether this instance [can update entity].
        /// Defaults to true unless overridden
        /// </summary>
        [TestMethod]
        public virtual void CanUpdateEntity()
        {
            CanUpdateEntity(true);
        }


        /// <summary>
        /// Determines whether this instance [can delete entity].
        /// </summary>
        [TestMethod]
        public virtual void CanDeleteEntity()
        {

            if (typeof(IdT) == typeof(int))
            {
                var count = _intRepository.GetAll().ToList().Count();
                var foundEntity = _intRepository.GetAll().ToList()[2];

                //Update and commit entity
                _intRepository.DbContext.BeginTransaction();
                _intRepository.Remove(foundEntity);
                _intRepository.DbContext.CommitTransaction();
                Assert.AreEqual(count - 1, _intRepository.GetAll().ToList().Count());
                foundEntity = Repository.OfType<T>().GetNullableById(3);
                Assert.IsNull(foundEntity);
            }
            else if (typeof(IdT) == typeof(Guid))
            {
                var count = _guidRepository.GetAll().ToList().Count();
                var foundEntity = _guidRepository.GetAll().ToList()[2];

                //Update and commit entity
                _guidRepository.DbContext.BeginTransaction();
                _guidRepository.Remove(foundEntity);
                _guidRepository.DbContext.CommitTransaction();
                Assert.AreEqual(count - 1, _guidRepository.GetAll().ToList().Count());
                foundEntity = _guidRepository.GetNullableById(SpecificGuid.GetGuid(3));
                Assert.IsNull(foundEntity);
            }
            else
            {
                var count = _stringRepository.GetAll().ToList().Count();
                var foundEntity = _stringRepository.GetAll().ToList()[2];

                //Update and commit entity
                _stringRepository.DbContext.BeginTransaction();
                _stringRepository.Remove(foundEntity);
                _stringRepository.DbContext.CommitTransaction();
                Assert.AreEqual(count - 1, _stringRepository.GetAll().ToList().Count());
                foundEntity = _stringRepository.GetNullableById("3");
                Assert.IsNull(foundEntity);
            }
        }

        #endregion CRUD Tests

        #region Utilities



        /// <summary>
        /// Abstract Repository Tests Action
        /// </summary>
        public enum ARTAction
        {
            Compare = 1,
            Update,
            Restore,
            CompareNotUpdated
        }


        public void LoadStatusCodes(int entriesToAdd)
        {
            var orderStatusCodeRepository = new RepositoryWithTypedId<OrderStatusCode, string>();
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity  = CreateValidEntities.OrderStatusCode(i + 1);
                validEntity.Id = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                orderStatusCodeRepository.EnsurePersistent(validEntity);
            }
        }


        public void LoadUsers(int entriesToAdd)
        {
            var userRepository = new RepositoryWithTypedId<User, string>();
            var offset = userRepository.Queryable.Count();
            for(int i = offset; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.User(i + 1);
                validEntity.Id = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                userRepository.EnsurePersistent(validEntity);
            }
        }

        public void LoadAccounts(int entriesToAdd)
        {
            var accountRepository = new RepositoryWithTypedId<Account, string>();
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Account(i + 1);
                validEntity.Id = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                accountRepository.EnsurePersistent(validEntity);
            }
        }

        public void LoadSubAccounts(int entriesToAdd)
        {
            var subAccountRepository = new RepositoryWithTypedId<SubAccount, Guid>();
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.SubAccount(i + 1);
                validEntity.AccountNumber = (i + 1).ToString(CultureInfo.InvariantCulture);
                validEntity.Id = Guid.NewGuid();
                subAccountRepository.EnsurePersistent(validEntity);
            }
        }
        public void LoadSplits(int entriesToAdd)
        {
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Split(i + 1);
                validEntity.Order = Repository.OfType<Order>().Queryable.First();
                Repository.OfType<Split>().EnsurePersistent(validEntity);
            }
        }


        public void LoadOrders(int entriesToAdd)
        {
            var orderTypeRepository = new RepositoryWithTypedId<OrderType, string>();
            var organizationRepository = new RepositoryWithTypedId<Organization, string>();
            var orderStatusCodeRepository = new RepositoryWithTypedId<OrderStatusCode, string>();
            var userRepository = new RepositoryWithTypedId<User, string>();
            LoadOrderTypes();
            LoadOrganizations(3);
            LoadWorkgroups(3);
            LoadWorkgroupVendors(3);
            LoadWorkgroupAddress(3);
            LoadOrderStatusCodes();
            LoadUsers(3);
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Order(i + 1);
                validEntity.OrderType = orderTypeRepository.Queryable.Single(a => a.Id == OrderType.Types.OrderRequest);
                validEntity.Vendor = Repository.OfType<WorkgroupVendor>().Queryable.First();
                validEntity.Address = Repository.OfType<WorkgroupAddress>().Queryable.First();
                validEntity.Workgroup = Repository.OfType<Workgroup>().Queryable.First();
                validEntity.Organization = organizationRepository.Queryable.First();
                validEntity.StatusCode = orderStatusCodeRepository.Queryable.Single(a => a.Id == OrderStatusCode.Codes.Approver);
                validEntity.CreatedBy = userRepository.Queryable.First();
                Repository.OfType<Order>().EnsurePersistent(validEntity);
            }
        }

        public void LoadWorkgroups(int entriesToAdd)
        {
            var organizationRepository = new RepositoryWithTypedId<Organization, string>();
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Workgroup(i + 1);
                validEntity.PrimaryOrganization = organizationRepository.Queryable.First();
                Repository.OfType<Workgroup>().EnsurePersistent(validEntity);
            }
        }

        public void LoadRoles(int entriesToAdd)
        {
            var roleRepository = new RepositoryWithTypedId<Role, string>();
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Role(i + 1);
                roleRepository.EnsurePersistent(validEntity);
            }
        }

        public void LoadVendors(int entriesToAdd)
        {
            var vendorRepository = new RepositoryWithTypedId<Vendor, string>();
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Vendor(i + 1);
                validEntity.Id = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                vendorRepository.EnsurePersistent(validEntity);
            }
        }

        public void LoadOrganizations(int entriesToAdd)
        {
            var organizationRepository = new RepositoryWithTypedId<Organization, string>();
            var offset = organizationRepository.Queryable.Count();
            for(int i = offset; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Organization(i + 1);
                validEntity.Id = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                organizationRepository.EnsurePersistent(validEntity);
            }
        }

        public void LoadBuildings(int entriesToAdd)
        {
            var buildingRepository = new RepositoryWithTypedId<Building, string>();
            var offset = buildingRepository.Queryable.Count();
            for(int i = offset; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Building(i + 1);
                validEntity.Id = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                buildingRepository.EnsurePersistent(validEntity);
            }
        }


        public void LoadCustomField(int entriesToAdd)
        {
            var organizationRepository = new RepositoryWithTypedId<Organization, string>();
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.CustomField(i + 1);
                validEntity.Organization = organizationRepository.Queryable.First();
                Repository.OfType<CustomField>().EnsurePersistent(validEntity);
            }
        }

        public void LoadCommodity(int entriesToAdd)
        {
            var commodityRepository = new RepositoryWithTypedId<Commodity, string>();
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Commodity(i + 1);
                validEntity.Id = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                commodityRepository.EnsurePersistent(validEntity);
            }
        }
        public void LoadOrderTypes()
        {
            var orderTypeRepository = new RepositoryWithTypedId<OrderType, string>();
            var record = new OrderType(OrderType.Types.DepartmentalPurchaseOrder);
            record.Name = record.Id;
            orderTypeRepository.EnsurePersistent(record);
            record = new OrderType(OrderType.Types.DepartmentalRepairOrder);
            record.Name = record.Id;
            orderTypeRepository.EnsurePersistent(record);
            record = new OrderType(OrderType.Types.OrderRequest);
            record.Name = record.Id;
            orderTypeRepository.EnsurePersistent(record);
            record = new OrderType(OrderType.Types.PurchaseRequest);
            record.Name = record.Id;
            orderTypeRepository.EnsurePersistent(record);
            record = new OrderType(OrderType.Types.PurchasingCard);
            record.Name = record.Id;
            orderTypeRepository.EnsurePersistent(record);
            record = new OrderType(OrderType.Types.UcdBuyOrder);
            record.Name = record.Id;
            orderTypeRepository.EnsurePersistent(record);
        }

        public void LoadWorkgroupVendors(int entriesToAdd)
        {          
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.WorkgroupVendor(i + 1);
                validEntity.Workgroup = Repository.OfType<Workgroup>().Queryable.First();
                Repository.OfType<WorkgroupVendor>().EnsurePersistent(validEntity);
            }
        }

        public void LoadLineItems(int entriesToAdd)
        {
            if(!Repository.OfType<Order>().Queryable.Any())
            {
                LoadOrders(3);
            }
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.LineItem(i + 1);
                validEntity.Order = Repository.OfType<Order>().Queryable.First();
                Repository.OfType<LineItem>().EnsurePersistent(validEntity);
            }
        }

        public void LoadWorkgroupAddress(int entriesToAdd)
        {
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.WorkgroupAddress(i + 1);
                validEntity.Workgroup = Repository.OfType<Workgroup>().Queryable.First();
                Repository.OfType<WorkgroupAddress>().EnsurePersistent(validEntity);
            }
        }

        public void LoadApprovals(int entriesToAdd)
        {
            var orderStatusCodeRepository = new RepositoryWithTypedId<OrderStatusCode, string>();
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Approval(i + 1);
                validEntity.StatusCode = orderStatusCodeRepository.Queryable.First();
                validEntity.Order = Repository.OfType<Order>().Queryable.First();
                validEntity.User = null;
                Repository.OfType<Approval>().EnsurePersistent(validEntity);
            }
        }

        public void LoadShippingTypes(int entriesToAdd)
        {
            var shippingTypeRepository = new RepositoryWithTypedId<ShippingType, string>();
            for(int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.ShippingType(i + 1);
                validEntity.Id = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                shippingTypeRepository.EnsurePersistent(validEntity);
            }
        }

        public void LoadOrderStatusCodes()
        {
            var orderStatusCodeRepository = new RepositoryWithTypedId<OrderStatusCode, string>();
            var orderStatusCodes = new List<OrderStatusCode>();
            var orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Account Manager";
            orderStatusCode.Level = 3;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.Id = "AM";
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Approver";
            orderStatusCode.Level = 2;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.Id = "AP";
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Conditional Approval";
            orderStatusCode.Level = 2;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = false;
            orderStatusCode.Id = "CA";
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Complete-Not Uploaded KFS";
            orderStatusCode.Level = 5;
            orderStatusCode.IsComplete = true;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = false;
            orderStatusCode.Id = "CN";
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Complete";
            orderStatusCode.Level = 5;
            orderStatusCode.IsComplete = true;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.Id = "CP";
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Purchaser";
            orderStatusCode.Level = 4;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.Id = "PR";
            orderStatusCodes.Add(orderStatusCode);


            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Requester";
            orderStatusCode.Level = 1;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = false;
            orderStatusCode.Id = "RQ";
            orderStatusCodes.Add(orderStatusCode);

            foreach (var statusCode in orderStatusCodes)
            {
                orderStatusCodeRepository.EnsurePersistent(statusCode);
            }

        }
        #endregion Utilities


    }
}
