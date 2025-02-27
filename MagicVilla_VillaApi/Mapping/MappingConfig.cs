using AutoMapper;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.DTOs;

namespace MagicVilla_VillaApi.Mapping;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Villa, VillaDto>().ReverseMap();
        CreateMap<Villa, CreateVillaDto>().ReverseMap();
        CreateMap<Villa, UpdateVillaDto>().ReverseMap();
        
    }
}