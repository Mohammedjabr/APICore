using APICore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Controllers
{
    [Route("api/Author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        public static List<Author> Authors = new List<Author>()
        {
             new Author(){AuthorId =10,AuthorName="Ali",Location = "Gaza"},
             new Author(){AuthorId =20,AuthorName="Mohammed",Location = "AlBurij"},
              new Author(){AuthorId =30,AuthorName="Mohanad",Location = "Gaza"},
        };

        [HttpGet]
        public List<Author> GetAllAuthors()
        {
            return Authors;
        }

        [HttpGet("{Id}")]
        public IActionResult GetAuthorById(int id)
        {
            var CurAuthor = Authors.Where(x => x.AuthorId == id).FirstOrDefault();
            if (CurAuthor == null)
            {
                return NotFound(new { ErrorCode = 320, Message = "Invalid Author Id" });
            }
            return Ok(CurAuthor);
        }

        [HttpPost]
        public IActionResult AddAuthor(Author newAuthor)
        {
            if (String.IsNullOrWhiteSpace(newAuthor.AuthorName))
            {
                return BadRequest(new { ErrorCode = 501, ErrorMessage = "Invalid Empty Author Name" });
            }
            var CurAuthor = Authors.Where(m => m.AuthorId == newAuthor.AuthorId).SingleOrDefault();
            if (CurAuthor != null)
            {
                return Conflict(new { ErrorCode = 502, ErrorMessage = "Duplicate in Author Id" });

            }

            Authors.Add(newAuthor);
            return CreatedAtAction("GetAuthorById", new { id = newAuthor.AuthorId }, newAuthor);
        }

        [HttpPut]
        public IActionResult UpdateAuthor(Author newAuthor)
        {
            if (String.IsNullOrWhiteSpace(newAuthor.AuthorName))
            {
                return BadRequest(new { ErrorCode = 501, ErrorMessage = "Invalid Empty Author Name" });
            }

            var CurAuthor = Authors.Where(m => m.AuthorId == newAuthor.AuthorId).SingleOrDefault();
            if (CurAuthor == null)
            {
                return NotFound(new { ErrorCode = 503, ErrorMessage = "Invalid Author Id" });
            }

            CurAuthor.AuthorName = newAuthor.AuthorName;
            CurAuthor.Location = newAuthor.Location;

            return NoContent();//Ok(CurAuthor);
        }

        [HttpPatch("{Authorid}")]
        public IActionResult UpdateAuthorPartialy(int Authorid, JsonPatchDocument AuhtorPatch)
        {
            var CurAuthor = Authors.Where(m => m.AuthorId == Authorid).SingleOrDefault();
            if (CurAuthor == null)
            {
                return NotFound(new { ErrorCode = 503, ErrorMessage = "Invalid Author Id" });
            }

            AuhtorPatch.ApplyTo(CurAuthor);

            return NoContent();
        }
    }
}
