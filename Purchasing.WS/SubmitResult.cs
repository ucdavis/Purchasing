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
            Messages = result.messages.Select(a => a.message).ToList();

        }

        public bool Success { get; set; }
        public string DocNumber { get; set; }
        public List<string> Messages { get; set; }
    }
}