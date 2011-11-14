using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Order : DomainObject
    {
        public Order()
        {
            Attachments = new List<Attachment>();
            LineItems = new List<LineItem>();
            Approvals = new List<Approval>();
            Splits = new List<Split>();
            OrderTrackings = new List<OrderTracking>();
            KfsDocuments = new List<KfsDocument>();
            OrderComments = new List<OrderComment>();
            ControlledSubstances = new List<ControlledSubstanceInformation>();
            EmailQueues = new List<EmailQueue>();

            DateCreated = DateTime.Now;
            HasControlledSubstance = false;

            EstimatedTax = 7.75m; //Default 7.75% UCD estimated tax
        }

        public virtual OrderType OrderType { get; set; }
        //public virtual int VendorId { get; set; }//TODO: Replace with actual vendor
        //public virtual int AddressId { get; set; }//TODO: Replace

        public virtual WorkgroupVendor Vendor { get; set; }
        public virtual WorkgroupAddress Address { get; set; }

        public virtual ShippingType ShippingType { get; set; }
        public virtual string DeliverTo { get; set; }
        public virtual string DeliverToEmail { get; set; }
        public virtual DateTime? DateNeeded { get; set; }
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
        public virtual bool HasControlledSubstance { get; set; }
       
        public virtual IList<Attachment> Attachments { get; set; }
        public virtual IList<LineItem> LineItems { get; set; }
        public virtual IList<Approval> Approvals { get; set; }
        public virtual IList<Split> Splits { get; set; }
        public virtual IList<OrderTracking> OrderTrackings { get; set; }
        public virtual IList<KfsDocument> KfsDocuments { get; set; }
        public virtual IList<OrderComment> OrderComments { get; set; }
        public virtual IList<EmailQueue> EmailQueues { get; set; } 

        private IList<ControlledSubstanceInformation> ControlledSubstances { get; set; }

        /// <summary>
        /// Sets the authorization number for this order, replacing existing info if it exists
        /// </summary>
        public virtual void SetAuthorizationInfo(ControlledSubstanceInformation controlledSubstanceInformation)
        {
            controlledSubstanceInformation.Order = this;
            
            ControlledSubstances.Clear();
            ControlledSubstances.Add(controlledSubstanceInformation);

            HasControlledSubstance = true;
        }

        public virtual void ClearAuthorizationInfo()
        {
            HasControlledSubstance = false;
            ControlledSubstances.Clear();
        }

        public virtual ControlledSubstanceInformation GetAuthorizationInfo()
        {
            return ControlledSubstances.FirstOrDefault();
        }

        /// <summary>
        /// Total is sum of all line unit amts * quantities
        /// </summary>
        public virtual decimal Total()
        {
            return LineItems.Sum(amt => amt.Quantity*amt.UnitPrice);
        }

        /// <summary>
        /// Grand total is total + tax/shipping/freight
        /// </summary>
        /// <returns></returns>
        public virtual decimal GrandTotal()
        {
            //TODO: add calculation for tax/shipping/freight
            return Total();
        }

        /// <summary>
        /// Order Request Number (unique identifier for the specific order)
        /// </summary>
        /// <returns></returns>
        public virtual string OrderRequestNumber()
        {
            //TODO: What happens when 1 million orders are done system-wide (format)?
            return string.Format("{0}-{1}", string.Format("{0:yyMMdd}", DateCreated), string.Format("{0:000000}", Id));
        }

        /// <summary>
        /// Check if the order is split by line items
        /// </summary>
        public virtual bool HasLineSplits
        {
            get { return Splits.Any(a => a.LineItem != null); }
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

        public virtual void AddOrderComment(OrderComment orderComment)
        {
            orderComment.Order = this;
            OrderComments.Add(orderComment);
        }

        public virtual void AddAttachment(Attachment attachment)
        {
            attachment.Order = this;
            Attachments.Add(attachment);
        }

        public virtual void AddEmailQueue(EmailQueue emailQueue)
        {
            emailQueue.Order = this;
            EmailQueues.Add(emailQueue);
        }

        /// <summary>
        /// Note, this is not a queryable field.
        /// </summary>
        public virtual int DaysNotActedOn
        {
            get
            {
                if(OrderTrackings.Where(a => a.StatusCode.IsComplete).Any())
                {
                    return 0;
                }
                var lastDate = OrderTrackings.Max(a => a.DateCreated).Date;
                var timeSpan = DateTime.Now.Date - lastDate;
                return timeSpan.Days;

            }
        }

        /// <summary>
        /// Note, this is not a queryable field.
        /// </summary>
        public virtual string PeoplePendingAction
        {
            get
            {
                return string.Join(", ", Approvals.Where(a => !a.Completed  && a.User != null).OrderBy(b => b.StatusCode.Level).Select(x => x.User.FullName).ToList());
            }
        }

        /// <summary>
        /// Note, this is not a queryable field.
        /// </summary>
        public virtual string AccountNumbers
        {
            get
            {
                return string.Join(", ", Splits.Where(a => a.Account != null).Select(x => x.Account).Distinct().ToList());
            }
        }

        /// <summary>
        /// Note, this is not a queryable field.
        /// </summary>
        public virtual string ApproverName
        {
            get { return GetNameFromApprovals(OrderStatusCode.Codes.Approver); }
        }

        /// <summary>
        /// Note, this is not a queryable field.
        /// </summary>
        public virtual string AccountManagerName
        {
            get { return GetNameFromApprovals(OrderStatusCode.Codes.AccountManager); }
        }

        /// <summary>
        /// Note, this is not a queryable field.
        /// </summary>
        public virtual string PurchaserName
        {
            get { return GetNameFromApprovals(OrderStatusCode.Codes.Purchaser); }
        }

        public virtual string GetNameFromApprovals(string orderStatusCodeId)
        {
            var apprv = Approvals.Where(a => a.StatusCode.Id == orderStatusCodeId && a.User != null).FirstOrDefault();
            if(apprv == null)
            {
                return string.Empty;
            }
            if(apprv.User.IsActive && !apprv.User.IsAway) //User is not away show them
            {
                return apprv.User.FullName;
            }
            if(apprv.SecondaryUser != null && apprv.SecondaryUser.IsActive && !apprv.SecondaryUser.IsAway) //Primary user is away, show Secondary if active and not away
            {
                return apprv.SecondaryUser.FullName;
            }
            return string.Empty;
        }


        public static class Expressions
        {
            public static readonly Expression<Func<Order, object>> AuthorizationNumbers = x => x.ControlledSubstances;
        }
    }

    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Id(x => x.Id);

            //Map(x => x.VendorId);
            //Map(x => x.AddressId); //TODO: Replace these with actual lookups

            References(x => x.Vendor).Column("WorkgroupVendorId");
            References(x => x.Address).Column("WorkgroupAddressId");

            Map(x => x.DateNeeded).Nullable();
            Map(x => x.AllowBackorder);
            Map(x => x.EstimatedTax);
            Map(x => x.PoNumber);
            Map(x => x.ShippingAmount);
            Map(x => x.DeliverTo);
            Map(x => x.DeliverToEmail);
            Map(x => x.Justification);
            Map(x => x.DateCreated);
            Map(x => x.HasControlledSubstance).Column("HasAuthorizationNum");

            References(x => x.OrderType);
            References(x => x.ShippingType);
            References(x => x.Workgroup);
            References(x => x.Organization);
            References(x => x.LastCompletedApproval).Column("LastCompletedApprovalId");
            References(x => x.StatusCode).Column("OrderStatusCodeId");
            References(x => x.CreatedBy).Column("CreatedBy");

            HasMany(x => x.Attachments).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.LineItems).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.Approvals).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse(); //TODO: check out this mapping when used with splits
            HasMany(x => x.Splits).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse(); //TODO: check out this mapping when used with splits
            HasMany(x => x.OrderTrackings).Table("OrderTracking").ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.KfsDocuments).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.OrderComments).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.EmailQueues).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();

            //Private mapping accessor
            HasMany<ControlledSubstanceInformation>(FluentNHibernate.Reveal.Member<Order>("ControlledSubstances")).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
        }
    }
}