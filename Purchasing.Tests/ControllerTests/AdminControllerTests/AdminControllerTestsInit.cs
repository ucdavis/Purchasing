using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.ControllerTests.AdminControllerTests
{
    [TestClass]
    public partial class AdminControllerTests : ControllerTestBase<AdminController>
    {
        protected readonly Type ControllerClass = typeof(AdminController);
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IRepositoryWithTypedId<Role, string> RoleRepository;
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository;
        public IDirectorySearchService SearchService;
        public IRepositoryWithTypedId<EmailPreferences, string> EmailPreferencesRepository;
        public IUserIdentity UserIdentity;
        public IRepositoryFactory RepositoryFactory;
        public IWorkgroupService WorkgroupService;


        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            RoleRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Role, string>>();
            OrganizationRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Organization, string>>();
            SearchService = MockRepository.GenerateStub<IDirectorySearchService>();
            EmailPreferencesRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<EmailPreferences, string>>();
            UserIdentity = MockRepository.GenerateStub<IUserIdentity>();

            WorkgroupService = MockRepository.GenerateStub<IWorkgroupService>();
            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();
            RepositoryFactory.WorkgroupRepository = MockRepository.GenerateStub<IRepository<Workgroup>>();
            RepositoryFactory.WorkgroupPermissionRepository = MockRepository.GenerateStub<IRepository<WorkgroupPermission>>();

            Controller = new TestControllerBuilder().CreateController<AdminController>(UserRepository, RoleRepository, OrganizationRepository,SearchService, EmailPreferencesRepository, UserIdentity, RepositoryFactory, WorkgroupService);
        }

        protected override void RegisterRoutes()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();

            //Fixes problem where .Fetch is used in a query
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));

            base.RegisterAdditionalServices(container);
        }

        #endregion Init


        #region Helpers
        protected void SetupRoles()
        {
            var roles = new List<Role>();

            var role = new Role(Role.Codes.Admin);
            role.SetIdTo(Role.Codes.Admin);
            role.Name = "Admin";
            role.Level = 0;
            role.IsAdmin = true;
            roles.Add(role);

            role = new Role(Role.Codes.DepartmentalAdmin);
            role.SetIdTo(Role.Codes.DepartmentalAdmin);
            role.Name = "Departmental Admin";
            role.Level = 0;
            role.IsAdmin = true;
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

            role = new Role(Role.Codes.SscAdmin);
            role.SetIdTo(Role.Codes.SscAdmin);
            role.Name = "SSC Admin";
            role.Level = 0;
            role.IsAdmin = true;
            roles.Add(role);

            new FakeRoles(0, RoleRepository, roles, true);
        } 
        #endregion Helpers
    }
}
