using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Betabid.Application.Services.Interfaces;

public interface ILotService
{
    Task<AddLotDto> CreateNewLotAsync(AddLotDto newLot, IList<IFormFile> pictures);
    
    Task<IEnumerable<GetLotsDto>> GetAllLotsAsync(string userId);
    
    Task<GetLotDto> GetLotByIdAsync(int id);
    
    Task DeleteLotAsync(int id);
    
    Task<bool> SaveLotAsync(int lotId, string userId);
}