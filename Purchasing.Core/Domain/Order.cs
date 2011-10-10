using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Order : DomainObject
    {
        public Order()
        {
            LineItems = new List<LineItem>();
            Approvals = new List<Approval>();
            Splits = new List<Split>();
            OrderTrackings = new List<OrderTracking>();
            KfsDocuments = new List<KfsDocument>();

            DateCreated = DateTime.Now;
        }

        public virtual OrderType OrderType { get; set; }
        public virtual int VendorId { get; set; }//TODO: Replace with actual vendor
        public virtual int AddressId { get; set; }//TODO: Replace
        public virtual ShippingType ShippingType { get; set; }
        public virtual DateTime DateNeeded { get; set; }
        public virtual bool AllowBackorder { get; set; }
        public virtual decimal EstimatedTax { get; set; }
        public virtual Workgroup Workgroup { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual string PoNumber { get; set; }
        public virtual Approval LastCompletedApproval { get; set; }
        public virtual decimal ShippingAmount { get; set; }
        public virtual string Justification { get; set; }
        public virtual OrderStatusCode StatusCode { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual DateTime DateCreated { get; set; }

        public virtual IList<LineItem> LineItems { get; set; }
        public virtual IList<Approval> Approvals { get; set; }
        public virtual IList<Split> Splits { get; set; }
        public virtual IList<OrderTracking> OrderTrackings { get; set; }
        public virtual IList<KfsDocument> KfsDocuments { get; set; }

        /// <summary>
        /// Total is sum of all line unit amts * quantities
        /// </summary>
        public virtual decimal Total()
        {
            return LineItems.Sum(amt => amt.Quantity*amt.UnitPrice);
        }

        public virtual string OrderRequestNumber()
        {
            return string.Format("{0}-{1}", string.Format("{0:mmddyy}", DateCreated), string.Format("{0:000000}", Id));
        }

        public virtual void AddLineItem(LineItem lineItem)
        {
            lineItem.Order = this;
            LineItems.Add(lineItem);
        }

        public virtual void AddApproval(Approval approval)
        {
            approval.Order = this;
            Approvals.Add(approval);
        }

        public virtual void AddSplit(Split split)
        {
            split.Order = this;
            Splits.Add(split);
        }

        public virtual void AddTracking(OrderTracking orderTracking)
        {
            orderTracking.Order = this;
            OrderTrackings.Add(orderTracking);
        }

        public virtual void AddKfsDocument(KfsDocument kfsDocument)
        {
            kfsDocument.Order = this;
            KfsDocuments.Add(kfsDocument);
        }
    }

    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Id(x => x.Id);

            Map(x => x.VendorId);
            Map(x => x.AddressId); //TODO: Replace these with actual lookups

            Map(x => x.DateNeeded);
            Map(x => x.AllowBackorder);
            Map(x => x.EstimatedTax);
            Map(x => x.PoNumber);
            Map(x => x.ShippingAmount);
            Map(x => x.Justification);

            References(x => x.OrderType);
            References(x => x.ShippingType);
            References(x => x.Workgroup);
            References(x => x.Organization);
            References(x => x.LastCompletedApproval).Column("LastCompletedApprovalId");
            References(x => x.StatusCode);
            References(x => x.CreatedBy).Column("CreatedBy");
            Map(x => x.DateCreated);

            HasMany(x => x.LineItems).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.Approvals).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse(); //TODO: check out this mapping when used with splits
            HasMany(x => x.Splits).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse(); //TODO: check out this mapping when used with splits
            HasMany(x => x.OrderTrackings).Table("OrderTracking").ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.KfsDocuments).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
        }
    }
}