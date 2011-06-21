using System.Linq;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Repositories.Purchasing
{
    public class CommodityTypeRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IQueryable<CommodityType> GetCommodityTypes()
        {
            return dc.CommodityTypes;
        }

        public IQueryable<CommodityType> GetCommodityTypesByUnit(int? unitId)
        {
            return dc.CommodityTypes.Where(c => c.UnitId == null || c.UnitId == unitId);
        }
    }
}
