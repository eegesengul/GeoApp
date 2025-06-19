using AutoMapper;
using GeoApp.Application.Dtos;
using GeoApp.Domain.Entities;

namespace GeoApp.Application.Mappings
{
    public class AreaMappingProfile : Profile
    {
        public AreaMappingProfile()
        {
            CreateMap<Area, AreaDto>();
            CreateMap<AreaDto, Area>();
        }
    }
}