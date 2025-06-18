using AutoMapper;
using GeoApp.Domain.Entities;
using GeoApp.Infrastructure.Entities;

namespace GeoApp.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<AppUser, User>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName));

            CreateMap<User, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.Ignore()) // Email bizden alınacak
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Identity kendisi yapacak
        }
    }
}
