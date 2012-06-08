using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Purchasing.Core.Domain;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Helpers;
using MvcContrib;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the CustomField class
    /// </summary>
    [Authorize(Roles=Role.Codes.DepartmentalAdmin)]
    public class CustomFieldController : ApplicationController
    {
	    private readonly IRepository<CustomField> _customFieldRepository;
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository;
        private readonly ISecurityService _securityService;

        public CustomFieldController(IRepository<CustomField> customFieldRepository, IRepositoryWithTypedId<Organization, string> organizationRepository, ISecurityService securityService )
        {
            _customFieldRepository = customFieldRepository;
            _organizationRepository = organizationRepository;
            _securityService = securityService;
        }

        /// <summary>
        /// List of Custom Fields for the organization
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            var org = string.IsNullOrWhiteSpace(id) ? null : _organizationRepository.GetNullableById(id);
            
            if (org == null)
            {
                Message = "Organization not found.";
                return this.RedirectToAction<OrganizationController>(a => a.Index());
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, org, out message))
            {
                Message = message;
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }

            return View(org);
        }

        //
        // GET: /CustomField/Create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns></returns>
        public ActionResult Create(string id)
        {
            var org = string.IsNullOrWhiteSpace(id) ? null : _organizationRepository.GetNullableById(id);

            if (org == null)
            {
                Message = "Organization not found for custom field.";
                return this.RedirectToAction<OrganizationController>(a => a.Index());
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, org, out message))
            {
                Message = message;
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }

			var viewModel = CustomFieldViewModel.Create(Repository, org);
            
            return View(viewModel);
        } 

        //
        // POST: /CustomField/Create
        [HttpPost]
        public ActionResult Create(string id, CustomField customField)
        {
            var org = string.IsNullOrWhiteSpace(id) ? null : _organizationRepository.GetNullableById(id);

            if (org == null)
            {
                Message = "Organization not found for custom field.";
                return this.RedirectToAction<OrganizationController>(a => a.Index());
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, org, out message))
            {
                Message = message;
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }

            var customFieldToCreate = new CustomField();
            customFieldToCreate.Organization = org;

            TransferValues(customField, customFieldToCreate);

            ModelState.Clear();
            customFieldToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _customFieldRepository.EnsurePersistent(customFieldToCreate);

                Message = "CustomField Created Successfully";

                //return RedirectToAction("Index", new {id=id});
                return this.RedirectToAction(a => a.Index(id));
            }
            else
            {
				var viewModel = CustomFieldViewModel.Create(Repository, org);
                viewModel.CustomField = customField;

                return View(viewModel);
            }
        }

        //
        // GET: /CustomField/Edit/5
        public ActionResult Edit(int id)
        {
            var customField = _customFieldRepository.GetNullableById(id);

            if (customField == null) return RedirectToAction("Index");

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, customField.Organization, out message))
            {
                return new HttpUnauthorizedResult(message);
            }

			var viewModel = CustomFieldViewModel.Create(Repository, customField.Organization, customField);

			return View(viewModel);
        }
        
        //
        // POST: /CustomField/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, CustomField customField)
        {
            var customFieldToArchive = _customFieldRepository.GetNullableById(id);

            if(customFieldToArchive == null)
            {
                return RedirectToAction("Index");
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, customFieldToArchive.Organization, out message))
            {
                return new HttpUnauthorizedResult(message);
            }

            var customFieldToEdit = new CustomField();
            customFieldToEdit.Organization = customFieldToArchive.Organization;

            TransferValues(customField, customFieldToEdit);

            ModelState.Clear();
            customFieldToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                customFieldToArchive.IsActive = false;
                _customFieldRepository.EnsurePersistent(customFieldToArchive);
                _customFieldRepository.EnsurePersistent(customFieldToEdit);

                Message = "CustomField Edited Successfully";

                return RedirectToAction("Index", new {id=customFieldToEdit.Organization.Id});
            }
            else
            {
                var viewModel = CustomFieldViewModel.Create(Repository, customFieldToEdit.Organization, customFieldToEdit);

                return View(viewModel);
            }
        }
        
        //
        // GET: /CustomField/Delete/5 
        public ActionResult Delete(int id)
        {
			var customField = _customFieldRepository.GetNullableById(id);

            if (customField == null) return RedirectToAction("Index");

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, customField.Organization, out message))
            {
                return new HttpUnauthorizedResult(message);
            }

            return View(customField);
        }

        //
        // POST: /CustomField/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, CustomField customField)
        {
			var customFieldToDelete = _customFieldRepository.GetNullableById(id);

            if (customFieldToDelete == null) return RedirectToAction("Index");

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, customFieldToDelete.Organization, out message))
            {
                return new HttpUnauthorizedResult(message);
            }

            customFieldToDelete.IsActive = false;
            _customFieldRepository.EnsurePersistent(customFieldToDelete);

            Message = "CustomField Removed Successfully";

            return RedirectToAction("Index", new {id=customFieldToDelete.Organization.Id});
        }

        [HttpPost]
        public JsonNetResult UpdateOrder(string id, List<int> customFieldIds)
        {
            if (customFieldIds != null)
            {
                for (var i = 0; i < customFieldIds.Count; i++)
                {
                    var cf = _customFieldRepository.GetNullableById(customFieldIds[i]);

                    if (cf != null && cf.Organization.Id == id)
                    {
                        cf.Rank = i;
                        _customFieldRepository.EnsurePersistent(cf);
                    }

                }

                return new JsonNetResult(true);
            }

            return new JsonNetResult(false);
        }

        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(CustomField source, CustomField destination)
        {
            Mapper.Map(source, destination);
        }
    }
}
