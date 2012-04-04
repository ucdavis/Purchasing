using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests.NotificationServiceTests
{
    [TestClass]
    public partial class NotificationServiceTests
    {
        #region Init
        public INotificationService NotificationService;

        public IRepositoryWithTypedId<EmailQueue, Guid> EmailRepository;
        public IRepositoryWithTypedId<EmailPreferences, string> EmailPreferenceRepository;
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodeRepository; 
        public IUserIdentity UserIdentity;
        public IServerLink ServerLink;
        public IRepository<Approval> ApprovalRepository;
        public IQueryRepositoryFactory QueryRepositoryFactory;
        public IRepositoryFactory RepositoryFactory;

        public NotificationServiceTests()
        {
            EmailRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<EmailQueue, Guid>>();
            EmailPreferenceRepository = MockRepository.GenerateStub < IRepositoryWithTypedId<EmailPreferences, string>>();
            UserIdentity = MockRepository.GenerateStub<IUserIdentity>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            OrderStatusCodeRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OrderStatusCode, string>>();
            ServerLink = MockRepository.GenerateStub<IServerLink>();
            QueryRepositoryFactory = MockRepository.GenerateStub<IQueryRepositoryFactory>();
            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();

            NotificationService = new NotificationService(EmailRepository, EmailPreferenceRepository, UserRepository, OrderStatusCodeRepository, UserIdentity, ServerLink, QueryRepositoryFactory, RepositoryFactory);

            ServerLink.Expect(a => a.Address).Return("FakeHost").Repeat.Any();
            ApprovalRepository = MockRepository.GenerateStub<IRepository<Approval>>();

            SetupOrderStatusCodes();
        }
 
        #endregion Init

        #region Setup Data
        public void SetupOrderStatusCodes()
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

            new FakeOrderStatusCodes(0, OrderStatusCodeRepository, orderStatusCodes, true);
        }

        public void SetupUsers(User updateUser = null, bool fakem = true)
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

            user = CreateValidEntities.User(11);
            user.FirstName = "Hermes";
            user.LastName = "Conrad";
            user.SetIdTo("hconrad");
            users.Add(user);

            if(updateUser != null)
            {
                if(users.Where(a => a.Id == updateUser.Id).Any())
                {
                    var index = users.FindIndex(a => a.Id == updateUser.Id);
                    users[index] = updateUser;
                }
                else
                {
                    users.Add(updateUser);
                }
            }

            if(fakem)
            {
                new FakeUsers(0, UserRepository, users, true);
            }
        }

        public Order SetupData1(string userId, OrderStatusCode currentLevel)
        {
            var approvals = new List<Approval>();

            var order = CreateValidEntities.Order(1);
            order.SetIdTo(1);
            order.CreatedBy = UserRepository.GetNullableById(userId);
            Assert.IsNotNull(order.CreatedBy);
            order.StatusCode = currentLevel;

            CreateApprovals(approvals, currentLevel, order);
            new FakeApprovals(0, ApprovalRepository, approvals);

            order.OrderTrackings = SetupOrderTracking(order);

            order.Organization = CreateValidEntities.Organization(9);
            order.Organization.SetIdTo("testOrg");

            return order;
        }

        public List<OrderTracking> SetupOrderTracking(Order order)
        {
            var whenApproved = DateTime.Now.AddDays(-10);
            var orderTracking = new List<OrderTracking>();

            var tracking = new OrderTracking();
            tracking.DateCreated = whenApproved;
            tracking.Description = "Description1";
            tracking.Order = order;
            tracking.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Requester);
            tracking.User = order.CreatedBy;
            orderTracking.Add(tracking);

            Order order1 = order;

            var count = 1;
            var approvals = ApprovalRepository.Queryable.Where(a => a.Order.Id == order1.Id && a.Completed).OrderBy(b => b.StatusCode.Level);
            foreach(var approval in approvals)
            {
                tracking = new OrderTracking();
                tracking.DateCreated = whenApproved.AddDays(count);
                tracking.Description = "Description" + count;
                tracking.Order = order;
                tracking.StatusCode = approval.StatusCode;
                tracking.User = approval.User;

                orderTracking.Add(tracking);
                count++;
            }


            return orderTracking;
        }

        private void CreateApprovals(List<Approval> approvals, OrderStatusCode currentLevel, Order order)
        {
            //var approval = new Approval();
            //approval.Order = new Order();
            //approval.Order.SetIdTo(i + 1 + currentOffSet);
            //approval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Requester);
            //approval.User = createdBy;
            //approval.Approved = true;
            //approvals.Add(approval);

            var approval = new Approval();
            approval.Order = new Order();
            approval.Order.SetIdTo(order.Id);
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Approver);
            approval.User = UserRepository.GetNullableById("hsimpson");
            if(approval.StatusCode.Level < currentLevel.Level)
            {
                approval.Completed = true;
            }
            approvals.Add(approval);

            approval = new Approval();
            approval.Order = new Order();
            approval.Order.SetIdTo(order.Id);
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.AccountManager);
            approval.User = UserRepository.GetNullableById("flanders");
            if(approval.StatusCode.Level < currentLevel.Level)
            {
                approval.Completed = true;
            }
            approvals.Add(approval);

            approval = new Approval();
            approval.Order = new Order();
            approval.Order.SetIdTo(order.Id);
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(Role.Codes.Purchaser);
            approval.User = UserRepository.GetNullableById("awong");
            if(approval.StatusCode.Level < currentLevel.Level)
            {
                approval.Completed = true;
            }
            approvals.Add(approval);

            approval = new Approval();
            approval.Order = new Order();
            approval.Order.SetIdTo(order.Id);
            approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.CompleteNotUploadedKfs);
            approval.User = UserRepository.GetNullableById("zoidberg");
            if(approval.StatusCode.Level < currentLevel.Level)
            {
                approval.Completed = true;
            }
            approvals.Add(approval);
        }

        #endregion Setup Data
    }
}
