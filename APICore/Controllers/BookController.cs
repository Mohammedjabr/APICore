using APICore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Controllers
{
    [Route("api/Author/{AuthorId}/Book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public static List<Book> BookList = new List<Book>
        {
            new Book(){BookId =1,BookName="Computer Information",PaperCount=100,PublishYear=2000,AuthorId=20 },
             new Book(){BookId =2,BookName="Database Introduction",PaperCount=150,PublishYear=2020,AuthorId=20 },
              new Book(){BookId =3,BookName="Data Structured",PaperCount=250,PublishYear=2005,AuthorId=30 },
               new Book(){BookId =4,BookName="Operating System",PaperCount=120,PublishYear=2008,AuthorId=20 },
                new Book(){BookId =5,BookName="Asp Net Core 3 API",PaperCount=190,PublishYear=2010,AuthorId=30 },
        };

        [HttpGet]
        public IActionResult GetBookById(int AuthorId)
        {
            return Ok(BookList.Where(x => x.AuthorId == AuthorId).ToList());
        }

        [HttpGet("{BookId}",Name="SingleBookRoute")]
        public IActionResult AllAuthorBooks(int AuthorId, int BookId)
        {
            var curBook = BookList.Where(m => m.AuthorId == AuthorId && m.BookId == BookId).SingleOrDefault();
            if (curBook == null)
                return NotFound("Book Not Found");
            return Ok(curBook);
        }

        [HttpPost]
        public IActionResult AddBook(int AuthorId, Book newBook)
        {

            if (!AuthorController.Authors.Any(m => m.AuthorId == AuthorId))
                return NotFound("Author Is Not Exist!");

            if (AuthorId != newBook.AuthorId)
                return BadRequest("Invalid Author Id");

            if (BookList.Any(m => m.AuthorId == AuthorId && m.BookId == newBook.BookId))
                return Conflict("Book Is Already Exist!");

            BookList.Add(newBook);
            return CreatedAtRoute("SingleBookRoute", new { AuthorId, newBook.BookId },
                      newBook);
            //return CreatedAtAction(nameof(GetBookById), new { AuthorId, newBook.BookId },
            //          newBook);

        }
        [HttpDelete("{BookId}")]
        public IActionResult DeleteBook(int AuthorId, int BookId)
        {
            if (!AuthorController.Authors.Any(m => m.AuthorId == AuthorId))
                return NotFound("Author Is Not Exist!");

            var CurBook = BookList.Where(m => m.AuthorId == AuthorId && m.BookId == BookId).SingleOrDefault();
            if (CurBook == null)
               return NotFound("Book Is Not Found!");

           

            BookList.Remove(CurBook);
            return NoContent();
        }
    }
}
