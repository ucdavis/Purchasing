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
                _workgroupPermissionRepository.Queryable.Where(
                    x => x.Role.Id == Role.Codes.DepartmentalAdmin && x.User.Id == CurrentUser.Identity.Name).Select(x=>x.Id).ToList();

            //Get the orgs
            var orgIds =
                _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).
                    Single().Organizations.Select(x=>x.Id).ToList();

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
    }

    public class ConditionalApprovalViewModel
    {
        public IList<ConditionalApproval> ConditionalApprovalsForWorkgroups { get; set; }
        public IList<ConditionalApproval> ConditionalApprovalsForOrgs { get; set; }
    }
}
