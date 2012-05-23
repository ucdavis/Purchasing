using FluentNHibernate.Mapping;

namespace Purchasing.Core.Queries
{
    public class OpenOrderByUser : OrderHistoryBase
    {
        public virtual string VendorName { get; set; }
    }

    public class OpenOrderByUserMap : ClassMap<OpenOrderByUser>
    {
        public OpenOrderByUserMap()
        {
            Id(x => x.Id);

            Table("vOpenOrdersByUser");
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
            Map(x => x.VendorName);
        }
    }
}
