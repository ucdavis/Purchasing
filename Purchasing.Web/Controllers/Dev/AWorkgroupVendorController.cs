/*
 * This whole controller was merged into the workgroup controller on 10/5/2011
 **/



//using System;
//using System.Linq;
//using System.Web.Helpers;
//using System.Web.Mvc;
//using AutoMapper;
//using Purchasing.Core.Domain;
//using Purchasing.Web.Models;
//using UCDArch.Core.PersistanceSupport;
//using MvcContrib;
//using UCDArch.Web.ActionResults;
//using UCDArch.Web.Helpers;

//namespace Purchasing.Web.Controllers
//{
//    /// <summary>
//    /// Controller for the WorkgroupVendor class
//    /// </summary>
//    public class AWorkgroupVendorController : ApplicationController
//    {
//        private readonly IRepository<WorkgroupVendor> _workgroupVendorRepository;
//        private readonly IRepository<Workgroup> _workgroupRepository;
//        private readonly IRepositoryWithTypedId<Vendor, string> _vendorRepository;
//        private readonly IRepository<VendorAddress> _vendorAddressRepository;

//        public AWorkgroupVendorController(IRepository<WorkgroupVendor> workgroupVendorRepository, IRepository<Workgroup> workgroupRepository, IRepositoryWithTypedId<Vendor, string> vendorRepository, IRepository<VendorAddress> vendorAddressRepository  )
//        {
//            _workgroupVendorRepository = workgroupVendorRepository;
//            _workgroupRepository = workgroupRepository;
//            _vendorRepository = vendorRepository;
//            _vendorAddressRepository = vendorAddressRepository;
//        }

//        public ActionResult Index()
//        {
//            return View();
//        }

//        /// <summary>
//        /// GET: Workgroup/Vendor/{workgroup id}
//        /// </summary>
//        /// <param name="id">Workgroup Id</param>
//        /// <returns></returns>
//        public ActionResult VendorList(int id)
//        {
//            var workgroup = _workgroupRepository.GetNullableById(id);

//            if (workgroup == null)
//            {
//                Message = "Workgroup could not be found.";
//                return this.RedirectToAction<WorkgroupController>(a => a.Index());
//            }

//            var workgroupVendorList = _workgroupVendorRepository.Queryable.Where(a => a.Workgroup == workgroup && a.IsActive);
//            ViewBag.WorkgroupId = id;
//            return View(workgroupVendorList.ToList());
//        }

//        //
//        // GET: /WorkgroupVendor/Details/5
//        public ActionResult Details(int id)
//        {
//            var workgroupVendor = _workgroupVendorRepository.GetNullableById(id);

//            if (workgroupVendor == null) return RedirectToAction("Index");

//            return View(workgroupVendor);
//        }

//        /// <summary>
//        /// GET: Workgroup/Vendor/Create/{workgroup id}
//        /// </summary>
//        /// <param name="id">Workgroup Id</param>
//        /// <returns></returns>
//        public ActionResult CreateVendor(int id)
//        {
//            var workgroup = _workgroupRepository.GetNullableById(id);

//            if (workgroup == null)
//            {
//                Message = "Workgroup could not be found.";
//                return this.RedirectToAction<WorkgroupController>(a => a.Index());
//            }

//            var viewModel = WorkgroupVendorViewModel.Create(Repository, new WorkgroupVendor(){Workgroup = workgroup});
            
//            return View(viewModel);
//        } 

//        /// <summary>
//        /// POST: Workgroup/Vendor/Create/{workgroup id}
//        /// </summary>
//        /// <param name="id">Workgroup Id</param>
//        /// <param name="workgroupVendor">Workgroup Vendor Object</param>
//        /// <param name="newVendor">New Vendor</param>
//        /// <returns></returns>
//        [HttpPost]
//        public ActionResult CreateVendor(int id, WorkgroupVendor workgroupVendor, bool newVendor)
//        {
//            var workgroup = _workgroupRepository.GetNullableById(id);

//            if (workgroup == null)
//            {
//                Message = "Unable to locate workgroup.";
//                return RedirectToAction("Index");
//            }
            
//            var workgroupVendorToCreate = new WorkgroupVendor();

//            TransferValues(workgroupVendor, workgroupVendorToCreate);

//            workgroupVendorToCreate.Workgroup = workgroup;

//            ModelState.Clear();
//            workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);

//            if (ModelState.IsValid)
//            {
//                _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

//                Message = "WorkgroupVendor Created Successfully";

//                return RedirectToAction("VendorList", new {id=id});
//            }

//            WorkgroupVendorViewModel viewModel;

//            if (!newVendor)
//            {
//                var vendor = _vendorRepository.GetNullableById(workgroupVendor.VendorId);
//                var vendorAddress = _vendorAddressRepository.Queryable.Where(a => a.Vendor == vendor && a.TypeCode == workgroupVendor.VendorAddressTypeCode).FirstOrDefault();
//                viewModel = WorkgroupVendorViewModel.Create(Repository, workgroupVendorToCreate, vendor, vendorAddress, newVendor);
//            }
//            else
//            {
//                viewModel = WorkgroupVendorViewModel.Create(Repository, workgroupVendorToCreate, newVendor: true);
//            }
            

//            return View(viewModel);
//        }

//        /// <summary>
//        /// GET: Workgroup/EditWorkgroupVendor/{workgroup vendor id}
//        /// </summary>
//        /// <remarks>Only allow editing of non-kfs workgroup vendors</remarks>
//        /// <param name="id">Workgroup Vendor Id</param>
//        /// <returns></returns>
//        public ActionResult EditWorkgroupVendor(int id)
//        {
//            var workgroupVendor = _workgroupVendorRepository.GetNullableById(id);

