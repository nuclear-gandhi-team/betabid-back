using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Application.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace betabid.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotsController : ControllerBase
{
    private readonly ILotService _lotService;
    private readonly IValidator<AddLotDto> _lotValidator;

    public LotsController(ILotService lotService, IValidator<AddLotDto> lotValidator)
    {
        _lotService = lotService;
        _lotValidator = lotValidator;
    }
    
    [HttpPost ("create")]
    public async Task<IActionResult> CreateNewLotAsync([FromForm] AddLotDto newLot)
    {
        var validationResult = await _lotValidator.ValidateAsync(newLot);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var createdLot = await _lotService.CreateNewLotAsync(newLot);
        return Ok(createdLot);
    }
    
    [HttpDelete ("delete/{id}")]
    public async Task<IActionResult> DeleteLotAsync(int id)
    {
        await _lotService.DeleteLotAsync(id);
        return Ok();
    }
    
    [HttpGet ("get/{id}")]
    public async Task<IActionResult> GetLotByIdAsync(int id)
    {
        var lot = await _lotService.GetLotByIdAsync(id);
        return Ok(lot);
    }
    
    [HttpGet ("getall")]
    public async Task<IActionResult> GetAllLotsAsync(string userId)
    {
        var lots = await _lotService.GetAllLotsAsync(userId);
        return Ok(lots);
    }
    
    [HttpPost ("save/{lotId}")]
    public async Task<IActionResult> SaveLotAsync(int lotId, string userId)
    {
        var isSaved = await _lotService.SaveLotAsync(lotId, userId);
        return Ok(isSaved);
    }
}