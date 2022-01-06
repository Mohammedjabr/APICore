using APICore.Data;
using APICore.helper;
using APICore.Models;
using APICore.Models.RequestDTO;
using APICore.Models.ResponseDTO;
using APICore.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
        //public static List<Author> Authors = new List<Author>()
        //{
        //     new Author(){AuthorId =10,AuthorName="Ali",Location = "Gaza",IsDeleted=false,Books = new List<Book>()},
        //     new Author(){AuthorId =20,AuthorName="Mohammed",Location = "AlBurij",IsDeleted=false,Books = new List<Book>()},
        //      new Author(){AuthorId =30,AuthorName="Mohanad",Location = "Gaza",IsDeleted=false,Books = new List<Book>()},
        //};


        private readonly IMapper mapper;
        private readonly IErrorClass error;
        private readonly IAuthorRepository author;

        public AuthorController(IMapper Mapper, IErrorClass error, IAuthorRepository Author)
        {
            mapper = Mapper;
            this.error = error;
            author = Author;
        }

        [HttpGet(Name = "GetAllAuthors")]
        [Authorize]
        public IActionResult GetAllAuthors([FromQuery] string FilterAuthorName, [FromQuery] string Location,
                                           [FromQuery] string SearchingString,
                                           string orderby,
                                          [FromQuery] PagingDTO paging)
        {

            var Result = author.GetAll(Url, FilterAuthorName, Location, SearchingString, orderby, paging);
            return Ok(Result);
        }

        [HttpGet("{Id}")]
        public IActionResult GetAuthorById(int id)
        {
            string ErrorCode = "";
            var result = author.GetById(id,out ErrorCode);
            if (!String.IsNullOrWhiteSpace(ErrorCode))
            {
                error.LoadError(ErrorCode);
                ModelState.AddModelError(error.ErrorProp, error.ErrorMessage);
                return ValidationProblem();
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult AddAuthor([FromBody] AuthorAddRequestDTO newAuthor)
        {
            string ErrorCode = "";
            var Result = author.AddAuthor(newAuthor, out ErrorCode);
            if (!String.IsNullOrEmpty(ErrorCode))
            {
                error.LoadError(ErrorCode);
                ModelState.AddModelError(error.ErrorProp, error.ErrorMessage);
                return ValidationProblem();
            }

            return CreatedAtAction("GetAuthorById", new { id = Result.AuthorId }, MapDomainToResponse(Result));
        }

        [HttpPut("{AuthorId}")]
        public IActionResult UpdateAuthor(int AuthorId, AuthorUpdateRequestDTO newAuthor)
        {
            string ErrorCode = "";
            author.UpdateAuthor(AuthorId, newAuthor, out ErrorCode);
            if (!String.IsNullOrEmpty(ErrorCode))
            {
                error.LoadError(ErrorCode);
                ModelState.AddModelError(error.ErrorProp, error.ErrorMessage);
                return ValidationProblem();
            }
            return NoContent();//Ok(CurAuthor);
        }

        [HttpPatch("{Authorid}")]
        public IActionResult UpdateAuthorPartialy(int Authorid, JsonPatchDocument AuhtorPatch)
        {
           string ErrorCode = "";
           var CurAuthor = author.UpdateAuthorPartialy( Authorid, AuhtorPatch, out ErrorCode);

            if (!TryValidateModel(CurAuthor))
                return ValidationProblem();

            author.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{AuthorId}")]
        public IActionResult Delete(int AuthorId)
        {
            string ErrorCode = "";
            if (!String.IsNullOrEmpty(ErrorCode))
            {
                error.LoadError(ErrorCode);
                ModelState.AddModelError(error.ErrorProp, error.ErrorMessage);
                return ValidationProblem();
            }
            author.Delete( AuthorId, out ErrorCode);
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
