using System;

namespace OrAdmin.Entities.Purchasing
{
    public class HistoryItem
    {
        public int RequestId { get; set; }
        public Guid RequestUniqueId { get; set; }
        public int TypeValue { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string Description { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedOn { get; set; }
        public string Comments { get; set; }
        public bool IsMilestone { get; set; }
    }
}
