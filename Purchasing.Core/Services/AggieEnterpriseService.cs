﻿using AggieEnterpriseApi;
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
using static Purchasing.Core.Services.AggieEnterpriseService;

namespace Purchasing.Core.Services
{
    public interface IAggieEnterpriseService
    {
        Task<bool> IsAccountValid(string financialSegmentString, bool validateCVRs = true);

        Task<SubmitResult> UploadOrder(Order order, string purchaserEmail);
        Task<AeResultStatus> LookupOrderStatus(string requestId);

        Task<Commodity[]> GetPurchasingCategories();
        Task<UnitOfMeasure[]> GetUnitOfMeasures();

        Task<List<IdAndName>> SearchSupplier(string query);
        Task<List<IdAndName>> SearchSupplierAddress(string query);
        Task<Supplier> GetSupplier(WorkgroupVendor vendor);
        Task<WorkgroupVendor> GetSupplierForWorkgroup(WorkgroupVendor workgroupVendor);

        Task<List<IdAndName>> SearchShippingAddress(string query);
        Task<WorkgroupAddress> GetShippingAddress(WorkgroupAddress workgroupAddress);
    }
    public class AggieEnterpriseService : IAggieEnterpriseService
    {
        //If there is any validation that uses nullable start and end dates, use this helper method: DateTime.UtcNow.ToPacificTime().IsActiveDate(a.StartDateActive, a.EndDateActive)


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
                return new SubmitResult { Success = false, Messages = new List<string>() { "Vendor/Supplier missing or not found" } };
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

            var shippingLocation = await GetShippingAddress(order.Address); //Verify still good.

