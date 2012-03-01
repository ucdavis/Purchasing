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
    /// Entity Name:		Faq
    /// LookupFieldName:	Question
    /// </summary>
    [TestClass]
    public class FaqRepositoryTests : AbstractRepositoryTests<Faq, int, FaqMap>
    {
        /// <summary>
        /// Gets or sets the Faq repository.
        /// </summary>
        /// <value>The Faq repository.</value>
        public IRepository<Faq> FaqRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="FaqRepositoryTests"/> class.
        /// </summary>
        public FaqRepositoryTests()
        {
            FaqRepository = new Repository<Faq>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Faq GetValid(int? counter)
        {
            return CreateValidEntities.Faq(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Faq> GetQuery(int numberAtEnd)
        {
            return FaqRepository.Queryable.Where(a => a.Question.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Faq entity, int counter)
        {
            Assert.AreEqual("Question" + counter, entity.Question);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Faq entity, ARTAction action)
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
            FaqRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            FaqRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region Category Tests

        [TestMethod]
        public void TestCategorySaves1()
        {
            #region Arrange
            var record = GetValid(9);
            record.Category = FaqCategory.Approver;
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(record);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(record.Category, FaqCategory.Approver);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCategorySaves2()
        {
            #region Arrange
            var record = GetValid(9);
            record.Category = FaqCategory.Requester;
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(record);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(record.Category, FaqCategory.Requester);
            #endregion Assert
        }

        [TestMethod]
        public void TestCategorySaves3()
        {
            #region Arrange
            var record = GetValid(9);
            record.Category = FaqCategory.AccountManager;
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(record);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(record.Category, FaqCategory.AccountManager);
            #endregion Assert
        }

        [TestMethod]
        public void TestCategorySaves4()
        {
            #region Arrange
            var record = GetValid(9);
            record.Category = FaqCategory.ConditionalApprover;
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(record);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(record.Category, FaqCategory.ConditionalApprover);
            #endregion Assert
        }

        [TestMethod]
        public void TestCategorySaves5()
        {
            #region Arrange
            var record = GetValid(9);
            record.Category = FaqCategory.General;
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(record);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(record.Category, FaqCategory.General);
            #endregion Assert
        }

        [TestMethod]
        public void TestCategorySaves6()
        {
            #region Arrange
            var record = GetValid(9);
            record.Category = FaqCategory.Purchaser;
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(record);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(record.Category, FaqCategory.Purchaser);
            #endregion Assert
        }

        #endregion Category Tests

        #region Question Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Question with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithNullValueDoesNotSave()
        {
            Faq faq = null;
            try
            {
                #region Arrange
                faq = GetValid(9);
                faq.Question = null;
                #endregion Arrange

                #region Act
                FaqRepository.DbContext.BeginTransaction();
                FaqRepository.EnsurePersistent(faq);
                FaqRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(faq);
                var results = faq.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Question"));
                Assert.IsTrue(faq.IsTransient());
                Assert.IsFalse(faq.IsValid());
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
            Faq faq = null;
            try
            {
                #region Arrange
                faq = GetValid(9);
                faq.Question = string.Empty;
                #endregion Arrange

                #region Act
                FaqRepository.DbContext.BeginTransaction();
                FaqRepository.EnsurePersistent(faq);
                FaqRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(faq);
                var results = faq.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Question"));
                Assert.IsTrue(faq.IsTransient());
                Assert.IsFalse(faq.IsValid());
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
            Faq faq = null;
            try
            {
                #region Arrange
                faq = GetValid(9);
                faq.Question = " ";
                #endregion Arrange

                #region Act
                FaqRepository.DbContext.BeginTransaction();
                FaqRepository.EnsurePersistent(faq);
                FaqRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(faq);
                var results = faq.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Question"));
                Assert.IsTrue(faq.IsTransient());
                Assert.IsFalse(faq.IsValid());
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
            var faq = GetValid(9);
            faq.Question = "x";
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(faq);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(faq.IsTransient());
            Assert.IsTrue(faq.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Question with long value saves.
        /// </summary>
        [TestMethod]
        public void TestQuestionWithLongValueSaves()
        {
            #region Arrange
            var faq = GetValid(9);
            faq.Question = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(faq);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, faq.Question.Length);
            Assert.IsFalse(faq.IsTransient());
            Assert.IsTrue(faq.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Question Tests

        #region Answer Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Answer with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswerWithNullValueDoesNotSave()
        {
            Faq faq = null;
            try
            {
                #region Arrange
                faq = GetValid(9);
                faq.Answer = null;
                #endregion Arrange

                #region Act
                FaqRepository.DbContext.BeginTransaction();
                FaqRepository.EnsurePersistent(faq);
                FaqRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(faq);
                var results = faq.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Answer"));
                Assert.IsTrue(faq.IsTransient());
                Assert.IsFalse(faq.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Answer with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswerWithEmptyStringDoesNotSave()
        {
            Faq faq = null;
            try
            {
                #region Arrange
                faq = GetValid(9);
                faq.Answer = string.Empty;
                #endregion Arrange

                #region Act
                FaqRepository.DbContext.BeginTransaction();
                FaqRepository.EnsurePersistent(faq);
                FaqRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(faq);
                var results = faq.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Answer"));
                Assert.IsTrue(faq.IsTransient());
                Assert.IsFalse(faq.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Answer with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswerWithSpacesOnlyDoesNotSave()
        {
            Faq faq = null;
            try
            {
                #region Arrange
                faq = GetValid(9);
                faq.Answer = " ";
                #endregion Arrange

                #region Act
                FaqRepository.DbContext.BeginTransaction();
                FaqRepository.EnsurePersistent(faq);
                FaqRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(faq);
                var results = faq.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Answer"));
                Assert.IsTrue(faq.IsTransient());
                Assert.IsFalse(faq.IsValid());
                throw;
            }
        }


        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Answer with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithOneCharacterSaves()
        {
            #region Arrange
            var faq = GetValid(9);
            faq.Answer = "x";
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(faq);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(faq.IsTransient());
            Assert.IsTrue(faq.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithLongValueSaves()
        {
            #region Arrange
            var faq = GetValid(9);
            faq.Answer = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(faq);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, faq.Answer.Length);
            Assert.IsFalse(faq.IsTransient());
            Assert.IsTrue(faq.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Answer Tests

        #region OrgId Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the OrgId with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOrgIdWithTooLongValueDoesNotSave()
        {
            Faq faq = null;
            try
            {
                #region Arrange
                faq = GetValid(9);
                faq.OrgId = "x".RepeatTimes((10 + 1));
                #endregion Arrange

                #region Act
                FaqRepository.DbContext.BeginTransaction();
                FaqRepository.EnsurePersistent(faq);
                FaqRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(faq);
                Assert.AreEqual(10 + 1, faq.OrgId.Length);
                var results = faq.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "OrgId", "10"));
                Assert.IsTrue(faq.IsTransient());
                Assert.IsFalse(faq.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the OrgId with null value saves.
        /// </summary>
        [TestMethod]
        public void TestOrgIdWithNullValueSaves()
        {
            #region Arrange
            var faq = GetValid(9);
            faq.OrgId = null;
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(faq);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(faq.IsTransient());
            Assert.IsTrue(faq.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the OrgId with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestOrgIdWithEmptyStringSaves()
        {
            #region Arrange
            var faq = GetValid(9);
            faq.OrgId = string.Empty;
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(faq);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(faq.IsTransient());
            Assert.IsTrue(faq.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the OrgId with one space saves.
        /// </summary>
        [TestMethod]
        public void TestOrgIdWithOneSpaceSaves()
        {
            #region Arrange
            var faq = GetValid(9);
            faq.OrgId = " ";
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(faq);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(faq.IsTransient());
            Assert.IsTrue(faq.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the OrgId with one character saves.
        /// </summary>
        [TestMethod]
        public void TestOrgIdWithOneCharacterSaves()
        {
            #region Arrange
            var faq = GetValid(9);
            faq.OrgId = "x";
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(faq);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(faq.IsTransient());
            Assert.IsTrue(faq.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the OrgId with long value saves.
        /// </summary>
        [TestMethod]
        public void TestOrgIdWithLongValueSaves()
        {
            #region Arrange
            var faq = GetValid(9);
            faq.OrgId = "x".RepeatTimes(10);
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(faq);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, faq.OrgId.Length);
            Assert.IsFalse(faq.IsTransient());
            Assert.IsTrue(faq.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion OrgId Tests

        #region Like Tests

  

        /// <summary>
        /// Tests the Like with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestLikeWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Like = int.MaxValue;
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(record);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.Like);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Like with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestLikeWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Like = int.MinValue;
            #endregion Arrange

            #region Act
            FaqRepository.DbContext.BeginTransaction();
            FaqRepository.EnsurePersistent(record);
            FaqRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.Like);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion Like Tests
        

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
            expectedFields.Add(new NameAndType("Answer", "System.String", new List<string>
            {                 
                 "[System.ComponentModel.DataAnnotations.DataTypeAttribute((System.ComponentModel.DataAnnotations.DataType)9)]",
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Category", "Purchasing.Core.Domain.FaqCategory", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Like", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("OrgId", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]"
            }));
            expectedFields.Add(new NameAndType("Question", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Faq));

        }

        #endregion Reflection of Database.	
		
		
    }
}