//            if (workgroupVendor == null) return RedirectToAction("Index");

//            if (!string.IsNullOrWhiteSpace(workgroupVendor.VendorId) && !string.IsNullOrWhiteSpace(workgroupVendor.VendorAddressTypeCode))
//            {
//                Message = "Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.";
//                return RedirectToAction("VendorList", new {id = workgroupVendor.Workgroup.Id});
//            }

//            var viewModel = WorkgroupVendorViewModel.Create(Repository, workgroupVendor);
//            return View(viewModel);
//        }
        
//        /// <summary>
//        /// POST: Workgroup/EditWorkgroupVendor/{workgroup vendor id}
//        /// </summary>
//        /// <remarks>Only allow editing of non-kfs workgroup vendors</remarks>
//        /// <param name="id"></param>
//        /// <param name="workgroupVendor"></param>
//        /// <returns></returns>
//        [HttpPost]
//        public ActionResult EditWorkgroupVendor(int id, WorkgroupVendor workgroupVendor)
//        {
//            var oldWorkgroupVendor = _workgroupVendorRepository.GetNullableById(id);

//            if (oldWorkgroupVendor == null) return RedirectToAction("Index");

//            if (!string.IsNullOrWhiteSpace(oldWorkgroupVendor.VendorId) && !string.IsNullOrWhiteSpace(oldWorkgroupVendor.VendorAddressTypeCode))
//            {
//                Message = "Cannot edit KFS Vendors.  Please delete and add a new vendor.";
//                return RedirectToAction("VendorList", new { id = oldWorkgroupVendor.Workgroup.Id });
//            }

//            var newWorkgroupVendor = new WorkgroupVendor();
//            newWorkgroupVendor.Workgroup = oldWorkgroupVendor.Workgroup;

//            TransferValues(workgroupVendor, newWorkgroupVendor);
//            ModelState.Clear();
//            newWorkgroupVendor.TransferValidationMessagesTo(ModelState);

//            if (ModelState.IsValid)
//            {
//                oldWorkgroupVendor.IsActive = false;
//                _workgroupVendorRepository.EnsurePersistent(oldWorkgroupVendor);
//                _workgroupVendorRepository.EnsurePersistent(newWorkgroupVendor);

//                Message = "WorkgroupVendor Edited Successfully";

//                return RedirectToAction("VendorList", new {Id=newWorkgroupVendor.Workgroup.Id});
//            }
            
//            var viewModel = WorkgroupVendorViewModel.Create(Repository, newWorkgroupVendor);

//            return View(viewModel);
//        }
        
//        /// <summary>
//        /// GET: Workgroup/DeleteWorkgroupVendor/{workgroup vendor id}
//        /// </summary>
//        /// <param name="id">Workgroup Vendor Id</param>
//        /// <returns></returns>
//        public ActionResult DeleteWorkgroupVendor(int id)
//        {
//            var workgroupVendor = _workgroupVendorRepository.GetNullableById(id);

//            if (workgroupVendor == null) return RedirectToAction("Index");

//            return View(workgroupVendor);
//        }

//        //
//        // POST: /WorkgroupVendor/Delete/5
//        [HttpPost]
//        public ActionResult DeleteWorkgroupVendor(int id, WorkgroupVendor workgroupVendor)
//        {
//            var workgroupVendorToDelete = _workgroupVendorRepository.GetNullableById(id);

//            if (workgroupVendorToDelete == null) return RedirectToAction("Index");

//            workgroupVendorToDelete.IsActive = false;

//            _workgroupVendorRepository.EnsurePersistent(workgroupVendorToDelete);

//            Message = "WorkgroupVendor Removed Successfully";

//            return RedirectToAction("VendorList", new {id=workgroupVendorToDelete.Workgroup.Id});
//        }
        
//        /// <summary>
//        /// Transfer editable values from source to destination
//        /// </summary>
//        private void TransferValues(WorkgroupVendor source, WorkgroupVendor destination)
//        {
//            Mapper.Map(source, destination);

//            // existing vendor, set the values
//            if (!string.IsNullOrWhiteSpace(source.VendorId) && !string.IsNullOrWhiteSpace(source.VendorAddressTypeCode))
//            {
//                var vendor = _vendorRepository.GetNullableById(source.VendorId);
//                var vendorAddress = _vendorAddressRepository.Queryable.Where(a => a.Vendor == vendor && a.TypeCode == source.VendorAddressTypeCode).FirstOrDefault();

//                if (vendor != null && vendorAddress != null)
//                {
//                    destination.Name = vendor.Name;
//                    destination.Line1 = vendorAddress.Line1;
//                    destination.Line2 = vendorAddress.Line2;
//                    destination.Line3 = vendorAddress.Line3;
//                    destination.City = vendorAddress.City;
//                    destination.State = vendorAddress.State;
//                    destination.Zip = vendorAddress.Zip;
//                    destination.CountryCode = vendorAddress.CountryCode;
//                }
//            }
//        }

//        /// <summary>
//        /// Ajax action for retreiving kfs vendor addresses
//        /// </summary>
//        /// <returns></returns>
//        public JsonNetResult GetVendorAddresses(string vendorId)
//        {
//            var vendorAddresses = _vendorAddressRepository.Queryable.Where(a => a.Vendor.Id == vendorId).ToList();
                
//            var results = vendorAddresses.Select(a => new { TypeCode = a.TypeCode, Name = string.Format("{0} ({1}, {2}, {3} {4})", a.Name, a.Line1, a.City, a.State, a.Zip)}).ToList();

//            return new JsonNetResult(results);
//        }
//    }
//}
