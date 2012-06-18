using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;



namespace Purchasing.Tests.ControllerTests.HistoryControllerTests
{
    [TestClass]
    public partial class HistoryControllerTests : ControllerTestBase<HistoryController>
    {
        protected readonly Type ControllerClass = typeof(HistoryController);
        protected IRepositoryFactory RepositoryFactory;
        protected IQueryRepositoryFactory QueryRepositoryFactory;
        protected IOrderService OrderService;
        protected IRepository<OrderHistory> OrderHistoryRepository; 

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();
            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
            OrderService = MockRepository.GenerateStub<IOrderService>();
            Controller = new TestControllerBuilder().CreateController<HistoryController>(RepositoryFactory, QueryRepositoryFactory, OrderService);
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes(); 
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();
            base.RegisterAdditionalServices(container);
        }

        public HistoryControllerTests()
        {
            OrderHistoryRepository = MockRepository.GenerateStub<IRepository<OrderHistory>>();

            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            //Controller.Repository.Expect(a => a.OfType<History>()).Return(HistoryRepository).Repeat.Any();	
        }
        #endregion Init





    }
}
