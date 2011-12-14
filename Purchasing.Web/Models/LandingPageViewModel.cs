using System.Collections.Generic;
using Purchasing.Core.Domain;
using Purchasing.Web.Controllers;

namespace Purchasing.Web.Models
{
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
}