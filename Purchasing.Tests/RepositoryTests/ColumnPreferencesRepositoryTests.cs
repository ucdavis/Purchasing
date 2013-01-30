using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		ColumnPreferences
    /// LookupFieldName:	Id
    /// </summary>
    [TestClass]
    public class ColumnPreferencesRepositoryTests : AbstractRepositoryTests<ColumnPreferences, string, ColumnPreferencesMap>
    {
        /// <summary>
        /// Gets or sets the ColumnPreferences repository.
        /// </summary>
        /// <value>The ColumnPreferences repository.</value>
        public IRepository<ColumnPreferences> ColumnPreferencesRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnPreferencesRepositoryTests"/> class.
        /// </summary>
        public ColumnPreferencesRepositoryTests()
        {
            ColumnPreferencesRepository = new Repository<ColumnPreferences>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ColumnPreferences GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ColumnPreferences(counter);
            if(counter != null && counter == 3)
            {
                rtValue.ShowRequestNumber = true;
            }
            else
            {
                rtValue.ShowRequestNumber = false;
            }

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ColumnPreferences> GetQuery(int numberAtEnd)
        {
            return ColumnPreferencesRepository.Queryable.Where(a => a.Id.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ColumnPreferences entity, int counter)
        {
            Assert.AreEqual(counter.ToString(), entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ColumnPreferences entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.ShowRequestNumber);
                    break;
                case ARTAction.Restore:
                    entity.ShowRequestNumber = BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.ShowRequestNumber;
                    entity.ShowRequestNumber = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region ShowRequestNumber Tests

        /// <summary>
        /// Tests the ShowRequestNumber is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowRequestNumberIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowRequestNumber = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowRequestNumber);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowRequestNumber is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowRequestNumberIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowRequestNumber = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowRequestNumber);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowRequestNumber Tests

        #region ShowPurchaseOrderNumber Tests

        /// <summary>
        /// Tests the ShowPurchaseOrderNumber is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowPurchaseOrderNumberIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowPurchaseOrderNumber = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowPurchaseOrderNumber);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowPurchaseOrderNumber is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowPurchaseOrderNumberIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowPurchaseOrderNumber = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowPurchaseOrderNumber);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowPurchaseOrderNumber Tests

        #region ShowWorkgroup Tests

        /// <summary>
        /// Tests the ShowWorkgroup is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowWorkgroupIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowWorkgroup = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowWorkgroup);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowWorkgroup is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowWorkgroupIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowWorkgroup = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowWorkgroup);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowWorkgroup Tests

        //#region ShowOrganization Tests

        ///// <summary>
        ///// Tests the ShowOrganization is false saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowOrganizationIsFalseSaves()
        //{
        //    #region Arrange
        //    ColumnPreferences columnPreferences = GetValid(9);
        //    columnPreferences.ShowOrganization = false;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsFalse(columnPreferences.ShowOrganization);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the ShowOrganization is true saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowOrganizationIsTrueSaves()
        //{
        //    #region Arrange
        //    var columnPreferences = GetValid(9);
        //    columnPreferences.ShowOrganization = true;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(columnPreferences.ShowOrganization);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        //#endregion ShowOrganization Tests

        #region ShowVendor Tests

        /// <summary>
        /// Tests the ShowVendor is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowVendorIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowVendor = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowVendor);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowVendor is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowVendorIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowVendor = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowVendor);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowVendor Tests

        #region ShowShipTo Tests

        /// <summary>
        /// Tests the ShowShipTo is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowShipToIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowShipTo = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowShipTo);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowShipTo is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowShipToIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowShipTo = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowShipTo);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowShipTo Tests

        #region ShowAllowBackorder Tests

        /// <summary>
        /// Tests the ShowAllowBackorder is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowAllowBackorderIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowAllowBackorder = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowAllowBackorder);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowAllowBackorder is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowAllowBackorderIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowAllowBackorder = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowAllowBackorder);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowAllowBackorder Tests

        #region ShowRestrictedOrder Tests

        /// <summary>
        /// Tests the ShowRestrictedOrder is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowRestrictedOrderIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowRestrictedOrder = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowRestrictedOrder);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowRestrictedOrder is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowRestrictedOrderIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowRestrictedOrder = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowRestrictedOrder);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowRestrictedOrder Tests

        #region ShowHasSplits Tests

        /// <summary>
        /// Tests the ShowHasSplits is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowHasSplitsIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowHasSplits = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowHasSplits);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowHasSplits is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowHasSplitsIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowHasSplits = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowHasSplits);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowHasSplits Tests

        //#region ShowHasAttachments Tests

        ///// <summary>
        ///// Tests the ShowHasAttachments is false saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowHasAttachmentsIsFalseSaves()
        //{
        //    #region Arrange
        //    ColumnPreferences columnPreferences = GetValid(9);
        //    columnPreferences.ShowHasAttachments = false;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsFalse(columnPreferences.ShowHasAttachments);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the ShowHasAttachments is true saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowHasAttachmentsIsTrueSaves()
        //{
        //    #region Arrange
        //    var columnPreferences = GetValid(9);
        //    columnPreferences.ShowHasAttachments = true;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(columnPreferences.ShowHasAttachments);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        //#endregion ShowHasAttachments Tests

        //#region ShowNumberOfLines Tests

        ///// <summary>
        ///// Tests the ShowNumberOfLines is false saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowNumberOfLinesIsFalseSaves()
        //{
        //    #region Arrange
        //    ColumnPreferences columnPreferences = GetValid(9);
        //    columnPreferences.ShowNumberOfLines = false;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsFalse(columnPreferences.ShowNumberOfLines);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the ShowNumberOfLines is true saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowNumberOfLinesIsTrueSaves()
        //{
        //    #region Arrange
        //    var columnPreferences = GetValid(9);
        //    columnPreferences.ShowNumberOfLines = true;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(columnPreferences.ShowNumberOfLines);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        //#endregion ShowNumberOfLines Tests

        #region ShowTotalAmount Tests

        /// <summary>
        /// Tests the ShowTotalAmount is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowTotalAmountIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowTotalAmount = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowTotalAmount);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowTotalAmount is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowTotalAmountIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowTotalAmount = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowTotalAmount);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowTotalAmount Tests

        #region ShowCreatedBy Tests

        /// <summary>
        /// Tests the ShowCreatedBy is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowCreatedByIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowCreatedBy = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowCreatedBy);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowCreatedBy is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowCreatedByIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowCreatedBy = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowCreatedBy);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowCreatedBy Tests

        #region ShowCreatedDate Tests

        /// <summary>
        /// Tests the ShowCreatedDate is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowCreatedDateIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowCreatedDate = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowCreatedDate);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowCreatedDate is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowCreatedDateIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowCreatedDate = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowCreatedDate);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowCreatedDate Tests

        #region ShowStatus Tests

        /// <summary>
        /// Tests the ShowStatus is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowStatusIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowStatus = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowStatus);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowStatus is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowStatusIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowStatus = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowStatus);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowStatus Tests

        #region ShowNeededDate Tests

        /// <summary>
        /// Tests the ShowNeededDate is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowNeededDateIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowNeededDate = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowNeededDate);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowNeededDate is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowNeededDateIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowNeededDate = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowNeededDate);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowNeededDate Tests

        #region ShowShippingType Tests

        /// <summary>
        /// Tests the ShowShippingType is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowShippingTypeIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowShippingType = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowShippingType);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowShippingType is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowShippingTypeIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowShippingType = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowShippingType);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowShippingType Tests

        #region ShowDaysNotActedOn Tests

        /// <summary>
        /// Tests the ShowDaysNotActedOn is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowDaysNotActedOnIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowDaysNotActedOn = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowDaysNotActedOn);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowDaysNotActedOn is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowDaysNotActedOnIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowDaysNotActedOn = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowDaysNotActedOn);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowDaysNotActedOn Tests

        #region ShowLastActedOnBy Tests

        /// <summary>
        /// Tests the ShowLastActedOnBy is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowLastActedOnByIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowLastActedOnBy = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowLastActedOnBy);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowLastActedOnBy is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowLastActedOnByIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowLastActedOnBy = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowLastActedOnBy);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowLastActedOnBy Tests

        //#region ShowPeoplePendingAction Tests

        ///// <summary>
        ///// Tests the ShowPeoplePendingAction is false saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowPeoplePendingActionIsFalseSaves()
        //{
        //    #region Arrange
        //    ColumnPreferences columnPreferences = GetValid(9);
        //    columnPreferences.ShowPeoplePendingAction = false;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsFalse(columnPreferences.ShowPeoplePendingAction);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the ShowPeoplePendingAction is true saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowPeoplePendingActionIsTrueSaves()
        //{
        //    #region Arrange
        //    var columnPreferences = GetValid(9);
        //    columnPreferences.ShowPeoplePendingAction = true;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(columnPreferences.ShowPeoplePendingAction);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        //#endregion ShowPeoplePendingAction Tests

        #region ShowAccountNumber Tests

        /// <summary>
        /// Tests the ShowAccountNumber is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowAccountNumberIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowAccountNumber = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowAccountNumber);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowAccountNumber is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowAccountNumberIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowAccountNumber = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowAccountNumber);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowAccountNumber Tests

        //#region ShowOrderedDate Tests

        ///// <summary>
        ///// Tests the ShowOrderedDate is false saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowOrderedDateIsFalseSaves()
        //{
        //    #region Arrange
        //    ColumnPreferences columnPreferences = GetValid(9);
        //    columnPreferences.ShowOrderedDate = false;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsFalse(columnPreferences.ShowOrderedDate);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the ShowOrderedDate is true saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowOrderedDateIsTrueSaves()
        //{
        //    #region Arrange
        //    var columnPreferences = GetValid(9);
        //    columnPreferences.ShowOrderedDate = true;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(columnPreferences.ShowOrderedDate);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        //#endregion ShowOrderedDate Tests

        #region ShowApprover Tests

        /// <summary>
        /// Tests the ShowApprover is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowApproverIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowApprover = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowApprover);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowApprover is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowApproverIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowApprover = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowApprover);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowApprover Tests

        #region ShowAccountManager Tests

        /// <summary>
        /// Tests the ShowAccountManager is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowAccountManagerIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowAccountManager = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowAccountManager);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowAccountManager is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowAccountManagerIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowAccountManager = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowAccountManager);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowAccountManager Tests

        #region ShowPurchaser Tests

        /// <summary>
        /// Tests the ShowPurchaser is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowPurchaserIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowPurchaser = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowPurchaser);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowPurchaser is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowPurchaserIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowPurchaser = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowPurchaser);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowPurchaser Tests

        #region ShowLastActedOnDate Tests

        /// <summary>
        /// Tests the ShowLastActedOnDate is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowLastActedOnDateIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowLastActedOnDate = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowLastActedOnDate);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowLastActedOnDate is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowLastActedOnDateIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowLastActedOnDate = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowLastActedOnDate);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowLastActedOnDate Tests

        //#region ShowLastYouActedOnDate Tests

        ///// <summary>
        ///// Tests the ShowLastYouActedOnDate is false saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowLastYouActedOnDateIsFalseSaves()
        //{
        //    #region Arrange
        //    ColumnPreferences columnPreferences = GetValid(9);
        //    columnPreferences.ShowLastYouActedOnDate = false;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsFalse(columnPreferences.ShowLastYouActedOnDate);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the ShowLastYouActedOnDate is true saves.
        ///// </summary>
        //[TestMethod]
        //public void TestShowLastYouActedOnDateIsTrueSaves()
        //{
        //    #region Arrange
        //    var columnPreferences = GetValid(9);
        //    columnPreferences.ShowLastYouActedOnDate = true;
        //    #endregion Arrange

        //    #region Act
        //    ColumnPreferencesRepository.DbContext.BeginTransaction();
        //    ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
        //    ColumnPreferencesRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsTrue(columnPreferences.ShowLastYouActedOnDate);
        //    Assert.IsFalse(columnPreferences.IsTransient());
        //    Assert.IsTrue(columnPreferences.IsValid());
        //    #endregion Assert
        //}

        //#endregion ShowLastYouActedOnDate Tests

        #region ShowLineItems Tests

        /// <summary>
        /// Tests the ShowLineItems is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowLineItemsIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowLineItems = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowLineItems);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowLineItems is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowLineItemsIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowLineItems = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowLineItems);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowLineItems Tests


        #region ShowOrderReceived Tests

        /// <summary>
        /// Tests the ShowOrderReceived is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowOrderReceivedIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowOrderReceived = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowOrderReceived);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowOrderReceived is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowOrderReceivedIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowOrderReceived = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowOrderReceived);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowOrderReceived Tests

        #region ShowOrderType Tests

        /// <summary>
        /// Tests the ShowOrderType is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowOrderTypeIsFalseSaves()
        {
            #region Arrange

            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowOrderType = false;

            #endregion Arrange

            #region Act

            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(columnPreferences.ShowOrderType);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowOrderType is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowOrderTypeIsTrueSaves()
        {
            #region Arrange

            var columnPreferences = GetValid(9);
            columnPreferences.ShowOrderType = true;

            #endregion Arrange

            #region Act

            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(columnPreferences.ShowOrderType);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());

            #endregion Assert
        }

        #endregion ShowOrderType Tests

        #region DisplayRows Tests

       

        /// <summary>
        /// Tests the DisplayRows with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestDisplayRowsWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.DisplayRows = 100;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(record);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, record.DisplayRows);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DisplayRows with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestDisplayRowsWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.DisplayRows = 10;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(record);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, record.DisplayRows);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestDisplayRowsOutOfRange1()
        {
            var thisFar = false;
            ColumnPreferences record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.DisplayRows = 9;
                thisFar = true;
                #endregion Arrange

                #region Act
                ColumnPreferencesRepository.DbContext.BeginTransaction();
                ColumnPreferencesRepository.EnsurePersistent(record);
                ColumnPreferencesRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field Display Rows must be between 10 and 100."));
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDisplayRowsOutOfRange2()
        {
            var thisFar = false;
            ColumnPreferences record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.DisplayRows = 101;
                thisFar = true;
                #endregion Arrange

                #region Act
                ColumnPreferencesRepository.DbContext.BeginTransaction();
                ColumnPreferencesRepository.EnsurePersistent(record);
                ColumnPreferencesRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field Display Rows must be between 10 and 100."));
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        #endregion DisplayRows Tests

        #region ShowShippingCost Tests

        /// <summary>
        /// Tests the ShowShippingCost is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowShippingCostIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowShippingCost = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowShippingCost);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowShippingCost is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowShippingCostIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowShippingCost = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowShippingCost);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowShippingCost Tests

        #region ShowReferenceNumber Tests

        /// <summary>
        /// Tests the ShowReferenceNumber is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowReferenceNumberIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowReferenceNumber = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowReferenceNumber);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowReferenceNumber is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowReferenceNumberIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowReferenceNumber = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowReferenceNumber);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowReferenceNumber Tests

        #region ShowFpdCompleted Tests

        /// <summary>
        /// Tests the ShowFpdCompleted is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowFpdCompletedIsFalseSaves()
        {
            #region Arrange
            ColumnPreferences columnPreferences = GetValid(9);
            columnPreferences.ShowFpdCompleted = false;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(columnPreferences.ShowFpdCompleted);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowFpdCompleted is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowFpdCompletedIsTrueSaves()
        {
            #region Arrange
            var columnPreferences = GetValid(9);
            columnPreferences.ShowFpdCompleted = true;
            #endregion Arrange

            #region Act
            ColumnPreferencesRepository.DbContext.BeginTransaction();
            ColumnPreferencesRepository.EnsurePersistent(columnPreferences);
            ColumnPreferencesRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(columnPreferences.ShowFpdCompleted);
            Assert.IsFalse(columnPreferences.IsTransient());
            Assert.IsTrue(columnPreferences.IsValid());
            #endregion Assert
        }

        #endregion ShowFpdCompleted Tests



        #region Constructor Tests

        [TestMethod]
        public void TestColumnPrefsSetsExpectedValues()
        {
            #region Arrange
            var record = new ColumnPreferences("Test");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("Test", record.Id);
            Assert.IsTrue(record.ShowWorkgroup);
            Assert.IsTrue(record.ShowVendor);
            Assert.IsTrue(record.ShowCreatedBy);
            Assert.IsTrue(record.ShowCreatedDate);
            Assert.IsTrue(record.ShowStatus);
            //Assert.IsTrue(record.ShowNeededDate);
            //Assert.IsTrue(record.ShowDaysNotActedOn);
            //Assert.IsTrue(record.ShowAccountManager);

            //Assert.IsFalse(record.ShowRequestNumber);
            Assert.IsFalse(record.ShowPurchaseOrderNumber);
            //Assert.IsFalse(record.ShowWorkgroup);
            ////Assert.IsFalse(record.ShowOrganization);
            //Assert.IsFalse(record.ShowVendor);
            Assert.IsFalse(record.ShowShipTo);
            Assert.IsFalse(record.ShowAllowBackorder);
            Assert.IsFalse(record.ShowRestrictedOrder);
            Assert.IsFalse(record.ShowHasSplits);
            ////Assert.IsFalse(record.ShowHasAttachments);
            ////Assert.IsFalse(record.ShowNumberOfLines);
            //Assert.IsFalse(record.ShowTotalAmount);
            //Assert.IsFalse(record.ShowCreatedBy);
            //Assert.IsFalse(record.ShowCreatedDate);
            //Assert.IsFalse(record.ShowStatus);
            //Assert.IsFalse(record.ShowNeededDate);
            Assert.IsFalse(record.ShowShippingType);
            //Assert.IsFalse(record.ShowDaysNotActedOn);
            Assert.IsFalse(record.ShowLastActedOnBy);
            ////Assert.IsFalse(record.ShowPeoplePendingAction);
            Assert.IsFalse(record.ShowAccountNumber);
            ////Assert.IsFalse(record.ShowOrderedDate);
            Assert.IsFalse(record.ShowApprover);
            //Assert.IsFalse(record.ShowAccountManager);
            Assert.IsFalse(record.ShowPurchaser);

            Assert.IsFalse(record.ShowOrderReceived);

            Assert.AreEqual(50, record.DisplayRows);

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
            expectedFields.Add(new NameAndType("DisplayRows", "System.Int32", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Display Rows\")]", 
                                                                             "[System.ComponentModel.DataAnnotations.RangeAttribute((Int32)10, (Int32)100)]"
                                                                         }));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
                                                                         {
                                                                             "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                                                                             "[System.Xml.Serialization.XmlIgnoreAttribute()]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowAccountManager", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Account Manager\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowAccountNumber", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Account #\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowAllowBackorder", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Allow Backorder\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowApprover", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Approver\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowCreatedBy", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Created By\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowCreatedDate", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Created Date\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowDaysNotActedOn", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Days Not Acted On\")]"
                                                                         }));
            //expectedFields.Add(new NameAndType("ShowHasAttachments", "System.Boolean", new List<string>
            //                                                             {
            //                                                                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Has Attachments\")]"
            //                                                             }));
            expectedFields.Add(new NameAndType("ShowFpdCompleted", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show FPD Completed\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowHasSplits", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Has Splits\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowLastActedOnBy", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Last Acted On By\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowLastActedOnDate", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Last Acted On Date\")]"
                                                                         }));
            //expectedFields.Add(new NameAndType("ShowLastYouActedOnDate", "System.Boolean", new List<string>
            //                                                             {
            //                                                                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Last You Acted On Date\")]"
            //                                                             }));
            expectedFields.Add(new NameAndType("ShowLineItems", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Line Items\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowNeededDate", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Needed Date\")]"
                                                                         }));
            //expectedFields.Add(new NameAndType("ShowNumberOfLines", "System.Boolean", new List<string>
            //                                                             {
            //                                                                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show # Of Lines\")]"
            //                                                             }));
            //expectedFields.Add(new NameAndType("ShowOrderedDate", "System.Boolean", new List<string>
            //                                                             {
            //                                                                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Date Ordered\")]"
            //                                                             }));
            //expectedFields.Add(new NameAndType("ShowOrganization", "System.Boolean", new List<string>
            //                                                             {
            //                                                                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Organization\")]"
            //                                                             }));
            //expectedFields.Add(new NameAndType("ShowPeoplePendingAction", "System.Boolean", new List<string>
            //                                                             {
            //                                                                 "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show People Pending Action\")]"
            //                                                             }));
            expectedFields.Add(new NameAndType("ShowOrderReceived", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Order Received\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowOrderType", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Order Type\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowPurchaseOrderNumber", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show PO#\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowPurchaser", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Purchaser\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowReferenceNumber", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Ref#\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowRequestNumber", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Request Number\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowRestrictedOrder", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Restricted Orders\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowShippingCost", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Shipping Cost\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowShippingType", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Shipping Type\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowShipTo", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Ship To\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowStatus", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Status\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowTotalAmount", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Total Amount\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowVendor", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Vendor\")]"
                                                                         }));
            expectedFields.Add(new NameAndType("ShowWorkgroup", "System.Boolean", new List<string>
                                                                         {
                                                                             "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Show Workgroup\")]"
                                                                         }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ColumnPreferences));

        }

        #endregion Reflection of Database.	
		
		
    }
}