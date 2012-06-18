using System;

namespace Purchasing.Web.Models
{
    public class OrderViewModel
    {
        public SplitTypes SplitType { get; set; }
        public string Justification { get; set; }
        public int Vendor { get; set; }
        public string ShipTo { get; set; }
        public string ShipEmail { get; set; }
        public string ShipPhone { get; set; }
        public int ShipAddress { get; set; }

        public string Shipping { get; set; }
        public string Freight { get; set; }
        public string Tax { get; set; }

        public LineItem[] Items { get; set; }
        public Split[] Splits { get; set; }
        public CustomField[] CustomFields { get; set; }

        public string Account { get; set; }
        public string SubAccount { get; set; }
        public string Project { get; set; }
        public string Approvers { get; set; }
        public string AccountManagers { get; set; }
        public int[] ConditionalApprovals { get; set; }

        public int Workgroup { get; set; }
        public bool? AdjustRouting { get; set; }

        public ControlledSubstance Restricted { get; set; }

        public string Backorder { get; set; }
        public bool AllowBackorder { get { return Backorder == "true"; } }

        public Guid[] FileIds { get; set; }
        public Guid FormSaveId { get; set; }

        public DateTime DateNeeded { get; set; }
        public string ShippingType { get; set; }
        public string Comments { get; set; }
        
        public class Split
        {
            public int? LineItemId { get; set; }
            public string Account { get; set; }
            public string AccountName { get; set; }
            public string SubAccount { get; set; }
            public string Project { get; set; }
            public string Amount { get; set; }
            public string Percent { get; set; }

            /// <summary>
            /// Split is valid if it has an account and amount
            /// TODO: is that true?
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                return !string.IsNullOrWhiteSpace(Account) && !string.IsNullOrWhiteSpace(Amount);
            }
        }

        public class LineItem
        {
            public int Id { get; set; }
            public string Quantity { get; set; }
            public string Price { get; set; }
            public string Units { get; set; }
            public string CatalogNumber { get; set; }
            public string Description { get; set; }
            public string CommodityCode { get; set; }
            public string Url { get; set; }
            public string Notes { get; set; }

            /// <summary>
            /// A line item is valid if it has a price and quantity
            /// TODO: is that true?
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                return !string.IsNullOrWhiteSpace(Quantity) && !string.IsNullOrWhiteSpace(Price);
            }
        }

        public class ControlledSubstance
        {
            public bool IsRestricted { get { return Status == "True"; } }
            public string Status { get; set; }
            public string Class { get; set; }
            public string Use { get; set; }
            public string StorageSite { get; set; }
            public string Custodian { get; set; }
            public string Users { get; set; }
        }

        public class CustomField
        {
            public int Id { get; set; }
            //public string Question { get; set; } //TODO: Do we need question and required?
            public string Answer { get; set; }
            //public bool Required { get; set; }
        }

        public enum SplitTypes
        {
            None,
            Order,
            Line
        }
    }
}