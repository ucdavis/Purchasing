using System.Collections.Generic;
using System.Linq;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Repositories.Purchasing
{
    public class DpoItemRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IQueryable<DpoItem> GetDpoItems()
        {
            return dc.DpoItems.OrderBy(i => i.SubmittedOn);
        }

        public void InsertDpoItem(DpoItem item)
        {
            dc.DpoItems.InsertOnSubmit(item);
        }

        public void Delete(DpoItem item)
        {
            // Mark for deletion
            dc.DpoItems.DeleteOnSubmit(item);
        }

        public void DeleteAll(IEnumerable<DpoItem> items)
        {
            // Mark for deletion
            dc.DpoItems.DeleteAllOnSubmit(items);
        }

        public void Save()
        {
            dc.SubmitChanges();
        }
    }
}
