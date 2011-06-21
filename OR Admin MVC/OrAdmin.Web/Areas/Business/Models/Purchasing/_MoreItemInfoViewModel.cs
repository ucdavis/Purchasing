using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OrAdmin.Entities.DataAnnotations.Validation;

namespace OrAdmin.Web.Areas.Business.Models.Purchasing
{
    public class _MoreItemInfoViewModel
    {
        public int Index { get; set; }
        public SelectList CommodityTypes { get; set; }

        [DisplayName("Item URL (must start with: http:// or https://)")]
        [StringLength(1024)]
        [Website]
        public string Url { get; set; }

        [StringLength(1024)]
        [DisplayName("Item notes")]
        public string Notes { get; set; }

        [StringLength(1024)]
        [DisplayName("Promotional code")]
        public string PromoCode { get; set; }

        [DisplayName("Commodity type")]
        public int CommodityTypeId { get; set; }
    }
}