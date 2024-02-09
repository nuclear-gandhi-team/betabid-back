using Betabid.Application.DTOs.FilteringDto;
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
    public async Task<IActionResult> CreateNewLotAsync([FromForm] AddLotDto newLotDto, [FromForm] IList<IFormFile> pictures)
    {
        var validationResult = await _lotValidator.ValidateAsync(newLotDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var createdLot = await _lotService.CreateNewLotAsync(newLotDto, pictures);
        return Ok(createdLot);
    }
    
    [HttpDelete ("delete/{id}")]
    public async Task<IActionResult> DeleteLotAsync(int id)
    {
        await _lotService.DeleteLotAsync(id);
        return Ok();
    }
    
    [HttpGet ("get/{id:int}/{userId}")]
    public async Task<IActionResult> GetLotByIdAsync(int id, string userId)
    {
        var lot = await _lotService.GetLotByIdAsync(id, userId);
        return Ok(lot);
    }
    
    //Authorize
    [HttpGet ("getall")]
    public async Task<IActionResult> GetAllLotsAsync([FromQuery] FilteringOptionsDto filterOptions, string userId)
    {
        var lots = await _lotService.GetAllLotsAsync(filterOptions, userId);
        return Ok(lots);
    }
}