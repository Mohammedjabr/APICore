using APICore.Models;
using APICore.Models.RequestDTO;
using APICore.Models.ResponseDTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        PagedResponse<AuthorResponseDTO> GetAll(IUrlHelper Url, string FilterAuthorName, string Location, string SearchingString 
            , string orderby, Models.RequestDTO.PagingDTO paging);
        AuthorResponseDTO GetById(int id, out string ErrorCode);
        Author AddAuthor( AuthorAddRequestDTO newAuthor, out string ErrorCode);
        int SaveChanges();
        void UpdateAuthor(int AuthorId, AuthorUpdateRequestDTO newAuthor, out string ErrorCode);
        Author UpdateAuthorPartialy(int Authorid, JsonPatchDocument AuhtorPatch, out string ErrorCode);
        void Delete(int AuthorId, out string ErrorCode);
    }
}
