using System.Collections.Generic;
using Purchasing.Core.Domain;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class OrderReceiveModel
    {
        public int OrderId { get; set; }
        public IList<LineItem> LineItems { get; set; }
 
        public static OrderReceiveModel Create(Order order)
        {
            Check.Require(order != null);
            var viewModel = new OrderReceiveModel
            {
                OrderId = order.Id,
                LineItems = order.LineItems
            };

            return viewModel;
        }
    }
}