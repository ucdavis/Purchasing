using System;
using FluentNHibernate.Mapping;
using Purchasing.Core.Domain;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class CompletedOrdersLastSevenDays : DomainObject
    {
        public virtual User OrderTrackingUser { get; set; }
        public virtual DateTime OrderTrackingDateCreated { get; set; }
        public virtual Order Order { get; set; }
    }

    public class CompletedOrdersLastSevenDaysMap : ClassMap<CompletedOrdersLastSevenDays>
    {
        public CompletedOrdersLastSevenDaysMap()
        {
            ReadOnly();
            Table("vCompletedOrdersLastSevenDays");
            Id(x => x.Id).GeneratedBy.Assigned().Column("OrderTrackingId");
            Map(x => x.OrderTrackingDateCreated);       
            References(x => x.OrderTrackingUser).Column("UserId");
            References(x => x.Order).Column("Id");
        }
    }
}