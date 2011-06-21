using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OrAdmin.Entities.DataAnnotations.Validation;

namespace OrAdmin.Entities.App
{
    [MetadataType(typeof(ProfileValidation))]
    public partial class Profile
    {
        //...
    }

    [Bind(Exclude="UserName")]
    public class ProfileValidation
    {
        [StringLength(64, ErrorMessage = "User name must be under 64 characters")]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(64, ErrorMessage = "First name must be under 64 characters")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(64, ErrorMessage = "Last name must be under 64 characters")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "An e-mail is required")]
        [StringLength(128, ErrorMessage = "E-mail must be under 128 characters")]
        [DisplayName("E-mail")]
        [Email(ErrorMessage = "E-mail address is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Last updated date is required")]
        [DateTime(ErrorMessage = "Last updated date is not valid")]
        [DisplayName("Last Updated")]
        public string LastUpdated { get; set; }

        [DisplayName("Default Unit")]
        public int DefaultUnitId { get; set; }
    }
}
