using System.Collections.Generic;
using System.Linq;
using OrAdmin.Entities.Purchasing;
using OrAdmin.Repositories.Purchasing;

namespace OrAdmin.Web.Areas.Business.Models.Purchasing
{
    public class MyRequestsViewModel
    {
        public IQueryable<Request> Requests { get; set; }
        public IEnumerable<RequestRepository.MyRequestFilterListItem> FilterList { get; set; }
        public RequestRepository.MyRequestFilter Filter { get; set; }
    }
}