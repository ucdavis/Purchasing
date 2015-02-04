using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Dapper;
using Purchasing.Core.Services;
using Purchasing.Mvc.Services;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Mvc.Controllers
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
        [OutputCache(Duration = 14400)] //4 hours
        public JsonNetResult Overall()
        {
            var stats = new OverallStatistics { LastUpdated = DateTime.UtcNow };

            using (var conn = _dbService.GetConnection())
            {
                stats.TotalOrdersPlaced = conn.Query<int>("select COUNT(*) from Orders").Single();
                stats.TotalOrdersCompleted = conn.Query<int>("select COUNT(*) from Orders where OrderStatusCodeId = 'CP'").Single();
                stats.TotalAmount = string.Format("{0:C2}", conn.Query<decimal>("select SUM(Total) as OrderTotalSumCompleted from Orders").Single());
                var amount = conn.Query<decimal>("select SUM(Total) as OrderTotalSumCompleted from Orders where OrderStatusCodeId = 'CP'").Single();
                stats.TotalAmountCompleted = string.Format("{0:C2}",amount);

                
                stats.ShowFireworks = ( amount > 100000000.0m && amount < 101000000.0m);

                stats.Attachments = conn.Query<int>("select COUNT(*) from Attachments").Single();
                stats.Accounts = conn.Query<int>("select COUNT(distinct Account) from Splits").Single();

                stats.ActiveUsersInWorkgroups = conn.Query<int>("select COUNT( distinct UserId) from WorkgroupPermissions").Single();
                stats.Actions = conn.Query<int>("select COUNT(*) from OrderTracking").Single();
                stats.Workgroups = conn.Query<int>("select count(*) from Workgroups where IsActive = 1").Single();

                stats.OrgsWithOrders = conn.Query<int>("select COUNT(distinct OrganizationId) from Orders").Single();
                stats.OrgsWithWorkgroups = conn.Query<int>("select COUNT(distinct OrganizationId) from WorkgroupsXOrganizations").Single();

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

        public int Attachments { get; set; }
        public int Accounts { get; set; }

        public int ActiveUsersInWorkgroups { get; set; }
        public int Actions { get; set; }
        public int Workgroups { get; set; }

        public int OrgsWithOrders { get; set; }
        public int OrgsWithWorkgroups { get; set; }

        public DateTime LastUpdated { get; set; }
        public bool ShowFireworks { get; set; }
    }
}
