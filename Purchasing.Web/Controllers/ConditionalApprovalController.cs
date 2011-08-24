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
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepository<User> _userRepository;

        public ConditionalApprovalController(IRepository<ConditionalApproval> conditionalApprovalRepository, IRepository<Workgroup> workgroupRepository, IRepository<User> userRepository)
        {
            _conditionalApprovalRepository = conditionalApprovalRepository;
            _workgroupRepository = workgroupRepository;
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
            var user = GetUserWithOrgs();

            var workgroupIds = GetWorkgroups(user).Select(x=>x.Id).ToList();

            var orgIds = user.Organizations.Select(x=>x.Id).ToList();

            //Now get all conditional approvals that exist for those workgroups
            var conditionalApprovalsForWorkgroups =
                _conditionalApprovalRepository.Queryable.Where(x => workgroupIds.Contains(x.Workgroup.Id));

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

            var userWithOrgs = GetUserWithOrgs();

            if (approvalType == "Workgroup")
            {
                model.Workgroups = GetWorkgroups(userWithOrgs).ToList();
            }
            else if (approvalType == "Organization")
            {
                model.Organizations = userWithOrgs.Organizations;
            }

            model.ConditionalApproval = new ConditionalApproval();

            return View(model);
        }

        private IQueryable<Workgroup>  GetWorkgroups(User user)
        {
            var orgIds = user.Organizations.Select(x => x.Id).ToArray();

            return _workgroupRepository.Queryable.Where(x => x.Organizations.Any(a => orgIds.Contains(a.Id)));

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
