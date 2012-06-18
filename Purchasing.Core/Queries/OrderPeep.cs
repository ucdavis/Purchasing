using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class OrderPeep : DomainObject
    {
        public virtual int OrderId { get; set; }
        public virtual int WorkgroupId { get; set; }
        public virtual string OrderStatusCodeId { get; set; }
        public virtual string UserId { get; set; }
        public virtual string Fullname { get; set; }
        public virtual bool Administrative { get; set; }
        public virtual bool SharedOrCluster { get; set; }
        public virtual string RoleId { get; set; }
    }

    public class OrderPeepMap : ClassMap<OrderPeep>
    {
        public OrderPeepMap()
        {
            Table("vOrderPeeps");

            ReadOnly();

            Id(x => x.Id);

            Map(x => x.OrderId);
            Map(x => x.WorkgroupId);
            Map(x => x.OrderStatusCodeId);
            Map(x => x.UserId);
            Map(x => x.Fullname);
            Map(x => x.Administrative);
            Map(x => x.SharedOrCluster);
            Map(x => x.RoleId);
        }
    }
}
