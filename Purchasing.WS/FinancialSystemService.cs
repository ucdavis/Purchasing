using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purchasing.Core.Domain;

namespace Purchasing.WS
{
    public class FinancialSystemService : IFinancialSystemService
    {
        public void SubmitOrder(Order order, string userId)
        {
            var requisition = new PurchaseDocumentService.purchaseRequisitionInfo();


            // try to upload the requisition
            var client = new PurchaseDocumentService.purchasingDocumentsInterfaceServiceSOAPClient();
            client.uploadRequisition(requisition);
        }
    }
}
