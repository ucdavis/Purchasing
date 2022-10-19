﻿using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FluentNHibernate.MappingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using UCDArch.Core;
using Moq;
using Purchasing.Core.Services;

namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    [TestClass]
    public partial class WorkgroupControllerTests : ControllerTestBase<WorkgroupController>
    {
        protected IRepositoryWithTypedId<User, string> UserRepository;
        protected IRepository<User> UserRepository2; 
        protected IRepositoryWithTypedId<Role, string> RoleRepository;
        protected ISecurityService SecurityService;
        protected IDirectorySearchService SearchService;
        protected readonly Type ControllerClass = typeof(WorkgroupController);
        protected IRepository<Workgroup> WorkgroupRepository;
        protected IRepository<WorkgroupPermission> WorkgroupPermissionRepository;
        protected IRepository<WorkgroupVendor> WorkgroupVendorRepository;
        protected IRepositoryWithTypedId<Vendor, string> VendorRepository;
        protected IRepositoryWithTypedId<VendorAddress, Guid> VendorAddressRepository;
        protected IRepositoryWithTypedId<State, string> StateRepository;
        protected IRepositoryWithTypedId<EmailPreferences, string> EmailPreferencesRepository;
        protected IRepository<Organization> OrganizationRepository;
        protected IRepository<WorkgroupAccount> WorkgroupAccountRepository;
        protected IWorkgroupAddressService WorkgroupAddressService;
        protected IWorkgroupService WorkgroupService;
        protected IQueryRepositoryFactory QueryRepositoryFactory;
        protected IRepository<OrganizationDescendant> OrganizationDescendantRepository;
        protected IRepository<AdminWorkgroup> AdminWorkgroupRepository;
        protected IRepository<ConditionalApproval> ConditionalApprovalRepository;
        protected IRepositoryFactory RepositoryFactory;

        protected IRepositoryWithTypedId<Account, string> AccountRepository; 

        protected IAggieEnterpriseService AggieEnterpriseService;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            WorkgroupRepository = FakeRepository<Workgroup>();
            UserRepository = Mock.Of<IRepositoryWithTypedId<User, string>>();
            RoleRepository = Mock.Of<IRepositoryWithTypedId<Role, string>>();
            SecurityService = Mock.Of<ISecurityService>();
            SearchService = Mock.Of<IDirectorySearchService>();
            WorkgroupPermissionRepository = Mock.Of<IRepository<WorkgroupPermission>>();
            WorkgroupVendorRepository = Mock.Of<IRepository<WorkgroupVendor>>();
            VendorRepository = Mock.Of<IRepositoryWithTypedId<Vendor, string>>();
            VendorAddressRepository = Mock.Of<IRepositoryWithTypedId<VendorAddress, Guid>>();
            StateRepository = Mock.Of<IRepositoryWithTypedId<State, string>>();
            EmailPreferencesRepository = Mock.Of<IRepositoryWithTypedId<EmailPreferences, string>>();
            WorkgroupAccountRepository = Mock.Of<IRepository<WorkgroupAccount>>();
            WorkgroupAddressService = Mock.Of<IWorkgroupAddressService>();
            WorkgroupService = Mock.Of<IWorkgroupService>();
            OrganizationDescendantRepository = Mock.Of<IRepository<OrganizationDescendant>>();
            QueryRepositoryFactory = Mock.Of<IQueryRepositoryFactory>();
            Mock.Get(QueryRepositoryFactory).SetupGet(r => r.OrganizationDescendantRepository).Returns(OrganizationDescendantRepository);

            AdminWorkgroupRepository = Mock.Of<IRepository<AdminWorkgroup>>();
            Mock.Get(QueryRepositoryFactory).SetupGet(r => r.AdminWorkgroupRepository).Returns(AdminWorkgroupRepository);
            AccountRepository = Mock.Of<IRepositoryWithTypedId<Account, string>>();

            RepositoryFactory = Mock.Of<IRepositoryFactory>();
            Mock.Get(RepositoryFactory).SetupGet(r => r.AccountRepository).Returns(AccountRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.RoleRepository).Returns(RoleRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.ColumnPreferencesRepository).Returns(Mock.Of<IRepositoryWithTypedId<ColumnPreferences, string>>());

            AggieEnterpriseService = Mock.Of<IAggieEnterpriseService>();

            Controller = new WorkgroupController(WorkgroupRepository,
                UserRepository,
                RoleRepository,
                WorkgroupPermissionRepository,
                SecurityService,
                SearchService,
                WorkgroupVendorRepository,
                VendorRepository,
                VendorAddressRepository,
                StateRepository,
                EmailPreferencesRepository,
                WorkgroupAccountRepository,
                QueryRepositoryFactory,
                RepositoryFactory,
                WorkgroupAddressService,
                WorkgroupService,
                SmartServiceLocator<IMapper>.GetService(),
                AggieEnterpriseService);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());

            //Fixes problem where .Fetch is used in a query
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));
            //container.AddComponent("queryExtensionProvider", typeof(IQueryExtensionProvider),
            //                       typeof(QueryExtensionFakes));
            SecurityService = Mock.Of<ISecurityService>();
            container.Kernel.Register(Component.For<ISecurityService>().Instance(SecurityService));

            base.RegisterAdditionalServices(container);
        }

        public WorkgroupControllerTests()
        {

            //    ExampleRepository = FakeRepository<Example>();
            //    Mock.Get(Controller.Repository).Setup(a => a.OfType<Example>()).Returns(ExampleRepository);
            UserRepository2 = FakeRepository<User>();
            Mock.Get(Controller.Repository).Setup(a => a.OfType<User>()).Returns(UserRepository2);

            OrganizationRepository = FakeRepository<Organization>();
            Mock.Get(Controller.Repository).Setup(a => a.OfType<Organization>()).Returns(OrganizationRepository);

            Mock.Get(Controller.Repository).Setup(a => a.OfType<Workgroup>()).Returns(WorkgroupRepository);

            Mock.Get(Controller.Repository).Setup(a => a.OfType<WorkgroupPermission>()).Returns(WorkgroupPermissionRepository);

            ConditionalApprovalRepository = Mock.Of<IRepository<ConditionalApproval>>();
            Mock.Get(Controller.Repository).Setup(a => a.OfType<ConditionalApproval>()).Returns(ConditionalApprovalRepository);
        }
        #endregion Init

        #region Helpers

        public void SetupDataForPeopleList()
        {
            #region Setup Roles
            var roles = new List<Role>();
            SetupRoles(roles);
            #endregion Setup Roles

            #region Setup Users
            var users = new List<User>();
            for (int i = 0; i < 6; i++)
            {
                users.Add(CreateValidEntities.User(i+1));
                users[i].Id = (i + 1).ToString();
            }
            new FakeUsers(0, UserRepository, users, true);
            #endregion Setup Users

            #region Setup Workgroups
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Setup Workgroups
            
            #region Setup WorkgroupPermissions
            var workgroupPermissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 18; i++)
            {
                workgroupPermissions.Add(new WorkgroupPermission());
                workgroupPermissions[i].Role = roles[(i%6)];
                workgroupPermissions[i].User = users[(i%6)];
                workgroupPermissions[i].Workgroup = WorkgroupRepository.GetNullableById((i/6) + 1);
            }
            for (int i = 0; i < 3; i++)
            {
                workgroupPermissions.Add(new WorkgroupPermission());
                workgroupPermissions[i + 18].Workgroup = WorkgroupRepository.GetNullableById(3);
                workgroupPermissions[i + 18].User = users[2];
            }

            workgroupPermissions[18].Role = RoleRepository.GetNullableById(Role.Codes.Approver);
            workgroupPermissions[19].Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            workgroupPermissions[20].Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);


            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Setup WorkgroupPermissions
        }

        public void SetupRoles(List<Role> roles)
        {
            var role = new Role(Role.Codes.Admin);
            role.Id = Role.Codes.Admin;
            role.Name = "Admin";
            role.Level = 0;
            role.IsAdmin = true;
            roles.Add(role);

            role = new Role(Role.Codes.DepartmentalAdmin);
            role.Id = Role.Codes.DepartmentalAdmin;
            role.Name = "Departmental Admin";
            role.Level = 0;
            role.IsAdmin = true;
            roles.Add(role);

            role = new Role(Role.Codes.Requester);
            role.Id = Role.Codes.Requester;
            role.Name = "Requester";
            role.Level = 1;
            roles.Add(role);

            role = new Role(Role.Codes.Approver);
            role.Id = Role.Codes.Approver;
            role.Name = "Approver";
            role.Level = 2;
            roles.Add(role);

            role = new Role(Role.Codes.AccountManager);
            role.Id = Role.Codes.AccountManager;
            role.Name = "Account Manager";
            role.Level = 3;
            roles.Add(role);

            role = new Role(Role.Codes.Purchaser);
            role.Id = Role.Codes.Purchaser;
            role.Name = "Purchaser";
            role.Level = 4;
            roles.Add(role);

            new FakeRoles(0, RoleRepository, roles, true);
        }

        public void SetupDataForWorkgroupActions1(bool skipOrg = false)
        {

            var organizations = new List<Organization>();
            for(int i = 0; i < 9; i++)
            {
                organizations.Add(CreateValidEntities.Organization(i + 1));
                organizations[i].Id = (i + 1).ToString();
            }

            var users = new List<User>();
            for(int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Id = (i + 1).ToString();
                users[i].Organizations = new List<Organization>();
                users[i].Organizations.Add(organizations[(i * 3) + 0]);
                users[i].Organizations.Add(organizations[(i * 3) + 1]);
                users[i].Organizations.Add(organizations[(i * 3) + 2]);
            }
            new FakeUsers(0, UserRepository, users, true);
            Mock.Get(UserRepository2).SetupGet(a => a.Queryable).Returns(users.AsQueryable());
            var workgroups = new List<Workgroup>();
            for(int i = 0; i < 9; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].Organizations = new List<Organization>();
                workgroups[i].Organizations.Add(organizations[0]);
                workgroups[i].Organizations.Add(organizations[i]);
            }
            if(!skipOrg)
            {
                Mock.Get(OrganizationRepository).SetupGet(a => a.Queryable).Returns(organizations.AsQueryable());
            }
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
        }

        public void SetupDataForDetails()
        {
            #region Setup Roles
            var roles = new List<Role>();
            SetupRoles(roles);
            #endregion Setup Roles

            #region Setup Users
            var users = new List<User>();
            for(int i = 0; i < 6; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Id = (i + 1).ToString();
            }

            new FakeUsers(0, UserRepository, users, true);
            #endregion Setup Users

            #region Setup Workgroups
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i+1));
            }
            workgroups[2].Organizations = new List<Organization>();
            workgroups[2].Accounts = new List<WorkgroupAccount>();
            workgroups[2].Vendors = new List<WorkgroupVendor>();
            workgroups[2].Addresses = new List<WorkgroupAddress>();
            
            workgroups[2].Organizations.Add(CreateValidEntities.Organization(1));
            workgroups[2].Organizations.Add(CreateValidEntities.Organization(2));

            workgroups[2].Accounts.Add(CreateValidEntities.WorkgroupAccount(1));
            workgroups[2].Accounts.Add(CreateValidEntities.WorkgroupAccount(2));
            workgroups[2].Accounts.Add(CreateValidEntities.WorkgroupAccount(3));

            workgroups[2].Vendors.Add(new WorkgroupVendor());

            
            workgroups[2].Addresses.Add(new WorkgroupAddress());
            workgroups[2].Addresses.Add(new WorkgroupAddress());
            workgroups[2].Addresses.Add(new WorkgroupAddress());
            workgroups[2].Addresses.Add(new WorkgroupAddress());
            new FakeWorkgroups(3, WorkgroupRepository, workgroups);
            #endregion Setup Workgroups

            #region Setup WorkgroupPermissions
            var workgroupPermissions = new List<WorkgroupPermission>();
            for(int i = 0; i < 18; i++)
            {
                workgroupPermissions.Add(new WorkgroupPermission());
                workgroupPermissions[i].Role = roles[(i % 6)];
                workgroupPermissions[i].User = users[(i % 6)];
                workgroupPermissions[i].Workgroup = WorkgroupRepository.GetNullableById((i / 3) + 1);
            }
            for(int i = 0; i < 9; i++)
            {
                workgroupPermissions.Add(new WorkgroupPermission());
                workgroupPermissions[i + 18].Workgroup = WorkgroupRepository.GetNullableById(3);
                workgroupPermissions[i + 18].User = users[2];
            }

            workgroupPermissions[18].Role = RoleRepository.GetNullableById(Role.Codes.Approver);
            workgroupPermissions[19].Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            workgroupPermissions[20].Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
            workgroupPermissions[21].Role = RoleRepository.GetNullableById(Role.Codes.Approver);
            workgroupPermissions[22].Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            workgroupPermissions[23].Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            workgroupPermissions[24].Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
            workgroupPermissions[25].Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
            workgroupPermissions[26].Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);


            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Setup WorkgroupPermissions
        }

        public void SetupDataForAddress()
        {
            new FakeStates(3, StateRepository);
            var workgroups = new List<Workgroup>();
            for(int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                for(int j = 0; j < 3; j++)
                {
                    var address = CreateValidEntities.WorkgroupAddress((i * 3) + (j + 1));
                    address.Id = (i * 3) + (j + 1);
                    if(j == 1)
                    {
                        address.IsActive = false;
                    }
                    address.State = "2";
                    workgroups[i].AddAddress(address);
                }
            }
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
        }

        public void SetupDataForAccounts1(bool skip = false)
        {
            #region Setup Roles
            var roles = new List<Role>();
            SetupRoles(roles);
            #endregion Setup Roles

            #region Setup Users
            var users = new List<User>();
            for(int i = 0; i < 6; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Id = (i + 1).ToString();
            }

            new FakeUsers(0, UserRepository, users, true);
            #endregion Setup Users

            var workgroups = new List<Workgroup>();
            for(int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
            }
            var organizations = new List<Organization>();
            for(int i = 0; i < 3; i++)
            {
                organizations.Add(CreateValidEntities.Organization(i + 1));
                organizations[i].Accounts = new List<Account>();
                for(int j = 0; j < 3; j++)
                {
                    var account = CreateValidEntities.Account((i * 3) + (j + 1));
                    account.Id = (i * 3) + (j + 1).ToString();
                    organizations[i].Accounts.Add(account);
                }
            }
            organizations.Add(CreateValidEntities.Organization(4));
            organizations[3].Accounts = new List<Account>();
            organizations[3].Accounts.Add(organizations[0].Accounts[0]);
            var account2 = CreateValidEntities.Account(10);
            account2.Id = "10";
            organizations[3].Accounts.Add(account2);
            workgroups[2].Organizations = organizations;
            workgroups[1].Organizations.Add(CreateValidEntities.Organization(5));
            workgroups[1].Organizations[0].Accounts = new List<Account>();
            var account3 = CreateValidEntities.Account(11);
            account3.Id = "11";
            workgroups[1].Organizations[0].Accounts.Add(account3);
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);

            #region Setup WorkgroupPermissions
            var workgroupPermissions = new List<WorkgroupPermission>();
            for(int i = 0; i < 18; i++)
            {
                workgroupPermissions.Add(new WorkgroupPermission());
                workgroupPermissions[i].Role = roles[(i % 6)];
                workgroupPermissions[i].User = users[(i % 6)];
                workgroupPermissions[i].Workgroup = WorkgroupRepository.GetNullableById((i / 3) + 1);
            }
            for(int i = 0; i < 9; i++)
            {
                workgroupPermissions.Add(new WorkgroupPermission());
                workgroupPermissions[i + 18].Workgroup = WorkgroupRepository.GetNullableById(3);
                workgroupPermissions[i + 18].User = users[2];
            }

            workgroupPermissions[18].Role = RoleRepository.GetNullableById(Role.Codes.Approver);
            workgroupPermissions[19].Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            workgroupPermissions[20].Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
            workgroupPermissions[21].Role = RoleRepository.GetNullableById(Role.Codes.Approver);
            workgroupPermissions[22].Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            workgroupPermissions[23].Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            workgroupPermissions[24].Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
            workgroupPermissions[25].Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
            workgroupPermissions[26].Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);


            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Setup WorkgroupPermissions           

            if(!skip)
            {
                new FakeWorkgroupAccounts(1, WorkgroupAccountRepository);
            }
        }

        private void SetupDataForVendors1()
        {
            new FakeWorkgroups(3, WorkgroupRepository);
            var vendors = new List<WorkgroupVendor>();
            for(var i = 0; i < 2; i++)
            {
                for(var j = 0; j < 3; j++)
                {
                    var vendor = CreateValidEntities.WorkgroupVendor((i * 3) + (j + 1));
                    vendor.Workgroup = WorkgroupRepository.GetNullableById(i + 1);
                    vendors.Add(vendor);
                }
            }
            vendors[1].IsActive = false;
            new FakeWorkgroupVendors(0, WorkgroupVendorRepository, vendors);
        }

        public void SetupDataForVendors2()
        {
            new FakeWorkgroups(3, WorkgroupRepository);
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(9));
            vendors[0].Id = "VendorId9";
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddresses = new List<VendorAddress>();
            for(int i = 0; i < 3; i++)
            {
                var vendorAddress = CreateValidEntities.VendorAddress(i + 1);
                vendorAddress.Vendor = VendorRepository.GetNullableById("VendorId9");
                vendorAddresses.Add(vendorAddress);
            }
            vendorAddresses[1].TypeCode = "tc9";
            vendorAddresses[1].Line2 = "Line2";
            vendorAddresses[1].Line3 = "Line3";
            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddresses, false);
        }

        public void SetupDataForVendors3()
        {
            new FakeWorkgroups(3, WorkgroupRepository);
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(9));
            vendors[0].Id = "VendorId9";
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddresses = new List<VendorAddress>();
            for(int i = 0; i < 3; i++)
            {
                var vendorAddress = CreateValidEntities.VendorAddress(i + 1);
                vendorAddress.Vendor = VendorRepository.GetNullableById("VendorId9");
                vendorAddresses.Add(vendorAddress);
            }
            vendorAddresses[1].Line1 = string.Empty; //Invalid
            vendorAddresses[1].TypeCode = "tc9";
            vendorAddresses[1].Line2 = "Line2";
            vendorAddresses[1].Line3 = "Line3";
            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddresses, false);
        }

        #endregion Helpers
    }
}
