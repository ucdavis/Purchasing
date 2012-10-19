using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using UCDArch.Core.PersistanceSupport;
using System.Text;

namespace Purchasing.Web.Models
{
    public class FilteredOrderListModelDto
    {
        public List<OrderHistory> OrderHistory { get; set; }
        
        // for building dropdown list
        public List<Tuple<string, string>> OrderStatusCodes { get; set; }
        
        public string SelectedOrderStatus { get; set; }
        public bool ShowPending { get; set; }
        public bool ShowCreated { get; set; }
        [Display(Name = "Created After")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "Created Before")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Last Action After")]
        public DateTime? StartLastActionDate { get; set; }
        [Display(Name = "Last Action Before")]
        public DateTime? EndLastActionDate { get; set; }
        public ColumnPreferences ColumnPreferences { get; set; }
        public string ShowLast { get; set; }

        public void PopulateStatusCodes(IRepositoryWithTypedId<OrderStatusCode, string> statusCodeRepository, List<Tuple<string, string>> orderStatusCodes = null)
        {
            if (orderStatusCodes == null)
            {
                OrderStatusCodes = new List<Tuple<string, string>> { new Tuple<string, string>("All", "All"), new Tuple<string, string>("Received", "Received"), new Tuple<string, string>("UnReceived", "UnReceived") };
                OrderStatusCodes.AddRange(statusCodeRepository.Queryable
                    .Where(a => a.ShowInFilterList)
                    .OrderBy(a => a.Level)
                    .Select(a => new Tuple<string, string>(a.Id, a.Name))
                    .ToList());
            }
            else
            {
                OrderStatusCodes = orderStatusCodes;
            }
        }

        public List<Split> Splits { get; set; }
    }
}