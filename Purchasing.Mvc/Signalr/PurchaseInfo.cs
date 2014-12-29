using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Purchasing.Mvc.Signalr
{
    public class PurchaseInfo
    {
        // Singleton instance
        private static readonly Lazy<PurchaseInfo> _instance = new Lazy<PurchaseInfo>(
            () => new PurchaseInfo(GlobalHost.ConnectionManager.GetHubContext<PurchaseInfoHub>().Clients));

        public static PurchaseInfo Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext<dynamic> Clients { get; set; }

        public PurchaseInfo(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        public void BroadcastOrderUpdate(int id)
        {
            Clients.All.orderUpdate(id);
        }
    }
}