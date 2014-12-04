using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Purchasing.Mvc.Models
{
    public class TotalByWorkgroupViewModel
    {
        public IEnumerable<OrderTotals> WorkgroupCounts { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Display(Name = "Show Administrative Workgroups")]
        public bool ShowAdmin { get; set; }

        public static TotalByWorkgroupViewModel Create(DateTime? startDate, DateTime? endDate, bool showAdmin)
        {
            var viewModel = new TotalByWorkgroupViewModel { StartDate = startDate, EndDate = endDate, ShowAdmin = showAdmin};
            viewModel.WorkgroupCounts = new List<OrderTotals>();

            return viewModel;
        }

    }

    public class OrderTotals
    {
        public string WorkgroupName { get; set; }
        public int WorkgroupId { get; set; }
        public bool Administrative { get; set; }
        public string PrimaryOrg { get; set; }
        public string Vendor { get; set; }
        public int InitiatedOrders { get; set; }
        public int DeniedOrders { get; set; }
        public int CanceledOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
    }
}