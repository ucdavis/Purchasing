using System.Collections.Generic;
using System.Linq;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Repositories.Purchasing
{
    public class VendorRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IQueryable<Vendor> GetVendors(int unitId, int vendorType)
        {
            return dc.Vendors.Where(v => v.UnitId == unitId && v.Type == vendorType).OrderBy(v => v.FriendlyName);
        }

        public IEnumerable<DaFISVendor> GetDaFISVendor(string whereClause)
        {
            return dc.sPUR_FindDaFISVendor(whereClause).ToList();
        }

        public void InsertVendor(Vendor vendor)
        {
            dc.Vendors.InsertOnSubmit(vendor);
        }

        public bool VendorNameExists(string name, int unitId, int vendorType)
        {
            return GetVendors(unitId, vendorType).Where(v => v.Name.ToLower() == name.ToLower()).Any();
        }

        public bool VendorFriendlyNameExists(string friendlyName, int unitId, int vendorType)
        {
            return GetVendors(unitId, vendorType).Where(v => v.FriendlyName.ToLower() == friendlyName.ToLower()).Any();
        }

        public void Delete(Vendor vendor)
        {
            // Mark for deletion
            dc.Vendors.DeleteOnSubmit(vendor);
        }

        public void Save()
        {
            dc.SubmitChanges();
        }
    }
}
