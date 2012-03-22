using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the ServiceMessage class
    /// </summary>
    public class ServiceMessageController : ApplicationController
    {
	    private readonly IRepository<ServiceMessage> _serviceMessageRepository;

        public ServiceMessageController(IRepository<ServiceMessage> serviceMessageRepository)
        {
            _serviceMessageRepository = serviceMessageRepository;
        }
    
        //
        // GET: /ServiceMessage/
        public ActionResult Index()
        {
            var serviceMessageList = _serviceMessageRepository.Queryable;

            return View(serviceMessageList.ToList());
        }

        [ChildActionOnly]
        public ActionResult ServiceMessages()
        {
            var currentDate = DateTime.Now.Date;
            var serviceMessageList = _serviceMessageRepository.Queryable.Where(a=> a.IsActive==true && (a.BeginDisplayDate == null || a.BeginDisplayDate >= currentDate) && (a.EndDisplayDate==null || a.EndDisplayDate <= currentDate) ).ToList();
            //return View(serviceMessageList.ToList());
            return PartialView("~/Views/Shared/_ServiceMessages.cshtml", serviceMessageList); 
        }

        //
        // GET: /ServiceMessage/Create
        public ActionResult Create()
        {
			var viewModel = ServiceMessageViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /ServiceMessage/Create
        [HttpPost]
        public ActionResult Create(ServiceMessage serviceMessage)
        {
            var serviceMessageToCreate = new ServiceMessage();

            TransferValues(serviceMessage, serviceMessageToCreate);

            if (ModelState.IsValid)
            {
                _serviceMessageRepository.EnsurePersistent(serviceMessageToCreate);

                Message = "ServiceMessage Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = ServiceMessageViewModel.Create(Repository);
                viewModel.ServiceMessage = serviceMessage;

                return View(viewModel);
            }
        }

        //
        // GET: /ServiceMessage/Edit/5
        public ActionResult Edit(int id)
        {
            var serviceMessage = _serviceMessageRepository.GetNullableById(id);

            if (serviceMessage == null) return RedirectToAction("Index");

			var viewModel = ServiceMessageViewModel.Create(Repository);
			viewModel.ServiceMessage = serviceMessage;
            
			return View(viewModel);
        }
        
        //
        // POST: /ServiceMessage/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ServiceMessage serviceMessage)
        {
            var serviceMessageToEdit = _serviceMessageRepository.GetNullableById(id);

            if (serviceMessageToEdit == null) return RedirectToAction("Index");

            TransferValues(serviceMessage, serviceMessageToEdit);

            if (ModelState.IsValid)
            {
                _serviceMessageRepository.EnsurePersistent(serviceMessageToEdit);

                Message = "ServiceMessage Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = ServiceMessageViewModel.Create(Repository);
                viewModel.ServiceMessage = serviceMessage;

                return View(viewModel);
            }
        }
        
        
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(ServiceMessage source, ServiceMessage destination)
        {
			//Recommendation: Use AutoMapper
            Mapper.Map(source, destination);
            //throw new NotImplementedException();
        }

    }

	/// <summary>
    /// ViewModel for the ServiceMessage class
    /// </summary>
    public class ServiceMessageViewModel
	{
		public ServiceMessage ServiceMessage { get; set; }
 
		public static ServiceMessageViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new ServiceMessageViewModel {ServiceMessage = new ServiceMessage()};
 
			return viewModel;
		}
	}
}
