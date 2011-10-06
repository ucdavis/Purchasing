using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;


namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    [TestClass]
    public partial class WorkgroupControllerTests : ControllerTestBase<WorkgroupController>
    {
        protected IRepositoryWithTypedId<User, string> UserRepository;
        protected IRepositoryWithTypedId<Role, string> RoleRepository;
        protected IHasAccessService HasAccessService;
        protected IDirectorySearchService SearchService;
        protected readonly Type ControllerClass = typeof(WorkgroupController);
        protected IRepository<Workgroup> WorkgroupRepository;
        protected IRepository<WorkgroupPermission> WorkgroupPermissionRepository;
        protected IRepository<WorkgroupVendor> WorkgroupVendorRepository;
        protected IRepositoryWithTypedId<Vendor, string> VendorRepository;
        protected IRepository<VendorAddress> VendorAddressRepository;
        protected IRepositoryWithTypedId<State, string> StateRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            WorkgroupRepository = FakeRepository<Workgroup>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            RoleRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Role, string>>();
            HasAccessService = MockRepository.GenerateStub<IHasAccessService>();
            SearchService = MockRepository.GenerateStub<IDirectorySearchService>();
            WorkgroupPermissionRepository = MockRepository.GenerateStub<IRepository<WorkgroupPermission>>();
            WorkgroupVendorRepository = MockRepository.GenerateStub<IRepository<WorkgroupVendor>>();
            VendorRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Vendor, string>>();
            VendorAddressRepository = MockRepository.GenerateStub<IRepository<VendorAddress>>();
            StateRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<State, string>>();

            Controller = new TestControllerBuilder().CreateController<WorkgroupController>(WorkgroupRepository,
                UserRepository,
                RoleRepository,
                WorkgroupPermissionRepository,
                HasAccessService,
                SearchService,
                WorkgroupVendorRepository,
                VendorRepository,
                VendorAddressRepository,
                StateRepository);
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            //RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        protected override void InitServiceLocator()
        {
            var container = Core.ServiceLocatorInitializer.Init();

            RegisterAdditionalServices(container);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();

            //Fixes problem where .Fetch is used in a query
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));
            //container.AddComponent("queryExtensionProvider", typeof(IQueryExtensionProvider),
            //                       typeof(QueryExtensionFakes));


            base.RegisterAdditionalServices(container);
        }

        public WorkgroupControllerTests()
        {

            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Workgroup>()).Return(WorkgroupRepository).Repeat.Any();
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
                users[i].SetIdTo((i + 1).ToString());
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
            role.SetIdTo(Role.Codes.Admin);
            role.Name = "Admin";
            role.Level = 0;
            roles.Add(role);

            role = new Role(Role.Codes.DepartmentalAdmin);
            role.SetIdTo(Role.Codes.DepartmentalAdmin);
            role.Name = "Departmental Admin";
            role.Level = 0;
            roles.Add(role);

            role = new Role(Role.Codes.Requester);
            role.SetIdTo(Role.Codes.Requester);
            role.Name = "Requester";
            role.Level = 1;
            roles.Add(role);

            role = new Role(Role.Codes.Approver);
            role.SetIdTo(Role.Codes.Approver);
            role.Name = "Approver";
            role.Level = 2;
            roles.Add(role);

            role = new Role(Role.Codes.AccountManager);
            role.SetIdTo(Role.Codes.AccountManager);
            role.Name = "Account Manager";
            role.Level = 3;
            roles.Add(role);

            role = new Role(Role.Codes.Purchaser);
            role.SetIdTo(Role.Codes.Purchaser);
            role.Name = "Purchaser";
            role.Level = 4;
            roles.Add(role);

            new FakeRoles(0, RoleRepository, roles, true);
        }

        public void SetupDataForWorkgroupActions1()
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

            var workgroups = new List<Workgroup>();
            for(int i = 0; i < 9; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].Organizations = new List<Organization>();
                workgroups[i].Organizations.Add(organizations[0]);
                workgroups[i].Organizations.Add(organizations[i]);
            }

            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
        }
        #endregion Helpers
    }
}
