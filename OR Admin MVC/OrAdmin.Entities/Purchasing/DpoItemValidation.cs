using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OrAdmin.Entities.DataAnnotations.Validation;

namespace OrAdmin.Entities.Purchasing
{
    [MetadataType(typeof(DpoItemValidation))]
    public partial class DpoItem
    {
        //...
    }

    public class DpoItemValidation
    {
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "The item quantity must be greater than 0")]
        public int Quantity { get; set; }

        [StringLength(512, MinimumLength = 3, ErrorMessage = "Catalog number must be between 3 - 512 characters")]
        [Required(ErrorMessage = "You must enter an item catalog number")]
        [DisplayName("Catalog number")]
        public string CatalogNumber { get; set; }

        [StringLength(256, MinimumLength = 2, ErrorMessage = "Unit must be between 2 - 256 characters")]
        [Required(ErrorMessage = "You must enter an item unit")]
        public string Unit { get; set; }

        [StringLength(int.MaxValue, MinimumLength = 2, ErrorMessage = "Description must be more than 2 characters")]
        [Required(ErrorMessage = "You must enter an item description")]
        [DisplayName("Complete description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "An item price is required")]
        [Range(0, int.MaxValue, ErrorMessage = "The item price must be greater than $0.00")]
        [DisplayName("Unit price")]
        public decimal PricePerUnit { get; set; }

        [StringLength(1024, ErrorMessage = "URL must be between less than 1024 characters")]
        [DisplayName("Item URL (must start with: http://)")]
        [Website(ErrorMessage = "Not a valid website address for this item")]
        public string Url { get; set; }

        [StringLength(1024, ErrorMessage = "Item notes must be between less than 1024 characters")]
        [DisplayName("Item notes")]
        public string Notes { get; set; }

        [StringLength(1024, ErrorMessage = "This item's promo code must be less than 1024 characters")]
        [DisplayName("Promotional code")]
        public string PromoCode { get; set; }

        [Required(ErrorMessage = "You must select a commodity type for this item")]
        public int CommodityTypeId { get; set; }

        [Required(ErrorMessage = "The username of the person responsible for submitting this item is required")]
        public string SubmittedBy { get; set; }

        [Required(ErrorMessage = "The date/time when this item was submitted is required")]
        public DateTime SubmittedOn { get; set; }
    }
}
