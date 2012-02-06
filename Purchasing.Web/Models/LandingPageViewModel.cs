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
        public int PendingYourAction { get; set; }
        public int UrgentOrderCount { get; set; }
        public int FinishedThisWeekCount { get; set; }
        public int FinishedThisMonthCount { get; set; }
        public IList<Order> UrgentOrders { get; set; }
        public string RecentActivityTime { get; set; }
        public Order RecentActivityOrder { get; set; }
        public IList<CommentHistory> RecientComments { get; set; } 
    }
}