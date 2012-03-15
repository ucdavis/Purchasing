using System.Collections.Generic;
using Purchasing.Core.Domain;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class OrderReceiveModel
    {
        public int OrderId { get; set; }
        public IList<LineItem> LineItems { get; set; }
        public ReviewOrderViewModel ReviewOrderViewModel { get; set; }

        public static OrderReceiveModel Create(Order order)
        {
            Check.Require(order != null);
            var viewModel = new OrderReceiveModel
            {
                OrderId = order.Id,
                LineItems = order.LineItems
            };
            viewModel.ReviewOrderViewModel = new ReviewOrderViewModel();
            viewModel.ReviewOrderViewModel.Status = order.StatusCode.Name;
            viewModel.ReviewOrderViewModel.WorkgroupName = order.Workgroup.Name;
            viewModel.ReviewOrderViewModel.OrganizationName = order.Workgroup.PrimaryOrganization.Name;
            viewModel.ReviewOrderViewModel.Order = order;
            viewModel.ReviewOrderViewModel.Vendor = order.Vendor;
            viewModel.ReviewOrderViewModel.Address = order.Address;
            return viewModel;
        }
    }
}