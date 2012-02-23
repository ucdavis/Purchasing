using System.Collections.Generic;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Models
{
    public class ReviewOrderViewModel
    {
        public ReviewOrderViewModel()
        {
            ExternalApprovals = new List<Approval>();
        }

        public Order Order { get; set; }

        public bool CanEditOrder { get; set; }
        public bool CanCancelOrder { get; set; }
        public bool IsPurchaser { get; set; }
        public bool IsRequesterInWorkgroup { get; set; }
        public bool IsAccountManager { get; set; }
        public bool HasAssociatedAccounts { get; set; }

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

        public List<Approval> ExternalApprovals { get; set; }

        public List<OrderType> OrderTypes { get; set; }

        public bool Complete { get; set; }
    }
}