using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OrAdmin.Entities.DataAnnotations.Validation;

namespace OrAdmin.Entities.Purchasing
{
    [MetadataType(typeof(VendorValidation))]
    public partial class Vendor
    {
        //...
    }

    [Bind(Exclude = "Type")]
    public class VendorValidation
    {
        [StringLength(512, MinimumLength = 3, ErrorMessage = "Must be between 3 - 512 characters")]
        [Required(ErrorMessage = "Friendly name is required")]
        [DisplayName("Friendly name")]
        public string FriendlyName { get; set; }

        [StringLength(256, MinimumLength = 3, ErrorMessage = "Must be between 3 - 256 characters")]
        [Required(ErrorMessage = "You must enter a vendor name")]
        [DisplayName("Vendor name")]
        public string Name { get; set; }

        [StringLength(512, MinimumLength = 3, ErrorMessage = "Must be between 3 - 512 characters")]
        [Required(ErrorMessage = "You must enter a vendor address")]
        public string Address { get; set; }

        [StringLength(64, ErrorMessage = "City must be under 64 characters in length")]
        [Required(ErrorMessage = "You must enter a vendor city")]
        public string City { get; set; }

        [StringLength(2, MinimumLength = 2, ErrorMessage = "State must be 2 characters")]
        [Required(ErrorMessage = "You must enter a vendor state")]
        public string State { get; set; }

        [StringLength(64, MinimumLength = 5, ErrorMessage = "Must be between 5 - 64 characters")]
        [Required(ErrorMessage = "You must enter a vendor zip")]
        public string Zip { get; set; }

        [StringLength(64, MinimumLength = 7, ErrorMessage = "Must be between 7 - 64 characters")]
        [Required(ErrorMessage = "You must enter a vendor phone number")]
        [DisplayName("Phone number")]
        public string Phone { get; set; }

        [StringLength(64, MinimumLength = 7, ErrorMessage = "Must be between 7 - 64 characters")]
        [Required(ErrorMessage = "You must enter a vendor fax number")]
        public string Fax { get; set; }

        [StringLength(512, ErrorMessage = "URL must be under 512 characters in length")]
        [Website(ErrorMessage = "You must enter a valid web address")]
        [DisplayName("Website address (must start with: http:// or https://)")]
        public string Url { get; set; }
    }
}
