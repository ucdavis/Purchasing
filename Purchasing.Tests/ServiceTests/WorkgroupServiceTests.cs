using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using FluentNHibernate.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Helpers;
using Purchasing.Web.Services;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;

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

        #region Init
        public WorkgroupServiceTests()
        {
            AutomapperConfig.Configure();
            VendorRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Vendor, string>>();
            VendorAddressRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<VendorAddress, Guid>>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            EmailPreferencesRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<EmailPreferences, string>>();
            WorkgroupPermissionRepository = MockRepository.GenerateStub<IRepository<WorkgroupPermission>>();
            WorkgroupRepository = MockRepository.GenerateStub<IRepository<Workgroup>>();
            OrganizationRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Organization, string>>();
            SearchService = MockRepository.GenerateStub<IDirectorySearchService>();
            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();
            RepositoryFactory.RoleRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Role, string>>();
            RepositoryFactory.WorkgroupPermissionRepository = WorkgroupPermissionRepository;
            RepositoryFactory.AccountRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Account, string>>();
            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
            UserIdentity = MockRepository.GenerateStub<IUserIdentity>();

            WorkgroupService = new WorkgroupService(VendorRepository,
                VendorAddressRepository,
                UserRepository,
                EmailPreferencesRepository,
                WorkgroupPermissionRepository,
                WorkgroupRepository,
                OrganizationRepository,
                SearchService,
                RepositoryFactory, QueryRepositoryFactory, UserIdentity);
        }
        #endregion Init

        #region TransferValues Tests
        
        [TestMethod]
        public void TestTransferValuesWhenKfsVendorHasNullState()
        {
            #region Arrange
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(1));
            vendors[0].SetIdTo("1");
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
            vendors[0].SetIdTo("1");
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
            vendors[0].SetIdTo("1");
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
            vendors[0].SetIdTo("1");
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
            SearchService.Expect(a => a.FindUser(Arg<string>.Is.Anything)).Return(null);
            var failCount = 0;
            var dupCount = 0;
            var notAdded = new List<KeyValuePair<string, string>>();
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, new Role(), new Workgroup(), 0, "test", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            UserRepository.AssertWasCalled(a => a.GetNullableById("test"));
            SearchService.AssertWasCalled(a => a.FindUser("test"));
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
            SearchService.Expect(a => a.FindUser(Arg<string>.Is.Anything)).Return(null);
            var failCount = 2;
            var dupCount = 4;
            var notAdded = new List<KeyValuePair<string, string>>();
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, new Role(), new Workgroup(), 0, "test", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            UserRepository.AssertWasCalled(a => a.GetNullableById("test"));
            SearchService.AssertWasCalled(a => a.FindUser("test"));
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
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
            new FakeUsers(3, UserRepository);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository);
            var failCount = 0;
            var dupCount = 0;
            var notAdded = new List<KeyValuePair<string, string>>();
            new FakeRoles(3, RepositoryFactory.RoleRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, RepositoryFactory.RoleRepository.Queryable.Single(a => a.Id == "2"), new Workgroup(), 0, "2", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(0, failCount);
            Assert.AreEqual(0, dupCount);
            Assert.AreEqual(0, notAdded.Count());
            EmailPreferencesRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailPreferences>.Is.Anything));
            WorkgroupPermissionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything));
            var args = (WorkgroupPermission) WorkgroupPermissionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("2", args.Role.Id);
            Assert.AreEqual("2", args.User.Id);
            Assert.AreEqual(1, args.Workgroup.Id);
            UserIdentity.AssertWasCalled(a => a.RemoveUserRoleFromCache(Resources.Role_CacheId, "2"));
            #endregion Assert		
        }

        [TestMethod]
        public void TestTryToAddPeopleWhenUserAlreadyExists2()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
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
            EmailPreferencesRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailPreferences>.Is.Anything));
            WorkgroupPermissionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything));
            Assert.AreEqual("Is a duplicate", notAdded[0].Value);
            Assert.AreEqual("2", notAdded[0].Key);
            #endregion Assert
        }

        [TestMethod]
        public void TestTryToAddPeopleWhenUserIsFoundWithLdap1()
        {
            #region Arrange
            //HttpContext.Current = new HttpContext(new HttpRequest(null, "http://test.org", null), new HttpResponse(null));
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
            SearchService.Expect(a => a.FindUser("LDAP")).Return(directoryUser);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, RepositoryFactory.RoleRepository.Queryable.Single(a => a.Id == "2"), new Workgroup(), 0, "LDAP", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(0, failCount);
            Assert.AreEqual(0, dupCount);
            Assert.AreEqual(0, notAdded.Count());
            UserRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            EmailPreferencesRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailPreferences>.Is.Anything));
            WorkgroupPermissionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything));
            var args = (WorkgroupPermission)WorkgroupPermissionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything))[0][0];
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
            UserRepository.Expect(a => a.GetNullableById("LDAP")).Return(null).Repeat.Twice(); 
            UserRepository.Expect(a => a.GetNullableById("LDAP")).Return(CreateValidEntities.User(3)).Repeat.Once();

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
            SearchService.Expect(a => a.FindUser("LDAP")).Return(directoryUser);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.TryToAddPeople(1, RepositoryFactory.RoleRepository.Queryable.Single(a => a.Id == "2"), new Workgroup(), 0, "LDAP", ref failCount, ref dupCount, notAdded);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(0, failCount);
            Assert.AreEqual(0, dupCount);
            Assert.AreEqual(0, notAdded.Count());
            UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));
            EmailPreferencesRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailPreferences>.Is.Anything));
            WorkgroupPermissionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything));
            var args = (WorkgroupPermission)WorkgroupPermissionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<WorkgroupPermission>.Is.Anything))[0][0];
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
            UserRepository.AssertWasCalled(a => a.GetNullableById("kerb1"));
            UserRepository.AssertWasCalled(a => a.GetNullableById("kerb2"));
            UserRepository.AssertWasCalled(a => a.GetNullableById("kerb3"));
            UserRepository.AssertWasCalled(a => a.GetNullableById(Arg<string>.Is.Anything), x => x.Repeat.Times(3));
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
            UserRepository.AssertWasNotCalled(a => a.GetNullableById(Arg<string>.Is.Anything));
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
            UserRepository.AssertWasCalled(a => a.GetNullableById("acgetchell@ucdavis.edu"));
            UserRepository.AssertWasCalled(a => a.GetNullableById("srkirkland@ucdavis.edu"));
            UserRepository.AssertWasCalled(a => a.GetNullableById("anlai@ucdavis.edu"));
            UserRepository.AssertWasCalled(a => a.GetNullableById("jsylvestre@ucdavis.edu"));
            UserRepository.AssertWasCalled(a => a.GetNullableById("kentaylor@ucdavis.edu"));
            UserRepository.AssertWasCalled(a => a.GetNullableById(Arg<string>.Is.Anything), x => x.Repeat.Times(5));
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
            #endregion Arrange

            #region Act
            var result = WorkgroupService.CreateWorkgroup(workgroup, null);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.PrimaryOrganization.Name);
            Assert.AreEqual(1, result.Organizations.Count());
            Assert.AreEqual("Name9", result.Organizations[0].Name);
            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(result));
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
            #endregion Arrange

            #region Act
            var result = WorkgroupService.CreateWorkgroup(workgroup, new string[]{"1"});
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.PrimaryOrganization.Name);
            Assert.AreEqual(2, result.Organizations.Count());
            Assert.AreEqual("Name1", result.Organizations[0].Name);
            Assert.AreEqual("Name9", result.Organizations[1].Name);
            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(result));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateWorkgroupWhenSyncAccountsIsNotSelectedDoesNotAddAccounts()
        {
            #region Arrange
            var workgroup = CreateValidEntities.Workgroup(1);
            workgroup.SyncAccounts = false;
            workgroup.Accounts = new List<WorkgroupAccount>();
            #endregion Arrange

            #region Act
            var result = WorkgroupService.CreateWorkgroup(workgroup, null);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(result));
            RepositoryFactory.AccountRepository.AssertWasNotCalled(a => a.Queryable);
            Assert.AreEqual(0, result.Accounts.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateWorkgroupWhenSyncAccountsIsSelectedAddsAccounts()
        {
            #region Arrange
            var organizations = new List<Organization>();
            organizations.Add(CreateValidEntities.Organization(1));
            organizations[0].SetIdTo("1");
            organizations.Add(CreateValidEntities.Organization(2));
            organizations[1].SetIdTo("2");

            new FakeOrganizations(0, OrganizationRepository, organizations, true);

            var accounts = new List<Account>();
            for (int i = 0; i < 5; i++)
            {
                accounts.Add(CreateValidEntities.Account(i+1));
                accounts[i].OrganizationId = "1";
            }
            accounts[3].OrganizationId = "2";
            new FakeAccounts(0, RepositoryFactory.AccountRepository, accounts, false);

            var workgroup = CreateValidEntities.Workgroup(1);
            workgroup.SyncAccounts = true;
            workgroup.Accounts = new List<WorkgroupAccount>();
            workgroup.PrimaryOrganization = organizations[0];
            workgroup.Organizations.Add(organizations[1]);
            #endregion Arrange

            #region Act
            var result = WorkgroupService.CreateWorkgroup(workgroup, new []{"2"});
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            WorkgroupRepository.AssertWasCalled(a => a.EnsurePersistent(result));
            RepositoryFactory.AccountRepository.AssertWasCalled(a => a.Queryable);
            Assert.AreEqual(4, result.Accounts.Count());
            #endregion Assert
        }
        
        #endregion CreateWorkgroup Tests
    }
}
