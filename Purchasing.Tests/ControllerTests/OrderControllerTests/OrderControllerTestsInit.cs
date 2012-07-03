using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Purchasing.Core.Queries;
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
        public IBugTrackingService BugTrackingService;
        public IRepositoryWithTypedId<User, string> UserRepository; 
        public IRepository<User> UserRepository2;
        public IRepositoryWithTypedId<Role, string> RoleRepository;
        public IRepository<OrderPeep> OrderPeepRepository; 

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
            BugTrackingService = MockRepository.GenerateStub<IBugTrackingService>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            RoleRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Role, string>>();
            OrderPeepRepository = MockRepository.GenerateStub<IRepository<OrderPeep>>();

            RepositoryFactory.ColumnPreferencesRepository = ColumnPreferencesRepository;
            RepositoryFactory.OrderRepository = OrderRepository;
            RepositoryFactory.OrderStatusCodeRepository = OrderStatusCodeRepository;
            RepositoryFactory.RoleRepository = RoleRepository;
            RepositoryFactory.UserRepository = UserRepository;

            QueryRepositoryFactory.OrderPeepRepository = OrderPeepRepository;

            Controller = new TestControllerBuilder().CreateController<OrderController>(
                RepositoryFactory,
                OrderService,
                SecurityService,
                DirectorySearchService,
                FinancialSystemService,
                QueryRepositoryFactory,
                EventService,
                BugTrackingService);
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

            UserRepository2 = MockRepository.GenerateStub<IRepository<User>>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository2).Repeat.Any();

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

        protected void SetupRoles()
        {
            var roles = new List<Role>();

            var role = new Role(Role.Codes.Admin);
            role.SetIdTo(Role.Codes.Admin);
            role.Name = "Admin";
            role.Level = 0;
            role.IsAdmin = true;
            roles.Add(role);

            role = new Role(Role.Codes.DepartmentalAdmin);
            role.SetIdTo(Role.Codes.DepartmentalAdmin);
            role.Name = "Departmental Admin";
            role.Level = 0;
            role.IsAdmin = true;
            roles.Add(role);

            role = new Role(Role.Codes.Requester);
            role.SetIdTo(Role.Codes.Requester);
            role.Name = "Requester";
            role.Level = 1;
            roles.Add(role);

            role = new Role(Role.Codes.Approver);
            role.SetIdTo(Role.Codes.Approver);
            role.Name = "Approver";
            role.Level = 2;
            roles.Add(role);

            role = new Role(Role.Codes.AccountManager);
            role.SetIdTo(Role.Codes.AccountManager);
            role.Name = "Account Manager";
            role.Level = 3;
            roles.Add(role);

            role = new Role(Role.Codes.Purchaser);
            role.SetIdTo(Role.Codes.Purchaser);
            role.Name = "Purchaser";
            role.Level = 4;
            roles.Add(role);

            new FakeRoles(0, RoleRepository, roles, true);
        }

    }
}
