using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        public string BookName { get; set; }

        public int PublishYear { get; set; }

        public int PaperCount { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
