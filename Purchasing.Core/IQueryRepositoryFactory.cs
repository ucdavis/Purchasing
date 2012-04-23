using Purchasing.Core.Queries;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Core
{
    public interface IQueryRepositoryFactory
    {
        IRepository<Access> AccessRepository { get; set; }
        IRepository<OrganizationDescendant> OrganizationDescendantRepository { get; set; }
        IRepository<AdminOrderAccess> AdminOrderAccessRepository { get; set; }
        IRepository<AdminWorkgroup> AdminWorkgroupRepository { get; set; }
        IRepository<AdminOrg> AdminOrgRepository { get; set; }
        IRepository<CommentHistory> CommentHistoryRepository { get; set; }
        IRepository<OrderTrackingHistory> OrderTrackingHistoryRepository { get; set; }
        IRepository<CompletedOrdersThisMonth> CompletedOrdersThisMonthRepository { get; set; }
        IRepository<CompletedOrdersThisWeek> CompletedOrdersThisWeekRepository { get; set; }
        IRepository<OpenOrderByUser> OpenOrderByUserRepository { get; set; }
        IRepository<PendingOrder> PendingOrderRepository { get; set; }
        IRepository<OrderPeep> OrderPeepRepository { get; set; }
    }

    public class QueryRepositoryFactory : IQueryRepositoryFactory
    {
        public IRepository<Access> AccessRepository { get; set; }
        public IRepository<OrganizationDescendant> OrganizationDescendantRepository { get; set; }
        public IRepository<AdminOrderAccess> AdminOrderAccessRepository { get; set; }
        public IRepository<AdminWorkgroup> AdminWorkgroupRepository { get; set; }
        public IRepository<AdminOrg> AdminOrgRepository { get; set; }

        public IRepository<CommentHistory> CommentHistoryRepository { get; set; }
        public IRepository<OrderTrackingHistory> OrderTrackingHistoryRepository { get; set; }
        public IRepository<CompletedOrdersThisMonth> CompletedOrdersThisMonthRepository { get; set; }
        public IRepository<CompletedOrdersThisWeek> CompletedOrdersThisWeekRepository { get; set; }
        public IRepository<PendingOrder> PendingOrderRepository { get; set; }
        public IRepository<OpenOrderByUser> OpenOrderByUserRepository { get; set; }
        public IRepository<OrderPeep> OrderPeepRepository { get; set; }
    }
}
