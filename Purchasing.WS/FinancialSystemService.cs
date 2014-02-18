using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Purchasing.Core.Domain;
using Purchasing.WS.PurchaseDocumentService;
using System.Configuration;

namespace Purchasing.WS
{
    public class FinancialSystemService : IFinancialSystemService
    {
        // hard coded values per the integration design spec
        private const string CampusCode = "DV";
        private const string ItemTypeCode = "ITEM";
        private const string Countrycode = "US";
        private const string RequestType = "PR";

        private readonly string _url = ConfigurationManager.AppSettings["AfsUrl"];
        private readonly string _token = ConfigurationManager.AppSettings["AfsToken"];

        private purchasingDocumentsInterfaceServiceSOAPClient InitializeClient(bool extendedWait = false)
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            var endpointAddress = new EndpointAddress(_url);

            if (extendedWait)
            {
                binding.OpenTimeout = new TimeSpan(0,0,0,100);
            }

            var client = new purchasingDocumentsInterfaceServiceSOAPClient(binding, endpointAddress);

            return client;
        }

        public SubmitResult SubmitOrder(Order order, string userId, string kfsDocType = null)
        {
            try
            {
                var doc = new purchaseRequisitionInfo();

                // required fields
                doc.documentInfo = new documentInfo();
                doc.documentInfo.explanation = SetForDafis(string.Format(order.Justification), 400);
                doc.documentInfo.description = order.RequestNumber;
                doc.documentInfo.initiatorUserId = userId;
                doc.requestTypeCode = kfsDocType; //TODO: Remove?
                doc.requiredDate = order.DateNeeded.ToString("d");
                doc.sourceSystemOrderId = order.RequestNumber;  // currently does nothing in DaFIS, but should in KFS?                

                // vendor, only if valid kfs vendor
                if (order.Vendor != null && !string.IsNullOrEmpty(order.Vendor.VendorId))
                {
                    doc.vendorHeaderId = order.Vendor.VendorId;
                    doc.vendorDetailId = order.Vendor.VendorAddressTypeCode;
                }

                string line1, line2;

                // building code is specified
                if (order.Address.BuildingCode != null || !string.IsNullOrEmpty(order.Address.Building))
                {
                    line1 = string.Format("{0} {1}", order.Address.Room, order.Address.Building);
                    line2 = order.Address.Address;
                }
                else
                {
                    line1 = order.Address.Address;
                    line2 = string.Empty;
                }

                // delivery address
                doc.deliveryAddress = new purchasingAddressInfo()
                {
                    name = order.DeliverTo,
                    addressLine1 = SetForDafis(line1, 40),
                    addressLine2 = SetForDafis(line2, 40),
                    cityName = SetForDafis(order.Address.City, 40),
                    stateCode = SetForDafis(order.Address.State,2),
                    countryCode = SetForDafis(Countrycode,3),
                    zipCode = order.Address.Zip,
                    emailAddress = order.DeliverToEmail,
                    phoneNumber = !string.IsNullOrEmpty(order.DeliverToPhone) ? order.DeliverToPhone : order.Address.Phone,
                    campusCode = CampusCode,
                    buildingCode = order.Address.BuildingCode != null ? order.Address.BuildingCode.Id : string.Empty,
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
                    li.catelogNumber = SetForDafis(line.CatalogNumber, 15);
                    li.unitOfMeasureCode = line.Unit;
                    li.description = SetForDafis(line.Description, 400);
                    li.commodityCode = line.Commodity != null ? line.Commodity.Id.ToUpper() : string.Empty;
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
                var result = client.uploadRequisition(doc, _token, "PP"); //Hard coded to PP which was assigned to us.

                return new SubmitResult(result);
            }
            catch (TimeoutException)
            {
                return new SubmitResult() { Success = false, Messages = new List<string>() { "Service call timed out." } };
            }
            catch (CommunicationException ex)
            {
                return new SubmitResult() {Success = false, Messages = new List<string>() { "There was an error communicating with the campus financial system." , ex.Message}};
            }
            catch (Exception ex)
            {
                return new SubmitResult() { Success = false, Messages = new List<string>() { ex.Message } };
            }
        }

        /// <summary>
        /// Calculates the distribution %s for each account
        /// </summary>
        /// <remarks>
        /// Distributions are calculated based on what is displayed on screen.  This ensures a 100% distribution for associated accounts.
        /// </remarks>
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
                distributions.Add(new KeyValuePair<Split, decimal>(split, 100m));
            }
            // order level splits
            else if (!order.HasLineSplits)
            {
                foreach (var sp in order.Splits)
                {

                    // calculate the distribution percent, over entire order
                    var dist = (sp.Amount/order.GrandTotalFromDb)*100m;
                    
                    distributions.Add(new KeyValuePair<Split, decimal>(sp, dist));
                }
            }
            // should be a line level splits
            else if (order.HasLineSplits)
            {
                foreach (var sp in order.Splits)
                {
                    // calculate the distribution percent, over the line totals
                    var dist = (sp.Amount/sp.LineItem.TotalWithTax())*100m;
                    distributions.Add(new KeyValuePair<Split, decimal>(sp, dist));
                }
            }

