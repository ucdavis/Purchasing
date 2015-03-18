using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		ServiceMessage
    /// LookupFieldName:	Message
    /// </summary>
    [TestClass]
    public class ServiceMessageRepositoryTests : AbstractRepositoryTests<ServiceMessage, int, ServiceMessageMap>
    {
        /// <summary>
        /// Gets or sets the ServiceMessage repository.
        /// </summary>
        /// <value>The ServiceMessage repository.</value>
        public IRepository<ServiceMessage> ServiceMessageRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMessageRepositoryTests"/> class.
        /// </summary>
        public ServiceMessageRepositoryTests()
        {
            ServiceMessageRepository = new Repository<ServiceMessage>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ServiceMessage GetValid(int? counter)
        {
            return CreateValidEntities.ServiceMessage(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ServiceMessage> GetQuery(int numberAtEnd)
        {
            return ServiceMessageRepository.Queryable.Where(a => a.Message.EndsWith(numberAtEnd.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ServiceMessage entity, int counter)
        {
            Assert.AreEqual("Message" + counter, entity.Message);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ServiceMessage entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Message);
                    break;
                case ARTAction.Restore:
                    entity.Message = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Message;
                    entity.Message = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            ServiceMessageRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ServiceMessageRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Message Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Message with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestMessageWithNullValueDoesNotSave()
        {
            ServiceMessage serviceMessage = null;
            try
            {
                #region Arrange
                serviceMessage = GetValid(9);
                serviceMessage.Message = null;
                #endregion Arrange

                #region Act
                ServiceMessageRepository.DbContext.BeginTransaction();
                ServiceMessageRepository.EnsurePersistent(serviceMessage);
                ServiceMessageRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(serviceMessage);
                var results = serviceMessage.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Message"));
                Assert.IsTrue(serviceMessage.IsTransient());
                Assert.IsFalse(serviceMessage.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Message with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestMessageWithEmptyStringDoesNotSave()
        {
            ServiceMessage serviceMessage = null;
            try
            {
                #region Arrange
                serviceMessage = GetValid(9);
                serviceMessage.Message = string.Empty;
                #endregion Arrange

                #region Act
                ServiceMessageRepository.DbContext.BeginTransaction();
                ServiceMessageRepository.EnsurePersistent(serviceMessage);
                ServiceMessageRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(serviceMessage);
                var results = serviceMessage.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Message"));
                Assert.IsTrue(serviceMessage.IsTransient());
                Assert.IsFalse(serviceMessage.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Message with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestMessageWithSpacesOnlyDoesNotSave()
        {
            ServiceMessage serviceMessage = null;
            try
            {
                #region Arrange
                serviceMessage = GetValid(9);
                serviceMessage.Message = " ";
                #endregion Arrange

                #region Act
                ServiceMessageRepository.DbContext.BeginTransaction();
                ServiceMessageRepository.EnsurePersistent(serviceMessage);
                ServiceMessageRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(serviceMessage);
                var results = serviceMessage.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Message"));
                Assert.IsTrue(serviceMessage.IsTransient());
                Assert.IsFalse(serviceMessage.IsValid());
                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Message with one character saves.
        /// </summary>
        [TestMethod]
        public void TestMessageWithOneCharacterSaves()
        {
            #region Arrange
            var serviceMessage = GetValid(9);
            serviceMessage.Message = "x";
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(serviceMessage);
            ServiceMessageRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(serviceMessage.IsTransient());
            Assert.IsTrue(serviceMessage.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Message with long value saves.
        /// </summary>
        [TestMethod]
        public void TestMessageWithLongValueSaves()
        {
            #region Arrange
            var serviceMessage = GetValid(9);
            serviceMessage.Message = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(serviceMessage);
            ServiceMessageRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, serviceMessage.Message.Length);
            Assert.IsFalse(serviceMessage.IsTransient());
            Assert.IsTrue(serviceMessage.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Message Tests

        #region BeginDisplayDate Tests

        /// <summary>
        /// Tests the BeginDisplayDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestBeginDisplayDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            ServiceMessage record = GetValid(99);
            record.BeginDisplayDate = compareDate;
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(record);
            ServiceMessageRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.BeginDisplayDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the BeginDisplayDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestBeginDisplayDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.BeginDisplayDate = compareDate;
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(record);
            ServiceMessageRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.BeginDisplayDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the BeginDisplayDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestBeginDisplayDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.BeginDisplayDate = compareDate;
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(record);
            ServiceMessageRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.BeginDisplayDate);
            #endregion Assert
        }
        #endregion BeginDisplayDate Tests
        
        #region EndDisplayDate Tests

        /// <summary>
        /// Tests the EndDisplayDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestEndDisplayDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(-10);
            ServiceMessage record = GetValid(99);
            record.EndDisplayDate = compareDate;
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(record);
            ServiceMessageRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.EndDisplayDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the EndDisplayDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestEndDisplayDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime();
            var record = GetValid(99);
            record.EndDisplayDate = compareDate;
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(record);
            ServiceMessageRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.EndDisplayDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the EndDisplayDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestEndDisplayDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.UtcNow.ToPacificTime().AddDays(15);
            var record = GetValid(99);
            record.EndDisplayDate = compareDate;
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(record);
            ServiceMessageRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.EndDisplayDate);
            #endregion Assert
        }
        #endregion EndDisplayDate Tests

        #region Critical Tests

        /// <summary>
        /// Tests the Critical is false saves.
        /// </summary>
        [TestMethod]
        public void TestCriticalIsFalseSaves()
        {
            #region Arrange
            ServiceMessage serviceMessage = GetValid(9);
            serviceMessage.Critical = false;
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(serviceMessage);
            ServiceMessageRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(serviceMessage.Critical);
            Assert.IsFalse(serviceMessage.IsTransient());
            Assert.IsTrue(serviceMessage.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Critical is true saves.
        /// </summary>
        [TestMethod]
        public void TestCriticalIsTrueSaves()
        {
            #region Arrange
            var serviceMessage = GetValid(9);
            serviceMessage.Critical = true;
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(serviceMessage);
            ServiceMessageRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(serviceMessage.Critical);
            Assert.IsFalse(serviceMessage.IsTransient());
            Assert.IsTrue(serviceMessage.IsValid());
            #endregion Assert
        }

        #endregion Critical Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            ServiceMessage serviceMessage = GetValid(9);
            serviceMessage.IsActive = false;
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(serviceMessage);
            ServiceMessageRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(serviceMessage.IsActive);
            Assert.IsFalse(serviceMessage.IsTransient());
            Assert.IsTrue(serviceMessage.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var serviceMessage = GetValid(9);
            serviceMessage.IsActive = true;
            #endregion Arrange

            #region Act
            ServiceMessageRepository.DbContext.BeginTransaction();
            ServiceMessageRepository.EnsurePersistent(serviceMessage);
            ServiceMessageRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(serviceMessage.IsActive);
            Assert.IsFalse(serviceMessage.IsTransient());
            Assert.IsTrue(serviceMessage.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructor()
        {
            #region Arrange
            var record = new ServiceMessage();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual(DateTime.UtcNow.ToPacificTime().Date, record.BeginDisplayDate.Date);
            Assert.IsNull(record.EndDisplayDate);
            Assert.IsFalse(record.Critical);
            Assert.IsTrue(record.IsActive);
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
            expectedFields.Add(new NameAndType("BeginDisplayDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Critical", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("EndDisplayDate", "System.Nullable`1[System.DateTime]", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Message", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ServiceMessage));

        }

        #endregion Reflection of Database.	
		
		
    }
}