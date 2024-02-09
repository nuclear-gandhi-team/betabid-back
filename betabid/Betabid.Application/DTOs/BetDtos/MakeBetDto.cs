using System.ComponentModel.DataAnnotations;

namespace Betabid.Application.DTOs.BetDtos;

public record MakeBetDto
{
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public int LotId { get; set; }
}