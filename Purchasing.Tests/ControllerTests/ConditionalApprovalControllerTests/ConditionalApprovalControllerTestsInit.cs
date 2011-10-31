using System;
using System.Linq;
using Castle.Windsor;
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

            Controller = new TestControllerBuilder().CreateController<ConditionalApprovalController>(ConditionalApprovalRepository, WorkgroupRepository, UserRepository, DirectorySearchService);
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

        public ConditionalApprovalControllerTests()
        {
            Controller.Repository.Expect(a => a.OfType<ConditionalApproval>()).Return(ConditionalApprovalRepository).Repeat.Any();	
        }
        #endregion Init



        

        
    }
}
