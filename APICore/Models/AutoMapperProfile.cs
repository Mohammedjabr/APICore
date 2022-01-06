using APICore.Models.Entities;
using APICore.Models.RequestDTO;
using APICore.Models.ResponseDTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICore.Models
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AuthorAddRequestDTO, Author>().
                ForMember(dest=>dest.IsDeleted,
                opt=>opt.MapFrom(src=>false));

            CreateMap<AuthorUpdateRequestDTO, Author>().
                ForMember(dest => dest.IsDeleted,
                opt => opt.MapFrom(src => false));

            CreateMap<Author, AuthorResponseDTO>().
                ForMember(x => x.BookCount, opt => opt.MapFrom(s => s.Books.Count+1));

            CreateMap<UserAddDTO, Users>().
                 ForMember(x=>x.UserId, op=>op.MapFrom(s => Guid.NewGuid()));

        }
    }
}
