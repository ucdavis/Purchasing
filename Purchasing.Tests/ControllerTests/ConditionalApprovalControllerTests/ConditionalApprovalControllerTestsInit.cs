using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Purchasing.Core;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
            UserRepository = Mock.Of<IRepositoryWithTypedId<User, string>>();
            DirectorySearchService = Mock.Of<IDirectorySearchService>();
            SecurityService = Mock.Of<ISecurityService>();

            OrganizationRepository = Mock.Of<IRepositoryWithTypedId<Organization, string>>();
            QueryRepositoryFactory = Mock.Of<IQueryRepositoryFactory>();
            AdminWorkgroupRepository = Mock.Of<IRepository<AdminWorkgroup>>();
            Mock.Get(QueryRepositoryFactory).SetupGet(r => r.AdminWorkgroupRepository).Returns(AdminWorkgroupRepository);

            Controller = new ConditionalApprovalController(
                ConditionalApprovalRepository, 
                WorkgroupRepository,
                UserRepository,
                OrganizationRepository,
                DirectorySearchService, 
                SecurityService,
                QueryRepositoryFactory);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());

            //Fixes problem where .Fetch is used in a query
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));

            base.RegisterAdditionalServices(container);
        }

        public ConditionalApprovalControllerTests()
        {
            Mock.Get(Controller.Repository).Setup(a => a.OfType<ConditionalApproval>()).Returns(ConditionalApprovalRepository);
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
                    organization.Id = ((i * 4) + (j + 1)).ToString();
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
