//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Purchasing.Core.Domain;
//using Purchasing.Tests.Core;
////using UCDArch.Testing;
using UCDArch.Testing.Extensions;

//namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
//{

//    public partial class OrderAccessServiceTests
//    {
//        #region Conditional Approver Tests
//        /// <summary>
//        /// Conrad is a conditional Approver for 4 orders
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForConditionalApprovals1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hconrad").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            for(int i = 0; i < 4; i++)
//            {
//                var approval = new Approval();
//                approval.Order = new Order();
//                approval.Order.SetIdTo(i + 1);
//                approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover);
//                approval.User = UserRepository.GetNullableById("hconrad");
//                approval.Completed = false;
//                approvals.Add(approval);
//            }

//            SetupOrders(orders, approvals, 0, null, null, 0, true);
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(4, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// Conrad is a conditional Approver, simpson isn't
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForConditionalApprovals2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);

//            for(int i = 0; i < 4; i++)
//            {
//                var approval = new Approval();
//                approval.Order = new Order();
//                approval.Order.SetIdTo(i + 1);
//                approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover);
//                approval.User = UserRepository.GetNullableById("hconrad");
//                approval.Completed = false;
//                approvals.Add(approval);
//            }

//            SetupOrders(orders, approvals, 0, null, null, 0, true);
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(0, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// Ok, it is in the conditional approval stage, but hsimpson can't see it because hermes is the CA approver, and he is away.
//        /// This does *NOT* trickle up to the wg.
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForConditionalApprovals3()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            var user = CreateValidEntities.User(2);
//            user.FirstName = "Hermes";
//            user.LastName = "Conrad";
//            user.SetIdTo("hconrad");
//            user.IsAway = true;

//            SetupUsers1(user);
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            for(int i = 0; i < 4; i++)
//            {
//                var approval = new Approval();
//                approval.Order = new Order();
//                approval.Order.SetIdTo(i + 1);
//                approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover);
//                approval.User = UserRepository.GetNullableById("hconrad");
//                approval.Completed = false;
//                approvals.Add(approval);
//            }

//            SetupOrders(orders, approvals, 0, null, null, 0, true);
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(0, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// Homer see's these because he is the secondary user.
//        /// </summary>
//        [TestMethod]
//        public void TestOrdersForConditionalApprovals4()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            var user = CreateValidEntities.User(2);
//            user.FirstName = "Hermes";
//            user.LastName = "Conrad";
//            user.SetIdTo("hconrad");
//            user.IsAway = true;

//            SetupUsers1(user);
//            SetupWorkgroupPermissions1();
//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover), 1);
//            for(int i = 0; i < 4; i++)
//            {
//                var approval = new Approval();
//                approval.Order = new Order();
//                approval.Order.SetIdTo(i + 1);
//                approval.StatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.ConditionalApprover);
//                approval.User = UserRepository.GetNullableById("hconrad");
//                approval.SecondaryUser = UserRepository.GetNullableById("hsimpson");
//                approval.Completed = false;
//                approvals.Add(approval);
//            }

//            SetupOrders(orders, approvals, 0, null, null, 0, true);
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(4, results.Count);
//            #endregion Assert
//        }
//        #endregion Conditional Approver Tests
//    }
//}
