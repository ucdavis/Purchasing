using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Organization
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class OrganizationRepositoryTests : AbstractRepositoryTests<Organization, string, OrganizationMap>
    {
        /// <summary>
        /// Gets or sets the Organization repository.
        /// </summary>
        /// <value>The Organization repository.</value>
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationRepositoryTests"/> class.
        /// </summary>
        public OrganizationRepositoryTests()
        {
            OrganizationRepository = new RepositoryWithTypedId<Organization, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Organization GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Organization(counter);
            rtValue.SetIdTo(counter.HasValue ? counter.Value.ToString() : "99");
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Organization> GetQuery(int numberAtEnd)
        {
            return OrganizationRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Organization entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Organization entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Name);
                    break;
                case ARTAction.Restore:
                    entity.Name = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Name;
                    entity.Name = updateValue;
                    break;
            }
        }

        [TestMethod]
        public override void CanUpdateEntity()
        {
            CanUpdateEntity(false);
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            OrganizationRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            OrganizationRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.Name = null;
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                var results = organization.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.Name = string.Empty;
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                var results = organization.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.Name = " ";
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                var results = organization.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.Name = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                Assert.AreEqual(50 + 1, organization.Name.Length);
                var results = organization.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "50"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var organization = GetValid(9);
            organization.Name = "x";
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(organization);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(organization.IsTransient());
            Assert.IsTrue(organization.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var organization = GetValid(9);
            organization.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(organization);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, organization.Name.Length);
            Assert.IsFalse(organization.IsTransient());
            Assert.IsTrue(organization.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region TypeCode Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the TypeCode with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeCodeWithNullValueDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.TypeCode = null;
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                var results = organization.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "TypeCode"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TypeCode with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeCodeWithEmptyStringDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.TypeCode = string.Empty;
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                var results = organization.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "TypeCode"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TypeCode with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeCodeWithSpacesOnlyDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.TypeCode = " ";
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                var results = organization.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "TypeCode"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TypeCode with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeCodeWithTooLongValueDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.TypeCode = "x".RepeatTimes((1 + 1));
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                Assert.AreEqual(1 + 1, organization.TypeCode.Length);
                var results = organization.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "TypeCode", "1"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the TypeCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestTypeCodeWithOneCharacterSaves()
        {
            #region Arrange
            var organization = GetValid(9);
            organization.TypeCode = "x";
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(organization);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(organization.IsTransient());
            Assert.IsTrue(organization.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the TypeCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTypeCodeWithLongValueSaves()
        {
            #region Arrange
            var organization = GetValid(9);
            organization.TypeCode = "x".RepeatTimes(1);
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(organization);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, organization.TypeCode.Length);
            Assert.IsFalse(organization.IsTransient());
            Assert.IsTrue(organization.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion TypeCode Tests

        #region TypeName Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the TypeName with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeNameWithNullValueDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.TypeName = null;
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                var results = organization.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "TypeName"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TypeName with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeNameWithEmptyStringDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.TypeName = string.Empty;
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                var results = organization.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "TypeName"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TypeName with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeNameWithSpacesOnlyDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.TypeName = " ";
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                var results = organization.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "TypeName"));
                //Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TypeName with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTypeNameWithTooLongValueDoesNotSave()
        {
            Organization organization = null;
            try
            {
                #region Arrange
                organization = GetValid(9);
                organization.TypeName = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(organization);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(organization);
                Assert.AreEqual(50 + 1, organization.TypeName.Length);
                var results = organization.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "TypeName", "50"));
               // Assert.IsTrue(organization.IsTransient());
                Assert.IsFalse(organization.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the TypeName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestTypeNameWithOneCharacterSaves()
        {
            #region Arrange
            var organization = GetValid(9);
            organization.TypeName = "x";
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(organization);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(organization.IsTransient());
            Assert.IsTrue(organization.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the TypeName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTypeNameWithLongValueSaves()
        {
            #region Arrange
            var organization = GetValid(9);
            organization.TypeName = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(organization);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, organization.TypeName.Length);
            Assert.IsFalse(organization.IsTransient());
            Assert.IsTrue(organization.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion TypeName Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            Organization organization = GetValid(9);
            organization.IsActive = false;
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(organization);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(organization.IsActive);
            Assert.IsFalse(organization.IsTransient());
            Assert.IsTrue(organization.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var organization = GetValid(9);
            organization.IsActive = true;
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(organization);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(organization.IsActive);
            Assert.IsFalse(organization.IsTransient());
            Assert.IsTrue(organization.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region Parent Tests

        [TestMethod]
        public void TestOrganizationWithExistingParentSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Parent = OrganizationRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.Parent.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestOrganizationWithNullParentSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Parent = null;
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.Parent);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Parent Tests

   
        #region Accounts Tests //Can't really test these because the Account Map doen't have the organization (table does)
        #region Invalid Tests



        #endregion Invalid Tests
        #region Valid Tests


        [TestMethod]
        public void TestAccountsWithEmptyListWillSave()
        {
            #region Arrange
            Organization record = GetValid(9);
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Accounts);
            Assert.AreEqual(0, record.Accounts.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests


        #endregion Accounts Tests

        #region ConditionalApprovals Tests
        #region Invalid Tests


        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestConditionalApprovalsWithPopulatedListWillSave()
        {
            #region Arrange
            var userRepository = new RepositoryWithTypedId<User, string>();
            userRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            LoadWorkgroups(3);
            userRepository.DbContext.CommitTransaction();
            Organization record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.ConditionalApprovals.Add(CreateValidEntities.ConditionalApproval(i+1));
                record.ConditionalApprovals[i].Workgroup = Repository.OfType<Workgroup>().Queryable.First();
                record.ConditionalApprovals[i].PrimaryApprover = userRepository.Queryable.First();
            }
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.ConditionalApprovals);
            Assert.AreEqual(addedCount, record.ConditionalApprovals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestConditionalApprovalsWithPopulatedExistingListWillSave()
        {
            
            #region Arrange
            var userRepository = new RepositoryWithTypedId<User, string>();
            userRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            LoadWorkgroups(3);
            userRepository.DbContext.CommitTransaction();
            Organization record = GetValid(9);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<ConditionalApproval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.ConditionalApproval(i + 1));
                relatedRecords[i].Organization = record;
                relatedRecords[i].Workgroup = Repository.OfType<Workgroup>().Queryable.First();
                relatedRecords[i].PrimaryApprover = userRepository.Queryable.First();
                Repository.OfType<ConditionalApproval>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.ConditionalApprovals.Add(relatedRecord);
            }
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.ConditionalApprovals);
            Assert.AreEqual(addedCount, record.ConditionalApprovals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestConditionalApprovalsWithEmptyListWillSave()
        {
            #region Arrange
            Organization record = GetValid(9);
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.ConditionalApprovals);
            Assert.AreEqual(0, record.ConditionalApprovals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        //This is a readonly table, this one would not pass
        //[TestMethod]
        //public void TestOrganizationCascadesSaveToConditionalApproval()
        //{
        //    #region Arrange
        //    var userRepository = new RepositoryWithTypedId<User, string>();
        //    userRepository.DbContext.BeginTransaction();
        //    LoadUsers(3);
        //    LoadWorkgroups(3);
        //    userRepository.DbContext.CommitTransaction();
        //    var count = Repository.OfType<ConditionalApproval>().Queryable.Count();
        //    Organization record = GetValid(9);
        //    const int addedCount = 3;
        //    for (int i = 0; i < addedCount; i++)
        //    {
        //        record.ConditionalApprovals.Add(CreateValidEntities.ConditionalApproval(i+1));
        //        record.ConditionalApprovals[i].Workgroup = Repository.OfType<Workgroup>().Queryable.First();
        //        record.ConditionalApprovals[i].PrimaryApprover = userRepository.Queryable.First();
        //    }

        //    OrganizationRepository.DbContext.BeginTransaction();
        //    OrganizationRepository.EnsurePersistent(record);
        //    OrganizationRepository.DbContext.CommitTransaction();
        //    var saveId = record.Id;
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    #endregion Arrange

        //    #region Act
        //    record = OrganizationRepository.GetNullableById(saveId);
        //    #endregion Act

        //    #region Assert
        //    Assert.IsNotNull(record);
        //    Assert.AreEqual(addedCount, record.ConditionalApprovals.Count);
        //    Assert.AreEqual(count + addedCount, Repository.OfType<ConditionalApproval>().Queryable.Count());
        //    #endregion Assert
        //}


        //[TestMethod]
        //public void TestOrganizationCascadesUpdateToConditionalApproval1()
        //{
        //    #region Arrange
        //    var userRepository = new RepositoryWithTypedId<User, string>();
        //    userRepository.DbContext.BeginTransaction();
        //    LoadUsers(3);
        //    LoadWorkgroups(3);
        //    userRepository.DbContext.CommitTransaction();
        //    var count = Repository.OfType<ConditionalApproval>().Queryable.Count();
        //    Organization record = GetValid(9);
        //    const int addedCount = 3;
        //    for (int i = 0; i < addedCount; i++)
        //    {
        //        record.ConditionalApprovals.Add(CreateValidEntities.ConditionalApproval(i+1));
        //        record.ConditionalApprovals[i].Workgroup = Repository.OfType<Workgroup>().Queryable.First();
        //        record.ConditionalApprovals[i].PrimaryApprover = userRepository.Queryable.First();
        //    }

        //    OrganizationRepository.DbContext.BeginTransaction();
        //    OrganizationRepository.EnsurePersistent(record);
        //    OrganizationRepository.DbContext.CommitTransaction();
        //    var saveId = record.Id;
        //    var saveRelatedId = record.ConditionalApprovals[1].Id;
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    #endregion Arrange

        //    #region Act
        //    record = OrganizationRepository.GetNullableById(saveId);
        //    record.ConditionalApprovals[1].Question = "Updated";
        //    OrganizationRepository.DbContext.BeginTransaction();
        //    OrganizationRepository.EnsurePersistent(record);
        //    OrganizationRepository.DbContext.CommitTransaction();
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(count + addedCount, Repository.OfType<ConditionalApproval>().Queryable.Count());
        //    var relatedRecord = Repository.OfType<ConditionalApproval>().GetNullableById(saveRelatedId);
        //    Assert.IsNotNull(relatedRecord);
        //    Assert.AreEqual("Updated", relatedRecord.Question);
        //    #endregion Assert
        //}

        [TestMethod]
        public void TestOrganizationCascadesUpdateToConditionalApproval2()
        {
            #region Arrange
            var userRepository = new RepositoryWithTypedId<User, string>();
            userRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            LoadWorkgroups(3);
            userRepository.DbContext.CommitTransaction();
            var count = Repository.OfType<ConditionalApproval>().Queryable.Count();
            Organization record = GetValid(9);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<ConditionalApproval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.ConditionalApproval(i + 1));
                relatedRecords[i].Organization = record;
                relatedRecords[i].Workgroup = Repository.OfType<Workgroup>().Queryable.First();
                relatedRecords[i].PrimaryApprover = userRepository.Queryable.First();
                Repository.OfType<ConditionalApproval>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.ConditionalApprovals.Add(relatedRecord);
            }
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.ConditionalApprovals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrganizationRepository.GetNullableById(saveId);
            record.ConditionalApprovals[1].Question = "Updated";
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<ConditionalApproval>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<ConditionalApproval>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Question);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it (Delete this test, or the one below)
        /// </summary>
        [TestMethod]
        public void TestOrganizationCascadesUpdateRemoveConditionalApproval()
        {
            #region Arrange
            var userRepository = new RepositoryWithTypedId<User, string>();
            userRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            LoadWorkgroups(3);
            userRepository.DbContext.CommitTransaction();
            var count = Repository.OfType<ConditionalApproval>().Queryable.Count();
            Organization record = GetValid(9);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<ConditionalApproval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.ConditionalApproval(i + 1));
                relatedRecords[i].Organization = record;
                relatedRecords[i].Workgroup = Repository.OfType<Workgroup>().Queryable.First();
                relatedRecords[i].PrimaryApprover = userRepository.Queryable.First();
                Repository.OfType<ConditionalApproval>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.ConditionalApprovals.Add(relatedRecord);
            }
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.ConditionalApprovals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrganizationRepository.GetNullableById(saveId);
            record.ConditionalApprovals.RemoveAt(1);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<ConditionalApproval>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<ConditionalApproval>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }

        [TestMethod]
        public void TestOrganizationCascadesDeleteToConditionalApproval()
        {
            #region Arrange
            var userRepository = new RepositoryWithTypedId<User, string>();
            userRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            LoadWorkgroups(3);
            userRepository.DbContext.CommitTransaction();
            var count = Repository.OfType<ConditionalApproval>().Queryable.Count();
            Organization record = GetValid(9);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<ConditionalApproval>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.ConditionalApproval(i + 1));
                relatedRecords[i].Organization = record;
                relatedRecords[i].Workgroup = Repository.OfType<Workgroup>().Queryable.First();
                relatedRecords[i].PrimaryApprover = userRepository.Queryable.First();
                Repository.OfType<ConditionalApproval>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.ConditionalApprovals.Add(relatedRecord);
            }
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.ConditionalApprovals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrganizationRepository.GetNullableById(saveId);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.Remove(record);
            OrganizationRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<ConditionalApproval>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<ConditionalApproval>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests


        #endregion ConditionalApprovals Tests

        #region Workgroups Tests
        #region Invalid Tests


        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestWorkgroupsWithANewValueDoesNotSave()
        {
            Organization record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Workgroups.Add(CreateValidEntities.Workgroup(1));
                #endregion Arrange

                #region Act
                OrganizationRepository.DbContext.BeginTransaction();
                OrganizationRepository.EnsurePersistent(record);
                OrganizationRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Workgroup, Entity: Purchasing.Core.Domain.Workgroup", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests


        [TestMethod]
        public void TestWorkgroupsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Organization record = GetValid(9);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Workgroup>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Workgroup(i + 1));
                relatedRecords[i].PrimaryOrganization = record;
                Repository.OfType<Workgroup>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Workgroups.Add(relatedRecord);
            }
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Workgroups);
            Assert.AreEqual(addedCount, record.Workgroups.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestWorkgroupsWithEmptyListWillSave()
        {
            #region Arrange
            Organization record = GetValid(9);
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Workgroups);
            Assert.AreEqual(0, record.Workgroups.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests



        [TestMethod]
        public void TestOrganizationCascadesUpdateToWorkgroup2()
        {
            #region Arrange
            var count = Repository.OfType<Workgroup>().Queryable.Count();
            Organization record = GetValid(9);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Workgroup>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Workgroup(i + 1));
                relatedRecords[i].PrimaryOrganization = record;
                Repository.OfType<Workgroup>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Workgroups.Add(relatedRecord);
            }
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Workgroups[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrganizationRepository.GetNullableById(saveId);
            record.Workgroups[1].Name = "Updated";
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Workgroup>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Workgroup>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Name);
            #endregion Assert
        }


        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestOrganizationDoesNotCascadesUpdateRemoveWorkgroup()
        {
            #region Arrange
            var count = Repository.OfType<Workgroup>().Queryable.Count();
            Organization record = GetValid(9);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Workgroup>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Workgroup(i + 1));
                relatedRecords[i].PrimaryOrganization = record;
                Repository.OfType<Workgroup>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Workgroups.Add(relatedRecord);
            }
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Workgroups[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrganizationRepository.GetNullableById(saveId);
            record.Workgroups.RemoveAt(1);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<Workgroup>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Workgroup>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }



        #endregion Cascade Tests
        #endregion Workgroups Tests

        #region CustomFields Tests

        #region Valid Tests
        [TestMethod]
        public void TestCustomFieldsWithPopulatedListWillSave()
        {
            #region Arrange
            Organization record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.CustomFields.Add(CreateValidEntities.CustomField(i+1));
                record.CustomFields[i].Organization = record;
            }
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CustomFields);
            Assert.AreEqual(addedCount, record.CustomFields.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCustomFieldsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Organization record = GetValid(9);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<CustomField>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.CustomField(i + 1));
                relatedRecords[i].Organization = record;
                Repository.OfType<CustomField>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.CustomFields.Add(relatedRecord);
            }
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CustomFields);
            Assert.AreEqual(addedCount, record.CustomFields.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCustomFieldsWithEmptyListWillSave()
        {
            #region Arrange
            Organization record = GetValid(9);
            #endregion Arrange

            #region Act
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CustomFields);
            Assert.AreEqual(0, record.CustomFields.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        //Read only table.
        //[TestMethod]
        //public void TestOrganizationCascadesSaveToCustomField()
        //{
        //    #region Arrange
        //    var count = Repository.OfType<CustomField>().Queryable.Count();
        //    Organization record = GetValid(9);
        //    const int addedCount = 3;
        //    for (int i = 0; i < addedCount; i++)
        //    {
        //        record.CustomFields.Add(CreateValidEntities.CustomField(i+1));
        //    }

        //    OrganizationRepository.DbContext.BeginTransaction();
        //    OrganizationRepository.EnsurePersistent(record);
        //    OrganizationRepository.DbContext.CommitTransaction();
        //    var saveId = record.Id;
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    #endregion Arrange

        //    #region Act
        //    record = OrganizationRepository.GetNullableById(saveId);
        //    #endregion Act

        //    #region Assert
        //    Assert.IsNotNull(record);
        //    Assert.AreEqual(addedCount, record.CustomFields.Count);
        //    Assert.AreEqual(count + addedCount, Repository.OfType<CustomField>().Queryable.Count());
        //    #endregion Assert
        //}


        //[TestMethod]
        //public void TestOrganizationCascadesUpdateToCustomField1()
        //{
        //    #region Arrange
        //    var count = Repository.OfType<CustomField>().Queryable.Count();
        //    Organization record = GetValid(9);
        //    const int addedCount = 3;
        //    for (int i = 0; i < addedCount; i++)
        //    {
        //        record.CustomFields.Add(CreateValidEntities.CustomField(i+1));
        //    }

        //    OrganizationRepository.DbContext.BeginTransaction();
        //    OrganizationRepository.EnsurePersistent(record);
        //    OrganizationRepository.DbContext.CommitTransaction();
        //    var saveId = record.Id;
        //    var saveRelatedId = record.CustomFields[1].Id;
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    #endregion Arrange

        //    #region Act
        //    record = OrganizationRepository.GetNullableById(saveId);
        //    record.CustomFields[1].Name = "Updated";
        //    OrganizationRepository.DbContext.BeginTransaction();
        //    OrganizationRepository.EnsurePersistent(record);
        //    OrganizationRepository.DbContext.CommitTransaction();
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(count + addedCount, Repository.OfType<CustomField>().Queryable.Count());
        //    var relatedRecord = Repository.OfType<CustomField>().GetNullableById(saveRelatedId);
        //    Assert.IsNotNull(relatedRecord);
        //    Assert.AreEqual("Updated", relatedRecord.Name);
        //    #endregion Assert
        //}

        [TestMethod]
        public void TestOrganizationCascadesUpdateToCustomField2()
        {
            #region Arrange
            var count = Repository.OfType<CustomField>().Queryable.Count();
            Organization record = GetValid(9);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<CustomField>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.CustomField(i + 1));
                relatedRecords[i].Organization = record;
                Repository.OfType<CustomField>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.CustomFields.Add(relatedRecord);
            }
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CustomFields[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = OrganizationRepository.GetNullableById(saveId);
            record.CustomFields[1].Name = "Updated";
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<CustomField>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<CustomField>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Name);
            #endregion Assert
        }


        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestOrganizationDoesNotCascadesUpdateRemoveCustomField()
        {
            #region Arrange
            var count = Repository.OfType<CustomField>().Queryable.Count();
            Organization record = GetValid(9);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<CustomField>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.CustomField(i + 1));
                relatedRecords[i].Organization = record;
                Repository.OfType<CustomField>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.CustomFields.Add(relatedRecord);
            }
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CustomFields[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = OrganizationRepository.GetNullableById(saveId);
            record.CustomFields.RemoveAt(1);
            OrganizationRepository.DbContext.BeginTransaction();
            OrganizationRepository.EnsurePersistent(record);
            OrganizationRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<CustomField>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<CustomField>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }

		


        #endregion Cascade Tests

        #endregion CustomFields Tests



        
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
            expectedFields.Add(new NameAndType("Accounts", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Account]", new List<string>()));
            expectedFields.Add(new NameAndType("ConditionalApprovals", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.ConditionalApproval]", new List<string>()));
            expectedFields.Add(new NameAndType("CustomFields", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.CustomField]", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Parent", "Purchasing.Core.Domain.Organization", new List<string>()));
            expectedFields.Add(new NameAndType("TypeCode", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)1)]"
            }));
            expectedFields.Add(new NameAndType("TypeName", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Workgroups", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.Workgroup]", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Organization));

        }

        #endregion Reflection of Database.	
		
		
    }
}