using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Services
{
    public interface IOrderAccess
    {
        IList<Order> GetViewableOrders(User user, IList<String> orderStatusCodeList );
    }

    public class OrderAccess : IOrderAccess
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IRepository<Order> _orderRepository;


        public IList<Order> GetViewableOrders(User user, IList<String> orderStatusCodeList)
        {
            throw new NotImplementedException();
        }
    }
}