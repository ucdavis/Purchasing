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
            RepositoryFactory = new Moq.Mock<IRepositoryFactory>().Object;
            OrderService = new Moq.Mock<IOrderService>().Object;
            SecurityService = new Moq.Mock<ISecurityService>().Object;
            DirectorySearchService = new Moq.Mock<IDirectorySearchService>().Object;
            FinancialSystemService = new Moq.Mock<IFinancialSystemService>().Object;
            ColumnPreferencesRepository = new Moq.Mock<IRepositoryWithTypedId<ColumnPreferences, string>>().Object;
            OrderStatusCodeRepository = new Moq.Mock<IRepositoryWithTypedId<OrderStatusCode, string>>().Object;
            QueryRepositoryFactory = new Moq.Mock<IQueryRepositoryFactory>().Object;
            EventService = new Moq.Mock<IEventService>().Object;
            BugTrackingService = new Moq.Mock<IBugTrackingService>().Object;
            UserRepository = new Moq.Mock<IRepositoryWithTypedId<User, string>>().Object;
            RoleRepository = new Moq.Mock<IRepositoryWithTypedId<Role, string>>().Object;
            //OrderPeepRepository = new Moq.Mock<IRepository<OrderPeep>>().Object;
            ApprovalRepository = new Moq.Mock<IRepository<Approval>>().Object;
            WorkgroupRepository = new Moq.Mock<IRepository<Workgroup>>().Object;
            WorkgroupPermissionRepository = new Moq.Mock<IRepository<WorkgroupPermission>>().Object;

            RepositoryFactory.ColumnPreferencesRepository = ColumnPreferencesRepository;
            RepositoryFactory.OrderRepository = OrderRepository;
            RepositoryFactory.OrderStatusCodeRepository = OrderStatusCodeRepository;
            RepositoryFactory.RoleRepository = RoleRepository;
            RepositoryFactory.UserRepository = UserRepository;
            RepositoryFactory.ApprovalRepository = ApprovalRepository;
            RepositoryFactory.WorkgroupRepository = WorkgroupRepository;
            RepositoryFactory.WorkgroupPermissionRepository = WorkgroupPermissionRepository;

            RepositoryFactory.UnitOfMeasureRepository = new Moq.Mock<IRepositoryWithTypedId<UnitOfMeasure, string>>().Object;
            RepositoryFactory.WorkgroupAccountRepository = new Moq.Mock<IRepository<WorkgroupAccount>>().Object;
            RepositoryFactory.WorkgroupVendorRepository = new Moq.Mock<IRepository<WorkgroupVendor>>().Object;
            RepositoryFactory.WorkgroupAddressRepository = new Moq.Mock<IRepository<WorkgroupAddress>>().Object;
            RepositoryFactory.ShippingTypeRepository = new Moq.Mock<IRepositoryWithTypedId<ShippingType, string>>().Object;
            RepositoryFactory.CustomFieldRepository = new Moq.Mock<IRepository<CustomField>>().Object;
            RepositoryFactory.OrganizationRepository = new Moq.Mock<IRepositoryWithTypedId<Organization, string>>().Object;
            RepositoryFactory.OrderTypeRepository = new Moq.Mock<IRepositoryWithTypedId<OrderType, string>>().Object;
            RepositoryFactory.AttachmentRepository = new Moq.Mock<IRepositoryWithTypedId<Attachment, Guid>>().Object;
            RepositoryFactory.CommodityRepository = new Moq.Mock<IRepositoryWithTypedId<Commodity, string>>().Object;
            RepositoryFactory.SplitRepository = new Moq.Mock<IRepository<Split>>().Object;
            RepositoryFactory.AccountRepository = new Moq.Mock<IRepositoryWithTypedId<Account, string>>().Object;

            FileService = new Moq.Mock<IFileService>().Object;
            MemoryCache = new Moq.Mock<IMemoryCache>().Object;

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
            //    Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<Example>()).Returns(ExampleRepository);

            UserRepository2 = new Moq.Mock<IRepository<User>>().Object;
            Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<User>()).Returns(UserRepository2);

            Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<Order>()).Returns(OrderRepository);
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
            
            //Moq.Mock.Get(OrderService).Setup(a => a.GetListofOrders(Moq.It.IsAny<bool>(), Moq.It.IsAny<bool>(), Moq.It.IsAny<string>(), Moq.It.IsAny<DateTime?>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<bool>())).Returns(OrderRepository.Queryable);
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
