using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Validations
{
    public class StringArrayValidation:ValidationAttribute
    {
        public String [] AllowStrings { get; set; }

        protected override ValidationResult IsValid(object value,ValidationContext validationContext)
        {
            if (AllowStrings.Contains(value))
                return ValidationResult.Success;


            return new ValidationResult("The Value is Out Of Allowed Options!");
        }
    }
}
