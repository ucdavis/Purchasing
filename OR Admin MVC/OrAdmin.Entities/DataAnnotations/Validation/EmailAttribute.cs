using System;
using System.ComponentModel.DataAnnotations;

namespace OrAdmin.Entities.DataAnnotations.Validation
{
    public class EmailAttribute : RegularExpressionAttribute
    {
        public EmailAttribute()
            : base("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$")
        // @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
        {
            if (String.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = "You must enter a valid e-mail address";
        }
    }
}
