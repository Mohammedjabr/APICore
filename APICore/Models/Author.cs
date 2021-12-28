using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Models
{
    public class Author
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string Location { get; set; }

        public List<Book> Books { get; set; }
    }
}
