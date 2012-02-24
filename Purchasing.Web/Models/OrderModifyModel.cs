using System.Collections.Generic;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Models
{
    public class OrderModifyModel
    {
        public Order Order { get; set; }
        public OrderViewModel.SplitTypes SplitType { get; set; }
        public IList<LineItem> LineItems { get; set; }
        public IList<Split> Splits { get; set; }
        public IEnumerable<UnitOfMeasure> Units { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<WorkgroupVendor> Vendors { get; set; }
        public IEnumerable<WorkgroupAddress> Addresses { get; set; }
        public IEnumerable<ShippingType> ShippingTypes { get; set; }
        public IEnumerable<ConditionalApproval> ConditionalApprovals { get; set; }
        public IEnumerable<CustomField> CustomFields { get; set; }
        public IEnumerable<User> Approvers { get; set; }
        public IEnumerable<User> AccountManagers { get; set; }
        /// <summary>
        /// New orders are either unsaved or first-action copies
        /// </summary>
        public bool IsNewOrder { get { return Order.IsTransient() || IsCopyOrder; } }
        
        public bool IsCopyOrder { get; set; }

        public Workgroup Workgroup { get; set; }
    }
}