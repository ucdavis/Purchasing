using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Models
{
    public class FilteredOrderListModelDto
    {
        public List<OrderHistoryDto> OrderHistoryDtos { get; set; }

        public class OrderHistoryDto
        {
            public Order Order { get; set; }
            public string Workgroup { get; set; }
            public WorkgroupVendor Vendor { get; set; }
            public string CreatedBy { get; set; }
            public string Status { get; set; }
        }

        // for building dropdown list
        public List<Tuple<string, string>> OrderStatusCodes { get; set; }
        
        public string SelectedOrderStatus { get; set; }
        public bool ShowPending { get; set; }
        [Display(Name = "Created After")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "Created Before")]
        public DateTime? EndDate { get; set; }
        public ColumnPreferences ColumnPreferences { get; set; }
        public string ShowLast { get; set; }

        /// <summary>
        /// Returns true if the user wants to view a column that requires order tracking info
        /// </summary>
        /// <remarks>
        /// Anything that shows "acted on" info will require order tracking history info
        /// </remarks>
        /// <returns></returns>
        public bool RequresOrderTracking()
        {
            return ColumnPreferences.ShowDaysNotActedOn || ColumnPreferences.ShowLastActedOnBy ||
                   ColumnPreferences.ShowLastActedOnDate;
        }

        public void PopulateStatusCodes(IRepositoryWithTypedId<OrderStatusCode, string> statusCodeRepository, List<Tuple<string, string>> orderStatusCodes = null)
        {
            if (orderStatusCodes == null)
            {
                OrderStatusCodes = new List<Tuple<string, string>> {new Tuple<string, string>("All", "All")};
                OrderStatusCodes.AddRange(statusCodeRepository.Queryable
                    .Where(a => a.ShowInFilterList)
                    .OrderBy(a => a.Level)
                    .Select(a => new Tuple<string, string>(a.Id, a.Name))
                    .ToList());
            }
            else
            {
                OrderStatusCodes = orderStatusCodes;
            }
        }

        public List<OrderTracking> OrderTracking { get; set; }

        public bool RequiresSplits()
        {
            return ColumnPreferences.ShowAccountNumber;
        }

        public bool RequiresApprovals()
        {
            return ColumnPreferences.ShowApprover || ColumnPreferences.ShowAccountManager ||
                   ColumnPreferences.ShowPurchaser;
        }

        public string GetNameFromApprovalsForOrder(string orderStatusCodeId, int orderId)
        {
            //TODO: why are we doing first for default?
            var apprv = Approvals.Where(x => x.Order.Id == orderId)
                    .FirstOrDefault(a => a.StatusCode.Id == orderStatusCodeId && a.User != null);
            if (apprv == null)
            {
                return "[Workgroup]";
            }
            if (apprv.User.IsActive && !apprv.User.IsAway) //User is not away show them
            {
                return apprv.User.FullName;
            }
            if (apprv.SecondaryUser != null && apprv.SecondaryUser.IsActive && !apprv.SecondaryUser.IsAway) //Primary user is away, show Secondary if active and not away
            {
                return apprv.SecondaryUser.FullName;
            }
            return "[Workgroup*]";
        }

        public List<Split> Splits { get; set; }

        public List<Approval> Approvals { get; set; }
    }
}