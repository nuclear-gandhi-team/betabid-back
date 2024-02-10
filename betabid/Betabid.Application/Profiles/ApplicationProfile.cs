using AutoMapper;
using Betabid.Application.DTOs.BetDtos;
using Betabid.Application.DTOs.CommentDtos;
using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Application.DTOs.TagsDtos;
using Betabid.Application.DTOs.UserDtos;
using Betabid.Application.Helpers;
using Betabid.Domain.Entities;

namespace Betabid.Application.Profiles;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<Lot, GetLotCardDto>()
            .ForMember(dto => dto.Title, opt => opt.MapFrom(lot => lot.Name))
            .ForMember(dto => dto.Tags, opt => opt.MapFrom(lot => lot.Tags.Select(t => t.Name).ToList()))
            .ForMember(dest=>dest.CurrentPrice, opt => opt.MapFrom(src => src.Bets.Any() ? src.Bets.Max(b => b.Amount) : src.StartPrice))
            .ReverseMap();

        CreateMap<AddLotDto, Lot>()
            .ForMember(lot => lot.Tags, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Lot, GetLotDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.Name))
            .ForMember(dest => dest.BidHistory, opt => opt.MapFrom(src => src.Bets))
            .ForMember(dest => dest.IsSaved, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CurrentPrice, opt =>
                opt.MapFrom(src => src.Bets.Any() ? src.Bets.Max(b => b.Amount) : src.StartPrice))
            .ForMember(dest => dest.MinBetStep, opt => opt.MapFrom(src => src.BetStep))
            .ForMember(dest=>dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest=>dest.MinNextPrice, opt => opt.MapFrom(src => src.Bets.Any() ? src.Bets.Max(b => b.Amount) + src.BetStep : src.StartPrice + src.BetStep))
            .ForMember(dest=>dest.ActiveBetsCount, opt => opt.MapFrom(src => src.Bets.Count))
            .ForMember(dest=>dest.ActiveUsersCount, opt => opt.MapFrom(src => src.Bets.Select(b => b.UserId).Distinct().Count()))
            .ReverseMap();

        CreateMap<Bet, GetBetDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest=> dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));
        
        CreateMap<User, GetFullUserDto>()
            .ForMember(dest => dest.Login,
                opt => opt.MapFrom(src => src.Name))
            .ReverseMap();
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.Name,
                opt =>opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.Login))
            .ReverseMap();

        CreateMap<Tag, GetTagDto>().ReverseMap();
        
        CreateMap<Comment, GetCommentDto>()
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.ChildComments,
                opt => opt.MapFrom(src => src.SubComments))
            .ReverseMap();
    }
}