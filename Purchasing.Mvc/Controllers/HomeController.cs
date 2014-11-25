using System.Linq;
using System.Web.Mvc;
using Dapper;
using Microsoft.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;

namespace Purchasing.Mvc.Controllers
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