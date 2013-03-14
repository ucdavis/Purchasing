using System;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class ReportProcessingTimeViewModel
    {
        public IEnumerable<Workgroup> Workgroups;
        public Workgroup Workgroup { get; set; }
        public DateTime? Month { get; set; }
        public List<ReportProcessingColumns> Columns { get; set; } 

        public static ReportProcessingTimeViewModel Create(IWorkgroupService workgroupService, Workgroup workgroup)
        {
            var workgroups = workgroupService.LoadAdminWorkgroups();

            var viewModel = new ReportProcessingTimeViewModel()
                                {
                                    Workgroups = workgroups,
                                    Workgroup = workgroup,
                                    Columns = null
                                };

            return viewModel;
        }


        public void GenerateDisplayTable(ISearchService indexSearchService, IRepositoryFactory repositoryFactory,IWorkgroupService workgroupService, int workgroupId, DateTime month)
        {
            Check.Require(month.Day == 1);
            var endMonth = month.AddMonths(1);

            var workgroup = repositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == workgroupId);
            var allChildWorkgroups = new List<int>();
            allChildWorkgroups.Add(workgroupId);
            if (workgroup.Administrative)
            {
                allChildWorkgroups = workgroupService.GetChildWorkgroups(workgroupId);
            }

            Columns = new List<ReportProcessingColumns>();
            var workgroups = repositoryFactory.WorkgroupRepository.Queryable.Where(a => allChildWorkgroups.Contains(a.Id)).ToList();
            var matchingOrders = indexSearchService.GetOrdersByWorkgroups(workgroups, month, endMonth);
            foreach (var matchingOrder in matchingOrders)
            {
                
                var column = new ReportProcessingColumns();
                var order = repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == matchingOrder.OrderId);
                if (!order.StatusCode.IsComplete)
                {
                    continue; //Only report on completed orders?
                }
                column.Organization = order.Workgroup.PrimaryOrganization.Id;
                column.Workgroup = order.Workgroup.Name;
                column.OrderRequestNumber = order.RequestNumber;
                column.OrderId = order.Id;
                var orderHistory = order.OrderTrackings.LastOrDefault(a => a.StatusCode.Id == OrderStatusCode.Codes.AccountManager && a.Description.StartsWith("approved"));
                if (orderHistory != null)
                {
                    column.ArrivedAtPurchaser = orderHistory.DateCreated;
                }

                orderHistory = order.OrderTrackings.LastOrDefault(a => a.Description.StartsWith("rerouted to purchaser"));
                if (orderHistory != null)
                {
                    column.ReroutedToPurchaserDate = orderHistory.DateCreated;
                    column.ReRoutedToPurchaserBy = orderHistory.User.FullName;
                    var xx = 
                    column.ReroutedToPurchaserName = orderHistory.Description.Substring(22);
                }
                orderHistory = order.OrderTrackings.FirstOrDefault(a => a.StatusCode.IsComplete);
                if (orderHistory != null)
                {
                    column.CompletedByPurchaserDate = orderHistory.DateCreated;
                    column.CompletedByPurchaserName = orderHistory.User.FullName;
                }
                Columns.Add(column);
            }


        }
    }

    //public class ReportWorkloadHeaders
    //{
    //    public string OrgId { get; set; }
    //    public List<string> Workgroups { get; set; }
    //}

    public class ReportProcessingColumns
    {
        public string Organization { get; set; }
        public string Workgroup { get; set; }
        public string OrderRequestNumber { get; set; }
        public int OrderId { get; set; }
        public DateTime? ArrivedAtPurchaser { get; set; }
        public DateTime? ReroutedToPurchaserDate { get; set; }
        public string ReroutedToPurchaserName { get; set; }
        public string ReRoutedToPurchaserBy { get; set; }
        public DateTime? CompletedByPurchaserDate { get; set; }
        public string CompletedByPurchaserName { get; set; }
    }


}