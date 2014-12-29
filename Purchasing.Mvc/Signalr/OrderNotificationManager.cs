using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Purchasing.Core.Domain;

namespace Purchasing.Mvc.Signalr
{
    public class OrderNotificationManager
    {
        // Singleton instance
        private static readonly Lazy<OrderNotificationManager> _instance = new Lazy<OrderNotificationManager>(
            () => new OrderNotificationManager(GlobalHost.ConnectionManager.GetHubContext<OrderNotificationHub>().Clients));

        public static OrderNotificationManager Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext<dynamic> Clients { get; set; }

        public OrderNotificationManager(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        public void BroadcastOrderUpdate(Order order)
        {
            Clients.All.orderUpdate(new {order.Id, order.RequestNumber});
        }
    }
}