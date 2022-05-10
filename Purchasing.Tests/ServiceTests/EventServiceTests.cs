using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using Moq;


namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class EventServiceTests
    {
        #region Init
        public IEventService EventService;
        public IUserIdentity UserIdentity;
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IRepositoryWithTypedId<OrderStatusCode, string> OrderStatusCodeRepository;
        public INotificationService NotificationService;

        public EventServiceTests()
        {
            UserIdentity = new Mock<IUserIdentity>().Object;
            UserRepository = new Mock<IRepositoryWithTypedId<User, string>>().Object;
            OrderStatusCodeRepository = new Mock<IRepositoryWithTypedId<OrderStatusCode, string>>().Object;
            NotificationService = new Mock<INotificationService>().Object;

            EventService = new EventService(UserIdentity, UserRepository, OrderStatusCodeRepository, NotificationService);
        }

        #endregion Init

        #region OrderApprovalAdded Tests

        [TestMethod]
        public void TestOrderApprovalAddedDoesNothing()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            var approval = CreateValidEntities.Approval(1);
            var trackingCount = order.OrderTrackings.Count();
            #endregion Arrange

            #region Act
            EventService.OrderApprovalAdded(order, approval);
            #endregion Act

            #region Assert
            Mock.Get(NotificationService).Verify(a => a.OrderReRouted(It.IsAny<Order>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never());
            Assert.AreEqual(trackingCount, order.OrderTrackings.Count());
            #endregion Assert		
        }
         

        #endregion OrderApprovalAdded Tests

        #region OrderAutoApprovalAdded Tests

        [TestMethod]
        public void TestOrderAutoApprovalAddedAddsTracking()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            var approval = CreateValidEntities.Approval(1);
            approval.User = CreateValidEntities.User(3);
            approval.StatusCode = CreateValidEntities.OrderStatusCode(4);            
            #endregion Arrange

            #region Act
            EventService.OrderAutoApprovalAdded(order, approval);
            #endregion Act

            #region Assert
            Mock.Get(NotificationService).Verify(a => a.OrderApproved(It.IsAny<Order>(), It.IsAny<Approval>()), Times.Never());
            Assert.AreEqual(1, order.OrderTrackings.Count());
            Assert.AreEqual("FirstName3 LastName3", order.OrderTrackings[0].User.FullName);
            Assert.AreEqual("Name4", order.OrderTrackings[0].StatusCode.Name);
            Assert.AreEqual("automatically approved", order.OrderTrackings[0].Description);
            #endregion Assert		
        }
        #endregion OrderAutoApprovalAdded Tests

        #region OrderApproved Tests

        [TestMethod]
        public void TestOrderApproved()
        {
            #region Arrange
            var users = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i+1));
                users[i].Id = (i + 1).ToString(CultureInfo.InvariantCulture);
            }
            new FakeUsers(0, UserRepository, users, true);

            var order = CreateValidEntities.Order(1);
            var approval = CreateValidEntities.Approval(1);
            approval.User = UserRepository.Queryable.Single(a => a.Id == "3");
            approval.StatusCode = CreateValidEntities.OrderStatusCode(4);
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("2");
            #endregion Arrange

            #region Act
            EventService.OrderApproved(order, approval);
            #endregion Act

            #region Assert
            Mock.Get(NotificationService).Verify(a => a.OrderApproved(order, approval));
            Assert.AreEqual(1, order.OrderTrackings.Count());
            Assert.AreEqual("FirstName2 LastName2", order.OrderTrackings[0].User.FullName);
            Assert.AreEqual("Name4", order.OrderTrackings[0].StatusCode.Name);
            Assert.AreEqual("approved", order.OrderTrackings[0].Description);
            #endregion Assert
        }
        #endregion OrderApproved Tests

        #region OrderDenied Tests

        [TestMethod]
        public void TestOrderDenied()
        {
            #region Arrange
            var users = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Id = (i + 1).ToString(CultureInfo.InvariantCulture);
            }
            new FakeUsers(0, UserRepository, users, true);

            var order = CreateValidEntities.Order(1);
            order.StatusCode = CreateValidEntities.OrderStatusCode(4);
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("2");
            #endregion Arrange

            #region Act
            EventService.OrderDenied(order, "Some Comment", order.StatusCode);
            #endregion Act

            #region Assert
            Mock.Get(NotificationService).Verify(a => a.OrderDenied(order, UserRepository.Queryable.Single(b => b.Id == "2"), "Some Comment", order.StatusCode));
            Assert.AreEqual(1, order.OrderTrackings.Count());
            Assert.AreEqual("FirstName2 LastName2", order.OrderTrackings[0].User.FullName);
            Assert.AreEqual("Name4", order.OrderTrackings[0].StatusCode.Name);
            Assert.AreEqual("denied", order.OrderTrackings[0].Description);
            #endregion Assert
        }
        #endregion OrderDenied Tests

        #region OrderCancelled Tests

        [TestMethod]
        public void TestOrderCancelled()
        {
            #region Arrange
            var users = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Id = (i + 1).ToString(CultureInfo.InvariantCulture);
            }
            new FakeUsers(0, UserRepository, users, true);

            var order = CreateValidEntities.Order(1);
            order.StatusCode = CreateValidEntities.OrderStatusCode(4);
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("2");
            #endregion Arrange

            #region Act
            EventService.OrderCancelled(order, "Some Comment", order.StatusCode);
            #endregion Act

            #region Assert
            Mock.Get(NotificationService).Verify(a => a.OrderCancelled(order, UserRepository.Queryable.Single(b => b.Id == "2"), "Some Comment", order.StatusCode));
            Assert.AreEqual(1, order.OrderTrackings.Count());
            Assert.AreEqual("FirstName2 LastName2", order.OrderTrackings[0].User.FullName);
            Assert.AreEqual("Name4", order.OrderTrackings[0].StatusCode.Name);
            Assert.AreEqual("cancelled", order.OrderTrackings[0].Description);
            #endregion Assert
        }
        #endregion OrderCancelled Tests

        #region OrderCompleted Tests

        [TestMethod]
        public void TestOrderCompleted()
        {
            #region Arrange
            var users = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Id = (i + 1).ToString(CultureInfo.InvariantCulture);
            }
            new FakeUsers(0, UserRepository, users, true);

            var order = CreateValidEntities.Order(1);
            order.StatusCode = CreateValidEntities.OrderStatusCode(4);
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("2");
            #endregion Arrange

            #region Act
            EventService.OrderCompleted(order);
            #endregion Act

            #region Assert
            Mock.Get(NotificationService).Verify(a => a.OrderCompleted(order, UserRepository.Queryable.Single(b => b.Id == "2")));
            Assert.AreEqual(1, order.OrderTrackings.Count());
            Assert.AreEqual("FirstName2 LastName2", order.OrderTrackings[0].User.FullName);
            Assert.AreEqual("Name4", order.OrderTrackings[0].StatusCode.Name);
            Assert.AreEqual("completed", order.OrderTrackings[0].Description);
            #endregion Assert
        }
        #endregion OrderCompleted Tests

        #region OrderCreated Tests

        [TestMethod]
        public void TestOrderCreated()
        {
            #region Arrange
            var users = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Id = (i + 1).ToString(CultureInfo.InvariantCulture);
            }
            new FakeUsers(0, UserRepository, users, true);
            var statusCodes = new List<OrderStatusCode>();
            statusCodes.Add(CreateValidEntities.OrderStatusCode(1));
            statusCodes[0].Id = OrderStatusCode.Codes.Requester;
            statusCodes[0].Name = "Requestor";
            new FakeOrderStatusCodes(0, OrderStatusCodeRepository, statusCodes, true);
            
            var order = CreateValidEntities.Order(1);
            order.StatusCode = CreateValidEntities.OrderStatusCode(4);
            order.CreatedBy = UserRepository.Queryable.First();
            order.Organization = CreateValidEntities.Organization(1);
            order.Organization.Id = "TEST";
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("2");

            #endregion Arrange

            #region Act
            EventService.OrderCreated(order);
            #endregion Act

            #region Assert
            Mock.Get(NotificationService).Verify(a => a.OrderCreated(order));
            Assert.AreEqual(1, order.OrderTrackings.Count());
            Assert.AreEqual("FirstName2 LastName2", order.OrderTrackings[0].User.FullName);
            Assert.AreEqual("Requestor", order.OrderTrackings[0].StatusCode.Name);
            Assert.AreEqual("created", order.OrderTrackings[0].Description);
            #endregion Assert
        }
        #endregion OrderCreated Tests

        #region OrderReRouted Tests

        [TestMethod]
        public void TestOrderReRouted()
        {
            #region Arrange
            var users = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Id = (i + 1).ToString(CultureInfo.InvariantCulture);
            }
            new FakeUsers(0, UserRepository, users, true);

            var order = CreateValidEntities.Order(1);
            order.StatusCode = CreateValidEntities.OrderStatusCode(4);
            order.StatusCode.Level = 911;
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("2");
            #endregion Arrange

            #region Act
            EventService.OrderReRouted(order);
            #endregion Act

            #region Assert
            Mock.Get(NotificationService).Verify(a => a.OrderReRouted(order, 911, false));
            Assert.AreEqual(1, order.OrderTrackings.Count());
            Assert.AreEqual("FirstName2 LastName2", order.OrderTrackings[0].User.FullName);
            Assert.AreEqual("Name4", order.OrderTrackings[0].StatusCode.Name);
            Assert.AreEqual("edited & rerouted", order.OrderTrackings[0].Description);
            #endregion Assert
        }
        #endregion OrderReRouted Tests

        #region OrderEdited Tests

        [TestMethod]
        public void TesOrderEdited()
        {
            #region Arrange
            var users = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                users[i].Id = (i + 1).ToString(CultureInfo.InvariantCulture);
            }
            new FakeUsers(0, UserRepository, users, true);

            var order = CreateValidEntities.Order(1);
            order.StatusCode = CreateValidEntities.OrderStatusCode(4);
            order.StatusCode.Level = 911;
            Mock.Get(UserIdentity).SetupGet(a => a.Current).Returns("2");
            #endregion Arrange

            #region Act
            EventService.OrderEdited(order);
            #endregion Act

            #region Assert
            Mock.Get(NotificationService).Verify(a => a.OrderEdited(order, UserRepository.Queryable.Single(b => b.Id == "2")));
            Assert.AreEqual(1, order.OrderTrackings.Count());
            Assert.AreEqual("FirstName2 LastName2", order.OrderTrackings[0].User.FullName);
            Assert.AreEqual("Name4", order.OrderTrackings[0].StatusCode.Name);
            Assert.AreEqual("edited", order.OrderTrackings[0].Description);
            #endregion Assert
        }
        #endregion OrderEdited Tests
    }
}
