using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Helpers;
using Purchasing.Web.Services;
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
        protected IRepository<Organization> OrganizationRepository;
        protected IRepository<WorkgroupAccount> WorkgroupAccountRepository;
        protected IWorkgroupAddressService WorkgroupAddressService;
        protected IWorkgroupService WorkgroupService;

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
                WorkgroupAddressService,
                WorkgroupService);
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            //RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();
            base.RegisterAdditionalServices(container);
        }

        public WizardControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Workgroup>()).Return(WorkgroupRepository).Repeat.Any();	
        }
        #endregion Init






    }
}
