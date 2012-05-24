using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Core.Domain;
using Purchasing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Controllers;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using Purchasing.WS;

namespace Purchasing.Tests.ControllerTests.OrderControllerTests
{
    [TestClass]
    public partial class OrderControllerTests : ControllerTestBase<OrderController>
    {
        protected readonly Type ControllerClass = typeof(OrderController);
        public IRepository<Order> OrderRepository;
        public IRepositoryFactory RepositoryFactory;
        public IOrderService OrderService;
        public ISecurityService SecurityService;
        public IDirectorySearchService DirectorySearchService;
        public IFinancialSystemService FinancialSystemService;
        public IQueryRepositoryFactory QueryRepositoryFactory;
        public IEventService EventService;

        public IRepositoryWithTypedId<ColumnPreferences, string> ColumnPreferencesRepository;
        public IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodeRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            OrderRepository = FakeRepository<Order>();
            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();
            OrderService = MockRepository.GenerateStub<IOrderService>();
            SecurityService = MockRepository.GenerateStub<ISecurityService>();
            DirectorySearchService = MockRepository.GenerateStub<IDirectorySearchService>();
            FinancialSystemService = MockRepository.GenerateStub<IFinancialSystemService>();
            ColumnPreferencesRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<ColumnPreferences, string>>();
            OrderStatusCodeRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OrderStatusCode, string>>();
            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
            EventService = MockRepository.GenerateStub<IEventService>();

            RepositoryFactory.ColumnPreferencesRepository = ColumnPreferencesRepository;
            RepositoryFactory.OrderRepository = OrderRepository;
            RepositoryFactory.OrderStatusCodeRepository = OrderStatusCodeRepository;

            Controller = new TestControllerBuilder().CreateController<OrderController>(
                RepositoryFactory,
                OrderService,
                SecurityService,
                DirectorySearchService,
                FinancialSystemService,
                QueryRepositoryFactory,
                EventService);
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

        public OrderControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Order>()).Return(OrderRepository).Repeat.Any();	
        }
        #endregion Init



        protected void SetupDataForTests1()
        {
                        var statusCodes = new List<OrderStatusCode>();

            for (int i = 0; i < 5; i++)
            {
                statusCodes.Add(CreateValidEntities.OrderStatusCode(i+1));
                statusCodes[i].Level = i + 1;
            }

            statusCodes[3].ShowInFilterList = true;
            statusCodes[2].ShowInFilterList = true;

            new FakeOrderStatusCodes(0, OrderStatusCodeRepository, statusCodes, false);
            var orders = new List<Order>();
            for (int i = 0; i < 3; i++)
            {
                orders.Add(CreateValidEntities.Order(i+1));
                orders[i].Workgroup = CreateValidEntities.Workgroup(i + 1);
                orders[i].CreatedBy = CreateValidEntities.User(i + 1);
                orders[i].StatusCode = OrderStatusCodeRepository.Queryable.First();
            }


            new FakeOrders(0, OrderRepository, orders);
            
            //OrderService.Expect(a => a.GetListofOrders(Arg<bool>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything)).Return(OrderRepository.Queryable);
        }

    }
}
