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
            ReRouteAbleAccountManagerApprovals = new List<Approval>();
        }

        public Order Order { get; set; }

        public bool CanEditOrder { get; set; }
        public bool CanCancelOrder { get; set; }
        public bool CanCancelCompletedOrder { get; set; }
        public bool IsApprover { get; set; }
        public bool IsPurchaser { get; set; }
        public bool IsRequesterInWorkgroup { get; set; }
        public bool IsAccountManager { get; set; }
        
        public HashSet<string> UserRoles { get; set; }

        public bool HasAssociatedAccounts { get; set; }

        public bool CanEditFdpCompleted
        {
            get
            {
                if (Order.StatusCode.IsComplete && Order.OrderType.Id.Trim() == "PC")
                {
                    return true;
                }

                return false;
            }
        }

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
                    //Approvers in the approver role (not CA) which are forced to have an account selected can only submit if there are associated accounts
                    if (IsApprover && WorkgroupForceAccountApprover && UserRoles.Contains(Role.Codes.Approver))
                    {
                        return HasAssociatedAccounts;
                    }

                    //Account managers can only submit if there are associated accounts
                    if (IsAccountManager)
                    {
                        return HasAssociatedAccounts;
                    }

                    return true;
                }

                return false;
            }
        }

        public bool HasLineSplits
        {
            get { return Splits.Any(a => a.LineItem != null); }
        }

        public List<Approval> ExternalApprovals { get; set; }
        public List<Approval> ReRouteAbleAccountManagerApprovals { get; set; }

        public List<OrderType> OrderTypes { get; set; }

        public bool Complete { get; set; }

        public string Status { get; set; }

        public string OrganizationName { get; set; }

        public string WorkgroupName { get; set; }

        public bool WorkgroupForceAccountApprover { get; set; }

        public WorkgroupAddress Address { get; set; }

        public WorkgroupVendor Vendor { get; set; }

        public IEnumerable<LineItem> LineItems { get; set; }

        public IEnumerable<Split> Splits { get; set; }

        public ControlledSubstanceInformation ControllerSubstance { get; set; }

        public IEnumerable<CustomFieldAnswer> CustomFieldsAnswers { get; set; }

        public IEnumerable<Approval> Approvals { get; set; }

        public IEnumerable<OrderComment> Comments { get; set; }

        public IEnumerable<Attachment> Attachments { get; set; }

        public IEnumerable<OrderTracking> OrderTracking { get; set; }

        public IEnumerable<SubAccount> SubAccounts { get; set; }

        public IEnumerable<User> ApprovalUsers { get; set; }

        public IOrderedEnumerable<Approval> OrderedUniqueApprovals { get; set; }

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