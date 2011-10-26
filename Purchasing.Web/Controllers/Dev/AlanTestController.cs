using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the AlanTest class
    /// </summary>
    public class AlanTestController : ApplicationController
    {
        private readonly IOrderAccessService _orderAccessService;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Approval> _approvalRepository;

        public AlanTestController(IOrderAccessService orderAccessService, IRepository<Order> orderRepository, IRepository<Approval> approvalRepository )
        {
            _orderAccessService = orderAccessService;
            _orderRepository = orderRepository;
            _approvalRepository = approvalRepository;
        }

        public ActionResult Index()
        {
            var orders = _orderAccessService.GetListofOrders();
            //var orders = new List<Order>();

            //var user = Repository.OfType<User>().Queryable.Where(a => a.Id == CurrentUser.Identity.Name).Single();
            //var permissions = Repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.User == user).ToList();

            //var approvals = new List<Approval>();
            //foreach (var perm in permissions)
            //{

            //    var result = from a in _approvalRepository.Queryable
            //                 where a.Order.Workgroup == perm.Workgroup && a.StatusCode.Level == perm.Role.Level
            //                    && a.StatusCode == a.Order.StatusCode && !a.Approved.HasValue
            //                    && (
            //                        (a.User == null)    // not assigned, use workgroup
            //                        ||
            //                        (a.User == user || a.SecondaryUser == user) // user is assigned
            //                        ||
            //                        (a.StatusCode.Id != OrderStatusCode.Codes.ConditionalApprover && a.User.IsAway)  // in standard approval, is user away
            //                        )
            //                 select a.Order;

            //    orders.AddRange(result.ToList());
            //}

            return View(orders);
        }

    }
}
