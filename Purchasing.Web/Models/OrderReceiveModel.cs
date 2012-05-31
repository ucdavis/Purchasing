using System.Collections.Generic;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using System.Linq.Expressions;
using System.Linq;

namespace Purchasing.Web.Models
{
    public class OrderReceiveModel
    {
        public int OrderId { get; set; }
        public IList<LineItem> LineItems { get; set; }
        public ReviewOrderViewModel ReviewOrderViewModel { get; set; }
        public Dictionary<int, HistoryReceivedLineItem> LastChangedBy { get; set; }
        public IList<OrderPeep> PurchasorPeeps { get; set; } 

        public static OrderReceiveModel Create(Order order, IRepository<HistoryReceivedLineItem> historyReceivedLineItemRepository)
        {
            Check.Require(order != null);
            var viewModel = new OrderReceiveModel
            {
                OrderId = order.Id,
                LineItems = order.LineItems,
                LastChangedBy = new Dictionary<int, HistoryReceivedLineItem>()
            };
            viewModel.ReviewOrderViewModel = new ReviewOrderViewModel();
            viewModel.ReviewOrderViewModel.Status = order.StatusCode.Name;
            viewModel.ReviewOrderViewModel.WorkgroupName = order.Workgroup.Name;
            viewModel.ReviewOrderViewModel.OrganizationName = order.Workgroup.PrimaryOrganization.Name;
            viewModel.ReviewOrderViewModel.Order = order;
            viewModel.ReviewOrderViewModel.Vendor = order.Vendor;
            viewModel.ReviewOrderViewModel.Address = order.Address;
            foreach (var lineItem in viewModel.LineItems)
            {
                var lastChangedBy =
                    historyReceivedLineItemRepository.Queryable.Where(a => a.LineItem.Id == lineItem.Id).OrderByDescending(a => a.UpdateDate).FirstOrDefault();
                viewModel.LastChangedBy.Add(lineItem.Id,lastChangedBy);

            }
            //var t = viewModel.LastChangedBy.FirstOrDefault(a => a.Key == 1).Value;
            return viewModel;
        }
    }
}