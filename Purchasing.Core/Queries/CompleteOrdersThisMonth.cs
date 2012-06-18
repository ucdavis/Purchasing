using System;
using FluentNHibernate.Mapping;
using Purchasing.Core.Domain;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    // TODO: If we switch to using stored procedure, we can parameterize query for period and use one sp for both month and week
    public class CompletedOrdersThisMonth : DomainObject
    {
        public virtual string OrderTrackingUser { get; set; }
        public virtual DateTime OrderTrackingDateCreated { get; set; }
        public virtual Order Order { get; set; }
    }

    public class CompletedOrdersThisMonthMap : ClassMap<CompletedOrdersThisMonth>
    {
        public CompletedOrdersThisMonthMap()
        {
            ReadOnly();
            Table("vCompletedOrdersThisMonth"); 
            Id(x => x.Id).GeneratedBy.Assigned().Column("OrderTrackingId");
            Map(x => x.OrderTrackingDateCreated);       
            Map(x => x.OrderTrackingUser).Column("UserId");
            References(x => x.Order).Column("Id");
        }
    }
}