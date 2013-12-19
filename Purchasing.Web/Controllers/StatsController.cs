

using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Dapper;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Stats class
    /// </summary>
    [SessionState(SessionStateBehavior.Disabled)]
    public class StatsController : ApplicationController
    {
        private readonly IDbService _dbService;


        public StatsController(IDbService dbService)
        {
            _dbService = dbService;

        }

        [HandleTransactionsManually]
        [OutputCache(Duration = 3600)]
        public JsonNetResult Overall()
        {
            var stats = new OverallStatistics { LastUpdated = DateTime.UtcNow };

            using (var conn = _dbService.GetConnection())
            {
                stats.TotalOrdersPlaced = conn.Query<int>("select COUNT(*) from Orders").Single();
                stats.TotalOrdersCompleted = conn.Query<int>("select COUNT(*) from Orders where OrderStatusCodeId = 'CP'").Single();
                stats.TotalAmount = string.Format("{0:C2}", conn.Query<decimal>("select SUM(Total) as OrderTotalSumCompleted from Orders").Single());
                stats.TotalAmountCompleted = string.Format("{0:C2}",conn.Query<decimal>("select SUM(Total) as OrderTotalSumCompleted from Orders where OrderStatusCodeId = 'CP'").Single());

                stats.ActiveUsersInWorkgroups = conn.Query<int>("select COUNT( distinct UserId) from WorkgroupPermissions").Single();




            }

            return new JsonNetResult(stats);
        }


    }

    public class OverallStatistics
    {
        public int TotalOrdersPlaced { get; set; }
        public int TotalOrdersCompleted { get; set; }
        public string TotalAmount { get; set; }
        public string TotalAmountCompleted { get; set; }

        public int ActiveUsersInWorkgroups { get; set; }



        public DateTime LastUpdated { get; set; }
    }
}
