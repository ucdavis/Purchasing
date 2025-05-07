using System;

namespace Purchasing.Mvc.Models
{
    public class FavoriteViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Category { get; set; }
        public string Notes { get; set; }

        public string RequestNumber { get; set; }

        public DateTime DateAdded { get; set; }

        public string Status { get; set; }
        public string WorkgroupName { get; set; }
        //Last action date?, Created Date?
    }
}
