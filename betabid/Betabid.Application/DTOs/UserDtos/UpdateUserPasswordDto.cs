using System.ComponentModel.DataAnnotations;

namespace Betabid.Application.DTOs.UserDtos;

public record UpdateUserPasswordDto
{
    [Required]
    public string Id { get; set; } = default!;
    
    [Required]
    public string OldPassword { get; set; } = default!;
    
    [Required]
    public string NewPassword { get; set; } = default!;
    
    [Required]
    public string ConfirmNewPassword { get; set; } = default!;
}