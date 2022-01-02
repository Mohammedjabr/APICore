using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Models.ResponseDTO
{
    public class AuthorResponseDTO
    {
        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string Location { get; set; }

        public int BookCount { get; set; }
    }

}
