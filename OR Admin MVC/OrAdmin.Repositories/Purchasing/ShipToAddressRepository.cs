using System.Linq;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Repositories.Purchasing
{
    public class ShipToAddressRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IQueryable<ShipToAddress> GetAddresses(int unitId)
        {
            return dc.ShipToAddresses.Where(a => a.UnitId == unitId).OrderBy(a => a.FriendlyName);
        }

        public void InsertAddress(ShipToAddress address)
        {
            dc.ShipToAddresses.InsertOnSubmit(address);
        }

        public bool AddressFriendlyNameExists(string friendlyName, int unitId)
        {
            return GetAddresses(unitId).Where(v => v.FriendlyName.ToLower() == friendlyName.ToLower()).Any();
        }

        public void Delete(ShipToAddress address)
        {
            // Mark for deletion
            dc.ShipToAddresses.DeleteOnSubmit(address);
        }

        public void Save()
        {
            dc.SubmitChanges();
        }
    }
}
