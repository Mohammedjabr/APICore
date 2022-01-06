using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Models.Entities
{
    public class Users
    {
        [Key]
        public string UserId { get; set; }
        [Required]
        [StringLength(150)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }
        [Required]
        [StringLength(200)]
        public byte[] Passwordhash { get; set; }
        [Required]
        [StringLength(200)]
        public byte[] Passwordsalt { get; set; }


    }
}
