using System.Collections.Generic;
using System.ServiceModel;
using Purchasing.Core.Domain;
using Purchasing.WS.PurchaseDocumentService;

namespace Purchasing.WS
{
    public class FinancialSystemService : IFinancialSystemService
    {
        // hard coded values per the integration design spec
        private const string CampusCode = "DV";
        private const string ItemTypeCode = "ITEM";
        private const string Countrycode = "US";
        private const string RequestType = "DPO";

        // url to webservice for testing
        private string _url = "https://kfs-test.ucdavis.edu/kfs-stg/remoting/purchaseDocumentsInterfaceServiceSOAP";
        private string _token = "stage";

        private purchasingDocumentsInterfaceServiceSOAPClient InitializeClient()
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            var endpointAddress = new EndpointAddress(_url);

            var client = new purchasingDocumentsInterfaceServiceSOAPClient(binding, endpointAddress);

            return client;
        }

        public SubmitResult SubmitOrder(Order order, string userId)
        {
            var doc = new purchaseRequisitionInfo();

            // required fields
            doc.documentInfo = new documentInfo();
            doc.documentInfo.explanation = order.Justification;
            doc.documentInfo.initiatorUserId = userId;
            doc.requestTypeCode = RequestType;
            doc.requiredDate = order.DateNeeded.ToString();

            // vendor, only if valid kfs vendor
            if (!string.IsNullOrEmpty(order.Vendor.VendorId))
            {
                doc.vendorHeaderId = order.Vendor.VendorId;
                doc.vendorDetailId = order.Vendor.VendorAddressTypeCode;    
            }
            
            // delivery address
            doc.deliveryAddress = new purchasingAddressInfo()
            {
                addressLine1 = order.Address.Address,
                cityName = order.Address.City,
                stateCode = order.Address.State,
                countryCode = Countrycode,
                emailAddress = order.DeliverToEmail,
                phoneNumber = order.Address.Phone,
                campusCode = CampusCode,
                buildingCode = order.Address.Building,    // need to deal with this
                roomNumber = order.Address.Room
            };
            doc.deliveryInstructionText = string.Empty;     // don't have this from anywhere yet

            // shipping/handling and freight
            doc.freightAmount = order.FreightAmount.ToString();
            doc.shippingAndHandlingAmount = order.ShippingAmount.ToString();

            var items = new List<purchasingItemInfo>();

            // line items
            foreach (var line in order.LineItems)
            {
                var li = new purchasingItemInfo();
                li.unitOfMeasureCode = line.Unit;
                li.description = line.Description;
                li.commodityCode = line.Commodity != null ? line.Commodity.Id : string.Empty;
                li.unitPrice = line.UnitPrice.ToString();
                li.quantity = line.Quantity.ToString();
                li.itemTypeCode = ItemTypeCode;

                if (order.HasLineSplits)
                {
                    var accountingLines = new List<purchasingAccountingInfo>();

                    foreach (var aline in line.Splits)
                    {
                        var pai = new purchasingAccountingInfo();

                        // account information
                        var splitIdentifier = aline.Account.IndexOf('-');
                        pai.chartOfAccountsCode = aline.Account.Substring(0, splitIdentifier);
                        pai.accountNumber = aline.Account.Substring(splitIdentifier + 1);
                        pai.subAccountNumber = aline.SubAccount;
                        pai.projectCode = aline.Project;
                        pai.distributionPercent = ((aline.Amount / (aline.LineItem.Quantity * aline.LineItem.UnitPrice)) * 100).ToString();

                        accountingLines.Add(pai);
                    }
                }

                items.Add(li);
            }

            doc.items = items.ToArray();

            // try to upload the requisition
            var client = InitializeClient();
            var result = client.uploadRequisition(doc, _token);

            return new SubmitResult(result);
        }

        /// <summary>
        /// Calculates the distribution %s for each account
        /// </summary>
        /// <param name="order"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<KeyValuePair<Split,decimal>> CalculateDistributions(Order order, LineItem line = null)
        {
            var distributions = new List<KeyValuePair<Split, decimal>>();

            // no split (single account)
            if (!order.HasLineSplits && order.Splits.Count == 1)
            {
                
            }
            // order level splits
            else if (!order.HasLineSplits)
            {
                
            }
            // should be a line level splits
            else if (order.HasLineSplits)
            {
                
            }

            return distributions;
        }

        public FinancialDocumentStatus GetOrderStatus(string docNumber)
        {
            var client = InitializeClient();

            var result = client.getPurchaseRequisitionStatus(docNumber);

            return new FinancialDocumentStatus(result);
        }

        public string GetDocumentUrl(string docNumber)
        {
            var client = InitializeClient();

            var result = client.getPurchasingDocumentUrl(docNumber);

            return result;
        }
    }
}
