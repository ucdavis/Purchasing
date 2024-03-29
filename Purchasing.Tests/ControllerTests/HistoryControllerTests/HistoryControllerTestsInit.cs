﻿using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Moq;



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
                Mock.Of<IRepositoryWithTypedId<ColumnPreferences, string>>();
            OrderStatusCodesRepository = Mock.Of<IRepositoryWithTypedId<OrderStatusCode, string>>();
            ApprovalRepository = Mock.Of<IRepository<Approval>>();
            OrderTrackingRepository = Mock.Of<IRepository<OrderTracking>>();

            RepositoryFactory = Mock.Of<IRepositoryFactory>();
            Mock.Get(RepositoryFactory).SetupGet(r => r.ColumnPreferencesRepository).Returns(ColumnPreferencesRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.OrderStatusCodeRepository).Returns(OrderStatusCodesRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.ApprovalRepository).Returns(ApprovalRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.OrderTrackingRepository).Returns(OrderTrackingRepository);

            OrderTrackingHistoryRepository = Mock.Of<IRepository<OrderTrackingHistory>>();
            CommentHistoryRepository = Mock.Of<IRepositoryWithTypedId<CommentHistory, Guid>>();

            QueryRepositoryFactory = Mock.Of<IQueryRepositoryFactory>();
            Mock.Get(QueryRepositoryFactory).SetupGet(r => r.OrderTrackingHistoryRepository).Returns(OrderTrackingHistoryRepository);
            Mock.Get(QueryRepositoryFactory).SetupGet(r => r.CommentHistoryRepository).Returns(CommentHistoryRepository);

            OrderService = Mock.Of<IOrderService>();
            
            Controller = new HistoryController(RepositoryFactory, OrderService);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());
            
            //Fixes problem where .Fetch is used in a query
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));

            base.RegisterAdditionalServices(container);
        }

        public HistoryControllerTests()
        {
            OrderHistoryRepository = Mock.Of<IRepository<OrderHistory>>();
            SetupData1();

            //    ExampleRepository = FakeRepository<Example>();
            //    Mock.Get(Controller.Repository).Setup(a => a.OfType<Example>()).Returns(ExampleRepository);

            //Mock.Get(Controller.Repository).Setup(a => a.OfType<History>()).Returns(HistoryRepository);	
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
            orderStatusCode.Id = "AM";
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Approver";
            orderStatusCode.Level = 2;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.Id = "AP";
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Conditional Approval";
            orderStatusCode.Level = 2;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = false;
            orderStatusCode.Id = "CA";
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Complete-Not Uploaded KFS";
            orderStatusCode.Level = 5;
            orderStatusCode.IsComplete = true;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = false;
            orderStatusCode.Id = "CN";
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Complete";
            orderStatusCode.Level = 5;
            orderStatusCode.IsComplete = true;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.Id = "CP";
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Purchaser";
            orderStatusCode.Level = 4;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = true;
            orderStatusCode.Id = "PR";
            orderStatusCodes.Add(orderStatusCode);


            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Requester";
            orderStatusCode.Level = 1;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.ShowInFilterList = false;
            orderStatusCode.Id = "RQ";
            orderStatusCodes.Add(orderStatusCode);

            new FakeOrderStatusCodes(0, OrderStatusCodesRepository, orderStatusCodes, true);

        }
        #endregion Init





    }
}
