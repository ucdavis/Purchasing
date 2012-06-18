//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Purchasing.Core;
//using Purchasing.Core.Domain;
//using Purchasing.Tests.Core;
//using Purchasing.Web.Services;
//using Rhino.Mocks;
//using UCDArch.Core.PersistanceSupport;
//using UCDArch.Testing;

//namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
//{
//    [TestClass]
//    public partial class OrderAccessServiceTests
//    {
//        #region Init
//        public IOrderService OrderService;
//        public IUserIdentity UserIdentity;
//        public IRepositoryWithTypedId<User, string> UserRepository;
//        public IRepository<Order> OrderRepository;
//        public IRepository<WorkgroupPermission> WorkgroupPermissionRepository;
//        public IRepository<Approval> ApprovalRepository;
//        public IRepository<OrderTracking> OrderTrackingRepository;
//        public IRepositoryWithTypedId<Organization, string> OrganizationRepository; 

//        //Not in Service, just setup for tests
//        public IRepositoryWithTypedId<Role, string> RoleRepository;
//        public IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodeRepository;
//        public IRepository<Workgroup> WorkgroupRepository;

//        public IRepositoryFactory RepositoryFactory;
//        public IQueryRepositoryFactory QueryRepositoryFactory;
//        public IEventService EventService;
//        public ISecurityService SecurityService;

//        public OrderAccessServiceTests()
//        {
//            UserIdentity = MockRepository.GenerateStub<IUserIdentity>();
//            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
//            OrderRepository = MockRepository.GenerateStub<IRepository<Order>>();
//            WorkgroupPermissionRepository = MockRepository.GenerateStub<IRepository<WorkgroupPermission>>();
//            ApprovalRepository = MockRepository.GenerateStub<IRepository<Approval>>();
//            OrderTrackingRepository = MockRepository.GenerateStub<IRepository<OrderTracking>>();
//            OrganizationRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Organization, string>>();

//            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();
//            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
//            EventService = MockRepository.GenerateStub<IEventService>();
//            SecurityService = MockRepository.GenerateStub<ISecurityService>();


//            OrderService = new OrderService(
//                RepositoryFactory,
//                EventService,
//                UserIdentity, 
//                SecurityService,
//                WorkgroupPermissionRepository, 
//                ApprovalRepository,
//                OrderTrackingRepository,
//                OrganizationRepository,
//                UserRepository, 
//                OrderRepository,
//                QueryRepositoryFactory);


//            RoleRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Role, string>>();
//            OrderStatusCodeRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OrderStatusCode, string>>();
//            WorkgroupRepository = MockRepository.GenerateStub<IRepository<Workgroup>>();
//            SetupRoles(null);
//            SetupOrderStatusCodes();
//            SetupWorkgroups();
//        }
//        #endregion Init

//        #region Setup Data
//        /// <summary>
//        /// Setup All Roles
//        /// </summary>
//        /// <param name="roles"></param>
//        public void SetupRoles(List<Role> roles)
//        {
//            if(roles == null)
//            {
//                roles = new List<Role>();
//            }
//            var role = new Role(Role.Codes.Admin);
//            role.SetIdTo(Role.Codes.Admin);
//            role.Name = "Admin";
//            role.Level = 0;
//            roles.Add(role);

//            role = new Role(Role.Codes.DepartmentalAdmin);
//            role.SetIdTo(Role.Codes.DepartmentalAdmin);
//            role.Name = "Departmental Admin";
//            role.Level = 0;
//            roles.Add(role);

//            role = new Role(Role.Codes.Requester);
//            role.SetIdTo(Role.Codes.Requester);
//            role.Name = "Requester";
//            role.Level = 1;
//            roles.Add(role);

//            role = new Role(Role.Codes.Approver);
//            role.SetIdTo(Role.Codes.Approver);
//            role.Name = "Approver";
//            role.Level = 2;
//            roles.Add(role);

//            role = new Role(Role.Codes.AccountManager);
//            role.SetIdTo(Role.Codes.AccountManager);
//            role.Name = "Account Manager";
//            role.Level = 3;
//            roles.Add(role);

