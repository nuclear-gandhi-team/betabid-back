using AutoMapper;
using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Application.Helpers;
using Betabid.Application.Interfaces.Repositories;
using Betabid.Application.Services.Interfaces;
using Betabid.Domain.Entities;

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

    public async Task<AddLotDto> CreateNewLotAsync(AddLotDto newLot)
    {
        if (newLot == null)
        {
            throw new ArgumentException("new Lot cannot be null", nameof(newLot));
        }
        if (newLot.TagIds.Count > 2)
        {
            throw new Exception("You can only add 2 tags to a lot");
        }
        var lot = _mapper.Map<Lot>(newLot);
        
        if (newLot.TagIds.Any())
        {
            lot!.Tags = new List<Tag>();
        }
        if( newLot.Pictures.Any())
        {
            lot!.Pictures = new List<Picture>();
        }
        
        foreach (var tagId in newLot.TagIds)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(tagId);
            if (tag != null)
            {
                lot.Tags.Add(tag);
            }
            else throw new Exception($"Tag with ID {tagId} not found");
        }
        //Todo: PICTURES???
        if (newLot.Pictures.Any())
        {
            foreach (var formFile in newLot.Pictures)
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
                    lot.Pictures.Add(picture);
                }
            }
        }

        await _unitOfWork.Lots.AddAsync(lot);
        await _unitOfWork.CommitAsync();
        var lotDto = _mapper.Map<AddLotDto>(lot);

        return lotDto;
        
    }

    public async Task<IEnumerable<GetLotsDto>> GetAllLotsAsync(string userId)
    {
        var lots = await _unitOfWork.Lots.GetAllAsync();

        if (!lots.Any())
        {
            return new List<GetLotsDto>();
        }
        var lotsDto = _mapper.Map<IEnumerable<GetLotsDto>>(lots);

        var lotsWithDto = lots.Zip(lotsDto, (lot, dto) => new { Lot = lot, Dto = dto });

        foreach (var item in lotsWithDto)
        {
            item.Dto.IsSaved = await _unitOfWork.Lots.IsLotSavedByUserAsync(item.Dto.Id, userId);
            var tags = await _unitOfWork.Lots.GetByIdWithTagsAsync(item.Dto.Id);
            item.Dto.Tags = tags.Tags.Select(t => t.Name).ToList();
            item.Dto.Status = GetStatus(item.Lot); 
        }

        return lotsDto;
    }

    public async Task<GetLotDto> GetLotByIdAsync(int id)
    {
        var lot = await _unitOfWork.Lots.GetByIdAsync(id);
        if (lot == null)
        {
            throw new Exception($"Lot with ID {id} not found");
        }

        var tags = await _unitOfWork.Lots.GetByIdWithTagsAsync(id);
        lot.Tags = tags.Tags;
        var lotDto = _mapper.Map<GetLotDto>(lot);
        lotDto.Status = GetStatus(lot); 
        lotDto.IsSaved = await _unitOfWork.Lots.IsLotSavedByUserAsync(id, lot.OwnerId);
        if (lot.Bets != null && lot.Bets.Any())
        {
            lotDto.CurrentPrice = lot.Bets.Max(b => b.Amount);  
        }
        // Todo : check bids
        return lotDto;
    }

    public async Task DeleteLotAsync(int id)
    {
        var lot = await _unitOfWork.Lots.GetByIdAsync(id);
        if (lot == null)
        {
            throw new Exception($"Lot with ID {id} not found");
        }
        if (lot.DateStarted < _timeProvider.Now)
        {
            throw new Exception("You cannot delete a lot that has already started");
        }
        var hasBids = await _unitOfWork.Bets.AnyAsync(b => b.LotId == id);
        if (hasBids)
        {
            throw new Exception("Cannot delete a lot that has bids.");
        }

        await _unitOfWork.Lots.DeleteAsync(lot);
        await _unitOfWork.CommitAsync();
    }

    public async Task<bool> SaveLotAsync(int lotId, string userId)
    {
        var lot = await _unitOfWork.Lots.GetByIdAsync(lotId);
        if (lot == null)
        {
            throw new Exception($"Lot with ID {lotId} not found");
        }
        var isAlreadySaved = await _unitOfWork.Lots.IsLotSavedByUserAsync(lotId, userId);

        if(isAlreadySaved)
            throw new Exception("The lot is already saved by the user.");

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
    
}