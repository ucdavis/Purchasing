using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Purchasing.Mvc.Signalr
{
    [HubName("orderNotification")]
    public class OrderNotificationHub : Hub
    {
        private readonly OrderNotificationManager _orderNotificationManager;

        public OrderNotificationHub() : this(OrderNotificationManager.Instance)
        {
            
        }

        public OrderNotificationHub(OrderNotificationManager orderNotificationManager)
        {
            _orderNotificationManager = orderNotificationManager;
        }
    }
}