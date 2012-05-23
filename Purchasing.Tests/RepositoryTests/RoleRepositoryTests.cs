using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    /// Entity Name:		Role
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class RoleRepositoryTests : AbstractRepositoryTests<Role, string, RoleMap>
    {
        /// <summary>
        /// Gets or sets the Role repository.
        /// </summary>
        /// <value>The Role repository.</value>
        public IRepositoryWithTypedId<Role, string> RoleRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRepositoryTests"/> class.
        /// </summary>
        public RoleRepositoryTests()
        {
            RoleRepository = new RepositoryWithTypedId<Role, string>();
            UserRepository = new RepositoryWithTypedId<User, string>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Role GetValid(int? counter)
        {
            return CreateValidEntities.Role(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Role> GetQuery(int numberAtEnd)
        {
            return RoleRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Role entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Role entity, ARTAction action)
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

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            RoleRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            RoleRepository.DbContext.CommitTransaction();
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
            Role role = null;
            try
            {
                #region Arrange
                role = GetValid(9);
                role.Name = null;
                #endregion Arrange

                #region Act
                RoleRepository.DbContext.BeginTransaction();
                RoleRepository.EnsurePersistent(role);
                RoleRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(role);
                var results = role.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(role.IsTransient());
                Assert.IsFalse(role.IsValid());
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
            Role role = null;
            try
            {
                #region Arrange
                role = GetValid(9);
                role.Name = string.Empty;
                #endregion Arrange

                #region Act
                RoleRepository.DbContext.BeginTransaction();
                RoleRepository.EnsurePersistent(role);
                RoleRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(role);
                var results = role.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(role.IsTransient());
                Assert.IsFalse(role.IsValid());
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
            Role role = null;
            try
            {
                #region Arrange
                role = GetValid(9);
                role.Name = " ";
                #endregion Arrange

                #region Act
                RoleRepository.DbContext.BeginTransaction();
                RoleRepository.EnsurePersistent(role);
                RoleRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(role);
                var results = role.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(role.IsTransient());
                Assert.IsFalse(role.IsValid());
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
            Role role = null;
            try
            {
                #region Arrange
                role = GetValid(9);
                role.Name = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                RoleRepository.DbContext.BeginTransaction();
                RoleRepository.EnsurePersistent(role);
                RoleRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(role);
                Assert.AreEqual(50 + 1, role.Name.Length);
                var results = role.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "50"));
                //Assert.IsTrue(role.IsTransient());
                Assert.IsFalse(role.IsValid());
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
            var role = GetValid(9);
            role.Name = "x";
            #endregion Arrange

            #region Act
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(role);
            RoleRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(role.IsTransient());
            Assert.IsTrue(role.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var role = GetValid(9);
            role.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(role);
            RoleRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, role.Name.Length);
            Assert.IsFalse(role.IsTransient());
            Assert.IsTrue(role.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Level Tests


        /// <summary>
        /// Tests the Level with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestLevelWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Level = int.MaxValue;
            #endregion Arrange

            #region Act
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.Level);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Level with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestLevelWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Level = int.MinValue;
            #endregion Arrange

            #region Act
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.Level);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion Level Tests

        #region Users Tests
        #region Invalid Tests


        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestUsersWithPopulatedListWillSave()
        {
            #region Arrange
            UserRepository.DbContext.BeginTransaction();
            for (int i = 0; i < 3; i++)
            {
                var user = CreateValidEntities.User(i + 1);
                UserRepository.EnsurePersistent(user);
            }
            UserRepository.DbContext.CommitTransaction();
            Role record = GetValid(9);
            const int addedCount = 3;
            for (int i = 0; i < addedCount; i++)
            {
                record.Users.Add(UserRepository.Queryable.Single(a => a.Id == (i+1).ToString(CultureInfo.InvariantCulture)));
            }
            #endregion Arrange

            #region Act
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Users);
            Assert.AreEqual(addedCount, record.Users.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestUsersWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Role record = GetValid(9);
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<User>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.User(i + 1));
                relatedRecords[i].Roles.Add(record);
                UserRepository.EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Users.Add(relatedRecord);
            }
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Users);
            Assert.AreEqual(addedCount, record.Users.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestUsersWithEmptyListWillSave()
        {
            #region Arrange
            Role record = GetValid(9);
            #endregion Arrange

            #region Act
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Users);
            Assert.AreEqual(0, record.Users.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestRoleCascadesUpdateToUser2()
        {
            #region Arrange
            var count = UserRepository.Queryable.Count();
            Role record = GetValid(9);
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<User>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.User(i + 1));
                relatedRecords[i].Roles.Add(record);
                UserRepository.EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Users.Add(relatedRecord);
            }
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Users[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = RoleRepository.GetNullableById(saveId);
            record.Users[1].FirstName = "Updated";
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, UserRepository.Queryable.Count());
            var relatedRecord2 = UserRepository.GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.FirstName);
            #endregion Assert
        }


        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestRoleDoesNotCascadesUpdateRemoveUser()
        {
            #region Arrange
            var count = UserRepository.Queryable.Count();
            Role record = GetValid(9);
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<User>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.User(i + 1));
                relatedRecords[i].Roles.Add(record);
                UserRepository.EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Users.Add(relatedRecord);
            }
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Users[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = RoleRepository.GetNullableById(saveId);
            record.Users.RemoveAt(1);
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(record);
            RoleRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), UserRepository.Queryable.Count());
            var relatedRecord2 = UserRepository.GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }




        #endregion Cascade Tests


        #endregion Users Tests


        #region IsAdmin Tests

        /// <summary>
        /// Tests the IsAdmin is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsAdminIsFalseSaves()
        {
            #region Arrange
            Role role = GetValid(9);
            role.IsAdmin = false;
            #endregion Arrange

            #region Act
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(role);
            RoleRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(role.IsAdmin);
            Assert.IsFalse(role.IsTransient());
            Assert.IsTrue(role.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsAdmin is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsAdminIsTrueSaves()
        {
            #region Arrange
            var role = GetValid(9);
            role.IsAdmin = true;
            #endregion Arrange

            #region Act
            RoleRepository.DbContext.BeginTransaction();
            RoleRepository.EnsurePersistent(role);
            RoleRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(role.IsAdmin);
            Assert.IsFalse(role.IsTransient());
            Assert.IsTrue(role.IsValid());
            #endregion Assert
        }

        #endregion IsAdmin Tests


        
        
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

            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsAdmin", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Level", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Users", "System.Collections.Generic.IList`1[Purchasing.Core.Domain.User]", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Role));

        }

        #endregion Reflection of Database.	
		
		
    }
}