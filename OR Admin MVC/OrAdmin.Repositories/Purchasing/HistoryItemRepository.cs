using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Repositories.Purchasing
{
    public class HistoryItemRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IEnumerable<HistoryItem> GetHistoryItems()
        {
            return
                (from mm in dc.MilestoneMaps
                 select new HistoryItem
                 {
                     Comments = mm.Comments,
                     Name = mm.Milestone.MilestoneName,
                     ImageName = mm.Milestone.ImageName,
                     Description = mm.Milestone.Description,
                     RequestId = mm.RequestId,
                     RequestUniqueId = mm.Request.UniqueId,
                     SubmittedBy = mm.SubmittedBy,
                     SubmittedOn = mm.SubmittedOn,
                     TypeValue = mm.MilestoneValue,
                     IsMilestone = true

                 }
                ).Union(
                from em in dc.EventMaps
                select new HistoryItem
                {
                    Comments = em.Comments,
                    Name = em.Event.EventName,
                    ImageName = em.Event.ImageName,
                    Description = em.Event.Description,
                    RequestId = em.RequestId,
                    RequestUniqueId = em.Request.UniqueId,
                    SubmittedBy = em.SubmittedBy,
                    SubmittedOn = em.SubmittedOn,
                    TypeValue = em.EventValue,
                    IsMilestone = false
                }
                ).OrderByDescending(hi => hi.SubmittedOn);
        }

        public IEnumerable<HistoryItem> GetHistoryByRequest(int requestId)
        {
            return GetHistoryItems().Where(hi => hi.RequestId == requestId);
        }

        public IEnumerable<HistoryItem> GetHistoryByUser(string userName)
        {
            return GetHistoryItems().Where(hi => hi.SubmittedBy.ToLower() == userName.ToLower());
        }
    }
}
