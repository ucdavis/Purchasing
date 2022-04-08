//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Purchasing.Core.Domain;
//using Purchasing.Tests.Core;
//
//namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
//{
//    public partial class OrderAccessServiceTests
//    {
//        #region User With Multiple Roles Tests

//        [TestMethod]
//        public void TestWhenRequestorAndApproverDifferentWorkGroups1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(2);
//            workgroupPermissions.Add(workgroupPermission);
//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(6, results.Count, "Should only see 6, 4 + 2 for wg 1 where simpson is the approver.");

//            #endregion Assert		
//        }

//        /// <summary>
//        /// Should see 7. 6 from wg 1 where simpson is the approver. 1 from wg 2 where simpson is the requestor
//        /// </summary>
//        [TestMethod]
//        public void TestWhenRequestorAndApproverDifferentWorkGroups2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(2);
//            workgroupPermissions.Add(workgroupPermission);
//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(7, results.Count);

//            #endregion Assert
//        }

//        /// <summary>
//        /// Should see 7 because is an approver for both wg
//        /// </summary>
//        [TestMethod]
//        public void TestWhenApproverForDifferentWorkGroups1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Approver);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(2);
//            workgroupPermissions.Add(workgroupPermission);
//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(7, results.Count);

//            #endregion Assert
//        }

//        /// <summary>
//        /// Should see 12 because simpson is an approver and account manager for wg, and there are 6 approvals where simpson is the user and 6 other where simpson is the approver.
//        /// </summary>
//        [TestMethod]
//        public void TestWhenApproverAndAccountManager1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);
//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 6, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            foreach (var approval in approvals)
//            {
//                if(approval.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
//                {
//                    approval.User = UserRepository.GetNullableById("hsimpson");
//                }
//            }

//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(12, results.Count);

//            #endregion Assert
//        }

//        /// <summary>
//        /// Should see 12 because simpson is an approver and account manager for wg, and there are 6 approvals where simpson is the user and 6 other where simpson is the approver.
//        /// basically same as above, but 2 orders added where flanders is the AccountManager
//        /// </summary>
//        [TestMethod]
//        public void TestWhenApproverAndAccountManager2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);
//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 6, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
//                {
//                    approval.User = UserRepository.GetNullableById("hsimpson");
//                }
//            }

//            SetupOrders(orders, approvals, 2, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);

//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(12, results.Count);

//            #endregion Assert
//        }

//        /// <summary>
//        /// Should see 12 because simpson is an approver and account manager for wg, and there are 6 approvals where simpson is the user and 6 other where simpson is the approver.
//        /// basically same as above, but simpson is secondary user and user is still flanders
//        /// </summary>
//        [TestMethod]
//        public void TestWhenApproverAndAccountManager3()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);
//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 6, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
//                {
//                    approval.SecondaryUser = UserRepository.GetNullableById("hsimpson");
//                }
//            }

//            SetupOrders(orders, approvals, 2, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);

//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(12, results.Count);

//            #endregion Assert
//        }

//        /// <summary>
//        /// Should see 12 because simpson is an approver and account manager for wg, and there are 6 approvals where simpson is the user and 6 other where simpson is the approver.
//        /// basically same as above, but added orders in purchase state
//        /// </summary>
//        [TestMethod]
//        public void TestWhenApproverAndAccountManager4()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);
//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 6, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
//                {
//                    approval.SecondaryUser = UserRepository.GetNullableById("hsimpson");
//                }
//            }

//            SetupOrders(orders, approvals, 2, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 9, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(12, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// Should see 12 because simpson is an approver and account manager for wg, and there are 6 approvals where simpson is the user and 6 other where simpson is the approver.
//        /// basically same as above, simpson is now requestor too.
//        /// </summary>
//        [TestMethod]
//        public void TestWhenApproverAndAccountManager5()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);
//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 6, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
//                {
//                    approval.SecondaryUser = UserRepository.GetNullableById("hsimpson");
//                }
//            }

//            SetupOrders(orders, approvals, 2, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 9, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(12, results.Count);
//            #endregion Assert
//        }

//        /// <summary>
//        /// Should see 21 because simpson is an approver and account manager for wg, and there are 6 approvals where simpson is the user and 6 other where simpson is the approver.
//        /// basically same as above, simpson is now requestor too for orders in purchased state.
//        /// </summary>
//        [TestMethod]
//        public void TestWhenApproverAndAccountManager6()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);
//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 6, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
//                {
//                    approval.SecondaryUser = UserRepository.GetNullableById("hsimpson");
//                }
//            }

//            SetupOrders(orders, approvals, 2, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 9, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            // Remove hsimpson from all approvals for wg 2 
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach(var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(21, results.Count);
//            #endregion Assert
//        }
//        /// <summary>
//        /// still just 12, because the orders in purchase status don't have simpson assigned.
//        /// </summary>
//        [TestMethod]
//        public void TestAproverAccountManagerAndPurchaser1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(3);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 6, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode.Id == OrderStatusCode.Codes.AccountManager || approval.StatusCode.Id == OrderStatusCode.Codes.Purchaser)
//                {
//                    approval.SecondaryUser = UserRepository.GetNullableById("hsimpson");
//                }
//            }

//            SetupOrders(orders, approvals, 2, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 9, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);

//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2); //This is Order #22

//            // Remove simpson from all approvals for wg 2 (order 22 added above)
//            var lastOrder = orders.Count();
//            var appr = approvals.Single(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson");
//            appr.User = null;

//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(12, results.Count);
//            #endregion Assert	
//        }

//        /// <summary>
//        /// 23 because simpson is now purchaser in the approval table for those orders too
//        /// </summary>
//        [TestMethod]
//        public void TestAproverAccountManagerAndPurchaser2()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
//            SetupUsers1();

//            var workgroupPermissions = new List<WorkgroupPermission>();
//            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.AccountManager);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);

//            workgroupPermission = CreateValidEntities.WorkgroupPermission(3);
//            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
//            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Purchaser);
//            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(1);
//            workgroupPermissions.Add(workgroupPermission);
//            SetupWorkgroupPermissions1(workgroupPermissions);

//            var orders = new List<Order>();
//            var approvals = new List<Approval>();
//            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
//            SetupOrders(orders, approvals, 6, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);


//            SetupOrders(orders, approvals, 2, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
//            SetupOrders(orders, approvals, 9, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
//            foreach(var approval in approvals)
//            {
//                if(approval.StatusCode.Id == OrderStatusCode.Codes.AccountManager || approval.StatusCode.Id == OrderStatusCode.Codes.Purchaser)
//                {
//                    approval.SecondaryUser = UserRepository.GetNullableById("hsimpson");
//                }
//            }
//            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
//            // Remove simpson from all approvals for wg 2 (order 22 added above)
//            var lastOrder = orders.Count();
//            var apprs = approvals.Where(a => a.Order.Id == lastOrder && a.User != null && a.User.Id == "hsimpson").ToList();
//            foreach (var aprv in apprs)
//            {
//                aprv.User = null;
//            }
//            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(23, results.Count);
//            #endregion Assert
//        }
//        #endregion User With Multiple Roles Tests
//    }
//}
