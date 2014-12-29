using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Purchasing.Mvc.Signalr
{
    [HubName("purchaseInfo")]
    public class PurchaseInfoHub : Hub
    {
        private readonly PurchaseInfo _purchaseInfo;

        public PurchaseInfoHub() : this(PurchaseInfo.Instance)
        {
            
        }

        public PurchaseInfoHub(PurchaseInfo purchaseInfo)
        {
            _purchaseInfo = purchaseInfo;
        }
    }
}