using System;
using System.Linq;
using Purchasing.Web;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Controllers.Dev;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace Purchasing.Tests.ControllerTests.AutoApprovalControllerTests
{
    [TestClass]
    public partial class AutoApprovalControllerTests : ControllerTestBase<AutoApprovalController>
    {
        public IRepositoryWithTypedId<User, string> UserRepository;
        private readonly Type _controllerClass = typeof(AutoApprovalController);
        public IRepository<AutoApproval> AutoApprovalRepository;
        //public IExampleService ExampleService;
        //public IRepository<Example> ExampleRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            AutoApprovalRepository = FakeRepository<AutoApproval>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();

            //ExampleService = MockRepository.GenerateStub<IExampleService>();  
            Controller = new TestControllerBuilder().CreateController<AutoApprovalController>(AutoApprovalRepository, UserRepository);
            //Controller = new TestControllerBuilder().CreateController<AutoApprovalController>(AutoApprovalRepository, ExampleService);
        }

        protected override void RegisterRoutes()
        {
           new RouteConfigurator().RegisterRoutes();
            
        }

        public AutoApprovalControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            //Controller.Repository.Expect(a => a.OfType<AutoApproval>()).Return(AutoApprovalRepository).Repeat.Any();

        }
        #endregion Init

        
    }
}
