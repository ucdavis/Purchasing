using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Helpers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the AlanTest class
    /// </summary>
    public class AlanTestController : ApplicationController
    {
        private readonly IOrderService _orderAccessService;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Approval> _approvalRepository;

        public AlanTestController(IOrderService orderAccessService, IRepository<Order> orderRepository, IRepository<Approval> approvalRepository )
        {
            _orderAccessService = orderAccessService;
            _orderRepository = orderRepository;
            _approvalRepository = approvalRepository;
        }

        public ActionResult Index()
        {
            

            var time1 = DateTime.Now;

            ViewBag.Time1 = time1.ToString();
            ViewBag.Ticks1 = time1.Ticks;
            ViewBag.EncodedId1 = string.Format("AAES-{0}", time1.Ticks.GetHashCode().ConvertToBase36());

            // display some defaults
            Message = "No Collision, hazahh!";
            ViewBag.Time2 = time1.ToString();
            ViewBag.Ticks2 = time1.Ticks;
            ViewBag.EncodedId2 = string.Format("AAES-{0}", time1.Ticks.GetHashCode());

            // see if we can find a collision in has
            var timer = time1;
            
            do
            {

                timer = timer.AddTicks(1);

                if (time1.Ticks.GetHashCode() == timer.Ticks.GetHashCode())
                {
                    Message = "Collision!";
                    ViewBag.Time2 = timer.ToString();
                    ViewBag.Ticks2 = timer.Ticks;
                    ViewBag.EncodedId2 = string.Format("AAES-{0}", timer.Ticks.GetHashCode());

                    break;
                }

                ViewBag.LastTime = timer.ToString();

            } while (timer < DateTime.Now.AddYears(5));


            return View();

            //var viewModel = DashboardViewModel.Create(Repository);
            //return View(viewModel);
        }

        private string EncodeId(int hashCode)
        {
            var encodedId = (hashCode*31) ^ User.Identity.Name.GetHashCode();
            return encodedId.ConvertToBase36();
        }

        public ActionResult AdminOrders()
        {
            var results = _orderAccessService.GetAdministrativeListofOrders();

            return View(results);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusFilter"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="showAll">Matches AllActive in GetListOfOrders</param>
        /// <param name="showCompleted">Matches All in GetListOfOrders</param>
        /// <param name="showOwned"></param>
        /// <returns></returns>
        public ActionResult Orders(string[] statusFilter, DateTime? startDate, DateTime? endDate, bool showAll = false, bool showCompleted = false, bool showOwned = false, bool hideCreatedByYou = false)
        {
            if (statusFilter == null)
            {
                statusFilter = new string[0];
            }

            var filters = statusFilter.ToList();
            var list = Repository.OfType<OrderStatusCode>().Queryable.Where(a => filters.Contains(a.Id)).ToList();

            var orders = _orderAccessService.GetListofOrders(showAll, showCompleted, showOwned, hideCreatedByYou, list, startDate, endDate);
            var viewModel = FilteredOrderListModel.Create(Repository, orders);
            viewModel.CheckedOrderStatusCodes = filters;
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.ShowAll = showAll;
            viewModel.ShowCompleted = showCompleted;
            viewModel.ShowOwned = showOwned;

            return View(viewModel);

        }

        public ActionResult Checkbox()
        {
            return View();
        }

        public ActionResult Table()
        {
            return View();
        }

        public JsonNetResult Updatetable(int? dataid)
        {
            var peeps = new List<Peep>();

            peeps.Add(new Peep(){FirstName = "Herbert", LastName = "Farnsworth", Email = "hfarns@planex.com"});
            peeps.Add(new Peep(){FirstName = "Cubert", LastName = "Farnsworth", Email = "cfarns@planex.com"});

            return new JsonNetResult(peeps);
        }


    }

    public class Peep
    {
        public string FirstName { get; set; }
        public string MI { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class DashboardViewModel
    {
        public static DashboardViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new DashboardViewModel();

            return viewModel;
        }
    }

}
