using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;

namespace OrAdmin.Entities.Purchasing
{
    [MetadataType(typeof(AttachmentValidation))]
    public partial class Attachment
    {
        //...
    }

    public class AttachmentValidation
    {
        [Required(ErrorMessage = "A filename is required")]
        [DisplayName("File Name")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "File size is required")]
        [DisplayName("File size")]
        public double FileSizeBytes { get; set; }

        [Required(ErrorMessage = "The username responsible for submitting this file is required")]
        [DisplayName("Submitted by")]
        public string SubmittedBy { get; set; }

        [Required(ErrorMessage = "The the date/time this file was submitted is required")]
        [DisplayName("Submitted on")]
        public DateTime SubmittedOn { get; set; }
    }
}
