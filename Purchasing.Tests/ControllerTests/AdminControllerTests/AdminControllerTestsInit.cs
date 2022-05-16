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
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public HttpContext HttpContext;


        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            UserRepository = new Mock<IRepositoryWithTypedId<User, string>>().Object;
            RoleRepository = new Mock<IRepositoryWithTypedId<Role, string>>().Object;
            OrganizationRepository = new Mock<IRepositoryWithTypedId<Organization, string>>().Object;
            SearchService = new Mock<IDirectorySearchService>().Object;
            EmailPreferencesRepository = new Mock<IRepositoryWithTypedId<EmailPreferences, string>>().Object;
            UserIdentity = new Mock<IUserIdentity>().Object;

            WorkgroupService = new Mock<IWorkgroupService>().Object;
            RepositoryFactory = new Mock<IRepositoryFactory>().Object;
            Mock.Get(RepositoryFactory).SetupGet(r => r.WorkgroupRepository).Returns(new Mock<IRepository<Workgroup>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.WorkgroupPermissionRepository).Returns(new Mock<IRepository<WorkgroupPermission>>().Object);
            Configuration = new Mock<IConfiguration>().Object;
            SendGridSettings = new Mock<IOptions<SendGridSettings>>().Object;
            HttpContext = new DefaultHttpContext();

            Controller = new AdminController(UserRepository, RoleRepository, OrganizationRepository, SearchService, EmailPreferencesRepository, UserIdentity, RepositoryFactory, WorkgroupService, SendGridSettings, Configuration)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = HttpContext
                },
                TempData = new TempDataDictionary(HttpContext, Mock.Of<ITempDataProvider>())
            };
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
