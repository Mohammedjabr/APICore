using APICore.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Models.RequestDTO
{
    public class AuthorAddRequestDTO
    {
        [Required]
        [StringLength(10)]
        public string AuthorName { get; set; }
        
        [StringLength(20)]
        [StringArrayValidation(AllowStrings = new String[] {"Gaza","Rafah","North Gaza" } )]
        public string Location { get; set; }

        //[EmailAddress]
        //public string AuthorEmail { get; set; }

        //[Range(minimum:1950,maximum:2015)]
        //public int BYear { get; set; }
    }
}
