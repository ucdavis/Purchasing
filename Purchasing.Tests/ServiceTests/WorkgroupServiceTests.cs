using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using AutoMapper;
using Moq;
using Purchasing.Core.Services;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class WorkgroupServiceTests
    {
        public IWorkgroupService WorkgroupService;
        public IRepositoryWithTypedId<Vendor, string> VendorRepository;
        public IRepositoryWithTypedId<VendorAddress, Guid> VendorAddressRepository;
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IRepositoryWithTypedId<EmailPreferences, string> EmailPreferencesRepository;
        public IRepository<WorkgroupPermission> WorkgroupPermissionRepository;
        public IRepository<Workgroup> WorkgroupRepository;
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository;
        public IDirectorySearchService SearchService;
        public IRepositoryFactory RepositoryFactory;
        public IQueryRepositoryFactory QueryRepositoryFactory;
        public IUserIdentity UserIdentity;
        public IMapper Mapper;
        public IAggieEnterpriseService AggieEnterpriseService;

        #region Init
        public WorkgroupServiceTests()
        {
            Mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ViewModelProfile>();
            }).CreateMapper();
            VendorRepository = Mock.Of<IRepositoryWithTypedId<Vendor, string>>();
            VendorAddressRepository = Mock.Of<IRepositoryWithTypedId<VendorAddress, Guid>>();
            UserRepository = Mock.Of<IRepositoryWithTypedId<User, string>>();
            EmailPreferencesRepository = Mock.Of<IRepositoryWithTypedId<EmailPreferences, string>>();
            WorkgroupPermissionRepository = Mock.Of<IRepository<WorkgroupPermission>>();
            WorkgroupRepository = Mock.Of<IRepository<Workgroup>>();
            OrganizationRepository = Mock.Of<IRepositoryWithTypedId<Organization, string>>();
            SearchService = Mock.Of<IDirectorySearchService>();
            RepositoryFactory = Mock.Of<IRepositoryFactory>();
            Mock.Get(RepositoryFactory).SetupGet(r => r.RoleRepository).Returns(Mock.Of<IRepositoryWithTypedId<Role, string>>());
            Mock.Get(RepositoryFactory).SetupGet(r => r.WorkgroupPermissionRepository).Returns(WorkgroupPermissionRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.AccountRepository).Returns(Mock.Of<IRepositoryWithTypedId<Account, string>>());
            QueryRepositoryFactory = Mock.Of<IQueryRepositoryFactory>();
            UserIdentity = Mock.Of<IUserIdentity>();

            QueryRepositoryFactory.RelatatedWorkgroupsRepository =
                Mock.Of<IRepository<RelatedWorkgroups>>();
            AggieEnterpriseService = Mock.Of<IAggieEnterpriseService>();

            WorkgroupService = new WorkgroupService(VendorRepository,
                VendorAddressRepository,
                UserRepository,
                EmailPreferencesRepository,
                WorkgroupPermissionRepository,
                WorkgroupRepository,
                OrganizationRepository,
                SearchService,
                RepositoryFactory,
                QueryRepositoryFactory,
                UserIdentity,
                Mapper,
                AggieEnterpriseService);
        }
        #endregion Init

        #region TransferValues Tests

        [TestMethod]
        public void TestTransferValuesWhenKfsVendorHasNullState()
        {
            #region Arrange
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(1));
            vendors[0].Id = "1";
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddress = new List<VendorAddress>();
            vendorAddress.Add(CreateValidEntities.VendorAddress(2));
            vendorAddress[0].Vendor = VendorRepository.Queryable.Single(a => a.Id == "1");
            vendorAddress[0].TypeCode = "2";
            vendorAddress[0].State = null;
            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddress, false);

            var source = CreateValidEntities.WorkgroupVendor(1);
            source.VendorId = "1";
            source.VendorAddressTypeCode = "2";

            var destination = new WorkgroupVendor();
            Assert.AreNotEqual(destination.State, "XX");
            #endregion Arrange

            #region Act
            WorkgroupService.TransferValues(source, ref destination);
            #endregion Act

            #region Assert
            Assert.AreEqual("XX", destination.State);
            #endregion Assert		
        }

        [TestMethod]
        public void TestTransferValuesWhenKfsVendorHasNullZip()
        {
            #region Arrange
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(1));
            vendors[0].Id = "1";
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddress = new List<VendorAddress>();
            vendorAddress.Add(CreateValidEntities.VendorAddress(2));
            vendorAddress[0].Vendor = VendorRepository.Queryable.Single(a => a.Id == "1");
            vendorAddress[0].TypeCode = "2";
            vendorAddress[0].Zip = null;
            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddress, false);

            var source = CreateValidEntities.WorkgroupVendor(1);
            source.VendorId = "1";
            source.VendorAddressTypeCode = "2";

            var destination = new WorkgroupVendor();
            Assert.AreNotEqual(destination.Zip, "XXXXX");
            #endregion Arrange

            #region Act
            WorkgroupService.TransferValues(source, ref destination);
            #endregion Act

            #region Assert
            Assert.AreEqual("XXXXX", destination.Zip);
            #endregion Assert
        }

        [TestMethod]
        public void TestTransferValuesWhenKfsVendor()
        {
            #region Arrange
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(1));
            vendors[0].Name = "VendorName1";
            vendors[0].Id = "1";
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddress = new List<VendorAddress>();
            vendorAddress.Add(CreateValidEntities.VendorAddress(2));
            vendorAddress[0].Vendor = VendorRepository.Queryable.Single(a => a.Id == "1");
            vendorAddress[0].TypeCode = "2";
            vendorAddress[0].Zip = "98765";
            vendorAddress[0].Name = "DoesNotMatter";
            vendorAddress[0].Line1 = "VaLine1";
            vendorAddress[0].Line2 = "VaLine2";
            vendorAddress[0].Line3 = "VaLine3";
            vendorAddress[0].City = "VaCity1";
            vendorAddress[0].State = "AB";
            vendorAddress[0].CountryCode = "UK";
            vendorAddress[0].PhoneNumber = "333-444-55555";
            vendorAddress[0].FaxNumber = "222-444-55555";

            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddress, false);

            var source = CreateValidEntities.WorkgroupVendor(1);
            source.VendorId = "1";
            source.VendorAddressTypeCode = "2";

            var destination = new WorkgroupVendor();

            #endregion Arrange

            #region Act
            WorkgroupService.TransferValues(source, ref destination);
            #endregion Act

            #region Assert
            Assert.AreEqual("VendorName1", destination.Name);
            Assert.AreEqual("VaLine1", destination.Line1);
            Assert.AreEqual("VaLine2", destination.Line2);
            Assert.AreEqual("VaLine3", destination.Line3);
            Assert.AreEqual("VaCity1", destination.City);
            Assert.AreEqual("AB", destination.State);
            Assert.AreEqual("98765", destination.Zip);
            Assert.AreEqual("UK", destination.CountryCode);
            Assert.AreEqual("333-444-55555", destination.Phone);
            Assert.AreEqual("222-444-55555", destination.Fax);
            #endregion Assert
        }

        [TestMethod]
        public void TestTransferValuesWhenNotKfsVendor()
        {
            #region Arrange
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(1));
            vendors[0].Name = "VendorName1";
            vendors[0].Id = "1";
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddress = new List<VendorAddress>();
            vendorAddress.Add(CreateValidEntities.VendorAddress(2));
            vendorAddress[0].Vendor = VendorRepository.Queryable.Single(a => a.Id == "1");
            vendorAddress[0].TypeCode = "2";
            vendorAddress[0].Zip = "98765";
            vendorAddress[0].Name = "DoesNotMatter";
            vendorAddress[0].Line1 = "VaLine1";
            vendorAddress[0].Line2 = "VaLine2";
            vendorAddress[0].Line3 = "VaLine3";
            vendorAddress[0].City = "VaCity1";
            vendorAddress[0].State = "AB";
            vendorAddress[0].CountryCode = "UK";
            vendorAddress[0].PhoneNumber = "333-444-55555";
            vendorAddress[0].FaxNumber = "222-444-55555";

            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddress, false);

            var source = CreateValidEntities.WorkgroupVendor(1);
            source.VendorId = null;
            source.VendorAddressTypeCode = "2";
            source.Phone = "111";
            source.Fax = "222";

            var destination = new WorkgroupVendor();

            #endregion Arrange

            #region Act
            WorkgroupService.TransferValues(source, ref destination);
            #endregion Act

            #region Assert
            Assert.AreEqual("Name1", destination.Name);
            Assert.AreEqual("Line11", destination.Line1);
            Assert.AreEqual(null, destination.Line2);
            Assert.AreEqual(null, destination.Line3);
            Assert.AreEqual("City1", destination.City);
            Assert.AreEqual("CA", destination.State);
            Assert.AreEqual("95616", destination.Zip);
            Assert.AreEqual("US", destination.CountryCode);
            Assert.AreEqual("111", destination.Phone);
            Assert.AreEqual("222", destination.Fax);
            #endregion Assert
        }
        #endregion TransferValues Tests

        #region TryToAddPeople Tests


        [TestMethod]
        public void TestTryToAddPeopleWhenUserIsNotFound1()
        {
            #region Arrange
            new FakeUsers(0, UserRepository);
            Mock.Get(SearchService).Setup(a => a.FindUser(It.IsAny<string>())).Returns<DirectoryUser>(null);
            var failCount = 0;
            var dupCount = 0;
            var notAdded = new List<KeyValuePair<string, string>>();
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, new Role(), new Workgroup(), 0, "test", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            Mock.Get(UserRepository).Verify(a => a.GetNullableById("test"));
            Mock.Get(SearchService).Verify(a => a.FindUser("test"));
            Assert.AreEqual(1, failCount);
            Assert.AreEqual(0, dupCount);
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, notAdded.Count());
            Assert.AreEqual("Not found", notAdded[0].Value);
            Assert.AreEqual("test", notAdded[0].Key);
            #endregion Assert		
        }

        [TestMethod]
        public void TestTryToAddPeopleWhenUserIsNotFound2()
        {
            #region Arrange
            new FakeUsers(0, UserRepository);
            Mock.Get(SearchService).Setup(a => a.FindUser(It.IsAny<string>())).Returns<DirectoryUser>(null);
            var failCount = 2;
            var dupCount = 4;
            var notAdded = new List<KeyValuePair<string, string>>();
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, new Role(), new Workgroup(), 0, "test", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            Mock.Get(UserRepository).Verify(a => a.GetNullableById("test"));
            Mock.Get(SearchService).Verify(a => a.FindUser("test"));
            Assert.AreEqual(3, failCount);
            Assert.AreEqual(4, dupCount);
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, notAdded.Count());
            Assert.AreEqual("Not found", notAdded[0].Value);
            Assert.AreEqual("test", notAdded[0].Key);
            #endregion Assert
        }


        [TestMethod]
        public void TestTryToAddPeopleWhenUserAlreadyExists1()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository);
            var failCount = 0;
            var dupCount = 0;
            var notAdded = new List<KeyValuePair<string, string>>();
            new FakeRoles(3, RepositoryFactory.RoleRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            WorkgroupPermission args = default;
            Mock.Get(WorkgroupPermissionRepository).Setup(a => a.EnsurePersistent(It.IsAny<WorkgroupPermission>()))
                .Callback<WorkgroupPermission>(x => args = x);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, RepositoryFactory.RoleRepository.Queryable.Single(a => a.Id == "2"), new Workgroup(), 0, "2", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(0, failCount);
            Assert.AreEqual(0, dupCount);
            Assert.AreEqual(0, notAdded.Count());
            Mock.Get(EmailPreferencesRepository).Verify(a => a.EnsurePersistent(It.IsAny<EmailPreferences>()), Times.Never());
            Mock.Get(WorkgroupPermissionRepository).Verify(a => a.EnsurePersistent(It.IsAny<WorkgroupPermission>()));
            Assert.IsNotNull(args);
            Assert.AreEqual("2", args.Role.Id);
            Assert.AreEqual("2", args.User.Id);
            Assert.AreEqual(1, args.Workgroup.Id);
            Mock.Get(UserIdentity).Verify(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "2"));
            #endregion Assert		
        }

        [TestMethod]
        public void TestTryToAddPeopleWhenUserAlreadyExists2()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            var failCount = 0;
            var dupCount = 0;
            var notAdded = new List<KeyValuePair<string, string>>();
            new FakeRoles(3, RepositoryFactory.RoleRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            var wp = new List<WorkgroupPermission>();
            wp.Add(CreateValidEntities.WorkgroupPermission(1));
            wp[0].User = UserRepository.Queryable.Single(a => a.Id == "2");
            wp[0].Role = RepositoryFactory.RoleRepository.Queryable.Single(a => a.Id == "2");
            wp[0].Workgroup = WorkgroupRepository.Queryable.Single(a => a.Id == 1);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, wp);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, RepositoryFactory.RoleRepository.Queryable.Single(a => a.Id == "2"), WorkgroupRepository.Queryable.Single(a => a.Id == 1), 0, "2", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, failCount);
            Assert.AreEqual(1, dupCount);
            Assert.AreEqual(1, notAdded.Count());
            Mock.Get(EmailPreferencesRepository).Verify(a => a.EnsurePersistent(It.IsAny<EmailPreferences>()), Times.Never());
            Mock.Get(WorkgroupPermissionRepository).Verify(a => a.EnsurePersistent(It.IsAny<WorkgroupPermission>()), Times.Never());
            Assert.AreEqual("Is a duplicate", notAdded[0].Value);
            Assert.AreEqual("2", notAdded[0].Key);
            #endregion Assert
        }

        [TestMethod]
        public void TestTryToAddPeopleWhenUserIsFoundWithLdap1()
        {
            #region Arrange
            new FakeUsers(3, UserRepository);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository);
            var failCount = 0;
            var dupCount = 0;
            var notAdded = new List<KeyValuePair<string, string>>();
            new FakeRoles(3, RepositoryFactory.RoleRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            var directoryUser = new DirectoryUser();
            directoryUser.LoginId = "3";
            directoryUser.EmailAddress = "test3@testy.com";
            directoryUser.FirstName = "F3";
            directoryUser.LastName = "Last3";
            Mock.Get(SearchService).Setup(a => a.FindUser("LDAP")).Returns(directoryUser);
            WorkgroupPermission args = default;
            Mock.Get(WorkgroupPermissionRepository).Setup(a => a.EnsurePersistent(It.IsAny<WorkgroupPermission>()))
                .Callback<WorkgroupPermission>(x => args = x);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, RepositoryFactory.RoleRepository.Queryable.Single(a => a.Id == "2"), new Workgroup(), 0, "LDAP", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(0, failCount);
            Assert.AreEqual(0, dupCount);
            Assert.AreEqual(0, notAdded.Count());
            Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(It.IsAny<User>()), Times.Never());
            Mock.Get(EmailPreferencesRepository).Verify(a => a.EnsurePersistent(It.IsAny<EmailPreferences>()), Times.Never());
            Mock.Get(WorkgroupPermissionRepository).Verify(a => a.EnsurePersistent(It.IsAny<WorkgroupPermission>()));
            Assert.IsNotNull(args);
            Assert.AreEqual("2", args.Role.Id);
            Assert.AreEqual("3", args.User.Id);
            Assert.AreEqual(1, args.Workgroup.Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestTryToAddPeopleWhenUserIsFoundWithLdap2()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
            //new FakeUsers(3, UserRepository);
            Mock.Get(UserRepository)
                .SetupSequence(a => a.GetNullableById("LDAP"))
                .Returns((User)null)
                .Returns((User)null)
                .Returns(CreateValidEntities.User(3))
                .Throws(new Exception("Called too many times"));

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository);
            var failCount = 0;
            var dupCount = 0;
            var notAdded = new List<KeyValuePair<string, string>>();
            new FakeRoles(3, RepositoryFactory.RoleRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            var directoryUser = new DirectoryUser();
            directoryUser.LoginId = "LDAP";
            directoryUser.EmailAddress = "test3@testy.com";
            directoryUser.FirstName = "F3";
            directoryUser.LastName = "Last3";
            Mock.Get(SearchService).Setup(a => a.FindUser("LDAP")).Returns(directoryUser);
            WorkgroupPermission args = default;
            Mock.Get(WorkgroupPermissionRepository).Setup(a => a.EnsurePersistent(It.IsAny<WorkgroupPermission>()))
                .Callback<WorkgroupPermission>(x => args = x);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, RepositoryFactory.RoleRepository.Queryable.Single(a => a.Id == "2"), new Workgroup(), 0, "LDAP", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(0, failCount);
            Assert.AreEqual(0, dupCount);
            Assert.AreEqual(0, notAdded.Count());
            Mock.Get(UserRepository).Verify(a => a.EnsurePersistent(It.IsAny<User>()));
            Mock.Get(EmailPreferencesRepository).Verify(a => a.EnsurePersistent(It.IsAny<EmailPreferences>()));
            Mock.Get(WorkgroupPermissionRepository).Verify(a => a.EnsurePersistent(It.IsAny<WorkgroupPermission>()));
            Assert.IsNotNull(args);
            Assert.AreEqual("2", args.Role.Id);
            Assert.AreEqual("3", args.User.Id);
            Assert.AreEqual(1, args.Workgroup.Id);
            #endregion Assert
        }

        #endregion TryToAddPeople Tests

        #region TryBulkLoadPeople Tests

        [TestMethod]
        public void TestTryBulkLoadPeopleWithKerb1()
        {
            #region Arrange
            var failCount = 0;
            var dupCount = 0;
            var notAdded = new List<KeyValuePair<string, string>>();
            var role = CreateValidEntities.Role(1);
            var workgroup = CreateValidEntities.Workgroup(1);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryBulkLoadPeople("kerb1,kerb2 kerb3", false, 1, role,
                                                            workgroup, 0, ref failCount, ref dupCount,
                                                            notAdded);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            Mock.Get(UserRepository).Verify(a => a.GetNullableById("kerb1"));
            Mock.Get(UserRepository).Verify(a => a.GetNullableById("kerb2"));
            Mock.Get(UserRepository).Verify(a => a.GetNullableById("kerb3"));
            Mock.Get(UserRepository).Verify(a => a.GetNullableById(It.IsAny<string>()), Times.Exactly(3));
            #endregion Assert		
        }

        [TestMethod]
        public void TestTryBulkLoadPeopleWithEmail1()
        {
            #region Arrange
            var failCount = 0;
            var dupCount = 0;
            var notAdded = new List<KeyValuePair<string, string>>();
            var role = CreateValidEntities.Role(1);
            var workgroup = CreateValidEntities.Workgroup(1);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryBulkLoadPeople("kerb1,kerb2 kerb3", true, 1, role,
                                                            workgroup, 0, ref failCount, ref dupCount,
                                                            notAdded);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            Mock.Get(UserRepository).Verify(a => a.GetNullableById(It.IsAny<string>()), Times.Never());
            #endregion Assert
        }

        [TestMethod]
        public void TestTryBulkLoadPeopleWithEmail2()
        {
            #region Arrange
            var failCount = 0;
            var dupCount = 0;
            var notAdded = new List<KeyValuePair<string, string>>();
            var role = CreateValidEntities.Role(1);
            var workgroup = CreateValidEntities.Workgroup(1);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryBulkLoadPeople("Getchell, Adam <acgetchell@ucdavis.edu>; Kirkland, Scott <srkirkland@ucdavis.edu>; Lai, Alan <anlai@ucdavis.edu>; Sylvestre, Jason <jsylvestre@ucdavis.edu>; Taylor, Ken <kentaylor@ucdavis.edu>", true, 1, role,
                                                            workgroup, 0, ref failCount, ref dupCount,
                                                            notAdded);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            Mock.Get(UserRepository).Verify(a => a.GetNullableById("acgetchell@ucdavis.edu"));
            Mock.Get(UserRepository).Verify(a => a.GetNullableById("srkirkland@ucdavis.edu"));
            Mock.Get(UserRepository).Verify(a => a.GetNullableById("anlai@ucdavis.edu"));
            Mock.Get(UserRepository).Verify(a => a.GetNullableById("jsylvestre@ucdavis.edu"));
            Mock.Get(UserRepository).Verify(a => a.GetNullableById("kentaylor@ucdavis.edu"));
            Mock.Get(UserRepository).Verify(a => a.GetNullableById(It.IsAny<string>()), Times.Exactly(5));
            #endregion Assert
        }

        #endregion TryBulkLoadPeople Tests

        #region CreateWorkgroup Tests

        [TestMethod]
        public void TestCreateWorkgroupAddsPrimaryOrgAsASubOrg()
        {
            #region Arrange
            var workgroup = CreateValidEntities.Workgroup(1);
            workgroup.PrimaryOrganization = CreateValidEntities.Organization(9);
            workgroup.Organizations = new List<Organization>();

            new FakeRelatedWorkgroups(3, QueryRepositoryFactory.RelatatedWorkgroupsRepository);
            new FakeWorkgroupPermissions(3, WorkgroupPermissionRepository);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.CreateWorkgroup(workgroup, null);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.PrimaryOrganization.Name);
            Assert.AreEqual(1, result.Organizations.Count());
            Assert.AreEqual("Name9", result.Organizations[0].Name);
            Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(result));
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateWorkgroupReplacesExistingSubOrgs()
        {
            #region Arrange
            new FakeOrganizations(3, OrganizationRepository);
            var workgroup = CreateValidEntities.Workgroup(1);
            workgroup.PrimaryOrganization = CreateValidEntities.Organization(9);
            workgroup.Organizations = new List<Organization>();
            workgroup.Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "2"));
            workgroup.Organizations.Add(OrganizationRepository.Queryable.Single(a => a.Id == "3"));

            new FakeRelatedWorkgroups(3, QueryRepositoryFactory.RelatatedWorkgroupsRepository);
            new FakeWorkgroupPermissions(3, WorkgroupPermissionRepository);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.CreateWorkgroup(workgroup, new string[] { "1" });
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.PrimaryOrganization.Name);
            Assert.AreEqual(2, result.Organizations.Count());
            Assert.AreEqual("Name1", result.Organizations[0].Name);
            Assert.AreEqual("Name9", result.Organizations[1].Name);
            Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(result));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateWorkgroupWhenSyncAccountsIsNotSelectedDoesNotAddAccounts()
        {
            #region Arrange
            var workgroup = CreateValidEntities.Workgroup(1);
            workgroup.SyncAccounts = false;
            workgroup.Accounts = new List<WorkgroupAccount>();

            new FakeRelatedWorkgroups(3, QueryRepositoryFactory.RelatatedWorkgroupsRepository);
            new FakeWorkgroupPermissions(3, WorkgroupPermissionRepository);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.CreateWorkgroup(workgroup, null);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(result));
            Mock.Get(RepositoryFactory.AccountRepository).Verify(a => a.Queryable, Times.Never());
            Assert.AreEqual(0, result.Accounts.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateWorkgroupWhenSyncAccountsIsSelectedAddsAccounts()
        {
            #region Arrange
            var organizations = new List<Organization>();
            organizations.Add(CreateValidEntities.Organization(1));
            organizations[0].Id = "1";
            organizations.Add(CreateValidEntities.Organization(2));
            organizations[1].Id = "2";

            new FakeOrganizations(0, OrganizationRepository, organizations, true);

            var accounts = new List<Account>();
            for (int i = 0; i < 5; i++)
            {
                accounts.Add(CreateValidEntities.Account(i + 1));
                accounts[i].OrganizationId = "1";
            }
            accounts[3].OrganizationId = "2";
            new FakeAccounts(0, RepositoryFactory.AccountRepository, accounts, false);

            var workgroup = CreateValidEntities.Workgroup(1);
            workgroup.SyncAccounts = true;
            workgroup.Accounts = new List<WorkgroupAccount>();
            workgroup.PrimaryOrganization = organizations[0];
            workgroup.Organizations.Add(organizations[1]);

            new FakeRelatedWorkgroups(3, QueryRepositoryFactory.RelatatedWorkgroupsRepository);
            new FakeWorkgroupPermissions(3, WorkgroupPermissionRepository);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.CreateWorkgroup(workgroup, new[] { "2" });
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(result));
            #endregion Assert
        }


        [Ignore("Need to test the related workgroups")]
        public void TestRelatedWorkgroups()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }


        #endregion CreateWorkgroup Tests
    }
}
