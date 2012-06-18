using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Purchasing.Web.Services;
using MvcContrib;

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
        private readonly IRepositoryWithTypedId<User,string> _userRepository;
        private readonly IDirectorySearchService _directorySearchService;
        private readonly ISecurityService _securityService;
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository; 
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        public const string WorkgroupType = "Workgroup";
        public const string OrganizationType = "Organization";

        public ConditionalApprovalController(
            IRepository<ConditionalApproval> conditionalApprovalRepository, 
            IRepository<Workgroup> workgroupRepository, 
            IRepositoryWithTypedId<User,string> userRepository, 
            IRepositoryWithTypedId<Organization, string> organizationRepository, 
            IDirectorySearchService directorySearchService, 
            ISecurityService securityService, 
            IQueryRepositoryFactory queryRepositoryFactory)
        {
            _conditionalApprovalRepository = conditionalApprovalRepository;
            _workgroupRepository = workgroupRepository;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _directorySearchService = directorySearchService;
            _securityService = securityService;
            _queryRepositoryFactory = queryRepositoryFactory;
        }

        /// <summary>
        /// Conditional approvals by org
        /// Test 1A
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ByOrg(string id)
        {
            var conditionalApprovals = _conditionalApprovalRepository.Queryable.Where(x => x.Organization.Id == id);

            ViewBag.OrganizationId = id;

            return View(conditionalApprovals);
        }

        /// <summary>
        /// Conditional approvals by workgroup
        /// Test 1B
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ByWorkgroup(int id)
        {
            var conditionalApprovals = _conditionalApprovalRepository.Queryable.Where(x => x.Workgroup.Id == id);

            ViewBag.WorkgroupId = id;

            return View(conditionalApprovals);
        }

        /// <summary>
        /// #2
        /// GET: /ConditionalApproval/Delete/
        /// </summary>
        /// <param name="id">ConditionalApproval Id</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            ActionResult redirectToAction;
            var conditionalApproval = GetConditionalApprovalAndCheckAccess(id, out redirectToAction);
            if(conditionalApproval == null)
            {
                return redirectToAction;
            }

            if(conditionalApproval.Workgroup != null)
            {
                ViewBag.WorkgroupId = conditionalApproval.Workgroup.Id;
                ViewBag.IsWorkgroup = true;
            }
            else
            {
                ViewBag.IsWorkgroup = false;
                ViewBag.OrganizationId = conditionalApproval.Organization.Id;
            }


            var model = new ConditionalApprovalViewModel
            {
                Id = conditionalApproval.Id,
                Question = conditionalApproval.Question,
                OrgOrWorkgroupName = conditionalApproval.Organization == null ? conditionalApproval.Workgroup.Name : conditionalApproval.Organization.Name,
                PrimaryUserName = conditionalApproval.PrimaryApprover.FullNameAndId,
                SecondaryUserName =
                    conditionalApproval.SecondaryApprover == null
                        ? string.Empty
                        : conditionalApproval.SecondaryApprover.FullNameAndId
            };

            return View(model);
        }
        
        /// <summary>
        /// #3 
        /// POST: /ConditionalApproval/Delete/
        /// </summary>
        /// <param name="conditionalApprovalViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(ConditionalApprovalViewModel conditionalApprovalViewModel)
        {
            ActionResult redirectToAction;
            var conditionalApproval = GetConditionalApprovalAndCheckAccess(conditionalApprovalViewModel.Id, out redirectToAction);
            if(conditionalApproval == null)
            {
                return redirectToAction;
            }

            // save the values for redirection
            int? workgroupId = conditionalApproval.Workgroup != null ? (int?) conditionalApproval.Workgroup.Id : null;
            var orgId = conditionalApproval.Organization != null ? conditionalApproval.Organization.Id : null;

            _conditionalApprovalRepository.Remove(conditionalApproval);

            Message = "Conditional Approval removed successfully";

            if (workgroupId.HasValue)
            {
                return this.RedirectToAction(a => a.ByWorkgroup(workgroupId.Value));
            }

            if (!string.IsNullOrWhiteSpace(orgId))
            {
                return this.RedirectToAction(a => a.ByOrg(orgId));
            }

            //return this.RedirectToAction(a => a.Index());
            return this.RedirectToAction<ErrorController>(a => a.Index());
        }

        /// <summary>
        /// #4
        /// GET: /ConditionalApproval/Edit/
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            ActionResult redirectToAction;
            var conditionalApproval = GetConditionalApprovalAndCheckAccess(id, out redirectToAction, extraFetch:true);
            if(conditionalApproval == null)
            {
                return redirectToAction;
            }

            if(conditionalApproval.Workgroup != null)
            {
                ViewBag.WorkgroupId = conditionalApproval.Workgroup.Id;
                ViewBag.IsWorkgroup = true;
            }
            else
            {
                ViewBag.IsWorkgroup = false;
                ViewBag.OrganizationId = conditionalApproval.Organization.Id;
            }

            var model = new ConditionalApprovalViewModel
                            {
                                Id = conditionalApproval.Id,
                                Question = conditionalApproval.Question,
                                OrgOrWorkgroupName = conditionalApproval.Organization == null ? conditionalApproval.Workgroup.Name : conditionalApproval.Organization.Name,
                                PrimaryUserName = conditionalApproval.PrimaryApprover.FullNameAndId,
                                SecondaryUserName =
                                    conditionalApproval.SecondaryApprover == null
                                        ? string.Empty
                                        : conditionalApproval.SecondaryApprover.FullNameAndId
                            };

            return View(model);
        }

        /// <summary>
        /// #5
        /// POST: /ConditionalApproval/Edit/
        /// </summary>
        /// <param name="conditionalApprovalViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ConditionalApprovalViewModel conditionalApprovalViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(conditionalApprovalViewModel);
            }

            ActionResult redirectToAction;
            var conditionalApprovalToEdit = GetConditionalApprovalAndCheckAccess(conditionalApprovalViewModel.Id, out redirectToAction);
            if(conditionalApprovalToEdit == null)
            {
                return redirectToAction;
            }

            //TODO: for now, only updating of the question is allowed
            conditionalApprovalToEdit.Question = conditionalApprovalViewModel.Question;

            _conditionalApprovalRepository.EnsurePersistent(conditionalApprovalToEdit);

            Message = "Conditional Approval edited successfully";

            if (conditionalApprovalToEdit.Workgroup != null)
            {
                return this.RedirectToAction(a => a.ByWorkgroup(conditionalApprovalToEdit.Workgroup.Id));
            }

            if (conditionalApprovalToEdit.Organization != null)
            {
                return this.RedirectToAction(a => a.ByOrg(conditionalApprovalToEdit.Organization.Id));
            }

            //return this.RedirectToAction(a => a.Index());
            return this.RedirectToAction<ErrorController>(a => a.Index());
        }

        /// <summary>
        /// #6
        /// Get: /ConditionalApproval/Create/
        /// </summary>
        /// <param name="workgroupId"> </param>
        /// <param name="orgId"> </param>
        /// <returns></returns>
        //public ActionResult Create(string approvalType)
        public ActionResult Create(int? workgroupId, string orgId)
        {
            Check.Require(workgroupId.HasValue || !string.IsNullOrWhiteSpace(orgId), "Missing Parameters");

            var approvalType = string.Empty;

            if (workgroupId.HasValue)
            {
                approvalType = WorkgroupType;
                ViewBag.WorkgroupId = workgroupId.Value;

                var workgroup = _workgroupRepository.Queryable.Single(a => a.Id == workgroupId.Value);
                if(workgroup.Administrative)
                {
                    ErrorMessage = "Conditional Approval may not be added to an administrative workgroup.";
                    return this.RedirectToAction<WorkgroupController>(a => a.Details(workgroup.Id));
                }
            }
            else if (!string.IsNullOrWhiteSpace(orgId))
            {
                approvalType = OrganizationType;
                ViewBag.OrganizationId = orgId;
            }
            
            

            var model = CreateModifyModel(approvalType);

            if (model == null)
            {
                ErrorMessage =
                    string.Format(
                        "You cannot create a conditional approval for type {0} because you are not associated with any {0}s.",
                        approvalType);

                //return this.RedirectToAction(a => a.Index());
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            
            return View(model);
        }

        /// <summary>
        /// #7
        /// Post: /ConditionalApproval/Create/
        /// </summary>
        /// <param name="orgId"> </param>
        /// <param name="modifyModel"></param>
        /// <param name="workgroupId"> </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int? workgroupId, string orgId, ConditionalApprovalModifyModel modifyModel)
        {
            if (workgroupId.HasValue)
            {
                ViewBag.WorkgroupId = workgroupId.Value;
                modifyModel.ApprovalType = WorkgroupType;
                modifyModel.Workgroup = _workgroupRepository.GetNullableById(workgroupId.Value);
                var workgroup = _workgroupRepository.Queryable.Single(a => a.Id == workgroupId.Value);
                if(workgroup.Administrative)
                {
                    ErrorMessage = "Conditional Approval may not be added to an administrative workgroup.";
                    return this.RedirectToAction<WizardController>(a => a.Details(workgroup.Id));
                }

            }
            else if (!string.IsNullOrWhiteSpace(orgId))
            {
                modifyModel.ApprovalType = OrganizationType;
                modifyModel.Organization = _organizationRepository.GetNullableById(orgId);
                ViewBag.OrganizationId = orgId;
            }

            var primaryApproverInDb = GetUserBySearchTerm(modifyModel.PrimaryApprover);
            var secondaryApproverInDb = string.IsNullOrWhiteSpace(modifyModel.SecondaryApprover)
                                            ? null
                                            : GetUserBySearchTerm(modifyModel.SecondaryApprover);

            if (primaryApproverInDb == null)
            {
                DirectoryUser primaryApproverInLdap = _directorySearchService.FindUser(modifyModel.PrimaryApprover);

                if (primaryApproverInLdap == null)
                {
                    ModelState.AddModelError("primaryapprover",
                         "No user could be found with the kerberos or email address entered");
                }
                else //found the primary approver in ldap
                {
                    primaryApproverInDb = new User(primaryApproverInLdap.LoginId)
                    {
                        FirstName = primaryApproverInLdap.FirstName,
                        LastName = primaryApproverInLdap.LastName,
                        Email = primaryApproverInLdap.EmailAddress,
                        IsActive = true
                    };

                    _userRepository.EnsurePersistent(primaryApproverInDb, forceSave: true);
                }
            }

            if (!string.IsNullOrWhiteSpace(modifyModel.SecondaryApprover)) //only check if a value was provided
            {
                if (secondaryApproverInDb == null)
                {
                    DirectoryUser secondaryApproverInLdap = _directorySearchService.FindUser(modifyModel.SecondaryApprover);

                    if (secondaryApproverInLdap == null)
                    {
                        ModelState.AddModelError("secondaryapprover",
                                                 "No user could be found with the kerberos or email address entered");
                    }
                    else //found the secondary approver in ldap
                    {
                        secondaryApproverInDb = new User(secondaryApproverInLdap.LoginId)
                        {
                            FirstName = secondaryApproverInLdap.FirstName,
                            LastName = secondaryApproverInLdap.LastName,
                            Email = secondaryApproverInLdap.EmailAddress,
                            IsActive = true
                        };

                        _userRepository.EnsurePersistent(secondaryApproverInDb, forceSave: true);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return View(CreateModifyModel(modifyModel.ApprovalType, modifyModel));
            }

            Check.Require(modifyModel.Workgroup != null || modifyModel.Organization != null, "Must have a Workgroup or an Organization");

            var newConditionalApproval = new ConditionalApproval
                                             {
                                                 Question = modifyModel.Question,
                                                 Organization = modifyModel.Organization,
                                                 Workgroup = modifyModel.Workgroup,
                                                 PrimaryApprover = primaryApproverInDb,
                                                 SecondaryApprover = secondaryApproverInDb
                                             };

            _conditionalApprovalRepository.EnsurePersistent(newConditionalApproval);

            Message = "Conditional approval added successfully";

            if (workgroupId.HasValue)
            {
                return this.RedirectToAction(a => a.ByWorkgroup(workgroupId.Value));
            }
            
            if (!string.IsNullOrWhiteSpace(orgId))
            {
                return this.RedirectToAction(a => a.ByOrg(orgId));
            }

            //return this.RedirectToAction(a => a.Index());
            return this.RedirectToAction<ErrorController>(a => a.Index());
        }

        private ConditionalApprovalModifyModel CreateModifyModel(string approvalType, ConditionalApprovalModifyModel existingModel = null)
        {
            var model = existingModel ?? new ConditionalApprovalModifyModel {ApprovalType = approvalType};
            
            var userWithOrgs = GetUserWithOrgs();

            if (approvalType == WorkgroupType)
            {
                model.Workgroups = GetWorkgroups(userWithOrgs).ToList();

                if (model.Workgroups.Count() == 0)
                {
                    return null;
                }
            }
            else if (approvalType == OrganizationType)
            {
                model.Organizations = userWithOrgs.Organizations.ToList();

                if (model.Organizations.Count() == 0)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return model;
        }

        private User GetUserBySearchTerm(string searchTerm)
        {
            return _userRepository.Queryable.Where(x => x.Id == searchTerm || x.Email == searchTerm).SingleOrDefault();
        }

        private IQueryable<Workgroup> GetWorkgroups(User user)
        {
            //var orgIds = user.Organizations.Select(x => x.Id).ToArray();

            //return _workgroupRepository.Queryable.Where(x => x.Organizations.Any(a => orgIds.Contains(a.Id)));

            var person = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).Single();

            var porgs = person.Organizations.Select(x => x.Id).ToList();
            //var orgIds = person.Organizations.Select(x => x.Id).ToArray();

            var wgIds = _queryRepositoryFactory.AdminWorkgroupRepository.Queryable.Where(a => porgs.Contains(a.RollupParentId)).Select(a => a.WorkgroupId);

            //var orgIds = _queryRepositoryFactory.OrganizationDescendantRepository.Queryable.Where(a => porgs.Contains(a.RollupParentId)).Select(a => a.OrgId).ToList();
            var workgroups = _workgroupRepository.Queryable.Where(a => a.Organizations.Any(b => wgIds.Contains(a.Id)));

            return workgroups;
        }

        private User GetUserWithOrgs()
        {
            return
                _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).
                    Single();
        }

        private ConditionalApproval GetConditionalApprovalAndCheckAccess(int id, out ActionResult redirectToAction, bool extraFetch = false)
        {
            ConditionalApproval conditionalApproval;
            if(extraFetch)
            {
                conditionalApproval = _conditionalApprovalRepository.Queryable.Where(a => a.Id == id).Fetch(x => x.Organization).Fetch(x => x.Workgroup).SingleOrDefault();
            }
            else
            {
                conditionalApproval = _conditionalApprovalRepository.GetNullableById(id);    
            }
            
            if(conditionalApproval == null)
            {
                ErrorMessage = "Conditional Approval not found";
                {
                    //redirectToAction = this.RedirectToAction(a => a.Index());
                    redirectToAction =  this.RedirectToAction<ErrorController>(a => a.Index());
                    return null;
                }
            }
            string message;
            if(!_securityService.HasWorkgroupOrOrganizationAccess(null, conditionalApproval.Organization ?? conditionalApproval.Workgroup.PrimaryOrganization, out message))
            {
                Message = message;
                {
                    redirectToAction = this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                    return null;
                }
            }
            redirectToAction = null;
            return conditionalApproval;
        }
    }

    public class ConditionalApprovalViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Question { get; set; }

        public string OrgOrWorkgroupName { get; set; }

        [Display(Name = "Primary")]
        public string PrimaryUserName { get; set; }

        [Display(Name = "Secondary")]
        public string SecondaryUserName { get; set; }
    }

    public class ConditionalApprovalModifyModel
    {
        public virtual IList<Workgroup> Workgroups { get; set; }
        public virtual IList<Organization> Organizations { get; set; }

        public virtual Workgroup Workgroup { get; set; }
        public virtual Organization Organization { get; set; }

        public virtual string ApprovalType { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public virtual string Question { get; set; }
        [Required]
        public virtual string PrimaryApprover { get; set; }
        public virtual string SecondaryApprover { get; set; }
    }

    public class ConditionalApprovalIndexModel
    {
        public IList<ConditionalApproval> ConditionalApprovalsForWorkgroups { get; set; }
        public IList<ConditionalApproval> ConditionalApprovalsForOrgs { get; set; }
    }
}
