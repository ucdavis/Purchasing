using System.Collections.Generic;
using System.Linq;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Web.Areas.Business.Models.Purchasing
{
    public class IndexViewModel
    {
        public IQueryable<Request> Requests { get; set; }
        public IQueryable<RequestComment> RequestComments { get; set; }
        public IEnumerable<HistoryItem> HistoryItems { get; set; }
    }
}