using System.Collections.Generic;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Models
{
    public class OrderModifyModel
    {
        public OrderModifyModel()
        {
            ControlledSubstanceInformation = new ControlledSubstanceInformation();
        }

        public Order Order { get; set; }
        public OrderViewModel.SplitTypes SplitType { get; set; }
        public IList<LineItem> LineItems { get; set; }
        public IList<Split> Splits { get; set; }
        public ControlledSubstanceInformation ControlledSubstanceInformation { get; set; }
        public IList<UnitOfMeasure> Units { get; set; }
        public IList<Account> Accounts { get; set; }
        public IList<WorkgroupVendor> Vendors { get; set; }
        public IList<WorkgroupAddress> Addresses { get; set; }
        public IList<ShippingType> ShippingTypes { get; set; }
        public IList<ConditionalApproval> ConditionalApprovals { get; set; }
        public IList<CustomField> CustomFields { get; set; }
        public IList<User> Approvers { get; set; }
        public IList<User> AccountManagers { get; set; }
        public bool IsNewOrder { get { return Order.IsTransient(); } }

        public Workgroup Workgroup { get; set; }
    }
}