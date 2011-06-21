using System;
using System.Collections.Generic;
using System.Linq;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Repositories.Purchasing
{
    public class CommentRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IQueryable<RequestComment> GetComments()
        {
            return dc.RequestComments.OrderByDescending(c => c.SubmittedOn);
        }

        public IQueryable<RequestComment> GetCommentsByUser(string userName)
        {
            return GetComments().Where(c => c.SubmittedBy == userName);
        }

        public IQueryable<RequestComment> GetCommentsByRequest(int requestId)
        {
            return GetComments().Where(c => c.RequestId == requestId);
        }

        public void InsertComment(RequestComment comment)
        {
            dc.RequestComments.InsertOnSubmit(comment);
        }

        public void Delete(RequestComment comment)
        {
            // Mark for deletion
            dc.RequestComments.DeleteOnSubmit(comment);
        }

        public void Save()
        {
            dc.SubmitChanges();
        }
    }
}
