using APICore.Models;
using APICore.Models.RequestDTO;
using APICore.Models.ResponseDTO;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace APICore.Controllers
{
    [Route("api/Author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        public static List<Author> Authors = new List<Author>()
        {
             new Author(){AuthorId =10,AuthorName="Ali",Location = "Gaza",IsDeleted=false,Books = new List<Book>()},
             new Author(){AuthorId =20,AuthorName="Mohammed",Location = "AlBurij",IsDeleted=false,Books = new List<Book>()},
              new Author(){AuthorId =30,AuthorName="Mohanad",Location = "Gaza",IsDeleted=false,Books = new List<Book>()},
        };
        private readonly IMapper mapper;

        public AuthorController(IMapper Mapper)
        {
            mapper = Mapper;
        }

        [HttpGet]
        public IActionResult GetAllAuthors(//[FromQuery]string FilterAuthorName,[FromQuery] string Location
                                           [FromQuery] string SearchingString,
                                           string orderby,
                                           int RowCount,int PageNumber)
        {
            var AuthorQuery = Authors.AsQueryable();
            AuthorQuery = AuthorQuery.Where(m => m.IsDeleted == false);
            //if (!string.IsNullOrWhiteSpace(FilterAuthorName))
            //{
            //    AuthorQuery = AuthorQuery.Where(m => m.AuthorName.Equals(FilterAuthorName));
            //}

            //if (!string.IsNullOrWhiteSpace(Location))
            //{
            //    AuthorQuery = AuthorQuery.Where(m => m.Location.Contains(Location,StringComparison.OrdinalIgnoreCase));
            //}

            if (!string.IsNullOrWhiteSpace(SearchingString))
            {
                AuthorQuery = AuthorQuery.Where(m => m.AuthorName.Equals(SearchingString) ||
                                                m.Location.Contains(SearchingString,StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(orderby))
            {
                AuthorQuery = AuthorQuery.OrderBy(orderby);
            }
            else
            {
                AuthorQuery = AuthorQuery.OrderBy(x=>x.AuthorName);
            }

            if (PageNumber == 0)
                PageNumber = 1;
            if (RowCount == 0)
                RowCount = 10;
            AuthorQuery = AuthorQuery.Skip((PageNumber - 1) * RowCount).Take(RowCount);
           
            return Ok(AuthorQuery.Select(m=> mapper.Map<AuthorResponseDTO>(m)));
        }

        [HttpGet("{Id}")]
        public IActionResult GetAuthorById(int id)
        {
            var CurAuthor = Authors.Where(x => x.AuthorId == id && x.IsDeleted == false).FirstOrDefault();
            if (CurAuthor == null)
            {
                return NotFound(new { ErrorCode = 320, Message = "Invalid Author Id" });
            }

            //Mapping Domain To Response DTO
            //AuthorResponseDTO ReturnAuthor = new AuthorResponseDTO()
            //{
            //    AuthorId = CurAuthor.AuthorId,
            //    AuthorName = CurAuthor.AuthorName,
            //    Location = CurAuthor.Location,
            //    BookCount = (CurAuthor.Books == null) ? 0 : CurAuthor.Books.Count()
            //};
            return Ok(mapper.Map<AuthorResponseDTO>(CurAuthor));
        }

        [HttpPost]
        public IActionResult AddAuthor([FromBody]AuthorAddRequestDTO newAuthor)
        {
            //check Name duplication
            if (Authors.Any(x => x.AuthorName == newAuthor.AuthorName))
            {
                // return BadRequest("Dublicated Author Name");
                ModelState.AddModelError("AuthorName", "Dublicated Author Name");
                return ValidationProblem();
            }
            

            //if (!ModelState.IsValid)
            //{
            //    // return BadRequest(ModelState);
            //    return ValidationProblem();
            //}

            //if (String.IsNullOrWhiteSpace(newAuthor.AuthorName))
            //{
            //    return BadRequest(new { ErrorCode = 501, ErrorMessage = "Invalid Empty Author Name" });
            //}
            //var CurAuthor = Authors.Where(m => m.AuthorId == newAuthor.AuthorId && m.IsDeleted == false).SingleOrDefault();
            //if (CurAuthor != null)
            //{
            //    return Conflict(new { ErrorCode = 502, ErrorMessage = "Duplicate in Author Id" });

            //}

            //Author CurAuthor = new Author()
            //{
            //    AuthorId = Authors.Max(x => x.AuthorId) + 1,
            //    AuthorName = newAuthor.AuthorName,
            //    Location = newAuthor.Location,
            //    IsDeleted = false,
            //    Books = new List<Book>()
            //};

            //Mapping
             var CurAuthor = mapper.Map<Author>(newAuthor);
            CurAuthor.AuthorId = Authors.Max(x => x.AuthorId) + 1;
            Authors.Add(CurAuthor);

            return CreatedAtAction("GetAuthorById", new { id = CurAuthor.AuthorId }, MapDomainToResponse(CurAuthor));
        }

        [HttpPut("{AuthorId}")]
        public IActionResult UpdateAuthor(int AuthorId, AuthorUpdateRequestDTO newAuthor)
        {
            if (String.IsNullOrWhiteSpace(newAuthor.AuthorName))
            {
                return BadRequest(new { ErrorCode = 501, ErrorMessage = "Invalid Empty Author Name" });
            }

            var CurAuthor = Authors.Where(m => m.AuthorId == AuthorId).SingleOrDefault();
            if (CurAuthor == null)
            {
                return NotFound(new { ErrorCode = 503, ErrorMessage = "Invalid Author Id" });
            }

            //CurAuthor.AuthorName = newAuthor.AuthorName;
            //CurAuthor.Location = newAuthor.Location;
            //Mapping
            mapper.Map(newAuthor,CurAuthor);
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

            if (!TryValidateModel(CurAuthor))
                return ValidationProblem();
          

            return NoContent();
        }

        [HttpDelete("{AuthorId}")]
        public IActionResult Delete(int AuthorId)
        {
            var CurAuthor = Authors.Where(m => m.AuthorId == AuthorId).SingleOrDefault();
            if (CurAuthor == null)
            {
                return NotFound(new { ErrorCode = 503, ErrorMessage = "Invalid Author Id" });
            }
            //check if he has book
            if (BookController.BookList.Any(m => m.AuthorId == AuthorId))
            {
                return BadRequest("This Author has Dependances");
            }
            // Authors.Remove(CurAuthor);
            CurAuthor.IsDeleted = true;
            return NoContent();
        }

        private AuthorResponseDTO MapDomainToResponse(Author x)
        {
           return new AuthorResponseDTO()
            {
                AuthorId = x.AuthorId,
                AuthorName = x.AuthorName,
                Location = x.Location,
                BookCount = (x.Books == null) ? 0 : x.Books.Count()
            };
        }
    }
}
