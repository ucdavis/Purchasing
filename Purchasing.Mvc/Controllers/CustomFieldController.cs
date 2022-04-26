using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Services;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Helpers;

namespace Purchasing.Mvc.Controllers
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
        private readonly IMapper _mapper;

        public CustomFieldController(IRepository<CustomField> customFieldRepository, IRepositoryWithTypedId<Organization, string> organizationRepository, ISecurityService securityService,IMapper mapper)
        {
            _customFieldRepository = customFieldRepository;
            _organizationRepository = organizationRepository;
            _securityService = securityService;
            _mapper = mapper;
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
                return this.RedirectToAction(nameof(OrganizationController.Index), typeof(OrganizationController).ControllerName());
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, org, out message))
            {
                Message = message;
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
            }

            return View(org);
        }


        /// <summary>
        /// GET: /CustomField/Create
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns></returns>
        public ActionResult Create(string id)
        {
            var org = string.IsNullOrWhiteSpace(id) ? null : _organizationRepository.GetNullableById(id);

            if (org == null)
            {
                Message = "Organization not found for custom field.";
                return this.RedirectToAction(nameof(OrganizationController.Index), typeof(OrganizationController).ControllerName());
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, org, out message))
            {
                Message = message;
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
            }

			var viewModel = CustomFieldViewModel.Create(Repository, org);
            
            return View(viewModel);
        } 

        /// <summary>
        /// POST: /CustomField/Create
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <param name="customField"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(string id, CustomField customField)
        {
            var org = string.IsNullOrWhiteSpace(id) ? null : _organizationRepository.GetNullableById(id);

            if (org == null)
            {
                Message = "Organization not found for custom field.";
                return this.RedirectToAction(nameof(OrganizationController.Index), typeof(OrganizationController).ControllerName());
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, org, out message))
            {
                Message = message;
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
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
                return this.RedirectToAction(nameof(Index));
            }
            else
            {
				var viewModel = CustomFieldViewModel.Create(Repository, org);
                viewModel.CustomField = customField;

                return View(viewModel);
            }
        }

        /// <summary>
        /// GET: /CustomField/Edit/5
        /// </summary>
        /// <param name="id">Custom Field Id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var customField = _customFieldRepository.GetNullableById(id);

            if (customField == null)
            {
                ErrorMessage = "Custom Field not found.";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, customField.Organization, out message))
            {
                Message = message;
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
            }

			var viewModel = CustomFieldViewModel.Create(Repository, customField.Organization, customField);

			return View(viewModel);
        }
        
        /// <summary>
        /// POST: /CustomField/Edit/5
        /// </summary>
        /// <param name="id">CustomField Id</param>
        /// <param name="customField"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, CustomField customField)
        {
            var customFieldToArchive = _customFieldRepository.GetNullableById(id);

            if(customFieldToArchive == null)
            {                               
                ErrorMessage = "Custom Field not found.";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, customFieldToArchive.Organization, out message))
            {
                Message = message;
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
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
                return this.RedirectToAction(nameof(Index));
            }
            else
            {
                var viewModel = CustomFieldViewModel.Create(Repository, customFieldToEdit.Organization, customFieldToEdit);

                return View(viewModel);
            }
        }
        
        /// <summary>
        /// GET: /CustomField/Delete/5 
        /// </summary>
        /// <param name="id">CustomField Id</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
			var customField = _customFieldRepository.GetNullableById(id);

            if (customField == null)
            {
                ErrorMessage = "Custom Field not found.";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, customField.Organization, out message))
            {
                Message = message;
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
            }

            return View(customField);
        }

        //
        // POST: /CustomField/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, CustomField customField)
        {
			var customFieldToDelete = _customFieldRepository.GetNullableById(id);

            if (customFieldToDelete == null)
            {
                ErrorMessage = "Custom Field not found.";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
            }

            var message = string.Empty;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, customFieldToDelete.Organization, out message))
            {
                Message = message;
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
            }

            customFieldToDelete.IsActive = false;
            _customFieldRepository.EnsurePersistent(customFieldToDelete);

            Message = "CustomField Removed Successfully";

            return this.RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public JsonNetResult UpdateOrder(string id, List<int> customFieldIds)
        {
            if (customFieldIds != null)
            {
                try
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
                }
                catch (Exception)
                {
                    return new JsonNetResult(false);
                }
                return new JsonNetResult(true);
            }

            return new JsonNetResult(false);
        }

        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private void TransferValues(CustomField source, CustomField destination)
        {
            _mapper.Map(source, destination);
        }
    }
}
