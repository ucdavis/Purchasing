using System.ComponentModel.DataAnnotations;

namespace Purchasing.Mvc.Models
{
    public class AddNewOrgEditModel
    {
        [Required]
        [StringLength(6)]
        public string ParentOrgCode { get; set; }
        [Required]
        [StringLength(6)]
        [RegularExpression(@"^[A-Z0-9]-[A-Z0-9]{4}$", ErrorMessage = "Org Code must be in the format X-XXXX.")]            
        public string OrgCode { get; set; }
        [Required]
        public string OrgName { get; set; }
    }
}
