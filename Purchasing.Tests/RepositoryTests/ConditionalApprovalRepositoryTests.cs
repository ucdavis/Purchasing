using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentNHibernate.Testing;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		ConditionalApproval
    /// LookupFieldName:	Question
    /// </summary>
    [TestClass]
    public class ConditionalApprovalRepositoryTests : AbstractRepositoryTests<ConditionalApproval, int, ConditionalApprovalMap>
    {
        /// <summary>
        /// Gets or sets the ConditionalApproval repository.
        /// </summary>
        /// <value>The ConditionalApproval repository.</value>
        public IRepository<ConditionalApproval> ConditionalApprovalRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalApprovalRepositoryTests"/> class.
        /// </summary>
        public ConditionalApprovalRepositoryTests()
        {
            ConditionalApprovalRepository = new Repository<ConditionalApproval>();
            UserRepository = new RepositoryWithTypedId<User, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ConditionalApproval GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ConditionalApproval(counter);
            rtValue.PrimaryApprover = UserRepository.Queryable.Single(a => a.Id == "3");

            //These really can't be both null, but that is controlled by the controller not the db.
            rtValue.Workgroup = null;
            rtValue.Organization = null;

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ConditionalApproval> GetQuery(int numberAtEnd)
        {
            return ConditionalApprovalRepository.Queryable.Where(a => a.Question.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ConditionalApproval entity, int counter)
        {
            Assert.AreEqual("Question" + counter, entity.Question);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ConditionalApproval entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Question);
                    break;
                case ARTAction.Restore:
                    entity.Question = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Question;
                    entity.Question = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Question Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Question with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithNullValueDoesNotSave()
        {
            ConditionalApproval conditionalApproval = null;
            try
            {
                #region Arrange
                conditionalApproval = GetValid(9);
                conditionalApproval.Question = null;
                #endregion Arrange

                #region Act
                ConditionalApprovalRepository.DbContext.BeginTransaction();
                ConditionalApprovalRepository.EnsurePersistent(conditionalApproval);
                ConditionalApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(conditionalApproval);
                var results = conditionalApproval.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Question"));
                Assert.IsTrue(conditionalApproval.IsTransient());
                Assert.IsFalse(conditionalApproval.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Question with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithEmptyStringDoesNotSave()
        {
            ConditionalApproval conditionalApproval = null;
            try
            {
                #region Arrange
                conditionalApproval = GetValid(9);
                conditionalApproval.Question = string.Empty;
                #endregion Arrange

                #region Act
                ConditionalApprovalRepository.DbContext.BeginTransaction();
                ConditionalApprovalRepository.EnsurePersistent(conditionalApproval);
                ConditionalApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(conditionalApproval);
                var results = conditionalApproval.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Question"));
                Assert.IsTrue(conditionalApproval.IsTransient());
                Assert.IsFalse(conditionalApproval.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Question with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithSpacesOnlyDoesNotSave()
        {
            ConditionalApproval conditionalApproval = null;
            try
            {
                #region Arrange
                conditionalApproval = GetValid(9);
                conditionalApproval.Question = " ";
                #endregion Arrange

                #region Act
                ConditionalApprovalRepository.DbContext.BeginTransaction();
                ConditionalApprovalRepository.EnsurePersistent(conditionalApproval);
                ConditionalApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(conditionalApproval);
                var results = conditionalApproval.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Question"));
                Assert.IsTrue(conditionalApproval.IsTransient());
                Assert.IsFalse(conditionalApproval.IsValid());
                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Question with one character saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionWithOneCharacterSaves()
        {
            #region Arrange
            var conditionalApproval = GetValid(9);
            conditionalApproval.Question = "x";
            #endregion Arrange

            #region Act
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            ConditionalApprovalRepository.EnsurePersistent(conditionalApproval);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(conditionalApproval.IsTransient());
            Assert.IsTrue(conditionalApproval.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Question with long value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionWithLongValueSaves()
        {
            #region Arrange
            var conditionalApproval = GetValid(9);
            conditionalApproval.Question = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            ConditionalApprovalRepository.EnsurePersistent(conditionalApproval);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, conditionalApproval.Question.Length);
            Assert.IsFalse(conditionalApproval.IsTransient());
            Assert.IsTrue(conditionalApproval.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Question Tests

        #region PrimaryApprover Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPrimaryApproverWithAValueOfNullDoesNotSave()
        {
            ConditionalApproval record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.PrimaryApprover = null;
                #endregion Arrange

                #region Act
                ConditionalApprovalRepository.DbContext.BeginTransaction();
                ConditionalApprovalRepository.EnsurePersistent(record);
                ConditionalApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.PrimaryApprover, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The PrimaryApprover field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestNewPrimaryApproverDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.PrimaryApprover = new User();
                thisFar = true;
                #endregion Arrange

                #region Act
                ConditionalApprovalRepository.DbContext.BeginTransaction();
                ConditionalApprovalRepository.EnsurePersistent(record);
                ConditionalApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestWithNewUserDoesNotCascadeSave()
        {
            #region Arrange
            var userCount = UserRepository.Queryable.Count();
            var record = GetValid(9);
            record.PrimaryApprover = new User("NoOne");
            #endregion Arrange

            #region Act
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            ConditionalApprovalRepository.EnsurePersistent(record);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(userCount, UserRepository.Queryable.Count());
            #endregion Assert
        }
        #endregion PrimaryApprover Tests

        #region SecondaryApprover Tests

        [TestMethod]
        public void TestSecondaryApproverWithNullValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.SecondaryApprover = null;
            #endregion Arrange

            #region Act
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            ConditionalApprovalRepository.EnsurePersistent(record);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.SecondaryApprover);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert	
        }

        [TestMethod]
        public void TestSecondaryApproverWithExistingUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.SecondaryApprover = UserRepository.Queryable.Single(a => a.Id == "1");
            #endregion Arrange

            #region Act
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            ConditionalApprovalRepository.EnsurePersistent(record);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("1", record.SecondaryApprover.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestWithNewSecondaryApproverUserDoesNotCascadeSave()
        {
            #region Arrange
            var userCount = UserRepository.Queryable.Count();
            var record = GetValid(9);
            record.SecondaryApprover = new User("NoOne");
            #endregion Arrange

            #region Act
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            ConditionalApprovalRepository.EnsurePersistent(record);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(userCount, UserRepository.Queryable.Count());
            #endregion Assert
        }
        #endregion SecondaryApprover Tests

        #region Workgroup Tests

        [TestMethod]
        public void TestWorkgroupWithNullValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Workgroup = null;
            #endregion Arrange

            #region Act
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            ConditionalApprovalRepository.EnsurePersistent(record);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Workgroup);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestWorkgroupWithExistingValueSaves()
        {
            #region Arrange
            var workgroupRepository = new Repository<Workgroup>();
            workgroupRepository.DbContext.BeginTransaction();
            LoadOrganizations(3);
            LoadWorkgroups(3);
            workgroupRepository.DbContext.CommitTransaction();

            var record = GetValid(9);
            record.Workgroup = workgroupRepository.Queryable.Single(a => a.Id == 2);
            #endregion Arrange

            #region Act
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            ConditionalApprovalRepository.EnsurePersistent(record);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, record.Workgroup.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof (NHibernate.TransientObjectException))]
        public void TestWorkgroupWithNewValueDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Workgroup = CreateValidEntities.Workgroup(99);
                thisFar = true;
                #endregion Arrange

                #region Act
                ConditionalApprovalRepository.DbContext.BeginTransaction();
                ConditionalApprovalRepository.EnsurePersistent(record);
                ConditionalApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Workgroup, Entity: Purchasing.Core.Domain.Workgroup", ex.Message);
                throw;
            }
        }
        #endregion Workgroup Tests

        #region Workgroup Tests

        [TestMethod]
        public void TestOrganizationWithNullValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Organization = null;
            #endregion Arrange

            #region Act
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            ConditionalApprovalRepository.EnsurePersistent(record);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Organization);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestOrganizationWithExistingValueSaves()
        {
            #region Arrange
            var organizationRepository = new RepositoryWithTypedId<Organization, string>();
            organizationRepository.DbContext.BeginTransaction();
            LoadOrganizations(3);
            organizationRepository.DbContext.CommitTransaction();

            var record = GetValid(9);
            record.Organization = organizationRepository.Queryable.Single(a => a.Id == "2");
            #endregion Arrange

            #region Act
            ConditionalApprovalRepository.DbContext.BeginTransaction();
            ConditionalApprovalRepository.EnsurePersistent(record);
            ConditionalApprovalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("2", record.Organization.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestOrganizationWithNewValueDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Organization = CreateValidEntities.Organization(99);
                thisFar = true;
                #endregion Arrange

                #region Act
                ConditionalApprovalRepository.DbContext.BeginTransaction();
                ConditionalApprovalRepository.EnsurePersistent(record);
                ConditionalApprovalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Organization, Entity: Purchasing.Core.Domain.Organization", ex.Message);
                throw;
            }
        }
        #endregion Organization Tests

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
            expectedFields.Add(new NameAndType("Organization", "Purchasing.Core.Domain.Organization", new List<string>()));
            expectedFields.Add(new NameAndType("PrimaryApprover", "Purchasing.Core.Domain.User", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Question", "System.String", new List<string>
            {                 
                 "[System.ComponentModel.DataAnnotations.DataTypeAttribute((System.ComponentModel.DataAnnotations.DataType)9)]",
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("SecondaryApprover", "Purchasing.Core.Domain.User", new List<string>()));
            expectedFields.Add(new NameAndType("Workgroup", "Purchasing.Core.Domain.Workgroup", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ConditionalApproval));

        }

        #endregion Reflection of Database.	
		
		
    }
}