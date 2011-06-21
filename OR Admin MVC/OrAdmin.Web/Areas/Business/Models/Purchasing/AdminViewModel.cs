using System.Collections.Generic;
using System.Linq;
using OrAdmin.Entities.Purchasing;
using OrAdmin.Repositories.Purchasing;
using OrAdmin.Entities.App;

namespace OrAdmin.Web.Areas.Business.Models.Purchasing
{
    public class AdminViewModel
    {
        public IQueryable<Request> Requests { get; set; }
        public Unit Unit { get; set; }
        public IEnumerable<RequestRepository.AdminRequestFilterListItem> FilterList { get; set; }
        public RequestRepository.AdminRequestFilter Filter { get; set; }
    }
}