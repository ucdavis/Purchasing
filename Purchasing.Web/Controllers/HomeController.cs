using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MvcContrib;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;
using Microsoft.Practices.ServiceLocation;

namespace Purchasing.Web.Controllers
{
    public class HomeController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IDbService _dbService;

        public HomeController(IRepositoryWithTypedId<User, string> userRepository, IDbService dbService)
        {
            _userRepository = userRepository;
            _dbService = dbService;
        }

        /// <summary>
        /// Landing Page welcoming Users to the PrePurchasing System
        /// </summary>
        /// <returns></returns>
        [HandleTransactionsManually]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HandleTransactionsManually]
        public ActionResult Landing()
        {
            if (!_userRepository.Queryable.Any(a => a.Id == CurrentUser.Identity.Name && a.IsActive))
            {
                Message = "You are currently not an active user for this program. If you believe this is incorrect contact your departmental administrator to add you.";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }

            using (var conn = _dbService.GetConnection())
            {
                var viewModel = new LandingViewModel
                    {
                        PendingOrders =
                            conn.Query<OrderHistoryBase>(
                                "SELECT * FROM [dbo].[udf_GetPendingOrdersForLogin] (@login) ORDER BY lastactiondate DESC",
                                new {login = CurrentUser.Identity.Name}),
                        YourOpenOrders =
                            conn.Query<OrderHistoryBase>(
                                "SELECT * FROM [dbo].[vOpenOrdersByUser] WHERE accessuserid = @login ORDER BY lastactiondate DESC",
                                new {login = CurrentUser.Identity.Name})
                    };
                
                return View(viewModel);
            }
        }
    }
}