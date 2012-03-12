using System.Collections.Generic;
using System.Linq;
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
            doc.sourceSystemOrderId = order.RequestNumber;  // currently does nothing in DaFIS, but should in KFS?

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

            var freightDistributions = new List<purchasingAccountingInfo>();
            var shippingDistributions = new List<purchasingAccountingInfo>();
            var fsDistributions = CalculateShippingFreightDistributions(order);
            foreach (var fs in fsDistributions)
            {
                freightDistributions.Add(CreateAccountInfo(fs.Key, fs.Value));
                shippingDistributions.Add(CreateAccountInfo(fs.Key, fs.Value));
            }
            doc.freightAccountingLines = freightDistributions.ToArray();
            doc.shippingAndHandlingAccountingLines = shippingDistributions.ToArray();

            var items = new List<purchasingItemInfo>();
            var distributions = CalculateDistributions(order);

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

                var accountingLines = new List<purchasingAccountingInfo>();

                // order with line splits
                if (order.HasLineSplits)
                {
                    foreach (var dist in distributions.Where(a => a.Key.LineItem == line))
                    {
                        accountingLines.Add(CreateAccountInfo(dist.Key, dist.Value));
                    }
                }
                // order or no splits
                else
                {
                    foreach (var dist in distributions)
                    {
                        accountingLines.Add(CreateAccountInfo(dist.Key, dist.Value));
                    }
                }

                li.accountingLines = accountingLines.ToArray();
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
        private List<KeyValuePair<Split,decimal>> CalculateDistributions(Order order)
        {
            var distributions = new List<KeyValuePair<Split, decimal>>();

            // no split (single account)
            if (!order.HasLineSplits && order.Splits.Count == 1)
            {
                var split = order.Splits.FirstOrDefault();
                distributions.Add(new KeyValuePair<Split, decimal>(split, 100));
            }
            // order level splits
            else if (!order.HasLineSplits)
            {
                foreach (var sp in order.Splits)
                {
                    // calculate the distribution percent, over entire order
                    var dist = (sp.Amount/order.TotalFromDb) * 100;
                    distributions.Add(new KeyValuePair<Split, decimal>(sp, dist));
                }
            }
            // should be a line level splits
            else if (order.HasLineSplits)
            {
                foreach (var sp in order.Splits)
                {
                    // calculate the distribution percent, over the line totals
                    var dist = (sp.Amount/sp.LineItem.Total())*100;
                    distributions.Add(new KeyValuePair<Split, decimal>(sp, dist));
                }
            }

            return distributions;
        }

        private List<KeyValuePair<Split, decimal>> CalculateShippingFreightDistributions(Order order)
        {
            var distributions = new List<KeyValuePair<Split, decimal>>();

            // get the distinct accounts
            var accts = order.Splits.Select(a => new { Account = a.Account, SubAccount = a.SubAccount, Project = a.Project }).Distinct();

            foreach (var acct in accts)
            {
                var amt = order.Splits.Where(a => a.Account == acct.Account && a.SubAccount == acct.SubAccount && a.Project == acct.Project).Sum(a => a.Amount);
                var distribution = amt/order.TotalWithTax();

                var split = new Split()
                                {
                                    Account = acct.Account,
                                    SubAccount = acct.SubAccount,
                                    Project = acct.Project
                                };

                distributions.Add(new KeyValuePair<Split, decimal>(split, distribution));
            }

            return distributions;
        }

        private purchasingAccountingInfo CreateAccountInfo(Split split, decimal distribution )
        {
            var pai = new purchasingAccountingInfo();

            var splitIdentifier = split.Account.IndexOf('-');
            pai.chartOfAccountsCode = split.Account.Substring(0, splitIdentifier);
            pai.accountNumber = split.Account.Substring(splitIdentifier + 1);
            pai.subAccountNumber = split.SubAccount;
            pai.projectCode = split.Project;
            pai.distributionPercent = distribution.ToString();

            return pai;
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
