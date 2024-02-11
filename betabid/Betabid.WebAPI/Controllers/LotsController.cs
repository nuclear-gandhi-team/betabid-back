using Betabid.Application.DTOs.FilteringDto;
using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Application.Services.Interfaces;
using betabid.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
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
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        
        var validationResult = await _lotValidator.ValidateAsync(newLotDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors + newLotDto.Name);
        }
        var createdLot = await _lotService.CreateNewLotAsync(newLotDto, pictures, userId);
        return Ok(createdLot);
    }
    
    [HttpDelete ("delete/{id:int}")]
    public async Task<IActionResult> DeleteLotAsync(int id)
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }

        await _lotService.DeleteLotAsync(id, userId);
        return Ok();
    }
    
    [HttpGet ("get/{id:int}")]
    public async Task<IActionResult> GetLotByIdAsync(int id)
    {
        var lot = await _lotService.GetLotByIdAsync(id, await this.GetUserIdFromJwtAsync());
        return Ok(lot);
    }

    [HttpGet ("get-all")]
    public async Task<IActionResult> GetAllLotsAsync([FromQuery] FilteringOptionsDto filterOptions)
    {
        var lots = await _lotService.GetAllLotsAsync(filterOptions, await this.GetUserIdFromJwtAsync());
        return Ok(lots);
    }

    [HttpGet("tags")]
    public async Task<IActionResult> GetAllTagsAsync()
    {
        var tags = await _lotService.GetAllTagsAsync();

        return Ok(tags);
    }

    [HttpGet("statuses")]
    public IActionResult GetAllStatuses()
    {
        var statuses = _lotService.GetAllStatuses();

        return Ok(statuses);
    }
}