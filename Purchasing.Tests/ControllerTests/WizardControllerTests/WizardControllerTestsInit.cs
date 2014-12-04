using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Purchasing.Core;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


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
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            RoleRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Role, string>>();
            SecurityService = MockRepository.GenerateStub<ISecurityService>();
            SearchService = MockRepository.GenerateStub<IDirectorySearchService>();
            WorkgroupPermissionRepository = MockRepository.GenerateStub<IRepository<WorkgroupPermission>>();
            WorkgroupVendorRepository = MockRepository.GenerateStub<IRepository<WorkgroupVendor>>();
            VendorRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Vendor, string>>();
            VendorAddressRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<VendorAddress, Guid>>();
            StateRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<State, string>>();
            WorkgroupAccountRepository = MockRepository.GenerateStub<IRepository<WorkgroupAccount>>();
            WorkgroupAddressService = MockRepository.GenerateStub<IWorkgroupAddressService>();
            WorkgroupService = MockRepository.GenerateStub<IWorkgroupService>();
            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
            OrganizationDescendantRepository = MockRepository.GenerateStub<IRepository<OrganizationDescendant>>();
            QueryRepositoryFactory.OrganizationDescendantRepository = OrganizationDescendantRepository;
            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();

            Controller = new TestControllerBuilder().CreateController<WizardController>(WorkgroupRepository,
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

        protected override void RegisterRoutes()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);//Try this one if below doesn't work
            //RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();
            SecurityService = MockRepository.GenerateStub<ISecurityService>();
            //Fixes problem where .Fetch is used in a query
            //container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));
            //container.Kernel.AddComponentInstance<ISecurityService>(SecurityService);
            container.Kernel.Register(Component.For<ISecurityService>().Instance(SecurityService));
            base.RegisterAdditionalServices(container);
        }

        public WizardControllerTests()
        {
            UserRepository2 = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository2).Repeat.Any();


            OrganizationRepository = FakeRepository<Organization>();
            Controller.Repository.Expect(a => a.OfType<Organization>()).Return(OrganizationRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Workgroup>()).Return(WorkgroupRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<WorkgroupPermission>()).Return(WorkgroupPermissionRepository).Repeat.Any();
            
        }
        #endregion Init


        #region Helpers
        public void SetupDataForWorkgroupActions1(bool skipOrg = false)
        {

            var organizations = new List<Organization>();
            for(int i = 0; i < 9; i++)
            {
                organizations.Add(CreateValidEntities.Organization(i + 1));
                organizations[i].SetIdTo((i + 1).ToString());
            }

            var users = new List<User>();
            for(int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].SetIdTo((i + 1).ToString());
                users[i].Organizations = new List<Organization>();
                users[i].Organizations.Add(organizations[(i * 3) + 0]);
                users[i].Organizations.Add(organizations[(i * 3) + 1]);
                users[i].Organizations.Add(organizations[(i * 3) + 2]);
            }
            new FakeUsers(0, UserRepository, users, true);
            UserRepository2.Expect(a => a.Queryable).Return(users.AsQueryable()).Repeat.Any();
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
                OrganizationRepository.Expect(a => a.Queryable).Return(organizations.AsQueryable()).Repeat.Any();
            }
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
        }
        #endregion Helpers



    }
}
