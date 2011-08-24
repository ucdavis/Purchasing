using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the ConditionalApproval class
    /// </summary>
    [Authorize(Roles=Role.Codes.DepartmentalAdmin)] //Must be a departmental admin to modify conditional approvals
    public class ConditionalApprovalController : ApplicationController
    {
	    private readonly IRepository<ConditionalApproval> _conditionalApprovalRepository;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly IRepository<User> _userRepository;

        public ConditionalApprovalController(IRepository<ConditionalApproval> conditionalApprovalRepository, IRepository<WorkgroupPermission> workgroupPermissionRepository, IRepository<User> userRepository)
        {
            _conditionalApprovalRepository = conditionalApprovalRepository;
            _workgroupPermissionRepository = workgroupPermissionRepository;
            _userRepository = userRepository;
        }

        //
        // GET: /ConditionalApproval/
        /// <summary>
        /// Returns the conditional approvals relating to your workgroups and departments
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //Get the workgroups the current user is a departmental admin in
            var userWorkgroupIds =
                GetCurrentWorkgroupPermissions().Select(x=>x.Workgroup.Id).ToList();

            //Get the orgs
            var orgIds = GetUserWithOrgs().Organizations.Select(x=>x.Id).ToList();

            //Now get all conditional approvals that exist for those workgroups
            var conditionalApprovalsForWorkgroups =
                _conditionalApprovalRepository.Queryable.Where(x => userWorkgroupIds.Contains(x.Workgroup.Id));

            var conditionalApprovalsForOrgs =
                _conditionalApprovalRepository.Queryable.Where(x => orgIds.Contains(x.Organization.Id));

            var model = new ConditionalApprovalViewModel
                            {
                                ConditionalApprovalsForOrgs = conditionalApprovalsForOrgs.ToList(),
                                ConditionalApprovalsForWorkgroups = conditionalApprovalsForWorkgroups.ToList()
                            };
            
            return View(model);
        }

        public ActionResult Create(string approvalType)
        {
            var model = new ConditionalApprovalModifyModel {ApprovalType = approvalType};

            if (approvalType == "Workgroup")
            {
                model.Workgroups = GetCurrentWorkgroupPermissions().Select(x => x.Workgroup).ToList();
            }
            else if (approvalType == "Organization")
            {
                model.Organizations = GetUserWithOrgs().Organizations;
            }

            model.ConditionalApproval = new ConditionalApproval();

            return View(model);
        }

        private IQueryable<WorkgroupPermission>  GetCurrentWorkgroupPermissions()
        {
            return _workgroupPermissionRepository.Queryable.Where(
                x => x.Role.Id == Role.Codes.DepartmentalAdmin && x.User.Id == CurrentUser.Identity.Name);
        }

        private User GetUserWithOrgs()
        {
            return
                _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).
                    Single();
        }
    }

    public class ConditionalApprovalModifyModel
    {
        public virtual IList<Workgroup> Workgroups { get; set; }
        public virtual IList<Organization> Organizations { get; set; }
        public virtual ConditionalApproval ConditionalApproval { get; set; }
        public virtual string ApprovalType { get; set; }
    }

    public class ConditionalApprovalViewModel
    {
        public IList<ConditionalApproval> ConditionalApprovalsForWorkgroups { get; set; }
        public IList<ConditionalApproval> ConditionalApprovalsForOrgs { get; set; }
    }
}
