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
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

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
        public IConfiguration Configuration;
        public IOptions<SendGridSettings> SendGridSettings;


        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            UserRepository = new Moq.Mock<IRepositoryWithTypedId<User, string>>().Object;
            RoleRepository = new Moq.Mock<IRepositoryWithTypedId<Role, string>>().Object;
            OrganizationRepository = new Moq.Mock<IRepositoryWithTypedId<Organization, string>>().Object;
            SearchService = new Moq.Mock<IDirectorySearchService>().Object;
            EmailPreferencesRepository = new Moq.Mock<IRepositoryWithTypedId<EmailPreferences, string>>().Object;
            UserIdentity = new Moq.Mock<IUserIdentity>().Object;

            WorkgroupService = new Moq.Mock<IWorkgroupService>().Object;
            RepositoryFactory = new Moq.Mock<IRepositoryFactory>().Object;
            RepositoryFactory.WorkgroupRepository = new Moq.Mock<IRepository<Workgroup>>().Object;
            RepositoryFactory.WorkgroupPermissionRepository = new Moq.Mock<IRepository<WorkgroupPermission>>().Object;
            Configuration = new Moq.Mock<IConfiguration>().Object;
            SendGridSettings = new Moq.Mock<IOptions<SendGridSettings>>().Object;

            Controller = new AdminController(UserRepository, RoleRepository, OrganizationRepository, SearchService, EmailPreferencesRepository, UserIdentity, RepositoryFactory, WorkgroupService, SendGridSettings, Configuration);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());

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

            role = new Role(Role.Codes.SscAdmin);
            role.Id = Role.Codes.SscAdmin;
            role.Name = "SSC Admin";
            role.Level = 0;
            role.IsAdmin = true;
            roles.Add(role);

            new FakeRoles(0, RoleRepository, roles, true);
        }
        #endregion Helpers
    }
}
