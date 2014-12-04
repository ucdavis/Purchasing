using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
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

namespace Purchasing.Tests.ControllerTests.DepartmentalAdminRequestControllerTests
{
    [TestClass]
    public partial class DepartmentalAdminRequestControllerTests : ControllerTestBase<DepartmentalAdminRequestController>
    {
        protected readonly Type ControllerClass = typeof(DepartmentalAdminRequestController);
        public IRepositoryWithTypedId<DepartmentalAdminRequest, string> DepartmentalAdminRequestRepository;
        public IRepositoryFactory RepositoryFactory;
        public IQueryRepositoryFactory QueryRepositoryFactory;
        public IDirectorySearchService DirectorySearchService;
        public IUserIdentity UserIdentity;
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository;
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IRepositoryWithTypedId<Role, string> RoleRepository; 

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            DepartmentalAdminRequestRepository =
                MockRepository.GenerateStub<IRepositoryWithTypedId<DepartmentalAdminRequest, string>>();
            OrganizationRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Organization, string>>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            RoleRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Role, string>>();

            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();
            RepositoryFactory.OrganizationRepository = OrganizationRepository;
            RepositoryFactory.UserRepository = UserRepository;
            RepositoryFactory.RoleRepository = RoleRepository;

            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
            QueryRepositoryFactory.OrganizationDescendantRepository = MockRepository.GenerateStub<IRepository<OrganizationDescendant>>();

            DirectorySearchService = MockRepository.GenerateStub<IDirectorySearchService>();
            UserIdentity = MockRepository.GenerateStub<IUserIdentity>();
            Controller =
                new TestControllerBuilder().CreateController<DepartmentalAdminRequestController>(
                    DepartmentalAdminRequestRepository,
                    RepositoryFactory,
                    QueryRepositoryFactory,
                    DirectorySearchService,
                    UserIdentity);
        }

        protected override void RegisterRoutes()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();
            base.RegisterAdditionalServices(container);
        }

        public DepartmentalAdminRequestControllerTests()
        {
            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(1));
            roles[0].SetIdTo(Role.Codes.DepartmentalAdmin);
            roles[0].IsAdmin = true;
            new FakeRoles(0, RoleRepository, roles, true);
            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            //Controller.Repository.Expect(a => a.OfType<DepartmentalAdminRequest>()).Return(DepartmentalAdminRequestRepository).Repeat.Any();	
        }
        #endregion Init
    }
}
