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

        //private string _url = "http://kfs-test.ucdavis.edu/kfs-stg/remoting/purchaseDocumentsInterfaceServiceSOAP";

        private string _url = "http://kfs-test1.ucdavis.edu/kfs-stg/remoting/purchaseDocumentsInterfaceServiceSOAP?wsdl";

        private purchasingDocumentsInterfaceServiceSOAPClient InitializeClient()
        {
            var binding = new BasicHttpBinding();
            var endpointAddress = new EndpointAddress(_url);

            var client = new purchasingDocumentsInterfaceServiceSOAPClient(binding, endpointAddress);

            return client;
        }

        public void SubmitOrder(Order order, string userId)
        {
            var doc = new purchaseRequisitionInfo();

            // submit the basic information for the request
            doc.requestorUserId = userId;
            doc.sourceSystemOrderId = order.RequestNumber;
            doc.deliveryAddress = new purchasingAddressInfo()
                                      {
                                          addressLine1 = order.Address.Address,
                                          cityName = order.Address.City,
                                          stateCode = order.Address.State,
                                          countryCode = Countrycode,
                                          emailAddress = order.DeliverToEmail,
                                          phoneNumber = order.Address.Phone,
                                          campusCode = CampusCode,
                                          buildingCode = order.Address.Building,
                                          roomNumber = order.Address.Room
                                      };
            doc.vendorHeaderId = order.Vendor.VendorId;
            doc.vendorDetailId = order.Vendor.VendorAddressTypeCode;
            doc.freightAmount = order.FreightAmount.ToString();
            doc.shippingAndHandlingAmount = order.ShippingAmount.ToString();

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
            }

            // try to upload the requisition
            var client = InitializeClient();
            var result = client.uploadRequisition(doc);
        }

        public void GetOrderStatus(string docNumber)
        {
            var client = InitializeClient();

            var result = client.getPurchaseRequisitionStatus(docNumber);
        }

        public void GetDocumentUrl(string docNumber)
        {
            var client = InitializeClient();

            var result = client.getPurchasingDocumentUrl(docNumber);
        }
    }
}
