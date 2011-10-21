using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Services;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
{
    [TestClass]
    public partial class OrderAccessServiceTests
    {
        #region Init
        public IOrderAccessService OrderAccessService;
        public IUserIdentity UserIdentity;
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IRepository<Order> OrderRepository;
        public IRepository<WorkgroupPermission> WorkgroupPermissionRepository;
        public IRepository<Approval> ApprovalRepository;
        public IRepository<OrderTracking> OrderTrackingRepository;

        //Not in Service, just setup for tests
        public IRepositoryWithTypedId<Role, string> RoleRepository;
        public IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodeRepository;
        public IRepository<Workgroup> WorkgroupRepository; 

        public OrderAccessServiceTests()
        {
            UserIdentity = MockRepository.GenerateStub<IUserIdentity>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            OrderRepository = MockRepository.GenerateStub<IRepository<Order>>();
            WorkgroupPermissionRepository = MockRepository.GenerateStub<IRepository<WorkgroupPermission>>();
            ApprovalRepository = MockRepository.GenerateStub<IRepository<Approval>>();
            OrderTrackingRepository = MockRepository.GenerateStub<IRepository<OrderTracking>>();

            OrderAccessService = new OrderAccessService(UserIdentity, UserRepository, OrderRepository,WorkgroupPermissionRepository, ApprovalRepository, OrderTrackingRepository );


            RoleRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Role, string>>();
            OrderStatusCodeRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OrderStatusCode, string>>();
            WorkgroupRepository = MockRepository.GenerateStub<IRepository<Workgroup>>();
            SetupRoles(null);
            SetupOrderStatusCodes();
            SetupWorkgroups();
        }
        #endregion Init

        #region Setup Data
        /// <summary>
        /// Setup All Roles
        /// </summary>
        /// <param name="roles"></param>
        public void SetupRoles(List<Role> roles)
        {
            if(roles == null)
            {
                roles = new List<Role>();
            }
            var role = new Role(Role.Codes.Admin);
            role.SetIdTo(Role.Codes.Admin);
            role.Name = "Admin";
            role.Level = 0;
            roles.Add(role);

            role = new Role(Role.Codes.DepartmentalAdmin);
            role.SetIdTo(Role.Codes.DepartmentalAdmin);
            role.Name = "Departmental Admin";
            role.Level = 0;
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

        public void SetupOrderStatusCodes()
        {
            var orderStatusCodes = new List<OrderStatusCode>();
            var orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Account Manager";
            orderStatusCode.Level = 3;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.SetIdTo("AM");
            orderStatusCodes.Add(orderStatusCode);
            
            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Approver";
            orderStatusCode.Level = 2;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.SetIdTo("AP");
            orderStatusCodes.Add(orderStatusCode);
            
            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Conditional Approval";
            orderStatusCode.Level = 2;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.SetIdTo("CA");
            orderStatusCodes.Add(orderStatusCode);
            
            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Complete-Not Uploaded KFS";
            orderStatusCode.Level = 5;
            orderStatusCode.IsComplete = true;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.SetIdTo("CN");
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Complete";
            orderStatusCode.Level = 1;
            orderStatusCode.IsComplete = true;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.SetIdTo("CP");
            orderStatusCodes.Add(orderStatusCode);

            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Purchaser";
            orderStatusCode.Level = 4;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.SetIdTo("PR");
            orderStatusCodes.Add(orderStatusCode);


            orderStatusCode = new OrderStatusCode();
            orderStatusCode.Name = "Requester";
            orderStatusCode.Level = 1;
            orderStatusCode.IsComplete = false;
            orderStatusCode.KfsStatus = false;
            orderStatusCode.SetIdTo("RQ");
            orderStatusCodes.Add(orderStatusCode);

            new FakeOrderStatusCodes(0, OrderStatusCodeRepository, orderStatusCodes, true);
        }

        public void SetupWorkgroups()
        {
            new FakeWorkgroups(3, WorkgroupRepository);
        }

        /// <summary>
        /// Setup 11 users.
        /// </summary>
        public void SetupUsers1()
        {
            var users = new List<User>();
            var user = CreateValidEntities.User(1);
            user.FirstName = "Philip";
            user.LastName = "Fry";
            user.SetIdTo("pjfry");
            users.Add(user);

            user = CreateValidEntities.User(2);
            user.FirstName = "Homer";
            user.LastName = "Simpson";
            user.SetIdTo("hsimpson");
            users.Add(user);

            user = CreateValidEntities.User(3);
            user.FirstName = "Zapp";
            user.LastName = "Brannigan";
            user.SetIdTo("brannigan");
            users.Add(user);

            user = CreateValidEntities.User(4);
            user.FirstName = "Amy";
            user.LastName = "Wong";
            user.SetIdTo("awong");
            users.Add(user);

            user = CreateValidEntities.User(5);
            user.FirstName = "John";
            user.LastName = "Zoidberg";
            user.SetIdTo("zoidberg");
            users.Add(user);

            user = CreateValidEntities.User(6);
            user.FirstName = "Moe";
            user.LastName = "Szyslak";
            user.SetIdTo("moe");
            users.Add(user);

            user = CreateValidEntities.User(7);
            user.FirstName = "Monty";
            user.LastName = "Burns";
            user.SetIdTo("burns");
            users.Add(user);

            user = CreateValidEntities.User(8);
            user.FirstName = "Ned";
            user.LastName = "Flanders";
            user.SetIdTo("flanders");
            users.Add(user);

            user = CreateValidEntities.User(9);
            user.FirstName = "Frank";
            user.LastName = "Grimes";
            user.SetIdTo("grimes");
            users.Add(user);

            user = CreateValidEntities.User(10);
            user.FirstName = "Bender";
            user.LastName = "Rodriguez";
            user.SetIdTo("bender");
            users.Add(user);

            new FakeUsers(0, UserRepository, users, true);
        }

        public void SetupWorkgroupPermissions1()
        {


            var workgroupPermissions = new List<WorkgroupPermission>();
            var workgroupPermission = CreateValidEntities.WorkgroupPermission(1);
            workgroupPermission.User = UserRepository.GetNullableById("pjfry");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
            workgroupPermissions.Add(workgroupPermission);

            workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Approver);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
            workgroupPermissions.Add(workgroupPermission);

            workgroupPermission = CreateValidEntities.WorkgroupPermission(3);
            workgroupPermission.User = UserRepository.GetNullableById("brannigan");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
            workgroupPermissions.Add(workgroupPermission);

            workgroupPermission = CreateValidEntities.WorkgroupPermission(4);
            workgroupPermission.User = UserRepository.GetNullableById("awong");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
            workgroupPermissions.Add(workgroupPermission);

            workgroupPermission = CreateValidEntities.WorkgroupPermission(5);
            workgroupPermission.User = UserRepository.GetNullableById("zoidberg");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Approver);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
            workgroupPermissions.Add(workgroupPermission);

            workgroupPermission = CreateValidEntities.WorkgroupPermission(6);
            workgroupPermission.User = UserRepository.GetNullableById("moe");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
            workgroupPermissions.Add(workgroupPermission);

            workgroupPermission = CreateValidEntities.WorkgroupPermission(7);
            workgroupPermission.User = UserRepository.GetNullableById("burns");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Approver);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
            workgroupPermissions.Add(workgroupPermission);

            workgroupPermission = CreateValidEntities.WorkgroupPermission(8);
            workgroupPermission.User = UserRepository.GetNullableById("flanders");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
            workgroupPermissions.Add(workgroupPermission);

            workgroupPermission = CreateValidEntities.WorkgroupPermission(9);
            workgroupPermission.User = UserRepository.GetNullableById("grimes");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
            workgroupPermissions.Add(workgroupPermission);

            workgroupPermission = CreateValidEntities.WorkgroupPermission(10);
            workgroupPermission.User = UserRepository.GetNullableById("bender");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
            workgroupPermissions.Add(workgroupPermission);

            workgroupPermission = CreateValidEntities.WorkgroupPermission(11);
            workgroupPermission.User = UserRepository.GetNullableById("bender");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(2);
            workgroupPermissions.Add(workgroupPermission);

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

        }

        public void SetupOrders1(string userId)
        {
            var orders = new List<Order>();

            for (int i = 0; i < 5; i++)
            {
                orders.Add(CreateValidEntities.Order(i+1));
                orders[i].CreatedBy = UserRepository.GetNullableById(userId);
                orders[i].StatusCode = OrderStatusCodeRepository.GetNullableById("RQ");
                orders[i].Workgroup = WorkgroupRepository.GetNullableById(1);
            }
            orders[1].StatusCode = OrderStatusCodeRepository.GetNullableById("AP");
            orders[4].Workgroup = WorkgroupRepository.GetNullableById(2);
            new FakeOrders(0, OrderRepository, orders);
        }

        public void SetupApprovals1()
        {
            var approvals = new List<Approval>();
            for (int i = 0; i < 5; i++) //Matches number of orders
            {
                var approval = new Approval();
                approval.Order = OrderRepository.GetNullableById(i + 1);
                approval.StatusCode = OrderStatusCodeRepository.GetNullableById("RQ");
                approval.User = approval.Order.CreatedBy;
                approval.Approved = true;
                approvals.Add(approval);

                approval = new Approval();
                approval.Order = OrderRepository.GetNullableById(i + 1);
                approval.StatusCode = OrderStatusCodeRepository.GetNullableById("AP");
                approval.User = UserRepository.GetNullableById("hsimpson");
                approvals.Add(approval);
                
                approval = new Approval();
                approval.Order = OrderRepository.GetNullableById(i + 1);
                approval.StatusCode = OrderStatusCodeRepository.GetNullableById("AM");
                approval.User = UserRepository.GetNullableById("flanders");
                approvals.Add(approval);
                
                approval = new Approval();
                approval.Order = OrderRepository.GetNullableById(i + 1);
                approval.StatusCode = OrderStatusCodeRepository.GetNullableById("PR");
                approval.User = UserRepository.GetNullableById("awong");
                approvals.Add(approval);
                
                approval = new Approval();
                approval.Order = OrderRepository.GetNullableById(i + 1);
                approval.StatusCode = OrderStatusCodeRepository.GetNullableById("CN");
                approval.User = UserRepository.GetNullableById("zoidberg");
                approvals.Add(approval);
            }

            new FakeApprovals(0, ApprovalRepository, approvals);
        }
        #endregion Setup Data

    }
}
