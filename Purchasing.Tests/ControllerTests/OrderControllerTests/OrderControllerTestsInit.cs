using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.Attributes;
using Purchasing.WS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;

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
        //public IRepository<OrderPeep> OrderPeepRepository;
        public IRepository<Approval> ApprovalRepository;
        public IRepository<Workgroup> WorkgroupRepository;
        public IRepository<WorkgroupPermission> WorkgroupPermissionRepository; 

        public IRepositoryWithTypedId<ColumnPreferences, string> ColumnPreferencesRepository;
        public IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodeRepository;

        public IFileService FileService;
        public IMemoryCache MemoryCache;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            OrderRepository = FakeRepository<Order>();
            RepositoryFactory = new Mock<IRepositoryFactory>().Object;
            OrderService = new Mock<IOrderService>().Object;
            SecurityService = new Mock<ISecurityService>().Object;
            DirectorySearchService = new Mock<IDirectorySearchService>().Object;
            FinancialSystemService = new Mock<IFinancialSystemService>().Object;
            ColumnPreferencesRepository = new Mock<IRepositoryWithTypedId<ColumnPreferences, string>>().Object;
            OrderStatusCodeRepository = new Mock<IRepositoryWithTypedId<OrderStatusCode, string>>().Object;
            QueryRepositoryFactory = new Mock<IQueryRepositoryFactory>().Object;
            EventService = new Mock<IEventService>().Object;
            BugTrackingService = new Mock<IBugTrackingService>().Object;
            UserRepository = new Mock<IRepositoryWithTypedId<User, string>>().Object;
            RoleRepository = new Mock<IRepositoryWithTypedId<Role, string>>().Object;
            //OrderPeepRepository = new Mock<IRepository<OrderPeep>>().Object;
            ApprovalRepository = new Mock<IRepository<Approval>>().Object;
            WorkgroupRepository = new Mock<IRepository<Workgroup>>().Object;
            WorkgroupPermissionRepository = new Mock<IRepository<WorkgroupPermission>>().Object;

            Mock.Get(RepositoryFactory).SetupGet(r => r.ColumnPreferencesRepository).Returns(ColumnPreferencesRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.OrderRepository).Returns(OrderRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.OrderStatusCodeRepository).Returns(OrderStatusCodeRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.RoleRepository).Returns(RoleRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.UserRepository).Returns(UserRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.ApprovalRepository).Returns(ApprovalRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.WorkgroupRepository).Returns(WorkgroupRepository);
            Mock.Get(RepositoryFactory).SetupGet(r => r.WorkgroupPermissionRepository).Returns(WorkgroupPermissionRepository);

            Mock.Get(RepositoryFactory).SetupGet(r => r.UnitOfMeasureRepository).Returns(new Mock<IRepositoryWithTypedId<UnitOfMeasure, string>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.WorkgroupAccountRepository).Returns(new Mock<IRepository<WorkgroupAccount>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.WorkgroupVendorRepository).Returns(new Mock<IRepository<WorkgroupVendor>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.WorkgroupAddressRepository).Returns(new Mock<IRepository<WorkgroupAddress>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.ShippingTypeRepository).Returns(new Mock<IRepositoryWithTypedId<ShippingType, string>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.CustomFieldRepository).Returns(new Mock<IRepository<CustomField>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.OrganizationRepository).Returns(new Mock<IRepositoryWithTypedId<Organization, string>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.OrderTypeRepository).Returns(new Mock<IRepositoryWithTypedId<OrderType, string>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.AttachmentRepository).Returns(new Mock<IRepositoryWithTypedId<Attachment, Guid>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.CommodityRepository).Returns(new Mock<IRepositoryWithTypedId<Commodity, string>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.SplitRepository).Returns(new Mock<IRepository<Split>>().Object);
            Mock.Get(RepositoryFactory).SetupGet(r => r.AccountRepository).Returns(new Mock<IRepositoryWithTypedId<Account, string>>().Object);

            FileService = new Mock<IFileService>().Object;
            MemoryCache = new Mock<IMemoryCache>().Object;

            //QueryRepositoryFactory.OrderPeepRepository = OrderPeepRepository;

            Controller = new OrderController(
                RepositoryFactory,
                OrderService,
                SecurityService,
                DirectorySearchService,
                FinancialSystemService,
                QueryRepositoryFactory,
                EventService,
                BugTrackingService,
                FileService,
                MemoryCache);
        }
        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<QueryExtensionFakes>().Named("queryExtensionProvider"));
            base.RegisterAdditionalServices(container);
        }

        public OrderControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Mock.Get(Controller.Repository).Setup(a => a.OfType<Example>()).Returns(ExampleRepository);

            UserRepository2 = new Mock<IRepository<User>>().Object;
            Mock.Get(Controller.Repository).Setup(a => a.OfType<User>()).Returns(UserRepository2);

            Mock.Get(Controller.Repository).Setup(a => a.OfType<Order>()).Returns(OrderRepository);
        }

        protected override void InitServiceLocator()
        {
            var container = ServiceLocatorInitializer.Init();
            container.Register(Component.For<ISecurityService>().ImplementedBy<FakeSecurityService>().Named("securityService"));
            RegisterAdditionalServices(container);
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
            
            //Mock.Get(OrderService).Setup(a => a.GetListofOrders(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime>(), It.IsAny<bool>())).Returns(OrderRepository.Queryable);
        }

        protected void SetupRoles()
        {
            var roles = new List<Role>();

            var role = new Role(Role.Codes.Admin);
            role.Id = Role.Codes.Admin;
            role.Name = "Admin";
            role.Level = 0;
            role.IsAdmin = true;
            roles.Add(role);

            role = new Role(Role.Codes.DepartmentalAdmin);
            role.Id = Role.Codes.DepartmentalAdmin;
            role.Name = "Departmental Admin";
            role.Level = 0;
            role.IsAdmin = true;
            roles.Add(role);

            role = new Role(Role.Codes.Requester);
            role.Id = Role.Codes.Requester;
            role.Name = "Requester";
            role.Level = 1;
            roles.Add(role);

            role = new Role(Role.Codes.Approver);
            role.Id = Role.Codes.Approver;
            role.Name = "Approver";
            role.Level = 2;
            roles.Add(role);

            role = new Role(Role.Codes.AccountManager);
            role.Id = Role.Codes.AccountManager;
            role.Name = "Account Manager";
            role.Level = 3;
            roles.Add(role);

            role = new Role(Role.Codes.Purchaser);
            role.Id = Role.Codes.Purchaser;
            role.Name = "Purchaser";
            role.Level = 4;
            roles.Add(role);

            new FakeRoles(0, RoleRepository, roles, true);
        }

        public delegate void SetOrderDelegate(Order order);
        public void SetOrderInstance(Order order)
        {
            order.Id = SetMyId.Id;
        }

        public static class SetMyId
        {
            public static int? SetId { get; set; }
            public static int Id
            {
                get { return SetId.HasValue ? SetId.Value : 99; }
            }
        }

    }

    public class FakeSecurityService : ISecurityService
    {
        public bool HasWorkgroupOrOrganizationAccess(Workgroup workgroup, Organization organization, out string message)
        {
            throw new NotImplementedException();
        }

        public bool HasWorkgroupAccess(Workgroup workgroup)
        {
            throw new NotImplementedException();
        }

        public bool HasWorkgroupEditAccess(int id, out string message)
        {
            throw new NotImplementedException();
        }

        public bool IsInRole(string roleCode, int workgroupId)
        {
            throw new NotImplementedException();
        }

        public bool IsInRole(Role role, Workgroup workgroup)
        {
            throw new NotImplementedException();
        }

        public bool hasWorkgroupRole(string roleCode, int workgroupId)
        {
            throw new NotImplementedException();
        }

        public bool hasAdminWorkgroupRole(string roleCode, int workgroupId)
        {
            throw new NotImplementedException();
        }

        public OrderAccessLevel GetAccessLevel(Order order, bool? closed = null)
        {
            throw new NotImplementedException();
        }

        public OrderAccessLevel GetAccessLevel(int orderId, bool? closed = null)
        {
            throw new NotImplementedException();
        }

        public OrderAccessLevel GetAccessLevel(Order order)
        {
            throw new NotImplementedException();
        }

        public OrderAccessLevel GetAccessLevel(int orderId)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string kerb)
        {
            throw new NotImplementedException();
        }

        public RolesAndAccessLevel GetAccessRoleAndLevel(Order order)
        {
            throw new NotImplementedException();
        }

        public bool HasReadAccess(int orderId)
        {
            throw new NotImplementedException();
        }

        public bool HasEditAccess(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
