using System.Collections.Generic;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Models
{
    public class ReadOnlyOrderViewModel
    {
        public ReadOnlyOrderViewModel()
        {
            ExternalApprovals = new List<Approval>();
        }

        public Order Order { get; set; }

        public bool CanEditOrder { get; set; }

        public List<Approval> ExternalApprovals { get; set; }
    }
}