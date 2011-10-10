using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using AutoMapper;
using System.Collections.Generic;
using UCDArch.Web.ActionResults;
using Purchasing.Web.Utility;
using MvcContrib;
using UCDArch.Web.Helpers;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Workgroup class
    /// </summary>
    public class WorkgroupController : ApplicationController
    {
	    private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly IHasAccessService _hasAccessService;
        private readonly IDirectorySearchService _searchService;
        private readonly IRepository<WorkgroupVendor> _workgroupVendorRepository;
        private readonly IRepositoryWithTypedId<Vendor, string> _vendorRepository;
        private readonly IRepository<VendorAddress> _vendorAddressRepository;
        private readonly IRepositoryWithTypedId<State, string> _stateRepository;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferencesRepository;

        public WorkgroupController(IRepository<Workgroup> workgroupRepository, 
            IRepositoryWithTypedId<User, string> userRepository, 
            IRepositoryWithTypedId<Role, string> roleRepository, 
            IRepository<WorkgroupPermission> workgroupPermissionRepository,
            IHasAccessService hasAccessService, IDirectorySearchService searchService,
            IRepository<WorkgroupVendor> workgroupVendorRepository, 
            IRepositoryWithTypedId<Vendor, string> vendorRepository, IRepository<VendorAddress> vendorAddressRepository,
            IRepositoryWithTypedId<State, string> stateRepository,
            IRepositoryWithTypedId<EmailPreferences, string> emailPreferencesRepository)
        {
            _workgroupRepository = workgroupRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _workgroupPermissionRepository = workgroupPermissionRepository;
            _hasAccessService = hasAccessService;
            _searchService = searchService;
            _workgroupVendorRepository = workgroupVendorRepository;
            _vendorRepository = vendorRepository;
            _vendorAddressRepository = vendorAddressRepository;
            _stateRepository = stateRepository;
            _emailPreferencesRepository = emailPreferencesRepository;
        }

        #region Workgroup Actions
        /// <summary>
        /// Actions #1
        /// GET: /Workgroup/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var person =
                _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).Single();

            var orgIds = person.Organizations.Select(x => x.Id).ToArray();

            var workgroupList =
                _workgroupRepository.Queryable.Where(x => x.Organizations.Any(a => orgIds.Contains(a.Id)));

            return View(workgroupList.ToList());
        }

        /// <summary>
        /// Actions #2
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();

            var model = WorkgroupModifyModel.Create(user);

            return View(model);
        }

        /// <summary>
        /// Actions #3
        /// </summary>
        /// <param name="workgroup"></param>
        /// <param name="selectedOrganizations"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Workgroup workgroup, string[] selectedOrganizations)
        {
            if (!ModelState.IsValid)
            {
                var model = WorkgroupModifyModel.Create(GetCurrentUser());
                model.Workgroup = workgroup;

                return View(model);
            }

            var workgroupToCreate = new Workgroup();

            Mapper.Map(workgroup, workgroupToCreate);

            if (selectedOrganizations != null)
            {
                workgroupToCreate.Organizations =
                    Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();
            }

            if (!workgroupToCreate.Organizations.Contains(workgroupToCreate.PrimaryOrganization))
            {
                workgroupToCreate.Organizations.Add(workgroupToCreate.PrimaryOrganization);
            }

            _workgroupRepository.EnsurePersistent(workgroupToCreate);

            Message = string.Format("{0} workgroup was created",
                                    workgroup.Name);

            return this.RedirectToAction(a => a.Index());
        }

        /// <summary>
        /// Actions #4
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = WorkgroupDetailsViewModel.Create(_workgroupPermissionRepository, workgroup);

            return View(viewModel);

        }

        public ActionResult Edit(int id)
        {
            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();
            var workgroup = _workgroupRepository.GetNullableById(id);

            var model = WorkgroupModifyModel.Create(user, workgroup);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Workgroup workgroup, string[] selectedOrganizations)
        {

            if (!ModelState.IsValid)
            {
                return View(new WorkgroupModifyModel { Workgroup = workgroup });
            }

            var workgroupToEdit = _workgroupRepository.GetNullableById(workgroup.Id);

            Mapper.Map(workgroup, workgroupToEdit);

            if (selectedOrganizations != null)
            {
                workgroupToEdit.Organizations =
                    Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();
            }

            if (!workgroupToEdit.Organizations.Contains(workgroupToEdit.PrimaryOrganization))
            {
                workgroupToEdit.Organizations.Add(workgroupToEdit.PrimaryOrganization);
            }

            _workgroupRepository.EnsurePersistent(workgroupToEdit);

            Message = string.Format("{0} was modified successfully",
                                    workgroup.Name);

            return RedirectToAction("Index");

        }

        public ActionResult Delete(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            var model = new WorkgroupViewModel
            {
                Workgroup = workgroup
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(WorkgroupViewModel workgroupViewModel)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupViewModel.Workgroup.Id);

            workgroup.IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroup);

            Message = string.Format("{0} was disabled successfully",
                                    workgroup.Name);

            return RedirectToAction("Index");

        }
        #endregion

        #region Workgroup Accounts
        #endregion

        #region Workgroup Vendors
        /// <summary>
        /// GET: Workgroup/Vendor/{workgroup id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult VendorList(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                Message = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index());
            }

            var workgroupVendorList = _workgroupVendorRepository.Queryable.Where(a => a.Workgroup == workgroup && a.IsActive);
            ViewBag.WorkgroupId = id;
            return View(workgroupVendorList.ToList());
        }

        /// <summary>
        /// GET: Workgroup/Vendor/Create/{workgroup id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult CreateVendor(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                Message = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index());
            }

            var viewModel = WorkgroupVendorViewModel.Create(Repository, new WorkgroupVendor() { Workgroup = workgroup });

            return View(viewModel);
        }

        /// <summary>
        /// POST: Workgroup/Vendor/Create/{workgroup id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="workgroupVendor">Workgroup Vendor Object</param>
        /// <param name="newVendor">New Vendor</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateVendor(int id, WorkgroupVendor workgroupVendor, bool newVendor)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                Message = "Unable to locate workgroup.";
                return RedirectToAction("Index");
            }

            var workgroupVendorToCreate = new WorkgroupVendor();

            TransferValues(workgroupVendor, workgroupVendorToCreate);

            workgroupVendorToCreate.Workgroup = workgroup;

            ModelState.Clear();
            workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                Message = "WorkgroupVendor Created Successfully";

                return RedirectToAction("VendorList", new { id = id });
            }

            WorkgroupVendorViewModel viewModel;

            if (!newVendor)
            {
                var vendor = _vendorRepository.GetNullableById(workgroupVendor.VendorId);
                var vendorAddress = _vendorAddressRepository.Queryable.Where(a => a.Vendor == vendor && a.TypeCode == workgroupVendor.VendorAddressTypeCode).FirstOrDefault();
                viewModel = WorkgroupVendorViewModel.Create(Repository, workgroupVendorToCreate, vendor, vendorAddress, newVendor);
            }
            else
            {
                viewModel = WorkgroupVendorViewModel.Create(Repository, workgroupVendorToCreate, newVendor: true);
            }


            return View(viewModel);
        }

        /// <summary>
        /// GET: Workgroup/EditWorkgroupVendor/{workgroup vendor id}
        /// </summary>
        /// <remarks>Only allow editing of non-kfs workgroup vendors</remarks>
        /// <param name="id">Workgroup Vendor Id</param>
        /// <returns></returns>
        public ActionResult EditWorkgroupVendor(int id)
        {
            var workgroupVendor = _workgroupVendorRepository.GetNullableById(id);

            if (workgroupVendor == null) return RedirectToAction("Index");

            if (!string.IsNullOrWhiteSpace(workgroupVendor.VendorId) && !string.IsNullOrWhiteSpace(workgroupVendor.VendorAddressTypeCode))
            {
                Message = "Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.";
                return RedirectToAction("VendorList", new { id = workgroupVendor.Workgroup.Id });
            }

            var viewModel = WorkgroupVendorViewModel.Create(Repository, workgroupVendor);
            return View(viewModel);
        }

        /// <summary>
        /// POST: Workgroup/EditWorkgroupVendor/{workgroup vendor id}
        /// </summary>
        /// <remarks>Only allow editing of non-kfs workgroup vendors</remarks>
        /// <param name="id"></param>
        /// <param name="workgroupVendor"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditWorkgroupVendor(int id, WorkgroupVendor workgroupVendor)
        {
            var oldWorkgroupVendor = _workgroupVendorRepository.GetNullableById(id);

            if (oldWorkgroupVendor == null) return RedirectToAction("Index");

            if (!string.IsNullOrWhiteSpace(oldWorkgroupVendor.VendorId) && !string.IsNullOrWhiteSpace(oldWorkgroupVendor.VendorAddressTypeCode))
            {
                Message = "Cannot edit KFS Vendors.  Please delete and add a new vendor.";
                return RedirectToAction("VendorList", new { id = oldWorkgroupVendor.Workgroup.Id });
            }

            var newWorkgroupVendor = new WorkgroupVendor();
            newWorkgroupVendor.Workgroup = oldWorkgroupVendor.Workgroup;

            TransferValues(workgroupVendor, newWorkgroupVendor);
            ModelState.Clear();
            newWorkgroupVendor.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                oldWorkgroupVendor.IsActive = false;
                _workgroupVendorRepository.EnsurePersistent(oldWorkgroupVendor);
                _workgroupVendorRepository.EnsurePersistent(newWorkgroupVendor);

                Message = "WorkgroupVendor Edited Successfully";

                return RedirectToAction("VendorList", new { Id = newWorkgroupVendor.Workgroup.Id });
            }

            var viewModel = WorkgroupVendorViewModel.Create(Repository, newWorkgroupVendor);

            return View(viewModel);
        }

        /// <summary>
        /// GET: Workgroup/DeleteWorkgroupVendor/{workgroup vendor id}
        /// </summary>
        /// <param name="id">Workgroup Vendor Id</param>
        /// <returns></returns>
        public ActionResult DeleteWorkgroupVendor(int id)
        {
            var workgroupVendor = _workgroupVendorRepository.GetNullableById(id);

            if (workgroupVendor == null) return RedirectToAction("Index");

            return View(workgroupVendor);
        }

        //
        // POST: /WorkgroupVendor/Delete/5
        [HttpPost]
        public ActionResult DeleteWorkgroupVendor(int id, WorkgroupVendor workgroupVendor)
        {
            var workgroupVendorToDelete = _workgroupVendorRepository.GetNullableById(id);

            if (workgroupVendorToDelete == null) return RedirectToAction("Index");

            workgroupVendorToDelete.IsActive = false;

            _workgroupVendorRepository.EnsurePersistent(workgroupVendorToDelete);

            Message = "WorkgroupVendor Removed Successfully";

            return RedirectToAction("VendorList", new { id = workgroupVendorToDelete.Workgroup.Id });
        }

        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private void TransferValues(WorkgroupVendor source, WorkgroupVendor destination)
        {
            Mapper.Map(source, destination);

            // existing vendor, set the values
            if (!string.IsNullOrWhiteSpace(source.VendorId) && !string.IsNullOrWhiteSpace(source.VendorAddressTypeCode))
            {
                var vendor = _vendorRepository.GetNullableById(source.VendorId);
                var vendorAddress = _vendorAddressRepository.Queryable.Where(a => a.Vendor == vendor && a.TypeCode == source.VendorAddressTypeCode).FirstOrDefault();

                if (vendor != null && vendorAddress != null)
                {
                    destination.Name = vendor.Name;
                    destination.Line1 = vendorAddress.Line1;
                    destination.Line2 = vendorAddress.Line2;
                    destination.Line3 = vendorAddress.Line3;
                    destination.City = vendorAddress.City;
                    destination.State = vendorAddress.State;
                    destination.Zip = vendorAddress.Zip;
                    destination.CountryCode = vendorAddress.CountryCode;
                }
            }
        }

        /// <summary>
        /// Ajax action for retreiving kfs vendor addresses
        /// </summary>
        /// <returns></returns>
        public JsonNetResult GetVendorAddresses(string vendorId)
        {
            var vendorAddresses = _vendorAddressRepository.Queryable.Where(a => a.Vendor.Id == vendorId).ToList();

            var results = vendorAddresses.Select(a => new { TypeCode = a.TypeCode, Name = string.Format("{0} ({1}, {2}, {3} {4})", a.Name, a.Line1, a.City, a.State, a.Zip) }).ToList();

            return new JsonNetResult(results);
        }
        #endregion

        #region Addresses
        public ActionResult Addresses(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var viewModel = WorkgroupAddressListModel.Create(workgroup);
            return View(viewModel);
        }

        public ActionResult AddAddress(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
            viewModel.WorkgroupAddress = new WorkgroupAddress();
            viewModel.WorkgroupAddress.Workgroup = workgroup;
            return View(viewModel);

        }

        [HttpPost]
        public ActionResult AddAddress(int id, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            workgroupAddress.Workgroup = workgroup;
            ModelState.Clear();
            workgroupAddress.TransferValidationMessagesTo(ModelState);
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Address not valid";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                viewModel.WorkgroupAddress = workgroupAddress;
                viewModel.WorkgroupAddress.Workgroup = workgroup;
                return View(viewModel);
            }
            var matchFound = 0;
            foreach (var address in workgroup.Addresses)
            {
                matchFound = CompareAddress(workgroupAddress, address);
                if (matchFound > 0)
                {
                    break;
                }
            }
            if (matchFound > 0)
            {
                var matchedAddress = workgroup.Addresses.Where(a => a.Id == matchFound).Single();
                if (!matchedAddress.IsActive)
                {
                    Message = "Address created.";
                    matchedAddress.IsActive = true;
                    _workgroupRepository.EnsurePersistent(workgroup);
                }
                else
                {
                    Message = "This Address already exists.";
                }
            }
            else
            {
                Message = "Address created";
                workgroup.AddAddress(workgroupAddress);
                _workgroupRepository.EnsurePersistent(workgroup);
            }
            return this.RedirectToAction(a => a.Addresses(id));
        }

        /// <summary>
        /// compare addresses
        /// </summary>
        /// <param name="workgroupAddress">New Address</param>
        /// <param name="address">Existing Address</param>
        /// <returns></returns>
        private static int CompareAddress(WorkgroupAddress workgroupAddress, WorkgroupAddress address)
        {
            int matchFound = address.Id;
            if (workgroupAddress.Address.ToLower() != address.Address.ToLower())
            {
                matchFound = 0;
            }
            if (!string.IsNullOrWhiteSpace(workgroupAddress.Building) && !string.IsNullOrWhiteSpace(address.Building))
            {
                if (workgroupAddress.Building.ToLower() != address.Building.ToLower())
                {
                    matchFound = 0;
                }
            }
            if ((!string.IsNullOrWhiteSpace(workgroupAddress.Building) && string.IsNullOrWhiteSpace(address.Building)) ||
                (string.IsNullOrWhiteSpace(workgroupAddress.Building) && !string.IsNullOrWhiteSpace(address.Building)))
            {
                matchFound = 0;
            }
            if (!string.IsNullOrWhiteSpace(workgroupAddress.Room) && !string.IsNullOrWhiteSpace(address.Room))
            {
                if (workgroupAddress.Room.ToLower() != address.Room.ToLower())
                {
                    matchFound = 0;
                }
            }
            if ((!string.IsNullOrWhiteSpace(workgroupAddress.Room) && string.IsNullOrWhiteSpace(address.Room)) ||
                (string.IsNullOrWhiteSpace(workgroupAddress.Room) && !string.IsNullOrWhiteSpace(address.Room)))
            {
                matchFound = 0;
            }
            if (workgroupAddress.Name.ToLower() != address.Name.ToLower())
            {
                matchFound = 0;
            }
            if (workgroupAddress.City.ToLower() != address.City.ToLower())
            {
                matchFound = 0;
            }
            if (workgroupAddress.State.ToLower() != address.State.ToLower())
            {
                matchFound = 0;
            }
            if (workgroupAddress.Zip.ToLower() != address.Zip.ToLower())
            {
                matchFound = 0;
            }
            if (!string.IsNullOrWhiteSpace(workgroupAddress.Phone) && !string.IsNullOrWhiteSpace(address.Phone))
            {
                if (workgroupAddress.Phone.ToLower() != address.Phone.ToLower())
                {
                    matchFound = 0;
                }
            }
            if ((!string.IsNullOrWhiteSpace(workgroupAddress.Phone) && string.IsNullOrWhiteSpace(address.Phone)) ||
                (string.IsNullOrWhiteSpace(workgroupAddress.Phone) && !string.IsNullOrWhiteSpace(address.Phone)))
            {
                matchFound = 0;
            }
            return matchFound;
        }

        public ActionResult DeleteAddress(int id, int addressId)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var workgroupAddress = workgroup.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository);
            viewModel.WorkgroupAddress = workgroupAddress;
            viewModel.State = _stateRepository.GetNullableById(workgroupAddress.State);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DeleteAddress(int id, int addressId, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var workgroupAddressToDelete = workgroup.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (workgroupAddressToDelete == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            workgroupAddressToDelete.IsActive = false;
            _workgroupRepository.EnsurePersistent(workgroup);
            Message = "Address deleted.";
            return this.RedirectToAction(a => a.Addresses(id));
        }

        public ActionResult AddressDetails(int id, int addressId)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var workgroupAddress = workgroup.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository);
            viewModel.WorkgroupAddress = workgroupAddress;
            viewModel.State = _stateRepository.GetNullableById(workgroupAddress.State);
            return View(viewModel);
        }

        public ActionResult EditAddress(int id, int addressId)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var workgroupAddress = workgroup.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
            viewModel.WorkgroupAddress = workgroupAddress;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditAddress(int id, int addressId, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var workgroupAddressToEdit = workgroup.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (workgroupAddressToEdit == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }

            workgroupAddress.Workgroup = workgroup;
            ModelState.Clear();
            workgroupAddress.TransferValidationMessagesTo(ModelState);
            if(!ModelState.IsValid)
            {
                ErrorMessage = "Unable to save due to errors.";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                viewModel.WorkgroupAddress = workgroupAddress;
                return View(viewModel);
            }

            if(CompareAddress(workgroupAddress, workgroup.Addresses.Where(a => a.Id == addressId).Single()) > 0)
            {
                Message = "No changes made";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                viewModel.WorkgroupAddress = workgroupAddress;
                return View(viewModel);
            }


            foreach (var activeAddress in workgroup.Addresses.Where(a => a.IsActive && a.Id != addressId))
            {
                var activeMatchFound = CompareAddress(workgroupAddress, activeAddress);
                if(activeMatchFound > 0)
                {
                    ErrorMessage = "The address you are changing this to already exists. Unable to save.";
                    var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                    viewModel.WorkgroupAddress = workgroupAddress;
                    return View(viewModel);
                }
            }

            var matchFound = 0;
            foreach(var activeAddress in workgroup.Addresses.Where(a => !a.IsActive && a.Id != addressId))
            {
                matchFound = CompareAddress(workgroupAddress, activeAddress);
                if(matchFound > 0)
                {
                    break;
                }
            }

            if(matchFound > 0)
            {
                Message = "Address updated";
                workgroup.Addresses.Where(a => a.Id == matchFound).Single().IsActive = true;                
            }
            else
            {
                Message = "Address updated.";
                var newAddress = new WorkgroupAddress();
                Mapper.Map(workgroupAddress, newAddress);
                newAddress.Workgroup = workgroup;
                workgroup.AddAddress(newAddress);
            }
            workgroup.Addresses.Where(a => a.Id == addressId).Single().IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroup);
            return this.RedirectToAction(a => a.Addresses(id));

        }
        #endregion

        #region Workgroup People

        /// <summary>
        /// People #1
        /// List of all people within a workgroup
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        public ActionResult People(int id, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorgroupPeopleListModel.Create(_workgroupPermissionRepository, _roleRepository, workgroup, roleFilter);
            ViewBag.rolefilter = roleFilter;
            return View(viewModel);
        }

        /// <summary>
        /// People #2
        /// GET: add a person with a role into a workgroup
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        public ActionResult AddPeople(int id, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);
            if(!string.IsNullOrWhiteSpace(roleFilter))
            {
                viewModel.Role = _roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4 && a.Id == roleFilter).SingleOrDefault();
            }
            ViewBag.rolefilter = roleFilter;

            return View(viewModel);
        }

        /// <summary>
        /// People #3
        /// POST: add a person with a role into a workgroup
        /// </summary>
        /// <param name="id"></param>
        /// <param name="workgroupPeoplePostModel"></param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPeople(int id, WorkgroupPeoplePostModel workgroupPeoplePostModel, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }


            //Ensure role picked is valid.
            if (workgroupPeoplePostModel.Role != null)
            {
                if(!_roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4 && a.Id == workgroupPeoplePostModel.Role.Id).Any())
                {
                    ModelState.AddModelError("Role", "Invalid Role Selected");
                }

            }

            if (!ModelState.IsValid)
            {
                var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);

                if (workgroupPeoplePostModel.Role != null)
                {
                    viewModel.Role = workgroupPeoplePostModel.Role;
                }
                if (workgroupPeoplePostModel.Users != null && workgroupPeoplePostModel.Users.Count > 0)
                {
                    var users = new List<IdAndName>();
                    foreach (var user in workgroupPeoplePostModel.Users)
                    {
                        var temp = _userRepository.GetNullableById(user);
                        if (temp != null)
                        {
                            users.Add(new IdAndName(temp.Id, temp.FullName));
                        }
                        else
                        {
                            var ldapuser = _searchService.FindUser(user);
                            if (ldapuser != null)
                            {
                                users.Add(new IdAndName(ldapuser.LoginId, string.Format("{0} {1}", ldapuser.FirstName, ldapuser.LastName)));
                            }
                        }
                    }
                    viewModel.Users = users;
                }
                ViewBag.rolefilter = roleFilter;
                return View(viewModel);
            }

            int successCount = 0;
            int failCount = 0;
            foreach (var u in workgroupPeoplePostModel.Users)
            {
                var user = _userRepository.GetNullableById(u);
                if (user == null)
                {
                    var ldapuser = _searchService.FindUser(u);
                    if (ldapuser != null)
                    {
                        user = new User(ldapuser.LoginId);
                        user.Email = ldapuser.EmailAddress;
                        user.FirstName = ldapuser.FirstName;
                        user.LastName = ldapuser.LastName;

                        _userRepository.EnsurePersistent(user);

                        var emailPrefs = new EmailPreferences(user.Id);
                        _emailPreferencesRepository.EnsurePersistent(emailPrefs);

                    }
                }

                if (user == null)
                {
                    //TODO: Do we want to just ignore these? Or report an error to the user?
                    continue;
                }

                if (!_workgroupPermissionRepository.Queryable.Where(a => a.Role == workgroupPeoplePostModel.Role && a.User == user && a.Workgroup == workgroup).Any())
                {
                    var workgroupPermission = new WorkgroupPermission();
                    workgroupPermission.Role = workgroupPeoplePostModel.Role;
                    workgroupPermission.User = _userRepository.GetNullableById(u);
                    workgroupPermission.Workgroup = Repository.OfType<Workgroup>().GetNullableById(id);

                    _workgroupPermissionRepository.EnsurePersistent(workgroupPermission);
                    successCount++;
                }
                else
                {
                    failCount++;
                }

            }

            Message = string.Format("Successfully added {0} people to workgroup as {1}. {2} not added because of duplicated role.", successCount,
                                    workgroupPeoplePostModel.Role.Name, failCount);

            return this.RedirectToAction(a => a.People(id, workgroupPeoplePostModel.Role.Id));

        }

        /// <summary>
        /// People #4
        /// GET: remove a person/role from a workgroup
        /// </summary>
        /// <param name="id">WorkgroupPermission ID</param>
        /// <param name="workgroupid"></param>
        /// <param name="rolefilter"></param>
        /// <returns></returns>
        public ActionResult DeletePeople(int id, int workgroupid, string rolefilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupid);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            var workgroupPermission = _workgroupPermissionRepository.GetNullableById(id);
            if(workgroupPermission == null)
            {
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter));
            }

            if(workgroupPermission.Workgroup != workgroup) //Need this because you might have DA access to a different workgroup 
            {
                Message = "Person does not belong to workgroup.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorkgroupPeopleDeleteModel.Create(_workgroupPermissionRepository, workgroupPermission);

            ViewBag.rolefilter = rolefilter;
            return View(viewModel);

        }

        /// <summary>
        /// People #5
        /// POST: remove a person/role from a workgroup
        /// </summary>
        /// <param name="id"></param>
        /// <param name="workgroupid"></param>
        /// <param name="rolefilter"></param>
        /// <param name="workgroupPermission"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePeople(int id, int workgroupid, string rolefilter, WorkgroupPermission workgroupPermission, string[] roles)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupid);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var workgroupPermissionToDelete = _workgroupPermissionRepository.GetNullableById(id);
            if (workgroupPermissionToDelete == null)
            {
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter));
            }

            if(workgroupPermissionToDelete.Workgroup != workgroup) //Need this because you might have DA access to a different workgroup 
            {
                Message = "Person does not belong to workgroup.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var availableWorkgroupPermissions = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && a.Role.Level >= 1 && a.Role.Level <= 4).ToList();
            if (availableWorkgroupPermissions.Count() == 1)
            {
                // TODO: Check for pending/open orders for this person. Set order to workgroup.
                _workgroupPermissionRepository.Remove(workgroupPermissionToDelete);
                Message = "Person successfully removed from role.";
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter));
            }
            else
            {
                if (roles == null || roles.Count() == 0)
                {
                    Message = "Must select at least 1 role to delete";
                    var viewModel = WorkgroupPeopleDeleteModel.Create(_workgroupPermissionRepository, workgroupPermissionToDelete);

                    ViewBag.rolefilter = rolefilter;
                    return View(viewModel);
                }
                var removedCount = 0;
                foreach (var role in roles)
                {
                    // TODO: Check for pending/open orders for this person. Set order to workgroup.
                    var wp = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && a.Role.Id == role).Single();
                    _workgroupPermissionRepository.Remove(wp);
                    removedCount++;
                }

                Message = string.Format("{0} {1} removed from {2}", removedCount, removedCount == 1 ? "role" : "roles", workgroupPermissionToDelete.User.FullName);
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter));
            }


        }

        #endregion

        #region Private Helpers

        #endregion

        #region Ajax Helpers
        public JsonNetResult SearchOrganizations(string searchTerm)
        {
            var results = Repository.OfType<Organization>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.DisplayNameAndId }));
        }

        /// <summary>
        /// People #6
        /// Search Users in the User Table and LDAP lookup if none found.
        /// </summary>
        /// <param name="searchTerm">Email or LoginId</param>
        /// <returns></returns>
        public JsonNetResult SearchUsers(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if (users.Count == 0)
            {
                var ldapuser = _searchService.FindUser(searchTerm);
                if (ldapuser != null)
                {
                    Check.Require(!string.IsNullOrWhiteSpace(ldapuser.LoginId));
                    Check.Require(!string.IsNullOrWhiteSpace(ldapuser.EmailAddress));

                    var user = new User(ldapuser.LoginId);
                    user.Email = ldapuser.EmailAddress;
                    user.FirstName = ldapuser.FirstName;
                    user.LastName = ldapuser.LastName;

                    users.Add(user);
                }
            }

            //We don't want to show users that are not active
            var results =
                users.Where(a => a.IsActive).Select(a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();
            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }
        #endregion
    }
}
