using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using MvcContrib;
using UCDArch.Web.Helpers;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupManagement class
    /// </summary>
    public class JandJWorkgroupManagementController : ApplicationController
    {
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IDirectorySearchService _searchService;
        private readonly IRepository<WorkgroupAddress> _workgroupAddressRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly ISecurityService _securityService;
        private readonly IRepositoryWithTypedId<State, string> _stateRepository;



        public JandJWorkgroupManagementController(IRepository<Workgroup> workgroupRepository, IDirectorySearchService searchService, IRepository<WorkgroupAddress> workgroupAddressRepository, IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<Role, string> roleRepository, IRepository<WorkgroupPermission> workgroupPermission, ISecurityService  securityService, IRepositoryWithTypedId<State,string> stateRepository  )
        {
            _workgroupRepository = workgroupRepository;
            _searchService = searchService;
            _workgroupAddressRepository = workgroupAddressRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _workgroupPermissionRepository = workgroupPermission;
            _securityService = securityService;
            _stateRepository = stateRepository;

        }

        //
        // GET: /WorkgroupManagement/
        /// <summary>
        /// TODO: For testing only!
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var workgroups = _workgroupRepository.Queryable;

            return View(workgroups.ToList());
        }




        public ActionResult Manage(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return RedirectToAction("Index");
            }

            var workgroupPermsByGroup = (from wp in Repository.OfType<WorkgroupPermission>().Queryable
                                         where wp.Workgroup.Id == workgroup.Id
                                         group wp.Role by wp.Role.Id
                                             into role
                                             select new { count = role.Count(), name = role.Key }).ToList();

            var model = new WorkgroupManageModel
                            {
                                Workgroup = workgroup,
                                OrganizationCount = workgroup.Organizations.Count(),
                                AccountCount = workgroup.Accounts.Count(),
                                VendorCount = workgroup.Vendors.Count(),
                                AddressCount = workgroup.Addresses.Count(),
                                UserCount =
                                    workgroupPermsByGroup.Where(x => x.name == Role.Codes.Requester).Select(x => x.count).
                                    SingleOrDefault(),
                                ApproverCount =
                                    workgroupPermsByGroup.Where(x => x.name == Role.Codes.Approver).Select(x => x.count)
                                    .SingleOrDefault(),
                                AccountManagerCount =
                                    workgroupPermsByGroup.Where(x => x.name == Role.Codes.AccountManager).Select(
                                        x => x.count).SingleOrDefault(),
                                PurchaserCount =
                                    workgroupPermsByGroup.Where(x => x.name == Role.Codes.Purchaser).Select(x => x.count)
                                    .SingleOrDefault()
                            };

            return View(model);
        }

        public ActionResult Accounts(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult Vendors(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult TestCreate()
        {
            return View(new User());
        }

        [HttpPost]
        public ActionResult TestCreate(User user)
        {
            if(!ModelState.IsValid)
            {
                ErrorMessage = "User is not Valid";
                return View(user);
            }
            return this.RedirectToAction(a => a.TestCreate());
        }



        public  JsonNetResult FindUser(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if(users.Count == 0)
            {
                var ldapuser = _searchService.FindUser(searchTerm);
                if(ldapuser != null)
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

            if(users.Count() == 0)
            {
                return null;
            }
            return new JsonNetResult(users.Select(a => new{id=a.Id, FirstName=a.FirstName, LastName=a.LastName, Email=a.Email, IsActive=a.IsActive}));
        }

        //public ActionResult Addresses(int id)
        //{
        //    var workgroup =
        //        _workgroupRepository.Queryable.Where(x => x.Id == id).Fetch(x => x.Addresses).SingleOrDefault();

        //    if (workgroup == null)
        //    {
        //        ErrorMessage = "Workgroup could not be found";
        //        return RedirectToAction("Index");
        //    }

        //    return View(workgroup);
        //}

        #region Workgroup Address

        //Moved to Workgroup Controller
        #endregion Workgroup Address


        
    }

    //public class WorkgroupManageModel
    //{
    //    public Workgroup Workgroup { get; set; }

    //    public virtual int OrganizationCount { get; set; }
    //    public virtual int AccountCount { get; set; }
    //    public virtual int VendorCount { get; set; }
    //    public virtual int AddressCount { get; set; }
    //    public virtual int UserCount { get; set; }
    //    public virtual int ApproverCount { get; set; }
    //    public virtual int AccountManagerCount { get; set; }
    //    public virtual int PurchaserCount { get; set; }
    //}


    public class xFilteredOrderListModel
    {
        public List<OrderStatusCode> OrderStatusCodes { get; set; }
        public List<Order> Orders { get; set; }
        public List<string> CheckedOrderStatusCodes { get; set; } 

        public static xFilteredOrderListModel Create(IRepository repository, List<Order> orders)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new xFilteredOrderListModel
            {
                Orders = orders
            };
            viewModel.OrderStatusCodes = repository.OfType<OrderStatusCode>().Queryable.ToList();
            viewModel.CheckedOrderStatusCodes = new List<string>();
            
            return viewModel;
        }
    }



}
