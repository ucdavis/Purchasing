using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Services
{
    public interface IOrderAccess
    {
        IList<Order> GetViewableOrders(OrderStatusCode statusCode );
        IList<Order> GetViewableOrders(IList<OrderStatusCode> orderStatusCodesList);
    }

    public class OrderAccess : IOrderAccess
    {
        private readonly IUserIdentity _userIdentity;
        //private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Approval> _approvalRespository;

        public OrderAccess(IUserIdentity userIdentity, IRepository<Approval> approvalRespository) //IRepository<Order> orderRepository,
        {
            _userIdentity = userIdentity;
            //_orderRepository = orderRepository;
            _approvalRespository = approvalRespository;
        }

        public IList<Order> GetViewableOrders(OrderStatusCode statusCode)
        {
            var user = _userIdentity.Current;
            //var orderIds =
            //    _approvalRespository.Queryable.Where(
            //        a => a.User != null && a.User.Id == user && !a.Approved && a.StatusCode == statusCode).Select(a => a.Order.Id).Distinct().ToList();
            //var orders = _orderRepository.Queryable.Where(a => orderIds.Contains(a.Id) && a.StatusCode == statusCode).ToList();

            var orders = _approvalRespository.Queryable.Where(a => a.User != null && a.User.Id == user && !a.Approved && a.StatusCode == statusCode)
                .Select(a => a.Order).Where(b => b.StatusCode == statusCode).ToList();
            return orders;
        }

        public IList<Order> GetViewableOrders(IList<OrderStatusCode> orderStatusCodesList )
        {
            var orders = new List<Order>();
            var comparer = new BaseObjectEqualityComparer<Order>();
            foreach (var orderStatusCode in orderStatusCodesList)
            {
                orders = orders.Union(GetViewableOrders(orderStatusCode), comparer).ToList();
            }
            return orders;
        }

    }
}