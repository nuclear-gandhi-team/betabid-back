using System.ComponentModel.DataAnnotations;

namespace Betabid.Application.DTOs.UserDtos;

public record RegisterUserDto
{
    [Required]
    public string Login { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
    
    [Required]
    public string Password { get; set; } = default!;

    [Required]
    public string ConfirmPassword { get; set; } = default!;
}