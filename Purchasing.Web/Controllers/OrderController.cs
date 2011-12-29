using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Order class
    /// </summary>
    public class OrderController : ApplicationController
    {
	    private readonly IRepository<Order> _orderRepository;
        private readonly IOrderAccessService _orderAccessService;
        private readonly IRepositoryWithTypedId<ColumnPreferences, string> _columnPreferences;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly ISecurityService _securityService;

        public OrderController(IRepository<Order> orderRepository, IOrderAccessService orderAccessService, IRepositoryWithTypedId<ColumnPreferences, string> columnPreferences, IRepositoryWithTypedId<Role, string> roleRepository, ISecurityService securityService )
        {
            _orderRepository = orderRepository;
            _orderAccessService = orderAccessService;
            _columnPreferences = columnPreferences;
            _roleRepository = roleRepository;
            _securityService = securityService;
        }

        /// <summary>
        /// List of orders
        /// </summary>
        /// <param name="statusFilter"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="showAll">Matches AllActive in GetListOfOrders</param>
        /// <param name="showCompleted">Matches All in GetListOfOrders</param>
        /// <param name="showOwned"></param>
        /// <param name="hideOrdersYouCreated">Hide orders which you have created</param>
        /// <returns></returns>
        public ActionResult Index(string[] statusFilter, DateTime? startDate, DateTime? endDate, bool showAll = false, bool showCompleted = false, bool showOwned = false, bool hideOrdersYouCreated = false)
        {
            //TODO: Review even/odd display of table once Trish has look at it. (This page is a single, and the background color is the same as the even background color.
            if(statusFilter == null)
            {
                statusFilter = new string[0];
            }

            var filters = statusFilter.ToList();
            var list = Repository.OfType<OrderStatusCode>().Queryable.Where(a => filters.Contains(a.Id)).ToList();

            var orders = _orderAccessService.GetListofOrders(showAll, showCompleted, showOwned, hideOrdersYouCreated, list, startDate, endDate);
            var viewModel = FilteredOrderListModel.Create(Repository, orders);
            viewModel.CheckedOrderStatusCodes = filters;
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.ShowAll = showAll;
            viewModel.ShowCompleted = showCompleted;
            viewModel.ShowOwned = showOwned;
            viewModel.HideOrdersYouCreated = hideOrdersYouCreated;
            viewModel.ColumnPreferences = _columnPreferences.GetNullableById(CurrentUser.Identity.Name) ??
                                          new ColumnPreferences(CurrentUser.Identity.Name);

            return View(viewModel);

        }

        /// <summary>
        /// Page to view Administrative Workgroup Orders
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminOrders(string[] statusFilter, DateTime? startDate, DateTime? endDate, bool showAll = false, bool showCompleted = false, bool showOwned = false, bool hideOrdersYouCreated = false)
        {
            //TODO: Review even/odd display of table once Trish has look at it. (This page is a single, and the background color is the same as the even background color.
            if(statusFilter == null)
            {
                statusFilter = new string[0];
            }

            var filters = statusFilter.ToList();
            var list = Repository.OfType<OrderStatusCode>().Queryable.Where(a => filters.Contains(a.Id)).ToList();

            //TODO: replace/update this so it gets the admin list of orders.
            var orders = _orderAccessService.GetListofOrders(showAll, showCompleted, showOwned, hideOrdersYouCreated, list, startDate, endDate);
            var viewModel = FilteredOrderListModel.Create(Repository, orders);
            viewModel.CheckedOrderStatusCodes = filters;
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.ShowAll = showAll;
            viewModel.ShowCompleted = showCompleted;
            viewModel.ShowOwned = showOwned;
            viewModel.HideOrdersYouCreated = hideOrdersYouCreated;
            viewModel.ColumnPreferences = _columnPreferences.GetNullableById(CurrentUser.Identity.Name) ??
                                          new ColumnPreferences(CurrentUser.Identity.Name);

            return View(viewModel);

        }

        /// <summary>
        /// If user has more than one workgroup, they select it for their order
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectWorkgroup()
        {
            var user = GetCurrentUser();
            var role = _roleRepository.GetNullableById(Role.Codes.Requester);
            var workgroups = user.WorkgroupPermissions.Where(a => a.Role == role && !a.Workgroup.Administrative).Select(a=>a.Workgroup);

            // only one workgroup, automatically redirect
            if (workgroups.Count() == 1)
            {
                var workgroup = workgroups.First();
                return this.RedirectToAction<OrderMockupController>(a => a.Request(workgroup.Id));
            }
            
            return View(workgroups.ToList());
        }

        /// <summary>
        /// Page to review an order and for approving/denying the order.
        /// </summary>
        /// <remarks>
        /// This page should be used by ad hoc account managers too, but without the link to edit
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ReadOnly(int id)
        {
            var order = _orderRepository.GetNullableById(id);

            if (order == null)
            {
                Message = "Order not found.";
                //TODO: Workout a way to get a return to where the person came from, rather than just redirecting to the generic index
                return RedirectToAction("index");
            }
            
            var status = _orderAccessService.GetAccessLevel(order);

            ViewBag.CanEdit = status == OrderAccessLevel.Edit;

            return View(order);
        }
    }
}
