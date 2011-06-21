using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OrAdmin.Entities.Purchasing;

namespace OrAdmin.Web.Areas.Business.Models.Purchasing
{
    public class _AddCommentViewModel
    {
        public int RequestId { get; set; }
        public Guid RequestUniqueId { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }

        [Required(ErrorMessage = "You must enter some comments.")]
        public string Comments { get; set; }

        [DisplayName("Notify requester of new comments")]
        public bool NotifyRequester { get; set; }

        [DisplayName("Notify approver of new comments")]
        public bool NotifyApprover { get; set; }

        [DisplayName("Notify manager of new comments")]
        public bool NotifyManager { get; set; }

        [DisplayName("Notify purchasing agent of new comments")]
        public bool NotifyAdmin { get; set; }
    }
}