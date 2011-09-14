using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Controllers.Dev
{
    /// <summary>
    /// Controller for the OrderService class
    /// </summary>
    public class OrderServiceController : ApplicationController
    {
        //TODO: should we associate accounts/workgroup accounts with line items?  orders? approvals?
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IOrderService _orderService;

        public OrderServiceController(IRepositoryFactory repositoryFactory,
            IOrderService orderService)
        {
            _repositoryFactory = repositoryFactory;
            _orderService = orderService;
        }

        //
        // GET: /OrderService/
        public ActionResult Index()
        {
            var orders = _repositoryFactory.OrderRepository.GetAll();

            if (orders.Count > 0)
            {
                _orderService.GetCurrentOrderStatus(1);
            }

            return View(orders);
        }

        /// <summary>
        /// Show details about the given order, mostly related to routing
        /// </summary>
        public ActionResult Details(int id)
        {
            var order = _repositoryFactory.OrderRepository.GetById(id);
            ViewBag.Approvals = _orderService.GetCurrentRequiredApprovals(id);
            ViewBag.CurrentStatus = _orderService.GetCurrentOrderStatus(id);
            ViewBag.Users = _repositoryFactory.UserRepository.GetAll();

            return View(order);
        }

        /// <summary>
        /// Approve an order in the context of a specific user
        /// </summary>
        [HttpPost]
        public ActionResult Approve(int id /*order*/, string user)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Fetch(x => x.Approvals).Where(x => x.Id == id).Single();
            
            _orderService.Approve(order, user);

            _repositoryFactory.OrderRepository.EnsurePersistent(order); //Save approval changes

            Message = "Order Approved by " + user;
            return RedirectToAction("Details", new {id});
        }

        /// <summary>
        /// Create a fake order with some approval criteria
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(int workgroupId)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup Not Found";
                return RedirectToAction("Index");
            }

            var model = new OrderServiceCreateModel
                            {
                                Workgroup = workgroup,
                                Approvers = workgroup.Permissions.Where(x => x.Role.Id == Role.Codes.Approver).ToList(),
                                AccountManagers =
                                    workgroup.Permissions.Where(x => x.Role.Id == Role.Codes.AccountManager).ToList()
                            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(int workgroupId, int? accountId, string approverId, string accountManagerId)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId);

            var order = new Order
                            {
                                VendorId = 1, //fake
                                AddressId = 1, //fake
                                ShippingType = Repository.OfType<ShippingType>().Queryable.First(),
                                DateNeeded = DateTime.Now.AddMonths(1),
                                AllowBackorder = false,
                                EstimatedTax = 8.75m,
                                Workgroup = workgroup,
                                Organization = workgroup.PrimaryOrganization, //why is this needed?
                                ShippingAmount = 12.25m,
                                OrderType = Repository.OfType<OrderType>().Queryable.First(),
                                StatusCode = Repository.OfType<OrderStatusCode>().Queryable.Where(x=>x.Id == OrderStatusCodeId.Approver).Single()
                            };

            var lineItem1 = new LineItem
                                {
                                    Quantity = 5,
                                    CatalogNumber = "SWE23A",//TODO: should this be nullable?
                                    Description = "Test",
                                    Unit = "Each",
                                    UnitPrice = 25.23m
                                };

            var lineItem2 = new LineItem
                                {
                                    Quantity = 2,
                                    CatalogNumber = "ASD2312",//TODO: should this be nullable?
                                    Description = "Another",
                                    Unit = "Each",
                                    UnitPrice = 12.23m
                                };

            order.AddLineItem(lineItem1);
            order.AddLineItem(lineItem2);

            _orderService.CreateApprovalsForNewOrder(order, workgroupAccountId: accountId, approverId: approverId, accountManagerId: accountManagerId);

            //No splits or anything yet...
            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = "Order Created Without Splits";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Create a fake order split by account
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateSplitOrder(int workgroupId)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup Not Found";
                return RedirectToAction("Index");
            }

            var model = new OrderServiceCreateModel {Workgroup = workgroup};

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSplitOrder(int workgroupId, int[] accountId)
        {
            Check.Require(accountId.Count() > 1, "You must select more than one account to split an order");

            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId);

            var order = new Order
            {
                VendorId = 1, //fake
                AddressId = 1, //fake
                ShippingType = Repository.OfType<ShippingType>().Queryable.First(),
                DateNeeded = DateTime.Now.AddMonths(1),
                AllowBackorder = false,
                EstimatedTax = 8.75m,
                Workgroup = workgroup,
                Organization = workgroup.PrimaryOrganization, //why is this needed?
                ShippingAmount = 12.25m,
                OrderType = Repository.OfType<OrderType>().Queryable.First(),
                StatusCode = Repository.OfType<OrderStatusCode>().Queryable.Where(x => x.Id == OrderStatusCodeId.Approver).Single()
                //TODO: What should initial status code be?
            };

            var lineItem1 = new LineItem
            {
                Quantity = 5,
                CatalogNumber = "SWE23A",//TODO: should this be nullable?
                Description = "Test",
                Unit = "Each",
                UnitPrice = 25.23m
            };

            var lineItem2 = new LineItem
            {
                Quantity = 2,
                CatalogNumber = "ASD2312",//TODO: should this be nullable?
                Description = "Another",
                Unit = "Each",
                UnitPrice = 12.23m
            };

            order.AddLineItem(lineItem1);
            order.AddLineItem(lineItem2);
            
            var underlyingAccountIds =
                _repositoryFactory.WorkgroupAccountRepository
                    .Queryable
                    .Where(x => accountId.Contains(x.Id))
                    .Select(x => x.Account.Id)
                    .ToList();

            //Create splits for each account, all in the same amount, and don't worry about approvals yet
            foreach (var account in underlyingAccountIds)
            {
                var split = new Split { Account = _repositoryFactory.AccountRepository.GetById(account), Amount = 125 };
                order.AddSplit(split);
            }

            _orderService.CreateApprovalsForNewOrder(order);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = "Order Created Including Order-Level Splits";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Create a fake order split by line items
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateLineItemSplitOrder(int workgroupId)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup Not Found";
                return RedirectToAction("Index");
            }

            var model = new OrderServiceCreateModel { Workgroup = workgroup };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateLineItemSplitOrder(int workgroupId, int[] accountId1, int[] accountId2)
        {
            Check.Require(accountId1.Count() > 1, "You must select more than one account to split on line1");
            Check.Require(accountId2.Count() > 1, "You must select more than one account to split on line2");

            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId);

            var order = new Order
            {
                VendorId = 1, //fake
                AddressId = 1, //fake
                ShippingType = Repository.OfType<ShippingType>().Queryable.First(),
                DateNeeded = DateTime.Now.AddMonths(1),
                AllowBackorder = false,
                EstimatedTax = 8.75m,
                Workgroup = workgroup,
                Organization = workgroup.PrimaryOrganization, //why is this needed?
                ShippingAmount = 12.25m,
                OrderType = Repository.OfType<OrderType>().Queryable.First(),
                StatusCode = Repository.OfType<OrderStatusCode>().Queryable.Where(x => x.Id == OrderStatusCodeId.Approver).Single()
            };

            var lineItem1 = new LineItem
            {
                Quantity = 5,
                CatalogNumber = "SWE23A",//TODO: should this be nullable?
                Description = "Test",
                Unit = "Each",
                UnitPrice = 25.23m
            };

            var lineItem2 = new LineItem
            {
                Quantity = 2,
                CatalogNumber = "ASD2312",//TODO: should this be nullable?
                Description = "Another",
                Unit = "Each",
                UnitPrice = 12.23m
            };

            order.AddLineItem(lineItem1);
            order.AddLineItem(lineItem2);

            var underlyingAccountIds1 =
                _repositoryFactory.WorkgroupAccountRepository
                    .Queryable
                    .Where(x => accountId1.Contains(x.Id))
                    .Select(x => x.Account.Id)
                    .ToList();

            var underlyingAccountIds2 =
                _repositoryFactory.WorkgroupAccountRepository
                    .Queryable
                    .Where(x => accountId2.Contains(x.Id))
                    .Select(x => x.Account.Id)
                    .ToList();

            //Create splits for each account, all in the same amount, and don't worry about approvals yet
            foreach (var account in underlyingAccountIds1)
            {
                var split = new Split { Account = _repositoryFactory.AccountRepository.GetById(account), Amount = 125, LineItem = lineItem1 };
                order.AddSplit(split);
            }

            foreach (var account in underlyingAccountIds2)
            {
                var split = new Split { Account = _repositoryFactory.AccountRepository.GetById(account), Amount = 125, LineItem = lineItem2 };
                order.AddSplit(split);
            }

            _orderService.CreateApprovalsForNewOrder(order);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = "Order Created Including LineItem-Level Splits";
            return RedirectToAction("Index");
        }

        public ActionResult CreateConditionalApprovals(int workgroupId)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup Not Found";
                return RedirectToAction("Index");
            }

            var model = new OrderServiceCreateModel
                            {
                                Workgroup = workgroup,
                                ConditionalApprovals = _repositoryFactory.ConditionalApprovalRepository.GetAll(),
                                Approvers = workgroup.Permissions.Where(x => x.Role.Id == Role.Codes.Approver).ToList(),
                                AccountManagers =
                                    workgroup.Permissions.Where(x => x.Role.Id == Role.Codes.AccountManager).ToList()
                            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateConditionalApprovals(int workgroupId, int[] conditionalApprovals, int accountId)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId);

            var order = new Order
            {
                VendorId = 1, //fake
                AddressId = 1, //fake
                ShippingType = Repository.OfType<ShippingType>().Queryable.First(),
                DateNeeded = DateTime.Now.AddMonths(1),
                AllowBackorder = false,
                EstimatedTax = 8.75m,
                Workgroup = workgroup,
                Organization = workgroup.PrimaryOrganization, //why is this needed?
                ShippingAmount = 12.25m,
                OrderType = Repository.OfType<OrderType>().Queryable.First(),
                StatusCode = Repository.OfType<OrderStatusCode>().Queryable.Where(x => x.Id == OrderStatusCodeId.Approver).Single()
            };

            var lineItem1 = new LineItem
            {
                Quantity = 5,
                CatalogNumber = "SWE23A",//TODO: should this be nullable?
                Description = "Test",
                Unit = "Each",
                UnitPrice = 25.23m
            };

            var lineItem2 = new LineItem
            {
                Quantity = 2,
                CatalogNumber = "ASD2312",//TODO: should this be nullable?
                Description = "Another",
                Unit = "Each",
                UnitPrice = 12.23m
            };

            order.AddLineItem(lineItem1);
            order.AddLineItem(lineItem2);

            _orderService.CreateApprovalsForNewOrder(order, 
                                        conditionalApprovalIds: conditionalApprovals.Where(x => x != 0).ToArray(), //only pass the chosen ones
                                        workgroupAccountId: accountId);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = "Order Created With Conditional Approvals";
            return RedirectToAction("Index");
        }

    }

    public class OrderServiceCreateModel
    {
        public Workgroup Workgroup { get; set; }

        public List<WorkgroupPermission> Approvers { get; set; }

        public List<WorkgroupPermission> AccountManagers { get; set; }

        public IList<ConditionalApproval> ConditionalApprovals { get; set; }
    }
}
