using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Security.Cryptography;
using API.DTOs;
using API.Entities;
using API.Extensions;

using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {

            CreateMap<AppUser, MemberDtos>()
            .ForMember(dest => dest.photoUrl, opt => opt.MapFrom(src => 
            src.Photos.FirstOrDefault(x => x.IsMain).Url))
             .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateofBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();

            CreateMap<MemberUpdateDto, AppUser>();

            CreateMap<RegisterDto, AppUser>();
        }
    }
}