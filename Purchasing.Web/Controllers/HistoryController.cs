using System;
using System.Linq;
using Purchasing.Core;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Helpers;

namespace Purchasing.Web.Controllers
{
    public class HistoryController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;

        public HistoryController(IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _queryRepositoryFactory = queryRepositoryFactory;
        }

        public ActionResult RecentActivity()
        {
            var lastOrderEvent = _repositoryFactory.OrderTrackingRepository.Queryable.Where(
                d => d.User.Id == CurrentUser.Identity.Name).
                OrderByDescending(e => e.DateCreated).FirstOrDefault();

            var lastOrder = lastOrderEvent == null ? null : lastOrderEvent.Order;

            return PartialView(lastOrder);
        }

        public ActionResult RecentComments()
        {
            var recentComments = Repository.OfType<CommentHistory>()
                .Queryable.Where(a => a.AccessUserId == CurrentUser.Identity.Name)
                .OrderByDescending(o => o.DateCreated)
                .Take(5).ToList();

            return PartialView(recentComments);
        }

        public ActionResult RecentlyFinished()
        {
            //viewModel.FinishedThisWeekCount =
              //  Repository.OfType<CompletedOrdersThisWeek>().Queryable.Count(c => c.OrderTrackingUser == CurrentUser.Identity.Name);
            //TODO: create a repository factory for just the queries
            
            var finishedThisMonth = Repository.OfType<CompletedOrdersThisMonth>().Queryable.Count(a => a.OrderTrackingUser == CurrentUser.Identity.Name);

            return PartialView(finishedThisMonth);
        }
    }
}