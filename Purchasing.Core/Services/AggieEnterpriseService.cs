using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Types;
using AggieEnterpriseApi.Validation;
using Microsoft.Extensions.Options;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Models.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Purchasing.Core.Services
{
    public interface IAggieEnterpriseService
    {
        Task<bool> IsAccountValid(string financialSegmentString, bool validateCVRs = true);

        Task<SubmitResult> UploadOrder(Order order, string purchaserEmail);
    }
    public class AggieEnterpriseService : IAggieEnterpriseService
    {
        private readonly IAggieEnterpriseClient _aggieClient;

        public AggieEnterpriseService(IOptions<AggieEnterpriseOptions> options)
        {
            _aggieClient = GraphQlClient.Get(options.Value.GraphQlUrl, options.Value.Token);
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
                    ConsumerReferenceId = order.ReferenceNumber,
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
                RequisitionSourceName = "UCD SLOTH",
                SupplierNumber = supplier.SupplierNumber,
                SupplierSiteCode = supplier.SupplierSiteCode ,
                RequesterEmailAddress = purchaserEmail,
                Description = order.ReferenceNumber,
                Justification = $"{order.Justification}{bp}".SafeTruncate(1000),
            };



            return new SubmitResult { Success = false, Messages = new List<string>() { "AE Code not ready yet." } };
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
        
        private class Supplier
        {
            public string SupplierNumber { get; set; }
            public string SupplierSiteCode { get; set; }
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
