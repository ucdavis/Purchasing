using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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

        public ConditionalApprovalController(IRepository<ConditionalApproval> conditionalApprovalRepository, IRepository<Workgroup> workgroupRepository, IRepositoryWithTypedId<User,string> userRepository, IDirectorySearchService directorySearchService)
        {
            _conditionalApprovalRepository = conditionalApprovalRepository;
            _workgroupRepository = workgroupRepository;
            _userRepository = userRepository;
            _directorySearchService = directorySearchService;
        }

        /// <summary>
        /// #1
        /// GET: /ConditionalApproval/
        /// </summary>
        /// <returns>Returns the conditional approvals relating to your workgroups and departments</returns>
        public ActionResult Index()
        {
            var user = GetUserWithOrgs();

            var workgroupIds = GetWorkgroups(user).Select(x=>x.Id).ToList();

            var orgIds = user.Organizations.Select(x=>x.Id).ToList();

            //Now get all conditional approvals that exist for those workgroups
            var conditionalApprovalsForWorkgroups =
                _conditionalApprovalRepository.Queryable.Where(x => x.Workgroup != null && workgroupIds.Contains(x.Workgroup.Id));

            var conditionalApprovalsForOrgs =
                _conditionalApprovalRepository.Queryable.Where(x => x.Organization != null && orgIds.Contains(x.Organization.Id));

            var model = new ConditionalApprovalIndexModel
                            {
                                ConditionalApprovalsForOrgs = conditionalApprovalsForOrgs.Fetch(x=>x.PrimaryApprover).Fetch(x=>x.SecondaryApprover).ToList(),
                                ConditionalApprovalsForWorkgroups = conditionalApprovalsForWorkgroups.Fetch(x=>x.PrimaryApprover).Fetch(x=>x.SecondaryApprover).Fetch(x=>x.Workgroup).ToList()
                            };
            
            return View(model);
        }

        /// <summary>
        /// #2
        /// GET: /ConditionalApproval/Delete/
        /// </summary>
        /// <param name="id">ConditionalApproval Id</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var conditionalApproval = _conditionalApprovalRepository.Queryable.Where(x => x.Id == id).Fetch(x => x.Organization).Fetch(x => x.Workgroup).SingleOrDefault();

            if (conditionalApproval == null)
            {
                ErrorMessage = "Conditional Approval not found";

                return this.RedirectToAction(a => a.Index());
            }

            //Suggested access check - JCS
            //var user = GetUserWithOrgs();
            //if(conditionalApproval.Workgroup != null)
            //{
            //    var workgroupIds = GetWorkgroups(user).Select(x => x.Id).ToList();
            //    if(!workgroupIds.Contains(conditionalApproval.Workgroup.Id))
            //    {
            //        ErrorMessage = "No access to that workgroup";
            //        return this.RedirectToAction<ErrorController>(a => a.Index());
            //    }
            //}
            //else
            //{
            //    var orgIds = user.Organizations.Select(x => x.Id).ToList();
            //    if(!orgIds.Contains(conditionalApproval.Organization.Id))
            //    {
            //        ErrorMessage = "No access to that organization";
            //        return this.RedirectToAction<ErrorController>(a => a.Index());
            //    }
            //}

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

        [HttpPost]
        public ActionResult Delete(ConditionalApprovalViewModel conditionalApprovalViewModel)
        {
            var conditionalApproval = _conditionalApprovalRepository.GetById(conditionalApprovalViewModel.Id);

            _conditionalApprovalRepository.Remove(conditionalApproval);

            Message = "Conditional Approval removed successfully";

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var conditionalApproval = _conditionalApprovalRepository.Queryable.Where(x => x.Id == id).Fetch(x=>x.Organization).Fetch(x=>x.Workgroup).SingleOrDefault();

            if (conditionalApproval == null)
            {
                ErrorMessage = "Conditional Approval not found";

                return RedirectToAction("Index");
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

        [HttpPost]
        public ActionResult Edit(ConditionalApprovalViewModel conditionalApprovalViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(conditionalApprovalViewModel);
            }

            var conditionalApprovalToEdit =
                _conditionalApprovalRepository.GetNullableById(conditionalApprovalViewModel.Id);

            //TODO: for now, only updating of the question is allowed
            conditionalApprovalToEdit.Question = conditionalApprovalViewModel.Question;

            _conditionalApprovalRepository.EnsurePersistent(conditionalApprovalToEdit);

            Message = "Conditional Approval edited successfully";

            return RedirectToAction("Index");
        }

        public ActionResult Create(string approvalType)
        {
            var model = CreateModifyModel(approvalType);

            if (model == null)
            {
                ErrorMessage =
                    string.Format(
                        "You cannot create a conditional approval for type {0} because you are not associated with any {0}s.",
                        approvalType);

                return RedirectToAction("Index");
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ConditionalApprovalModifyModel modifyModel)
        {
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

            return RedirectToAction("Index");
        }

        private ConditionalApprovalModifyModel CreateModifyModel(string approvalType, ConditionalApprovalModifyModel existingModel = null)
        {
            var model = existingModel ?? new ConditionalApprovalModifyModel {ApprovalType = approvalType};
            
            var userWithOrgs = GetUserWithOrgs();

            if (approvalType == "Workgroup")
            {
                model.Workgroups = GetWorkgroups(userWithOrgs).ToList();

                if (model.Workgroups.Count() == 0)
                {
                    return null;
                }
            }
            else if (approvalType == "Organization")
            {
                model.Organizations = userWithOrgs.Organizations.ToList();

                if (model.Organizations.Count() == 0)
                {
                    return null;
                }
            }

            return model;
        }

        private User GetUserBySearchTerm(string searchTerm)
        {
            return _userRepository.Queryable.Where(x => x.Id == searchTerm || x.Email == searchTerm).SingleOrDefault();
        }

        private IQueryable<Workgroup> GetWorkgroups(User user)
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

        [Required]
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
