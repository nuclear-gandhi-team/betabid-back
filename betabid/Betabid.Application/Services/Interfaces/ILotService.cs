using Betabid.Application.DTOs.FilteringDto;
using Betabid.Application.DTOs.LotsDTOs;
using Microsoft.AspNetCore.Http;

namespace Betabid.Application.Services.Interfaces;

public interface ILotService
{
    Task<AddLotDto> CreateNewLotAsync(AddLotDto newLot, IList<IFormFile> pictures);
    
    Task<LotsWithPagination> GetAllLotsAsync(FilteringOptionsDto filteringOptionsDto, string? userId);
    
    Task<GetLotDto> GetLotByIdAsync(int id, string? userId);
    
    Task DeleteLotAsync(int id, string userId);
}