            return distributions;
        }

        private List<KeyValuePair<Split, decimal>> CalculateShippingFreightDistributions(Order order)
        {
            var distributions = new List<KeyValuePair<Split, decimal>>();

            // single account, distribution is 100% over one account
            if (!order.HasLineSplits && order.Splits.Count == 1)
            {
                var split = order.Splits.FirstOrDefault();
                distributions.Add(new KeyValuePair<Split, decimal>(split, 100m));
            }
            // order level splits, just send the same %s over
            else if (!order.HasLineSplits)
            {
                foreach (var split in order.Splits)
                {
                    var dist = (split.Amount/order.GrandTotalFromDb)*100m;
                    distributions.Add(new KeyValuePair<Split, decimal>(split, dist));
                }
            }
            // line level splits, amortize the amounts to the total to find it's percent, by account
            else if (order.HasLineSplits)
            {
                // get the distinct accounts
                var accts = order.Splits.Select(a => new { a.Account, a.SubAccount, a.Project }).Distinct();

                foreach (var acct in accts)
                {
                    var amt = order.Splits.Where(a => a.Account == acct.Account && a.SubAccount == acct.SubAccount && a.Project == acct.Project).Sum(a => a.Amount);
                    var distribution = (amt / order.TotalWithTax()) * 100m;

                    var split = new Split()
                                    {
                                        Account = acct.Account,
                                        SubAccount = acct.SubAccount,
                                        Project = acct.Project
                                    };

                    // find the last one, make sure it adds up to 100%
                    if (distributions.Count == accts.Count() - 1)
                    {
                        var distSum = Math.Round(distributions.Sum(a => a.Value), 2);
                        var delta = 100m - Math.Round(distSum + distribution, 2);
                        if (delta > 0)
                        {
                            distribution = distribution + delta;
                        }
                    }

                    distributions.Add(new KeyValuePair<Split, decimal>(split, Math.Round(distribution,2)));
                }
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
            pai.projectCode = SetForDafis(split.Project, 10);
            pai.distributionPercent = distribution.ToString();

            return pai;
        }

        public FinancialDocumentStatus GetOrderStatus(string docNumber)
        {
            var client = InitializeClient(true);

            var result = client.getPurchaseRequisitionStatus(docNumber);

            return new FinancialDocumentStatus(result);
        }

        public string GetDocumentUrl(string docNumber)
        {
            var client = InitializeClient();

            var result = client.getPurchasingDocumentUrl(docNumber);

            return result;
        }

        public bool AllowedType(string docType)
        {
            var allowedKfsTypes = new string[1] { "PR" };

            return allowedKfsTypes.Contains(docType);
        }

        /// <summary>
        /// Shortens the string if necessary
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        private string SetForDafis(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            if (value.Length > maxLength)
            {
                return value.Substring(0, maxLength);
            }

            return value;
        }
    }
}
