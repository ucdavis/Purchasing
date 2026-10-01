using System.ComponentModel.DataAnnotations;

namespace Purchasing.Mvc.Models
{
    public class LocalLoginModel
    {
        [Required]
        [StringLength(10)]
        [Display(Name = "Login ID")]
        public string UserId { get; set; }

        public string ReturnUrl { get; set; }
    }
}
