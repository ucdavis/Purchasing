using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Purchasing.Web.Models
{
    public class ReportPurchaserWorkLoadViewModel
    {
        public DateTime? ReportDate { get; set; }
        public List<ReportPurchaserWorkLoadItem> Items { get; set; }
    }

    public class ReportPurchaserWorkLoadItem
    {
        public string userId { get; set; }
        public string UserName { get; set; }
        public int CompletedCount { get; set; }
        public int PendingCount { get; set; }
    }
}