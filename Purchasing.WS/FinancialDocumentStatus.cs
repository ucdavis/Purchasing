using Purchasing.WS.PurchaseDocumentService;

namespace Purchasing.WS
{
    public class FinancialDocumentStatus
    {
        public FinancialDocumentStatus(purchaseRequisitionStatusInfo prStatus)
        {
            DocumentNumber = prStatus.documentNumber;
            FullyPaid = prStatus.fullyPaid;
            Received = prStatus.received;
            RouteLevel = prStatus.routeLevel;
            RouteStatusCode = prStatus.routeStatusCode;

            PoDocumentNumber = prStatus.purchaseOrderDocumentNumber;
            PoNumber = prStatus.purchaseOrderNumber;
            PoStatusCode = prStatus.purchaseOrderStatusCode;
            PoStatusName = prStatus.purchaseOrderStatusName;
            PoTypeCode = prStatus.purchaseOrderTypeCode;

            PrStatusCode = prStatus.purchaseRequsitionStatusCode;
            PRStatusName = prStatus.purchaseRequsitionStatusName;

            DocUrl = prStatus.documentUrl;
        }

        public string DocumentNumber { get; set; }
        public bool FullyPaid { get; set; }
        public bool Received { get; set; }
        public string RouteLevel { get; set; }
        public string RouteStatusCode { get; set; }

        public string PoDocumentNumber { get; set; }
        public string PoNumber { get; set; }
        public string PoStatusCode { get; set; }
        public string PoStatusName { get; set; }
        public string PoTypeCode { get; set; }

        public string PrStatusCode { get; set; }
        public string PRStatusName { get; set; }

        /// <summary>
        /// Url for the document into the financial system, blank if not completed in financial system
        /// </summary>
        public string DocUrl { get; set; }
    }
}