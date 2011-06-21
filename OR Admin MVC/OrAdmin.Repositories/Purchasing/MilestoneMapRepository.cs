using System.Linq;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Repositories.Purchasing
{
    public class MilestoneMapRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IQueryable<MilestoneMap> GetMilestoneMapsByRequest(int requestId)
        {
            return dc.MilestoneMaps.Where(mm=>mm.RequestId == requestId).OrderByDescending(map => map.SubmittedOn);
        }

        public void InsertMilestone(MilestoneMap map)
        {
            dc.MilestoneMaps.InsertOnSubmit(map);
        }

        public void Delete(MilestoneMap map)
        {
            // Mark for deletion
            dc.MilestoneMaps.DeleteOnSubmit(map);
        }

        public void Save()
        {
            dc.SubmitChanges();
        }

        public enum MilestoneType
        {
            // Stopped
            Stopped = -1,

            // Saved
            Saved = 0,

            // Pending approval
            Requested = 1,
            Modified = 2,
            PIBackToRequester = 3,
            ManagerBackToRequester = 4,
            AdminBackToRequester = 5,

            // PI approved
            PIApproved = 6,
            ManagerBackToPI = 7,
            ManagerApproved = 8,
            AdminBackToPI = 9,

            // Placed
            Placed = 10,

            // Receipt
            ShippingVendorConflict = 11,
            NotReceived = 12,
            Received = 13
        }
    }
}
