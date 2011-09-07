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
        private readonly IRepositoryWithTypedId<OrderStatusCode, string> _orderStatusCodeRepository;

        public OrderServiceController(IRepository<Order> orderRepository, IRepository<Workgroup> workgroupRepository, IRepository<WorkgroupAccount> workgroupAccountRepository, IRepositoryWithTypedId<OrderStatusCode, string> orderStatusCodeRepository)
        {
            _orderRepository = orderRepository;
            _workgroupRepository = workgroupRepository;
            _workgroupAccountRepository = workgroupAccountRepository;
            _orderStatusCodeRepository = orderStatusCodeRepository;
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
            
            //Add in approvals for selected options
            if (accountId.HasValue) //Account was selected, try to route to the proper workgroup account people, or if not just use workgroup
            {
                var account = _workgroupAccountRepository.GetNullableById(accountId.Value);

                //Add in approval steps
                var approverApproval = new Approval
                                           {
                                               Approved = false,
                                               Level = 2, //TODO: is this redundant with status code?
                                               User = account.Approver,
                                               StatusCode =
                                                   _orderStatusCodeRepository.Queryable.Where(
                                                       x => x.Id == OrderStatusCodeId.Approver).Single()
                                           };

                var acctManagerApproval = new Approval
                {
                    Approved = false,
                    Level = 3, //TODO: is this redundant with status code?
                    User = account.AccountManager,
                    StatusCode =
                        _orderStatusCodeRepository.Queryable.Where(
                            x => x.Id == OrderStatusCodeId.AccountManager).Single()
                };

                var purchaserApproval = new Approval
                {
                    Approved = false,
                    Level = 4, //TODO: is this redundant with status code?
                    User = account.Purchaser,
                    StatusCode =
                        _orderStatusCodeRepository.Queryable.Where(
                            x => x.Id == OrderStatusCodeId.Purchaser).Single()
                };

                order.AddApproval(approverApproval);
                order.AddApproval(acctManagerApproval);
                order.AddApproval(purchaserApproval);
            }

            //No splits or anything yet...
            _orderRepository.EnsurePersistent(order);

            Message = "Order Created Without Splits";
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