//            role = new Role(Role.Codes.Purchaser);
//            role.SetIdTo(Role.Codes.Purchaser);
//            role.Name = "Purchaser";
//            role.Level = 4;
//            roles.Add(role);

//            new FakeRoles(0, RoleRepository, roles, true);
//        }

//        public void SetupOrderStatusCodes()
//        {
//            var orderStatusCodes = new List<OrderStatusCode>();
//            var orderStatusCode = new OrderStatusCode();
//            orderStatusCode.Name = "Account Manager";
//            orderStatusCode.Level = 3;
//            orderStatusCode.IsComplete = false;
//            orderStatusCode.KfsStatus = false;
//            orderStatusCode.ShowInFilterList = true;
//            orderStatusCode.SetIdTo("AM");
//            orderStatusCodes.Add(orderStatusCode);
            
//            orderStatusCode = new OrderStatusCode();
//            orderStatusCode.Name = "Approver";
//            orderStatusCode.Level = 2;
//            orderStatusCode.IsComplete = false;
//            orderStatusCode.KfsStatus = false;
//            orderStatusCode.ShowInFilterList = true;
//            orderStatusCode.SetIdTo("AP");
//            orderStatusCodes.Add(orderStatusCode);
            
//            orderStatusCode = new OrderStatusCode();
//            orderStatusCode.Name = "Conditional Approval";
//            orderStatusCode.Level = 2;
//            orderStatusCode.IsComplete = false;
//            orderStatusCode.KfsStatus = false;
//            orderStatusCode.ShowInFilterList = false;
//            orderStatusCode.SetIdTo("CA");
//            orderStatusCodes.Add(orderStatusCode);
            
//            orderStatusCode = new OrderStatusCode();
//            orderStatusCode.Name = "Complete-Not Uploaded KFS";
//            orderStatusCode.Level = 5;
//            orderStatusCode.IsComplete = true;
//            orderStatusCode.KfsStatus = false;
//            orderStatusCode.ShowInFilterList = false;
//            orderStatusCode.SetIdTo("CN");
//            orderStatusCodes.Add(orderStatusCode);

//            orderStatusCode = new OrderStatusCode();
//            orderStatusCode.Name = "Complete";
//            orderStatusCode.Level = 5;
//            orderStatusCode.IsComplete = true;
//            orderStatusCode.KfsStatus = false;
//            orderStatusCode.ShowInFilterList = true;
//            orderStatusCode.SetIdTo("CP");
//            orderStatusCodes.Add(orderStatusCode);

//            orderStatusCode = new OrderStatusCode();
//            orderStatusCode.Name = "Purchaser";
//            orderStatusCode.Level = 4;
//            orderStatusCode.IsComplete = false;
//            orderStatusCode.KfsStatus = false;
//            orderStatusCode.ShowInFilterList = true;
//            orderStatusCode.SetIdTo("PR");
//            orderStatusCodes.Add(orderStatusCode);


//            orderStatusCode = new OrderStatusCode();
//            orderStatusCode.Name = "Requester";
//            orderStatusCode.Level = 1;
//            orderStatusCode.IsComplete = false;
//            orderStatusCode.KfsStatus = false;
//            orderStatusCode.ShowInFilterList = false;
//            orderStatusCode.SetIdTo("RQ");
//            orderStatusCodes.Add(orderStatusCode);

//            new FakeOrderStatusCodes(0, OrderStatusCodeRepository, orderStatusCodes, true);
//        }

//        public void SetupWorkgroups()
//        {
//            new FakeWorkgroups(3, WorkgroupRepository);
//        }
//        /// <summary>
//        /// Setup 11 users.
//        /// </summary>
//        public void SetupUsers1(User updateUser = null, bool fakem = true)
//        {
//            var users = new List<User>();
//            var user = CreateValidEntities.User(1);
//            user.FirstName = "Philip";
//            user.LastName = "Fry";
//            user.SetIdTo("pjfry");
//            users.Add(user);

//            user = CreateValidEntities.User(2);
//            user.FirstName = "Homer";
//            user.LastName = "Simpson";
//            user.SetIdTo("hsimpson");
//            users.Add(user);

