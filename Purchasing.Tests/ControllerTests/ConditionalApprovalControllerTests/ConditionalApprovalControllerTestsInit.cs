using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Purchasing.Core;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;



namespace Purchasing.Tests.ControllerTests.ConditionalApprovalControllerTests
{
    [TestClass]
    public partial class ConditionalApprovalControllerTests : ControllerTestBase<ConditionalApprovalController>
    {
        public Type ControllerClass = typeof(ConditionalApprovalController);
        public IRepository<ConditionalApproval> ConditionalApprovalRepository;
        public IRepository<Workgroup> WorkgroupRepository;
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IDirectorySearchService DirectorySearchService;

        public IRepositoryWithTypedId<Organization, string> OrganizationRepository;
        public ISecurityService SecurityService;
        public IQueryRepositoryFactory QueryRepositoryFactory;
        public IRepository<AdminWorkgroup> AdminWorkgroupRepository; 

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            ConditionalApprovalRepository = FakeRepository<ConditionalApproval>();
            WorkgroupRepository = FakeRepository<Workgroup>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            DirectorySearchService = MockRepository.GenerateStub<IDirectorySearchService>();
            SecurityService = MockRepository.GenerateStub<ISecurityService>();

            OrganizationRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Organization, string>>();
            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
            AdminWorkgroupRepository = MockRepository.GenerateStub<IRepository<AdminWorkgroup>>();
            QueryRepositoryFactory.AdminWorkgroupRepository = AdminWorkgroupRepository;

            Controller = new TestControllerBuilder().CreateController<ConditionalApprovalController>(
                ConditionalApprovalRepository, 
                WorkgroupRepository,
                UserRepository,
                OrganizationRepository,
                DirectorySearchService, 
                SecurityService,
                QueryRepositoryFactory);
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            //RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();

            //Fixes problem where .Fetch is used in a query
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));

            base.RegisterAdditionalServices(container);
        }

        public ConditionalApprovalControllerTests()
        {
            Controller.Repository.Expect(a => a.OfType<ConditionalApproval>()).Return(ConditionalApprovalRepository).Repeat.Any();	
        }
        #endregion Init

        #region Setup Data
        public void SetupDateForIndex1()
        {
            var organizations = new List<Organization>();
            var users = new List<User>();
            for(int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Organizations = new List<Organization>();
                for(int j = 0; j < 4; j++)
                {
                    var organization = CreateValidEntities.Organization((i * 4) + (j + 1));
                    organization.Name = "O" + organization.Name;
                    organization.SetIdTo(((i * 4) + (j + 1)).ToString());
                    users[i].Organizations.Add(organization);
                    organizations.Add(organization);
                }
            }
            users.Add(CreateValidEntities.User(4));
            new FakeUsers(0, UserRepository, users, false);
            new FakeOrganizations(0, OrganizationRepository, organizations, true);

            var workgroups = new List<Workgroup>();
            for(int i = 0; i < 6; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i + 1));
                workgroups[i].Name = "W" + workgroups[i].Name;
                workgroups[i].Organizations = new List<Organization>();
                workgroups[i].Organizations.Add(OrganizationRepository.GetNullableById(((i * 2) + 1).ToString()));
                workgroups[i].Organizations.Add(OrganizationRepository.GetNullableById(((i * 2) + 2).ToString()));
            }
            workgroups[5].Organizations.Add(OrganizationRepository.GetNullableById("6"));
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);

            var conditionalApprovals = new List<ConditionalApproval>();
            for(int i = 0; i < 6; i++)
            {
                conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(i + 1));
                conditionalApprovals[i].Workgroup = WorkgroupRepository.GetNullableById(i + 1);
            }
            for(int i = 6; i < 18; i++)
            {
                conditionalApprovals.Add(CreateValidEntities.ConditionalApproval(i + 1));
                conditionalApprovals[i].Workgroup = null;
                conditionalApprovals[i].Organization = OrganizationRepository.GetNullableById((i - 5).ToString());
            }

            conditionalApprovals[0].PrimaryApprover = CreateValidEntities.User(99);
            conditionalApprovals[6].PrimaryApprover = CreateValidEntities.User(99);
            conditionalApprovals[6].SecondaryApprover = CreateValidEntities.User(88);

            new FakeConditionalApprovals(0, ConditionalApprovalRepository, conditionalApprovals);
        } 
        #endregion Setup Data

        

        
    }
}
