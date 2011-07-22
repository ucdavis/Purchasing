using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Order : DomainObject
    {
        public virtual OrderType OrderType { get; set; }
        public virtual int VendorId { get; set; }//TODO: Replace with actual vendor
        public virtual int AddressId { get; set; }//TODO: Replace
        public virtual ShippingType ShippingType { get; set; }
        public virtual DateTime DateNeeded { get; set; }
        public virtual bool AllowBackorder { get; set; }
        public virtual decimal EstimatedTax { get; set; }
        public virtual Workgroup Workgroup { get; set; }
        public virtual string PoNumber { get; set; }
        public virtual Approval LastCompletedApproval { get; set; }
    }

    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Map(x => x.DateNeeded);
            Map(x => x.AllowBackorder);
            Map(x => x.EstimatedTax);
            Map(x => x.PoNumber);
            
            References(x => x.OrderType);
            References(x => x.ShippingType);
            References(x => x.Workgroup);
            References(x => x.LastCompletedApproval);
        }
    }
}