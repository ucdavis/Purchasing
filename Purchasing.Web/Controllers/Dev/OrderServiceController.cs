using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepository<WorkgroupAccount> _workgroupAccountRepository;
        private readonly IRepositoryWithTypedId<Account,string> _accountRepository; //TODO: should we associate accounts/workgroup accounts with line items?  orders? approvals?
        private readonly IRepositoryWithTypedId<OrderStatusCode, string> _orderStatusCodeRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IOrderService _orderService;

        public OrderServiceController(IRepository<Order> orderRepository, 
            IRepository<Workgroup> workgroupRepository, 
            IRepository<WorkgroupAccount> workgroupAccountRepository,
            IRepositoryWithTypedId<Account, string> accountRepository,
            IRepositoryWithTypedId<OrderStatusCode, string> orderStatusCodeRepository, 
            IRepositoryWithTypedId<User, string> userRepository,
            IOrderService orderService)
        {
            _orderRepository = orderRepository;
            _workgroupRepository = workgroupRepository;
            _workgroupAccountRepository = workgroupAccountRepository;
            _accountRepository = accountRepository;
            _orderStatusCodeRepository = orderStatusCodeRepository;
            _userRepository = userRepository;
            _orderService = orderService;
        }

        //
        // GET: /OrderService/
        public ActionResult Index()
        {
            var orders = _orderRepository.GetAll();

            if (orders.Count > 0)
            {
                _orderService.GetCurrentOrderStatus(1);
            }

            return View(orders);
        }

        /// <summary>
        /// Create a fake order with some approval criteria
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(int workgroupId)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupId);

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
            var workgroup = _workgroupRepository.GetNullableById(workgroupId);

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
                                StatusCode = Repository.OfType<OrderStatusCode>().Queryable.Where(x=>x.Id == OrderStatusCodeId.Requester).Single()
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

            _orderService.AddApprovals(order, workgroupAccountId: accountId, approverId: approverId, accountManagerId: accountManagerId);

            //No splits or anything yet...
            _orderRepository.EnsurePersistent(order);

            Message = "Order Created Without Splits";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Create a fake order split by account
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateSplitOrder(int workgroupId)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupId);

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

            var workgroup = _workgroupRepository.GetNullableById(workgroupId);

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
                StatusCode = Repository.OfType<OrderStatusCode>().Queryable.Where(x => x.Id == OrderStatusCodeId.Requester).Single()
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
                _workgroupAccountRepository
                    .Queryable
                    .Where(x => accountId.Contains(x.Id))
                    .Select(x => x.Account.Id)
                    .ToList();

            //Create splits for each account, all in the same amount, and don't worry about approvals yet
            foreach (var account in underlyingAccountIds)
            {
                var split = new Split { Account = _accountRepository.GetById(account), Amount = 125 };
                order.AddSplit(split);
            }

            _orderService.AddApprovals(order);

            _orderRepository.EnsurePersistent(order);

            Message = "Order Created Including Order-Level Splits";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Create a fake order split by line items
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateLineItemSplitOrder(int workgroupId)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupId);

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

            var workgroup = _workgroupRepository.GetNullableById(workgroupId);

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
                StatusCode = Repository.OfType<OrderStatusCode>().Queryable.Where(x => x.Id == OrderStatusCodeId.Requester).Single()
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
                _workgroupAccountRepository
                    .Queryable
                    .Where(x => accountId1.Contains(x.Id))
                    .Select(x => x.Account.Id)
                    .ToList();

            var underlyingAccountIds2 =
                _workgroupAccountRepository
                    .Queryable
                    .Where(x => accountId2.Contains(x.Id))
                    .Select(x => x.Account.Id)
                    .ToList();

            //Create splits for each account, all in the same amount, and don't worry about approvals yet
            foreach (var account in underlyingAccountIds1)
            {
                var split = new Split { Account = _accountRepository.GetById(account), Amount = 125, LineItem = lineItem1 };
                order.AddSplit(split);
            }

            foreach (var account in underlyingAccountIds2)
            {
                var split = new Split { Account = _accountRepository.GetById(account), Amount = 125, LineItem = lineItem2 };
                order.AddSplit(split);
            }

            _orderService.AddApprovals(order);

            _orderRepository.EnsurePersistent(order);

            Message = "Order Created Including LineItem-Level Splits";
            return RedirectToAction("Index");
        }
    }

    public class OrderServiceCreateModel
    {
        public Workgroup Workgroup { get; set; }

        public List<WorkgroupPermission> Approvers { get; set; }

        public List<WorkgroupPermission> AccountManagers { get; set; }
    }
}
