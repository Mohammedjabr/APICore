using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Models
{
    public class Author
    {
        [Key]
     // [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Invalid Empty Name")]
        [MaxLength(100)]
        public string AuthorName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Location { get; set; }


        public bool IsDeleted { get; set; }

        public List<Book> Books { get; set; }
    }
}
