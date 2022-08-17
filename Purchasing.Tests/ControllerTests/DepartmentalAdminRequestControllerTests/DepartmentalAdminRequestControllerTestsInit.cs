using System;
using System.Collections.Generic;
using System.Linq;
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
using Moq;

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
                Mock.Of<IRepositoryWithTypedId<DepartmentalAdminRequest, string>>();
            OrganizationRepository = Mock.Of<IRepositoryWithTypedId<Organization, string>>();
            UserRepository = Mock.Of<IRepositoryWithTypedId<User, string>>();
            RoleRepository = Mock.Of<IRepositoryWithTypedId<Role, string>>();

            RepositoryFactory = Mock.Of<IRepositoryFactory>();
            Mock.Get(RepositoryFactory).SetupGet(r => r.OrganizationRepository).Returns(OrganizationRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.UserRepository).Returns(UserRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.RoleRepository).Returns(RoleRepository);

            QueryRepositoryFactory = Mock.Of<IQueryRepositoryFactory>();
            Mock.Get(QueryRepositoryFactory).SetupGet(r => r.OrganizationDescendantRepository).Returns(Mock.Of<IRepository<OrganizationDescendant>>());

            DirectorySearchService = Mock.Of<IDirectorySearchService>();
            UserIdentity = Mock.Of<IUserIdentity>();
            Controller =
                new DepartmentalAdminRequestController(
                    DepartmentalAdminRequestRepository,
                    RepositoryFactory,
                    QueryRepositoryFactory,
                    DirectorySearchService,
                    UserIdentity);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());
            base.RegisterAdditionalServices(container);
        }

        public DepartmentalAdminRequestControllerTests()
        {
            var roles = new List<Role>();
            roles.Add(CreateValidEntities.Role(1));
            roles[0].Id = Role.Codes.DepartmentalAdmin;
            roles[0].IsAdmin = true;
            new FakeRoles(0, RoleRepository, roles, true);
            //    ExampleRepository = FakeRepository<Example>();
            //    Mock.Get(Controller.Repository).Setup(a => a.OfType<Example>()).Returns(ExampleRepository);

            //Mock.Get(Controller.Repository).Setup(a => a.OfType<DepartmentalAdminRequest>()).Returns(DepartmentalAdminRequestRepository);	
        }
        #endregion Init
    }
}
