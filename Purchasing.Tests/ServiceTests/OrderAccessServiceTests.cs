using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;

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
    }
}