//            user = CreateValidEntities.User(3);
//            user.FirstName = "Zapp";
//            user.LastName = "Brannigan";
//            user.SetIdTo("brannigan");
//            users.Add(user);


//            user = CreateValidEntities.User(4);
//            user.FirstName = "Amy";
//            user.LastName = "Wong";
//            user.SetIdTo("awong");
//            users.Add(user);

//            user = CreateValidEntities.User(5);
//            user.FirstName = "John";
//            user.LastName = "Zoidberg";
//            user.SetIdTo("zoidberg");
//            users.Add(user);

//            user = CreateValidEntities.User(6);
//            user.FirstName = "Moe";
//            user.LastName = "Szyslak";
//            user.SetIdTo("moe");
//            users.Add(user);

//            user = CreateValidEntities.User(7);
//            user.FirstName = "Monty";
//            user.LastName = "Burns";
//            user.SetIdTo("burns");
//            users.Add(user);

//            user = CreateValidEntities.User(8);
//            user.FirstName = "Ned";
//            user.LastName = "Flanders";
//            user.SetIdTo("flanders");
//            users.Add(user);

//            user = CreateValidEntities.User(9);
//            user.FirstName = "Frank";
//            user.LastName = "Grimes";
//            user.SetIdTo("grimes");
//            users.Add(user);

//            user = CreateValidEntities.User(10);
//            user.FirstName = "Bender";
//            user.LastName = "Rodriguez";
//            user.SetIdTo("bender");
//            users.Add(user);

//            user = CreateValidEntities.User(11);
//            user.FirstName = "Hermes";
//            user.LastName = "Conrad";
//            user.SetIdTo("hconrad");
//            users.Add(user);

//            if(updateUser != null)
//            {
//                if(users.Where(a => a.Id == updateUser.Id).Any())
//                {
//                    var index = users.FindIndex(a => a.Id == updateUser.Id);
//                    users[index] = updateUser;
//                }
//                else
//                {
//                    users.Add(updateUser);
//                }
//            }

//            if(fakem)
//            {
//                new FakeUsers(0, UserRepository, users, true);
//            }
//        }


//        public void SetupWorkgroupPermissions1(List<WorkgroupPermission> workgroupPermissions = null, bool fakem = true)
//        {
//            if(workgroupPermissions == null)
//            {
//                workgroupPermissions = new List<WorkgroupPermission>();
//            }
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(1);
//            workgroupPermission.User = UserRepository.GetNullableById("pjfry");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Approver);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(3);
//            workgroupPermission.User = UserRepository.GetNullableById("brannigan");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(4);
//            workgroupPermission.User = UserRepository.GetNullableById("awong");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(5);
//            workgroupPermission.User = UserRepository.GetNullableById("zoidberg");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Approver);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(6);
//            workgroupPermission.User = UserRepository.GetNullableById("moe");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(7);
//            workgroupPermission.User = UserRepository.GetNullableById("burns");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Approver);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(8);
//            workgroupPermission.User = UserRepository.GetNullableById("flanders");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(9);
//            workgroupPermission.User = UserRepository.GetNullableById("grimes");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(10);
//            workgroupPermission.User = UserRepository.GetNullableById("bender");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(11);
//            workgroupPermission.User = UserRepository.GetNullableById("bender");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(2);
//            workgroupPermissions.Add(workgroupPermission);


//            workgroupPermission = CreateValidEntities.WorkgroupPermission(12);
//            workgroupPermission.User = UserRepository.GetNullableById("hconrad");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Approver);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            if(fakem)
//            {
//                new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);
//            }

//        }


//        public void SetupOrders(List<Order> orders, List<Approval> approvals, int numberToCreate, string userId, OrderStatusCode currentLevel, int workgroupId, bool fakem = false)
//        {
//            if(orders == null)
//            {
//                orders = new List<Order>();
//            }
//            if(approvals == null)
//            {
//                approvals = new List<Approval>();
//            }

//            var currentOffSet = orders.Count();

