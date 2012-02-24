using System;
using System.Collections.Generic;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Controllers;

namespace Purchasing.Web.Models
{
    [Obsolete]
    public class LandingPageViewModel
    {
        public int YourOpenRequestCount { get; set; }
        public int YourNotActedOnCount { get; set; }
        public int OrdersPendingYourActionCount { get; set; }
        public ColumnPreferences ColumnPreferences { get; set; }
        //public List<RequestedHistory> RequestedHistories { get; set; }
        public FilteredOrderListModel OldestFOLM { get; set; }
        public FilteredOrderListModel NewestFOLM { get; set; }
        public FilteredOrderListModel LastFiveFOLM { get; set; }
        public List<RequesterTotals> RequesterTotals { get; set; }

    }

    public class LandingViewModel
    {
        public IEnumerable<OrderHistoryBase> PendingOrders { get; set; }
        public IEnumerable<OrderHistoryBase> YourOpenOrders { get; set; }
    }

    public class RequesterTotals
    {
        public string FullNameAndId { get; set; }
        public string Pending { get; set; }
        public string Completed { get; set; }
        public string EvenOdd { get; set; }
    }
}