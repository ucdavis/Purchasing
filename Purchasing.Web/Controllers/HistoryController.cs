using System.Linq;
using Purchasing.Core;
using System.Web.Mvc;
using Purchasing.Core.Queries;

namespace Purchasing.Web.Controllers
{
    [Authorize]
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
            var lastOrderEvent = _queryRepositoryFactory.OrderTrackingHistoryRepository.Queryable.Where(
                d => d.AccessUserId == CurrentUser.Identity.Name).OrderByDescending(e => e.DateCreated).FirstOrDefault();

            return PartialView(lastOrderEvent);
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