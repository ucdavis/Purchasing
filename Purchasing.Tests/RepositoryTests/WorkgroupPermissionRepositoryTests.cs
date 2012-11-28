using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		WorkgroupPermission
    /// LookupFieldName:	Id
    /// </summary>
    [TestClass]
    public class WorkgroupPermissionRepositoryTests : AbstractRepositoryTests<WorkgroupPermission, int, WorkgroupPermissionMap>
    {
        /// <summary>
        /// Gets or sets the WorkgroupPermission repository.
        /// </summary>
        /// <value>The WorkgroupPermission repository.</value>
        public IRepository<WorkgroupPermission> WorkgroupPermissionRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; }
        public IRepositoryWithTypedId<Role, string> RoleRepository { get; set; }
        public IRepository<Workgroup> WorkgroupRepository { get; set; } 
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkgroupPermissionRepositoryTests"/> class.
        /// </summary>
        public WorkgroupPermissionRepositoryTests()
        {
            WorkgroupPermissionRepository = new Repository<WorkgroupPermission>();
            UserRepository = new RepositoryWithTypedId<User, string>();
            RoleRepository = new RepositoryWithTypedId<Role, string>();
            WorkgroupRepository = new Repository<Workgroup>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override WorkgroupPermission GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.WorkgroupPermission(counter);
            rtValue.Workgroup = Repository.OfType<Workgroup>().Queryable.First();
            rtValue.Role = RoleRepository.Queryable.First();
            rtValue.User = UserRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<WorkgroupPermission> GetQuery(int numberAtEnd)
        {
            return WorkgroupPermissionRepository.Queryable.Where(a => a.Id == numberAtEnd);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(WorkgroupPermission entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(WorkgroupPermission entity, ARTAction action)
        {
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(UserRepository.Queryable.Single(a => a.Id == "2").Id, entity.User.Id);
                    break;
                case ARTAction.Restore:
                    entity.User = UserRepository.Queryable.First();
                    break;
                case ARTAction.Update:
                    entity.User = UserRepository.Queryable.Single(a => a.Id == "2");
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            UserRepository.DbContext.BeginTransaction();
            LoadOrganizations(3);
            LoadWorkgroups(3);
            LoadRoles(3);
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();

            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region Workgroup Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestWorkgroupPermissionsFieldWorkgroupWithAValueOfNullDoesNotSave()
        {
            WorkgroupPermission record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Workgroup = null;
                #endregion Arrange

                #region Act
                WorkgroupPermissionRepository.DbContext.BeginTransaction();
                WorkgroupPermissionRepository.EnsurePersistent(record);
                WorkgroupPermissionRepository.DbContext.CommitTransaction();
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
        public void TestWorkgroupPermissionNewWorkgroupDoesNotSave()
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
                WorkgroupPermissionRepository.DbContext.BeginTransaction();
                WorkgroupPermissionRepository.EnsurePersistent(record);
                WorkgroupPermissionRepository.DbContext.CommitTransaction();
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

        [TestMethod]
        public void TestWorkgroupPermissionWithExistingWorkgroupSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Workgroup = Repository.OfType<Workgroup>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(record);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Workgroup.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Workgroup Tests

        #region User Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestWorkgroupPermissionsFieldUserWithAValueOfNullDoesNotSave()
        {
            WorkgroupPermission record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.User = null;
                #endregion Arrange

                #region Act
                WorkgroupPermissionRepository.DbContext.BeginTransaction();
                WorkgroupPermissionRepository.EnsurePersistent(record);
                WorkgroupPermissionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.User, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The User field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestWorkgroupPermissionNewUserDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.User = new User();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupPermissionRepository.DbContext.BeginTransaction();
                WorkgroupPermissionRepository.EnsurePersistent(record);
                WorkgroupPermissionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.User, Entity: Purchasing.Core.Domain.User", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestWorkgroupPermissionWithExistingUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = UserRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(record);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.User.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }
        #endregion User Tests

        #region Role Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestWorkgroupPermissionsFieldRoleWithAValueOfNullDoesNotSave()
        {
            WorkgroupPermission record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Role = null;
                #endregion Arrange

                #region Act
                WorkgroupPermissionRepository.DbContext.BeginTransaction();
                WorkgroupPermissionRepository.EnsurePersistent(record);
                WorkgroupPermissionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Role, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Role field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestWorkgroupPermissionNewRoleDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Role = new Role();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupPermissionRepository.DbContext.BeginTransaction();
                WorkgroupPermissionRepository.EnsurePersistent(record);
                WorkgroupPermissionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Role, Entity: Purchasing.Core.Domain.Role", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestWorkgroupPermissionWithExistingRoleSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Role = RoleRepository.Queryable.Single(a => a.Id == "3");
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(record);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("3", record.Role.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Role Tests

        #region IsAdmin Tests

        /// <summary>
        /// Tests the IsAdmin is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsAdminIsFalseSaves()
        {
            #region Arrange
            WorkgroupPermission workgroupPermission = GetValid(9);
            workgroupPermission.IsAdmin = false;
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(workgroupPermission);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupPermission.IsAdmin);
            Assert.IsFalse(workgroupPermission.IsTransient());
            Assert.IsTrue(workgroupPermission.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsAdmin is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsAdminIsTrueSaves()
        {
            #region Arrange
            var workgroupPermission = GetValid(9);
            workgroupPermission.IsAdmin = true;
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(workgroupPermission);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroupPermission.IsAdmin);
            Assert.IsFalse(workgroupPermission.IsTransient());
            Assert.IsTrue(workgroupPermission.IsValid());
            #endregion Assert
        }

        #endregion IsAdmin Tests

        #region IsFullFeatured Tests

        /// <summary>
        /// Tests the IsFullFeatured is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsFullFeaturedIsFalseSaves()
        {
            #region Arrange
            WorkgroupPermission workgroupPermission = GetValid(9);
            workgroupPermission.IsFullFeatured = false;
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(workgroupPermission);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupPermission.IsFullFeatured);
            Assert.IsFalse(workgroupPermission.IsTransient());
            Assert.IsTrue(workgroupPermission.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsFullFeatured is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsFullFeaturedIsTrueSaves()
        {
            #region Arrange
            var workgroupPermission = GetValid(9);
            workgroupPermission.IsFullFeatured = true;
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(workgroupPermission);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroupPermission.IsFullFeatured);
            Assert.IsFalse(workgroupPermission.IsTransient());
            Assert.IsTrue(workgroupPermission.IsValid());
            #endregion Assert
        }

        #endregion IsFullFeatured Tests

        #region ParentWorkgroup Tests

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestWorkgroupPermissionNewParentWorkgroupDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.ParentWorkgroup = new Workgroup();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupPermissionRepository.DbContext.BeginTransaction();
                WorkgroupPermissionRepository.EnsurePersistent(record);
                WorkgroupPermissionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual(
                    "object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: Purchasing.Core.Domain.Workgroup, Entity: Purchasing.Core.Domain.Workgroup",
                    ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestWorkgroupPermissionWithExistingParentWorkgroupSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.ParentWorkgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(record);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.ParentWorkgroup.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }


        [TestMethod]
        public void TestWorkgroupPermissionWithNullParentWorkgroupSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.ParentWorkgroup = null;
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(record);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, record.ParentWorkgroup);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion ParentWorkgroup Tests

        #region IsDefaultForAccount Tests

        /// <summary>
        /// Tests the IsDefaultForAccount is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsDefaultForAccountIsFalseSaves()
        {
            #region Arrange
            WorkgroupPermission workgroupPermission = GetValid(9);
            workgroupPermission.IsDefaultForAccount = false;
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(workgroupPermission);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupPermission.IsDefaultForAccount);
            Assert.IsFalse(workgroupPermission.IsTransient());
            Assert.IsTrue(workgroupPermission.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsDefaultForAccount is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsDefaultForAccountIsTrueSaves()
        {
            #region Arrange
            var workgroupPermission = GetValid(9);
            workgroupPermission.IsDefaultForAccount = true;
            #endregion Arrange

            #region Act
            WorkgroupPermissionRepository.DbContext.BeginTransaction();
            WorkgroupPermissionRepository.EnsurePersistent(workgroupPermission);
            WorkgroupPermissionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroupPermission.IsDefaultForAccount);
            Assert.IsFalse(workgroupPermission.IsTransient());
            Assert.IsTrue(workgroupPermission.IsValid());
            #endregion Assert
        }

        #endregion IsDefaultForAccount Tests


        
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
            expectedFields.Add(new NameAndType("IsAdmin", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("IsDefaultForAccount", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("IsFullFeatured", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("ParentWorkgroup", "Purchasing.Core.Domain.Workgroup", new List<string>()));
            expectedFields.Add(new NameAndType("Role", "Purchasing.Core.Domain.Role", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Workgroup", "Purchasing.Core.Domain.Workgroup", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(WorkgroupPermission));

        }

        #endregion Reflection of Database.	
		
		
    }
}