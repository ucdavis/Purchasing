using System;
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

            Controller = new TestControllerBuilder().CreateController<AdminController>(UserRepository, RoleRepository, OrganizationRepository,SearchService, EmailPreferencesRepository);
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes();
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();

            //Fixes problem where .Fetch is used in a query
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));

            base.RegisterAdditionalServices(container);
        }

        #endregion Init
    }
}
