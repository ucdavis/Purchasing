using System.Linq;
using OrAdmin.Entities.Purchasing;
using System.Collections.Generic;

namespace OrAdmin.Repositories.Purchasing
{
    public class PiApprovalRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IQueryable<PiApproval> GetPiApprovals()
        {
            return dc.PiApprovals.OrderBy(a => a.RequestId).ThenByDescending(a => a.SubmittedOn);
        }

        public void InsertPiApproval(PiApproval approval)
        {
            dc.PiApprovals.InsertOnSubmit(approval);
        }

        public void Delete(PiApproval approval)
        {
            // Mark for deletion
            dc.PiApprovals.DeleteOnSubmit(approval);
        }

        public void Save()
        {
            dc.SubmitChanges();
        }
    }
}
