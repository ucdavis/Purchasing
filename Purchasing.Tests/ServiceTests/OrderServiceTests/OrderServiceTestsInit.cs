using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.WS;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests.OrderServiceTests
{
    [TestClass]
    public partial class OrderServiceTests
    {
        public IRepositoryFactory RepositoryFactory;
        public IQueryRepositoryFactory QueryRepositoryFactory;
        public IFinancialSystemService FinancialSystemService;
        public IEventService EventService;
        public IUserIdentity UserIdentity;
        public ISecurityService SecurityService;
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



        public OrderServiceTests()
        {
            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();
            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
            FinancialSystemService = MockRepository.GenerateStub<IFinancialSystemService>();
            EventService = MockRepository.GenerateStub<IEventService>();
            UserIdentity = MockRepository.GenerateStub<IUserIdentity>();
            SecurityService = MockRepository.GenerateStub<ISecurityService>();
            WorkgroupPermissionRepository = MockRepository.GenerateStub<IRepository<WorkgroupPermission>>();
            ApprovalRepository = MockRepository.GenerateStub<IRepository<Approval>>();
            OrderTrackingRepository = MockRepository.GenerateStub<IRepository<OrderTracking>>();
            OrganizationRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Organization, string>>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            OrderRepository = MockRepository.GenerateStub<IRepository<Order>>();

            WorkgroupAccountRepository = MockRepository.GenerateStub<IRepository<WorkgroupAccount>>();
            AccountRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Account, string>>();
            OrderStatusCodeRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OrderStatusCode, string>>();
            AutoAprovalRepository = MockRepository.GenerateStub<IRepository<AutoApproval>>();


            RepositoryFactory.WorkgroupAccountRepository = WorkgroupAccountRepository;
            RepositoryFactory.AccountRepository = AccountRepository;
            RepositoryFactory.OrderStatusCodeRepository = OrderStatusCodeRepository;
            RepositoryFactory.AutoApprovalRepository = AutoAprovalRepository;
            RepositoryFactory.UserRepository = UserRepository;

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
                FinancialSystemService);


            
        }

        public void SetupValidOrderStatusCodes()
        {
            var statusCodes = new List<OrderStatusCode>();
            var statusCode = new OrderStatusCode();
            statusCode.SetIdTo(OrderStatusCode.Codes.AccountManager);
            statusCode.Name = "Account Manager";
            statusCode.Level = 3;
            statusCode.IsComplete = false;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.SetIdTo(OrderStatusCode.Codes.Approver);
            statusCode.Name = "Approver";
            statusCode.Level = 2;
            statusCode.IsComplete = false;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.SetIdTo(OrderStatusCode.Codes.ConditionalApprover);
            statusCode.Name = "Conditional Approval";
            statusCode.Level = 2;
            statusCode.IsComplete = false;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = false;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.SetIdTo(OrderStatusCode.Codes.CompleteNotUploadedKfs);
            statusCode.Name = "Complete-Not Uploaded KFS";
            statusCode.Level = 5;
            statusCode.IsComplete = true;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = false;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.SetIdTo(OrderStatusCode.Codes.Complete);
            statusCode.Name = "Complete";
            statusCode.Level = 5;
            statusCode.IsComplete = true;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.SetIdTo(OrderStatusCode.Codes.Cancelled);
            statusCode.Name = "Cancelled";
            statusCode.Level = 5;
            statusCode.IsComplete = true;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.SetIdTo(OrderStatusCode.Codes.Denied);
            statusCode.Name = "Denied";
            statusCode.Level = 5;
            statusCode.IsComplete = true;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);

            statusCode = new OrderStatusCode();
            statusCode.SetIdTo(OrderStatusCode.Codes.Purchaser);
            statusCode.Name = "Purchaser";
            statusCode.Level = 4;
            statusCode.IsComplete = false;
            statusCode.KfsStatus = false;
            statusCode.ShowInFilterList = true;
            statusCodes.Add(statusCode);
           
            statusCode = new OrderStatusCode();
            statusCode.SetIdTo(OrderStatusCode.Codes.Requester);
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
