using AutoMapper;
using GeoApp.Application.Dtos;
using GeoApp.Domain.Entities;

namespace GeoApp.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