//            for (int i = 0; i < numberToCreate; i++)
//            {
//                var order = CreateValidEntities.Order(i + 1 + currentOffSet);
//                order.CreatedBy = UserRepository.GetNullableById(userId);
//                Assert.IsNotNull(order.CreatedBy);
//                order.StatusCode = currentLevel;
//                order.Workgroup = WorkgroupRepository.GetNullableById(workgroupId);
//                Assert.IsNotNull(order.Workgroup);
//                orders.Add(order);

//                CreateApprovals(approvals, currentLevel, currentOffSet, i, order.CreatedBy);
//            }
//            if(fakem)
//            {
//                new FakeOrders(0, OrderRepository, orders);

//                foreach (var approval in approvals)
//                {
//                    approval.Order = OrderRepository.GetNullableById(approval.Order.Id);
//                }
//                new FakeApprovals(0, ApprovalRepository, approvals);
//            }
//        }

//        private void CreateApprovals(List<Approval> approvals, OrderStatusCode currentLevel, int currentOffSet, int i, User createdBy)
//        {
//            //var approval = new Approval();
//            //approval.Order = new Order();
//            //approval.Order.SetIdTo(i + 1 + currentOffSet);
//            //approval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Requester);
//            //approval.User = createdBy;
//            //approval.Approved = true;
//            //approvals.Add(approval);

//            var approval = new Approval();
//            approval.Order = new Order();
//            approval.Order.SetIdTo(i + 1 + currentOffSet);
//            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
//            approval.User = UserRepository.GetNullableById("hsimpson");
//            if (approval.StatusCode.Level < currentLevel.Level || currentLevel.Id == OrderStatusCode.Codes.ConditionalApprover)
//            {
//                approval.Completed = true;
//            }
//            approvals.Add(approval);

//            approval = new Approval();
//            approval.Order = new Order();
//            approval.Order.SetIdTo(i + 1 + currentOffSet);
//            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.AccountManager);
//            approval.User = UserRepository.GetNullableById("flanders");
//            if(approval.StatusCode.Level < currentLevel.Level)
//            {
//                approval.Completed = true;
//            }
//            approvals.Add(approval);

//            approval = new Approval();
//            approval.Order = new Order();
//            approval.Order.SetIdTo(i + 1 + currentOffSet);
//            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Purchaser);
//            approval.User = UserRepository.GetNullableById("awong");
//            if(approval.StatusCode.Level < currentLevel.Level)
//            {
//                approval.Completed = true;
//            }
//            approvals.Add(approval);

//            approval = new Approval();
//            approval.Order = new Order();
//            approval.Order.SetIdTo(i + 1 + currentOffSet);
//            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs);
//            approval.User = UserRepository.GetNullableById("zoidberg");
//            if(approval.StatusCode.Level < currentLevel.Level)
//            {
//                approval.Completed = true;
//            }
//            approvals.Add(approval);
//        }

//        public void SetupOrderTracking()
//        {
//            var whenApproved = DateTime.Now.AddDays(-10);
//            var orderTracking = new List<OrderTracking>();
//            var allOrders = OrderRepository.Queryable.ToList();
//            foreach (var order in allOrders)
//            {
//                var tracking = new OrderTracking();
//                tracking.DateCreated = whenApproved;
//                tracking.Description = "Description1";
//                tracking.Order = order;
//                tracking.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Requester);
//                tracking.User = order.CreatedBy;
//                orderTracking.Add(tracking);

//                Order order1 = order;
                
//                var count = 1;
//                var approvals = ApprovalRepository.Queryable.Where(a => a.Order.Id == order1.Id && a.Completed).OrderBy(b => b.StatusCode.Level);
//                foreach (var approval in approvals)
//                {
//                    tracking = new OrderTracking();
//                    tracking.DateCreated = whenApproved.AddDays(count);
//                    tracking.Description = "Description" + count;
//                    tracking.Order = order;
//                    tracking.StatusCode = approval.StatusCode;
//                    tracking.User = approval.User;

//                    orderTracking.Add(tracking);
//                    count++;
//                }
//            }

//            new FakeOrderTracking(0, OrderTrackingRepository, orderTracking);
//        }

//        #endregion Setup Data

//    }
//}
