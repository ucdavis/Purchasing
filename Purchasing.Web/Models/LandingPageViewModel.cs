using System.Collections.Generic;
using Purchasing.Core.Queries;

namespace Purchasing.Web.Models
{
    public class LandingViewModel
    {
        public IEnumerable<OrderHistoryBase> PendingOrders { get; set; }
        public IEnumerable<OrderHistoryBase> YourOpenOrders { get; set; }
        public int CompletedOrdersThisMonth { get; set; }
        public int DeniedOrdersThisMonth { get; set; }
    }

    public class RequesterTotals
    {
        public string FullNameAndId { get; set; }
        public string Pending { get; set; }
        public string Completed { get; set; }
        public string EvenOdd { get; set; }
    }
}