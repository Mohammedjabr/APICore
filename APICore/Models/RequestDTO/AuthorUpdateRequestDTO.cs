using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Models.RequestDTO
{
    public class AuthorUpdateRequestDTO
    {
        public string AuthorName { get; set; }
        public string Location { get; set; }
    }
}
