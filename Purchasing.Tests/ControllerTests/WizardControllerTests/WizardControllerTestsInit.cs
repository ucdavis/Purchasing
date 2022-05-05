using System;
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

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            WorkgroupRepository = FakeRepository<Workgroup>();
            UserRepository = new Moq.Mock<IRepositoryWithTypedId<User, string>>().Object;
            RoleRepository = new Moq.Mock<IRepositoryWithTypedId<Role, string>>().Object;
            SecurityService = new Moq.Mock<ISecurityService>().Object;
            SearchService = new Moq.Mock<IDirectorySearchService>().Object;
            WorkgroupPermissionRepository = new Moq.Mock<IRepository<WorkgroupPermission>>().Object;
            WorkgroupVendorRepository = new Moq.Mock<IRepository<WorkgroupVendor>>().Object;
            VendorRepository = new Moq.Mock<IRepositoryWithTypedId<Vendor, string>>().Object;
            VendorAddressRepository = new Moq.Mock<IRepositoryWithTypedId<VendorAddress, Guid>>().Object;
            StateRepository = new Moq.Mock<IRepositoryWithTypedId<State, string>>().Object;
            WorkgroupAccountRepository = new Moq.Mock<IRepository<WorkgroupAccount>>().Object;
            WorkgroupAddressService = new Moq.Mock<IWorkgroupAddressService>().Object;
            WorkgroupService = new Moq.Mock<IWorkgroupService>().Object;
            QueryRepositoryFactory = new Moq.Mock<IQueryRepositoryFactory>().Object;
            OrganizationDescendantRepository = new Moq.Mock<IRepository<OrganizationDescendant>>().Object;
            QueryRepositoryFactory.OrganizationDescendantRepository = OrganizationDescendantRepository;
            RepositoryFactory = new Moq.Mock<IRepositoryFactory>().Object;

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
                RepositoryFactory);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());
            SecurityService = new Moq.Mock<ISecurityService>().Object;
            //Fixes problem where .Fetch is used in a query
            //container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));
            //container.Kernel.AddComponentInstance<ISecurityService>(SecurityService);
            container.Kernel.Register(Component.For<ISecurityService>().Instance(SecurityService));
            base.RegisterAdditionalServices(container);
        }

        public WizardControllerTests()
        {
            UserRepository2 = FakeRepository<User>();
            Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<User>()).Returns(UserRepository2);


            OrganizationRepository = FakeRepository<Organization>();
            Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<Organization>()).Returns(OrganizationRepository);

            Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<Workgroup>()).Returns(WorkgroupRepository);

            Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<WorkgroupPermission>()).Returns(WorkgroupPermissionRepository);
            
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
            Moq.Mock.Get(UserRepository2).SetupGet(a => a.Queryable).Returns(users.AsQueryable());
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
                Moq.Mock.Get(OrganizationRepository).SetupGet(a => a.Queryable).Returns(organizations.AsQueryable());
            }
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
        }
        #endregion Helpers



    }
}
