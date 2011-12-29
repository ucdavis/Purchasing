﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Purchasing.Core.Domain;
using Purchasing.Web.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Helpers;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the CustomField class
    /// </summary>
    public class CustomFieldController : ApplicationController
    {
	    private readonly IRepository<CustomField> _customFieldRepository;
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository;

        public CustomFieldController(IRepository<CustomField> customFieldRepository, IRepositoryWithTypedId<Organization, string> organizationRepository )
        {
            _customFieldRepository = customFieldRepository;
            _organizationRepository = organizationRepository;
        }

        /// <summary>
        /// List of Custom Fields for the organization
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            var org = _organizationRepository.GetNullableById(id);

            if (org == null)
            {
                Message = "Organization not found.";
                return RedirectToAction("Index", "Workgroup");
            }

            return View(org);
        }

        //
        // GET: /CustomField/Details/5
        public ActionResult Details(int id)
        {
            var customField = _customFieldRepository.GetNullableById(id);

            if (customField == null) return RedirectToAction("Index");

            return View(customField);
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
            var org = _organizationRepository.GetNullableById(id);

            if (org == null)
            {
                Message = "Organization not found.";
                return RedirectToAction("Index", "Workgroup");
            }

			var viewModel = CustomFieldViewModel.Create(Repository, org);
            
            return View(viewModel);
        } 

        //
        // POST: /CustomField/Create
        [HttpPost]
        public ActionResult Create(string id, CustomField customField)
        {
            var org = _organizationRepository.GetNullableById(id);

            if (org == null)
            {
                Message = "Organization not found.";
                return RedirectToAction("Index", "Workgroup");
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

                return RedirectToAction("Index", new {id=id});
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

			var viewModel = CustomFieldViewModel.Create(Repository, customField.Organization, customField);

			return View(viewModel);
        }
        
        //
        // POST: /CustomField/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, CustomField customField)
        {
            var customFieldToEdit = _customFieldRepository.GetNullableById(id);

            if (customFieldToEdit == null) return RedirectToAction("Index");

            TransferValues(customField, customFieldToEdit);

            ModelState.Clear();
            customFieldToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
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

            return View(customField);
        }

        //
        // POST: /CustomField/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, CustomField customField)
        {
			var customFieldToDelete = _customFieldRepository.GetNullableById(id);

            if (customFieldToDelete == null) return RedirectToAction("Index");

            customFieldToDelete.IsActive = false;
            _customFieldRepository.EnsurePersistent(customFieldToDelete);

            Message = "CustomField Removed Successfully";

            return RedirectToAction("Index", new {id=customFieldToDelete.Organization.Id});
        }

        [HttpPost]
        public RedirectToRouteResult UpdateOrder(string id, List<CustomFieldOrderPostModel> customFieldOrderPostModel)
        {
            try
            {
                foreach (var cfo in customFieldOrderPostModel)
                {
                    var cf = _customFieldRepository.GetNullableById(cfo.CustomFieldId);

                    if (cf != null)
                    {
                        cf.Rank = cfo.Order;

                        _customFieldRepository.EnsurePersistent(cf);
                    }
                }

                Message = "Order of custom fields was successfully updated.";
                return RedirectToAction("Index", new {id=id});
            }
            catch
            {
                Message = "There was an error updating the order of custom fields.";
                return RedirectToAction("Index", new { id = id });
            }

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
