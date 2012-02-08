using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class FilteredOrderListModel
    {
        public List<OrderStatusCode> OrderStatusCodes { get; set; }
        public IList<Order> Orders { get; set; }
        public List<string> CheckedOrderStatusCodes { get; set; }
        public bool ShowAll { get; set; }
        public bool ShowCompleted { get; set; }
        [Display(Name = "Created After")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "Created Before")]
        public DateTime? EndDate { get; set; }
        public bool ShowOwned { get; set; }
        public ColumnPreferences ColumnPreferences { get; set; }
        public bool HideOrdersYouCreated { get; set; }
        public string ShowLast { get; set; }

        public static FilteredOrderListModel Create(IRepository repository, IList<Order> orders, List<OrderStatusCode> orderStatusCodes = null)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new FilteredOrderListModel
            {
                Orders = orders
            };
            if(orderStatusCodes == null)
            {
                viewModel.OrderStatusCodes =
                    repository.OfType<OrderStatusCode>().Queryable.Where(a => a.ShowInFilterList).OrderBy(a => a.Level).
                        ToList();
            }
            else
            {
                viewModel.OrderStatusCodes = orderStatusCodes;
            }
            viewModel.CheckedOrderStatusCodes = new List<string>();

            return viewModel;
        }
    }
}