using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;
using System;

namespace Purchasing.Web.Controllers
{
    [HandleTransactionsManually]
    [Authorize(Roles = Role.Codes.Admin)]
    public class SystemController : SuperController
    {
        private readonly IIndexService _indexService;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepositoryWithTypedId<EmailQueue, Guid> _emailQueueRepository;
        //private readonly INotificationSender _notificationSender;

        public SystemController(IIndexService indexService, IRepositoryFactory repositoryFactory, IRepositoryWithTypedId<EmailQueue, Guid> emailQueueRepository)//, INotificationSender notificationSender)
        {
            _indexService = indexService;
            _repositoryFactory = repositoryFactory;
            _emailQueueRepository = emailQueueRepository;
            //_notificationSender = notificationSender;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Indexes()
        {
            ViewBag.ModifiedDates = new Dictionary<string, DateTime>
                                    {
                                        {"OrderHistory", _indexService.LastModified(Services.Indexes.OrderHistory)}
                                        ,{"LineItems", _indexService.LastModified(Services.Indexes.LineItems)}
                                        ,{"Comments", _indexService.LastModified(Services.Indexes.Comments)}
                                        ,{"CustomAnswers", _indexService.LastModified(Services.Indexes.CustomAnswers)}
                                        ,{"Accounts", _indexService.LastModified(Services.Indexes.Accounts)}
                                        ,{"Buildings", _indexService.LastModified(Services.Indexes.Buildings)}
                                        ,{"Commodities", _indexService.LastModified(Services.Indexes.Commodities)}
                                        ,{"Vendors", _indexService.LastModified(Services.Indexes.Vendors)}
                                    };

            ViewBag.NumRecords = new Dictionary<string, int>
                                     {
                                         {"OrderHistory", _indexService.NumRecords(Services.Indexes.OrderHistory)}
                                         ,{"LineItems", _indexService.NumRecords(Services.Indexes.LineItems)}
                                         ,{"Comments", _indexService.NumRecords(Services.Indexes.Comments)}
                                         ,{"CustomAnswers", _indexService.NumRecords(Services.Indexes.CustomAnswers)}
                                         ,{"Accounts", _indexService.NumRecords(Services.Indexes.Accounts)}
                                        ,{"Buildings", _indexService.NumRecords(Services.Indexes.Buildings)}
                                        ,{"Commodities", _indexService.NumRecords(Services.Indexes.Commodities)}
                                        ,{"Vendors", _indexService.NumRecords(Services.Indexes.Vendors)}
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