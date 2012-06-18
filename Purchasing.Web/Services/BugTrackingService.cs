using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;
using Purchasing.Web.Models;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Services
{
    public interface IBugTrackingService
    {
        void CheckForClearedOutSubAccounts(Order order, OrderViewModel.Split[] splits, OrderViewModel model);
    }

    public class BugTrackingService : IBugTrackingService
    {
        private readonly IRepository<BugTracking> _bugTrackingRepository;
        private readonly IUserIdentity _userIdentity;

        public BugTrackingService(IRepository<BugTracking> bugTrackingRepository ,IUserIdentity userIdentity)
        {
            _bugTrackingRepository = bugTrackingRepository;
            _userIdentity = userIdentity;
        }


        public void CheckForClearedOutSubAccounts(Order order, OrderViewModel.Split[] splits, OrderViewModel model)
        {
            try
            {
                if (splits == null || !splits.Any())
                {
                    var orderViewModelSplit = new OrderViewModel.Split{Account = model.Account,SubAccount = model.SubAccount,LineItemId = null};
                    splits = new OrderViewModel.Split[] { orderViewModelSplit };
                }

                var lastLineId = 0;
                var count = 0;
                foreach (var split in splits)
                {
                    if (string.IsNullOrWhiteSpace(split.SubAccount))
                    {
                        Split orderSplit = null;
                        if (split.LineItemId.HasValue)
                        {
                            if (split.LineItemId.Value != lastLineId)
                            {
                                lastLineId = split.LineItemId.Value;
                                count = 0;
                            }
                            else
                            {
                                count++;
                            }
                            var lineItem = order.LineItems.ElementAtOrDefault(split.LineItemId.Value - 1);
                            if (lineItem != null)
                            {
                                orderSplit = lineItem.Splits.ElementAtOrDefault(count);
                            }
                        }
                        else
                        {
                            orderSplit = order.Splits.FirstOrDefault(a => a.LineItem == null && a.Account == split.Account);
                        }

                        if (orderSplit != null)
                        {
                            if (orderSplit.Account == split.Account && !string.IsNullOrWhiteSpace(orderSplit.SubAccount))
                            {
                                //ok, the sub account has been cleared out.
                                var bugTracking = new BugTracking();
                                bugTracking.OrderId = order.Id;
                                bugTracking.UserId = _userIdentity.Current;
                                bugTracking.SplitId = orderSplit.Id;
                                bugTracking.TrackingMessage = string.Format("SubAccount Cleared Out for Account {0}. Previous value {1}", orderSplit.Account, orderSplit.SubAccount);
                                bugTracking.LineItemId = orderSplit.LineItem != null ? (int?)orderSplit.LineItem.Id : null;

                                _bugTrackingRepository.EnsurePersistent(bugTracking);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}