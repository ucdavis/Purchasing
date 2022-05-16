using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Services;
using Purchasing.Tests.Core;
using Purchasing.WS;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Moq;

namespace Purchasing.Tests.ServiceTests.OrderServiceTests
{
    [TestClass]
    public partial class OrderServiceTests
    {
        public IRepositoryFactory RepositoryFactory;
        public IQueryRepositoryFactory QueryRepositoryFactory;
        public IFinancialSystemService FinancialSystemService;
        public IIndexService IndexService;
        public IEventService EventService;
        public IUserIdentity UserIdentity;
        public ISecurityService SecurityService;
        public IAccessQueryService AccessQueryService;
        public IRepository<WorkgroupPermission> WorkgroupPermissionRepository;
        public IRepository<Approval> ApprovalRepository;
        public IRepository<OrderTracking> OrderTrackingRepository;
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository;
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IRepository<Order> OrderRepository;
        
        
        public IOrderService OrderService { get; set; }

        public IRepository<WorkgroupAccount> WorkgroupAccountRepository { get; set; }
        public IRepositoryWithTypedId<Account, string> AccountRepository { get; set; }
        public IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodeRepository { get; set; }
        public IRepository<AutoApproval> AutoAprovalRepository { get; set; }
        public IRepository<ConditionalApproval> ConditionalApprovalRepository; 



        public OrderServiceTests()
        {
            RepositoryFactory = Mock.Of<IRepositoryFactory>();
            QueryRepositoryFactory = Mock.Of<IQueryRepositoryFactory>();
            FinancialSystemService = Mock.Of<IFinancialSystemService>();
            IndexService = Mock.Of<IIndexService>();
            EventService = Mock.Of<IEventService>();
            UserIdentity = Mock.Of<IUserIdentity>();
            SecurityService = Mock.Of<ISecurityService>();
            AccessQueryService = Mock.Of<IAccessQueryService>();
            WorkgroupPermissionRepository = Mock.Of<IRepository<WorkgroupPermission>>();
            ApprovalRepository = Mock.Of<IRepository<Approval>>();
            OrderTrackingRepository = Mock.Of<IRepository<OrderTracking>>();
            OrganizationRepository = Mock.Of<IRepositoryWithTypedId<Organization, string>>();
            UserRepository = Mock.Of<IRepositoryWithTypedId<User, string>>();
            OrderRepository = Mock.Of<IRepository<Order>>();

            WorkgroupAccountRepository = Mock.Of<IRepository<WorkgroupAccount>>();
            AccountRepository = Mock.Of<IRepositoryWithTypedId<Account, string>>();
            OrderStatusCodeRepository = Mock.Of<IRepositoryWithTypedId<OrderStatusCode, string>>();
            AutoAprovalRepository = Mock.Of<IRepository<AutoApproval>>();
            ConditionalApprovalRepository = Mock.Of<IRepository<ConditionalApproval>>();


            Mock.Get(RepositoryFactory).SetupGet(r => r.WorkgroupAccountRepository).Returns(WorkgroupAccountRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.AccountRepository).Returns(AccountRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.OrderStatusCodeRepository).Returns(OrderStatusCodeRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.AutoApprovalRepository).Returns(AutoAprovalRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.UserRepository).Returns(UserRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.ConditionalApprovalRepository).Returns(ConditionalApprovalRepository);

            SetupValidOrderStatusCodes();

            OrderService = new OrderService(RepositoryFactory,
                EventService,
                UserIdentity,
                SecurityService,
                WorkgroupPermissionRepository,
                ApprovalRepository,
                OrderTrackingRepository,
                OrganizationRepository,
                //UserRepository,
                OrderRepository,
                QueryRepositoryFactory,
                AccessQueryService,
                FinancialSystemService,
                IndexService);


            
        }

        public void SetupValidOrderStatusCodes()
        {
            var statusCodes = new List<OrderStatusCode>();
            var statusCode = new OrderStatusCode();
            statusCode.Id = OrderStatusCode.Codes.AccountManager;
            statusCode.Name = "Account Manager";
            statusCode.Level = 3;
            statusCode.IsComplete = false;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.Id = OrderStatusCode.Codes.Approver;
            statusCode.Name = "Approver";
            statusCode.Level = 2;
            statusCode.IsComplete = false;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.Id = OrderStatusCode.Codes.ConditionalApprover;
            statusCode.Name = "Conditional Approval";
            statusCode.Level = 2;
            statusCode.IsComplete = false;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = false;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.Id = OrderStatusCode.Codes.CompleteNotUploadedKfs;
            statusCode.Name = "Complete-Not Uploaded KFS";
            statusCode.Level = 5;
            statusCode.IsComplete = true;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = false;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.Id = OrderStatusCode.Codes.Complete;
            statusCode.Name = "Complete";
            statusCode.Level = 5;
            statusCode.IsComplete = true;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.Id = OrderStatusCode.Codes.Cancelled;
            statusCode.Name = "Cancelled";
            statusCode.Level = 5;
            statusCode.IsComplete = true;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.Id = OrderStatusCode.Codes.Denied;
            statusCode.Name = "Denied";
            statusCode.Level = 5;
            statusCode.IsComplete = true;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.Id = OrderStatusCode.Codes.Purchaser;
            statusCode.Name = "Purchaser";
            statusCode.Level = 4;
            statusCode.IsComplete = false;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);
           
            statusCode = new OrderStatusCode();
            statusCode.Id = OrderStatusCode.Codes.Requester;
            statusCode.Name = "Requester";
            statusCode.Level = 1;
            statusCode.IsComplete = false;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = false;
            statusCodes.Add(statusCode);

            new FakeOrderStatusCodes(0, OrderStatusCodeRepository, statusCodes, true);
        }

    }
}
