using AutoMapper;
using Betabid.Domain.Entities;
using Betabid.Features.UserFeatures;

namespace Betabid.Features.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, GetFullUserDto>()
            .ForMember(dest => dest.Login,
            opt => opt.MapFrom(src => src.Name))
            .ReverseMap();
        CreateMap<User, RegisterUserDto>()
            .ForMember(dest => dest.Login,
                opt => opt.MapFrom(src => src.Name))
            .ReverseMap();
    }
}