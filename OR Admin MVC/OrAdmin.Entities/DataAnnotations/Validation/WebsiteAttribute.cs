using System;
using System.ComponentModel.DataAnnotations;

namespace OrAdmin.Entities.DataAnnotations.Validation
{
    public class WebsiteAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // If value is missing, defer to required field validator if available
            if (value == null)
                return ValidationResult.Success;

            // Test for a valid date by attempting to convert to DateTime

            Uri valid = null;
            if ((value.ToString().StartsWith("http://") || value.ToString().StartsWith("https://")) &&
                Uri.TryCreate(value.ToString(), UriKind.Absolute, out valid))
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage);

        }
    }
}
