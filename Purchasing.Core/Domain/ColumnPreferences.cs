using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ColumnPreferences : DomainObjectWithTypedId<string>
    {
        public ColumnPreferences()
        {
        }

        public ColumnPreferences(string id) : this() {Id = id;}
        [Display(Name = "Show Request Number")]
        public bool ShowRequestNumber { get; set; }
        [Display(Name = "Show PO #")]
        public bool ShowPurchaseOrderNumber { get; set; }
        [Display(Name = "Show Workgroup")]
        public bool ShowWorkgroup { get; set; }
        [Display(Name = "Show Organization")]
        public bool ShowOrganization { get; set; }
        [Display(Name = "Show Vendor")]
        public bool ShowVendor { get; set; }
        [Display(Name = "Show Ship To")]
        public bool ShowShipTo { get; set; }
        [Display(Name = "Show Allow Backorder")]
        public bool ShowAllowBackorder { get; set; }
        [Display(Name = "Show Restricted Orders")]
        public bool ShowRestrictedOrders { get; set; }
        [Display(Name = "Show Has Splits")]
        public bool ShowHasSplits { get; set; }
        [Display(Name = "Show Has Attachments")]
        public bool ShowHasAttachments { get; set; }
        [Display(Name = "Show # Of Lines")]
        public bool ShowNumberOfLines { get; set; }
        [Display(Name = "Show Total Ammount")]
        public bool ShowTotalAmount { get; set; }
        [Display(Name = "Show Created By")]
        public bool ShowCreatedBy { get; set; }
        [Display(Name = "Show Created Date")]
        public bool ShowCreatedDate { get; set; }
        [Display(Name = "Show Status")]
        public bool ShowStatus { get; set; }
        [Display(Name = "Show Needed Date")]
        public bool ShowNeededDate { get; set; }
        [Display(Name = "Show Shipping Type")]
        public bool ShowShippingType { get; set; }
        [Display(Name = "Show Days Not Acted On")]
        public bool ShowDaysNotActedOn { get; set; }
        [Display(Name = "Show Last Acted On By")]
        public bool ShowLastActedOnBy { get; set; }
        [Display(Name = "Show People Pending Action")]
        public bool ShowPeoplePendingAction { get; set; }
        [Display(Name = "Show Account #")]
        public bool ShowAccountNumber { get; set; }
        [Display(Name = "Show Date Ordered")]
        public bool ShowOrderedDate { get; set; }
        [Display(Name = "Show Approver")]
        public bool ShowApprover { get; set; }
        [Display(Name = "Show Account Manager")]
        public bool ShowAccountManager { get; set; }
        [Display(Name = "Show Purchaser")]
        public bool ShowPurchaser { get; set; }

    }

    public class ColumnPreferencesMap : ClassMap<ColumnPreferences>
    {
        public ColumnPreferencesMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.ShowRequestNumber);
            Map(x => x.ShowPurchaseOrderNumber);
            Map(x => x.ShowWorkgroup);
            Map(x => x.ShowOrganization);
            Map(x => x.ShowVendor);
            Map(x => x.ShowShipTo);
            Map(x => x.ShowAllowBackorder);
            Map(x => x.ShowRestrictedOrders);
            Map(x => x.ShowHasSplits);
            Map(x => x.ShowHasAttachments);
            Map(x => x.ShowNumberOfLines);
            Map(x => x.ShowTotalAmount);
            Map(x => x.ShowCreatedBy);
            Map(x => x.ShowCreatedDate);
            Map(x => x.ShowStatus);
            Map(x => x.ShowNeededDate);
            Map(x => x.ShowShippingType);
            Map(x => x.ShowDaysNotActedOn);
            Map(x => x.ShowLastActedOnBy);
            Map(x => x.ShowPeoplePendingAction);
            Map(x => x.ShowAccountNumber);
            Map(x => x.ShowOrderedDate);
            Map(x => x.ShowApprover);
            Map(x => x.ShowAccountManager);
            Map(x => x.ShowPurchaser);

        }
    }
}
