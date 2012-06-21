using FluentNHibernate.Mapping;

namespace Purchasing.Core.Queries
{
    public class PendingOrder : OrderHistoryBase { }

    public class PendingOrderMap : ClassMap<PendingOrder>
    {
        public PendingOrderMap()
        {
            Id(x => x.Id);

            Table("vPendingOrders");
            ReadOnly();

            Map(x => x.OrderId);
            Map(x => x.RequestNumber);
            Map(x => x.DateCreated);
            Map(x => x.DateNeeded);
            Map(x => x.Creator);
            Map(x => x.LastActionDate);
            Map(x => x.StatusName);
            Map(x => x.Summary);
            Map(x => x.AccessUserId);
            Map(x => x.IsDirectlyAssigned);
        }
    }
}
