using System;
using System.ComponentModel.DataAnnotations;

namespace OrAdmin.Web.Areas.Business.Models.Purchasing
{
    public class _StopRequestViewModel
    {
        public int RequestId { get; set; }
        public Guid RequestUniqueId { get; set; }
        [Required(ErrorMessage = "You must explain why you are stopping this request.")]
        public string Comments { get; set; }
    }
}