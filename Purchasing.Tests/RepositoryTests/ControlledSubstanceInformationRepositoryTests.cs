using System;
using System.Collections;
using System.Collections.Generic;
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
    /// Entity Name:		ControlledSubstanceInformation
    /// LookupFieldName:	AuthorizationNum
    /// </summary>
    [TestClass]
    public class ControlledSubstanceInformationRepositoryTests : AbstractRepositoryTests<ControlledSubstanceInformation, int, ControlledSubstanceInformationMap>
    {
        /// <summary>
        /// Gets or sets the ControlledSubstanceInformation repository.
        /// </summary>
        /// <value>The ControlledSubstanceInformation repository.</value>
        public IRepository<ControlledSubstanceInformation> ControlledSubstanceInformationRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; } 

        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlledSubstanceInformationRepositoryTests"/> class.
        /// </summary>
        public ControlledSubstanceInformationRepositoryTests()
        {
            ControlledSubstanceInformationRepository = new Repository<ControlledSubstanceInformation>();
            OrderRepository = new Repository<Order>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ControlledSubstanceInformation GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ControlledSubstanceInformation(counter);
            rtValue.Order = OrderRepository.Queryable.Single(a => a.Id == 2);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ControlledSubstanceInformation> GetQuery(int numberAtEnd)
        {
            return ControlledSubstanceInformationRepository.Queryable.Where(a => a.Use.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ControlledSubstanceInformation entity, int counter)
        {
            Assert.AreEqual("Use" + counter, entity.Use);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ControlledSubstanceInformation entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Use);
                    break;
                case ARTAction.Restore:
                    entity.Use = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Use;
                    entity.Use = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            OrderRepository.DbContext.BeginTransaction();
            LoadOrders(3);
            OrderRepository.DbContext.CommitTransaction();

            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        //#region AuthorizationNum Tests
        //#region Invalid Tests

        ///// <summary>
        ///// Tests the AuthorizationNum with null value does not save.
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(ApplicationException))]
        //public void TestAuthorizationNumWithNullValueDoesNotSave()
        //{
        //    ControlledSubstanceInformation controlledSubstanceInformation = null;
        //    try
        //    {
        //        #region Arrange
        //        controlledSubstanceInformation = GetValid(9);
        //        controlledSubstanceInformation.AuthorizationNum = null;
        //        #endregion Arrange

        //        #region Act
        //        ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
        //        ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
        //        ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
        //        #endregion Act
        //    }
        //    catch (Exception)
        //    {
        //        Assert.IsNotNull(controlledSubstanceInformation);
        //        var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
        //        results.AssertErrorsAre(string.Format("The {0} field is required.", "AuthorizationNum"));
        //        Assert.IsTrue(controlledSubstanceInformation.IsTransient());
        //        Assert.IsFalse(controlledSubstanceInformation.IsValid());
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Tests the AuthorizationNum with empty string does not save.
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(ApplicationException))]
        //public void TestAuthorizationNumWithEmptyStringDoesNotSave()
        //{
        //    ControlledSubstanceInformation controlledSubstanceInformation = null;
        //    try
        //    {
        //        #region Arrange
        //        controlledSubstanceInformation = GetValid(9);
        //        controlledSubstanceInformation.AuthorizationNum = string.Empty;
        //        #endregion Arrange

        //        #region Act
        //        ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
        //        ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
        //        ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
        //        #endregion Act
        //    }
        //    catch (Exception)
        //    {
        //        Assert.IsNotNull(controlledSubstanceInformation);
        //        var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
        //        results.AssertErrorsAre(string.Format("The {0} field is required.", "AuthorizationNum"));
        //        Assert.IsTrue(controlledSubstanceInformation.IsTransient());
        //        Assert.IsFalse(controlledSubstanceInformation.IsValid());
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Tests the AuthorizationNum with spaces only does not save.
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(ApplicationException))]
        //public void TestAuthorizationNumWithSpacesOnlyDoesNotSave()
        //{
        //    ControlledSubstanceInformation controlledSubstanceInformation = null;
        //    try
        //    {
        //        #region Arrange
        //        controlledSubstanceInformation = GetValid(9);
        //        controlledSubstanceInformation.AuthorizationNum = " ";
        //        #endregion Arrange

        //        #region Act
        //        ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
        //        ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
        //        ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
        //        #endregion Act
        //    }
        //    catch (Exception)
        //    {
        //        Assert.IsNotNull(controlledSubstanceInformation);
        //        var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
        //        results.AssertErrorsAre(string.Format("The {0} field is required.", "AuthorizationNum"));
        //        Assert.IsTrue(controlledSubstanceInformation.IsTransient());
        //        Assert.IsFalse(controlledSubstanceInformation.IsValid());
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Tests the AuthorizationNum with too long value does not save.
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(ApplicationException))]
        //public void TestAuthorizationNumWithTooLongValueDoesNotSave()
        //{
        //    ControlledSubstanceInformation controlledSubstanceInformation = null;
        //    try
        //    {
        //        #region Arrange
        //        controlledSubstanceInformation = GetValid(9);
        //        controlledSubstanceInformation.AuthorizationNum = "x".RepeatTimes((10 + 1));
        //        #endregion Arrange

        //        #region Act
        //        ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
        //        ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
        //        ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
        //        #endregion Act
        //    }
        //    catch (Exception)
        //    {
        //        Assert.IsNotNull(controlledSubstanceInformation);
        //        Assert.AreEqual(10 + 1, controlledSubstanceInformation.AuthorizationNum.Length);
        //        var results = controlledSubstanceInformation.ValidationResults().AsMessageList();		
        //        results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "AuthorizationNum", "10"));
        //        Assert.IsTrue(controlledSubstanceInformation.IsTransient());
        //        Assert.IsFalse(controlledSubstanceInformation.IsValid());
        //        throw;
        //    }
        //}
        //#endregion Invalid Tests

        //#region Valid Tests

        ///// <summary>
        ///// Tests the AuthorizationNum with one character saves.
        ///// </summary>
        //[TestMethod]
        //public void TestAuthorizationNumWithOneCharacterSaves()
        //{
        //    #region Arrange
        //    var controlledSubstanceInformation = GetValid(9);
        //    controlledSubstanceInformation.AuthorizationNum = "x";
        //    #endregion Arrange

        //    #region Act
        //    ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
        //    ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
        //    ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsFalse(controlledSubstanceInformation.IsTransient());
        //    Assert.IsTrue(controlledSubstanceInformation.IsValid());
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the AuthorizationNum with long value saves.
        ///// </summary>
        //[TestMethod]
        //public void TestAuthorizationNumWithLongValueSaves()
        //{
        //    #region Arrange
        //    var controlledSubstanceInformation = GetValid(9);
        //    controlledSubstanceInformation.AuthorizationNum = "x".RepeatTimes(10);
        //    #endregion Arrange

        //    #region Act
        //    ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
        //    ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
        //    ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(10, controlledSubstanceInformation.AuthorizationNum.Length);
        //    Assert.IsFalse(controlledSubstanceInformation.IsTransient());
        //    Assert.IsTrue(controlledSubstanceInformation.IsValid());
        //    #endregion Assert
        //}

        //#endregion Valid Tests
        //#endregion AuthorizationNum Tests
        
        #region ClassSchedule Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the ClassSchedule with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestClassScheduleWithNullValueDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.ClassSchedule = null;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "ClassSchedule"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the ClassSchedule with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestClassScheduleWithEmptyStringDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.ClassSchedule = string.Empty;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "ClassSchedule"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the ClassSchedule with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestClassScheduleWithSpacesOnlyDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.ClassSchedule = " ";
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "ClassSchedule"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the ClassSchedule with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestClassScheduleWithTooLongValueDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.ClassSchedule = "x".RepeatTimes((10 + 1));
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                Assert.AreEqual(10 + 1, controlledSubstanceInformation.ClassSchedule.Length);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "ClassSchedule", "10"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the ClassSchedule with one character saves.
        /// </summary>
        [TestMethod]
        public void TestClassScheduleWithOneCharacterSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.ClassSchedule = "x";
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ClassSchedule with long value saves.
        /// </summary>
        [TestMethod]
        public void TestClassScheduleWithLongValueSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.ClassSchedule = "x".RepeatTimes(10);
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, controlledSubstanceInformation.ClassSchedule.Length);
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion ClassSchedule Tests

        #region Use Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Use with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUseWithNullValueDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.Use = null;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Use"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Use with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUseWithEmptyStringDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.Use = string.Empty;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Use"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Use with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUseWithSpacesOnlyDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.Use = " ";
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Use"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Use with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUseWithTooLongValueDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.Use = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                Assert.AreEqual(200 + 1, controlledSubstanceInformation.Use.Length);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Use", "200"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Use with one character saves.
        /// </summary>
        [TestMethod]
        public void TestUseWithOneCharacterSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.Use = "x";
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Use with long value saves.
        /// </summary>
        [TestMethod]
        public void TestUseWithLongValueSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.Use = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, controlledSubstanceInformation.Use.Length);
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Use Tests

        #region StorageSite Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the StorageSite with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStorageSiteWithNullValueDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.StorageSite = null;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "StorageSite"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the StorageSite with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStorageSiteWithEmptyStringDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.StorageSite = string.Empty;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "StorageSite"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the StorageSite with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStorageSiteWithSpacesOnlyDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.StorageSite = " ";
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "StorageSite"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the StorageSite with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStorageSiteWithTooLongValueDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.StorageSite = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                Assert.AreEqual(50 + 1, controlledSubstanceInformation.StorageSite.Length);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "StorageSite", "50"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the StorageSite with one character saves.
        /// </summary>
        [TestMethod]
        public void TestStorageSiteWithOneCharacterSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.StorageSite = "x";
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the StorageSite with long value saves.
        /// </summary>
        [TestMethod]
        public void TestStorageSiteWithLongValueSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.StorageSite = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, controlledSubstanceInformation.StorageSite.Length);
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion StorageSite Tests

        #region Custodian Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Custodian with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCustodianWithNullValueDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.Custodian = null;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Custodian"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Custodian with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCustodianWithEmptyStringDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.Custodian = string.Empty;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Custodian"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Custodian with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCustodianWithSpacesOnlyDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.Custodian = " ";
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Custodian"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Custodian with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCustodianWithTooLongValueDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.Custodian = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                Assert.AreEqual(200 + 1, controlledSubstanceInformation.Custodian.Length);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Custodian", "200"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Custodian with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCustodianWithOneCharacterSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.Custodian = "x";
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Custodian with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCustodianWithLongValueSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.Custodian = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, controlledSubstanceInformation.Custodian.Length);
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Custodian Tests

        #region EndUser Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the EndUser with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEndUserWithNullValueDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.EndUser = null;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "EndUser"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the EndUser with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEndUserWithEmptyStringDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.EndUser = string.Empty;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "EndUser"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the EndUser with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEndUserWithSpacesOnlyDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.EndUser = " ";
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "EndUser"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the EndUser with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEndUserWithTooLongValueDoesNotSave()
        {
            ControlledSubstanceInformation controlledSubstanceInformation = null;
            try
            {
                #region Arrange
                controlledSubstanceInformation = GetValid(9);
                controlledSubstanceInformation.EndUser = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(controlledSubstanceInformation);
                Assert.AreEqual(200 + 1, controlledSubstanceInformation.EndUser.Length);
                var results = controlledSubstanceInformation.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "EndUser", "200"));
                Assert.IsTrue(controlledSubstanceInformation.IsTransient());
                Assert.IsFalse(controlledSubstanceInformation.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the EndUser with one character saves.
        /// </summary>
        [TestMethod]
        public void TestEndUserWithOneCharacterSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.EndUser = "x";
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the EndUser with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEndUserWithLongValueSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.EndUser = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, controlledSubstanceInformation.EndUser.Length);
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion EndUser Tests

        #region Order Tests


        [TestMethod]
        public void TestControlledSubstanceInformationsWithExistingOrderSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = OrderRepository.Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(record);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Order.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestControlledSubstanceInformationsFieldOrderWithAValueOfNullDoesNotSave()
        {
            ControlledSubstanceInformation record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Order = null;
                #endregion Arrange

                #region Act
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(record);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
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
        public void TestControlledSubstanceInformationNewOrderDoesNotSave()
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
                ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
                ControlledSubstanceInformationRepository.EnsurePersistent(record);
                ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
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

        #endregion Order Tests

        #region PharmaceuticalGrade Tests

        /// <summary>
        /// Tests the PharmaceuticalGrade is false saves.
        /// </summary>
        [TestMethod]
        public void TestPharmaceuticalGradeIsFalseSaves()
        {
            #region Arrange
            ControlledSubstanceInformation controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.PharmaceuticalGrade = false;
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(controlledSubstanceInformation.PharmaceuticalGrade);
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PharmaceuticalGrade is true saves.
        /// </summary>
        [TestMethod]
        public void TestPharmaceuticalGradeIsTrueSaves()
        {
            #region Arrange
            var controlledSubstanceInformation = GetValid(9);
            controlledSubstanceInformation.PharmaceuticalGrade = true;
            #endregion Arrange

            #region Act
            ControlledSubstanceInformationRepository.DbContext.BeginTransaction();
            ControlledSubstanceInformationRepository.EnsurePersistent(controlledSubstanceInformation);
            ControlledSubstanceInformationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(controlledSubstanceInformation.PharmaceuticalGrade);
            Assert.IsFalse(controlledSubstanceInformation.IsTransient());
            Assert.IsTrue(controlledSubstanceInformation.IsValid());
            #endregion Assert
        }

        #endregion PharmaceuticalGrade Tests



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
            //expectedFields.Add(new NameAndType("AuthorizationNum", "System.String", new List<string>
            //{
            //     "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
            //     "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]"
            //}));
            expectedFields.Add(new NameAndType("ClassSchedule", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]"
            }));
            expectedFields.Add(new NameAndType("Custodian", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)200)]"
            }));
            expectedFields.Add(new NameAndType("EndUser", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)200)]"
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
            expectedFields.Add(new NameAndType("PharmaceuticalGrade", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("StorageSite", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Use", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)200)]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ControlledSubstanceInformation));

        }

        #endregion Reflection of Database.	
		
		
    }
}