using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Types;
using AggieEnterpriseApi.Validation;
using Microsoft.Extensions.Options;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Models.AggieEnterprise;
using Purchasing.Core.Models.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Purchasing.Core.Services
{
    public interface IAggieEnterpriseService
    {
        Task<bool> IsAccountValid(string financialSegmentString, bool validateCVRs = true);

        Task<SubmitResult> UploadOrder(Order order, string purchaserEmail);
        Task<AeResultStatus> LookupOrderStatus(string requestId);

        Task<Commodity[]> GetPurchasingCategories();
    }
    public class AggieEnterpriseService : IAggieEnterpriseService
    {
        private readonly IAggieEnterpriseClient _aggieClient;
        private readonly AggieEnterpriseOptions _options;
        private readonly IRepositoryFactory _repositoryFactory;

        public AggieEnterpriseService(IOptions<AggieEnterpriseOptions> options, IRepositoryFactory repositoryFactory)
        {
            _aggieClient = GraphQlClient.Get(options.Value.GraphQlUrl, options.Value.Token);
            _options = options.Value;
            _repositoryFactory = repositoryFactory;
        }

        public async Task<bool> IsAccountValid(string financialSegmentString, bool validateCVRs = true)
        {
            var segmentStringType = FinancialChartValidation.GetFinancialChartStringType(financialSegmentString);

            if (segmentStringType == FinancialChartStringType.Gl)
            {
                var result = await _aggieClient.GlValidateChartstring.ExecuteAsync(financialSegmentString, validateCVRs);

                var data = result.ReadData();

                var isValid = data.GlValidateChartstring.ValidationResponse.Valid;


                return isValid;
            }

            if (segmentStringType == FinancialChartStringType.Ppm)
            {
                var result = await _aggieClient.PpmStringSegmentsValidate.ExecuteAsync(financialSegmentString);

                var data = result.ReadData();

                var isValid = data.PpmStringSegmentsValidate.ValidationResponse.Valid;

                return isValid;
            }



            return false;
        }

        public async Task<SubmitResult> UploadOrder(Order order, string purchaserEmail)
        {
            

            var inputOrder = new ScmPurchaseRequisitionRequestInput
            {
                Header = new ActionRequestHeaderInput
                {
                    ConsumerTrackingId = order.ConsumerTrackingId.ToString(),
                    ConsumerReferenceId = order.RequestNumber,
                    ConsumerNotes = $"Workgroup: {order.Workgroup.Name}".SafeTruncate(240),
                    BoundaryApplicationName = "PrePurchasing"
                }
            };
            var supplier = await GetSupplier(order.Vendor);
            if (supplier == null)
            {
                //TODO: Create an error message that the supplier was missing or not found.
                return new SubmitResult { Success = false, Messages = new List<string>() { "Supplier missing or not found" } };
            }
            string bp = string.Empty;
            if (!string.IsNullOrWhiteSpace(order.BusinessPurpose)){
                bp = $" -- Business Purpose: {order.BusinessPurpose}";
            }

            inputOrder.Payload = new ScmPurchaseRequisitionInput
            {
                RequisitionSourceName = _options.RequisitionSourceName,
                SupplierNumber = supplier.SupplierNumber,
                SupplierSiteCode = supplier.SupplierSiteCode ,
                RequesterEmailAddress = purchaserEmail,
                Description = order.RequestNumber,
                Justification = $"{order.Justification}{bp}".SafeTruncate(1000),                
            };
                
            var unitCodes = order.LineItems.Select(a => a.Unit).Distinct().ToArray();

            var unitsOfMeasure = _repositoryFactory.UnitOfMeasureRepository.Queryable.Where(a => unitCodes.Contains(a.Id)).ToArray();

            var distributions = await CalculateDistributions(order);
            var lineItems = new List<ScmPurchaseRequisitionLineInput>();
            foreach(var line in order.LineItems)
            {
                var li = new ScmPurchaseRequisitionLineInput
                {
                    Amount = Math.Round(line.Total(), 3),
                    Quantity = line.Quantity,
                    ItemDescription = line.Description.SafeTruncate(240),
                    UnitPrice = line.UnitPrice,
                    UnitOfMeasure = unitsOfMeasure.FirstOrDefault(a => a.Id == line.Unit)?.Name,
                    PurchasingCategoryName = await LookupCategoryCode(line.Commodity.Id), //Mostly faked TODO: Fix this
                    NoteToBuyer = line.Notes.SafeTruncate(1000),
                    RequestedDeliveryDate = order.DateNeeded.ToString("yyyy-MM-dd"),                   
                };

                var aeDist = new List<ScmPurchaseRequisitionDistributionInput>();
                // order with line splits
                if (order.HasLineSplits)
                {
                    foreach (var dist in distributions.Where(a => a.Key.Split.LineItem == line))
                    {
                        //accountingLines.Add(CreateAccountInfo(dist.Key, dist.Value));
                        aeDist.Add(new ScmPurchaseRequisitionDistributionInput()
                        {
                            Percent = dist.Value,
                            GlSegmentString = dist.Key.FinincialSegmentString,
                        });
                    }
                }
                // order or no splits
                else
                {
                    foreach (var dist in distributions)
                    {
                        aeDist.Add(new ScmPurchaseRequisitionDistributionInput()
                        {
                            Percent = dist.Value,
                            GlSegmentString = dist.Key.FinincialSegmentString,
                        });
                    }
                }

                li.Distributions = aeDist.ToArray();


                lineItems.Add(li);
            }
            

            inputOrder.Payload.Lines = lineItems;
            
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(inputOrder);
            Log.ForContext("jsonPayload", jsonPayload).Information("Aggie Enterprise Payload");

            var NewOrderRequsition = await _aggieClient.ScmPurchaseRequisitionCreate.ExecuteAsync(inputOrder);
            
            var responseData = NewOrderRequsition.ReadData();

            Log.ForContext("response", System.Text.Json.JsonSerializer.Serialize(responseData)).Information("Aggie Enterprise Response");
            
            var rtValue = new SubmitResult { Success = false, Messages = new List<string>() };

            if (responseData.ScmPurchaseRequisitionCreate.RequestStatus.RequestStatus == RequestStatus.Rejected || responseData.ScmPurchaseRequisitionCreate.RequestStatus.RequestStatus == RequestStatus.Error)
            {
                
                if (responseData.ScmPurchaseRequisitionCreate.RequestStatus.Equals(RequestStatus.Error))
                {
                    rtValue.Messages.Add("Aggie Enterprise Error");
                    foreach(var message in responseData.ScmPurchaseRequisitionCreate.RequestStatus.ErrorMessages)
                    {
                        rtValue.Messages.Add($"Error: {message}");
                    }
                }
                else
                {
                    rtValue.Messages.Add("Aggie Enterprise Rejected");
                    foreach (var message in responseData.ScmPurchaseRequisitionCreate.ValidationResults.ErrorMessages)
                    {
                        rtValue.Messages.Add(message);
                    }
                }

                return rtValue;
            }

            rtValue = new SubmitResult { Success = true, DocNumber = responseData.ScmPurchaseRequisitionCreate.RequestStatus.RequestId.ToString() };

            return rtValue;
        }

        public async Task<AeResultStatus> LookupOrderStatus(string requestId)
        {
            try
            {
                var result = await _aggieClient.ScmPurchaseRequisitionRequestStatus.ExecuteAsync(requestId);

                var data = result.ReadData();

                var rtValue = new AeResultStatus
                {
                    RequestId = data.ScmPurchaseRequisitionRequestStatus.RequestStatus.RequestId,
                    ConsumerTrackingId = data.ScmPurchaseRequisitionRequestStatus.RequestStatus.ConsumerTrackingId,
                    ConsumerReferenceId = data.ScmPurchaseRequisitionRequestStatus.RequestStatus.ConsumerReferenceId,
                    ConsumerNotes = data.ScmPurchaseRequisitionRequestStatus.RequestStatus.ConsumerNotes,
                    RequestDateTime = data.ScmPurchaseRequisitionRequestStatus.RequestStatus.RequestDateTime.DateTime.ToPacificTime().ToString("dddd, MMMM dd yyyy h:mm tt"),
                    LastStatusDateTime = data.ScmPurchaseRequisitionRequestStatus.RequestStatus.LastStatusDateTime.DateTime.ToPacificTime().ToString("dddd, MMMM dd yyyy h:mm tt"),
                    ProcessedDateTime = data.ScmPurchaseRequisitionRequestStatus.RequestStatus.ProcessedDateTime?.DateTime.ToPacificTime().ToString("dddd, MMMM dd yyyy h:mm tt"),
                    Status = Enum.GetName(typeof(RequestStatus), data.ScmPurchaseRequisitionRequestStatus.RequestStatus.RequestStatus),
                };

                return rtValue;
            }
            catch
            {
                Log.Warning("Aggie Enterprise LookupOrderStatus failed for {requestId}", requestId);
                return null;
            }
        }

        public async Task<Commodity[]> GetPurchasingCategories()
        {
            var filter = new ScmPurchasingCategoryFilterInput();
            filter.SearchCommon = new SearchCommonInputs();
            filter.SearchCommon.Limit = 5000; //Should only be a few hundred
            filter.LastUpdateDateTime = new DateFilterInput { Gt = new DateTime(2000, 01, 01) }; //2022-07-14 should return a smaller number


            var result = await _aggieClient.ScmPurchasingCategorySearch.ExecuteAsync(filter);

            var data = result.ReadData();

            if(data.ScmPurchasingCategorySearch.Metadata.NextStartIndex != null)
            {
                Log.Error("Aggie Enterprise GetPurchasingCategories has a NextStartIndex.  This is not expected.  Please check the code.");
            }

            return data.ScmPurchasingCategorySearch.Data.Select(a => new Commodity{ Id = a.Code, Name = a.Name, IsActive = a.Enabled }).ToArray();
        }

        
        /// <summary>
        /// Potentially we could cache this lookup, but it should really only be a SIT2 test thing....
        /// </summary>
        /// <param name="split"></param>
        /// <returns></returns>
        private async Task<KfsToAeCoa> LookupAccount(Split split)
        {
            var rtValue = new KfsToAeCoa { Split = split };

            var chart = split.Account.Split('-')[0];
            var account = split.Account.Split('-')[1];

            var distributionResult = await _aggieClient.KfsConvertAccount.ExecuteAsync(chart, account, split.SubAccount);
            var distributionData = distributionResult.ReadData();
            if (distributionData.KfsConvertAccount.GlSegments != null)
            {
                rtValue.FinincialSegmentString = new GlSegments(distributionData.KfsConvertAccount.GlSegments).ToSegmentString();
            }
            else
            {
                //TODO: REMOVE THIS!!!!
                Log.Warning("No GL Segments found for {chart}-{account}-{subAccount} FAKING IT!!!!", chart, account, split.SubAccount);
                rtValue.FinincialSegmentString = "3110-13U02-ADNO006-000000-43-000-0000000000-000000-0000-000000-000000";
            }
            return rtValue;
        }

        private async Task<string> LookupCategoryCode(string category)
        {
            try { 
            var result = await _aggieClient.ScmPurchasingCategoryByCode.ExecuteAsync(category);
            var data = result.ReadData();
            return data.ScmPurchasingCategoryByCode?.Name; }
            catch
            {
                Log.Warning("Aggie Enterprise LookupCategoryCode failed for {category} FAKING IT!!!!", category);
                return "Paper products";
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
        private async Task<List<KeyValuePair<KfsToAeCoa, decimal>>> CalculateDistributions(Order order)
        {
            var distributions = new List<KeyValuePair<KfsToAeCoa, decimal>>();

            // no split (single account)
            if (!order.HasLineSplits && order.Splits.Count == 1)
            {
                var split = order.Splits.FirstOrDefault();
                distributions.Add(new KeyValuePair<KfsToAeCoa, decimal>(await LookupAccount(split), 100m));
            }
            // order level splits
            else if (!order.HasLineSplits)
            {
                foreach (var sp in order.Splits)
                {

                    // calculate the distribution percent, over entire order
                    var dist = Math.Round( (sp.Amount / order.GrandTotalFromDb) * 100m, 3);

                    distributions.Add(new KeyValuePair<KfsToAeCoa, decimal>(await LookupAccount(sp), dist));
                }
            }
            // should be a line level splits
            else if (order.HasLineSplits)
            {
                foreach (var sp in order.Splits)
                {
                    // calculate the distribution percent, over the line totals
                    var dist = Math.Round( (sp.Amount / sp.LineItem.TotalWithTax()) * 100m, 3);
                    distributions.Add(new KeyValuePair<KfsToAeCoa, decimal>(await LookupAccount(sp), dist));
                }
            }

            return distributions;
        }

        private async Task<Supplier> GetSupplier(WorkgroupVendor vendor)
        {
            if (vendor == null || vendor.VendorId == null || vendor.VendorAddressTypeCode == null)
            {
                return null;
            }
            try
            {
                var search = new ScmSupplierFilterInput { SupplierNumber = new StringFilterInput { Eq = vendor.VendorId.ToString() } };
                var searchResult = await _aggieClient.ScmSupplierSearch.ExecuteAsync(search);
                var searchData = searchResult.ReadData();

                var rtValue = new Supplier();
                rtValue.SupplierNumber = searchData.ScmSupplierSearch.Data.First().SupplierNumber.ToString();
                rtValue.SupplierSiteCode = searchData.ScmSupplierSearch.Data.First().Sites.Where(a =>  
                    a.Location.City.Equals(vendor.City, System.StringComparison.OrdinalIgnoreCase) &&
                    a.Location.State.Equals(vendor.State, System.StringComparison.OrdinalIgnoreCase) &&
                    (a.Location.AddressLine1.Equals(vendor.Name, System.StringComparison.OrdinalIgnoreCase) && a.Location.AddressLine2.Equals(vendor.Line1, System.StringComparison.OrdinalIgnoreCase)) || 
                    (a.Location.AddressLine1.Equals(vendor.Line1, System.StringComparison.OrdinalIgnoreCase))
                    ).First().SupplierSiteCode;                
                
                if (!string.IsNullOrWhiteSpace(rtValue.SupplierNumber) && !string.IsNullOrWhiteSpace(rtValue.SupplierSiteCode))
                {
                    return rtValue;
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }

            return null;
        }



        public class Supplier
        {
            public string SupplierNumber { get; set; }
            public string SupplierSiteCode { get; set; }
        }

        public class KfsToAeCoa
        {
            public Split Split { get;set;}
            public string FinincialSegmentString { get;set;}
        }
    }

    //TODO: Replace with something more robust?
    public class SubmitResult
    {

        public SubmitResult()
        {

        }

        public bool Success { get; set; }
        public string DocNumber { get; set; }
        public List<string> Messages { get; set; }
    }
}
