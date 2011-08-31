using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class OrderTracking : DomainObject
    {
        public virtual string Description { get; set; } //TODO: Enum?
        public virtual DateTime DateCreated { get; set; }
        public virtual OrderStatusCode StatusCode { get; set; }

        public virtual Order Order { get; set; }
        public virtual User User { get; set; }
    }

    public class OrderTrackingMap : ClassMap<OrderTracking>
    {
        public OrderTrackingMap()
        {
            Table("OrderTracking");

            Id(x => x.Id);

            Map(x => x.Description);
            Map(x => x.DateCreated);

            References(x => x.Order);
            References(x => x.User);
            References(x => x.StatusCode);
        }
    }
}