            var distributions = await CalculateDistributions(order);
            var lineItems = new List<ScmPurchaseRequisitionLineInput>();
            foreach(var line in order.LineItems)
            {
                var li = new ScmPurchaseRequisitionLineInput
                {
                    //Amount = Math.Round(line.Total(), 3),
                    Quantity = line.Quantity,
                    ItemDescription = line.Description.SafeTruncate(240),
                    UnitPrice = line.UnitPrice,
                    UnitOfMeasure = unitsOfMeasure.FirstOrDefault(a => a.Id == line.Unit)?.Name,
                    PurchasingCategoryName = line.Commodity.Name, 
                    NoteToBuyer = line.Notes.SafeTruncate(1000),
                    RequestedDeliveryDate = order.DateNeeded.ToString("yyyy-MM-dd"),     
                    LineType = ScmPurchaseRequisitionLineType.Quantity,
                };
                if (shippingLocation != null && !string.IsNullOrWhiteSpace(shippingLocation.AeLocationCode))
                {
                    li.DeliveryToLocationCode = shippingLocation.AeLocationCode;
                }

                var aeDist = new List<ScmPurchaseRequisitionDistributionInput>();
                // order with line splits
                if (order.HasLineSplits)
                {
                    foreach (var dist in distributions.Where(a => a.Key.Split.LineItem == line))
                    {
                        if (dist.Key.IsPPm)
                        {
                            aeDist.Add(new ScmPurchaseRequisitionDistributionInput()
                            {
                                Percent = dist.Value,
                                PpmSegmentString = dist.Key.FinincialSegmentString,
                            });
                        }
                        else
                        {
                            //accountingLines.Add(CreateAccountInfo(dist.Key, dist.Value));
                            aeDist.Add(new ScmPurchaseRequisitionDistributionInput()
                            {
                                Percent = dist.Value,
                                GlSegmentString = dist.Key.FinincialSegmentString,
                            });
                        }
                    }
                }
                // order or no splits
                else
                {
                    foreach (var dist in distributions)
                    {
                        if (dist.Key.IsPPm)
                        {
                            aeDist.Add(new ScmPurchaseRequisitionDistributionInput()
                            {
                                Percent = dist.Value,
                                PpmSegmentString = dist.Key.FinincialSegmentString,
                            });
                        }
                        else
                        {
                            //accountingLines.Add(CreateAccountInfo(dist.Key, dist.Value));
                            aeDist.Add(new ScmPurchaseRequisitionDistributionInput()
                            {
                                Percent = dist.Value,
                                GlSegmentString = dist.Key.FinincialSegmentString,
                            });
                        }
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
            filter.SearchCommon.Limit = 5000; //Should only be a few hundred eventually, AE may cap this at 500
            filter.LastUpdateDateTime = new DateFilterInput { Gt = new DateTime(2000, 01, 01) }; //2022-07-14 should return a smaller number


            var result = await _aggieClient.ScmPurchasingCategorySearch.ExecuteAsync(filter);

            var data = result.ReadData();

            if(data.ScmPurchasingCategorySearch.Metadata.NextStartIndex != null)
            {
                Log.Error("Aggie Enterprise GetPurchasingCategories has a NextStartIndex.  This is not expected.  Please check the code.");
                throw new Exception("Aggie Enterprise GetPurchasingCategories has a NextStartIndex.  This is not expected.  Please check the code.");
                //Ok, if this exception ever happens, we just need to do paging and join the results together.
            }

            return data.ScmPurchasingCategorySearch.Data.Select(a => new Commodity
            {
                Id = a.Code,
                Name = a.Name,
                IsActive =
                a.Enabled && DateTime.UtcNow.ToPacificTime().IsActiveDate(a.StartDateActive, a.EndDateActive)
            }).ToArray();
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

            var lookupInAe = false;

            Log.Error("Faking AE CCOA for specific account. Remove before Go Live");
            
            //TODO: Remove
            switch (split.Account)
            {
                case "3-CRU9033":
                    rtValue.FinincialSegmentString = "3110-13U00-ADNO006-534101-40-000-0000000000-200504-0000-000000-000000";
                    break;
                case "3-ADOIGAA":
                    rtValue.FinincialSegmentString = "3110-13U00-ADNO003-534101-40-000-0000000000-200504-0000-000000-000000";
                    break;
                case "3-CRUCECP":
                    rtValue.FinincialSegmentString = "K30CRUCECP-TASK01-ADNO006-522400";
                    rtValue.IsPPm = true;
                    break;
                case "3-IPFFPR3":
                    rtValue.FinincialSegmentString = "K30IPFFPR3-TASK01-ADNO006-522400";
                    rtValue.IsPPm = true;
                    break;
                case "P-9530101":
                    rtValue.FinincialSegmentString = "KP0953010U-301001-ADNO001-525900";
                    rtValue.IsPPm = true;
                    break;
                default:
                    lookupInAe = true;
                    break;
            }

            if (!lookupInAe)
            {
                return rtValue;
            }

            var distributionResult = await _aggieClient.KfsConvertAccount.ExecuteAsync(chart, account, split.SubAccount);
            var distributionData = distributionResult.ReadData();
            if (distributionData.KfsConvertAccount.GlSegments != null)
            {
                var tempGlSegments = new GlSegments(distributionData.KfsConvertAccount.GlSegments);
                if (string.IsNullOrWhiteSpace(tempGlSegments.Account) || tempGlSegments.Account == "000000")
                {
                    //770000
                    Log.Warning($"Natural Account of 000000 detected. Substituting {_options.DefaultNaturalAccount}");
                    tempGlSegments.Account = _options.DefaultNaturalAccount;
                }
                rtValue.FinincialSegmentString = tempGlSegments.ToSegmentString();
            }
            else
            {
                if (distributionData.KfsConvertAccount.PpmSegments != null)
                {
                    rtValue.IsPPm = true;
                    var tempPpmSegments = new PpmSegments(distributionData.KfsConvertAccount.PpmSegments);
                    if (string.IsNullOrWhiteSpace(tempPpmSegments.ExpenditureType) || tempPpmSegments.ExpenditureType == "000000")
                    {
                        //770000
                        Log.Warning($"Natural Account (ExpenditureType) of 000000 detected. Substituting {_options.DefaultNaturalAccount}");
                        tempPpmSegments.ExpenditureType = _options.DefaultNaturalAccount;
                    }
                    rtValue.FinincialSegmentString = tempPpmSegments.ToSegmentString();
                }
                else
                {
                    //TODO: REMOVE THIS!!!!
                    Log.Error("No GL Segments found for {chart}-{account}-{subAccount} FAKING IT!!!!", chart, account, split.SubAccount);
                    rtValue.FinincialSegmentString = $"3110-13U02-ADNO006-{_options.DefaultNaturalAccount}-43-000-0000000000-000000-0000-000000-000000";
                }
            }
            return rtValue;
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

        public async Task<Supplier> GetSupplier(WorkgroupVendor vendor)
        {
            if ((string.IsNullOrWhiteSpace(vendor.AeSupplierNumber) || string.IsNullOrWhiteSpace(vendor.AeSupplierSiteCode)) && (vendor == null || vendor.VendorId == null || vendor.VendorAddressTypeCode == null))
            {
                //We have neither new or old values to lookup, can't continue. This should probably be caught earlier.
                return null;
            }
            try
            {
                var rtValue = new Supplier();
                ScmSupplierFilterInput search;
                var updateWorkgroupVendor = false;
                
                if (!string.IsNullOrWhiteSpace(vendor.AeSupplierNumber) && !string.IsNullOrWhiteSpace(vendor.AeSupplierSiteCode))
                {
                    //We only need to do this if we need to validate that it is still eligible for use. We will need to check that
                    //TODO: Validate still good to use.
                    search = new ScmSupplierFilterInput { SupplierNumber = new StringFilterInput { Eq = vendor.AeSupplierNumber } };
                    var searchResult1 = await _aggieClient.ScmSupplierSearch.ExecuteAsync(search);
                    var searchData1 = searchResult1.ReadData();
                    rtValue.SupplierNumber = searchData1.ScmSupplierSearch.Data.FirstOrDefault()?.SupplierNumber.ToString();
                    rtValue.SupplierSiteCode = searchData1.ScmSupplierSearch.Data.FirstOrDefault()?.Sites.Where(a => a.SupplierSiteCode.Equals(vendor.AeSupplierSiteCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.SupplierSiteCode;
                }
                else
                {
                    search = new ScmSupplierFilterInput { SupplierNumber = new StringFilterInput { Eq = vendor.VendorId.ToString() } };
                    var searchResult = await _aggieClient.ScmSupplierSearch.ExecuteAsync(search);
                    var searchData = searchResult.ReadData();

                    rtValue.SupplierNumber = searchData.ScmSupplierSearch.Data.First().SupplierNumber.ToString();

                    rtValue.SupplierSiteCode = searchData.ScmSupplierSearch.Data.First().Sites.Where(a =>
                        a.Location.City.Equals(vendor.City, System.StringComparison.OrdinalIgnoreCase) &&
                        a.Location.State.Equals(vendor.State, System.StringComparison.OrdinalIgnoreCase) &&
                        (a.Location.AddressLine1.Equals(vendor.Name, System.StringComparison.OrdinalIgnoreCase) && a.Location.AddressLine2.Equals(vendor.Line1, System.StringComparison.OrdinalIgnoreCase)) ||
                        (a.Location.AddressLine1.Equals(vendor.Line1, System.StringComparison.OrdinalIgnoreCase))
                        ).First().SupplierSiteCode;

                    updateWorkgroupVendor = true;
                }

             
                
                if (!string.IsNullOrWhiteSpace(rtValue.SupplierNumber) && !string.IsNullOrWhiteSpace(rtValue.SupplierSiteCode))
                {
                    if (updateWorkgroupVendor)
                    {
                        try
                        {
                            var vendorToUpdate = _repositoryFactory.WorkgroupVendorRepository.GetById(vendor.Id);

                            vendorToUpdate.AeSupplierNumber = rtValue.SupplierNumber;
                            vendorToUpdate.AeSupplierSiteCode = rtValue.SupplierSiteCode;
                            _repositoryFactory.WorkgroupVendorRepository.EnsurePersistent(vendorToUpdate);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Error updating WorkgroupVendor {vendorId} with Aggie Enterprise Vendor/Supplier Number {supplierNumber} and Site Code {siteCode}", vendor.VendorId, rtValue.SupplierNumber, rtValue.SupplierSiteCode);
                        }
                    }

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

        public async Task<UnitOfMeasure[]> GetUnitOfMeasures()
        {
            var filter = new ErpUnitOfMeasureFilterInput();
            filter.SearchCommon = new SearchCommonInputs();
            filter.SearchCommon.Limit = 300; //Should only be a few hundred eventually, AE may cap this at 500
            filter.Name = new StringFilterInput { Contains = "%" }; 


            var result = await _aggieClient.ErpUnitOfMeasureSearch.ExecuteAsync(filter);

            var data = result.ReadData();

            if (data.ErpUnitOfMeasureSearch.Metadata.NextStartIndex != null)
            {
                Log.Error("Aggie Enterprise GetUnitOfMeasures has a NextStartIndex.  This is not expected.  Please check the code.");
                throw new Exception("Aggie Enterprise GetUnitOfMeasures has a NextStartIndex.  This is not expected.  Please check the code.");
                //Ok, if this exception ever happens, we just need to do paging and join the results together.
            }

            return data.ErpUnitOfMeasureSearch.Data.Select(a => new UnitOfMeasure
            {
                Name = a.Name,
                Id = a.UomCode,
            }).ToArray();
        }

        public async Task<List<IdAndName>> SearchSupplier(string query)
        {
            var rtValue = new List<IdAndName>();
            if (String.IsNullOrWhiteSpace(query) || query.Trim().Length < 3)
            {
                return rtValue;
            }
            var filter = new ScmSupplierFilterInput();
            filter.SearchCommon = new SearchCommonInputs();
            filter.SearchCommon.Limit = 20;
            filter.Name = new StringFilterInput { Contains = query.Trim().Replace(" ", "%") };

            var result = await _aggieClient.SupplierNameAndNumberSupplierSearch.ExecuteAsync(filter, query.Trim());
            var data = result.ReadData();

            if (data.ScmSupplierByNumber != null)
            {
                rtValue.Add(new IdAndName(
                
                    data.ScmSupplierByNumber.SupplierNumber.ToString(),
                    data.ScmSupplierByNumber.Name
                ));

            }
            if (data.ScmSupplierSearch != null && data.ScmSupplierSearch.Data != null && data.ScmSupplierSearch.Data.Count > 0)
            {
                rtValue.AddRange(data.ScmSupplierSearch.Data.Select(a => new IdAndName(a.SupplierNumber.ToString(), a.Name)));

            }

            if(rtValue.Count > 0)
            {
                rtValue = rtValue.Distinct().ToList();
            }

            return rtValue;
        }

        public async Task<List<IdAndName>> SearchSupplierAddress(string query)
        {
            var filter = new ScmSupplierFilterInput();
            filter.SearchCommon = new SearchCommonInputs();
            filter.SearchCommon.Limit = 5; //Shouldn't really matter, only expect one or two depending if the new eligible field for use gets implemented
            filter.SupplierNumber = new StringFilterInput { Eq = query.Trim() };

            var result = await _aggieClient.ScmSupplierSearch.ExecuteAsync(filter);
            var data = result.ReadData();

            var rtValue = new List<IdAndName>();
            if (data.ScmSupplierSearch != null && data.ScmSupplierSearch.Data != null && data.ScmSupplierSearch.Data.Count > 0 && data.ScmSupplierSearch.Data.First().Sites != null)
            {
                var temp = data.ScmSupplierSearch.Data.First().Sites;

                //We need both PUR and PAY addresses. But only PUR can be used with AE
                rtValue.AddRange(temp.Where(a => a.SupplierSiteCode.StartsWith("PUR")).OrderBy(a => a.SupplierSiteCode).Select(a => new IdAndName(a.SupplierSiteCode, $"({a.SupplierSiteCode}) Name: {a.Location.AddressLine1} Address: {a.Location.AddressLine2} {a.Location.AddressLine3} {a.Location.City} {a.Location.State} {a.Location.PostalCode} {a.Location.CountryCode}")));
                rtValue.AddRange(temp.Where(a => !a.SupplierSiteCode.StartsWith("PUR")).OrderBy(a => a.SupplierSiteCode).Select(a => new IdAndName(a.SupplierSiteCode, $"({a.SupplierSiteCode}) Name: {a.Location.AddressLine1} Address: {a.Location.AddressLine2} {a.Location.AddressLine3} {a.Location.City} {a.Location.State} {a.Location.PostalCode} {a.Location.CountryCode}")));
            }

            return rtValue;
        }

        public async Task<WorkgroupVendor> GetSupplierForWorkgroup(WorkgroupVendor workgroupVendor)
        {
            //TODO add validation here to see if it is still active?
            var filter = new ScmSupplierFilterInput();
            filter.SearchCommon = new SearchCommonInputs();
            filter.SearchCommon.Limit = 5;
            filter.SupplierNumber = new StringFilterInput { Eq = workgroupVendor.AeSupplierNumber };

            var result = await _aggieClient.ScmSupplierSearch.ExecuteAsync(filter);
            var data = result.ReadData();

            if (data.ScmSupplierSearch != null && data.ScmSupplierSearch.Data != null && data.ScmSupplierSearch.Data.Count > 0)
            {
                var supplier = data.ScmSupplierSearch.Data.First();

                var address = supplier.Sites.Where(a => a.SupplierSiteCode == workgroupVendor.AeSupplierSiteCode).FirstOrDefault().Location;

                workgroupVendor.Name        = supplier.Name.SafeTruncate(45);
                workgroupVendor.Line1       = address.AddressLine1.SafeTruncate(40);
                workgroupVendor.Line2       = address.AddressLine2.SafeTruncate(40);
                workgroupVendor.Line3       = address.AddressLine3.SafeTruncate(40);
                workgroupVendor.City        = address.City.SafeTruncate(40);
                workgroupVendor.State       = address.State.SafeTruncate(2);
                workgroupVendor.Zip         = address.PostalCode.SafeTruncate(11);
                workgroupVendor.CountryCode = address.CountryCode.SafeTruncate(2);
            }

            return workgroupVendor;
        }

        public async Task<List<IdAndName>> SearchShippingAddress(string query)
        {
            if (String.IsNullOrWhiteSpace(query) || query.Trim().Length < 3)
            {
                return new List<IdAndName>();
            }
            
            var filter = new ErpInstitutionLocationFilterInput();
            filter.SearchCommon = new SearchCommonInputs();
            filter.SearchCommon.Limit = 25; //More? Less?
            filter.LocationCode = new StringFilterInput { Contains = query.Trim().Replace(" ", "%") };

            var result = await _aggieClient.ErpInstitutionLocationSearch.ExecuteAsync(filter, query.Trim());

            var rtValue = new List<IdAndName>();
            
            var data = result.ReadData();
            if (data.ErpInstitutionLocationByCode != null && data.ErpInstitutionLocationByCode.Enabled)
            {
                //Exact Match first.
                rtValue.Add(new IdAndName(
                    data.ErpInstitutionLocationByCode.LocationCode,
                    $"({data.ErpInstitutionLocationByCode.LocationCode}) {data.ErpInstitutionLocationByCode.AddressLine1} {data.ErpInstitutionLocationByCode.AddressLine2} {data.ErpInstitutionLocationByCode.AddressLine3} {data.ErpInstitutionLocationByCode.City}"
                ));
            }
            if (data.ErpInstitutionLocationSearch != null && data.ErpInstitutionLocationSearch.Data != null && data.ErpInstitutionLocationSearch.Data.Count > 0)
            {
                rtValue.AddRange(data.ErpInstitutionLocationSearch.Data.Where(a => a.Enabled).Select(a => new IdAndName(
                    a.LocationCode,
                    $"({a.LocationCode}) {a.AddressLine1} {a.AddressLine2} {a.AddressLine3} {a.City} {a.State}"
                )));
            }
            rtValue = rtValue.Distinct().ToList();

            return rtValue;
        }

        public async Task<WorkgroupAddress> GetShippingAddress(WorkgroupAddress workgroupAddress)
        {
            if (workgroupAddress == null || String.IsNullOrWhiteSpace(workgroupAddress.AeLocationCode))
            {
                return null;
            }
            var result = await _aggieClient.ErpInstitutionLocationByCode.ExecuteAsync(workgroupAddress.AeLocationCode);
            var data = result.ReadData();
            if( data != null && data.ErpInstitutionLocationByCode != null && data.ErpInstitutionLocationByCode.Enabled)
            {
                var address = data.ErpInstitutionLocationByCode;
                workgroupAddress.Address = address.AddressLine1.SafeTruncate(100);
                workgroupAddress.State   = address.State.SafeTruncate(2);
                workgroupAddress.City    = address.City.SafeTruncate(100);
                workgroupAddress.Zip     = address.PostalCode?.SafeTruncate(10) ?? "95616-5270"; // if they supply it, use it, otherwise use the campus default.

                //Ok, try to parse the locationcode to get a building name and room.
                if (address.LocationCode.Contains("Room", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = address.LocationCode.Split(new string[] { "Room" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        workgroupAddress.Building = parts[0]?.Trim()?.TrimEnd(',').SafeTruncate(50);
                    }
                    if (parts.Length > 1)
                    {
                        workgroupAddress.Room = parts[1].SafeTruncate(50);
                    }
                }
                else
                {
                    workgroupAddress.Building = address.LocationCode.SafeTruncate(50);
                    workgroupAddress.Room = null;
                }

                return workgroupAddress;
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
            public bool IsPPm { get; set; } = false;
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
