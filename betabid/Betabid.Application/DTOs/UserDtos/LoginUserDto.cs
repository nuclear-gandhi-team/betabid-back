using System.ComponentModel.DataAnnotations;

namespace Betabid.Application.DTOs.UserDtos;

public record LoginUserDto
{
    [Required]
    public string Login { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}