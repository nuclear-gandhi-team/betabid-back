using System.Security.Authentication;
using AutoMapper;
using Betabid.Application.DTOs.FilteringDto;
using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Application.DTOs.StatusesDtos;
using Betabid.Application.DTOs.TagsDtos;
using Betabid.Application.Exceptions;
using Betabid.Application.Filtering;
using Betabid.Application.Filtering.Lots;
using Betabid.Application.Helpers;
using Betabid.Application.Interfaces.Repositories;
using Betabid.Application.Services.Interfaces;
using Betabid.Domain.Entities;
using Betabid.Domain.Enums;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Application.Services.Implementations;

public class LotService : ILotService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITimeProvider _timeProvider;
    private readonly UserManager<User> _userManager;

    public LotService(IUnitOfWork unitOfWork, IMapper mapper, ITimeProvider timeProvider, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
        _userManager = userManager;
    }

    public async Task<AddLotDto> CreateNewLotAsync(AddLotDto newLot, IList<IFormFile> pictures)
    {
        ValidateAddingNewLot(newLot, pictures);
        var lot = _mapper.Map<Lot>(newLot);
        
        await HandleTagsAsync(newLot, lot);
        
        lot.Pictures = await HandlePicturesAsync(pictures, lot);

        await _unitOfWork.Lots.AddAsync(lot);
        await _unitOfWork.CommitAsync();
        
        var lotDto = _mapper.Map<AddLotDto>(lot);

        return lotDto;
        
    }

    public async Task<LotsWithPagination> GetAllLotsAsync(FilteringOptionsDto filteringOptionsDto, string? userId)
    {
        var predicate = PredicateBuilder.New<Lot>(true);
        var specifications = new List<IFilteringCriteria<Lot>>();
        
        if (filteringOptionsDto.Tags != null && filteringOptionsDto.Tags.Any())
        {
            specifications.Add(new TagsFilteringCriteria(filteringOptionsDto.Tags));
        }
        if (filteringOptionsDto.Status != null)
        {
            specifications.Add(new LotStatusFilteringCriteria(filteringOptionsDto.Status.Value, _timeProvider));
        }
        if (filteringOptionsDto.NameStartsWith != null)
        {
            specifications.Add(new LotNameFilteringCriteria(filteringOptionsDto.NameStartsWith));
        }
        predicate = specifications.Aggregate(predicate, (current, specification) => current.And(specification.Criteria));

        var (filteredLots, totalCount) = await _unitOfWork.Lots.GetAllFilteredAsync(predicate, filteringOptionsDto);

        if (!filteredLots.Any())
        {
            return new LotsWithPagination();
        }
    
        var lotsDto = _mapper.Map<IEnumerable<GetLotCardDto>>(filteredLots);

        foreach (var lotDto in lotsDto)
        {
            var lot = filteredLots.FirstOrDefault(lot => lot.Id == lotDto.Id);
            if (lot == null)
            {
                continue;
            }

            if (userId is not null)
            {
                lotDto.IsSaved = await _unitOfWork.Lots.IsLotSavedByUserAsync(lotDto.Id, userId);
            }
            else
            {
                lotDto.IsSaved = false;
            }

            lotDto.Status = new StatusHelper(_timeProvider).GetStatus(lot); 

            var tags = await _unitOfWork.Lots.GetByIdWithTagsAsync(lotDto.Id);
            lotDto.Tags = tags?.Tags?.Select(t => t.Name).ToList() ?? new List<string> { "Other" };

            var picture = await _unitOfWork.Pictures.GetPictureByLotIdAsync(lotDto.Id);
            lotDto.Image = picture != null ? Convert.ToBase64String(picture.Data) : null;
        }
    
        return new LotsWithPagination
        {
            Lots = lotsDto,
            TotalPages = totalCount,
            CurrentPage = filteringOptionsDto.Page ?? 1
        };
    }

    public async Task<GetLotDto> GetLotByIdAsync(int id, string? userId)
    {
        var lot = await _unitOfWork.Lots.GetByIdAsync(id);
        if (lot == null)
        {
            throw new EntityNotFoundException($"Lot with ID {id} not found");
        }

        var tags = await _unitOfWork.Lots.GetByIdWithTagsAsync(id);
        lot.Tags = tags.Tags;
        var lotDto = _mapper.Map<GetLotDto>(lot) ?? throw new NullReferenceException("Error mapping.");
        lotDto.Status = new StatusHelper(_timeProvider).GetStatus(lot); 
        
        var pictures = await _unitOfWork.Pictures.GetPicturesByLotIdAsync(id);
        
        lotDto.Images = pictures.Select(pic => 
                Convert.ToBase64String(pic.Data ?? throw new NullReferenceException("Image is null.")))
                .ToList();
        if (userId is not null)
        {
            lotDto.IsSaved = await _unitOfWork.Lots.IsLotSavedByUserAsync(id, userId);
        }
        else
        {
            lotDto.IsSaved = false;
        }
        
        if (lot.Bets != null && lot.Bets.Any())
        {
            lotDto.CurrentPrice = lot.Bets.Max(b => b.Amount);  
        }
        return lotDto;
    }

    public async Task DeleteLotAsync(int id, string userId)
    {
        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId) is null)
        {
            throw new EntityNotFoundException($"No user with Id {userId}.");
        }

        var lot = await _unitOfWork.Lots.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"Lot with ID {id} not found.");

        if (lot.OwnerId != userId)
        {
            throw new AuthenticationException("You can't delete someone else's lot.");
        }
        
        if (lot.DateStarted < _timeProvider.Now)
        {
            throw new LotAlreadyStartedException("You cannot delete a lot that has already started.");
        }
        var hasBids = await _unitOfWork.Bets.AnyAsync(b => b.LotId == id);
        if (hasBids)
        {
            throw new LotHasBidsException("Cannot delete a lot that has bids.");
        }

        await _unitOfWork.Lots.DeleteAsync(lot);
        await _unitOfWork.CommitAsync();
    }

    public async Task<IList<GetTagDto>> GetAllTagsAsync()
    {
        var tags = await _unitOfWork.Tags.GetAllAsync();

        return _mapper.Map<IList<GetTagDto>>(tags)
            ?? new List<GetTagDto>();
    }

    public IList<GetStatusDto> GetAllStatuses()
    {
        return new List<GetStatusDto>
        {
            new() { Name = LotStatus.Preparing.ToString() },
            new() { Name = LotStatus.Open.ToString() },
            new() { Name = LotStatus.Finished.ToString() },
        };
    }

    private async Task HandleTagsAsync(AddLotDto newLot, Lot lot)
    {
        if (!newLot.TagIds.Any())
        {
            return;
        }
        lot.Tags = new List<Tag>();
        foreach (var tagId in newLot.TagIds)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(tagId);
            if (tag != null)
            {
                lot.Tags.Add(tag);
            }
            else throw new EntityNotFoundException($"Tag with ID {tagId} not found");
        }
    }

    private async Task<List<Picture>> HandlePicturesAsync(IList<IFormFile> pictures, Lot lot)
    {
        var picturesList = new List<Picture>();

        foreach (var formFile in pictures)
        {
            if (formFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await formFile.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                Picture picture = new Picture
                {
                    Data = fileBytes,
                    Lot = lot,
                    LotId = lot.Id
                };

                picturesList.Add(picture);
            }
        }

        return picturesList;
    }
    
    private static void ValidateAddingNewLot(AddLotDto newLot, ICollection<IFormFile> pictures)
    {
        if(pictures.Count == 0)
        {
            throw new ArgumentException("You must add at least one picture");
        }
    
        if (newLot == null)
        {
            throw new ArgumentException("new Lot cannot be null", nameof(newLot));
        }
    }
}