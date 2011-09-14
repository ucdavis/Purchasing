using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Services
{
    public interface IEventService
    {
        void OrderApproved(Order order, Approval approval);
        void OrderStatusChange(Order order, OrderStatusCode newStatusCode);
        void OrderApprovalAdded(Order order, Approval approval);
    }

    public class EventService : IEventService
    {
        public void OrderApprovalAdded(Order order, Approval approval){}
        public void OrderApproved(Order order, Approval approval) { }
        public void OrderStatusChange(Order order, OrderStatusCode newStatusCode){ }
    }
}