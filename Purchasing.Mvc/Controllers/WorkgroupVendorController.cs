using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Services;
using Purchasing.Web.Attributes;
using Purchasing.Web.Controllers;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Helpers;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupVendor class
    /// </summary>
    [AuthorizeApplicationAccess]
    public class WorkgroupVendorController : ApplicationController
    {
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<VendorAddress> _vendorAddressRepository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ISearchService _searchService;
        private readonly ISecurityService _securityService;
        private readonly IWorkgroupService _workgroupService;

        public WorkgroupVendorController(IRepository<Vendor> vendorRepository, IRepository<VendorAddress> vendorAddressRepository, IRepositoryFactory repositoryFactory, ISearchService searchService, ISecurityService securityService, IWorkgroupService workgroupService)
        {
            _vendorRepository = vendorRepository;
            _vendorAddressRepository = vendorAddressRepository;
            _repositoryFactory = repositoryFactory;
            _searchService = searchService;
            _securityService = securityService;
            _workgroupService = workgroupService;
        }

        public JsonNetResult SearchVendor(string searchTerm)
        {
            searchTerm = searchTerm.Replace("'", "''");
            var results = _searchService.SearchVendors(searchTerm).ToList();

            return new JsonNetResult(results.Select(a => new {a.Id, Name = string.Format("{0} ({1})", a.Name, a.Id)}));
        }

        public JsonNetResult SearchVendorAddress(string vendorId)
        {
            var results = _vendorAddressRepository.Queryable.Where(a => a.Vendor.Id == vendorId).OrderByDescending(b=> b.IsDefault).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.TypeCode, Name = a.DisplayNameWithDefault }));
        }

        [HttpPost]
        public JsonNetResult AddKfsVendor(int workgroupId, string vendorId, string addressTypeCode)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetById(workgroupId);
            var vendor = _vendorRepository.Queryable.Single(a => a.Id == vendorId);
            var vendorAddress = _vendorAddressRepository.Queryable.First(a => a.Vendor == vendor && a.TypeCode == addressTypeCode);
            var added = true;
            var duplicate = false;
            var wasInactive = false;
            string errorMessage = null;

            // just make sure user has access to the workgroup
            Check.Require(_securityService.HasWorkgroupAccess(workgroup), Resources.NoAccess_Workgroup);

            var workgroupVendor = new WorkgroupVendor
                                      {
                                          Workgroup = workgroup,
                                          VendorId = vendor.Id,
                                          VendorAddressTypeCode = vendorAddress.TypeCode
                                          //,
                                          //Name = vendor.Name,
                                          //Line1 = vendorAddress.Line1,
                                          //Line2 = vendorAddress.Line2,
                                          //Line3 = vendorAddress.Line3,
                                          //City = vendorAddress.City,
                                          //State = vendorAddress.State,
                                          //Zip = vendorAddress.Zip,
                                          //CountryCode = vendorAddress.CountryCode
                                      };
            var workgroupVendorToCreate = new WorkgroupVendor();
            _workgroupService.TransferValues(workgroupVendor, ref workgroupVendorToCreate); //Central location for values that get transferred (Fax, state, etc.)
            workgroupVendorToCreate.Workgroup = workgroup;


            if(!_repositoryFactory.WorkgroupVendorRepository.Queryable
                .Any(a => a.Workgroup.Id == workgroupId &&
                    a.VendorId == workgroupVendorToCreate.VendorId &&
                    a.VendorAddressTypeCode == workgroupVendorToCreate.VendorAddressTypeCode))
            {
                //doesn't find any
                var tempModelState = new ModelStateDictionary();
                workgroupVendorToCreate.TransferValidationMessagesTo(tempModelState);
                if (!tempModelState.IsValid)
                {
                    if (tempModelState.ContainsKey("WorkgroupVendor.Email"))
                    {
                        workgroupVendorToCreate.Email = null;
                        errorMessage = "Warning, Email removed. KFS Vendor's Email was invalid";
                    }
                }

                try
                {
                    _repositoryFactory.WorkgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);
                }
                catch (Exception)
                {
                    errorMessage = "An Error occurred while trying to add that vendor to your workgroup.";
                }
                
                added = true;
                duplicate = false;
                wasInactive = false;
            }
            else
            {
                //found one
                var inactiveVendor = _repositoryFactory.WorkgroupVendorRepository.Queryable
                    .FirstOrDefault(a => a.Workgroup.Id == workgroupId &&
                                         a.VendorId == workgroupVendorToCreate.VendorId &&
                                         a.VendorAddressTypeCode == workgroupVendorToCreate.VendorAddressTypeCode &&
                                         !a.IsActive);
                if(inactiveVendor != null)
                {
                    // we found one this is disabled, activate it and return it
                    inactiveVendor.IsActive = true;
                    _repositoryFactory.WorkgroupVendorRepository.EnsurePersistent(inactiveVendor);
                    added = false;
                    duplicate = false;
                    wasInactive = true;
                    return new JsonNetResult(new { id = inactiveVendor.Id, name = inactiveVendor.Name, added, duplicate, wasInactive, errorMessage });
                }
                // there was an active duplicate, return the first one.
                workgroupVendorToCreate = _repositoryFactory.WorkgroupVendorRepository.Queryable
                    .First(a => a.Workgroup.Id == workgroupId &&
                                         a.VendorId == workgroupVendorToCreate.VendorId &&
                                         a.VendorAddressTypeCode == workgroupVendorToCreate.VendorAddressTypeCode &&
                                         a.IsActive);
                added = false;
                duplicate = true;
                wasInactive = false;
            }

            return new JsonNetResult(new { id = workgroupVendorToCreate.Id, name = workgroupVendorToCreate.Name, added, duplicate, wasInactive, errorMessage });
        }

    }
}