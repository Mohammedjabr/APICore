using APICore.Data;
using APICore.Models.RequestDTO;
using APICore.Models.ResponseDTO;
using APICore.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using APICore.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace APICore.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public AuthorRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }
        public PagedResponse<AuthorResponseDTO> GetAll(IUrlHelper Url, string FilterAuthorName, string Location, string SearchingString, string orderby, PagingDTO paging)
        {
            var AuthorQuery = _context.Authors.AsQueryable();
            AuthorQuery = AuthorQuery.Where(m => m.IsDeleted == false);

            if (!string.IsNullOrWhiteSpace(SearchingString))
            {
                AuthorQuery = AuthorQuery.Where(m => m.AuthorName.Equals(SearchingString) ||
                                                m.Location.Contains(SearchingString, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(orderby))
            {
                AuthorQuery = AuthorQuery.OrderBy(orderby);
            }
            else
            {
                AuthorQuery = AuthorQuery.OrderBy(x => x.AuthorName);
            }

            var ResponseDTO = AuthorQuery.Select(m => mapper.Map<AuthorResponseDTO>(m));
            var pagedResponse = new PagedResponse<AuthorResponseDTO>(ResponseDTO, paging);

            if (pagedResponse.Paging.HasNextPage)
            {
                pagedResponse.Paging.NextPageURL = Url.Link("GetAllAuthors", new
                {
                    SearchingString,
                    orderby,
                    paging.RowCount,
                    PageNumber = paging.PageNumber + 1
                });
            }

            if (pagedResponse.Paging.HasPrevPage)
            {
                pagedResponse.Paging.PrevPageURL = Url.Link("GetAllAuthors", new
                {
                    SearchingString,
                    orderby,
                    paging.RowCount,
                    PageNumber = paging.PageNumber - 1
                });
            }
            return (pagedResponse);
        }

        public AuthorResponseDTO GetById(int id, out string ErrorCode)
        {
            ErrorCode = "";
            var CurAuthor = _context.Authors.Where(x => x.AuthorId == id && x.IsDeleted == false).FirstOrDefault();
            if (CurAuthor == null)
            {
                ErrorCode = "Ath001";
                return null;
            }
            return mapper.Map<AuthorResponseDTO>(CurAuthor);
        }

        public Author AddAuthor(AuthorAddRequestDTO newAuthor, out string ErrorCode)
        {
            ErrorCode = "";
            //check Name duplication
            if (_context.Authors.Any(x => x.AuthorName == newAuthor.AuthorName))
            {
                ErrorCode = "Ath002";
                return null;
            }
            var CurAuthor = mapper.Map<Author>(newAuthor);
            //  CurAuthor.AuthorId = _context.Authors.Max(x => x.AuthorId) + 1;
            _context.Authors.Add(CurAuthor);
            SaveChanges();
            return CurAuthor;
           
            
        }

        public void UpdateAuthor(int AuthorId, AuthorUpdateRequestDTO newAuthor, out string ErrorCode)
        {
            ErrorCode = "";
            if (String.IsNullOrWhiteSpace(newAuthor.AuthorName))
            {
                ErrorCode = "Ath003";
                return;
            }

            var CurAuthor = _context.Authors.Where(m => m.AuthorId == AuthorId).SingleOrDefault();
            if (CurAuthor == null)
            {
                ErrorCode = "Ath001";
                return;
            }

            //Mapping
            mapper.Map(newAuthor, CurAuthor);
          SaveChanges();
        }

        public Author UpdateAuthorPartialy(int Authorid, JsonPatchDocument AuhtorPatch, out string ErrorCode)
        {
            ErrorCode = "";
            var CurAuthor = _context.Authors.Where(m => m.AuthorId == Authorid).SingleOrDefault();
            if (CurAuthor == null)
            {
                ErrorCode = "Ath001";
                return null;
            }

            AuhtorPatch.ApplyTo(CurAuthor);
            return CurAuthor;
        }

        public void Delete(int AuthorId,out string ErrorCode)
        {
            ErrorCode = "";
            var CurAuthor = _context.Authors.Where(m => m.AuthorId == AuthorId).SingleOrDefault();
            if (CurAuthor == null)
            {
                ErrorCode = "Ath001";
                return;
            }
           
            CurAuthor.IsDeleted = true;
            _context.SaveChanges();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

    }
}
