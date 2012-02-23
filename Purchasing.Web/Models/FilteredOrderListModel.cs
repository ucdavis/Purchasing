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
        // for building dropdown list
        public List<Tuple<string,string>> OrderStatusCodes { get; set; }
        public IList<Order> Orders { get; set; }
        //public List<string> CheckedOrderStatusCodes { get; set; }
        public string SelectedOrderStatus { get; set; }
        //public bool ShowAll { get; set; }
        //public bool ShowCompleted { get; set; }
        public bool ShowPending { get; set; }
        [Display(Name = "Created After")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "Created Before")]
        public DateTime? EndDate { get; set; }
        //public bool ShowOwned { get; set; }
        public ColumnPreferences ColumnPreferences { get; set; }
        //public bool HideOrdersYouCreated { get; set; }
        public string ShowLast { get; set; }

        public static FilteredOrderListModel Create(IRepository repository, IList<Order> orders, List<Tuple<string,string>> orderStatusCodes = null)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new FilteredOrderListModel
            {
                Orders = orders
            };
            if(orderStatusCodes == null)
            {
                viewModel.OrderStatusCodes = new List<Tuple<string, string>>();
                viewModel.OrderStatusCodes.Add(new Tuple<string, string>("All", "All"));
                viewModel.OrderStatusCodes.AddRange(repository.OfType<OrderStatusCode>().Queryable
                    .Where(a => a.ShowInFilterList)
                    .OrderBy(a => a.Level)
                    .Select(a => new Tuple<string, string>(a.Id, a.Name))
                    .ToList());
            }
            else
            {
                viewModel.OrderStatusCodes = orderStatusCodes;
            }
            //viewModel.CheckedOrderStatusCodes = new List<string>();

            return viewModel;
        }
    }
}