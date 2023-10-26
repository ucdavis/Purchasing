using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Services;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;
using System;
using System.Threading.Tasks;

namespace Purchasing.Mvc.Controllers
{
    [HandleTransactionsManually]
    [Authorize(Policy = Role.Codes.Admin)]
    public class SystemController : SuperController
    {
        private readonly IIndexService _indexService;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepositoryWithTypedId<EmailQueue, Guid> _emailQueueRepository;
        private readonly IAeLookupsService _aeLookupService;

        //private readonly INotificationSender _notificationSender;

        public SystemController(IIndexService indexService, IRepositoryFactory repositoryFactory, IRepositoryWithTypedId<EmailQueue, Guid> emailQueueRepository, IAeLookupsService aeLookupService)//, INotificationSender notificationSender)
        {
            _indexService = indexService;
            _repositoryFactory = repositoryFactory;
            _emailQueueRepository = emailQueueRepository;
            _aeLookupService = aeLookupService;
            //_notificationSender = notificationSender;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategories()
        {           
            await _aeLookupService.UpdateCategories(true);
            _indexService.CreateCommoditiesIndex(); // We are looking up from the index, so need to do this after an update.
            Message = "Categories Updated";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUnitOfMeasures()
        {
            await _aeLookupService.UpdateUnitOfMeasure();
            Message = "Unit of measures updated";
            return RedirectToAction("Index");
        }

        public ActionResult Indexes()
        {
            ViewBag.ModifiedDates = new Dictionary<string, DateTime>
                                    {
                                        {"OrderHistory", _indexService.LastModified(Core.Services.Indexes.OrderHistory)},
                                        {"OrderTracking", _indexService.LastModified(Core.Services.Indexes.OrderTracking)}
                                        ,{"LineItems", _indexService.LastModified(Core.Services.Indexes.LineItems)}
                                        ,{"Comments", _indexService.LastModified(Core.Services.Indexes.Comments)}
                                        ,{"CustomAnswers", _indexService.LastModified(Core.Services.Indexes.CustomAnswers)}
                                        ,{"Accounts", _indexService.LastModified(Core.Services.Indexes.Accounts)}
                                        ,{"Buildings", _indexService.LastModified(Core.Services.Indexes.Buildings)}
                                        ,{"Commodities", _indexService.LastModified(Core.Services.Indexes.Commodities)}
                                        ,{"Vendors", _indexService.LastModified(Core.Services.Indexes.Vendors)}
                                    };

            ViewBag.NumRecords = new Dictionary<string, int>
                                     {
                                         {"OrderHistory", _indexService.NumRecords(Core.Services.Indexes.OrderHistory)}
                                         ,{"OrderTracking", _indexService.NumRecords(Core.Services.Indexes.OrderTracking)}
                                         ,{"LineItems", _indexService.NumRecords(Core.Services.Indexes.LineItems)}
                                         ,{"Comments", _indexService.NumRecords(Core.Services.Indexes.Comments)}
                                         ,{"CustomAnswers", _indexService.NumRecords(Core.Services.Indexes.CustomAnswers)}
                                         ,{"Accounts", _indexService.NumRecords(Core.Services.Indexes.Accounts)}
                                        ,{"Buildings", _indexService.NumRecords(Core.Services.Indexes.Buildings)}
                                        ,{"Commodities", _indexService.NumRecords(Core.Services.Indexes.Commodities)}
                                        ,{"Vendors", _indexService.NumRecords(Core.Services.Indexes.Vendors)}
                                     };

            return View();
        }

        public ActionResult Overwrite(string index)
        {
            switch (index)
            {
                case "OrderHistory":
                    _indexService.CreateHistoricalOrderIndex();
                    break;
                case "OrderTracking":
                    _indexService.CreateTrackingIndex();
                    break;
                case "LineItems":
                    _indexService.CreateLineItemsIndex();
                    break;
                case "Comments":
                    _indexService.CreateCommentsIndex();
                    break;
                case "CustomAnswers":
                    _indexService.CreateCustomAnswersIndex();
                    break;
                case "Accounts":
                    _indexService.CreateAccountsIndex();
                    break;
                case "Buildings":
                    _indexService.CreateBuildingsIndex();
                    break;
                case "Commodities":
                    _indexService.CreateCommoditiesIndex();
                    break;
                case "Vendors":
                    _indexService.CreateVendorsIndex();
                    break;
            }

            Message = index + " Updated";

            return RedirectToAction("Indexes");
        }

        public ActionResult Backups()
        {
            var backupLogs = _repositoryFactory.BackupLogRepository.Queryable.OrderByDescending(a => a.DateTimeCreated).Take(20);
            return View(backupLogs);
        }

        //public ActionResult Emails()
        //{
        //    ViewBag.PerEvent = _emailQueueRepository.Queryable.Count(x => x.Pending && x.NotificationType == EmailPreferences.NotificationTypes.PerEvent);
        //    ViewBag.Daily = _emailQueueRepository.Queryable.Count(x => x.Pending && x.NotificationType == EmailPreferences.NotificationTypes.Daily);
        //    ViewBag.Weekly = _emailQueueRepository.Queryable.Count(x => x.Pending && x.NotificationType == EmailPreferences.NotificationTypes.Weekly);
        //    return View();
        //}

        //[HttpPost]
        //public RedirectToRouteResult TriggerEmail()
        //{
        //    //_notificationSender.SendNotifications();
        //    Message = "Emails Processed";
        //    return RedirectToAction("Emails");
        //}
    }
}