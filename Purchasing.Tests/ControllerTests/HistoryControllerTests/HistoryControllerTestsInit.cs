using System;
using System.Collections.Generic;
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
        protected IRepositoryWithTypedId<ColumnPreferences, string> ColumnPreferencesRepository;
        protected IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodesRepository;
        protected IRepository<Approval> ApprovalRepository;
        protected IRepository<OrderTracking> OrderTrackingRepository;
        protected IRepository<OrderTrackingHistory> OrderTrackingHistoryRepository;
        protected IRepositoryWithTypedId<CommentHistory, Guid> CommentHistoryRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            ColumnPreferencesRepository =
                MockRepository.GenerateStub<IRepositoryWithTypedId<ColumnPreferences, string>>();
            OrderStatusCodesRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OrderStatusCode, string>>();
            ApprovalRepository = MockRepository.GenerateStub<IRepository<Approval>>();
            OrderTrackingRepository = MockRepository.GenerateStub<IRepository<OrderTracking>>();

            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();
            RepositoryFactory.ColumnPreferencesRepository = ColumnPreferencesRepository;
            RepositoryFactory.OrderStatusCodeRepository = OrderStatusCodesRepository;
            RepositoryFactory.ApprovalRepository = ApprovalRepository;
            RepositoryFactory.OrderTrackingRepository = OrderTrackingRepository;

            OrderTrackingHistoryRepository = MockRepository.GenerateStub<IRepository<OrderTrackingHistory>>();
            CommentHistoryRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<CommentHistory, Guid>>();

            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
            QueryRepositoryFactory.OrderTrackingHistoryRepository = OrderTrackingHistoryRepository;
            QueryRepositoryFactory.CommentHistoryRepository = CommentHistoryRepository;

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
            
            //Fixes problem where .Fetch is used in a query
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));

            base.RegisterAdditionalServices(container);
        }

        public HistoryControllerTests()
        {
            OrderHistoryRepository = MockRepository.GenerateStub<IRepository<OrderHistory>>();
            SetupData1();

            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            //Controller.Repository.Expect(a => a.OfType<History>()).Return(HistoryRepository).Repeat.Any();	
        }

        private void SetupData1()
        {
            var orderStatusCodes = new List<OrderStatusCode>();
            var orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Account Manager";
            orderStatusCode.Level = 3;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.SetIdTo("AM");
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Approver";
            orderStatusCode.Level = 2;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.SetIdTo("AP");
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Conditional Approval";
            orderStatusCode.Level = 2;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = false;
            orderStatusCode.SetIdTo("CA");
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Complete-Not Uploaded KFS";
            orderStatusCode.Level = 5;
            orderStatusCode.IsComplete = true;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = false;
            orderStatusCode.SetIdTo("CN");
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Complete";
            orderStatusCode.Level = 5;
            orderStatusCode.IsComplete = true;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.SetIdTo("CP");
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Purchaser";
            orderStatusCode.Level = 4;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.SetIdTo("PR");
            orderStatusCodes.Add(orderStatusCode);


            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Requester";
            orderStatusCode.Level = 1;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = false;
            orderStatusCode.SetIdTo("RQ");
            orderStatusCodes.Add(orderStatusCode);

            new FakeOrderStatusCodes(0, OrderStatusCodesRepository, orderStatusCodes, true);

        }
        #endregion Init





    }
}
