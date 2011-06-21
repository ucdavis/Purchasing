using System.Linq;
using OrAdmin.Entities.Purchasing;
using System.Collections.Generic;

namespace OrAdmin.Repositories.Purchasing
{
    public class ShippingMethodRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IEnumerable<ShippingMethod> GetShippingMethods()
        {
            return dc.ShippingMethods.ToList();
        }

        public void InsertShippingMethod(ShippingMethod shippingMethod)
        {
            dc.ShippingMethods.InsertOnSubmit(shippingMethod);
        }

        public void Delete(ShippingMethod shippingMethod)
        {
            // Mark for deletion
            dc.ShippingMethods.DeleteOnSubmit(shippingMethod);
        }

        public void Save()
        {
            dc.SubmitChanges();
        }
    }
}
