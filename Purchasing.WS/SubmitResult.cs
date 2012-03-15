using System.Collections.Generic;
using System.Linq;
using Purchasing.WS.PurchaseDocumentService;

namespace Purchasing.WS
{
    public class SubmitResult
    {
        public SubmitResult(documentCreationResult result)
        {
            Success = result.success;

            if (result.documentNumbers != null && result.documentNumbers[0] != null)
            {
                DocNumber = result.documentNumbers[0];
            }

            if (result.messages != null)
            {
                Messages = result.messages.Select(a => a.message).ToList();    
            }
        }

        public SubmitResult()
        {
            
        }

        public bool Success { get; set; }
        public string DocNumber { get; set; }
        public List<string> Messages { get; set; }
    }
}