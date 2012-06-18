using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using Purchasing.Core.Helpers;
using UCDArch.Core.DomainModel;
using DataAnnotationsExtensions;
using UCDArch.Core.Utils;

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
            CustomFieldAnswers = new List<CustomFieldAnswer>();

            DateCreated = DateTime.Now;
            HasControlledSubstance = false;

            EstimatedTax = 7.25m; //Default 7.25% UCD estimated tax
        }

        public virtual string RequestNumber { get; protected set; }
        [Required]
        public virtual OrderType OrderType { get; set; }
        [StringLength(3)]
        public virtual string KfsDocType { get; set; }
        
        //public virtual int VendorId { get; set; }//TODO: Replace with actual vendor
        //public virtual int AddressId { get; set; }//TODO: Replace

        public virtual WorkgroupVendor Vendor { get; set; }
        [Required]
        public virtual WorkgroupAddress Address { get; set; }

        public virtual ShippingType ShippingType { get; set; }
        [StringLength(50)]
        [Required]
        public virtual string DeliverTo { get; set; }
        [StringLength(50)]
        [Email]
        public virtual string DeliverToEmail { get; set; }
        [StringLength(15)]
        public virtual string DeliverToPhone { get; set; }
        [Required]
        [Date]
        public virtual DateTime DateNeeded { get; set; }
        public virtual bool AllowBackorder { get; set; }
        public virtual decimal EstimatedTax { get; set; }
        [Required]
        public virtual Workgroup Workgroup { get; set; }
        [Required]
        public virtual Organization Organization { get; set; }
        [StringLength(50)]
        public virtual string ReferenceNumber { get; set; }
        public virtual Approval LastCompletedApproval { get; set; }
        public virtual decimal ShippingAmount { get; set; }
        public virtual decimal FreightAmount { get; set; }
        [Required]
        public virtual string Justification { get; set; }
        [Required]
        public virtual OrderStatusCode StatusCode { get; set; }
        [Required]
        public virtual User CreatedBy { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual bool HasControlledSubstance { get; set; }

        public virtual string CompletionReason { get; set; }

        public virtual IList<Attachment> Attachments { get; set; }
        public virtual IList<LineItem> LineItems { get; set; }
        public virtual IList<Approval> Approvals { get; set; }
        public virtual IList<Split> Splits { get; set; }
        public virtual IList<OrderTracking> OrderTrackings { get; set; }
        public virtual IList<KfsDocument> KfsDocuments { get; set; }
        public virtual IList<OrderComment> OrderComments { get; set; }
        public virtual IList<EmailQueue> EmailQueues { get; set; }

        public virtual IList<CustomFieldAnswer> CustomFieldAnswers { get; set; }


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

        public virtual decimal TotalFromDb { get; set; }

        public virtual decimal TotalWithTax()
        {
            return LineItems.Sum(amt => amt.TotalWithTax());
        }

        /// <summary>
        /// Grand total is total + tax/shipping/freight
        /// </summary>
        /// <returns></returns>
        public virtual decimal GrandTotalFromDb
        {
            get { return (TotalFromDb + FreightAmount)*(1 + EstimatedTax/100.0m) + ShippingAmount; }
        }

        /// <summary>
        /// Grand total is total + tax/shipping/freight
        /// </summary>
        /// <returns></returns>
        public virtual decimal GrandTotal()
        {
            return (Total() + FreightAmount) * (1 + EstimatedTax / 100.0m) + ShippingAmount;
        }

        /// <summary>
        /// Order Request Number (unique identifier for the specific order)
        /// </summary>
        /// <returns></returns>
        public virtual string OrderRequestNumber()
        {
            return RequestNumber;
        }

        /// <summary>
        /// Summarizes first few line items, with elipse in more items.
        /// </summary>
        /// <returns></returns>
        public virtual string LineItemSummary()
        {
            var lineSummary = new StringBuilder();
            int linecount = 0;
            foreach (var lineItem in LineItems)
            {
                if(linecount++==2)
                {
                    break;
                }
                if(linecount==1)
                {
                    lineSummary.Append(string.Format("{0} [{1}] {2}", lineItem.Quantity, lineItem.Unit,
                        lineItem.Description.Length > 100 ? lineItem.Description.Remove(100) : lineItem.Description));
                } else
                {
                    lineSummary.Append(string.Format(", {0} [{1}] {2}", lineItem.Quantity, lineItem.Unit,
                                                    lineItem.Description.Length > 100 ? lineItem.Description.Remove(100) : lineItem.Description));
                }

            }
            if (LineItems.Count > 2)
            {
                lineSummary.Append(", ...");
            }
            return lineSummary.ToString();
        }

        public virtual bool OrderReceived
        {
            get { return LineItems != null && LineItems.Count > 0 && LineItems.All(a => a.Received); }
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

        public virtual void AddCustomAnswer(CustomFieldAnswer customFieldAnswer)
        {
            customFieldAnswer.Order = this;
            CustomFieldAnswers.Add(customFieldAnswer);
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

        public virtual void AddComment(OrderComment orderComment)
        {
            orderComment.Order = this;
            OrderComments.Add(orderComment);
        }

        /// <summary>
        /// Note, this is not a queryable field.
        /// </summary>
        public virtual int DaysNotActedOn
        {
            get
            {
                if(OrderTrackings.Any(a => a.StatusCode.IsComplete))
                {
                    return 0;
                }
                var lastDate = OrderTrackings.Max(a => a.DateCreated).Date;
                var timeSpan = DateTime.Now.Date - lastDate;
                return timeSpan.Days;

            }
        }

        public virtual TimeSpan TimeUntilDue()
        {
            return DateNeeded - DateTime.Now;
        }

        /// <summary>
        /// Note, this is not a queryable field.
        /// </summary>
        public virtual string PeoplePendingAction
        {
            get
            {
                return string.Join(", ", Approvals.Where(a => !a.Completed && a.User != null).OrderBy(b => b.StatusCode.Level).Select(x => string.Format("{0} ({1})", x.User.FullName, x.StatusCode.Id)).ToList());
            }
        }

        public virtual string VendorName
        {
            get { return Vendor == null ? "-- Unspecified --" : Vendor.DisplayName; }
        }

        /// <summary>
        /// Note, this is not a queryable field.
        /// </summary>
        public virtual DateTime? DateOrdered
        {
            get
            {
                var orderTracking = OrderTrackings.FirstOrDefault(a => a.StatusCode.Id == OrderStatusCode.Codes.Purchaser);
                if(orderTracking == null)
                {
                    return null;
                }
                return orderTracking.DateCreated;
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
                return "[Workgroup]";
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

        /// <summary>
        /// Validates if an order's expenses are valid, meaning all costs are accounted for
        /// including order/line splits
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> ValidateExpenses()
        {
            //Always make sure there are >0 line items
            if (LineItems.Count == 0) yield return "There must be at least one line item";

            if (Splits.Count == 0) //validate no splits
            {
                yield break; //TODO: For now, no splits is always valid.  Later we can validate accounts are given eventually
            }

            if (HasLineSplits) //validate line splits
            {
                //if line items are split, make sure #1 all money is accounted for, #2 every line item has at least one split
                foreach (var lineItem in LineItems)
                {
                    var item = lineItem;
                    var splits = Splits.Where(x => x.LineItem == item).ToList();
                    
                    if (!splits.Any())
                        yield return "You have a line item split with an amount but no account specified";

                    var splitTotal = splits.Sum(x => x.Amount);
                    var lineTotal = lineItem.Total()*(1 + EstimatedTax/100.0m);
                    
                    if (lineTotal != splitTotal)
                        yield return "You must account for the entire line item amount in each line item split";
                }
            }
            else //Validate order splits
            {
                //If order is split, make sure all order money is accounted for
                var splitTotal = Splits.Sum(x => x.Amount);
                var orderTotal = GrandTotal();

                //Make sure each split with an amount has an account chosen
                if (Splits.Any(x => x.Amount != 0 && string.IsNullOrWhiteSpace(x.Account))) yield return "You have an order split with an amount but no account specified";

                if (splitTotal != orderTotal) yield return "You must account for the entire order amount";
            }
        }

        public static class Expressions
        {
            public static readonly Expression<Func<Order, object>> AuthorizationNumbers = x => x.ControlledSubstances;
        }

        public virtual void GenerateRequestNumber()
        {
            Check.Require(this.CreatedBy != null, "Created by is required.");

            var dateHash = DateCreated.Ticks.GetHashCode();
            var userHash = CreatedBy.Id.GetHashCode();
            var indicator = "A";

            if (dateHash < 0)
            {
                indicator = "B";
                dateHash = Math.Abs(dateHash);
            }

            if (userHash < 0)
            {
                indicator = indicator == "A" ? "C" : "D";
                userHash = Math.Abs(userHash);
            }

            var encodedId = (dateHash * 31) ^ userHash;

            if (encodedId < 0)
            {
                indicator = (indicator == "A" || indicator == "B") ? "E" : "F";
                encodedId = Math.Abs(encodedId);
            }

            RequestNumber = string.Format("{0}-{1}{2}", Organization.Id.Substring(Organization.Id.IndexOf('-') + 1), indicator, encodedId.ConvertToBase36());
        }
    }

    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Id(x => x.Id);

            References(x => x.Vendor).Column("WorkgroupVendorId");
            References(x => x.Address).Column("WorkgroupAddressId");

            Map(x => x.RequestNumber);
            Map(x => x.DateNeeded).Nullable();
            Map(x => x.AllowBackorder);
            Map(x => x.EstimatedTax);
            Map(x => x.ReferenceNumber);
            Map(x => x.ShippingAmount);
            Map(x => x.FreightAmount);
            Map(x => x.DeliverTo);
            Map(x => x.DeliverToEmail);
            Map(x => x.DeliverToPhone);
            Map(x => x.Justification).Length(int.MaxValue);
            Map(x => x.DateCreated);
            Map(x => x.HasControlledSubstance).Column("HasAuthorizationNum");
            Map(x => x.TotalFromDb).Column("Total");
            Map(x => x.KfsDocType);

            References(x => x.OrderType);
            References(x => x.ShippingType);
            References(x => x.Workgroup);
            References(x => x.Organization);
            References(x => x.LastCompletedApproval).Column("LastCompletedApprovalId");
            References(x => x.StatusCode).Column("OrderStatusCodeId");
            References(x => x.CreatedBy).Column("CreatedBy");

            Map(x => x.CompletionReason).Length(int.MaxValue);

            HasMany(x => x.Attachments).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.LineItems).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.Approvals).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.Splits).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.OrderTrackings).Table("OrderTracking").ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.KfsDocuments).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.OrderComments).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.EmailQueues).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.CustomFieldAnswers).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();

            //Private mapping accessor
            HasMany<ControlledSubstanceInformation>(FluentNHibernate.Reveal.Member<Order>("ControlledSubstances")).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
        }
    }
}