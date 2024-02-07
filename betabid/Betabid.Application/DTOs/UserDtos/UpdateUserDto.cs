using System.ComponentModel.DataAnnotations;

namespace Betabid.Application.DTOs.UserDtos;

public record UpdateUserDto
{
    [Required]
    public string Id { get; set; } = default!;
    
    public string? NewName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [RegularExpression("^\\S+@\\S+\\.\\S+$", ErrorMessage = "Invalid Email Address")]
    public string? NewEmail { get; set; }
}