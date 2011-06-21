using System;
using System.ComponentModel.DataAnnotations;

namespace OrAdmin.Entities.DataAnnotations.Validation
{
    public class DateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // If value is missing, defer to required field validator if available
            if (value == null)
                return ValidationResult.Success;

            // Test for a valid date by attempting to convert to DateTime
            try
            {
                DateTime dt = Convert.ToDateTime(value);
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                return new ValidationResult(!String.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : "Date is not valid");
            }
        }
    }
}
