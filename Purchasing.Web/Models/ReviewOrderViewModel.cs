using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Models
{
    public class ReviewOrderViewModel
    {
        public ReviewOrderViewModel()
        {
            ExternalApprovals = new List<Approval>();
            SubAccounts = new List<SubAccount>();
        }

        public Order Order { get; set; }

        public bool CanEditOrder { get; set; }
        public bool CanCancelOrder { get; set; }
        public bool IsPurchaser { get; set; }
        public bool IsRequesterInWorkgroup { get; set; }
        public bool IsAccountManager { get; set; }
        public bool HasAssociatedAccounts { get; set; }

        public bool CanReceiveItems
        {
            get
            {
                if(!Order.StatusCode.IsComplete || Order.StatusCode.Id == OrderStatusCode.Codes.Cancelled || Order.StatusCode.Id == OrderStatusCode.Codes.Denied)
                {
                    return false;
                }

                return true;
            }
        }

        public bool CanSubmitOrder
        {
            get
            {
                if (CanEditOrder)
                {
                    return !IsAccountManager || HasAssociatedAccounts; //if you are an account manager you must have associated accounts
                }

                return false;
            }
        }

        public bool HasLineSplits
        {
            get { return Splits.Any(a => a.LineItem != null); }
        }

        public List<Approval> ExternalApprovals { get; set; }

        public List<OrderType> OrderTypes { get; set; }

        public bool Complete { get; set; }

        public string Status { get; set; }

        public string OrganizationName { get; set; }

        public string WorkgroupName { get; set; }

        public WorkgroupAddress Address { get; set; }

        public WorkgroupVendor Vendor { get; set; }

        public List<LineItem> LineItems { get; set; }

        public List<Split> Splits { get; set; }

        public ControlledSubstanceInformation ControllerSubstance { get; set; }

        public List<CustomFieldAnswer> CustomFieldsAnswers { get; set; }

        public List<Approval> Approvals { get; set; }

        public List<OrderComment> Comments { get; set; }

        public List<Attachment> Attachments { get; set; }

        public List<OrderTracking> OrderTracking { get; set; }

        public List<SubAccount> SubAccounts { get; set; }

        public string GetSubAccountDisplayForSplit(Split split)
        {
            if (string.IsNullOrWhiteSpace(split.SubAccount))
            {
                return string.Empty;
            }
            
            var subAccount =
                SubAccounts.FirstOrDefault(x => x.AccountNumber == split.Account && x.SubAccountNumber == split.SubAccount);

            return subAccount == null ? split.SubAccount : string.Format("{0} ({1})", subAccount.Name, subAccount.SubAccountNumber);
        }
    }
}