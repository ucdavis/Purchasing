﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Purchasing.Core;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Purchasing.Core.Services;

namespace Purchasing.Tests.ControllerTests.WizardControllerTests
{
    [TestClass]
    public partial class WizardControllerTests : ControllerTestBase<WizardController>
    {
        private readonly Type _controllerClass = typeof(WizardController);
        protected IRepositoryWithTypedId<User, string> UserRepository;
        protected IRepository<User> UserRepository2; 
        protected IRepositoryWithTypedId<Role, string> RoleRepository;
        protected ISecurityService SecurityService;
        protected IDirectorySearchService SearchService;
        protected readonly Type ControllerClass = typeof(WizardController);
        protected IRepository<Workgroup> WorkgroupRepository;
        protected IRepository<WorkgroupPermission> WorkgroupPermissionRepository;
        protected IRepository<WorkgroupVendor> WorkgroupVendorRepository;
        protected IRepositoryWithTypedId<Vendor, string> VendorRepository;
        protected IRepositoryWithTypedId<VendorAddress, Guid> VendorAddressRepository;
        protected IRepositoryWithTypedId<State, string> StateRepository;
        protected IRepository<Organization> OrganizationRepository;
        protected IRepository<WorkgroupAccount> WorkgroupAccountRepository;
        protected IWorkgroupAddressService WorkgroupAddressService;
        protected IWorkgroupService WorkgroupService;
        protected IQueryRepositoryFactory QueryRepositoryFactory;
        protected IRepository<OrganizationDescendant> OrganizationDescendantRepository;
        protected IRepositoryFactory RepositoryFactory;
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
            WorkgroupAccountRepository = Mock.Of<IRepository<WorkgroupAccount>>();
            WorkgroupAddressService = Mock.Of<IWorkgroupAddressService>();
            WorkgroupService = Mock.Of<IWorkgroupService>();
            QueryRepositoryFactory = Mock.Of<IQueryRepositoryFactory>();
            OrganizationDescendantRepository = Mock.Of<IRepository<OrganizationDescendant>>();
            Mock.Get(QueryRepositoryFactory).SetupGet(r => r.OrganizationDescendantRepository).Returns(OrganizationDescendantRepository);
            RepositoryFactory = Mock.Of<IRepositoryFactory>();
            AggieEnterpriseService = Mock.Of<IAggieEnterpriseService>();

            Controller = new WizardController(WorkgroupRepository,
                UserRepository,
                RoleRepository,
                WorkgroupPermissionRepository,
                SecurityService,
                SearchService,
                WorkgroupVendorRepository,
                VendorRepository,
                VendorAddressRepository,
                StateRepository,
                WorkgroupAccountRepository,
                QueryRepositoryFactory,
                WorkgroupAddressService,
                WorkgroupService,
                RepositoryFactory,
                AggieEnterpriseService);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());
            SecurityService = Mock.Of<ISecurityService>();
            //Fixes problem where .Fetch is used in a query
            //container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));
            //container.Kernel.AddComponentInstance<ISecurityService>(SecurityService);
            container.Kernel.Register(Component.For<ISecurityService>().Instance(SecurityService));
            base.RegisterAdditionalServices(container);
        }

        public WizardControllerTests()
        {
            UserRepository2 = FakeRepository<User>();
            Mock.Get(Controller.Repository).Setup(a => a.OfType<User>()).Returns(UserRepository2);


            OrganizationRepository = FakeRepository<Organization>();
            Mock.Get(Controller.Repository).Setup(a => a.OfType<Organization>()).Returns(OrganizationRepository);

            Mock.Get(Controller.Repository).Setup(a => a.OfType<Workgroup>()).Returns(WorkgroupRepository);

            Mock.Get(Controller.Repository).Setup(a => a.OfType<WorkgroupPermission>()).Returns(WorkgroupPermissionRepository);
            
        }
        #endregion Init


        #region Helpers
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
        #endregion Helpers



    }
}
