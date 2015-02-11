using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class OrderTracking : DomainObject
    {
        public OrderTracking()
        {
            DateCreated = DateTime.Now.ToUniversalTime();
        }
        [Required]
        public virtual string Description { get; set; } //TODO: Enum?
        public virtual DateTime DateCreated { get; set; }
        [Required]
        public virtual OrderStatusCode StatusCode { get; set; }
        [Required]
        public virtual Order Order { get; set; }
        [Required]
        public virtual User User { get; set; }
    }

    public class OrderTrackingMap : ClassMap<OrderTracking>
    {
        public OrderTrackingMap()
        {
            Table("OrderTracking");

            Id(x => x.Id);

            Map(x => x.Description).Length(int.MaxValue);
            Map(x => x.DateCreated);

            References(x => x.Order);
            References(x => x.User);
            References(x => x.StatusCode);
        }
    }
}