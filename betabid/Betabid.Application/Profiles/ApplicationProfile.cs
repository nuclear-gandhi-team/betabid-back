using AutoMapper;
using Betabid.Application.DTOs.BetDtos;
using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Application.Helpers;
using Betabid.Domain.Entities;

namespace Betabid.Application.Profiles;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<Lot, GetLotsDto>()
            .ForMember(dto => dto.Title, opt => opt.MapFrom(lot => lot.Name))
            .ForMember(dto => dto.Image, opt => opt.MapFrom(lot => lot.Pictures.FirstOrDefault()))
            .ForMember(dto => dto.Tags, opt => opt.MapFrom(lot => lot.Tags.Select(t => t.Name).ToList()))
            .ReverseMap();

        CreateMap<AddLotDto, Lot>()
            .ForMember(lot => lot.Tags, opt => opt.Ignore())
            .ForMember(lot => lot.Pictures, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Lot, GetLotDto>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Pictures.Select(p => p.Data)))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)))
            .ForMember(dest => dest.OwnerUsername, opt => opt.MapFrom(src => src.Owner.Name))
            .ForMember(dest => dest.BidHistory, opt => opt.MapFrom(src => src.Bets))
            .ForMember(dest => dest.IsSaved, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CurrentPrice, opt => 
                opt.MapFrom(src => src.Bets.Any() ? src.Bets.Max(b => b.Amount) : src.StartPrice));

        CreateMap<Bet, BetDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time));
    }
}