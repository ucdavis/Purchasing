using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Purchasing.Mvc.Models
{
    public class AwayStatusViewModel
    {
        [Display(Name = "Away until")]
        [DataType(DataType.Date)]
        public DateTime? AwayUntil { get; set; }

        [BindNever]
        public bool IsAway { get; set; }

        [BindNever]
        public DateTime? CurrentAwayUntil { get; set; }
    }
}
