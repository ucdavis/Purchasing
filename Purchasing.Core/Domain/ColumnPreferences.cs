using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ColumnPreferences : DomainObjectWithTypedId<string>
    {
        public ColumnPreferences()
        {
            SetDefaults();
        }

        public virtual void SetDefaults()
        {
            ShowRequestNumber = true;
            ShowWorkgroup = true;
            ShowVendor = true;
            ShowCreatedBy = true;
            ShowCreatedDate = true;
            ShowLineItems = true;
            ShowTotalAmount = true;
            ShowStatus = true;
            ShowApUser = false;
            DisplayRows = 50;
        }

        public ColumnPreferences(string id) : this() {Id = id;}
        
        /* default columns*/
        [Display(Name = "Show Request Number")]
        public virtual bool ShowRequestNumber { get; set; }
        [Display(Name = "Show Workgroup")]
        public virtual bool ShowWorkgroup { get; set; }
        [Display(Name = "Show Vendor")]
        public virtual bool ShowVendor { get; set; }
        [Display(Name = "Show Created By")]
        public virtual bool ShowCreatedBy { get; set; }
        [Display(Name = "Show Created Date")]
        public virtual bool ShowCreatedDate { get; set; }
        [Display(Name="Show Line Items")]
        public virtual bool ShowLineItems { get; set; }
        [Display(Name = "Show Total Amount")]
        public virtual bool ShowTotalAmount { get; set; }
        [Display(Name = "Show Status")]
        public virtual bool ShowStatus { get; set; }
        [Display(Name = "Display Rows")]
        [Range(10,100)]
        public virtual int DisplayRows { get; set; }

        /* optional columns */
        [Display(Name = "Show Approver")]
        public virtual bool ShowApprover { get; set; }
        [Display(Name = "Show Account Manager")]
        public virtual bool ShowAccountManager { get; set; }
        [Display(Name = "Show Purchaser")]
        public virtual bool ShowPurchaser { get; set; }
        [Display(Name = "Show Account #")]
        public virtual bool ShowAccountNumber { get; set; }
        [Display(Name = "Show Ship To")]
        public virtual bool ShowShipTo { get; set; }
        [Display(Name = "Show Allow Backorder")]
        public virtual bool ShowAllowBackorder { get; set; }
        [Display(Name = "Show Restricted Orders")]
        public virtual bool ShowRestrictedOrder { get; set; }
        [Display(Name = "Show Needed Date")]
        public virtual bool ShowNeededDate { get; set; }
        [Display(Name = "Show Shipping Type")]
        public virtual bool ShowShippingType { get; set; }

        [Display(Name = "Show PO#")]
        public virtual bool ShowPurchaseOrderNumber { get; set; }
        [Display(Name = "Show Tag")]
        public virtual bool ShowTag { get; set; }
        [Display(Name = "Show Ref#")]
        public virtual bool ShowReferenceNumber { get; set; }

        [Display(Name = "Show Request Type")]
        public virtual bool ShowRequestType { get; set; }

        [Display(Name = "Show Last Acted On Date")]
        public virtual bool ShowLastActedOnDate { get; set; }
        [Display(Name = "Show Days Not Acted On")]
        public virtual bool ShowDaysNotActedOn { get; set; }
        [Display(Name = "Show Last Acted On By")]
        public virtual bool ShowLastActedOnBy { get; set; }
        [Display(Name = "Show Order Received")]
        public virtual bool ShowOrderReceived { get; set; }

        [Display(Name = "Show Order Paid")]
        public virtual bool ShowOrderPaid { get; set; }

        [Display(Name = "Show Order Type")]
        public virtual bool ShowOrderType { get; set; }

       
        [Display(Name = "Show Has Splits")]
        public virtual bool ShowHasSplits { get; set; }
       
        [Display(Name = "Show Shipping Cost")]
        public virtual bool ShowShippingCost { get; set; }
        [Display(Name = "Show FPD Completed")]
        public virtual bool ShowFpdCompleted { get; set; }

        [Display(Name = "Show Accounts Payable User")]
        public virtual bool ShowApUser { get; set; }
        //PostStatus in Order model
        [Display(Name = "Show Order Substatus")]
        public virtual bool ShowSubStatus { get; set; }
        [Display(Name = "Show Order Note")]
        public virtual bool ShowOrderNote  { get; set; }

    }

    public class ColumnPreferencesMap : ClassMap<ColumnPreferences>
    {
        public ColumnPreferencesMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.ShowRequestNumber);
            Map(x => x.ShowRequestType);
            Map(x => x.ShowWorkgroup);
            Map(x => x.ShowVendor);
            Map(x => x.ShowCreatedBy);
            Map(x => x.ShowCreatedDate);
            Map(x => x.ShowLineItems);
            Map(x => x.ShowTotalAmount);
            Map(x => x.ShowStatus);
            Map(x => x.ShowPurchaseOrderNumber);
            Map(x => x.ShowTag);
            Map(x => x.ShowReferenceNumber);
            Map(x => x.ShowShipTo);
            Map(x => x.ShowAllowBackorder);
            Map(x => x.ShowNeededDate);
            Map(x => x.ShowShippingType);
            Map(x => x.ShowDaysNotActedOn);
            Map(x => x.ShowLastActedOnBy);
            Map(x => x.ShowAccountNumber);
            Map(x => x.ShowApprover);
            Map(x => x.ShowAccountManager);
            Map(x => x.ShowPurchaser);
            Map(x => x.ShowLastActedOnDate);
            Map(x => x.ShowRestrictedOrder);
            Map(x => x.ShowOrderReceived);
            Map(x => x.ShowOrderPaid);
            Map(x => x.ShowOrderType);
            Map(x => x.DisplayRows);
            Map(x => x.ShowHasSplits);
            Map(x => x.ShowShippingCost);
            Map(x => x.ShowFpdCompleted);
            Map(x => x.ShowApUser);

            Map(x => x.ShowSubStatus);
            Map(x => x.ShowOrderNote);
        }
    }
}
