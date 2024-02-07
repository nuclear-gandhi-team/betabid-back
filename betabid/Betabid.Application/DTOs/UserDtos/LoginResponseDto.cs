using System.ComponentModel.DataAnnotations;

namespace Betabid.Application.DTOs.UserDtos;

public record LoginResponseDto
{
    [Required]
    public string Token { get; set; } = default!;
}