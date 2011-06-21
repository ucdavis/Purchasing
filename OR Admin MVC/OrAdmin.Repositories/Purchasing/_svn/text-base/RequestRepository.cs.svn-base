using System;
using System.Linq;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Repositories.Purchasing
{
    public class RequestRepository
    {
        private PurchasingDataContext dc = new PurchasingDataContext();

        public IQueryable<Request> GetRequests()
        {
            return dc.Requests.OrderByDescending(r => r.SubmittedOn);
        }

        public IQueryable<Request> GetRequestsByUser(string userName)
        {
            return GetRequests().Where(
                r => r.RequesterId.ToLower() == userName.ToLower() ||
                     r.PurchaserId.ToLower() == userName.ToLower() ||
                     r.PiApprovals.Any(a => a.PiId.ToLower() == userName.ToLower())
                    );
        }

        public IQueryable<Request> GetRequestsByUser(string userName, MyRequestFilter filter)
        {
            return FilterMyRequests(GetRequestsByUser(userName), filter);
        }

        public IQueryable<Request> GetRequestsByUnit(int unitId)
        {
            return GetRequests().Where(r => 
                r.UnitId == unitId && 
                r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue != (int)MilestoneMapRepository.MilestoneType.Saved);
        }

        public IQueryable<Request> GetRequestsByUnit(int unitId, AdminRequestFilter filter)
        {
            return FilterAdminRequests(GetRequestsByUnit(unitId), filter);
        }

        private IQueryable<Request> FilterMyRequests(IQueryable<Request> requests, MyRequestFilter filter)
        {
            // Apply filter
            switch (filter)
            {
                case MyRequestFilter.Conflicted:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.ShippingVendorConflict ||
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.NotReceived);
                    break;
                case MyRequestFilter.Saved:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.Saved);
                    break;
                case MyRequestFilter.Received:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.Received);
                    break;
                case MyRequestFilter.Sendbacks:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.PIBackToRequester ||
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.ManagerBackToRequester ||
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.AdminBackToRequester ||
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.ManagerBackToPI ||
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.AdminBackToPI);
                    break;
                case MyRequestFilter.Pending_Approval:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue >= (int)MilestoneMapRepository.MilestoneType.Requested &&
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue <= (int)MilestoneMapRepository.MilestoneType.AdminBackToRequester);
                    break;
                case MyRequestFilter.Placed:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.Placed);
                    break;
                default: break;
            }

            return requests;
        }

        private IQueryable<Request> FilterAdminRequests(IQueryable<Request> requests, AdminRequestFilter filter)
        {
            // Apply filter
            switch (filter)
            {
                case AdminRequestFilter.Conflict:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.ShippingVendorConflict);
                    break;
                case AdminRequestFilter.Not_Received:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.NotReceived);
                    break;
                case AdminRequestFilter.Received:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.Received);
                    break;
                case AdminRequestFilter.Awaiting_PI:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue >= (int)MilestoneMapRepository.MilestoneType.Requested &&
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue <= (int)MilestoneMapRepository.MilestoneType.AdminBackToRequester);
                    break;
                case AdminRequestFilter.Awaiting_Manager:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.PIApproved ||
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.ManagerBackToPI);
                    break;
                case AdminRequestFilter.Manager_Approved:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.ManagerApproved);
                    break;
                case AdminRequestFilter.Placed:
                    requests = requests.Where(r =>
                        r.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue == (int)MilestoneMapRepository.MilestoneType.Placed);
                    break;
                default: break;
            }

            return requests;
        }

        public Request GetRequest(Guid uniqueId)
        {
            return dc.Requests.Where(r => r.UniqueId == uniqueId).SingleOrDefault();
        }

        public Request GetRequest(int requestId)
        {
            return dc.Requests.Where(r => r.Id == requestId).SingleOrDefault();
        }

        public void InsertRequest(Request request)
        {
            dc.Requests.InsertOnSubmit(request);
        }

        public void Delete(Request request)
        {
            // Mark for deletion
            dc.Requests.DeleteOnSubmit(request);
        }

        public void Save()
        {
            dc.SubmitChanges();
        }

        public enum MyRequestFilter
        {
            All,
            Saved,
            Pending_Approval,
            Placed,
            Sendbacks,
            Conflicted,
            Received
        }

        public class MyRequestFilterListItem
        {
            public RequestRepository.MyRequestFilter Filter { get; set; }
            public int RequestCount { get; set; }
            public bool Selected { get; set; }
        }

        public enum AdminRequestFilter
        {
            All,
            Awaiting_PI,
            Awaiting_Manager,
            Manager_Approved,
            Placed,
            Not_Received,
            Conflict,
            Received,
        }

        public class AdminRequestFilterListItem
        {
            public RequestRepository.AdminRequestFilter Filter { get; set; }
            public int RequestCount { get; set; }
            public bool Selected { get; set; }
        }

        public enum RequestType
        {
            Dpo = 0,
            Dro = 1,
            Ba = 2
        }
    }
}
