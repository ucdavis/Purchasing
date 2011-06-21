using System.Linq;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Repositories.Purchasing
{
    public class EventMapRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IQueryable<EventMap> GetEventMapsByRequest(int requestId)
        {
            return dc.EventMaps.Where(em => em.RequestId == requestId).OrderByDescending(map => map.SubmittedOn);
        }

        public void InsertEventMap(EventMap map)
        {
            dc.EventMaps.InsertOnSubmit(map);
        }

        public void Delete(EventMap map)
        {
            // Mark for deletion
            dc.EventMaps.DeleteOnSubmit(map);
        }

        public void Save()
        {
            dc.SubmitChanges();
        }

        public enum EventMapType
        {
            Stop_Requested = 0,
            Stopped = 1,
            Locked = 2,
            Unlocked = 3,
            Admin_Update = 4
        }
    }
}
