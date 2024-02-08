using AutoMapper;
using Betabid.Application.DTOs.FilteringDto;
using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Application.Exceptions;
using Betabid.Application.Filtering;
using Betabid.Application.Filtering.Lots;
using Betabid.Application.Helpers;
using Betabid.Application.Interfaces.Repositories;
using Betabid.Application.Services.Interfaces;
using Betabid.Domain.Entities;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Betabid.Application.Services.Implementations;

public class LotService : ILotService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITimeProvider _timeProvider;
    
    public LotService(IUnitOfWork unitOfWork, IMapper mapper, ITimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
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

    public async Task<LotsWithPagination> GetAllLotsAsync(FilteringOptionsDto filteringOptionsDto, string userId)
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
    
        var lotsDto = _mapper.Map<IEnumerable<GetLotsDto>>(filteredLots);

        foreach (var lotDto in lotsDto)
        {
            var lot = filteredLots.FirstOrDefault(lot => lot.Id == lotDto.Id);
            if(lot == null)
            {
                continue;
            }

            lotDto.IsSaved = await _unitOfWork.Lots.IsLotSavedByUserAsync(lotDto.Id, userId);

            lotDto.Status = GetStatus(lot);

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

    public async Task<GetLotDto> GetLotByIdAsync(int id, string userId)
    {
        var lot = await _unitOfWork.Lots.GetByIdAsync(id);
        if (lot == null)
        {
            throw new EntityNotFoundException($"Lot with ID {id} not found");
        }

        var tags = await _unitOfWork.Lots.GetByIdWithTagsAsync(id);
        lot.Tags = tags.Tags;
        var lotDto = _mapper.Map<GetLotDto>(lot) ?? throw new NullReferenceException("Error mapping.");
        lotDto.Status = GetStatus(lot); 
        
        var pictures = await _unitOfWork.Pictures.GetPicturesByLotIdAsync(id);
        
        lotDto.Images = pictures.Select(pic => 
                Convert.ToBase64String(pic.Data ?? throw new NullReferenceException("Image is null.")))
                .ToList();
        
        lotDto.IsSaved = await _unitOfWork.Lots.IsLotSavedByUserAsync(id, userId);
        
        if (lot.Bets != null && lot.Bets.Any())
        {
            lotDto.CurrentPrice = lot.Bets.Max(b => b.Amount);  
        }
        return lotDto;
    }

    public async Task DeleteLotAsync(int id)
    {
        var lot = await _unitOfWork.Lots.GetByIdAsync(id);
        if (lot == null)
        {
            throw new EntityNotFoundException($"Lot with ID {id} not found");
        }
        if (lot.DateStarted < _timeProvider.Now)
        {
            throw new LotAlreadyStartedException("You cannot delete a lot that has already started");
        }
        var hasBids = await _unitOfWork.Bets.AnyAsync(b => b.LotId == id);
        if (hasBids)
        {
            throw new LotHasBidsException("Cannot delete a lot that has bids.");
        }

        await _unitOfWork.Lots.DeleteAsync(lot);
        await _unitOfWork.CommitAsync();
    }

    public async Task<bool> SaveLotAsync(int lotId, string userId)
    {
        var lot = await _unitOfWork.Lots.GetByIdAsync(lotId);
        if (lot == null)
        {
            throw new EntityNotFoundException($"Lot with ID {lotId} not found");
        }
        var isAlreadySaved = await _unitOfWork.Lots.IsLotSavedByUserAsync(lotId, userId);

        if(isAlreadySaved)
            throw new LotAlreadySavedException();

        var saved = new Saved
        {
            UserId = userId,
            LotId = lotId,
        };

        await _unitOfWork.Saved.AddAsync(saved);
        await _unitOfWork.CommitAsync();
        return true;
    }

    private string GetStatus(Lot lot)
    {
        if (_timeProvider.Now < lot.DateStarted)
        {
            return "Preparing";
        }
        if (_timeProvider.Now >= lot.DateStarted && _timeProvider.Now < lot.Deadline)
        {
            return "Active";
        }
        return "Ended";
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