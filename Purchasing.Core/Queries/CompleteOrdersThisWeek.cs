using System;
using FluentNHibernate.Mapping;
using Purchasing.Core.Domain;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class CompletedOrdersThisWeek : DomainObject
    {
        public virtual string OrderTrackingUser { get; set; }
        public virtual DateTime OrderTrackingDateCreated { get; set; }
        public virtual Order Order { get; set; }
    }

    public class CompletedOrdersThisWeekMap : ClassMap<CompletedOrdersThisWeek>
    {
        public CompletedOrdersThisWeekMap()
        {
            ReadOnly();
            Table("vCompletedOrdersThisWeek");
            Id(x => x.Id).GeneratedBy.Assigned().Column("OrderTrackingId");
            Map(x => x.OrderTrackingDateCreated);       
            Map(x => x.OrderTrackingUser).Column("UserId");
            References(x => x.Order).Column("Id");
        }
    }
}