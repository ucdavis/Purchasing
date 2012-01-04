using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using UCDArch.Core.DomainModel;
using DataAnnotationsExtensions;

namespace Purchasing.Core.Domain
{
    public class OrderArchive : DomainObject
    {
        [Required]
        public virtual OrderType OrderType { get; set; }
        
        public virtual string VendorName { get; set; }
        public virtual string VendorAddress { get; set; }
        public virtual string VendorPhone { get; set; }
        public virtual string VendorEmail { get; set; }

        public virtual string WorkgroupAddressName { get; set; }
        public virtual string WorkgroupAddressBuilding { get; set; }
        public virtual string WorkgroupAddressRoom { get; set; }
        public virtual string WorkgroupAddressAddress { get; set; }
        public virtual string WorkgroupAddressPhone { get; set; }

        public virtual ShippingType ShippingType { get; set; }
        [StringLength(50)]
        [Required]
        public virtual string DeliverTo { get; set; }
        [StringLength(50)]
        [Email]
        public virtual string DeliverToEmail { get; set; }
        public virtual DateTime? DateNeeded { get; set; }
        public virtual bool AllowBackorder { get; set; }
        public virtual decimal EstimatedTax { get; set; }

        public virtual string  WorkgroupName { get; set; }
        public virtual string WorkgroupPrimaryOrganizationId { get; set; }
        public virtual string WorkgroupPrimaryOrganizationName { get; set; }
       
        [StringLength(50)]
        public virtual string PoNumber { get; set; }
       
        public virtual decimal ShippingAmount { get; set; }
        public virtual decimal FreightAmount { get; set; }
        [Required]
        public virtual string Justification { get; set; }
        [Required]
        public virtual OrderStatusCode StatusCode { get; set; }
        [Required]
        public virtual string CreatedById { get; set; }

        public virtual  string CreatedByName { get; set; }
        public virtual string CreatedByEmail { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual bool HasControlledSubstance { get; set; }
        public virtual decimal Total { get; set; }
        public virtual string Approvals { get; set; }
        
        public virtual string OrderTrackings { get; set; }
        public virtual string CompletionReason { get; set; }

        public virtual IList<Attachment> Attachments { get; set; }
        public virtual IList<LineItem> LineItems { get; set; }
        
        public virtual IList<Split> Splits { get; set; }
        
        
        public virtual string OrderComments { get; set; }
        

        public virtual IList<CustomFieldAnswer> CustomFieldAnswers { get; set; }

    }

    public class OrderArchiveMap : ClassMap<OrderArchive>
    {
        public OrderArchiveMap()
        {
            Id(x => x.Id);

            Map(x => x.VendorName);
            Map(x => x.VendorAddress);
            Map(x => x.VendorPhone);
            Map(x => x.VendorEmail);

            Map(x => x.WorkgroupAddressName);
            Map(x => x.WorkgroupAddressBuilding);
            Map(x => x.WorkgroupAddressRoom);
            Map(x => x.WorkgroupAddressAddress);
            Map(x => x.WorkgroupAddressPhone);

            Map(x => x.WorkgroupName);
            Map(x => x.WorkgroupPrimaryOrganizationId);
            Map(x => x.WorkgroupPrimaryOrganizationName);
           

            Map(x => x.DateNeeded).Nullable();
            Map(x => x.AllowBackorder);
            Map(x => x.EstimatedTax);
            Map(x => x.PoNumber);
            Map(x => x.ShippingAmount);
            Map(x => x.FreightAmount);
            Map(x => x.DeliverTo);
            Map(x => x.DeliverToEmail);
            Map(x => x.Justification);
            Map(x => x.DateCreated);
            Map(x => x.HasControlledSubstance).Column("HasAuthorizationNum");
            Map(x => x.Total);

            References(x => x.OrderType);
            References(x => x.ShippingType);
            
            References(x => x.StatusCode).Column("OrderStatusCodeId");
            Map(x => x.CreatedById);
            Map(x => x.CreatedByName);
            Map(x => x.CreatedByEmail);

            Map(x => x.CompletionReason);
            Map(x => x.Approvals);
            Map(x => x.OrderTrackings);
            Map(x => x.OrderComments);

            HasMany(x => x.Attachments).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.LineItems).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
           
            HasMany(x => x.Splits).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse(); //TODO: check out this mapping when used with splits
            HasMany(x => x.CustomFieldAnswers).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();

            //Private mapping accessor
           // HasMany<ControlledSubstanceInformation>(FluentNHibernate.Reveal.Member<Order>("ControlledSubstances")).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
        }
    }
}
