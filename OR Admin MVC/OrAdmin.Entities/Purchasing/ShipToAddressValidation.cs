using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrAdmin.Entities.Purchasing
{
    [MetadataType(typeof(ShipToAddressValidation))]
    public partial class ShipToAddress
    {
        //...
    }

    public class ShipToAddressValidation
    {
        [StringLength(512, MinimumLength = 3, ErrorMessage = "Must be between 3 - 512 characters")]
        [Required(ErrorMessage = "*")]
        [DisplayName("Friendly name")]
        public string FriendlyName { get; set; }

        [StringLength(512, MinimumLength = 3, ErrorMessage = "Must be between 3 - 512 characters")]
        [Required(ErrorMessage = "You must enter a campus")]
        public string Campus { get; set; }

        [StringLength(64, MinimumLength = 3, ErrorMessage = "Must be between 3 - 64 characters")]
        [Required(ErrorMessage = "You must enter a building")]
        public string Building { get; set; }

        [StringLength(256, MinimumLength = 3, ErrorMessage = "Must be between 3 - 256 characters")]
        [Required(ErrorMessage = "You must enter a street")]
        public string Street { get; set; }

        [StringLength(64, MinimumLength = 3, ErrorMessage = "Must be between 3 - 64 characters")]
        [Required(ErrorMessage = "You must enter a room")]
        public string Room { get; set; }

        [StringLength(64, ErrorMessage = "City must be under 64 characters in length")]
        [Required(ErrorMessage = "You must enter a city")]
        public string City { get; set; }

        [StringLength(2, MinimumLength = 2, ErrorMessage = "State must be 2 characters")]
        [Required(ErrorMessage = "You must enter a state")]
        public string State { get; set; }

        [StringLength(64, MinimumLength = 5, ErrorMessage = "Must be between 5 - 64 characters")]
        [Required(ErrorMessage = "You must enter a zip")]
        public string Zip { get; set; }

        [StringLength(64, MinimumLength = 7, ErrorMessage = "Must be between 7 - 64 characters")]
        [Required(ErrorMessage = "You must enter a phone number")]
        [DisplayName("Phone number")]
        public string Phone { get; set; }

        [StringLength(64, MinimumLength = 7, ErrorMessage = "Must be between 7 - 64 characters")]
        [Required(ErrorMessage = "You must enter a fax number")]
        public string Fax { get; set; }
    }
}
