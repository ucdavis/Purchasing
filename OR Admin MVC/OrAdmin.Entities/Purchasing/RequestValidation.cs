using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OrAdmin.Entities.DataAnnotations.Validation;

namespace OrAdmin.Entities.Purchasing
{
    [MetadataType(typeof(RequestValidation))]
    public partial class Request
    {
        //...
    }

    public class RequestValidation
    {
        [StringLength(64, ErrorMessage = "RequesterId must be under 64 characters")]
        [Required(ErrorMessage = "A requester is required")]
        public string RequesterId { get; set; }

        [StringLength(64, ErrorMessage = "PurchaserId must be under 64 characters")]
        [Required(ErrorMessage = "You must select a purchasing agent")]
        [DisplayName("Purchasing agent")]
        public string PurchaserId { get; set; }

        [Required(ErrorMessage = "You must select a vendor")]
        public int VendorId { get; set; }

        [Required(ErrorMessage = "A request type is required")]
        public int RequestType { get; set; }

        [DisplayName("Date needed")]
        public DateTime? DateNeeded { get; set; }

        [DisplayName("Okay to backorder")]
        public bool OkayToBackorder { get; set; }

        [DisplayName("Perishable")]
        public bool IsPerishable { get; set; }

        [DisplayName("Radioactive")]
        public bool IsRadioactive { get; set; }

        [DisplayName("Comments and/or special instructions")]
        public bool Comments { get; set; }

        [Required(ErrorMessage = "You must select a shipping method")]
        [DisplayName("Shipping method")]
        public string ShippingMethodId { get; set; }

        [DisplayName("RUA")]
        public string Rua { get; set; }

        [Required(ErrorMessage = "You must enter an account name")]
        [DisplayName("Account name")]
        public string RequestedAccount { get; set; }

        [Required(ErrorMessage = "You must select a delivery location")]
        [DisplayName("Delivery location")]
        public int ShipToId { get; set; }

        [StringLength(512, ErrorMessage = "Delivery location contact must be under 512 characters")]
        [Required(ErrorMessage = "You must enter a delivery location contact person")]
        [DisplayName("Deliver to")]
        public string ShipToName { get; set; }

        [StringLength(256, ErrorMessage = "Delivery location contact e-mail must be under 256 characters")]
        [Required(ErrorMessage = "You must enter the delivery location contact person's e-mail address")]
        [Email(ErrorMessage = "You must enter a valid delivery location contact e-mail address")]
        [DisplayName("E-mail")]
        public string ShipToEmail { get; set; }
    }
}
