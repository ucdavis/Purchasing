using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Types;
using AggieEnterpriseApi.Validation;
using Microsoft.Extensions.Options;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Models.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Purchasing.Core.Services
{
    public interface IAggieEnterpriseService
    {
        Task<bool> IsAccountValid(string financialSegmentString, bool validateCVRs = true);

        Task<SubmitResult> UploadOrder(Order order);
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

        public async Task<SubmitResult> UploadOrder(Order order)
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



            throw new System.NotImplementedException();
        }

        private async Task<Supplier> GetSupplier(WorkgroupVendor vendor)
        {
            if (vendor == null || vendor.VendorId == null || vendor.VendorAddressTypeCode == null)
            {
                return null;
            }

            var search = new ScmSupplierFilterInput { SupplierNumber = new StringFilterInput { Eq = vendor.VendorId.ToString() } };
            var searchResult = await _aggieClient.ScmSupplierSearch.ExecuteAsync(search);
            var searchData = searchResult.ReadData();

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
