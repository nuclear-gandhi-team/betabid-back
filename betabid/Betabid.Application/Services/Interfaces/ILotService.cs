using Betabid.Application.DTOs.FilteringDto;
using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Application.DTOs.StatusesDtos;
using Betabid.Application.DTOs.TagsDtos;
using Microsoft.AspNetCore.Http;

namespace Betabid.Application.Services.Interfaces;

public interface ILotService
{
    Task<AddLotResponseDto> CreateNewLotAsync(AddLotDto newLot, IList<IFormFile> pictures, string userId);
    
    Task<LotsWithPagination> GetAllLotsAsync(FilteringOptionsDto filteringOptionsDto, string? userId);
    
    Task<GetLotDto> GetLotByIdAsync(int id, string? userId);
    
    Task DeleteLotAsync(int id, string userId);

    Task<IList<GetTagDto>> GetAllTagsAsync();
    
    IList<GetStatusDto> GetAllStatuses();
}