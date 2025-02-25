using AutoMapper;
using UserProfile.Models.db;
using UserProfile.Models.Items;
using UserProfile.Models.Levels;

namespace UserProfile.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Item, ItemDTO>().ReverseMap();
        CreateMap<Level, LevelDTO>().ReverseMap();
    }
}