using Purchasing.Core.Queries;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Core
{
    public interface IQueryRepositoryFactory
    {
        IRepository<CommentHistory> CommentHistoryRepository { get; set; }
        IRepository<OrderTrackingHistory> OrderTrackingHistoryRepository { get; set; }
        IRepository<CompletedOrdersThisMonth> CompletedOrdersThisMonthRepository { get; set; }
        IRepository<CompletedOrdersThisWeek> CompletedOrdersThisWeekRepository { get; set; }
    }

    public class QueryRepositoryFactory : IQueryRepositoryFactory
    {
        public IRepository<CommentHistory> CommentHistoryRepository { get; set; }
        public IRepository<OrderTrackingHistory> OrderTrackingHistoryRepository { get; set; }
        public IRepository<CompletedOrdersThisMonth> CompletedOrdersThisMonthRepository { get; set; }
        public IRepository<CompletedOrdersThisWeek> CompletedOrdersThisWeekRepository { get; set; }
    }
}
