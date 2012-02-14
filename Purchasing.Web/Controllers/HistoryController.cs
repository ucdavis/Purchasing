using System;
using System.Linq;
using Purchasing.Core;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Helpers;

namespace Purchasing.Web.Controllers
{
    public class HistoryController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public HistoryController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        [ChildActionOnly]
        public ActionResult RecentActivity()
        {
            var lastOrderEvent = _repositoryFactory.OrderTrackingRepository.Queryable.Where(
                d => d.User.Id == CurrentUser.Identity.Name).
                OrderByDescending(e => e.DateCreated).FirstOrDefault();

            var lastOrder = lastOrderEvent == null ? null : lastOrderEvent.Order;

            return PartialView(lastOrder);
        }
    }
}