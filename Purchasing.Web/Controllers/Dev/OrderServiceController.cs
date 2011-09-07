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

        public OrderServiceController(IRepository<Order> orderRepository, IRepository<Workgroup> workgroupRepository)
        {
            _orderRepository = orderRepository;
            _workgroupRepository = workgroupRepository;
        }

        //
        // GET: /OrderService/
        public ActionResult Index()
        {
            var orders = _orderRepository.GetAll();

            return View(orders);
        }

        /// <summary>
        /// Create a fake order with some approval/split criteria
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
            
            //No splits or anything yet...
            _orderRepository.EnsurePersistent(order);

            Message = "Order Created Without Splits/Line Items";
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
