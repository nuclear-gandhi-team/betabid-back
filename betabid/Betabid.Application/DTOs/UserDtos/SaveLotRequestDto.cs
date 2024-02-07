using System.ComponentModel.DataAnnotations;

namespace Betabid.Application.DTOs.UserDtos;

public class SaveLotRequestDto
{
    [Required]
    public string UserId { get; set; } = default!;
    
    [Required]
    public int LotId { get; set; }
}