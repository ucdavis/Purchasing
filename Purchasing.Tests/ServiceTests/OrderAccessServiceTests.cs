using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class OrderAccessServiceTests
    {
        #region Init
        public IOrderAccessService OrderAccessService;
        public IUserIdentity UserIdentity;
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IRepository<Order> OrderRepository;
        public IRepository<WorkgroupPermission> WorkgroupPermissionRepository;
        public IRepository<Approval> ApprovalRepository;
        public IRepository<OrderTracking> OrderTrackingRepository;

        public OrderAccessServiceTests()
        {
            UserIdentity = MockRepository.GenerateStub<IUserIdentity>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            OrderRepository = MockRepository.GenerateStub<IRepository<Order>>();
            WorkgroupPermissionRepository = MockRepository.GenerateStub<IRepository<WorkgroupPermission>>();
            ApprovalRepository = MockRepository.GenerateStub<IRepository<Approval>>();
            OrderTrackingRepository = MockRepository.GenerateStub<IRepository<OrderTracking>>();

            OrderAccessService = new OrderAccessService(UserIdentity, UserRepository, OrderRepository,WorkgroupPermissionRepository, ApprovalRepository, OrderTrackingRepository );
        }
        #endregion Init

        #region Setup Data

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
            user.FirstName = "John";
            user.LastName = "Zoidberg";
            user.SetIdTo("zoidberg");
            users.Add(user);

            user = CreateValidEntities.User(7);
            user.FirstName = "Moe";
            user.LastName = "Szyslak";
            user.SetIdTo("moe");
            users.Add(user);

            user = CreateValidEntities.User(8);
            user.FirstName = "Monty";
            user.LastName = "Burns";
            user.SetIdTo("burns");
            users.Add(user);

            user = CreateValidEntities.User(9);
            user.FirstName = "Ned";
            user.LastName = "Flanders";
            user.SetIdTo("flanders");
            users.Add(user);

            user = CreateValidEntities.User(10);
            user.FirstName = "Frank";
            user.LastName = "Grimes";
            user.SetIdTo("grimes");
            users.Add(user);

            user = CreateValidEntities.User(11);
            user.FirstName = "Bender";
            user.LastName = "Rodriguez";
            user.SetIdTo("bender");
            users.Add(user);

            new FakeUsers(0, UserRepository, users, true);
        }


        #endregion Setup Data

        #region Temp Tests

        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            SetupUsers1();
            UserIdentity.Expect(a => a.Current).Return("bender").Repeat.Any();

            var workgroupPermissions = new List<WorkgroupPermission>();
            for (int i = 0; i < 6; i++)
            {
                workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i+1));
                workgroupPermissions[i].User = UserRepository.GetNullableById(((i)%3 + 1).ToString());              
            }

            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);
            #endregion Arrange

            #region Act
            OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            #endregion Assert		
        } 
        #endregion Temp Tests
    }
}
